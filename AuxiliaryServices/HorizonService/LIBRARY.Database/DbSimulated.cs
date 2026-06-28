using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;
using CastleLibrary.NetHasher;
using CastleLibrary.NetHasher.CRC;
using CastleLibrary.S0ny.Edge;
using CastleLibrary.Utils;
using Horizon.LIBRARY.Database.Models;
using MultiServerLibrary.Extension.NET;
using Newtonsoft.Json;

namespace Horizon.LIBRARY.Database
{
    public class DbSimulated
    {
        public int[] AppIds { get; set; } =
        [
            0,
            120,
            10010,
            10130,
            10394,
            10414,
            10421,
            10538,
            10540,
            10550,
            10582,
            10584,
            10680,
            10681,
            10683,
            10684,
            10694,
            10782,
            10933,
            10934,
            10952,
            10954,
            10984,
            11204,
            11354,
            20032,
            20034,
            20040,
            20041,
            20042,
            20043,
            20044,
            20060,
            20190,
            20230,
            20244,
            20304,
            20314,
            20344,
            20371,
            20374,
            20384,
            20434,
            20454,
            20463,
            20624, // Warhawk DME.
            20764,
            20804,
            21064,
            21094,
            21244,
            21354,
            21513,
            21564,
            21574,
            21584,
            21594,
            21614,
            21624,
            21731,
            21784,
            21834,
            21874,
            21914,
            22204,
            22274,
            22284,
            22294,
            22304,
            23014,
            22500,
            22720,
            22920,
            22923,
            22924,
            23360,
            23624,
            24000,
            24180,
            97134,
        ];
        public ConcurrentList<uint> BannedIps = [];
        public ConcurrentList<(string, bool)> BannedMacs = [];
        public ConcurrentList<AccountRelationInviteDTO> BuddyInvitations = [];
        public ConcurrentList<NpIdDTO> NpIdAccounts = [];
        public int AccountIdCounter { get; set; } = 1;
        public ConcurrentList<AccountDTO> Accounts { get; set; } = [];
        public int ClanIdCounter { get; set; } = 1;
        public int ClanMessageIdCounter { get; set; } = 1;
        public int ClanInvitationIdCounter { get; set; } = 1;
        public ConcurrentList<ClanDTO> Clans { get; set; } = [];
        public ConcurrentList<MatchmakingSupersetDTO> MatchmakingSupersets = [];
        public ConcurrentList<FileDTO> MediusFiles = [];
        public ConcurrentList<FileMetaDataDTO> FileMetaData = [];
        public ConcurrentList<FileAttributesDTO> FileAttributes = [];
        public ConcurrentDictionary<int, Dictionary<string, string>> AppSettings { get; set; } = [];

        [RequiresUnreferencedCode("Uses reflection that may break when trimming.")]
        public bool Save(string filepath, string key)
        {
            using (
                var mutex = new Mutex(
                    false,
                    $"Global\\{nameof(DbSimulated) + CRC32.CreateCastagnoli(Encoding.UTF8.GetBytes(filepath + "sec"))}Lock"
                )
            )
            {
                try
                {
                    return MutexExtensions.TryWithMutex(
                        mutex,
                        TimeSpan.FromSeconds(10),
                        () =>
                        {
                            bool hasCrypto = !string.IsNullOrEmpty(key);
                            byte[] payload = Encoding.UTF8.GetBytes(
                                JsonConvert.SerializeObject(this, Formatting.Indented)
                            );

                            using (
                                var inStream = new MemoryStream(
                                    hasCrypto ? Zlib.EdgeZlibCompress(payload) : payload
                                )
                            )
                            {
                                MemoryStream outStream = null;

                                if (hasCrypto)
                                {
                                    using (var aes = Aes.Create())
                                    {
                                        aes.Key = DeriveKey(key, aes.KeySize / 8);
                                        aes.GenerateIV();

                                        using (outStream = new MemoryStream())
                                        {
                                            outStream.Write(aes.IV, 0, aes.IV.Length); // store IV at start of file

                                            using (
                                                var cryptoStream = new CryptoStream(
                                                    outStream,
                                                    aes.CreateEncryptor(),
                                                    CryptoStreamMode.Write
                                                )
                                            )
                                                inStream.CopyTo(cryptoStream);
                                        }
                                    }
                                }

                                // save
                                File.WriteAllBytes(
                                    filepath,
                                    outStream != null ? outStream.ToArray() : inStream.ToArray()
                                );
                            }
                        }
                    );
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError(
                        $"[DbSimulated] - Save: Failed to save database: {ex}"
                    );
                }
            }

            return false;
        }

        public bool Load(string filepath, string key)
        {
            if (!File.Exists(filepath))
                return false;

            using (
                var mutex = new Mutex(
                    false,
                    $"Global\\{nameof(DbSimulated) + CRC32.CreateCastagnoli(Encoding.UTF8.GetBytes(filepath + "sec"))}Lock"
                )
            )
            {
                try
                {
                    return MutexExtensions.TryWithMutex(
                        mutex,
                        TimeSpan.FromSeconds(10),
                        () =>
                        {
                            using (var inStream = new MemoryStream(File.ReadAllBytes(filepath)))
                            {
                                DbSimulated obj;

                                if (!string.IsNullOrEmpty(key))
                                {
                                    using (var aes = Aes.Create())
                                    {
                                        aes.Key = DeriveKey(key, aes.KeySize / 8);

                                        var iv = new byte[aes.BlockSize / 8];
                                        inStream.Read(iv, 0, iv.Length);
                                        aes.IV = iv;

                                        using (
                                            var cryptoStream = new CryptoStream(
                                                inStream,
                                                aes.CreateDecryptor(),
                                                CryptoStreamMode.Read
                                            )
                                        )
                                        using (var outStream = new MemoryStream())
                                        {
                                            cryptoStream.CopyTo(outStream);

                                            obj = JsonConvert.DeserializeObject<DbSimulated>(
                                                Encoding.UTF8.GetString(
                                                    Zlib.EdgeZlibDecompress(outStream.ToArray())
                                                )
                                            );
                                        }
                                    }
                                }
                                else
                                    obj = JsonConvert.DeserializeObject<DbSimulated>(
                                        Encoding.UTF8.GetString(inStream.ToArray())
                                    );

                                AppIds = obj.AppIds;
                                BannedIps = obj.BannedIps;
                                BannedMacs = obj.BannedMacs;
                                AccountIdCounter = obj.AccountIdCounter;
                                ClanIdCounter = obj.ClanIdCounter;
                                ClanMessageIdCounter = obj.ClanMessageIdCounter;
                                ClanInvitationIdCounter = obj.ClanInvitationIdCounter;
                                Accounts = obj.Accounts;
                                Clans = obj.Clans;
                                AppSettings = obj.AppSettings;
                            }
                        }
                    );
                }
                catch (Exception ex)
                {
                    CustomLogger.LoggerAccessor.LogError(
                        $"[DbSimulated] - Load: Failed to load database: {ex}"
                    );
                }
            }

            return false;
        }

        // Derive a fixed-length key from a passphrase
        private static byte[] DeriveKey(string password, int length)
        {
            var hash = DotNetHasher.ComputeSHA256(Encoding.UTF8.GetBytes(password));
            Array.Resize(ref hash, length);
            return hash;
        }
    }
}
