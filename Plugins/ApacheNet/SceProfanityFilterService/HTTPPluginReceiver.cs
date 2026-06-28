using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using ApacheNet;
using ApacheNet.Models;
using ApacheNet.PluginManager;
using CastleLibrary.NetHasher;
using CustomLogger;
using MultiServerLibrary.Extension;
using Org.BouncyCastle.Utilities.Encoders;

namespace SonyCdnReroute
{
    public partial class HTTPPluginReceiver : IHTTPPlugin
    {
        private static readonly byte[] pfKey = new byte[16];
        private static readonly byte[] pfIV = new byte[16];

        private const string pfManifestRegex =
            @"^/manifest/(?<locale>[a-z]{2}_[A-Z]{2})/(?<uuid>[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12})$";
        private const string pfFileRegex =
            @"/(?<uuid>[0-9a-fA-F\-]{36})/(?<timestamp>\d{14})/(?<filename>[^/]+)";

        private static string pfApiPath;
        private static byte[] _privateKey;

        Task IHTTPPlugin.HTTPStartPlugin(string param)
        {
            string? privKeyStr = null;
            pfApiPath = param + "/SceProfanityFilterService/";
            Directory.CreateDirectory(pfApiPath);
            var privKeyPath = pfApiPath + "privateKey.json";
            if (File.Exists(privKeyPath))
                privKeyStr = JsonSerializer.Deserialize<string>(File.ReadAllText(privKeyPath));
            if (string.IsNullOrEmpty(privKeyStr))
                _privateKey = "NVluu9dWima10JIUKhCVvg==".IsBase64().DecodedBytes;
            else
            {
                var (IsValid, DecodedBytes) = privKeyStr.IsBase64();
                if (IsValid)
                    _privateKey = DecodedBytes;
                else
                {
                    LoggerAccessor.LogError(
                        $"[SceProfanityFilterService] - The private key at path: {privKeyPath} is not a base64 string, falling back to default key..."
                    );
                    _privateKey = "NVluu9dWima10JIUKhCVvg==".IsBase64().DecodedBytes;
                }
            }
            return Task.CompletedTask;
        }

        public static async Task<object?> ProcessPluginMessageAsync(object obj)
        {
            if (obj is ApacheContext ctx)
            {
                var request = ctx.Request;

                if (ctx.GetHost() == "update-prod.pfs.online.scee.com")
                {
                    var absolutePath = ctx.AbsolutePath;

                    var match = MyRegex().Match(absolutePath);

                    if (match.Success)
                    {
                        var locale = match.Groups["locale"].Value.Replace("_", "-");
                        var uuid = match.Groups["uuid"].Value;
                        var pfEbinPath = pfApiPath + $"{uuid}/Filter-{locale}.ebin";
                        var expectedHash = DotNetHasher.ComputeSHA256String(
                            Encoding.UTF8.GetBytes(
                                $"Everything in moderation, including moderation.{uuid}"
                            ),
                            _privateKey
                        );

                        if (
                            request.HeaderExists("ps4-auth")
                            && expectedHash == request.RetrieveHeaderValue("ps4-auth")
                        )
                        {
                            if (File.Exists(pfEbinPath))
                            {
                                // Note: the ebin file has a big endian timestamp at the end, it must match the file creation date for the client to accept it.
                                var timestamp = new FileInfo(pfEbinPath).CreationTimeUtc;
                                var timestampStr = timestamp.ToString("yyyyMMddHHmmss");
                                var iv = new byte[16];
                                var message = Base64.ToBase64String(
                                    EncryptAES(
                                        Encoding.UTF8.GetBytes(
                                            $@"{{
                                      ""uri"": ""http://update-prod.pfs.online.scee.com/{uuid}/{timestampStr}/Filter-{locale}.ebin"",
                                      ""version"": ""{timestampStr}"",
                                      ""iv"": ""{Base64.ToBase64String(pfIV)}"",
                                      ""key"": ""{Base64.ToBase64String(pfKey)}""
                                    }}"
                                        ),
                                        _privateKey,
                                        iv
                                    )
                                );
                                ctx.StatusCode = HttpStatusCode.OK;
                                ctx.Response.ContentType = "application/json";
                                var payload =
                                    @$"{{""rmac"":""{DotNetHasher.ComputeSHA256String(Encoding.UTF8.GetBytes(message), _privateKey).ToLower()}"",
                                ""message"":""{message}"",""iv"":""{Base64.ToBase64String(iv)}"",""timestamp"":
                                {(long)(timestamp - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero).UtcDateTime).TotalMilliseconds}}}";
                                return await ctx.SendImmediate(payload, ApacheContext.AcceptChunked)
                                    .ConfigureAwait(false);
                            }
                            else
                                LoggerAccessor.LogWarn(
                                    $"[SceProfanityFilterService] - Filter data with path: {pfEbinPath} was not found in the profanity cache, skipping..."
                                );
                        }
                        else
                            LoggerAccessor.LogError(
                                $"[SceProfanityFilterService] - Security header was incorrect! expected:{expectedHash}"
                            );
                    }
                    else
                    {
                        match = MyRegex1().Match(absolutePath);

                        if (match.Success)
                        {
                            var pfEbinPath =
                                pfApiPath
                                + $"{match.Groups["uuid"].Value}/{match.Groups["filename"].Value}";
                            ctx.StatusCode = HttpStatusCode.OK;
                            ctx.Response.ContentType = "application/octet-stream";
                            using (
                                var st = await FileSystemUtils
                                    .TryOpen(
                                        pfEbinPath,
                                        FileShare.ReadWrite,
                                        LocalFileStreamHelper.FileLockAwaitMs
                                    )
                                    .ConfigureAwait(false)
                            )
                                return await ctx.SendImmediate(
                                        EncryptAES(st, pfKey, pfIV),
                                        ApacheContext.AcceptChunked
                                    )
                                    .ConfigureAwait(false);
                        }
                    }
                }

                return false;
            }

            return null;
        }

        object IHTTPPlugin.ProcessPluginMessage(object request)
        {
            return ProcessPluginMessageAsync(request);
        }

        private static byte[] EncryptAES(byte[] cipherText, byte[] key, byte[] iv)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.Zeros;

            using var encryptor = aesAlg.CreateEncryptor();

            return encryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
        }

        private static byte[] EncryptAES(FileStream plainTextStream, byte[] key, byte[] iv)
        {
            using var aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.Zeros;

            using var encryptedStream = new MemoryStream();
            using var encryptor = aesAlg.CreateEncryptor();
            using var cryptoStream = new CryptoStream(
                encryptedStream,
                encryptor,
                CryptoStreamMode.Write
            );

            plainTextStream.CopyTo(cryptoStream);
            cryptoStream.FlushFinalBlock();

            return encryptedStream.ToArray();
        }

        public List<Route> GetRoutes()
        {
            return new List<Route> { };
        }

        [GeneratedRegex(pfManifestRegex)]
        private static partial Regex MyRegex();

        [GeneratedRegex(pfFileRegex)]
        private static partial Regex MyRegex1();
    }
}
