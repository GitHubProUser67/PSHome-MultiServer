using CastleLibrary.Sony.XI5;
using CustomLogger;
using NetHasher;
using SSFWServer.Helpers.DataMigrator;
using System.Text;
using CastleLibrary.Sony.SSFW;
using SSFWServer.Helpers.FileHelper;

namespace SSFWServer.Services
{
    public class IdentityService
    {
        private string? XHomeClientVersion;
        private string? generalsecret;
        private string? homeClientVersion;
        private string? xsignature;
        private string? key;

        public IdentityService(string XHomeClientVersion, string generalsecret, string homeClientVersion, string? xsignature, string? key)
        {
            this.XHomeClientVersion = XHomeClientVersion;
            this.generalsecret = generalsecret;
            this.homeClientVersion = homeClientVersion;
            this.xsignature = xsignature;
            this.key = key;
        }

        public string? HandleLogin(byte[]? ticketBuffer, string env)
        {
            if (ticketBuffer != null)
            {
                bool IsRPCN = false;
                string salt = string.Empty;
                string? RPCNsessionIdFallback = null;

                // Extract the desired portion of the binary data
                byte[] extractedData = new byte[0x63 - 0x54 + 1];

                // Copy it
                Array.Copy(ticketBuffer, 0x54, extractedData, 0, extractedData.Length);

                // Convert 0x00 bytes to 0x48 so FileSystem can support it
                for (int i = 0; i < extractedData.Length; i++)
                {
                    if (extractedData[i] == 0x00)
                        extractedData[i] = 0x48;
                }

                // setup username
                string username = Encoding.ASCII.GetString(extractedData);

                // get ticket
                XI5Ticket ticket = XI5Ticket.ReadFromBytes(ticketBuffer);

                // invalid ticket
                if (!ticket.Valid)
                {
                    // log to console
                    LoggerAccessor.LogWarn($"[SSFW] : User {username.Replace("H", string.Empty)} tried to alter their ticket data");

                    return null;
                }

                // RPCN
                if (ticket.IsSignedByRPCN)
                {
                    LoggerAccessor.LogInfo($"[SSFW] : User {username.Replace("H", string.Empty)} connected at: {DateTime.Now} and is on RPCN");

                    IsRPCN = true;
                }
                else if (username.EndsWith($"@{XI5Ticket.RPCNSigner}"))
                {
                    LoggerAccessor.LogError($"[SSFW] : User {username.Replace("H", string.Empty)} was caught using a RPCN suffix while not on it!");

                    return null;
                }
                else
                    LoggerAccessor.LogInfo($"[SSFW] : User {username.Replace("H", string.Empty)} connected at: {DateTime.Now} and is on PSN");

                (string, string) UserNames = new();
                (string, string) ResultStrings = new();
                (string, string) SessionIDs = new();

                // Convert the modified data to a string
                UserNames.Item2 = ResultStrings.Item2 = username + homeClientVersion;

                // Calculate the MD5 hash of the result
                if (!string.IsNullOrEmpty(xsignature))
                    salt = generalsecret + xsignature + XHomeClientVersion;
                else
                    salt = generalsecret + XHomeClientVersion;

                string hash = DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(ResultStrings.Item2 + salt));

                // Trim the hash to a specific length
                hash = hash[..14];

                // Append the trimmed hash to the result
                ResultStrings.Item2 += hash;

                string sessionIdFallback = GuidGenerator.SSFWGenerateGuid(hash, ResultStrings.Item2);

                SessionIDs.Item2 = GuidGenerator.SSFWGenerateGuid(hash, ResultStrings.Item2, SSFWServerConfiguration.SSFWSessionIdKey);

                if (IsRPCN)
                {
                    // Convert the modified data to a string
                    UserNames.Item1 = ResultStrings.Item1 = username + XI5Ticket.RPCNSigner + homeClientVersion;

                    // Calculate the MD5 hash of the result
                    if (!string.IsNullOrEmpty(xsignature))
                        salt = generalsecret + xsignature + XHomeClientVersion;
                    else
                        salt = generalsecret + XHomeClientVersion;

                    hash = DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(ResultStrings.Item1 + salt));

                    // Trim the hash to a specific length
                    hash = hash[..10];

                    // Append the trimmed hash to the result
                    ResultStrings.Item1 += hash;

                    RPCNsessionIdFallback = GuidGenerator.SSFWGenerateGuid(hash, ResultStrings.Item1);

                    SessionIDs.Item1 = GuidGenerator.SSFWGenerateGuid(hash, ResultStrings.Item1, SSFWServerConfiguration.SSFWSessionIdKey);
                }

