using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.QNetZ.Connection;
using System.Net;
using CustomLogger;
using QuazalServer.QNetZ.DDL;

namespace QuazalServer.RDVServices.GameServices.PS3SparkServices
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
                    plInfo = NetworkPlayers.CreatePlayerInfo(Context);
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

                plInfo = NetworkPlayers.CreatePlayerInfo(Context);

                // Console login not uses Quazal storage, they use a given account to log-in.
                plInfo.PID = NetworkPlayers.GenerateUniqueUint(userName + "a1nPut!");
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

                if (sourcePID == 100) // Quazal guest account.
                    ticketData.pbufResponse = kerberos.ToBuffer(Context.Handler.AccessKey, "h7fyctiuucf");
                else
                    ticketData.pbufResponse = kerberos.ToBuffer(Context.Handler.AccessKey);

                return Result(ticketData);
            }

            return Error(0);
        }
    }
}
