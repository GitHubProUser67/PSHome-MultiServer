using System.Diagnostics;
using CastleLibrary.NetHasher.CRC;
using CustomLogger;
using EdNetService.Models;

namespace EdenServer.ClientChallengeService
{
    internal static class ChallengeHandler
    {
        private const ushort CrcCheckSize = 1024;
        private const uint DevQuestion = 0x12345678;
        const string devFlag = "[VED]";

        private static readonly ThreadLocal<uint> _holdrand = new(() => 1);

        private static readonly Dictionary<string, byte[]> exeBytesChal = GenerateChallenges();

        public static bool GenerateClientChallenge(string Version, ClientObject client)
        {
            var isValid = false;

            if (exeBytesChal.TryGetValue(Version, out var value))
                isValid = GenerateKeySet(value, client);
            else if (Version.StartsWith(devFlag))
                isValid = GenerateKeySet(null, client);
            else
                LoggerAccessor.LogWarn(
                    $"[ChallengeHandler] - GenerateClientChallenge: Unknown Version:{Version} requested by User:{client.Username}, if the version is legit, please insert the challenge data in the TDUClientsEXEs folder."
                );

            return isValid;
        }

        public static (uint, uint, uint) GenerateClientQuestions(string Version)
        {
            if (exeBytesChal.TryGetValue(Version, out var value))
                return GenerateQuestions(value);
            else if (Version.StartsWith(devFlag))
                return GenerateQuestions(null);

            LoggerAccessor.LogWarn(
                $"[ChallengeHandler] - GenerateClientQuestions: Unknown Version:{Version} requested, if the version is legit, please insert the challenge data in the TDUClientsEXEs folder."
            );

            return (0, 0, 0);
        }

        private static (uint, uint, uint) GenerateQuestions(byte[]? tduClientBytes)
        {
            // Dev client.
            if (tduClientBytes == null)
                return (DevQuestion, DevQuestion, DevQuestion);

            var clientLength = tduClientBytes.Length;

            return (
                (uint)(Rand() % (clientLength - CrcCheckSize)),
                (uint)(Rand() % (clientLength - CrcCheckSize)),
                (uint)(Rand() % (clientLength - CrcCheckSize))
            );
        }

        private static bool GenerateKeySet(byte[]? tduClientBytes, ClientObject client)
        {
            // Ignored for dev clients.
            if (tduClientBytes == null)
                return true;

            var Answer1 = GetFileCrcAt(tduClientBytes, client.Question1);
            var Answer2 = GetFileCrcAt(tduClientBytes, client.Question2);
            var Answer3 = GetFileCrcAt(tduClientBytes, client.Question3);

            if (Answer1 != client.Answer1 || Answer2 != client.Answer2 || Answer3 != client.Answer3)
            {
                LoggerAccessor.LogError(
                    $"[ChallengeHandler] - GenerateKeySet: User:{client.Username} requested and invalid client challenge for Version:{client.Version}."
                );
                return false;
            }

            return true;
        }

        private static uint GetFileCrcAt(byte[] tduClientBytes, long offset)
        {
            uint result = 0;

            if (offset >= 0 && offset < tduClientBytes.Length)
            {
                var buffer = new byte[CrcCheckSize]; // 1024-byte buffer, will be zero-padded if fewer bytes remain

                Array.Copy(
                    tduClientBytes,
                    offset,
                    buffer,
                    0,
                    (int)Math.Min(CrcCheckSize, tduClientBytes.Length - offset)
                );

                result = CRC32.Create(buffer, 0, buffer.Length);
            }

            return result;
        }

        private static uint Rand()
        {
            var uVar2 = (_holdrand.Value * 0x343fd) + 0x269ec3;
            _holdrand.Value = uVar2;
            return (uVar2 >> 16) & 0x7fff;
        }

        private static Dictionary<string, byte[]> GenerateChallenges()
        {
            var result = new Dictionary<string, byte[]>();
            var chalsDir = Directory.GetCurrentDirectory() + "/static/TDUClientsEXEs";

            if (Directory.Exists(chalsDir))
            {
                foreach (
                    var filePath in Directory.GetFiles(chalsDir, "*.*", SearchOption.AllDirectories)
                )
                {
                    try
                    {
                        var versionInfo = FileVersionInfo.GetVersionInfo(filePath);
                        if (
                            versionInfo != null
                            && !string.IsNullOrEmpty(versionInfo.ProductVersion)
                        )
                        {
                            result[versionInfo.ProductVersion] = File.ReadAllBytes(filePath);
                            LoggerAccessor.LogInfo(
                                $"[ChallengeHandler] - File: {filePath} with ProductVersion:{versionInfo.ProductVersion} was added to the challenge list."
                            );
                        }
                        else
                            LoggerAccessor.LogInfo(
                                $"[ChallengeHandler] - File: {filePath} has no ProductVersion, skipping..."
                            );
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(
                            $"[ChallengeHandler] - Failed to process {filePath}: {ex.Message}, skipping..."
                        );
                    }
                }
            }
            else
                LoggerAccessor.LogWarn(
                    $"[ChallengeHandler] - No Challenges folder found at location:{chalsDir}, generating empty challenge listing..."
                );

            return result;
        }
    }
}
