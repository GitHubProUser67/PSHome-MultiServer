using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.QNetZ.Connection;
using QuazalServer.QNetZ.DDL;
using System.Net;
using RDVServices;
using CustomLogger;
using Alcatraz.DTO.Helpers;

namespace QuazalServer.RDVServices.GameServices.PCDriverServices
{
    /// <summary>
    /// Authentication service (ticket granting)
    /// </summary>
    [RMCService((ushort)RMCProtocolId.TicketGrantingService)]
    public class TicketGrantingService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult Login(string userName)
        {
            if (Context != null)
            {
                string prudplink = string.IsNullOrWhiteSpace(QuazalServerConfiguration.ServerBindAddress) ? Dns.GetHostName() : QuazalServerConfiguration.ServerBindAddress;
                if (QuazalServerConfiguration.UsePublicIP)
                    prudplink = string.IsNullOrWhiteSpace(QuazalServerConfiguration.ServerPublicBindAddress) ? Dns.GetHostName() : QuazalServerConfiguration.ServerPublicBindAddress;

                PlayerInfo? plInfo = null;

                if (userName == "guest")
                {
                    plInfo = NetworkPlayers.CreatePlayerInfo(Context.Client);
                    plInfo.PID = 100;
                    plInfo.AccountId = userName;
                    plInfo.Name = userName;

                    return Result(new Login(plInfo.PID)
                    {
                        retVal = (int)ErrorCode.Core_NoError,
                        pConnectionData = new RVConnectionData()
                        {
                            m_urlRegularProtocols = new(
                                    "prudps",
                                    prudplink,
                                    new Dictionary<string, int>() {
                                        { "port", Context.Handler.BackendPort },
                                        { "CID", 1 },
                                        { "PID", (int)Context.Client.sPID },
                                        { "sid", 1 },
                                        { "stream", 3 },
                                        { "type", 2 } // Public, not BehindNAT
                                    })
                        },
                        strReturnMsg = string.Empty,
                        pbufResponse = new KerberosTicket(plInfo.PID, Context.Client.sPID, Constants.SessionKey, Constants.TicketData).ToBuffer(Context.Handler.AccessKey, "h7fyctiuucf")
                    });
                }
                else if (userName == "Tracking")
                {
                    plInfo = NetworkPlayers.CreatePlayerInfo(Context.Client);
                    plInfo.PID = 0;
                    plInfo.AccountId = userName;
                    plInfo.Name = userName;

                    return Result(new Login(plInfo.PID)
                    {
                        retVal = (int)ErrorCode.Core_NoError,
                        pConnectionData = new RVConnectionData()
                        {
                            m_urlRegularProtocols = new(
                                    "prudps",
                                    prudplink,
                                    new Dictionary<string, int>() {
                                        { "port", Context.Handler.BackendPort },
                                        { "CID", 1 },
                                        { "PID", (int)Context.Client.sPID },
                                        { "sid", 1 },
                                        { "stream", 3 },
                                        { "type", 2 } // Public, not BehindNAT
                                    })
                        },
                        strReturnMsg = string.Empty,
                        pbufResponse = new KerberosTicket(plInfo.PID, Context.Client.sPID, Constants.SessionKey, Constants.TicketData).ToBuffer(Context.Handler.AccessKey, "JaDe!")
                    });
                }

                plInfo = NetworkPlayers.GetPlayerInfoByUsername(userName);

                if (plInfo != null)
                {
                    if (plInfo.Client != null &&
                        !plInfo.Client.Endpoint.Equals(Context.Client.Endpoint) &&
                        plInfo.Client.TimeSinceLastPacket < Constants.ClientTimeoutSeconds)
                    {
                        LoggerAccessor.LogWarn($"[RMC Authentication] - User login request {userName} was already logged-in - disconnecting...");
                        return Result(new Login(0)
                        {
                            retVal = (uint)ErrorCode.RendezVous_ConcurrentLoginDenied,
                            pConnectionData = new RVConnectionData()
                            {
                                m_urlRegularProtocols = new StationURL("prudp:/")
                            },
                            strReturnMsg = string.Empty,
                            pbufResponse = new byte[] { }
                        });
                    }
                    else
                        NetworkPlayers.DropPlayerInfo(plInfo);
                }

                LoggerAccessor.LogInfo($"[RMC Authentication] - User login request {userName}");

                plInfo = NetworkPlayers.CreatePlayerInfo(Context.Client);

                // Console login not uses Quazal storage, they use a given account to log-in.
                plInfo.PID = NetworkPlayers.GenerateUniqueUint(userName + "!PcF0r3ver");
                plInfo.AccountId = userName;
                plInfo.Name = userName;

                return Result(new Login(plInfo.PID)
                {
                    retVal = (int)ErrorCode.Core_NoError,
                    pConnectionData = new RVConnectionData()
                    {
                        m_urlRegularProtocols = new(
                                    "prudps",
                                    prudplink,
                                    new Dictionary<string, int>() {
                                        { "port", Context.Handler.BackendPort },
                                        { "CID", 1 },
                                        { "PID", (int)Context.Client.sPID },
                                        { "sid", 1 },
                                        { "stream", 3 },
                                        { "type", 2 } // Public, not BehindNAT
                                    })
                    },
                    strReturnMsg = string.Empty,
                    pbufResponse = new KerberosTicket(plInfo.PID, Context.Client.sPID, Constants.SessionKey, Constants.TicketData).ToBuffer(Context.Handler.AccessKey)
                });
            }

