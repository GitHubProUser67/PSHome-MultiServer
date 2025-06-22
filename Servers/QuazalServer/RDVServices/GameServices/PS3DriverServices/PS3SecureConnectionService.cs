using CustomLogger;
using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.DDL;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.QNetZ.Connection;
using NetworkLibrary.Extension;
using XI5;

namespace QuazalServer.RDVServices.GameServices.PS3DriverServices
{
    /// <summary>
	/// Secure connection service protocol
	/// </summary>
	[RMCService((ushort)RMCProtocolId.SecureConnectionService)]
    public class PS3SecureConnectionService : RMCServiceBase
    {
        private static readonly byte[] TicketVersion = new byte[] { 0x21, 0x01, 0x00, 0x00 };

        [RMCMethod(1)]
        public RMCResult? Register(List<string> vecMyURLs)
        {
            if (Context != null)
            {
                // change address
                StationURL rdvConnectionUrl = new(vecMyURLs.Last().ToString())
                {
                    Address = Context.Client.Endpoint.Address.ToString()
                };
                rdvConnectionUrl["type"] = 3;

                RegisterResult result = new()
                {
                    pidConnectionID = Context.Client.PlayerInfo?.RVCID ?? 0,
                    retval = (int)ErrorCode.Core_NoError,
                    urlPublic = rdvConnectionUrl
                };

                return Result(result);
            }

            return null;
        }

        [RMCMethod(2)]
        public void RequestConnectionData()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(3)]
        public void RequestUrls()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(4)]
        public RMCResult RegisterEx(ICollection<StationURL> vecMyURLs, AnyData<SonyNPTicket> hCustomData)
        {
            if (hCustomData.data != null && hCustomData.data.ticket != null && hCustomData.data.ticket.data != null && hCustomData.data.ticket.length > 188 && Context != null && Context.Client.PlayerInfo != null)
            {
                const string RPCNSigner = "RPCN";

                // change address
                StationURL rdvConnectionUrl = new(vecMyURLs.Last().ToString())
                {
                    Address = Context.Client.Endpoint.Address.ToString()
                };
                rdvConnectionUrl["type"] = 3;

                // get ticket
                XI5Ticket ticket = XI5Ticket.ReadFromBytes(ByteUtils.CombineByteArray(TicketVersion, hCustomData.data.ticket.data));

                // setup username
                string username = ticket.Username;

                // invalid ticket
                if (!ticket.Valid)
                {
                    // log to console
                    LoggerAccessor.LogWarn($"[PS3SecureConnectionService] - User {username} tried to alter their ticket data");

                    return Result(new RegisterResult()
                    {
                        pidConnectionID = Context.Client.PlayerInfo.RVCID,
                        retval = (int)ErrorCode.Core_AccessDenied,
                        urlPublic = rdvConnectionUrl
                    });
                }

                // RPCN
                if (ticket.SignatureIdentifier == RPCNSigner)
                    LoggerAccessor.LogInfo($"[PS3SecureConnectionService] - User {username} connected at: {DateTime.Now} and is on RPCN");
                else if (username.EndsWith($"@{RPCNSigner}"))
                {
                    LoggerAccessor.LogError($"[PS3SecureConnectionService] - User {username} was caught using a RPCN suffix while not on it!");

                    return Result(new RegisterResult()
                    {
                        pidConnectionID = Context.Client.PlayerInfo.RVCID,
                        retval = (int)ErrorCode.Core_AccessDenied,
                        urlPublic = rdvConnectionUrl
                    });
                }
                else
                    LoggerAccessor.LogInfo($"[PS3SecureConnectionService] - User {username} connected at: {DateTime.Now} and is on PSN");

                return Result(new RegisterResult()
                {
                    pidConnectionID = Context.Client.PlayerInfo.RVCID,
                    retval = (int)ErrorCode.Core_NoError,
                    urlPublic = rdvConnectionUrl
                });
            }
            else
                LoggerAccessor.LogError($"[RMC Secure] Error: Invalid XI5 Request received or not connected");

            return Error((int)ErrorCode.RendezVous_ClassNotFound);
        }

        [RMCMethod(5)]
        public void TestConnectivity()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(6)]
        public void UpdateURLs()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(7)]
        public void ReplaceURL()
        {
            UNIMPLEMENTED();
        }

        [RMCMethod(8)]
        public void SendReport()
        {
            UNIMPLEMENTED();
        }
    }
}
