using System.Text;
using CastleLibrary.NetHasher;
using CastleLibrary.S0ny.SSFW;
using CastleLibrary.S0ny.XI5;
using CustomLogger;
using SSFWServer.Helpers.DataMigrator;
using SSFWServer.Helpers.FileHelper;

namespace SSFWServer.Services
{
    public class IdentityService(
        string XHomeClientVersion,
        string generalsecret,
        string homeClientVersion,
        string? xsignature,
        string? key
    )
    {
        private readonly string? XHomeClientVersion = XHomeClientVersion;
        private readonly string? generalsecret = generalsecret;
        private readonly string? homeClientVersion = homeClientVersion;
        private readonly string? xsignature = xsignature;
        private readonly string? key = key;

        public string? HandleLogin(byte[]? ticketBuffer, string env)
        {
            if (ticketBuffer != null)
            {
                var IsRPCN = false;
                var salt = string.Empty;
                string? RPCNsessionIdFallback = null;

                // Extract the desired portion of the binary data
                var extractedData = new byte[0x63 - 0x54 + 1];

                // Copy it
                Array.Copy(ticketBuffer, 0x54, extractedData, 0, extractedData.Length);

                // Convert 0x00 bytes to 0x48 so FileSystem can support it
                for (var i = 0; i < extractedData.Length; i++)
                {
                    if (extractedData[i] == 0x00)
                        extractedData[i] = 0x48;
                }

                // setup username
                var username = Encoding.ASCII.GetString(extractedData);

                // get ticket
                var ticket = XI5Ticket.ReadFromBytes(ticketBuffer);

                // invalid ticket
                if (!ticket.Valid)
                {
                    // log to console
                    LoggerAccessor.LogWarn(
                        $"[SSFW] : User {username.Replace("H", string.Empty)} tried to alter their ticket data"
                    );

                    return null;
                }

                // RPCN
                if (ticket.IsSignedByRPCN)
                {
                    LoggerAccessor.LogInfo(
                        $"[SSFW] : User {username.Replace("H", string.Empty)} connected at: {DateTime.Now} and is on RPCN"
                    );

                    IsRPCN = true;
                }
                else if (username.EndsWith($"@{XI5Ticket.RPCNSigner}"))
                {
                    LoggerAccessor.LogError(
                        $"[SSFW] : User {username.Replace("H", string.Empty)} was caught using a RPCN suffix while not on it!"
                    );

                    return null;
                }
                else
                    LoggerAccessor.LogInfo(
                        $"[SSFW] : User {username.Replace("H", string.Empty)} connected at: {DateTime.Now} and is on PSN"
                    );

                (string, string) UserNames = new();
                (string, string) ResultStrings = new();
                (string, string) SessionIDs = new();

                // Convert the modified data to a string
                UserNames.Item2 = ResultStrings.Item2 = username + homeClientVersion;

                // Calculate the MD5 hash of the result
                salt = !string.IsNullOrEmpty(xsignature)
                    ? generalsecret + xsignature + XHomeClientVersion
                    : generalsecret + XHomeClientVersion;

                var hash = DotNetHasher.ComputeMD5String(
                    Encoding.ASCII.GetBytes(ResultStrings.Item2 + salt)
                );

                // Trim the hash to a specific length
                hash = hash[..14];

                // Append the trimmed hash to the result
                ResultStrings.Item2 += hash;

                var sessionIdFallback = GuidGenerator.SSFWGenerateGuid(hash, ResultStrings.Item2);

                SessionIDs.Item2 = GuidGenerator.SSFWGenerateGuid(
                    hash,
                    ResultStrings.Item2,
                    SSFWServerConfiguration.SSFWSessionIdKey
                );

                if (IsRPCN)
                {
                    // Convert the modified data to a string
                    UserNames.Item1 = ResultStrings.Item1 =
                        username + XI5Ticket.RPCNSigner + homeClientVersion;

                    // Calculate the MD5 hash of the result
                    salt = !string.IsNullOrEmpty(xsignature)
                        ? generalsecret + xsignature + XHomeClientVersion
                        : generalsecret + XHomeClientVersion;

                    hash = DotNetHasher.ComputeMD5String(
                        Encoding.ASCII.GetBytes(ResultStrings.Item1 + salt)
                    );

                    // Trim the hash to a specific length
                    hash = hash[..10];

                    // Append the trimmed hash to the result
                    ResultStrings.Item1 += hash;

                    RPCNsessionIdFallback = GuidGenerator.SSFWGenerateGuid(
                        hash,
                        ResultStrings.Item1
                    );

                    SessionIDs.Item1 = GuidGenerator.SSFWGenerateGuid(
                        hash,
                        ResultStrings.Item1,
                        SSFWServerConfiguration.SSFWSessionIdKey
                    );
                }

                if (
                    !string.IsNullOrEmpty(UserNames.Item1) && !SSFWServerConfiguration.SSFWCrossSave
                ) // RPCN confirmed.
                {
                    SSFWUserSessionManager.RegisterUser(
                        UserNames.Item1,
                        SessionIDs.Item1!,
                        ResultStrings.Item1!,
                        ticket.Username.Length
                    );

                    if (SSFWAccountManagement.AccountExists(UserNames.Item2, SessionIDs.Item2))
                        SSFWAccountManagement.CopyAccountProfile(
                            UserNames.Item2,
                            UserNames.Item1,
                            SessionIDs.Item2,
                            SessionIDs.Item1!,
                            key
                        );
                    else if (
                        SSFWAccountManagement.AccountExists(UserNames.Item2, sessionIdFallback)
                    )
                        SSFWAccountManagement.CopyAccountProfile(
                            UserNames.Item2,
                            UserNames.Item1,
                            sessionIdFallback,
                            SessionIDs.Item1!,
                            key
                        );
                }
                else
                {
                    IsRPCN = false;

                    SSFWUserSessionManager.RegisterUser(
                        UserNames.Item2,
                        SessionIDs.Item2,
                        ResultStrings.Item2,
                        ticket.Username.Length
                    );
                }

                var logoncount = SSFWAccountManagement.ReadOrMigrateAccount(
                    extractedData,
                    IsRPCN ? UserNames.Item1 : UserNames.Item2,
                    IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2,
                    key
                );

                if (logoncount <= 0)
                {
                    logoncount = SSFWAccountManagement.ReadOrMigrateAccount(
                        extractedData,
                        IsRPCN ? UserNames.Item1 : UserNames.Item2,
                        IsRPCN ? RPCNsessionIdFallback : sessionIdFallback,
                        key
                    );

                    if (logoncount <= 0)
                    {
                        LoggerAccessor.LogError(
                            $"[SSFWLogin] - Invalid Account or LogonCount value for user: {(IsRPCN ? UserNames.Item1 : UserNames.Item2)}"
                        );
                        return null;
                    }
                }

                if (
                    IsRPCN
                    && Directory.Exists(
                        $"{SSFWServerConfiguration.SSFWStaticFolder}/AvatarLayoutService/{env}/{ResultStrings.Item2}"
                    )
                    && !Directory.Exists(
                        $"{SSFWServerConfiguration.SSFWStaticFolder}/AvatarLayoutService/{env}/{ResultStrings.Item1}"
                    )
                )
                    DataMigrator.MigrateSSFWData(
                        SSFWServerConfiguration.SSFWStaticFolder,
                        ResultStrings.Item2,
                        ResultStrings.Item1
                    );

                var resultString = IsRPCN ? ResultStrings.Item1 : ResultStrings.Item2;

                if (string.IsNullOrEmpty(resultString))
                {
                    LoggerAccessor.LogError(
                        $"[SSFWLogin] - Invalid ResultString value for user: {(IsRPCN ? UserNames.Item1 : UserNames.Item2)}"
                    );
                    return null;
                }

                var myLayoutPath =
                    $"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/mylayout.json";

                Directory.CreateDirectory(
                    $"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}"
                );
                Directory.CreateDirectory(
                    $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{resultString}"
                );
                Directory.CreateDirectory(
                    $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks"
                );
                Directory.CreateDirectory(
                    $"{SSFWServerConfiguration.SSFWStaticFolder}/AvatarLayoutService/{env}/{resultString}"
                );

                if (File.Exists(SSFWServerConfiguration.ScenelistFile))
                {
                    var handled = false;

                    IDictionary<string, string> scenemap = ScenelistParser.sceneDictionary;

                    if (File.Exists(myLayoutPath)) // Migrate data.
                    {
                        const string harborUuid = "00000000-00000000-00000000-00000004";

                        // Parsing each value in the dictionary
                        foreach (
                            var kvp in new LayoutService(key).SSFWGetLegacyFurnitureLayouts(
                                myLayoutPath
                            )
                        )
                        {
                            if (kvp.Key == harborUuid)
                            {
                                File.WriteAllText(
                                    $"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/HarborStudio.json",
                                    kvp.Value
                                );
                                handled = true;
                            }
                            else
                            {
                                var scenename = scenemap
                                    .FirstOrDefault(x =>
                                        x.Value == Program.ExtractPortion(kvp.Key, 13, 18)
                                    )
                                    .Key;
                                if (!string.IsNullOrEmpty(scenename))
                                {
                                    if (
                                        File.Exists(
                                            $"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/{kvp.Key}.json"
                                        )
                                    ) // SceneID now mapped, so SceneID based file has become obsolete.
                                        File.Delete(
                                            $"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/{kvp.Key}.json"
                                        );

                                    File.WriteAllText(
                                        $"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/{scenename}.json",
                                        kvp.Value
                                    );
                                    handled = true;
                                }
                            }

                            if (!handled)
                                File.WriteAllText(
                                    $"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/{kvp.Key}.json",
                                    kvp.Value
                                );

                            handled = false;
                        }

                        File.Delete(myLayoutPath);
                    }
                    else if (
                        !File.Exists(
                            $"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/HarborStudio.json"
                        )
                    )
                    {
                        var defaultLayoutPath =
                            $"{SSFWServerConfiguration.SSFWLayoutsFolder}/HarborStudio.json";

                        if (File.Exists(defaultLayoutPath))
                            File.WriteAllText(
                                $"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/HarborStudio.json",
                                File.ReadAllText(defaultLayoutPath)
                            );
                        else
                            File.WriteAllText(
                                $"{SSFWServerConfiguration.SSFWStaticFolder}/LayoutService/{env}/person/{resultString}/HarborStudio.json",
                                @"{
                              ""version"": 3,
                              ""wallpaper"": 2,
                              ""furniture"": [
                              ]
                            }"
                            );
                    }
                }
                else if (!File.Exists(myLayoutPath))
                {
                    var defaultLegacyLayoutPath =
                        $"{SSFWServerConfiguration.SSFWLayoutsFolder}/LegacyLayout.json";

                    if (File.Exists(defaultLegacyLayoutPath))
                        File.WriteAllText(myLayoutPath, File.ReadAllText(defaultLegacyLayoutPath));
                    else
                        File.WriteAllText(myLayoutPath, "[]");
                }

                var miniPath =
                    $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{resultString}/mini.json";
                var trunksPath =
                    $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{resultString}.json";
                var avtrListPath =
                    $"{SSFWServerConfiguration.SSFWStaticFolder}/AvatarLayoutService/{env}/{resultString}/list.json";

                if (!File.Exists(miniPath))
                    File.WriteAllText(miniPath, SSFWServerConfiguration.SSFWMinibase);
                if (!File.Exists(trunksPath))
                    File.WriteAllText(trunksPath, "{\"objects\":[]}");
                if (!File.Exists(avtrListPath))
                    File.WriteAllText(avtrListPath, "[]");

                return $"{{\"session\":[{{\"@id\":\"{(IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2)}\",\"person\":{{\"@id\":\"{resultString}\",\"logonCount\":\"{logoncount}\"}}}}]}}";
            }

            return null;
        }

