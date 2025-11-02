using CustomLogger;
using HttpMultipartParser;
using System.Text;
using System.IO;
using System;
using NetHasher;
using XI5;
using CastleLibrary.Sony.SSFW;

namespace WebAPIService.GameServices.HELLFIRE.Helpers
{
    public class NPTicket
    {
        public static string RequestNPTicket(byte[] PostData, string boundary, bool cross_save = false)
        {
            string userid = string.Empty;
            string sessionid = string.Empty;
            string resultString = string.Empty;
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
                // Extract the desired portion of the binary data
                byte[] extractedData = new byte[0x63 - 0x54 + 1];

                // Copy it
                Array.Copy(ticketData, 0x54, extractedData, 0, extractedData.Length);

                // Convert 0x00 bytes to 0x20 so we pad as space.
                for (int i = 0; i < extractedData.Length; i++)
                {
                    if (extractedData[i] == 0x00)
                        extractedData[i] = 0x20;
                }

                const string RPCNSigner = "RPCN";

                // get ticket
                XI5Ticket ticket = XI5Ticket.ReadFromBytes(ticketData);

                // setup username
                string username = ticket.Username;

                // invalid ticket
                if (!ticket.Valid)
                {
                    // log to console
                    LoggerAccessor.LogWarn($"[HFGames] - Hellfire : User {username} tried to alter their ticket data");

                    return null;
                }

                const string salt = "H0mETyc00n!";

                // RPCN
                if (ticket.SignatureIdentifier == RPCNSigner)
                {
                    if (!cross_save)
                    {
                        // Convert the modified data to a string
                        resultString = Encoding.ASCII.GetString(extractedData) + RPCNSigner;

                        userid = resultString.Replace(" ", string.Empty);

                        // Calculate the MD5 hash of the result
                        string hash = DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(resultString + salt));

                        // Trim the hash to a specific length
                        hash = hash.Substring(0, 10);

                        // Append the trimmed hash to the result
                        resultString += hash;

                        sessionid = GuidGenerator.SSFWGenerateGuid(hash, resultString);
                    }

                    LoggerAccessor.LogInfo($"[HFGames] - Hellfire : User {username} connected at: {DateTime.Now} and is on RPCN");
                }
                else if (username.EndsWith($"@{RPCNSigner}"))
                {
                    LoggerAccessor.LogError($"[HFGames] - Hellfire : User {username} was caught using a RPCN suffix while not on it!");

                    return null;
                }
                else
                    LoggerAccessor.LogInfo($"[HFGames] - Hellfire : User {username} connected at: {DateTime.Now} and is on PSN");

                if (resultString == string.Empty)
                {
                    // Convert the modified data to a string
                    resultString = Encoding.ASCII.GetString(extractedData);

                    userid = resultString.Replace(" ", string.Empty);

                    // Calculate the MD5 hash of the result
                    string hash = DotNetHasher.ComputeMD5String(Encoding.ASCII.GetBytes(resultString + salt));

                    // Trim the hash to a specific length
                    hash = hash.Substring(0, 14);

                    // Append the trimmed hash to the result
                    resultString += hash;

                    sessionid = GuidGenerator.SSFWGenerateGuid(hash, resultString);
                }

                return $"<response><Thing>{userid};{sessionid}</Thing></response>";
            }

            return null;
        }
    }
}
