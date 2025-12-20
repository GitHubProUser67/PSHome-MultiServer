using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using RDVServices;

namespace QuazalServer.RDVServices.GameServices.PS3GFRSServices
{
    /// <summary>
    /// Ubi account management service
    /// </summary>
    [RMCService((ushort)RMCProtocolId.UbiAccountManagementService)]
    public class UbiAccountManagementService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult CreateAccount()
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                PlayerInfo playerInfo = Context.Client.PlayerInfo;

                UbiAccount account = new()
                {
                    m_ubi_account_id = playerInfo.AccountId,
                    m_username = playerInfo.UbiAcctName,
                    m_password = playerInfo.UbiPass,
                    m_first_name = playerInfo.Name,
                    m_last_name = string.Empty,
                    m_country_code = playerInfo.UbiCountryCode,
                    m_email = playerInfo.UbiMail,
                    m_preferred_language = playerInfo.UbiLanguageCode,
                    m_gender = 0,
                    m_opt_in = true,
                    m_third_party_opt_in = true,
                    m_status = new UbiAccountStatus()
                    {
                        m_basic_status = 2,
                        m_missing_required_informations = false,
                        m_pending_deactivation = false,
                        m_recovering_password = false
                    },
                    m_date_of_birth = new System.DateTime(1990, 11, 1)
                };

                if (DBHelper.UpdateUbiAccountDataByUserName(Context.Handler.Factory.Item1, playerInfo.Name!, account))
                {
                    DBHelper.UpdateUbiTokensDataByUserName(Context.Handler.Factory.Item1, playerInfo.Name!, 10);
                    return Result(new { ubi_account = account, failed_reasons = new List<ValidationFailureReason>() });
                }