        public string? HandleLoginSS(byte[]? ticketBuffer, string env)
        {
            if (ticketBuffer != null)
            {
                var IsRPCN = false;
                var salt = string.Empty;
                string? RPCNsessionIdFallback = null;

                // Extract the desired portion of the binary data
                var extractedData = new byte[0x63 - 0x54 + 1];

                // Copy it
                Array.Copy(ticketBuffer, 0x54, extractedData, 0, extractedData.Length);

                // Convert 0x00 bytes to 0x48 so FileSystem can support it
                for (var i = 0; i < extractedData.Length; i++)
                {
                    if (extractedData[i] == 0x00)
                        extractedData[i] = 0x48;
                }

                // setup username
                var username = Encoding.ASCII.GetString(extractedData);

                // get ticket
                var ticket = XI5Ticket.ReadFromBytes(ticketBuffer);

                // invalid ticket
                if (!ticket.Valid)
                {
                    // log to console
                    LoggerAccessor.LogWarn(
                        $"[SSFW] : User {username.Replace("H", string.Empty)} tried to alter their ticket data"
                    );

                    return null;
                }

                // RPCN
                if (ticket.IsSignedByRPCN)
                {
                    LoggerAccessor.LogInfo(
                        $"[SSFW] : User {username.Replace("H", string.Empty)} connected at: {DateTime.Now} and is on RPCN"
                    );

                    IsRPCN = true;
                }
                else if (username.EndsWith($"@{XI5Ticket.RPCNSigner}"))
                {
                    LoggerAccessor.LogError(
                        $"[SSFW] : User {username.Replace("H", string.Empty)} was caught using a RPCN suffix while not on it!"
                    );

                    return null;
                }
                else
                    LoggerAccessor.LogInfo(
                        $"[SSFW] : User {username.Replace("H", string.Empty)} connected at: {DateTime.Now} and is on PSN"
                    );

                (string, string) UserNames = new();
                (string, string) ResultStrings = new();
                (string, string) SessionIDs = new();

                // Convert the modified data to a string
                UserNames.Item2 = ResultStrings.Item2 = username + homeClientVersion;

                // Calculate the MD5 hash of the result
                salt = !string.IsNullOrEmpty(xsignature)
                    ? generalsecret + xsignature + XHomeClientVersion
                    : generalsecret + XHomeClientVersion;

                var hash = DotNetHasher.ComputeMD5String(
                    Encoding.ASCII.GetBytes(ResultStrings.Item2 + salt)
                );

                // Trim the hash to a specific length
                hash = hash[..14];

                // Append the trimmed hash to the result
                ResultStrings.Item2 += hash;

                var sessionIdFallback = GuidGenerator.SSFWGenerateGuid(hash, ResultStrings.Item2);

                SessionIDs.Item2 = GuidGenerator.SSFWGenerateGuid(
                    hash,
                    ResultStrings.Item2,
                    SSFWServerConfiguration.SSFWSessionIdKey
                );

                if (IsRPCN)
                {
                    // Convert the modified data to a string
                    UserNames.Item1 = ResultStrings.Item1 =
                        username + XI5Ticket.RPCNSigner + homeClientVersion;

                    // Calculate the MD5 hash of the result
                    salt = !string.IsNullOrEmpty(xsignature)
                        ? generalsecret + xsignature + XHomeClientVersion
                        : generalsecret + XHomeClientVersion;

                    hash = DotNetHasher.ComputeMD5String(
                        Encoding.ASCII.GetBytes(ResultStrings.Item1 + salt)
                    );

                    // Trim the hash to a specific length
                    hash = hash[..10];

                    // Append the trimmed hash to the result
                    ResultStrings.Item1 += hash;

                    RPCNsessionIdFallback = GuidGenerator.SSFWGenerateGuid(
                        hash,
                        ResultStrings.Item1
                    );

                    SessionIDs.Item1 = GuidGenerator.SSFWGenerateGuid(
                        hash,
                        ResultStrings.Item1,
                        SSFWServerConfiguration.SSFWSessionIdKey
                    );
                }

                if (
                    !string.IsNullOrEmpty(UserNames.Item1) && !SSFWServerConfiguration.SSFWCrossSave
                ) // RPCN confirmed.
                {
                    SSFWUserSessionManager.RegisterUser(
                        UserNames.Item1,
                        SessionIDs.Item1!,
                        ResultStrings.Item1!,
                        ticket.Username.Length
                    );

                    if (SSFWAccountManagement.AccountExists(UserNames.Item2, SessionIDs.Item2))
                        SSFWAccountManagement.CopyAccountProfile(
                            UserNames.Item2,
                            UserNames.Item1,
                            SessionIDs.Item2,
                            SessionIDs.Item1!,
                            key
                        );
                    else if (
                        SSFWAccountManagement.AccountExists(UserNames.Item2, sessionIdFallback)
                    )
                        SSFWAccountManagement.CopyAccountProfile(
                            UserNames.Item2,
                            UserNames.Item1,
                            sessionIdFallback,
                            SessionIDs.Item1!,
                            key
                        );
                }
                else
                {
                    IsRPCN = false;

                    SSFWUserSessionManager.RegisterUser(
                        UserNames.Item2,
                        SessionIDs.Item2,
                        ResultStrings.Item2,
                        ticket.Username.Length
                    );
                }

                var logoncount = SSFWAccountManagement.ReadOrMigrateAccount(
                    extractedData,
                    IsRPCN ? UserNames.Item1 : UserNames.Item2,
                    IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2,
                    key
                );

                if (logoncount <= 0)
                {
                    logoncount = SSFWAccountManagement.ReadOrMigrateAccount(
                        extractedData,
                        IsRPCN ? UserNames.Item1 : UserNames.Item2,
                        IsRPCN ? RPCNsessionIdFallback : sessionIdFallback,
                        key
                    );

                    if (logoncount <= 0)
                    {
                        LoggerAccessor.LogError(
                            $"[SSFWLogin] - Invalid Account or LogonCount value for user: {(IsRPCN ? UserNames.Item1 : UserNames.Item2)}"
                        );
                        return null;
                    }
                }

                var resultString = IsRPCN ? ResultStrings.Item1 : ResultStrings.Item2;

                if (string.IsNullOrEmpty(resultString))
                {
                    LoggerAccessor.LogError(
                        $"[SSFWLogin] - Invalid ResultString value for user: {(IsRPCN ? UserNames.Item1 : UserNames.Item2)}"
                    );
                    return null;
                }

                return $"{{\"session\": {{\"expires\": \"3097114741746\" ,\"id\":\"{(IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2)}\",\"person\":{{\"id\":\"{(IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2)}\",\"display_name\":\"{resultString}\"}},\"service\":{{\"id\":\"{(IsRPCN ? SessionIDs.Item1 : SessionIDs.Item2)}\",\"display_name\":\"{resultString}\"}} }} }} }}";
            }

            return null;
        }
    }
}
