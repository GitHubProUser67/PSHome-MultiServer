using CustomLogger;
using NetworkLibrary.Extension;
using WebAPIService.WebCrypto;
using Newtonsoft.Json;
using DotNetty.Transport.Channels;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.LIBRARY.Database.Config;
using Horizon.LIBRARY.Database.Entities;
using Horizon.LIBRARY.Database.Models;
using HorizonService.LIBRARY.Database.Simulated;
using System.Text;
#if !NETFRAMEWORK
using System.Web;
#endif
using System.Net;
using System.IO;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Linq;

namespace Horizon.LIBRARY.Database
{
    public class DbController
    {
        private string directoryPath = null;
        public DbSettings _settings = new DbSettings();

        private int _simulatedAccountIdCounter = 1;
        private int _simulatedClanIdCounter = 1;
        private int _simulatedClanMessageIdCounter = 1;
        private int _simulatedClanInvitationIdCounter = 1;
        private readonly int[] SimulatedAppIdList = new int[] {
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
            97134
        };
        private string _dbAccessToken = null;
        private string _dbAccountName = null;

        private readonly ConcurrentList<AccountDTO> _simulatedAccounts = new ConcurrentList<AccountDTO>();
        private readonly ConcurrentList<AccountRelationInviteDTO> _simulatedBuddyInvitations = new ConcurrentList<AccountRelationInviteDTO>();
        private readonly ConcurrentList<NpIdDTO> _simulatedNpIdAccounts = new ConcurrentList<NpIdDTO>();
        private readonly ConcurrentList<ClanDTO> _simulatedClans = new ConcurrentList<ClanDTO>();
        private readonly ConcurrentList<MatchmakingSupersetDTO> _simulatedMatchmakingSupersets = new ConcurrentList<MatchmakingSupersetDTO>();
        private readonly ConcurrentList<FileDTO> _simulatedMediusFiles = new ConcurrentList<FileDTO>();
        private readonly ConcurrentList<FileMetaDataDTO> _simulatedFileMetaData = new ConcurrentList<FileMetaDataDTO>();
        private readonly ConcurrentList<FileAttributesDTO> _simulatedFileAttributes = new ConcurrentList<FileAttributesDTO>();

        public DbController(string configFile)
        {
            /*Task t = new Task(() => {
                SimulatedAppIdList = new int[65536];
             
                // Initialize the first element
                SimulatedAppIdList[0] = 0;

                // Initialize the first small chunk manually
                for (int i = 1; i < 1024; i++)
                {
                    SimulatedAppIdList[i] = i;
                }

                int currentLength = 1024;

                // Use Array.Copy to double the size of initialized chunks
                while (currentLength < SimulatedAppIdList.Length)
                {
                    int copyLength = Math.Min(currentLength, SimulatedAppIdList.Length - currentLength);
                    Array.Copy(SimulatedAppIdList, 0, SimulatedAppIdList, currentLength, copyLength);

                    // Adjust the copied values
                    for (int i = currentLength; i < currentLength + copyLength; i++)
                    {
                        SimulatedAppIdList[i] += currentLength;
                    }

                    currentLength += copyLength;
                }
            });*/

            if (!string.IsNullOrEmpty(configFile))
            {
                directoryPath = Path.GetDirectoryName(configFile);

                if (!string.IsNullOrEmpty(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);

                    if (File.Exists(configFile))
                    {
                        // Populate existing object
                        try
                        {
                            JsonConvert.PopulateObject(File.ReadAllText(configFile), _settings);

                            /*if (_settings.SimulatedMode)
                                t.Start();*/
                        }
                        catch (Exception ex) { LoggerAccessor.LogError(ex); }
                    }
                    else
                    {
                        // Populate existing object
                        try
                        { 
                            File.WriteAllText(configFile, JsonConvert.SerializeObject(_settings)); 
                            JsonConvert.PopulateObject(File.ReadAllText(configFile), _settings);

                            /*if (_settings.SimulatedMode)
                                t.Start();*/
                        }
                        catch (Exception ex) { LoggerAccessor.LogError(ex); }
                    }
                }
            }

            /*if (t.Status != TaskStatus.Created)
            {
                t.Wait();
                t.Dispose();
            }*/
        }

        /// <summary>
        /// Authenticate with middleware.
        /// </summary>
        public async Task<bool> Authenticate()
        {
            // Succeed in simulated mode#region account
            if (_settings.SimulatedMode)
                return true;

            var response = await Authenticate(_settings.DatabaseUsername, _settings.DatabasePassword);

            // Validate
            if (response == null || response.Roles == null || !response.Roles.Contains("database"))
                return false;

            _dbAccountName = response.AccountName;
            _dbAccessToken = response.Token;

            return !string.IsNullOrEmpty(_dbAccessToken);
        }

        public bool AmIAuthenticated()
        {
            if (_settings.SimulatedMode)
                return true;

            return !string.IsNullOrEmpty(_dbAccessToken);
        }

        public string GetUsername()
        {
            if (_settings.SimulatedMode)
                return _settings.DatabaseUsername;

            return _dbAccountName;
        }

        #region Account

