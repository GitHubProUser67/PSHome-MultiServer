using CustomLogger;
using HttpMultipartParser;
using System.Text;
using System.IO;
using System;
using CastleLibrary.S0ny.XI5;

namespace WebAPIService.GameServices.PSHOME.HTS.Helpers
{
    public class NPTicketSample
    {
        public static string RequestNPTicket(byte[] PostData, string boundary)
        {
            string userid = string.Empty;
            string sessionid = string.Empty;
            string resultString = string.Empty;
            string region = string.Empty;
            byte[] ticketData = null;

            if (PostData != null)
            {
                using (MemoryStream copyStream = new MemoryStream(PostData))
                {
                    foreach (var file in MultipartFormDataParser.Parse(copyStream, boundary).Files)
                    {
                        using (Stream filedata = file.Data)
                        {
                            filedata.Position = 0;

                            // Find the number of bytes in the stream
                            int contentLength = (int)filedata.Length;

                            // Create a byte array
                            byte[] buffer = new byte[contentLength];

                            // Read the contents of the memory stream into the byte array
                            filedata.Read(buffer, 0, contentLength);

                            if (file.FileName == "ticket.bin")
                                ticketData = buffer;

                            filedata.Flush();
                        }
                    }

                    copyStream.Flush();
                }
            }

            if (ticketData != null && ticketData.Length > 188)
            {
                #region Region
                // Extract part of the byte array from the specific index
                byte[] ticketRegion = new byte[4];
                Array.Copy(ticketData, 0x78, ticketRegion, 0, 4);
                #endregion

                // get ticket
                XI5Ticket ticket = XI5Ticket.ReadFromBytes(ticketData);

                // setup username
                string username = ticket.Username;

                // invalid ticket
                if (!ticket.Valid)
                {
                    // log to console
                    LoggerAccessor.LogWarn($"[HTS] - User {username} tried to alter their ticket data");

                    return null;
                }

                // RPCN
                if (ticket.IsSignedByRPCN)
                    LoggerAccessor.LogInfo($"[HTS] - User {username} connected at: {DateTime.Now} and is on RPCN");
                else if (username.EndsWith($"@{XI5Ticket.RPCNSigner}"))
                {
                    LoggerAccessor.LogError($"[HTS] - User {username} was caught using a RPCN suffix while not on it!");

                    return null;
                }
                else
                    LoggerAccessor.LogInfo($"[HTS] - User {username} connected at: {DateTime.Now} and is on PSN");

                return $@"<xml>
                        <npID>{username}</npID>
                        <Environment></Environment>
                        <issuerID>{ticket.IssuerId}</issuerID>
                        <Issued></Issued>
                        <Expires></Expires>
                        <ServiceID>{ticket.ServiceId}</ServiceID>
                        <Region>{Encoding.UTF8.GetString(ticketRegion)}</Region>
                        <Language></Language>
                        <entitlements></entitlements>
                    </xml>";
            }

            return null;
        }
    }
}