                if (!string.IsNullOrEmpty(UserNames.Item1) && !SSFWServerConfiguration.SSFWCrossSave) // RPCN confirmed.
                {
                    SSFWUserSessionManager.RegisterUser(UserNames.Item1, SessionIDs.Item1!, ResultStrings.Item1!, ticket.Username.Length);

                    if (SSFWAccountManagement.AccountExists(UserNames.Item2, SessionIDs.Item2))
                        SSFWAccountManagement.CopyAccountProfile(UserNames.Item2, UserNames.Item1, SessionIDs.Item2, SessionIDs.Item1!, key);
                    else if (SSFWAccountManagement.AccountExists(UserNames.Item2, sessionIdFallback))
                        SSFWAccountManagement.CopyAccountProfile(UserNames.Item2, UserNames.Item1, sessionIdFallback, SessionIDs.Item1!, key);
                }
                else
                {
                    IsRPCN = false;

                    SSFWUserSessionManager.RegisterUser(UserNames.Item2, SessionIDs.Item2, ResultStrings.Item2, ticket.Username.Length);
                }

                int logoncount = SSFWAccountManagement.ReadOrMigrateAccount(extractedData, IsRPCN ? UserNames.Item1 : UserNames.Item2, IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2, key);

                if (logoncount <= 0)
                {
                    logoncount = SSFWAccountManagement.ReadOrMigrateAccount(extractedData, IsRPCN ? UserNames.Item1 : UserNames.Item2, IsRPCN ? RPCNsessionIdFallback : sessionIdFallback, key);

                    if (logoncount <= 0)
                    {
                        LoggerAccessor.LogError($"[SSFWLogin] - Invalid Account or LogonCount value for user: {(IsRPCN ? UserNames.Item1 : UserNames.Item2)}");
                        return null;
                    }
                }

                if (IsRPCN && Directory.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/AvatarLayoutService/{env}/{ResultStrings.Item2}") && !Directory.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/AvatarLayoutService/{env}/{ResultStrings.Item1}"))
                    DataMigrator.MigrateSSFWData(SSFWServerConfiguration.SSFWStaticFolder, ResultStrings.Item2, ResultStrings.Item1);

                string? resultString = IsRPCN ? ResultStrings.Item1 : ResultStrings.Item2;

                if (string.IsNullOrEmpty(resultString))
                {
                    LoggerAccessor.LogError($"[SSFWLogin] - Invalid ResultString value for user: {(IsRPCN ? UserNames.Item1 : UserNames.Item2)}");
                    return null;
                }

                Directory.CreateDirectory($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}");
                Directory.CreateDirectory($"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{resultString}");
                Directory.CreateDirectory($"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks");
                Directory.CreateDirectory($"{SSFWServerConfiguration.SSFWStaticFolder}/AvatarLayoutService/{env}/{resultString}");

