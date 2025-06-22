using CustomLogger;
using EdNetService.Models;
using NetHasher.CRC;
using EdNetService.ClientChallengeData.TDU;

namespace EdenServer.ClientChallengeService
{
    internal static class ChallengeHandler
    {
        private const ushort CrcCheckSize = 1024;
        private const uint DevQuestion = 0x12345678;
        const string devFlag = "[VED]";

        private static ThreadLocal<uint> _holdrand = new ThreadLocal<uint>(() => 1);

        public static bool GenerateClientChallenge(string Version, ClientObject client)
        {
            bool isValid = false;

            switch (Version)
            {
                case "MC 1.45 A":
                    isValid = GenerateKeySet(TestDriveUnlimited145AExe.Data, client);
                    break;
                case "MC 1.66 A":
                    isValid = GenerateKeySet(TestDriveUnlimited166AExe.Data, client);
                    break;
                default:
                    if (Version.StartsWith(devFlag))
                        isValid = GenerateKeySet(null, client);
                    else
                        LoggerAccessor.LogWarn($"[ChallengeHandler] - GenerateClientChallenge: Unknown Version:{Version} requested by User:{client.Username}, please report to GITHUB!");
                    break;
            }

            return isValid;
        }

        public static (uint, uint, uint) GenerateClientQuestions(string Version)
        {
            switch (Version)
            {
                case "MC 1.45 A":
                    return GenerateQuestions(TestDriveUnlimited145AExe.Data);
                case "MC 1.66 A":
                    return GenerateQuestions(TestDriveUnlimited166AExe.Data);
                default:
                    if (Version.StartsWith(devFlag))
                        return GenerateQuestions(null);
                    LoggerAccessor.LogWarn($"[ChallengeHandler] - GenerateClientQuestions: Unknown Version:{Version} requested, please report to GITHUB!");
                    break;
            }

            return (0,0,0);
        }

        private static (uint, uint, uint) GenerateQuestions(byte[]? tduClientBytes)
        {
            // Dev client.
            if (tduClientBytes == null)
                return (DevQuestion, DevQuestion, DevQuestion);

            int clientLength = tduClientBytes.Length;

            return ((uint)(Rand() % (clientLength - CrcCheckSize)), (uint)(Rand() % (clientLength - CrcCheckSize)), (uint)(Rand() % (clientLength - CrcCheckSize)));
        }

        private static bool GenerateKeySet(byte[]? tduClientBytes, ClientObject client)
        {
            // Ignored for dev clients.
            if (tduClientBytes == null)
                return true;

            uint Answer1 = GetFileCrcAt(tduClientBytes, client.Question1);
            uint Answer2 = GetFileCrcAt(tduClientBytes, client.Question2);
            uint Answer3 = GetFileCrcAt(tduClientBytes, client.Question3);

            if (Answer1 != client.Answer1 || Answer2 != client.Answer2 || Answer3 != client.Answer3)
            {
                LoggerAccessor.LogError($"[ChallengeHandler] - GenerateKeySet: User:{client.Username} requested and invalid client challenge for Version:{client.Version}.");
                return false;
            }

            return true;
        }

        private static uint GetFileCrcAt(byte[] tduClientBytes, long offset)
        {
            uint result = 0;

            if (offset >= 0 && offset < tduClientBytes.Length)
            {
                byte[] buffer = new byte[CrcCheckSize]; // 1024-byte buffer, will be zero-padded if fewer bytes remain

                Array.Copy(tduClientBytes, offset, buffer, 0, (int)Math.Min(CrcCheckSize, tduClientBytes.Length - offset));

                result = CRC32.Create(buffer, 0, buffer.Length);
            }

            return result;
        }

        private static uint Rand()
        {
            uint uVar2 = _holdrand.Value * 0x343fd + 0x269ec3;
            _holdrand.Value = uVar2;
            return (uVar2 >> 16) & 0x7fff;
        }
    }
}