            return Error(0);
        }

        /// <summary>
		/// Function where client login is performed by account ID and password
		/// </summary>
		/// <param name="login"></param>
		[RMCMethod(2)]
        public RMCResult LoginEx(string userName, AnyData<UbiAuthenticationLoginCustomData> oExtraData)
        {
            string prudplink = string.IsNullOrWhiteSpace(QuazalServerConfiguration.ServerBindAddress) ? Dns.GetHostName() : QuazalServerConfiguration.ServerBindAddress;
            if (QuazalServerConfiguration.UsePublicIP)
                prudplink = string.IsNullOrWhiteSpace(QuazalServerConfiguration.ServerPublicBindAddress) ? Dns.GetHostName() : QuazalServerConfiguration.ServerPublicBindAddress;

            var rdvConnectionString = new StationURL(
                "prudps",
                prudplink,
                new Dictionary<string, int>() {
                    { "port", Context.Handler.BackendPort },
                    { "CID", 1 },
                    { "PID", (int)Context.Client.sPID },
                    { "sid", 1 },
                    { "stream", 3 },
                    { "type", 2 }	// Public, not BehindNAT
				});

            if (oExtraData.data != null)
            {
                ErrorCode loginCode = ErrorCode.Core_NoError;

                var plInfo = NetworkPlayers.GetPlayerInfoByUsername(userName);

                if (plInfo != null)
                {
                    if (plInfo.Client != null &&
                        !plInfo.Client.Endpoint.Equals(Context.Client.Endpoint) &&
                        plInfo.Client.TimeSinceLastPacket < Constants.ClientTimeoutSeconds)
                    {
                        LoggerAccessor.LogWarn($"[RMC Authentication] - User login request {userName} was already logged-in - disconnecting...");
                        loginCode = ErrorCode.RendezVous_ConcurrentLoginDenied;
                    }
                    else
                        NetworkPlayers.DropPlayerInfo(plInfo);
                }

                LoggerAccessor.LogInfo($"[RMC Authentication] - User login request {userName}");

                var user = DBHelper.GetUserByUserName(Context.Handler.Factory.Item1, oExtraData.data.username);

                if (user != null)
                {
                    if (user.Password == oExtraData.data.password)
                        LoggerAccessor.LogInfo($"[RMC Authentication] - User login request {userName} - success");
                    else
                    {
                        bool passwordCheckResult = false;
                        try
                        {
                            passwordCheckResult = SecurePasswordHasher.Verify($"{user.Id}-{oExtraData.data.password}", user.Password);
                        }
                        catch
                        {
                        }

                        if (passwordCheckResult)
                            LoggerAccessor.LogInfo($"[RMC Authentication] - User login request {userName} - success");
                        else
                        {
                            LoggerAccessor.LogWarn($"[RMC Authentication] - User login request {userName} - invalid password");
                            loginCode = ErrorCode.RendezVous_InvalidPassword;
                        }
                    }
                }
                else
                {
                    LoggerAccessor.LogWarn($"[RMC Authentication] - User login request {userName} - invalid user name");
                    loginCode = ErrorCode.RendezVous_InvalidUsername;
                }

                if (loginCode != ErrorCode.Core_NoError)
                {
                    var loginData = new Login(0)
                    {
                        retVal = (uint)loginCode,
                        pConnectionData = new RVConnectionData()
                        {
                            m_urlRegularProtocols = new StationURL("prudp:/")
                        },
                        strReturnMsg = string.Empty,
                        pbufResponse = new byte[] { }
                    };

                    return Result(loginData);
                }
                else
                {
                    plInfo = NetworkPlayers.CreatePlayerInfo(Context.Client);
                    plInfo.PID = user.Id;
                    plInfo.AccountId = userName;
                    plInfo.Name = oExtraData.data.username;

                    var kerberos = new KerberosTicket(plInfo.PID, Context.Client.sPID, Constants.SessionKey, Constants.TicketData);

                    var loginData = new Login(plInfo.PID)
                    {
                        retVal = (uint)loginCode,
                        pConnectionData = new RVConnectionData()
                        {
                            m_urlRegularProtocols = rdvConnectionString
                        },
                        strReturnMsg = string.Empty,
                        pbufResponse = kerberos.ToBuffer(Context.Handler.AccessKey)
                    };

                    return Result(loginData);
                }
            }
            else
                LoggerAccessor.LogError($"[RMC Authentication] - Error: Unknown Custom Data class '{oExtraData.className}'");

            return Error((int)ErrorCode.RendezVous_ClassNotFound);
        }

        [RMCMethod(3)]
        public RMCResult RequestTicket(uint sourcePID, uint targetPID)
        {
            if (Context != null)
            {
                KerberosTicket kerberos = new(sourcePID, targetPID, Constants.SessionKey, Constants.TicketData);

                TicketData ticketData = new()
                {
                    retVal = (int)ErrorCode.Core_NoError,
                };

                if (sourcePID == 0) // Ubisoft tracker account.
                    ticketData.pbufResponse = kerberos.ToBuffer(Context.Handler.AccessKey, "JaDe!");
                else if (sourcePID == 100) // Quazal guest account.
                    ticketData.pbufResponse = kerberos.ToBuffer(Context.Handler.AccessKey, "h7fyctiuucf");
                else
                    ticketData.pbufResponse = kerberos.ToBuffer(Context.Handler.AccessKey);

                return Result(ticketData);
            }

            return Error(0);
        }
    }
}