                if (File.Exists(SSFWServerConfiguration.ScenelistFile))
                {
                    bool handled = false;

                    IDictionary<string, string> scenemap = ScenelistParser.sceneDictionary;

                    if (File.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/mylayout.json")) // Migrate data.
                    {
                        // Parsing each value in the dictionary
                        foreach (var kvp in new Services.LayoutService(key).SSFWGetLegacyFurnitureLayouts($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/mylayout.json"))
                        {
                            if (kvp.Key == "00000000-00000000-00000000-00000004")
                            {
                                File.WriteAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/HarborStudio.json", kvp.Value);
                                handled = true;
                            }
                            else
                            {
                                string scenename = scenemap.FirstOrDefault(x => x.Value == Program.ExtractPortion(kvp.Key, 13, 18)).Key;
                                if (!string.IsNullOrEmpty(scenename))
                                {
                                    if (File.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/{kvp.Key}.json")) // SceneID now mapped, so SceneID based file has become obsolete.
                                        File.Delete($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/{kvp.Key}.json");

                                    File.WriteAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/{scenename}.json", kvp.Value);
                                    handled = true;
                                }
                            }

                            if (!handled)
                                File.WriteAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/{kvp.Key}.json", kvp.Value);

                            handled = false;
                        }

                        File.Delete($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/mylayout.json");
                    }
                    else if (!File.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/HarborStudio.json"))
                        File.WriteAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/HarborStudio.json",
                            File.ReadAllText($"{SSFWServerConfiguration.SSFWLayoutsFolder}/HarborStudio.json"));
                }
                else
                {
                    if (!File.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/mylayout.json"))
                        File.WriteAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/mylayout.json",
                            File.ReadAllText($"{SSFWServerConfiguration.SSFWLayoutsFolder}/LegacyLayout.json"));
                }

                if (!File.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{resultString}/mini.json"))
                    File.WriteAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{resultString}/mini.json", SSFWServerConfiguration.SSFWMinibase);
                if (!File.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{resultString}.json"))
                    File.WriteAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{resultString}.json", "{\"objects\":[]}");
                if (!File.Exists($"{SSFWServerConfiguration.SSFWStaticFolder}/AvatarLayoutService/{env}/{resultString}/list.json"))
                    File.WriteAllText($"{SSFWServerConfiguration.SSFWStaticFolder}/AvatarLayoutService/{env}/{resultString}/list.json", "[]");

                return $"{{\"session\":[{{\"@id\":\"{(IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2)}\",\"person\":{{\"@id\":\"{resultString}\",\"logonCount\":\"{logoncount}\"}}}}]}}";
            }

            return null;
        }

        public string? HandleLoginSS(byte[]? ticketBuffer, string env)
        {
            if (ticketBuffer != null)
            {
                bool IsRPCN = false;
                string salt = string.Empty;
                string? RPCNsessionIdFallback = null;

                // Extract the desired portion of the binary data
                byte[] extractedData = new byte[0x63 - 0x54 + 1];

                // Copy it
                Array.Copy(ticketBuffer, 0x54, extractedData, 0, extractedData.Length);

                // Convert 0x00 bytes to 0x48 so FileSystem can support it
                for (int i = 0; i < extractedData.Length; i++)
                {
                    if (extractedData[i] == 0x00)
                        extractedData[i] = 0x48;
                }

                // setup username
                string username = Encoding.ASCII.GetString(extractedData);

                // get ticket
                XI5Ticket ticket = XI5Ticket.ReadFromBytes(ticketBuffer);

                // invalid ticket
                if (!ticket.Valid)
                {
                    // log to console
                    LoggerAccessor.LogWarn($"[SSFW] : User {username.Replace("H", string.Empty)} tried to alter their ticket data");

                    return null;
                }

                // RPCN
                if (ticket.IsSignedByRPCN)
                {
                    LoggerAccessor.LogInfo($"[SSFW] : User {username.Replace("H", string.Empty)} connected at: {DateTime.Now} and is on RPCN");

                    IsRPCN = true;
                }
                else if (username.EndsWith($"@{XI5Ticket.RPCNSigner}"))
                {
                    LoggerAccessor.LogError($"[SSFW] : User {username.Replace("H", string.Empty)} was caught using a RPCN suffix while not on it!");

                    return null;
                }
                else
                    LoggerAccessor.LogInfo($"[SSFW] : User {username.Replace("H", string.Empty)} connected at: {DateTime.Now} and is on PSN");

                (string, string) UserNames = new();
                (string, string) ResultStrings = new();
                (string, string) SessionIDs = new();

                // Convert the modified data to a string
                UserNames.Item2 = ResultStrings.Item2 = username + homeClientVersion;

                // Calculate the MD5 hash of the result
                if (!string.IsNullOrEmpty(xsignature))
                    salt = generalsecret + xsignature + XHomeClientVersion;
                else
                    salt = generalsecret + XHomeClientVersion;

                string hash = DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(ResultStrings.Item2 + salt));

                // Trim the hash to a specific length
                hash = hash[..14];

                // Append the trimmed hash to the result
                ResultStrings.Item2 += hash;

                string sessionIdFallback = GuidGenerator.SSFWGenerateGuid(hash, ResultStrings.Item2);

                SessionIDs.Item2 = GuidGenerator.SSFWGenerateGuid(hash, ResultStrings.Item2, SSFWServerConfiguration.SSFWSessionIdKey);

                if (IsRPCN)
                {
                    // Convert the modified data to a string
                    UserNames.Item1 = ResultStrings.Item1 = username + XI5Ticket.RPCNSigner + homeClientVersion;

                    // Calculate the MD5 hash of the result
                    if (!string.IsNullOrEmpty(xsignature))
                        salt = generalsecret + xsignature + XHomeClientVersion;
                    else
                        salt = generalsecret + XHomeClientVersion;

                    hash = DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(ResultStrings.Item1 + salt));

                    // Trim the hash to a specific length
                    hash = hash[..10];

                    // Append the trimmed hash to the result
                    ResultStrings.Item1 += hash;

                    RPCNsessionIdFallback = GuidGenerator.SSFWGenerateGuid(hash, ResultStrings.Item1);

                    SessionIDs.Item1 = GuidGenerator.SSFWGenerateGuid(hash, ResultStrings.Item1, SSFWServerConfiguration.SSFWSessionIdKey);
                }

                if (!string.IsNullOrEmpty(UserNames.Item1) && !SSFWServerConfiguration.SSFWCrossSave) // RPCN confirmed.
                {
                    SSFWUserSessionManager.RegisterUser(UserNames.Item1, SessionIDs.Item1!, ResultStrings.Item1!, ticket.Username.Length);

                    if (SSFWAccountManagement.AccountExists(UserNames.Item2, SessionIDs.Item2))
                        SSFWAccountManagement.CopyAccountProfile(UserNames.Item2, UserNames.Item1, SessionIDs.Item2, SessionIDs.Item1!, key);
                    else if (SSFWAccountManagement.AccountExists(UserNames.Item2, sessionIdFallback))
                        SSFWAccountManagement.CopyAccountProfile(UserNames.Item2, UserNames.Item1, sessionIdFallback, SessionIDs.Item1!, key);
                }
                else
                {
                    IsRPCN = false;

                    SSFWUserSessionManager.RegisterUser(UserNames.Item2, SessionIDs.Item2, ResultStrings.Item2, ticket.Username.Length);
                }

                int logoncount = SSFWAccountManagement.ReadOrMigrateAccount(extractedData, IsRPCN ? UserNames.Item1 : UserNames.Item2, IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2, key);

                if (logoncount <= 0)
                {
                    logoncount = SSFWAccountManagement.ReadOrMigrateAccount(extractedData, IsRPCN ? UserNames.Item1 : UserNames.Item2, IsRPCN ? RPCNsessionIdFallback : sessionIdFallback, key);

                    if (logoncount <= 0)
                    {
                        LoggerAccessor.LogError($"[SSFWLogin] - Invalid Account or LogonCount value for user: {(IsRPCN ? UserNames.Item1 : UserNames.Item2)}");
                        return null;
                    }
                }

                string? resultString = IsRPCN ? ResultStrings.Item1 : ResultStrings.Item2;

                if (string.IsNullOrEmpty(resultString))
                {
                    LoggerAccessor.LogError($"[SSFWLogin] - Invalid ResultString value for user: {(IsRPCN ? UserNames.Item1 : UserNames.Item2)}");
                    return null;
                }

                return $"{{\"session\": {{\"expires\": \"3097114741746\" ,\"id\":\"{(IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2)}\",\"person\":{{\"id\":\"{(IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2)}\",\"display_name\":\"{resultString}\"}},\"service\":{{\"id\":\"{(IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2)}\",\"display_name\":\"{resultString}\"}} }} }} }}";
            }

            return null;
        }

    }
}