        public async Task<string> GetPlayerList()
        {
            string results = null;

            try
            {
                if (_settings.SimulatedMode) // Deprecated
                    return "[]";
                else
                {
                    HttpResponseMessage Resp = await GetDbAsync($"Account/getOnlineAccounts");
                    if (Resp != null)
                        results = await Resp.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return results;
        }

        /// <summary>
        /// Get account by name.
        /// </summary>
        /// <param name="name">Case insensitive name of player.</param>
        /// <returns>Returns account.</returns>
        public async Task<AccountDTO> GetAccountByName(string name, int appId, bool forceRpcnCheck = false)
        {
            AccountDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    if (name != null)
                    {
                        if (name == "gameRecorder_r2_pubeta_master" && appId == 21731)
                        {
                            AccountDTO R2PuBeta;
                            _simulatedAccounts.Add(R2PuBeta = new AccountDTO()
                            {
                                AccountId = 2,
                                AccountName = "gameRecorder_r2_pubeta_master",
                                AccountPassword = string.Empty,
                                AccountWideStats = new int[Constants.LADDERSTATSWIDE_MAXLEN],
                                AccountCustomWideStats = new int[1000],
                                AppId = 21731,
                                MachineId = string.Empty,
                                MediusStats = string.Empty,
                                Friends = Array.Empty<AccountRelationDTO>(),
                                Ignored = Array.Empty<AccountRelationDTO>(),
                                IsBanned = false
                            });

                            return R2PuBeta;
                        }
                        else if (name == "ftb3 Moderator_0" && appId == 21694)
                        {
                            AccountDTO ftb3Mod;
                            _simulatedAccounts.Add(ftb3Mod = new AccountDTO()
                            {
                                AccountId = 2,
                                AccountName = "ftb3 Moderator_0",
                                AccountPassword = string.Empty,
                                AccountWideStats = new int[Constants.LADDERSTATSWIDE_MAXLEN],
                                AccountCustomWideStats = new int[1000],
                                AppId = 21694,
                                MachineId = string.Empty,
                                MediusStats = string.Empty,
                                Friends = Array.Empty<AccountRelationDTO>(),
                                Ignored = Array.Empty<AccountRelationDTO>(),
                                IsBanned = false
                            });

                            return ftb3Mod;
                        }
                        else
                            result = _simulatedAccounts.FirstOrDefault(x =>
                                x.AppId == appId &&
                                x.AccountName != null &&
                                (forceRpcnCheck
                                    ? x.AccountName.ToLower() == name.ToLower()
                                    : (x.AccountName.EndsWith("@RPCN", StringComparison.OrdinalIgnoreCase)
                                        ? x.AccountName.Substring(0, x.AccountName.Length - "@RPCN".Length)
                                        : x.AccountName).ToLower() == name.ToLower()
                                )
                            );
                    }
                }
                else
                {
#if NETFRAMEWORK
                    name = Uri.EscapeDataString(name);
#else
                    name = HttpUtility.UrlEncode(name);
#endif
                    string route = $"Account/searchAccountByName?AccountName={name}&AppId={appId}&ForceRpcnCheck={forceRpcnCheck}";
                    result = await GetDbAsync<AccountDTO>(route);
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get account by id.
        /// </summary>
        /// <param name="id">Id of player.</param>
        /// <returns>Returns account.</returns>
        public async Task<AccountDTO> GetAccountById(int id)
        {
            AccountDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                    result = _simulatedAccounts.FirstOrDefault(x => x.AccountId == id);
                else
                    result = await GetDbAsync<AccountDTO>($"Account/getAccount?AccountId={id}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get account by the first registered Ip (usually unsafe, TODO: make medius refresh this on regular intervals).
        /// </summary>
        /// <param name="RequestedIp">Requested Ip to search for.</param>
        /// <returns>Returns account.</returns>
        public async Task<AccountDTO> GetAccountByFirstIp(string RequestedIp)
        {
            AccountDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                    result = _simulatedAccounts.FirstOrDefault(x => x.FirstClientIp == RequestedIp);
                else
                    result = await GetDbAsync<AccountDTO>($"Account/getAccount?RequestedIp={RequestedIp}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Creates an account.
        /// </summary>
        /// <param name="createAccount">Account creation parameters.</param>
        /// <returns>Returns created account.</returns>
        public async Task<AccountDTO> CreateAccount(CreateAccountDTO createAccount, IChannel clientChannel)
        {
            AccountDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var checkExisting = await GetAccountByName(createAccount.AccountName, createAccount.AppId, true);
                    if (checkExisting == null)
                    {
                        _simulatedAccounts.Add(result = new AccountDTO()
                        {
                            FirstClientIp = ((IPEndPoint)clientChannel.RemoteAddress).Address.ToString().Trim(new char[] { ':', 'f', '{', '}' }),
                            AccountId = _simulatedAccountIdCounter++,
                            AccountName = createAccount.AccountName,
                            AccountPassword = createAccount.AccountPassword,
                            AccountWideStats = new int[Constants.LADDERSTATSWIDE_MAXLEN],
                            AccountCustomWideStats = new int[1000],
                            AppId = createAccount.AppId,
                            MachineId = createAccount.MachineId,
                            MediusStats = createAccount.MediusStats,
                            Friends = Array.Empty<AccountRelationDTO>(),
                            Ignored = Array.Empty<AccountRelationDTO>(),
                            IsBanned = false
                        });
                    }
                    else
                        LoggerAccessor.LogError($"Account creation failed account name already exists!");
                }
                else
                {
                    var response = await PostDbAsync($"Account/createAccount", JsonConvert.SerializeObject(createAccount));

                    // Deserialize on success
                    if (response != null && response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<AccountDTO>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Delete account by name.
        /// </summary>
        /// <param name="accountName">Case insensitive name of account.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> DeleteAccount(string accountName, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = _simulatedAccounts.RemoveAll(x => x.AccountName != null && x.AccountName.ToLower() == accountName.ToLower() && x.AppId == appId) > 0;
                else
                    result = (await GetDbAsync($"Account/deleteAccount?AccountName={accountName}&AppId={appId}")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }
            return result;
        }

        public async Task<bool> PostAccountUpdatePassword(int accountId, string oldPassword, string newPassword)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await GetDbAsync($"Account/updateAccountPassword?accountId={accountId}&oldPassowrd={oldPassword}&newPassword={newPassword}")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Posts the account sign in date to the database.
        /// </summary>
        /// <param name="accountId">Id to post login date to.</param>
        /// <param name="time">Time logged in.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> PostAccountSignInDate(int accountId, DateTime time)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await PostDbAsync($"Account/postAccountSignInDate?AccountId={accountId}", $"\"{time.ToUniversalTime()}\"")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get account status by account id.
        /// </summary>
        /// <param name="accountId">Unique id of account.</param>
        /// <returns>Account status.</returns>
        public async Task<AccountStatusDTO> GetAccountStatus(int accountId)
        {
            AccountStatusDTO result = null;

            try
            {
                if (!_settings.SimulatedMode)
                    result = await GetDbAsync<AccountStatusDTO>($"Account/getAccountStatus?AccountId={accountId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Posts the current account status.
        /// </summary>
        /// <param name="status">Account status.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> PostAccountStatus(AccountStatusDTO status)
        {
            bool result = false;

            try
            {
                if (!_settings.SimulatedMode)
                    result = (await PostDbAsync($"Account/postAccountStatusUpdates", JsonConvert.SerializeObject(status))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        public async Task<bool> ClearAccountStatuses()
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await PostDbAsync($"Account/clearAccountStatuses", null)).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get account metadata by account id.
        /// </summary>
        /// <param name="accountId">Unique id of account.</param>
        /// <returns>Account metadata.</returns>
        public async Task<string> GetAccountMetadata(int accountId)
        {
            string result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = _simulatedAccounts.FirstOrDefault(x => x.AccountId == accountId);
                    result = account?.Metadata;
                }
                else
                    result = await GetDbAsync<string>($"Account/getAccountMetadata?accountId={accountId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Posts the given metadata to the given account.
        /// </summary>
        /// <param name="accountId">Id of account.</param>
        /// <param name="metadata">Metadata to post.</param>
        /// <returns>True on success.</returns>
        public async Task<bool> PostAccountMetadata(int accountId, string metadata)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = _simulatedAccounts.FirstOrDefault(x => x.AccountId == accountId);
                    if (account != null)
                    {
                        account.Metadata = metadata;
                        result = true;
                    }
                    else
                        result = false;
                }
                else
                    result = (await PostDbAsync($"Account/postAccountMetadata?accountId={accountId}", JsonConvert.SerializeObject(metadata))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Posts ip to account.
        /// </summary>
        public async Task<bool> PostAccountIp(int accountId, string ip)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await PostDbAsync($"Account/postAccountIp?AccountId={accountId}", $"\"{ip}\"")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Gets the total number of active players by app id.
        /// </summary>
        /// <param name="appId">App Id to filter total active accounts by.</param>
        /// <returns>Number of active accounts or null.</returns>
        public async Task<int?> GetActiveAccountCountByAppId(int appId)
        {
            int? result = null;

            try
            {
                if (_settings.SimulatedMode)
                    result = _simulatedAccounts.Count;
                else
                {
                    var response = await GetDbAsync($"Account/getActiveAccountCountByAppId?AppId={appId}");

                    // Deserialize on success
                    if (response.IsSuccessStatusCode && int.TryParse(await response.Content.ReadAsStringAsync(), out int r))
                        result = r;
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Gets the total number of active clans by app id.
        /// </summary>
        /// <param name="appId">App Id to filter total active clans by.</param>
        /// <returns>Number of active clans or null.</returns>
        public async Task<int?> GetActiveClanCountByAppId(int appId)
        {
            int? result = null;

            try
            {
                if (_settings.SimulatedMode)
                    result = _simulatedClans.Count(x => !x.IsDisbanded);
                else
                {
                    var response = await GetDbAsync($"Clan/getActiveClanCountByAppId?AppId={appId}");

                    // Deserialize on success
                    if (response.IsSuccessStatusCode && int.TryParse(await response.Content.ReadAsStringAsync(), out int r))
                        result = r;
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Gets whether or not the ip is banned.
        /// </summary>
        public async Task<bool> GetIsIpBanned(IPAddress ip)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    if (ip != null && ip != IPAddress.None && ip != IPAddress.Any && ip != IPAddress.IPv6Any)
                    {
                        (string, bool) ResultItem = JsonDatabaseController.ReadFromJsonFile(directoryPath, "IPAddress", InternetProtocolUtils.GetIPAddressAsUInt(ip).ToString());

                        switch (ResultItem.Item1)
                        {
                            case "OK":
                                return ResultItem.Item2;
                        }
                    }

                    return false;
                }
                else
                    result = (await GetDbAsync($"Account/getIpIsBanned?ipAddress={ip}")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Gets whether the mac address is banned.
        /// </summary>
        /// <param name="mac">MAC Address as a Base64 string</param>
        public async Task<bool> GetIsMacBanned(string mac)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    if (string.IsNullOrEmpty(mac))
                        return false;
                    else if (mac.Contains('-'))
                        mac = mac.Replace("-", string.Empty);

                    (string, bool) ResultItem = JsonDatabaseController.ReadFromJsonFile(directoryPath, "MacDatabase", mac);

                    switch (ResultItem.Item1)
                    {
                        case "OK":
                            return ResultItem.Item2;
                        default:
                            return false;
                    }
                }
                else
                    result = (await GetDbAsync($"Account/getMacIsBanned?macAddress={mac}")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Gets whether the Account Name is Mac Banned
        /// </summary>
        /// <param name="accountName">Account name to query.</param>
        public async Task<bool> GetIsAccountNameMacBanned(string accountName, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = false;
                else
                {
#if NETFRAMEWORK
                    accountName = Uri.EscapeDataString(accountName);
#else
                    accountName = HttpUtility.UrlEncode(accountName);
#endif
                    string route = $"Account/getAccountNameMacIsBanned?AccountName={accountName}&AppId={appId}";
                    result = await GetDbAsync<bool>(route);
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Check if an account is banned by Account Name, IP, or MAC
        /// </summary>
        /// <param name="accountName">Case insensitive name of player.</param>
        /// <param name="appId">Application ID.</param>
        /// <returns>Returns account.</returns>
        public async Task<bool> GetAccountIsBanned(string accountName, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    result = false;
                }
                else
                {
#if NETFRAMEWORK
                    accountName = Uri.EscapeDataString(accountName);
#else
                    accountName = HttpUtility.UrlEncode(accountName);
#endif
                    string route = $"Account/checkAccountIsBanned?AccountName={accountName}&AppId={appId}";
                    result = await GetDbAsync<bool>(route);
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Posts the given machine id to the database account with the given account id.
        /// </summary>
        /// <param name="accountId">Account id.</param>
        /// <param name="machineId">Machine id.</param>
        public async Task<bool> PostMachineId(int accountId, string machineId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    if (string.IsNullOrEmpty(machineId))
                        return false;
                    else if (machineId.Contains('-'))
                        machineId = machineId.Replace("-", string.Empty);

                    JsonDatabaseController.WriteToJsonFile(directoryPath, "MacDatabase", machineId);

                    return true;
                }
                else
                    result = (await PostDbAsync($"Account/postMachineId?AccountId={accountId}", $"\"{machineId}\"")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Posts the given ip to ban to the database.
        /// </summary>
        /// <param name="ipToBan">client ip.</param>
        public async Task<bool> BanIp(IPAddress ipToBan)
        {
            bool result = false;

            if (ipToBan == null || ipToBan == IPAddress.None || ipToBan == IPAddress.Any || ipToBan == IPAddress.IPv6Any)
                return result;

            try
            {
                if (_settings.SimulatedMode)
                {
                    JsonDatabaseController.WriteToJsonFile(directoryPath, "IPAddress", InternetProtocolUtils.GetIPAddressAsUInt(ipToBan).ToString());

                    return true;
                }
                else
                    result = (await PostDbAsync($"Account/BanIp", $"\"{ipToBan}\"")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

#endregion

        #region Buddy / Ignored

        /// <summary>
        /// Add buddy to buddy list.
        /// </summary>
        /// <param name="addBuddy">Add buddy parameters.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> addBuddyInvitation(AccountRelationInviteDTO addBuddyInvite)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    _simulatedBuddyInvitations.Add(addBuddyInvite);

                    result = true;
                }
                else
                    result = (await PostDbAsync($"Buddy/addBuddyInvitation", JsonConvert.SerializeObject(addBuddyInvite))).IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Add buddy to buddy list.
        /// </summary>
        /// <param name="addBuddy">Add buddy parameters.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> deleteBuddyInvitation(AccountRelationInviteDTO addBuddyInvite)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    _simulatedBuddyInvitations.Remove(addBuddyInvite);

                    result = true;
                }
                else
                    result = (await PostDbAsync($"Buddy/deleteBuddyInvitation", JsonConvert.SerializeObject(addBuddyInvite))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Retrieve pending buddy invites to buddy list.
        /// </summary>
        /// <param name="addBuddy">Add buddy parameters.</param>
        /// <returns>Success or failure.</returns>
        public async Task<List<AccountRelationInviteDTO>> retrieveBuddyInvitations(int appId, int accountId)
        {
            List<AccountRelationInviteDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(accountId);
                    if (account != null)
                    {
                        result = _simulatedBuddyInvitations.Where(x => x.AppId == appId).ToList();
                    }
                }
                else
                    result = await GetDbAsync<List<AccountRelationInviteDTO>>($"Buddy/retrieveBuddyInvitations?appId={appId}&accountId={accountId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Add buddy to buddy list.
        /// </summary>
        /// <param name="addBuddy">Add buddy parameters.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> AddBuddy(BuddyDTO addBuddy)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(addBuddy.AccountId);
                    var buddyAccount = await GetAccountById(addBuddy.BuddyAccountId);
                    if (account != null && buddyAccount != null && account.Friends != null)
                    {
                        var friends = account.Friends;
                        Array.Resize(ref friends, account.Friends.Length + 1);
                        friends[friends.Length - 1] = new AccountRelationDTO()
                        {
                            AccountId = buddyAccount.AccountId,
                            AccountName = buddyAccount.AccountName
                        };
                        account.Friends = friends;
                        result = true;
                    }
                }
                else
                    result = (await PostDbAsync($"Buddy/addBuddy", JsonConvert.SerializeObject(addBuddy))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Remove buddy from buddy list.
        /// </summary>
        /// <param name="removeBuddy">Remove buddy parameters.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> RemoveBuddy(BuddyDTO removeBuddy)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(removeBuddy.AccountId);
                    var buddyAccount = await GetAccountById(removeBuddy.BuddyAccountId);
                    if (account != null && buddyAccount != null && account.Friends != null)
                    {
                        var newFriends = new List<AccountRelationDTO>();
                        foreach (var friend in account.Friends)
                        {
                            if (friend.AccountId == buddyAccount.AccountId)
                                continue;

                            newFriends.Add(friend);
                        }
                        account.Friends = newFriends.ToArray();
                        result = true;
                    }
                }
                else
                    result = (await PostDbAsync($"Buddy/removeBuddy", JsonConvert.SerializeObject(removeBuddy))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Add player to ignored list.
        /// </summary>
        /// <param name="addIgnored">Add ignored parameters.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> AddIgnored(IgnoredDTO addIgnored)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(addIgnored.AccountId);
                    var ignoreAccount = await GetAccountById(addIgnored.IgnoredAccountId);
                    if (account != null && ignoreAccount != null && account.Ignored != null)
                    {
                        var ignored = account.Ignored;
                        Array.Resize(ref ignored, account.Ignored.Length + 1);
                        ignored[ignored.Length - 1] = new AccountRelationDTO()
                        {
                            AccountId = ignoreAccount.AccountId,
                            AccountName = ignoreAccount.AccountName
                        };
                        account.Ignored = ignored;
                        result = true;
                    }
                }
                else
                    result = (await PostDbAsync($"Buddy/addIgnored", JsonConvert.SerializeObject(addIgnored))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Remove player from ignored list.
        /// </summary>
        /// <param name="removeIgnored">Remove ignored parameters.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> RemoveIgnored(IgnoredDTO removeIgnored)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(removeIgnored.AccountId);
                    var ignoreAccount = await GetAccountById(removeIgnored.IgnoredAccountId);
                    if (account != null && ignoreAccount != null && account.Ignored != null)
                    {
                        var newIgnored = new List<AccountRelationDTO>();
                        foreach (var ignored in account.Ignored)
                        {
                            if (ignored.AccountId == ignoreAccount.AccountId)
                                continue;

                            newIgnored.Add(ignored);
                        }
                        account.Ignored = newIgnored.ToArray();
                        result = true;
                    }
                }
                else
                    result = (await PostDbAsync($"Buddy/removeIgnored", JsonConvert.SerializeObject(removeIgnored))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        #endregion

        #region Stats

        /// <summary>
        /// Get player wide stats.
        /// </summary>
        /// <param name="accountId">Account id of player.</param>
        /// <returns></returns>
        public async Task<StatPostDTO> GetPlayerWideStats(int accountId)
        {
            StatPostDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var stats = _simulatedAccounts.FirstOrDefault(x => x.AccountId == accountId)?.AccountWideStats;
                    if (stats != null)
                    {
                        result = new StatPostDTO()
                        {
                            AccountId = accountId,
                            Stats = stats
                        };
                    }
                }
                else
                    result = await GetDbAsync<StatPostDTO>($"Stats/getStats?AccountId={accountId}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Get clan wide stats.
        /// </summary>
        /// <param name="accountId">Clan id of clan.</param>
        /// <returns></returns>
        public async Task<ClanStatPostDTO> GetClanWideStats(int clanId)
        {
            ClanStatPostDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var stats = _simulatedClans.FirstOrDefault(x => x.ClanId == clanId)?.ClanWideStats;
                    if (stats != null)
                    {
                        result = new ClanStatPostDTO()
                        {
                            ClanId = clanId,
                            Stats = stats
                        };
                    }
                }
                else
                    result = await GetDbAsync<ClanStatPostDTO>($"Stats/getClanStats?ClanId={clanId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get player ranking in a given leaderboard.
        /// </summary>
        /// <param name="accountId">Account id of player.</param>
        /// <param name="statId">Index of stat. Starts at 1.</param>
        /// <returns>Leaderboard result for player.</returns>
        public async Task<LeaderboardDTO> GetPlayerLeaderboard(int accountId)
        {
            LeaderboardDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(accountId);
                    if (account == null)
                        return null;

                    return new LeaderboardDTO()
                    {
                        AccountId = accountId,
                        AccountName = account.AccountName,
                        Index = 1,
                        MediusStats = account.MediusStats,
                        TotalRankedAccounts = 1
                    };
                }
                else
                    result = await GetDbAsync<LeaderboardDTO>($"Stats/getPlayerLeaderboard?AccountId={accountId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get player ranking in a given leaderboard.
        /// </summary>
        /// <param name="accountId">Account id of player.</param>
        /// <param name="statId">Index of stat. Starts at 1.</param>
        /// <returns>Leaderboard result for player.</returns>
        public async Task<LeaderboardDTO> GetPlayerLeaderboardIndex(int accountId, int statId, int appId)
        {
            LeaderboardDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(accountId);
                    if (account == null)
                        return null;

                    return new LeaderboardDTO()
                    {
                        AccountId = accountId,
                        AccountName = account.AccountName,
                        Index = 1,
                        MediusStats = account.MediusStats,
                        StatValue = account.AccountWideStats[statId - 1],
                        TotalRankedAccounts = 1
                    };
                }
                else
                    result = await GetDbAsync<LeaderboardDTO>($"Stats/getPlayerLeaderboardIndex?AccountId={accountId}&StatId={statId}&AppId={appId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get clan ranking in a given leaderboard.
        /// </summary>
        /// <param name="clanId">Clan id of clan.</param>
        /// <param name="statId">Index of stat. Starts at 1.</param>
        /// <returns>Leaderboard result for clan.</returns>
        public async Task<ClanLeaderboardDTO> GetClanLeaderboardIndex(int clanId, int statId, int appId)
        {
            ClanLeaderboardDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var clan = await GetClanById(clanId, appId);
                    if (clan == null)
                        return null;

                    var ordered = _simulatedClans.Where(x => !x.IsDisbanded).OrderByDescending(x => x.ClanWideStats[statId]).ToList();
                    return new ClanLeaderboardDTO()
                    {
                        ClanId = clan.ClanId,
                        ClanName = clan.ClanName,
                        Index = ordered.FindIndex(0, ordered.Count, x => x.ClanId == clanId),
                        MediusStats = clan.ClanMediusStats,
                        StatValue = clan.ClanWideStats[statId],
                        TotalRankedClans = _simulatedClans.Count(x => !x.IsDisbanded)
                    };
                }
                else
                    result = await GetDbAsync<ClanLeaderboardDTO>($"Stats/getClanLeaderboardIndex?ClanId={clanId}&StatId={statId + 1}&AppId={appId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get clan leaderboard for a given stat by page and size.
        /// </summary>
        /// <param name="statId">Stat id. Starts at 1.</param>
        /// <param name="startIndex">Position to start gathering results from. Starts at 0.</param>
        /// <param name="size">Max number of items to retrieve.</param>
        /// <returns>Collection of leaderboard results for each player in page.</returns>
        public async Task<ClanLeaderboardDTO[]> GetClanLeaderboard(int statId, int startIndex, int size, int appId)
        {
            ClanLeaderboardDTO[] result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var ordered = _simulatedClans.Where(x => x.AppId == appId).Where(x => !x.IsDisbanded).OrderByDescending(x => x.ClanWideStats[statId]).Skip(startIndex).Take(size).ToList();
                    result = ordered.Select(x => new ClanLeaderboardDTO()
                    {
                        ClanId = x.ClanId,
                        ClanName = x.ClanName,
                        MediusStats = x.ClanMediusStats,
                        StatValue = x.ClanWideStats[statId],
                        TotalRankedClans = _simulatedClans.Count(y => !y.IsDisbanded),
                        Index = startIndex + ordered.IndexOf(x)
                    }).ToArray();
                }
                else
                    result = await GetDbAsync<ClanLeaderboardDTO[]>($"Stats/getClanLeaderboard?StatId={statId + 1}&StartIndex={startIndex}&Size={size}&AppId={appId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get leaderboard for a given stat by page and size.
        /// </summary>
        /// <param name="statId">Stat id. Starts at 1.</param>
        /// <param name="startIndex">Position to start gathering results from. Starts at 0.</param>
        /// <param name="size">Max number of items to retrieve.</param>
        /// <returns>Collection of leaderboard results for each player in page.</returns>
        public async Task<LeaderboardDTO[]> GetLeaderboard(int statId, int startIndex, int size, int appId)
        {
            LeaderboardDTO[] result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var ordered = _simulatedAccounts.Where(x => x.AppId == appId).OrderByDescending(x => x.AccountWideStats?[statId]).Skip(startIndex).Take(size).ToList();
                    result = ordered.Select(x => new LeaderboardDTO()
                    {
                        AccountId = x.AccountId,
                        AccountName = x.AccountName,
                        MediusStats = x.MediusStats,
                        StatValue = x.AccountWideStats[statId],
                        TotalRankedAccounts = 0,
                        Index = startIndex + ordered.IndexOf(x)
                    }).ToArray();
                }
                else
                    result = await GetDbAsync<LeaderboardDTO[]>($"Stats/getLeaderboard?StatId={statId}&StartIndex={startIndex}&Size={size}&AppId={appId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get leaderboard for a given stat by page and size.
        /// </summary>
        /// <param name="startIndex">Position to start gathering results from. Starts at 0.</param>
        /// <param name="size">Max number of items to retrieve.</param>
        /// <returns>Collection of leaderboard results for each player in page.</returns>
        public async Task<LeaderboardDTO[]> GetLeaderboardList(int startIndex, int size, int appId)
        {
            LeaderboardDTO[] result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var ordered = _simulatedAccounts.Where(x => x.AppId == appId).OrderByDescending(x => x.AccountWideStats?[0]).Skip(startIndex).Take(size).ToList();
                    result = ordered.Select(x => new LeaderboardDTO()
                    {
                        AccountId = x.AccountId,
                        AccountName = x.AccountName,
                        MediusStats = x.MediusStats,
                        TotalRankedAccounts = 0,
                        Index = startIndex + ordered.IndexOf(x)
                    }).ToArray();
                }
                else
                    result = await GetDbAsync<LeaderboardDTO[]>($"Stats/getLeaderboard?Size={size}&AppId={appId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Posts ladder stats to account id.
        /// </summary>
        /// <param name="statPost">Model containing account id and ladder stats collection.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> PostAccountLadderStats(StatPostDTO statPost)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(statPost.AccountId);
                    if (account == null)
                        return false;

                    account.AccountWideStats = statPost.Stats;
                    result = true;
                }
                else
                    result = (await PostDbAsync($"Stats/postStats", JsonConvert.SerializeObject(statPost))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Posts custom ladder stats to account id.
        /// </summary>
        /// <param name="statPost">Model containing account id and ladder stats collection.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> PostAccountLadderCustomStats(StatPostDTO statPost)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(statPost.AccountId);
                    if (account == null)
                        return false;

                    account.AccountCustomWideStats = statPost.Stats;
                    result = true;
                }
                else
                    result = (await PostDbAsync($"Stats/postStatsCustom", JsonConvert.SerializeObject(statPost))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Posts ladder stats to clan id.
        /// </summary>
        /// <param name="statPost">Model containing clan id and ladder stats collection.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> PostClanLadderStats(int accountId, int? clanId, int[] stats, int appId)
        {
            bool result = false;
            if (!clanId.HasValue)
                return false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(accountId);
                    if (account.ClanId != clanId)
                        return false;

                    var clan = await GetClanById(account.ClanId.Value, appId);
                    if (clan == null)
                        return false;

                    clan.ClanWideStats = stats;
                    result = true;
                }
                else
                {
                    result = (await PostDbAsync($"Stats/postClanStats", JsonConvert.SerializeObject(new ClanStatPostDTO()
                    {
                        ClanId = clanId.Value,
                        Stats = stats
                    }))).IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Posts custom ladder stats to clan id.
        /// </summary>
        /// <returns>Success or failure.</returns>
        public async Task<bool> PostClanLadderCustomStats(int accountId, int? clanId, int[] stats, int appId)
        {
            bool result = false;
            if (!clanId.HasValue)
                return false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(accountId);
                    if (account.ClanId != clanId)
                        return false;

                    var clan = await GetClanById(account.ClanId.Value, appId);
                    if (clan == null)
                        return false;

                    clan.ClanCustomWideStats = stats;
                    result = true;
                }
                else
                {
                    result = (await PostDbAsync($"Stats/postClanStatsCustom", JsonConvert.SerializeObject(new ClanStatPostDTO()
                    {
                        ClanId = clanId.Value,
                        Stats = stats
                    }))).IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Post medius stats to account.
        /// </summary>
        /// <param name="accountId">Account id to post stats to.</param>
        /// <param name="stats">Stats to post encoded as a Base64 string.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> PostMediusStats(int accountId, string stats)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var account = await GetAccountById(accountId);
                    if (account == null)
                        return false;

                    account.MediusStats = stats;
                    result = true;
                }
                else
                    result = (await PostDbAsync($"Account/postMediusStats?AccountId={accountId}", $"\"{stats}\""))?.IsSuccessStatusCode ?? false;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Post medius stats to clan.
        /// </summary>
        /// <param name="clanId">Clan id to post stats to.</param>
        /// <param name="stats">Stats to post encoded as a Base64 string.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> PostClanMediusStats(int clanId, string stats, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var clan = await GetClanById(clanId, appId);
                    if (clan == null)
                        return false;

                    clan.ClanMediusStats = stats;
                    result = true;
                }
                else
                    result = (await PostDbAsync($"Clan/postClanMediusStats?ClanId={clanId}", $"\"{stats}\""))?.IsSuccessStatusCode ?? false;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        #endregion

        #region Clan

        /// <summary>
        /// Get clan by name.
        /// </summary>
        /// <param name="name">Case insensitive name of clan.</param>
        /// <returns>Returns clan.</returns>
        public async Task<ClanDTO> GetClanByName(string name, int appId)
        {
            ClanDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                    result = _simulatedClans.FirstOrDefault(x => x.AppId == appId && x.ClanName != null && x.ClanName.ToLower() == name.ToLower());
                else
                    result = await GetDbAsync<ClanDTO>($"Clan/searchClanByName?clanName={name}&appId={appId}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Get clan by id.
        /// </summary>
        /// <param name="id">Id of clan.</param>
        /// <returns>Returns clan.</returns>
        public async Task<ClanDTO> GetClanById(int id, int appId)
        {
            ClanDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    result = _simulatedClans.FirstOrDefault(x => x.AppId == appId && x.ClanId == id);
                    /*
                    _simulatedClans.Add(result = new ClanDTO()
                    {
                        ClanId = _simulatedClanIdCounter++,
                        ClanName = "RTIME GROUP",
                        ClanLeaderAccount = creatorAccount,
                        ClanMemberAccounts = new List<AccountDTO>(new AccountDTO[] { creatorAccount }),
                        ClanMemberInvitations = new List<ClanInvitationDTO>(),
                        ClanMessages = new List<ClanMessageDTO>(),
                        ClanMediusStats = Convert.ToBase64String(new byte[Constants.CLANSTATS_MAXLEN]),
                        ClanWideStats = new int[Constants.LADDERSTATSWIDE_MAXLEN],
                        AppId = 1
                    });
                    result.ClanMediusStats = "1:2:3";
                    creatorAccount.ClanId = result.ClanId;
                    */

                }
                else
                    result = await GetDbAsync<ClanDTO>($"Clan/getClan?clanId={id}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get clan by id.
        /// </summary>
        /// <param name="id">Id of clan.</param>
        /// <returns>Returns clan.</returns>
        public async Task<List<ClanDTO>> GetClans(int appId)
        {
            List<ClanDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    result = _simulatedClans.Where(x => x.AppId == appId).ToList();
                    /*
                    _simulatedClans.Add(result = new ClanDTO()
                    {
                        ClanId = _simulatedClanIdCounter++,
                        ClanName = "RTIME GROUP",
                        ClanLeaderAccount = creatorAccount,
                        ClanMemberAccounts = new List<AccountDTO>(new AccountDTO[] { creatorAccount }),
                        ClanMemberInvitations = new List<ClanInvitationDTO>(),
                        ClanMessages = new List<ClanMessageDTO>(),
                        ClanMediusStats = Convert.ToBase64String(new byte[Constants.CLANSTATS_MAXLEN]),
                        ClanWideStats = new int[Constants.LADDERSTATSWIDE_MAXLEN],
                        AppId = 1
                    });
                    result.ClanMediusStats = "1:2:3";
                    creatorAccount.ClanId = result.ClanId;
                    */

                }
                else
                    result = await GetDbAsync<List<ClanDTO>>($"Clan/getClans?appId={appId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Creates a clan.
        /// </summary>
        /// <param name="createClan">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<ClanDTO> CreateClan(int creatorAccountId, string clanName, int appId, string mediusStats)
        {
            ClanDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var checkExisting = await GetClanByName(clanName, appId);
                    if (checkExisting == null)
                    {
                        var creatorAccount = await GetAccountById(creatorAccountId);
                        _simulatedClans.Add(result = new ClanDTO()
                        {
                            ClanId = _simulatedClanIdCounter++,
                            ClanName = clanName,
                            ClanLeaderAccount = creatorAccount,
                            ClanMember = new List<AccountDTO>(new AccountDTO[] { creatorAccount }),
                            ClanInvitations = new List<ClanInvitationDTO>(),
                            ClanMessages = new List<ClanMessageDTO>(),
                            ClanMediusStats = Convert.ToBase64String(new byte[Constants.CLANSTATS_MAXLEN]),
                            ClanStats = new int[Constants.LADDERSTATSWIDE_MAXLEN],
                            ClanWideStats = new int[Constants.LADDERSTATSWIDE_MAXLEN],
                            AppId = appId
                        });

                        creatorAccount.ClanId = result.ClanId;
                    }
                    else
                        LoggerAccessor.LogError($"Clan creation failed clan name already exists!");
                }
                else
                {
                    var response = await PostDbAsync($"Clan/createClan?accountId={creatorAccountId}&clanName={clanName}&appId={appId}&mediusStats={mediusStats}", null);

                    // Deserialize on success
                    if (response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<ClanDTO>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            { 
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Creates a clan.
        /// </summary>
        /// <param name="CreateClanSVO">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<ClanDTO> CreateClanSVO(int creatorAccountId, string clanName, string clanTAG, string playerName, int appId)
        {
            ClanDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {

                    var checkExisting = await GetClanByName(clanName, 0);
                    if (checkExisting == null)
                    {
                        var creatorAccount = await GetAccountById(creatorAccountId);
                        _simulatedClans.Add(result = new ClanDTO()
                        {
                            ClanId = _simulatedClanIdCounter++,
                            ClanName = clanName,
                            ClanLeaderAccount = creatorAccount,
                            ClanMember = new List<AccountDTO>(new AccountDTO[] { creatorAccount }),
                            ClanInvitations = new List<ClanInvitationDTO>(),
                            ClanMessages = new List<ClanMessageDTO>(),
                            ClanMediusStats = Convert.ToBase64String(new byte[Constants.CLANSTATS_MAXLEN]),
                            ClanStats = new int[Constants.LADDERSTATSWIDE_MAXLEN],
                            ClanWideStats = new int[Constants.LADDERSTATSWIDE_MAXLEN],
                        });

                        creatorAccount.ClanId = result.ClanId;
                    }
                    else
                    {
                        throw new Exception($"Clan creation failed clan name already exists!");
                    }
                }
                else
                {
                    var response = await PostDbAsync($"Clan/createClanSVO?accountId={creatorAccountId}&playerName={playerName}&clanName={clanName}&clanTAG={clanTAG}&appId={appId}", null);

                    // Deserialize on success
                    if (response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<ClanDTO>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanUpdateMetaDataSVO">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<bool> ClanUpdateMetaDataSVO(int clanId, string metaDataKey, string metaDataValue)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    result = (await PostDbAsync($"Clan/clanUpdateMetaDataSVO?&clanId={clanId}&metadatakey={metaDataKey}&metadatavalue={metaDataValue}", null)).IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanUpdateMetaDataSVO">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<List<ClanMetaDataDTO>> ClanGetMetaDataSVO(int clanId)
        {
            List<ClanMetaDataDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    var response = await GetDbAsync($"Clan/clan_GetMetaDataSVO?&clanId={clanId}");// Deserialize on success
                    if (response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<List<ClanMetaDataDTO>>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get SVO Clan Info by ID
        /// </summary>
        /// <param name="Clan_GetClanInfoByIDSVO">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<ClanDTO> Clan_GetClanInfoByIDSVO(int clanId, int appId)
        {
            ClanDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    var response = await GetDbAsync($"Clan/Clan_GetClanInfoByIDSVO?clanId={clanId}&appId={appId}");

                    // Deserialize on success
                    if (response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<ClanDTO>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get SVO Clan Info by ID
        /// </summary>
        /// <param name="Clan_GetClanInfoByIDSVO">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<ClanDTO> Clan_GetClanInfoByAcctIDSVO(int acctId)
        {
            ClanDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    var response = await GetDbAsync($"Clan/Clan_GetClanInfoByAcctIDSVO?acctId={acctId}");

                    // Deserialize on success
                    if (response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<ClanDTO>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get SVO Clan Players
        /// </summary>
        /// <param name="Clan_GetClanPlayersSVO">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<ClanPlayerDTO> Clan_GetClanPlayersSVO(int clanId, int start, int end)
        {
            ClanPlayerDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    var response = await GetDbAsync($"Clan/Clan_GetClanPlayersSVO?clanId={clanId}&start={start}&end={end}");

                    // Deserialize on success
                    if (response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<ClanPlayerDTO>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanCreateNews">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<ClanNewsDTO> ClanCreateNews(int clanId, string news, int appId)
        {
            ClanNewsDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    var response = await GetDbAsync($"Clan/clan_CreateNewsSVO?clanId={clanId}&newsBody={news}&appId={appId}");// Deserialize on success
                    if (response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<ClanNewsDTO>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanModifyNews">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<bool> ClanModifyNews(int clanId, int newsID, bool newsEdit, bool newsDelete, string newsBody, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    result = (await GetDbAsync($"Clan/clan_ModifyNewsSVO?clanId={clanId}&newsID={newsID}&newsEdit={newsEdit}&newsDelete={newsDelete}&newsBody={newsBody}&appId={appId}")).IsSuccessStatusCode;// Deserialize on success

                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanRevokeInvite">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<bool> ClanRevokeInvite(int acctId, int clanId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    result = (await GetDbAsync($"Clan/clan_RevokeInviteSVO?acctId={acctId}&clanId={clanId}")).IsSuccessStatusCode;// Deserialize on success

                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanRevokeInvite">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<bool> ClanRespondInvite(int clanInviteID, string news, bool accept, bool reject, int acctId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    result = (await GetDbAsync($"Clan/clan_RespondInviteSVO?clanInviteID={clanInviteID}&news={news}&accept={accept}&reject={reject}&acctId={acctId}")).IsSuccessStatusCode;// Deserialize on success

                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanModifyNews">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<bool> ClanDisband(int clanId, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    result = (await GetDbAsync($"Clan/clan_DisbandClanSVO?clanId={clanId}&appId={appId}")).IsSuccessStatusCode;// Deserialize on success

                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanUpdateMetaDataSVO">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<List<ClanNewsDTO>> ClanReadNews(int clanId, int appId)
        {
            List<ClanNewsDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    var response = await GetDbAsync($"Clan/clan_ReadNewsSVO?clanId={clanId}&appId={appId}");// Deserialize on success
                    if (response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<List<ClanNewsDTO>>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanUpdateMetaDataSVO">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<bool> SVOCreateClanEvent(SVOEventDTO SVOEvent)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    var response = (await PostDbAsync($"SVO/Calendar_CreateClanEvent", SVOEvent)).IsSuccessStatusCode;// Deserialize on success
                    result = response;
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanUpdateMetaDataSVO">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<List<SVOEventDTO>> SVOGetCalendarEvents(int appId, int acctId, int clanId, string startDate, string endDate, bool bTween)
        {
            List<SVOEventDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    var response = await GetDbAsync($"SVO/Calendar_GetEvents?&appId={appId}&acctId={acctId}&clanId={clanId}&startDate={startDate}&endDate={endDate}&bTween={bTween}");// Deserialize on success
                    if (response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<List<SVOEventDTO>>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// ClanUpdateMetaDataSVO
        /// </summary>
        /// <param name="ClanSendInviteNews">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<bool> ClanSendInvite(int clanId, string playerName, string inviteMsg, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    result = (await GetDbAsync($"Clan/clan_SendInviteSVO?clanId={clanId}&playerName={playerName}&inviteMsg={inviteMsg}&appId={appId}")).IsSuccessStatusCode;// Deserialize on success

                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Get SVO Clan Info by ID
        /// </summary>
        /// <param name="Clan_GetClanInfoByIDSVO">Clan creation parameters.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<List<ClanInvitationDTO>> ClanViewInvites(int acctId)
        {
            List<ClanInvitationDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    //SVO unimplemented
                }
                else
                {
                    var response = await GetDbAsync($"Clan/clan_ViewInviteSVO?acctId={acctId}");

                    // Deserialize on success
                    if (response.IsSuccessStatusCode)
                        result = JsonConvert.DeserializeObject<List<ClanInvitationDTO>>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Delete clan by id.
        /// </summary>
        /// <param name="clanId">Id of clan.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> DeleteClan(int accountId, int clanId, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var clan = await GetClanById(clanId, appId);
                    if (clan == null || clan.ClanLeaderAccount?.AccountId != accountId)
                        return false;

                    // remove members
                    foreach (var member in clan.ClanMember)
                        member.ClanId = null;

                    // revoke invitations
                    foreach (var inv in clan.ClanInvitations)
                        inv.ResponseStatus = 3;

                    // remove
                    return _simulatedClans.Remove(clan);
                }
                else
                    result = (await GetDbAsync($"Clan/deleteClan?accountId={accountId}&clanId={clanId}")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Transfers leadership of a clan to a new leader.
        /// </summary>
        /// <param name="leaderAccountId">Account id of leader.</param>
        /// <param name="clanId">Id of clan.</param>
        /// <param name="newLeaderAccountId">Account id of new leader.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<bool> ClanTransferLeadership(int leaderAccountId, int clanId, int newLeaderAccountId, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var clan = await GetClanById(clanId, appId);
                    if (clan == null || clan.ClanLeaderAccount?.AccountId != leaderAccountId)
                        return false;

                    var newLeaderAccount = await GetAccountById(newLeaderAccountId);
                    if (newLeaderAccount == null)
                        return false;

                    // must be a member
                    if (newLeaderAccount.ClanId != clanId)
                        return false;

                    clan.ClanLeaderAccount = newLeaderAccount;
                    result = true;
                }
                else
                {
                    result = (await PostDbAsync($"Clan/transferLeadership", JsonConvert.SerializeObject(new ClanTransferLeadershipDTO()
                    {
                        AccountId = leaderAccountId,
                        ClanId = clanId,
                        NewLeaderAccountId = newLeaderAccountId
                    }))).IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Transfers leadership of a clan to a new leader.
        /// </summary>
        /// <param name="leaderAccountId">Account id of leader.</param>
        /// <param name="clanId">Id of clan.</param>
        /// <param name="newLeaderAccountId">Account id of new leader.</param>
        /// <returns>Returns created clan.</returns>
        public async Task<bool> ClanLeave(int fromAccountId, int clanId, int accountId, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var clan = await GetClanById(clanId, appId);
                    if (clan == null)
                        return false;

                    // only allow leader or player remove player
                    if (fromAccountId != accountId && clan.ClanLeaderAccount?.AccountId != fromAccountId)
                        return false;

                    // prevent leader from leaving -- must transfer or disband
                    if (clan.ClanLeaderAccount?.AccountId == accountId)
                        return false;

                    var account = clan.ClanMember?.FirstOrDefault(x => x.AccountId == accountId);
                    if (account != null)
                    {
                        account.ClanId = null;
                        clan.ClanMember.Remove(account);
                    }

                    result = true;
                }
                else
                    result = (await PostDbAsync($"Clan/leaveClan?fromAccountId={fromAccountId}&clanId={clanId}&accountId={accountId}", null))?.IsSuccessStatusCode ?? false;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Creates a new clan invitation for the given player.
        /// </summary>
        /// <param name="fromAccountId">Id of account sending invite.</param>
        /// <param name="clanId">Id of clan.</param>
        /// <param name="accountId">Id of target player.</param>
        /// <param name="message">Invite message.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> CreateClanInvitation(int fromAccountId, int clanId, int accountId, string message)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get clan
                    var clan = _simulatedClans.FirstOrDefault(x => x.ClanId == clanId);
                    if (clan == null)
                        return false;

                    // validate from is leader
                    if (clan.ClanLeaderAccount?.AccountId != fromAccountId)
                        return false;

                    // get target account
                    var account = _simulatedAccounts.FirstOrDefault(x => x.AccountId == accountId);
                    if (account == null)
                        return false;

                    // check if invitations already made
                    if (clan.ClanInvitations != null && clan.ClanInvitations.Any(x => x.AccountId == accountId && x.ResponseStatus == 0))
                        return false;

                    // add
                    clan.ClanInvitations?.Add(new ClanInvitationDTO()
                    {
                        Id = _simulatedClanInvitationIdCounter++,
                        AppId = clan.AppId,
                        ClanId = clanId,
                        ClanName = clan.ClanName,
                        AccountId = accountId,
                        AccountName = account.AccountName,
                        InviteMsg = message
                    });

                    return true;
                }
                else
                {
                    result = (await PostDbAsync($"Clan/createInvitation?accountId={fromAccountId}", new ClanInvitationDTO()
                    {
                        ClanId = clanId,
                        AccountId = accountId,
                        InviteMsg = message
                    })).IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }


        /// <summary>
        /// Returns all clan invitations for the given player.
        /// </summary>
        /// <param name="accountId">Id of target player.</param>
        /// <returns>Success or failure.</returns>
        public async Task<List<AccountClanInvitationDTO>> GetClanInvitationsByAccount(int accountId)
        {
            List<AccountClanInvitationDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get clans
                    var clans = _simulatedClans.Where(x => x.ClanInvitations != null && x.ClanInvitations.Any(y => y.AccountId == accountId));

                    result = clans
                        .Select(x => new AccountClanInvitationDTO()
                        {
                            LeaderAccountId = x.ClanLeaderAccount.AccountId,
                            LeaderAccountName = x.ClanLeaderAccount.AccountName,
                            Invitation = x.ClanInvitations?.FirstOrDefault(y => y.AccountId == accountId)
                        })
                        .Where(x => x.Invitation != null)
                        .ToList();
                }
                else
                    result = (await GetDbAsync<List<AccountClanInvitationDTO>>($"Clan/invitations?accountId={accountId}"));
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Sets the response to the given clan invitation.
        /// </summary>
        /// <param name="accountId">Id of account.</param>
        /// <param name="inviteId">Id of clan invitation.</param>
        /// <param name="message">Response message to record.</param>
        /// <param name="responseStatus">Response to invitation.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> RespondToClanInvitation(int accountId, int inviteId, string message, int responseStatus)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // find invitation
                    var invite = _simulatedClans.Select(x => x.ClanInvitations?.FirstOrDefault(y => y.Id == inviteId)).FirstOrDefault(x => x != null);
                    if (invite == null)
                        return false;

                    // get clan
                    var clan = _simulatedClans.FirstOrDefault(x => x.ClanInvitations != null && x.ClanInvitations.Contains(invite));
                    if (clan == null)
                        return false;

                    // get account
                    var account = _simulatedAccounts.FirstOrDefault(x => x.AccountId == accountId);
                    if (account == null)
                        return false;

                    // validate its for user
                    if (invite.AccountId != accountId)
                        return false;

                    // validate it's not already been decided
                    if (invite.ResponseStatus != 0)
                        return false;

                    invite.ResponseMessage = message;
                    invite.ResponseStatus = responseStatus;
                    invite.ResponseTime = (int)DateTimeUtils.GetUnixTimeU32();

                    // handle accept
                    if (responseStatus == 1)
                    {
                        account.ClanId = invite.ClanId;
                        clan.ClanMember?.Add(account);
                    }

                    result = true;
                }
                else
                {
                    result = (await PostDbAsync($"Clan/respondInvitation", new ClanInvitationResponseDTO()
                    {
                        AccountId = accountId,
                        InvitationId = inviteId,
                        Response = responseStatus,
                        ResponseMessage = message,
                        ResponseTime = (int)DateTimeUtils.GetUnixTimeU32()
                    })).IsSuccessStatusCode;
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Revokes a given clan's invitation to the target account.
        /// </summary>
        /// <param name="fromAccountId">Id of account requesting revoke.</param>
        /// <param name="clanId">Id of clan.</param>
        /// <param name="targetAccountId">Target account to revoke invitation to.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> RevokeClanInvitation(int fromAccountId, int clanId, int targetAccountId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get clan
                    var clan = _simulatedClans.FirstOrDefault(x => x.ClanId == clanId);
                    if (clan == null)
                        return false;

                    // validate leader is fromAccount
                    if (clan.ClanLeaderAccount?.AccountId != fromAccountId)
                        return false;

                    // find invitation
                    var invite = clan.ClanInvitations?.FirstOrDefault(x => x.AccountId == targetAccountId);
                    if (invite == null)
                        return false;

                    // validate it's not already been decided
                    if (invite.ResponseStatus != 0)
                        return false;

                    invite.ResponseStatus = 3;

                    result = true;
                }
                else
                    result = (await PostDbAsync($"Clan/revokeInvitation?fromAccountId={fromAccountId}&clanId={clanId}&targetAccountId={targetAccountId}", null)).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Returns all clan invitations for the given player.
        /// </summary>
        /// <param name="accountId">Id of target player.</param>
        /// <returns>Success or failure.</returns>
        public async Task<List<ClanMessageDTO>> GetClanMessages(int accountId, int clanId, int startIndex, int pageSize, int appId)
        {
            List<ClanMessageDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get clan
                    var clan = await GetClanById(clanId, appId);
                    if (clan != null)
                    {
                        result = clan.ClanMessages?
                            .Skip(startIndex * pageSize)
                            .Take(pageSize)
                            .ToList();
                    }
                }
                else
                    result = await GetDbAsync<List<ClanMessageDTO>>($"Clan/messages?accountId={accountId}&clanId={clanId}&start={startIndex}&pageSize={pageSize}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Adds a clan message to the clan.
        /// </summary>
        /// <param name="accountId">Id of sender account.</param>
        /// <param name="clanId">Id of clan.</param>
        /// <param name="message">Message to add.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> ClanAddMessage(int accountId, int clanId, string message, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get clan
                    var clan = await GetClanById(clanId, appId);
                    if (clan == null)
                        return false;

                    // validate leader
                    if (clan.ClanLeaderAccount?.AccountId != accountId)
                        return false;

                    clan.ClanMessages?.Add(new ClanMessageDTO()
                    {
                        Id = _simulatedClanMessageIdCounter++,
                        Message = message
                    });

                    result = true;
                }
                else
                {
                    result = (await PostDbAsync($"Clan/addMessage?accountId={accountId}&clanId={clanId}", new ClanMessageDTO()
                    {
                        Message = message
                    })).IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Edits an existing clan message.
        /// </summary>
        /// <param name="accountId">Id of sender account.</param>
        /// <param name="clanId">Id of clan.</param>
        /// <param name="messageId">Id of clan message to edit.</param>
        /// <param name="message">Message to add.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> ClanEditMessage(int accountId, int clanId, int messageId, string message, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get clan
                    var clan = await GetClanById(clanId, appId);
                    if (clan == null)
                        return false;

                    // validate leader
                    if (clan.ClanLeaderAccount?.AccountId != accountId)
                        return false;

                    // find message
                    var clanMessage = clan.ClanMessages?.FirstOrDefault(x => x.Id == messageId);
                    if (clanMessage == null)
                        return false;

                    clanMessage.Message = message;

                    result = true;
                }
                else
                {
                    result = (await PutDbAsync($"Clan/editMessage?accountId={accountId}&clanId={clanId}", new ClanMessageDTO()
                    {
                        Id = messageId,
                        Message = message
                    })).IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Edits an existing clan message.
        /// </summary>
        /// <param name="accountId">Id of sender account.</param>
        /// <param name="clanId">Id of clan.</param>
        /// <param name="messageId">Id of clan message to delete.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> ClanDeleteMessage(int accountId, int clanId, int messageId, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get clan
                    var clan = await GetClanById(clanId, appId);
                    if (clan == null)
                        return false;

                    // validate leader
                    if (clan.ClanLeaderAccount?.AccountId != accountId)
                        return false;

                    // find message
                    var clanMessage = clan.ClanMessages?.FirstOrDefault(x => x.Id == messageId);
                    if (clanMessage == null)
                        return false;

                    clanMessage.Message = null;

                    result = true;
                }
                else
                {
                    result = (await PostDbAsync($"Clan/deleteMessage?accountId={accountId}&clanId={clanId}", new ClanMessageDTO()
                    {
                        Id = messageId
                    })).IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Request Clan Team Challenge from client
        /// </summary>
        /// <param name="challengerClanId">The ClanId of the clan to request a team challenge on.</param>
        /// <param name="accountId">Is this accountId the Clan Leader?</param>
        /// <param name="appId">AppId of the game to filter by</param>
        /// <param name="message">Message by the game to send for requesting the clan team challenge</param>
        /// <returns>Returns clan.</returns>
        public async Task<ClanTeamChallengeDTO> RequestClanTeamChallenge(int challengerClanId, int againstClanId, int accountId, string message, int appId)
        {
            ClanTeamChallengeDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get clan
                    var clan = await GetClanById((int)challengerClanId, appId);
                    if (clan == null)
                        return null;

                    // validate leader
                    if (clan.ClanLeaderAccount?.AccountId != accountId)
                        return null;

                    result = null; //_simulatedClans.FirstOrDefault(x => x.AppId == appId && x.ClanId == clanId);
                }
                else
                {
                    result = await PostDbAsync<ClanTeamChallengeDTO>($"Clan/requestClanTeamChallenge?challengerClanId={challengerClanId}&againstClanId={againstClanId}&accountId={accountId}&message={message}&appId={appId}", new ClanTeamChallengeDTO()
                    {

                    });
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        public async Task<List<ClanTeamChallengeDTO>> GetClanTeamChallenges(int clanId, int accountId, MediusClanChallengeStatus clanChallengeStatus, int startIdx, int pageSize, int appId)
        {
            List<ClanTeamChallengeDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get clan
                    var clan = await GetClanById(clanId, appId);
                    if (clan == null)
                        return null;

                    // validate leader
                    if (clan.ClanLeaderAccount?.AccountId != accountId)
                        return null;

                    result = null; //_simulatedClans.FirstOrDefault(x => x.AppId == appId && x.ClanId == clanId);
                }
                else
                    result = await GetDbAsync<List<ClanTeamChallengeDTO>>($"Clan/getClanTeamChallenges?clanId={clanId}&accountId={accountId}&clanChallengeStatus={(int)clanChallengeStatus}&appId={appId}&start={startIdx}&pageSize={pageSize}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        public async Task<bool> RespondClanTeamChallenge(int clanChallengeId, MediusClanChallengeStatus clanChallengeStatus, int accountId, string message, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = false; //_simulatedClans.FirstOrDefault(x => x.AppId == appId && x.ClanId == clanId);
                else
                {
                    result = (await PostDbAsync($"Clan/respondClanTeamChallenge?clanChallengeId={clanChallengeId}&clanChallengeStatus={(int)clanChallengeStatus}&accountId={accountId}&message={message}&appId={appId}", new ClanTeamChallengeDTO()
                    {

                    })).IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        public async Task<bool> RevokeClanTeamChallenge(int clanChallengeId, int accountId, int appId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = false; //_simulatedClans.FirstOrDefault(x => x.AppId == appId && x.ClanId == clanId);
                else
                {
                    result = (await PostDbAsync($"Clan/revokeClanTeamChallenge?clanChallengeId={clanChallengeId}&accountId={accountId}&appId={appId}", new ClanTeamChallengeDTO()
                    {

                    })).IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        #endregion

        #region Announcements / Policy

        /// <summary>
        /// Gets the latest announcement.
        /// </summary>
        public async Task<DimAnnouncements> GetLatestAnnouncement(int appId)
        {
            DimAnnouncements result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    /*
                        return new DimAnnouncements()
                        {
                            Id = 1,
                            AnnouncementTitle = "Announcement Title",
                            AnnouncementBody = "Announcement Body",
                            CreateDt = DateTime.UtcNow,
                        };
                    }*/
                }
                else
                    result = await GetDbAsync<DimAnnouncements>($"api/Keys/getAnnouncements?fromDt={DateTime.UtcNow}&AppId={appId}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Gets the latest announcements.
        /// </summary>
        public async Task<DimAnnouncements[]> GetLatestAnnouncementsList(int appId, int size = 10)
        {
            DimAnnouncements[] result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    switch (appId)
                    {
                        case 24000:
                            return new DimAnnouncements[]
                            {
                                new DimAnnouncements()
                                {
                                    Id = 1,
                                    AnnouncementTitle = "MultiServer Announcement! ",
                                    AnnouncementBody = "Welcome to the MultiServer Up Your Arsenal HD Server!",
                                    CreateDt = DateTime.UtcNow,
                                }
                            };
                        case 24180:
                            return new DimAnnouncements[]
                            {
                                new DimAnnouncements()
                                {
                                    Id = 1,
                                    AnnouncementTitle = "MultiServer Announcement! ",
                                    AnnouncementBody = "Welcome to the MultiServer Deadlocked HD Server!",
                                    CreateDt = DateTime.UtcNow,
                                }
                            };
                        case 10680:
                            return new DimAnnouncements[]
                            {
                                new DimAnnouncements()
                                {
                                    Id = 1,
                                    AnnouncementTitle = "MultiServer Announcement! ",
                                    AnnouncementBody = "Welcome to the MultiServer Up Your Arsenal Beta Trial Code Server!",
                                    CreateDt = DateTime.UtcNow,
                                }
                            };
                        case 10681:
                            return new DimAnnouncements[]
                            {
                                new DimAnnouncements()
                                {
                                    Id = 1,
                                    AnnouncementTitle = "MultiServer Announcement! ",
                                    AnnouncementBody = "Welcome to the MultiServer Up Your Arsenal Press Beta Server!",
                                    CreateDt = DateTime.UtcNow,
                                }
                            };
                        case 10683:
                        case 10684:
                            return new DimAnnouncements[]
                            {
                                new DimAnnouncements()
                                {
                                    Id = 1,
                                    AnnouncementTitle = "MultiServer Announcement! ",
                                    AnnouncementBody = "Welcome to the MultiServer Up Your Arsenal Server!",
                                    CreateDt = DateTime.UtcNow,
                                }
                            };
                        case 11354:
                            return new DimAnnouncements[]
                            {
                                new DimAnnouncements()
                                {
                                    Id = 1,
                                    AnnouncementTitle = "MultiServer Announcement! ",
                                    AnnouncementBody = "Welcome to the MultiServer Deadlocked Server!",
                                    CreateDt = DateTime.UtcNow,
                                }
                            };
                        case 11204:
                            return new DimAnnouncements[]
                            {
                                new DimAnnouncements()
                                {
                                    Id = 1,
                                    AnnouncementTitle = "MultiServer Announcement! ",
                                    AnnouncementBody = "Welcome to the MultiServer JakX Server!",
                                    CreateDt = DateTime.UtcNow,
                                }
                            };
                        case 21564:
                        case 21574:
                        case 21584:
                        case 21594:
                        case 22274:
                        case 22284:
                        case 22294:
                        case 22304:
                        case 20040:
                        case 20041:
                        case 20042:
                        case 20043:
                        case 20044:
                            return new DimAnnouncements[]
                            {
                                new DimAnnouncements()
                                {
                                    Id = 1,
                                    AnnouncementTitle = "MultiServer Announcement! ",
                                    AnnouncementBody = "Welcome to the MultiServer Warhawk Server!",
                                    CreateDt = DateTime.UtcNow,
                                }
                            };
                        default:
                            return new DimAnnouncements[]
                            {
                                new DimAnnouncements()
                                {
                                    Id = 1,
                                    AnnouncementTitle = "Announcement Title",
                                    AnnouncementBody = "Announcement Body",
                                    CreateDt = DateTime.UtcNow,
                                }
                            };
                    }
                }
                else
                    result = await GetDbAsync<DimAnnouncements[]>($"api/Keys/getAnnouncementsList?Dt={DateTime.UtcNow}&TakeSize={size}&AppId={appId}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Gets the Usage/Privacy policy.
        /// </summary>
        public async Task<DimEula> GetPolicy(int policyType, int appId)
        {
            DimEula result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    return new DimEula()
                    {
                        Id = 1,
                        AppId = 0,
                        PolicyType = policyType,
                        EulaTitle = "Simulated Policy Title.",
                        EulaBody = "Simulated Policy Body.",
                    };
                }
                else
                    result = await GetDbAsync<DimEula>($"api/Keys/getEULA?policyType={policyType}&appId={appId}&fromDt={DateTime.UtcNow}");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        #endregion

        #region Locations

        /// <summary>
        /// Get Locations
        /// </summary>
        public async Task<DimLocations> GetDimLocations(int LocationId, int appId)
        {
            DimLocations result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    return new DimLocations()
                    {
                        Id = 1,
                        LocationId = 1,
                        LocationName = "Default",
                    };
                }
                else
                    result = await GetDbAsync<DimLocations>($"api/keys/getLocations?LocationId={LocationId}&AppId={appId}&fromDt={DateTime.UtcNow}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        #endregion

        #region Medius File Services

        #region createFile
        public async Task<bool> createFile(FileDTO createFile)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    _simulatedMediusFiles.Add(createFile);
                    _simulatedFileAttributes.Add(new FileAttributesDTO
                    {
                        AppId = createFile.AppId,
                        FileID = createFile.FileID,
                        LastChangedByUserID = createFile.fileAttributesDTO.LastChangedByUserID,
                        LastChangedTimeStamp = createFile.fileAttributesDTO.LastChangedTimeStamp,
                        Description = createFile.fileAttributesDTO.Description,
                        StreamableFlag = createFile.fileAttributesDTO.StreamableFlag,
                        StreamingDataRate = createFile.fileAttributesDTO.StreamingDataRate,
                    });

                    result = true;
                }
                else
                {
                    result = (await PostDbAsync($"FileServices/addFile?AppId={createFile.AppId}&File={createFile}", JsonConvert.SerializeObject(new FileDTO
                    {
                        AppId = createFile.AppId,
                        FileName = createFile.FileName,
                        ServerChecksum = createFile.ServerChecksum,
                        FileID = createFile.FileID,
                        FileSize = createFile.FileSize,
                        CreationTimeStamp = createFile.CreationTimeStamp,
                        OwnerID = createFile.OwnerID,
                        GroupID = createFile.GroupID,
                        OwnerPermissionRWX = createFile.OwnerPermissionRWX,
                        GroupPermissionRWX = createFile.GroupPermissionRWX,
                        GlobalPermissionRWX = createFile.GlobalPermissionRWX,
                        ServerOperationID = createFile.ServerOperationID,
                        fileAttributesDTO = new FileAttributesDTO()
                        {
                            AppId = createFile.AppId,
                            FileID = createFile.FileID,
                            FileName = createFile.FileName,
                            LastChangedByUserID = createFile.fileAttributesDTO.LastChangedByUserID,
                            LastChangedTimeStamp = createFile.fileAttributesDTO.LastChangedTimeStamp,
                            Description = createFile.fileAttributesDTO.Description,
                            StreamableFlag = createFile.fileAttributesDTO.StreamableFlag,
                            StreamingDataRate = createFile.fileAttributesDTO.StreamingDataRate,
                        }
                    }))).IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }
        #endregion

        #region deleteFile
        public async Task<bool> deleteFile(FileDTO file)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    _simulatedMediusFiles.Remove(file);

                    result = true;
                }
                else
                    result = (await PostDbAsync($"FileServices/deleteFile", file)).IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }
        #endregion

        #region getFileList
        public async Task<List<FileDTO>> getFileList(int appId, string FileNameBeginsWith, uint OwnerByID)
        {
            List<FileDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    if (FileNameBeginsWith == "*")
                    {
                        _simulatedMediusFiles.Find(x => x.AppId == appId && x.OwnerID == OwnerByID);

                        result = _simulatedMediusFiles.ToList();
                    }
                    else
                    {
                        _simulatedMediusFiles.Find(x => x.AppId == appId && x.OwnerID == OwnerByID && x.FileName != null && x.FileName.StartsWith(FileNameBeginsWith));

                        result = _simulatedMediusFiles.ToList();
                    }
                }
                else
                {
                    if (OwnerByID > 2147483647)
                    {
                        OwnerByID = 2147483646;

                        LoggerAccessor.LogWarn($"FileNameBeginsWith: {FileNameBeginsWith} OwnerByID: {OwnerByID}");
                        result = await GetDbAsync<List<FileDTO>>($"FileServices/getFileList?AppId={appId}&FileNameBeginsWith={FileNameBeginsWith}&OwnerByID={OwnerByID}");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }
        #endregion

        #region getFileListExt
        public async Task<List<FileDTO>> getFileListExt(int appId, string FileNameBeginsWith, int OwnerByID, MediusFileMetaData fileMetaData)
        {
            List<FileDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    if (FileNameBeginsWith == "*")
                    {
                        FileMetaDataDTO fileMetaDataDTO = new FileMetaDataDTO()
                        {
                            AppId = appId,
                            Key = fileMetaData.Key,
                            Value = fileMetaData.Value,
                        };
                        _simulatedMediusFiles.Find(x => x.AppId == appId && x.OwnerID == OwnerByID && x.fileMetaDataDTO == _simulatedFileMetaData.Find(y => y == fileMetaDataDTO));

                        result = _simulatedMediusFiles.ToList();
                    }
                    else
                    {
                        FileMetaDataDTO fileMetaDataDTO = new FileMetaDataDTO()
                        {
                            AppId = appId,
                            Key = fileMetaData.Key,
                            Value = fileMetaData.Value,
                        };
                        _simulatedMediusFiles.Find(x => x.AppId == appId && x.OwnerID == OwnerByID && x.FileName != null && x.FileName.StartsWith(FileNameBeginsWith) && x.fileMetaDataDTO == _simulatedFileMetaData.Find(y => y == fileMetaDataDTO));

                        result = _simulatedMediusFiles.ToList();
                    }
                }
                else
                {
                    LoggerAccessor.LogWarn($"FileNameBeginsWith: {FileNameBeginsWith} OwnerByID: {OwnerByID} metaKey: {fileMetaData.Key}");
                    result = await GetDbAsync<List<FileDTO>>($"FileServices/getFileListExt?AppId={appId}&FileNameBeginsWith={FileNameBeginsWith}&OwnerByID={OwnerByID}&metaKey={fileMetaData.Key}&metaValue={fileMetaData.Value}");
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }
        #endregion

        #region UpdateFileAttributes
        /// <summary>
        /// Posts the given key and value to the database for that given file
        /// </summary>
        /// <param name="appId">appId for MFS</param>
        /// <param name="mediusFile">MediusFile specified by UpdateFileMetaRequest</param>
        /// <param name="mediusFileAttributes">mediusFileAttributes specified by Game Developer.</param>
        public async Task<List<FileAttributesDTO>> UpdateFileAttributes(FileDTO file)
        {
            List<FileAttributesDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    if (_simulatedFileMetaData == null)
                        return result;

                    _simulatedMediusFiles.Add(file);
                    _simulatedFileAttributes.Add(new FileAttributesDTO
                    {
                        AppId = file.AppId,
                        FileID = file.FileID,
                        FileName = file.FileName,
                        LastChangedByUserID = file.fileAttributesDTO.LastChangedByUserID,
                        LastChangedTimeStamp = file.fileAttributesDTO.LastChangedTimeStamp,
                        Description = file.fileAttributesDTO.Description,
                        StreamableFlag = file.fileAttributesDTO.StreamableFlag,
                        StreamingDataRate = file.fileAttributesDTO.StreamingDataRate,
                    });


                    result = _simulatedFileAttributes.ToList();
                }
                else
                    result = await PostDbAsync<List<FileAttributesDTO>>($"FileServices/updateFileAttributes?File={file}", file);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }
        #endregion

        #region GetFileAttributes
        /// <summary>
        /// Posts the given key and value to the database for that given file
        /// </summary>
        /// <param name="appId">appId for MFS</param>
        /// <param name="mediusFile">MediusFile specified by UpdateFileMetaRequest</param>
        public async Task<List<FileAttributesDTO>> GetFileAttributes(FileDTO mediusFile)
        {
            List<FileAttributesDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    if (_simulatedFileMetaData == null)
                        return result;

                    _simulatedMediusFiles.Contains(mediusFile);
                    _simulatedFileAttributes.Find(x => x.FileID == mediusFile.FileID);


                    result = _simulatedFileAttributes.ToList();
                }
                else
                    result = await GetDbAsync<List<FileAttributesDTO>>($"FileServices/getFileAttributes?AppId={mediusFile.AppId}&File={mediusFile}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }
        #endregion

        #region UpdateFileMetaData
        /// <summary>
        /// Posts the given key and value to the database for that given file
        /// </summary>
        /// <param name="appId">appId for MFS</param>
        /// <param name="mediusFile">MediusFile specified by UpdateFileMetaRequest</param>
        /// <param name="mediusFileMetaData">MediusFileMetaData specified by Game Developer.</param>
        public async Task<bool> UpdateFileMetaData(FileDTO file)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    if (!_simulatedMediusFiles.Contains(file))
                    {
                        _simulatedMediusFiles.Add(file);
                        if (file.fileMetaDataDTO != null)
                            _simulatedFileMetaData.Add(file.fileMetaDataDTO);
                    }

                    result = true;
                }
                else
                {
                    result = (await PostDbAsync($"FileServices/updateFileMetaData?AppId={file.AppId}", JsonConvert.SerializeObject(new FileMetaDataDTO
                    {
                        AppId = file.AppId,
                        FileID = file.FileID,
                        FileName = file.FileName,
                        Key = file.fileMetaDataDTO.Key,
                        Value = file.fileMetaDataDTO.Value,
                        CreateDt = DateTime.Now,
                    }))).IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }
        #endregion

        #region GetFileMetaData
        /// <summary>
        /// Posts the given key and value to the database for that given file
        /// </summary>
        /// <param name="appId">appId for MFS</param>
        /// <param name="mediusFile">MediusFile specified by UpdateFileMetaRequest</param>
        /// <param name="mediusFileMetaData">MediusFileMetaData specified by Game Developer.</param>
        public async Task<List<FileMetaDataDTO>> GetFileMetaData(int appId, string fileName, string Key)
        {
            List<FileMetaDataDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    if (_simulatedFileMetaData == null)
                        return result;

                    result = _simulatedFileMetaData.ToList();
                }
                else
                    result = await GetDbAsync<List<FileMetaDataDTO>>($"FileServices/getFileMetaData?appId={appId}&FileName={fileName}&Key={Key}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }
        #endregion

        #endregion

        #region NpIds
        /// <summary>
        /// Post the NpId to the database
        /// </summary>
        public async Task<bool> PostNpId(NpIdDTO NpId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                {
                    _simulatedNpIdAccounts.Add(new NpIdDTO()
                    {
                        AppId = NpId.AppId,
                        data = NpId.data,
                        term = NpId.term,
                        dummy = NpId.dummy,

                        opt = NpId.opt,
                        reserved = NpId.reserved,
                        CreateDt = DateTime.UtcNow
                    });

                    return true;
                }
                else
                {
                    result = (await PostDbAsync($"Account/postNpId?&AppId={NpId.AppId}&SceNpId={NpId}&createDt={DateTime.UtcNow}", JsonConvert.SerializeObject(new NpIdDTO
                    {
                        AppId = NpId.AppId,
                        data = NpId.data,
                        term = NpId.term,
                        dummy = NpId.dummy,

                        opt = NpId.opt,
                        reserved = NpId.reserved,
                        CreateDt = DateTime.UtcNow
                    }))).IsSuccessStatusCode;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Post the NpId to the database
        /// </summary>
        public async Task<List<NpIdDTO>> NpIdSearchByAccountNames(NpIdDTO NpId, int appId)
        {
            List<NpIdDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    _simulatedNpIdAccounts.FirstOrDefault(simulatedNpId => simulatedNpId == NpId && simulatedNpId.AppId == appId);

                    result = _simulatedNpIdAccounts.ToList();

                    return result;
                }
                else
                    result = await GetDbAsync<List<NpIdDTO>>($"Account/searchNpIdByAccountName?SceNpId={NpId}&AppId={appId}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }
        #endregion

        #region Game

        public async Task<string> GetGameList()
        {
            string results = null;

            HttpResponseMessage Resp = null;
            try
            {
                if (_settings.SimulatedMode) // Deprecated
                    return "[]";
                else
                {
                    Resp = await GetDbAsync($"api/Game/list");
                    if (Resp != null)
                        results = await Resp.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return results;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public async Task<bool> CreateGame(GameDTO game)
        {
            bool result = false;
            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await PostDbAsync($"api/Game/create", JsonConvert.SerializeObject(game))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public async Task<bool> UpdateGame(GameDTO game)
        {
            bool result = false;
            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await PutDbAsync($"api/Game/update/{game.GameId}", JsonConvert.SerializeObject(game))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public async Task<bool> UpdateGameMetadata(int gameId, string metadata)
        {
            bool result = false;
            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await PutDbAsync($"api/Game/updateMetaData/{gameId}", JsonConvert.SerializeObject(metadata))).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Delete game by game id.
        /// </summary>
        /// <param name="gameId">Game id.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> DeleteGame(int gameId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await DeleteDbAsync($"api/Game/delete/{gameId}")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        /// <summary>
        /// Clear the active games table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ClearActiveGames()
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await DeleteDbAsync($"api/Game/clear")).IsSuccessStatusCode;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }

        #endregion

        #region World

        public async Task<ChannelDTO[]> GetChannels()
        {
            ChannelDTO[] results = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    return new ChannelDTO[]
                    {
                        
                    };
                }
                else
                    results = await GetDbAsync<ChannelDTO[]>($"api/World/getChannels");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return results;
        }

        public async Task<LocationDTO[]> GetLocations()
        {
            LocationDTO[] results = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    return new LocationDTO[]
                    {
                        new LocationDTO()
                        {
                            AppId = 0,
                            Id = 0,
                            Name = "Location 1"
                        },
                        new LocationDTO()
                        {
                            AppId = 23044,
                            Id = 0,
                            Name = "US"
                        },
                        new LocationDTO()
                        {
                            AppId = 24000,
                            Id = 40,
                            Name = "Aquatos"
                        },
                        new LocationDTO()
                        {
                            AppId = 24180,
                            Id = 40,
                            Name = "Aquatos"
                        },
                        new LocationDTO()
                        {
                            AppId = 10680,
                            Id = 40,
                            Name = "Aquatos"
                        },
                        new LocationDTO()
                        {
                            AppId = 10681,
                            Id = 40,
                            Name = "Aquatos"
                        },
                        new LocationDTO()
                        {
                            AppId = 10683,
                            Id = 40,
                            Name = "Aquatos"
                        },
                        new LocationDTO()
                        {
                            AppId = 10684,
                            Id = 40,
                            Name = "Aquatos"
                        }
                    };
                }
                else
                    results = await GetDbAsync<LocationDTO[]>($"api/World/getLocations");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return results;
        }

        public async Task<LocationDTO[]> GetLocations(int appId)
        {
            LocationDTO[] results = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    switch (appId)
                    {
                        case 24000:
                        case 24180:
                        case 10680:
                        case 10681:
                        case 10683:
                        case 10684:
                            return new LocationDTO[]
                            {
                                new LocationDTO()
                                {
                                    AppId = appId,
                                    Id = 40,
                                    Name = "Aquatos"
                                }
                            };
                        case 23360:
                            return new LocationDTO[]
                            {
                                new LocationDTO()
                                {
                                    AppId = appId,
                                    Id = 1,
                                    Name = "Europe"
                                },
                                new LocationDTO()
                                {
                                    AppId = appId,
                                    Id = 2,
                                    Name = "United States"
                                },
                                new LocationDTO()
                                {
                                    AppId = appId,
                                    Id = 3,
                                    Name = "Mars City"
                                },
                                new LocationDTO()
                                {
                                    AppId = appId,
                                    Id = 4,
                                    Name = "Metro Polis"
                                }
                            };
                        case 23044:
                            return new LocationDTO[]
                            {
                                new LocationDTO()
                                {
                                    AppId = appId,
                                    Id = 40,
                                    Name = "US"
                                }
                            };
                        default:
                            return new LocationDTO[]
                            {
                                new LocationDTO()
                                {
                                    AppId = 0,
                                    Id = 0,
                                    Name = "Location 1"
                                }
                            };
                    }
                }
                else
                    results = await GetDbAsync<LocationDTO[]>($"api/World/getLocations/{appId}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return results;
        }

        #endregion

        #region Matchmaking 

        /// <summary>
        /// Returns all Matchmaking Supersets.
        /// </summary>
        /// <param name="appId">AppId of the game.</param>
        /// <returns>Returns list of supersets or no result.</returns>
        public async Task<List<MatchmakingSupersetDTO>> GetMatchmakingSupersets(int appId)
        {
            List<MatchmakingSupersetDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get matchmaking supersets
                    var supersets = _simulatedMatchmakingSupersets.Where(x => x.AppId == appId);

                    result = supersets.Select(x => new MatchmakingSupersetDTO()
                    {
                        SupersetID = 1,
                        SupersetName = "Casual",
                        SupersetDescription = "M:PR Matchmaking",
                        AppId = 21624
                    }).Where(x => x.SupersetID != 0).ToList();
                }
                else
                    result = (await GetDbAsync<List<MatchmakingSupersetDTO>>($"Matchmaking/supersets?appId={appId}"));

            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        #endregion

        #region Party

        /// <summary>
        /// 
        /// </summary>
        /// <param name="party"></param>
        /// <returns></returns>
        public async Task<bool> CreateParty(PartyDTO party)
        {
            bool result = false;
            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await PostDbAsync($"api/Party/create", JsonConvert.SerializeObject(party))).IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="party"></param>
        /// <returns></returns>
        public async Task<bool> UpdateParty(PartyDTO party)
        {
            bool result = false;
            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await PutDbAsync($"api/Game/update/{party.PartyId}", JsonConvert.SerializeObject(party))).IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="partyId"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        public async Task<bool> UpdatePartyMetadata(int partyId, string metadata)
        {
            bool result = false;
            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await PutDbAsync($"api/Party/updateMetaData/{partyId}", JsonConvert.SerializeObject(metadata))).IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Delete party by party id.
        /// </summary>
        /// <param name="partyId">Party id.</param>
        /// <returns>Success or failure.</returns>
        public async Task<bool> DeleteParty(int partyId)
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await DeleteDbAsync($"api/Party/delete/{partyId}")).IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// Clear the active Parties table.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> ClearActiveParties()
        {
            bool result = false;

            try
            {
                if (_settings.SimulatedMode)
                    result = true;
                else
                    result = (await DeleteDbAsync($"api/Party/clear")).IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        #endregion

        #region Universes

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId">appid of that universe to find</param>
        /// <returns></returns>
        public async Task<List<UniverseDTO>> GetUniverses(int appId)
        {
            List<UniverseDTO> result = null;
            try
            {
                if (_settings.SimulatedMode)
                    result = null;
                else
                    result = await GetDbAsync<List<UniverseDTO>>($"Universes/getUniverses?appId={appId}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="appId">appid of that universe to find</param>
        /// <returns></returns>
        public async Task<List<UniverseNewsDTO>> GetUniverseNews(int appId)
        {
            List<UniverseNewsDTO> result = null;
            try
            {
                if (_settings.SimulatedMode)
                    result = null;
                else
                    result = await GetDbAsync<List<UniverseNewsDTO>>($"Universes/getUniverseNews?appId={appId}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        #endregion

        #region SVO CUSTOM
        /*
        /// <summary>
        /// Returns all clan invitations for the given player.
        /// </summary>
        /// <param name="accountId">Id of target player.</param>
        /// <returns>Success or failure.</returns>
        public async Task<List<MatchmakingSupersetDTO>> getStarhawkURIStore(string path)
        {
            List<MatchmakingSupersetDTO> result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    // get clans
                    var supersets = _simulatedURIstores.Where(x => x.path == path);

                    // 
                    result = supersets.Select(x => new MatchmakingSupersetDTO()
                    {
                        SupersetID = 1,
                        SupersetName = "Casual",
                        SupersetDescription = "M:PR Matchmaking",
                        AppId = 21624
                    }).Where(x => x.SupersetID != 0).ToList();
                }
                else
                {
                    result = (await GetDbAsync<List<MatchmakingSupersetDTO>>($"SVO/uriStore?path={path}"));
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }
        */

        /*
        /// <summary>
        /// Get player wide stats.
        /// </summary>
        /// <param name="accountId">Account id of player.</param>
        /// <returns></returns>
        public async Task<StatPostDTO> GetPlayerWideStats(int accountId, Dictionary<string, string> stats)
        {
            StatPostDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    var stats = _simulatedAccounts.FirstOrDefault(x => x.AccountId == accountId)?.AccountWideStats;
                    if (stats != null)
                    {
                        result = new StatPostDTO()
                        {
                            AccountId = accountId,
                            Stats = stats
                        };
                    }
                }
                else
                {
                    result = await GetDbAsync<StatPostDTO>($"Stats/getStats?AccountId={accountId}");
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError(e);
            }

            return result;
        }
        */
        #endregion

        #region Key

        public async Task<AppIdDTO[]> GetAppIds()
        {
            AppIdDTO[] results = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    return new AppIdDTO[]
                    {
                        new AppIdDTO()
                        {
                            Name = "All",
                            AppIds = SimulatedAppIdList.ToList()
                        }
                    };
                }
                else
                    results = await GetDbAsync<AppIdDTO[]>($"api/Keys/getAppIds");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return results;
        }

        public async Task<Dictionary<string, string>> GetServerSettings(int appId)
        {
            Dictionary<string, string> result = null;

            try
            {
                if (_settings.SimulatedMode)
                    return new Dictionary<string, string>();
                else
                    result = await GetDbAsync<Dictionary<string, string>>($"api/Keys/getSettings?appId={appId}");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        public async Task SetServerSettings(int appId, Dictionary<string, string> settings)
        {
            try
            {
                if (_settings.SimulatedMode)
                {

                }
                else
                    await PostDbAsync($"api/Keys/setSettings?appId={appId}", settings);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }
        }

        public async Task<ServerFlagsDTO> GetServerFlags()
        {
            ServerFlagsDTO result = null;

            try
            {
                if (_settings.SimulatedMode)
                {
                    return new ServerFlagsDTO()
                    {
                        MaintenanceMode = new MaintenanceDTO()
                        {
                            IsActive = false,
                            FromDt = DateTime.UtcNow - TimeSpan.FromSeconds(10),
                            ToDt = DateTime.UtcNow + TimeSpan.FromSeconds(1)
                        }
                    };
                }
                else
                    result = await GetDbAsync<ServerFlagsDTO>($"api/Keys/getServerFlags");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        #endregion

        #region Auth

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private async Task<AuthenticationResponse> Authenticate(string username, string password)
        {
            AuthenticationResponse result = null;

            try
            {
                result = await PostDbAsync<AuthenticationResponse>($"Account/authenticate", new AuthenticationRequest()
                {
                    AccountName = username,
                    Password = (!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.EncryptCTR(password, _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : password
                });
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }

            return result;
        }

        #endregion

        #region Http

        private async Task<HttpResponseMessage> DeleteDbAsync(string route)
        {
            HttpResponseMessage result = null;

            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(_dbAccessToken))
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", (!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.EncryptCTR(_dbAccessToken, _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : _dbAccessToken);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    try
                    {
                        result = await client.DeleteAsync($"{_settings.DatabaseUrl}/{route}");
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(ex);
                        result = null;
                    }
                }
            }

            return result;
        }

        private async Task<HttpResponseMessage> GetDbAsync(string route)
        {
            HttpResponseMessage result = null;

            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(_dbAccessToken))
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", (!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.EncryptCTR(_dbAccessToken, _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : _dbAccessToken);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    try
                    {
                        result = await client.GetAsync($"{_settings.DatabaseUrl}/{route}");
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(ex);
                        result = null;
                    }
                }
            }

            return result;
        }

        private async Task<T> GetDbAsync<T>(string route)
        {
            T result = default;

            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                using (var client = new HttpClient(handler))
                {
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    try
                    {
                        if (!string.IsNullOrEmpty(_dbAccessToken))
                            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", (!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.EncryptCTR(_dbAccessToken, _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : _dbAccessToken);
                        var response = await client.GetAsync($"{_settings.DatabaseUrl}/{route}");

                        // Deserialize on success
                        if (response.IsSuccessStatusCode)
                            result = JsonConvert.DeserializeObject<T>((!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.DecryptCTR(await response.Content.ReadAsStringAsync(), _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : await response.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(ex);
                        result = default;
                    }
                }
            }

            return result;
        }

        private async Task<HttpResponseMessage> PostDbAsync(string route, string body)
        {
            HttpResponseMessage result = null;

            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(_dbAccessToken))
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", (!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.EncryptCTR(_dbAccessToken, _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : _dbAccessToken);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    try
                    {
                        result = await client.PostAsync($"{_settings.DatabaseUrl}/{route}", string.IsNullOrEmpty(body) ? null : new StringContent(body, Encoding.UTF8, "application/json"));
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(ex);
                        result = null;
                    }
                }
            }

            return result;
        }

        private async Task<HttpResponseMessage> PostDbAsync(string route, object body)
        {
            HttpResponseMessage result = null;

            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(_dbAccessToken))
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", (!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.EncryptCTR(_dbAccessToken, _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : _dbAccessToken);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    try
                    {
                        result = await client.PostAsync($"{_settings.DatabaseUrl}/{route}", new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(ex);
                        result = null;
                    }
                }
            }

            return result;
        }

        private async Task<T> PostDbAsync<T>(string route, object body)
        {
            T result = default;

            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(_dbAccessToken))
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", (!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.EncryptCTR(_dbAccessToken, _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : _dbAccessToken);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    try
                    {
                        var response = await client.PostAsync($"{_settings.DatabaseUrl}/{route}", new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

                        // Deserialize on success
                        if (response.IsSuccessStatusCode)
                            result = JsonConvert.DeserializeObject<T>((!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.DecryptCTR(await response.Content.ReadAsStringAsync(), _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : await response.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(ex);
                        result = default;
                    }
                }
            }

            return result;
        }

        private async Task<HttpResponseMessage> PutDbAsync(string route, string body)
        {
            HttpResponseMessage result = null;

            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(_dbAccessToken))
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", (!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.EncryptCTR(_dbAccessToken, _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : _dbAccessToken);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    try
                    {
                        result = await client.PutAsync($"{_settings.DatabaseUrl}/{route}", string.IsNullOrEmpty(body) ? null : new StringContent(body, Encoding.UTF8, "application/json"));
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(ex);
                        result = null;
                    }
                }
            }

            return result;
        }

        private async Task<HttpResponseMessage> PutDbAsync(string route, object body)
        {
            HttpResponseMessage result = null;

            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(_dbAccessToken))
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", (!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.EncryptCTR(_dbAccessToken, _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : _dbAccessToken);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    try
                    {
                        result = await client.PutAsync($"{_settings.DatabaseUrl}/{route}", new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(ex);
                        result = null;
                    }
                }
            }

            return result;
        }

        private async Task<T> PutDbAsync<T>(string route, object body)
        {
            T result = default;

            using (var handler = new HttpClientHandler())
            {
                handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                handler.ServerCertificateCustomValidationCallback =
                    (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };

                using (var client = new HttpClient(handler))
                {
                    if (!string.IsNullOrEmpty(_dbAccessToken))
                        client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", (!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.EncryptCTR(_dbAccessToken, _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : _dbAccessToken);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    try
                    {
                        var response = await client.PutAsync($"{_settings.DatabaseUrl}/{route}", new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

                        // Deserialize on success
                        if (response.IsSuccessStatusCode)
                            result = JsonConvert.DeserializeObject<T>((!string.IsNullOrEmpty(_settings.DatabaseAccessKey)) ? WebCryptoClass.DecryptCTR(await response.Content.ReadAsStringAsync(), _settings.DatabaseAccessKey, WebCryptoClass.AuthIV) : await response.Content.ReadAsStringAsync());
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(ex);
                        result = default;
                    }
                }
            }

            return result;
        }


        #endregion
    }
}
