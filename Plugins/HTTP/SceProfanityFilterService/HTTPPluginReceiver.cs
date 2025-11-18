using ApacheNet;
using ApacheNet.Models;
using ApacheNet.PluginManager;
using CustomLogger;
using MultiServerLibrary.Extension;
using NetHasher;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WatsonWebserver.Core;

namespace SonyCdnReroute
{
    public class HTTPPluginReceiver : HTTPPlugin
    {
        private static readonly byte[] pfKey = new byte[16];
        private static readonly byte[] pfIV = new byte[16];

        private const string pfManifestRegex = @"^/manifest/(?<locale>[a-z]{2}_[A-Z]{2})/(?<uuid>[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})$";
        private const string pfFileRegex = @"/(?<uuid>[0-9a-fA-F\-]{36})/(?<timestamp>\d{14})/(?<filename>[^/]+)";

        private static string pfApiPath;
        private static byte[] _privateKey;

        Task HTTPPlugin.HTTPStartPlugin(string param, ushort port)
        {
            string? privKeyStr = null;
            pfApiPath = param + "/SceProfanityFilterService/";
            Directory.CreateDirectory(pfApiPath);
            string privKeyPath = pfApiPath + "privateKey.json";
            if (File.Exists(privKeyPath))
                privKeyStr = JsonSerializer.Deserialize<string>(File.ReadAllText(privKeyPath));
            if (string.IsNullOrEmpty(privKeyStr))
                _privateKey = StringUtils.IsBase64("NVluu9dWima10JIUKhCVvg==").Item2;
            else
            {
                var isBase64 = StringUtils.IsBase64(privKeyStr);
                if (isBase64.Item1)
                    _privateKey = isBase64.Item2;
                else
                {
                    LoggerAccessor.LogError($"[SceProfanityFilterService] - The private key at path: {privKeyPath} is not a base64 string, falling back to default key...");
                    _privateKey = StringUtils.IsBase64("NVluu9dWima10JIUKhCVvg==").Item2;
                }
            }
            return Task.CompletedTask;
        }

        public async Task<object?> ProcessPluginMessageAsync(object obj)
        {
            if (obj is ApacheContext ctx)
            {
                HttpRequestBase request = ctx.Request;
                HttpResponseBase response = ctx.Response;

                if (ctx.GetHost() == "update-prod.pfs.online.scee.com")
                {
                    string absolutePath = ctx.AbsolutePath;

                    Match match = Regex.Match(absolutePath, pfManifestRegex);

                    if (match.Success)
                    {
                        string locale = match.Groups["locale"].Value.Replace("_", "-");
                        string uuid = match.Groups["uuid"].Value;
                        string pfEbinPath = pfApiPath + $"{uuid}/Filter-{locale}.ebin";
                        string expectedHash = DotNetHasher.ComputeSHA256String(Encoding.UTF8.GetBytes($"Everything in moderation, including moderation.{uuid}"), _privateKey);

                        if (request.HeaderExists("ps4-auth") && expectedHash == request.RetrieveHeaderValue("ps4-auth"))
                        {
                            if (File.Exists(pfEbinPath))
                            {
                                // Note: the ebin file has a big endian timestamp at the end, it must match the file creation date for the client to accept it.
                                DateTime timestamp = new FileInfo(pfEbinPath).CreationTimeUtc;
                                string timestampStr = timestamp.ToString("yyyyMMddHHmmss");
                                byte[] iv = new byte[16];
                                string message = Convert.ToBase64String(EncryptAES(Encoding.UTF8.GetBytes($@"{{
                                      ""uri"": ""http://update-prod.pfs.online.scee.com/{uuid}/{timestampStr}/Filter-{locale}.ebin"",
                                      ""version"": ""{timestampStr}"",
                                      ""iv"": ""{Convert.ToBase64String(pfIV)}"",
                                      ""key"": ""{Convert.ToBase64String(pfKey)}""
                                    }}"), _privateKey, iv));
                                ctx.StatusCode = HttpStatusCode.OK;
                                ctx.Response.ContentType = "application/json";
                                string payload = @$"{{""rmac"":""{DotNetHasher.ComputeSHA256String(Encoding.UTF8.GetBytes(message), _privateKey).ToLower()}"",
                                ""message"":""{message}"",""iv"":""{Convert.ToBase64String(iv)}"",""timestamp"":
                                {(long)(timestamp - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).UtcDateTime).TotalMilliseconds}}}";
                                return await ctx.SendImmediate(payload, ctx.AcceptChunked).ConfigureAwait(false);
                            }
                            else
                                LoggerAccessor.LogWarn($"[SceProfanityFilterService] - Filter data with path: {pfEbinPath} was not found in the profanity cache, skipping...");
                        }
                        else
                            LoggerAccessor.LogError($"[SceProfanityFilterService] - Security header was incorrect! expected:{expectedHash}");
                    }
                    else
                    {
                        match = Regex.Match(absolutePath, pfFileRegex);

                        if (match.Success)
                        {
                            string pfEbinPath = pfApiPath + $"{match.Groups["uuid"].Value}/{match.Groups["filename"].Value}";
                            ctx.StatusCode = HttpStatusCode.OK;
                            ctx.Response.ContentType = "application/octet-stream";
                            using (FileStream st = await FileSystemUtils.TryOpen(pfEbinPath, FileShare.ReadWrite, LocalFileStreamHelper.FileLockAwaitMs).ConfigureAwait(false))
                                return await ctx.SendImmediate(EncryptAES(st, pfKey, pfIV), ctx.AcceptChunked).ConfigureAwait(false);
                        }
                    }
                }

                return false;
            }

            return null;
        }

        object HTTPPlugin.ProcessPluginMessage(object request)
        {
            return ProcessPluginMessageAsync(request);
        }

        private static byte[] EncryptAES(byte[] cipherText, byte[] key, byte[] iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.Zeros;

            using ICryptoTransform encryptor = aesAlg.CreateEncryptor();

            return encryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
        }

        private static byte[] EncryptAES(FileStream plainTextStream, byte[] key, byte[] iv)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.Zeros;

            using MemoryStream encryptedStream = new MemoryStream();
            using ICryptoTransform encryptor = aesAlg.CreateEncryptor();
            using CryptoStream cryptoStream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write);

            plainTextStream.CopyTo(cryptoStream);
            cryptoStream.FlushFinalBlock();

            return encryptedStream.ToArray();
        }
    }
}