                // TODO, reverse validation reasons!
            }

            return Error(0);
        }

        [RMCMethod(2)]
        public void UpdateAccount()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(3)]
        public RMCResult GetAccount()
        {
            PlayerInfo? playerInfo = Context?.Client.PlayerInfo;

            if (playerInfo != null)
            {
                string? userName = playerInfo.Name;

                if (string.IsNullOrEmpty(userName))
                    return Result(new { exist = false });

                if (userName == "Tracking")
                {
                    return Result(new
                    {
                        account = new UbiAccount()
                        {
                            m_ubi_account_id = playerInfo.AccountId,
                            m_username = playerInfo.Name,
                            m_password = "",
                            m_first_name = "",
                            m_last_name = "",
                            m_country_code = "KZ",
                            m_email = "whatever@dontcare.com",
                            m_preferred_language = "en",
                            m_gender = 0,
                            m_opt_in = true,
                            m_third_party_opt_in = true,
                            m_status = new UbiAccountStatus()
                            {
                                m_basic_status = 2,
                                m_missing_required_informations = false,
                                m_pending_deactivation = false,
                                m_recovering_password = false
                            },
                            m_external_accounts = new List<ExternalAccount>()
                            {
                                new ExternalAccount()
                                {
                                    m_account_type = 11,
                                    m_id = "loh",
                                    m_username = "aabb0"
                                },
                                new ExternalAccount()
                                {
                                    m_account_type = 25,
                                    m_id = "pidr",
                                    m_username = "aabb1"
                                },
                                new ExternalAccount()
                                {
                                    m_account_type = 31,
                                    m_id = "whatev",
                                    m_username = "aabb2"
                                }

                            },
                            m_date_of_birth = new DateTime(1990, 11, 1)
                        },
                        exist = true
                    });
                }
                else
                {
                    UbiAccount? account = DBHelper.GetUbiAccountDataByUserName(Context!.Handler.Factory.Item1, userName);
                    if (account != null)
                        return Result(new { account = account, exist = true });
                }
            }

            return Result(new { exist = false });
        }

        [RMCMethod(4)]
        public RMCResult? LinkAccount(string ubi_account_username, string ubi_account_password)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                UbiAccount? account = DBHelper.GetUbiAccountDataByUserName(Context.Handler.Factory.Item1, ubi_account_username);

                if (account != null && account.m_password == ubi_account_password)
                {
                    Context.Client.PlayerInfo.UbiAcctName = account.m_username;
                    Context.Client.PlayerInfo.UbiPass = account.m_password;
                    Context.Client.PlayerInfo.UbiMail = account.m_email;
                    Context.Client.PlayerInfo.UbiLanguageCode = account.m_preferred_language;
                    Context.Client.PlayerInfo.UbiCountryCode = account.m_country_code;
                    return Error(0);
                }
            }

            return null; // Only way to reject invalid accounts.
        }

        [RMCMethod(5)]
        public RMCResult GetTOS(string country_code, string language_code, bool html_version)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
            {
                Context.Client.PlayerInfo.UbiCountryCode = country_code;
                Context.Client.PlayerInfo.UbiLanguageCode = language_code;
            }

            return Result(new { tos = new TOS(country_code, language_code) });
        }

        [RMCMethod(6)]
        public RMCResult ValidateUsername(string username)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
                Context.Client.PlayerInfo.UbiAcctName = username;

            return Result(new { username_validation = new UsernameValidation() });
        }

        [RMCMethod(7)]
        public RMCResult ValidatePassword(string password, string username)
        {
            if (Context != null && Context.Client.PlayerInfo != null && Context.Client.PlayerInfo.UbiAcctName == username)
                Context.Client.PlayerInfo.UbiPass = password;

            return Result(new { failed_reasons = new List<ValidationFailureReason>() });
        }

        [RMCMethod(8)]
        public RMCResult ValidateEmail(string email)
        {
            if (Context != null && Context.Client.PlayerInfo != null)
                Context.Client.PlayerInfo.UbiMail = email;

            return Result(new { failed_reasons = new List<ValidationFailureReason>() });
        }

        [RMCMethod(9)]
        public void GetCountryList()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(10)]
        public void ForgetPassword()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(11)]
        public RMCResult LookupPrincipalIds(IEnumerable<string> ubiAccountIds)
        {
            var pids = new Dictionary<string, uint>();
            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var usersList = db.Users.Where(x => ubiAccountIds.Contains(x.Username)).ToArray();

                foreach (var usr in usersList)
                {
                    pids[usr.Username] = usr.Id;
                }
            }
            return Result(pids);
        }

        [RMCMethod(12)]
        public RMCResult LookupUbiAccountIDsByPids(IEnumerable<uint> pids)
        {
            var ubiaccountIDs = new Dictionary<uint, string>();

            ubiaccountIDs.Add(1, "Temp");

            /*
            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var usersList = db.Users.Where(x => pids.Contains(x.Id)).ToArray();

                foreach (var usr in usersList)
                {
                    ubiaccountIDs[usr.Id] = usr.PlayerNickName;
                }
            }
            */
            return Result(ubiaccountIDs);
        }

        [RMCMethod(13)]
        public RMCResult LookupUbiAccountIDsByUsernames(IEnumerable<string> Usernames)
        {
            var UbiAccountIDs = new Dictionary<string, string>();

            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var usersList = db.Users.Where(x => Usernames.Contains(x.PlayerNickName)).ToArray();

                foreach (var usr in usersList)
                {
                    UbiAccountIDs[usr.Username] = usr.PlayerNickName;
                }
            }

            return Result(UbiAccountIDs);
        }

        [RMCMethod(14)]
        public RMCResult LookupUsernamesByUbiAccountIDs(IEnumerable<string> UbiAccountIds)
        {
            var Usernames = new Dictionary<string, string>();

            using (var db = DBHelper.GetDbContext(Context.Handler.Factory.Item1))
            {
                var usersList = db.Users.Where(x => UbiAccountIds.Contains(x.Username)).ToArray();

                foreach (var usr in usersList)
                {
                    Usernames[usr.Username] = usr.Username;
                }
            }

            return Result(Usernames);
        }

        [RMCMethod(15)]
        public RMCResult LookupUbiAccountIDsByUsernameSubString(string UsernameSubString)
        {
            var UbiAccountIDs = new Dictionary<string, string>();

            UNIMPLEMENTED();
            return Result(UbiAccountIDs);
        }

        [RMCMethod(16)]
        public void UserHasPlayed()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(17)]
        public void IsUserPlaying()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(18)]
        public void LookupUbiAccountIDsByUsernamesGlobal()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(19)]
        public void LookupUbiAccountIDsByEmailsGlobal()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(20)]
        public void LookupUsernamesByUbiAccountIDsGlobal()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(21)]
        public void GetTOSEx()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(22)]
        public void HasAcceptedLatestTOS()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(23)]
        public void AcceptLatestTOS()
        {
            UNIMPLEMENTED();
        }
    }
}
