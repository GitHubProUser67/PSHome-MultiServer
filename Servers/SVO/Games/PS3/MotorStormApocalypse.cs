using CastleLibrary.Sony.XI5;
using CustomLogger;
using MultiServerLibrary.Extension;
using SpaceWizards.HttpListener;
using System.Text;

namespace SVO
{
    public class MotorStormApocalypse
    {
        public static async Task MSApocalypse_OTG(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                if (request.Url == null)
                {
                    response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    return;
                }

                string? method = request.HttpMethod;

                using (response)
                {
                    switch (request.Url.AbsolutePath)
                    {
                        #region MotorstormApocalypse
                        case "/motorstorm3ps3_xml/account/TicketLoginAction":

                            switch (method)
                            {
                                case "POST":
                                    string signature = string.Empty;

                                    string signatureClass = string.Empty;

                                    string userContext = string.Empty;

                                    string languageId = string.Empty;

                                    string timeZone = string.Empty;

                                    string psnname = string.Empty;

                                    int accountId = -1;

                                    string url = request.Url.ToString();

                                    string[] urlParts = url.Split('?');
                                    string basePath = urlParts[0];
                                    string queryString = urlParts.Length > 1 ? urlParts[1] : string.Empty;

                                    string[] parameters = queryString.Split('&');

                                    foreach (string parameter in parameters)
                                    {
                                        string[] parts = parameter.Split('=');

                                        string key = Uri.UnescapeDataString(parts[0]);
                                        string value = Uri.UnescapeDataString(parts[1]);

                                        if (key == "signature")
                                            signature = value;
                                        else if (key == "signatureClass")
                                            signatureClass = value;
                                        else if (key == "userContext")
                                            userContext = value;
                                        else if (key == "languageId")
                                            languageId = value;
                                        else if (key == "timeZone")
                                            timeZone = value;
                                    }

                                    string? SVOPLATFORM = request.Headers.Get("SVO-Platform");

                                    string? XSCEEWebCoreCountry = request.Headers.Get("X-SCEE-WebCore-Country");

                                    string? XSCEEWebCoreTitle = request.Headers.Get("X-SCEE-WebCore-Title");

                                    string? AcceptLanguage = request.Headers.Get("Accept-Language");

                                    using (MemoryStream ms = new())
                                    {
                                        request.InputStream.CopyTo(ms);

                                        // Reset the memory stream position to the beginning
                                        ms.Position = 0;

                                        // Find the number of bytes in the stream
                                        int contentLength = (int)ms.Length;

                                        // Create a byte array
                                        byte[] buffer = new byte[contentLength];

                                        // Read the contents of the memory stream into the byte array
                                        ms.Read(buffer, 0, contentLength);

                                        // get ticket
                                        XI5Ticket ticket = XI5Ticket.ReadFromBytes(buffer);

                                        // setup username
                                        psnname = ticket.Username;

                                        // invalid ticket
                                        if (!ticket.Valid)
                                        {
                                            // log to console
                                            LoggerAccessor.LogWarn($"[OTG] - MSApocalypse : User {psnname} tried to alter their ticket data");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }

                                        // RPCN
                                        if (ticket.IsSignedByRPCN)
                                        {
                                            LoggerAccessor.LogInfo($"[OTG] - MSApocalypse : User {psnname} connected at: {DateTime.Now} and is on RPCN");

                                            psnname += $"@{XI5Ticket.RPCNSigner}";
                                        }
                                        else if (psnname.EndsWith($"@{XI5Ticket.RPCNSigner}"))
                                        {
                                            LoggerAccessor.LogError($"[OTG] - MSApocalypse : User {psnname} was caught using a RPCN suffix while not on it!");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                            LoggerAccessor.LogInfo($"[OTG] - MSApocalypse : User {psnname} connected at: {DateTime.Now} and is on PSN");

                                        ms.Flush();
                                    }

                                    string langId = "0";

                                    try
                                    {
                                        await SVOServerConfiguration.Database.GetAccountByName(psnname, 22500).ContinueWith((r) =>
                                        {
                                            //Found in database so keep.
                                            langId = request.Url.Query.Substring(94, request.Url.Query.Length - 94);
                                            if (r.Result != null)
                                                accountId = r.Result.AccountId;
                                        });
                                    }
                                    catch (Exception)
                                    {
                                        langId = request.Url.Query.Substring(94, request.Url.Query.Length - 94);
                                        accountId = 0;
                                    }

                                    response.AddHeader("Set-Cookie", $"id=ddb4fac6-f908-33e5-80f9-febd2e2ef58f; Path=/");
                                    response.AppendHeader("Set-Cookie", $"name={(psnname.EndsWith($"@{XI5Ticket.RPCNSigner}") ? psnname.Replace($"@{XI5Ticket.RPCNSigner}", string.Empty) : psnname)}; Path=/");
                                    response.AppendHeader("Set-Cookie", $"authKey=2b8e1723-9e40-41e6-a740-05ddefacfe94; Path=/");
                                    response.AppendHeader("Set-Cookie", $"timeZone=GMT; Path=/");
                                    response.AppendHeader("Set-Cookie", $"signature=ghpE-ws_dBmIY-WNbkCQb1NnamA; Path=/");

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    byte[] TicketLoginActionData = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<SP_Login>\r\n" +
                                            "   <status> \r\n" +
                                            "        <id>1</id> \r\n" +
                                            "        <message>Success</message> \r\n" +
                                            "   </status> \r\n" +
                                            $"  <accountID>{accountId}</accountID>\r\n\t" +
                                            $"  <languageID>{languageId}</languageID>\r\n" +
                                            $"  <userContext>{userContext}</userContext> \r\n" +
                                            "</SP_Login>");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                                    response.SendChunked = true;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = TicketLoginActionData.Length;
                                            response.OutputStream.Write(TicketLoginActionData, 0, TicketLoginActionData.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        case "/motorstorm3ps3_xml/binary/actionList":

                            switch (method)
                            {
                                case "GET":

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    byte[] actionListData = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<!-- Announcements -->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"retrieveAnnounceementText\" value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/announcements/RetrieveAnnouncementTextAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"listCurrentAnnouncements\" value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/announcements/ListCurrentAnnouncementsAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"markAnnouncementRead\" value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/announcements/MarkAnnouncementReadAction\" />\r\n\t\r\n" +
                                            "<!-- Ghosts-->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"uploadGhostData\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/ghost/UploadGhostDataAction\" /> \r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"uploadGhost\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/ghost/uploadGhostData\" />\r\n\t<DATA dataType=\"URI\" name=\"downloadGhostData\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/ghost/DownloadGhostDataAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"downloadGhost\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/ghost/downloadGhostData\" />\r\n\r\n" +
                                            "<!-- Playlists -->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"getPlaylists\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/playlist/GetPlaylistsAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"getPlaylist\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/playlist/GetPlaylistAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"generateNextGame\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/playlist/GenerateNextGameAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"joinPlaylist\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/playlist/JoinPlaylistAction\" />\r\n\r\n" +
                                            "<!-- Pictures -->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"downloadPicture\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/pictures/DownloadPictureAction\" />\r\n\r\n" +
                                            "<!-- Lobbies -->\r\n\t\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"findLobbies\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/lobbies/FindLobbiesAction\" />\r\n\t\r\n" +
                                            "<!-- Vehicle Customizations -->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"uploadVehicleCustomisationData\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/customisations/UploadVehicleCustomisationDataAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"downloadVehicleCustomisationData\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/customisations/customisations/DownloadVehicleCustomisationDataAction\" />\r\n\r\n" +
                                            "<!-- Gamemode -->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"getGameModes\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/gamemode/GetGameModesAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"getGameModesAttributes\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/gamemode/GetGameModesAttributesAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"uploadCustomGameMode\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/gamemode/UploadGameModeFileAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"downloadCustomGameMode\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/gamemode/DownloadGameModeFileAction\" />\r\n\r\n" +
                                            "<!-- Game Configuration-->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"getAvailableGameConfiguration\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/gameconfig/GetAvailableGameConfigurationAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"getGameConfigurationyName\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/gameconfig/GetGameConfigurationyNameAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"createOrUpdateGameConfiguration\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/gameconfig/CreateOrUpdateGameConfigurationAction\" />\r\n" +
                                            "<!-- Game -->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"createGame\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/game/CreateGameAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"leaveGame\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/game/LeaveGameAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"updateGame\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/game/UpdateGameAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"listGames\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/game/ListGamesAction\" />\r\n\t\r\n" +
                                            "<!-- Friends -->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"getFriends\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/friends/GetFriendsAction\" />\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"syncFriends\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/friends/SyncFriendsAction\" />\r\n\t\r\n" +
                                            "<!-- Account -->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"ticketLogin\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/account/TicketLoginAction\" />\r\n" +
                                            "<!-- Trophies -->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"syncTrophies\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/trophies/SyncTrophiesAction\" />\r\n\t\r\n" +
                                            "<!-- Profile -->\r\n\t\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"verifyProfileVersion\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/profile/VerifyProfileVersionAction\" />\r\n" +
                                            "<!-- Misc -->\r\n\t" +
                                            "<DATA dataType=\"URI\" name=\"downloadBinary\"value=\"http://motorstorm3ps3.ws.online.scee.com:10060/motorstorm3ps3_xml/binary/DownloadBinaryAction\" />");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                                    response.SendChunked = true;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = actionListData.Length;
                                            response.OutputStream.Write(actionListData, 0, actionListData.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        case "/motorstorm3ps3_xml/friends/SyncFriendsAction":

                            switch (method)
                            {
                                case "GET":

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    byte[] SyncFriendsActionData = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<Friends><status value=\"0\"/></Friends>");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                                    response.SendChunked = true;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = SyncFriendsActionData.Length;
                                            response.OutputStream.Write(SyncFriendsActionData, 0, SyncFriendsActionData.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        case "/motorstorm3ps3_xml/playlist/GetPlaylistsAction":

                            switch (method)
                            {
                                case "GET":

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    byte[] GetPlaylistsActionData = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<PlayLists>\r\n" +
                                            "<status value=\"0\"/>\r\n" +
                                            "<PlayListId>1</PlayListId>\r\n" +
                                            "</PlayLists>");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                                    response.SendChunked = true;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = GetPlaylistsActionData.Length;
                                            response.OutputStream.Write(GetPlaylistsActionData, 0, GetPlaylistsActionData.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        default:
                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                            break;
                            #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[OTG] - MSApocalypse_OTG thrown an assertion - {ex}");
                response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }
    }
}
