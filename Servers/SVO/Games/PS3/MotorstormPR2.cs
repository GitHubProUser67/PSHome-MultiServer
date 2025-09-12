using CustomLogger;
using MultiServerLibrary.Extension;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using HttpListenerRequest = SpaceWizards.HttpListener.HttpListenerRequest;
using HttpListenerResponse = SpaceWizards.HttpListener.HttpListenerResponse;

namespace SVO.Games.PS3
{
    public class MotorstormPR2
    {
        public static async Task MotorStormPR_SVO(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string? method = request.HttpMethod;

                using (response)
                {
                    switch (request.Url.AbsolutePath)
                    {
                        #region MotorStorm 2
                        case "/MOTORSTORM2PS3_SVML/index.jsp":
                            {
                                switch (request.HttpMethod)
                                {
                                    case "GET":
                                        HttpListenerRequest req = request;
                                        HttpListenerResponse resp = response;
                                        resp.Headers.Set("Content-Type", "text/xml");

                                        string? clientMac = req.Headers.Get("X-SVOMac");

                                        string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                        if (string.IsNullOrEmpty(serverMac))
                                        {
                                            response.StatusCode = (int)HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                        {
                                            resp.Headers.Set("X-SVOMac", serverMac);

                                            string domain = "motorstorm2ps3.svo.online.scee.com";

                                            if (!SVOServerConfiguration.PreferDNSUrls)
                                                await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                            byte[]? uriStore = null;
                                            if (SVOServerConfiguration.SVOHTTPSBypass)
                                            {
                                                uriStore = Encoding.UTF8.GetBytes($@"<?xml version=""1.0"" encoding=""UTF-8""?> 
                                                <XML> 
                                                 <URL_List> 
                                                  <!-- SVO Actions --> 
                                                   <eula text=""Test""/> 
   
                                                   <Data dataType=""DATA"" name=""SVOActionInitialise"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/init.jsp"" /> 
                                                   <Data dataType=""DATA"" country=""US"" name=""getRegion"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/regionAction.jsp"" /> 
   
                                                   <Data dataType=""DATA"" name=""SVOActionMonitor"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/ActionMonitor.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""poststats"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/stats/stats.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""SVOActionGetGhost"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/ghost/ghost.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""SVOActionGetLeaderboard"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/leaderboard/leaderboard.jsp"" /> 
                                                   <Data dataType=""DATA"" country=""US"" langaugeCode=""1"" name=""getEula"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/actionEula.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""eula"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/actionEula.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""SVOActionGetLocation"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/region/region.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""AnnouncementTxt"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/announcement/announcement.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""region"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/login/login.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""login"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/login/login.jsp"" /> 
    
                                                   <Data dataType=""DATA"" name=""announcementsURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/announcements/News.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""leaderboardsListURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/leaderboard/Stat_Leaderboard.jsp"" /> 
    
    
                                                   <DATA dataType=""URI"" name=""ticketLogin"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_Login.jsp"" /> 
    
                                                  <!-- Normal URIs--> 
                                                   <DATA dataType=""URI"" name=""login"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_Login.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""spUpdateTicketURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_UpdateTicket.jsp"" /> 
    
                                                   <DATA dataType=""URI"" name=""entryURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/Account_Login.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""loginEncryptedURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/Account_Encrypted_Login_Submit.jsp"" />    
                                                   <DATA dataType=""URI"" name=""loginEncryptedURL"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/Account_Encrypted_Login_Submit.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""createGameURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/game/Game_Create.jsp?gameMode=%d"" /> 
                                                   <DATA dataType=""URI"" name=""createGameSubmitURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/game/Game_Create_Submit.jsp"" />  
                                                   <DATA dataType=""URI"" name=""finishGameURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/game/Game_Finish_Submit.jsp"" /> 
    
                                                   <DATA dataType=""URI"" name=""gamePostBinaryStatsURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/game/Game_PostBinaryStats_Submit.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""createGamePlayerURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/game/Game_Create_Player_Submit.jsp?SVOGameID=%d&amp;playerSide=%d"" /> 
                                                   <DATA dataType=""URI"" name=""mediusAccountLoginURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Account_Login.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""mediusAccountCreateURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Account_Create.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""mediusLobbyListURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Lobby_List.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""mediusChatLobbyURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Chat_Lobby.jsp"" />   
                                                   <DATA dataType=""URI"" name=""mediusChallengePopupURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Challenge_Popup.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""mediusAcceptPopupURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Accept_Popup.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""tickerStrURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/ticker/TickerStr.jsp"" /> 
                                                   <DATA dataType=""DATA"" name=""getLadderMatchDataURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/ladder/Ladder_GetMatchData.jsp?ladderMatchID=%d"" /> 
                                                   <DATA dataType=""URI"" name=""getForfeitLadderMatchURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/ladder/Ladder_Forfeit_Submit.jsp?ladderMatchID=%d&amp;clanID=%d"" /> 
                                                   <DATA dataType=""URI"" name=""downloadPatch"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/download/patchDownload.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""playerStatsURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/stats/Stats_GetPlayerStats.jsp?PlayerID=%d&amp;gameMode=%d"" /> 
                                                   <DATA dataType=""URI"" name=""playerProfileURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/profile/Profile_GetPlayerProfile.jsp?PlayerID=%d"" /> 
                                                   <DATA dataType=""URI"" name=""rankInfoURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/stats/Stats_CareerRankInfo.jsp?playerList=""  />  
                                                   <DATA dataType=""URI"" name=""downloadVerificationURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/commerce/Commerce_VerifySubmit.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""purchaseListURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/commerce/Commerce_PurchaseList.jsp?categoryID=default"" /> 
                                                   <DATA dataType=""URI"" name=""createVerifiedFileGameURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/commerce/Commerce_GameCreatorFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;spectatorPassword=$3"" /> 
                                                   <DATA dataType=""URI"" name=""joinVerifiedFileGameURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/commerce/Commerce_GameJoinerFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;ticket=$3"" /> 
                                                   <DATA dataType=""URI"" name=""spectateVerifiedFileGameURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/commerce/Commerce_GameSpectatorFileVerification.jsp?fileList=$1&amp;spectatorPassword=$2&amp;ticket=$3"" /> 
                                                   <DATA dataType=""URI"" name=""TicketLoginURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_Login_Submit.jsp"" /> 
    
                                                   <DATA dataType=""URI"" name=""SetBuddyListURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_UpdateBuddyList_Submit.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""SetIgnoreListURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_UpdateIgnoreList_Submit.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""SetUniversePasswordURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_SetPassword_Submit.jsp=%d?passWord=%d"" /> 
 
 
                                                   <DATA dataType=""URI"" name=""homeURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/home.jsp"" />     
                                                 </URL_List> 
                                                </XML>"
                                                );
                                            }
                                            else
                                            {
                                                uriStore = Encoding.UTF8.GetBytes($@"<?xml version=""1.0"" encoding=""UTF-8""?> 
                                                <XML> 
                                                 <URL_List> 
                                                  <!-- SVO Actions --> 
                                                   <eula text=""Test""/> 
   
                                                   <Data dataType=""DATA"" name=""SVOActionInitialise"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/init.jsp"" /> 
                                                   <Data dataType=""DATA"" country=""US"" name=""getRegion"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/regionAction.jsp"" /> 
   
                                                   <Data dataType=""DATA"" name=""SVOActionMonitor"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/ActionMonitor.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""poststats"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/stats/stats.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""SVOActionGetGhost"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/ghost/ghost.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""SVOActionGetLeaderboard"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/leaderboard/leaderboard.jsp"" /> 
                                                   <Data dataType=""DATA"" country=""US"" langaugeCode=""1"" name=""getEula"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/actionEula.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""eula"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/actionEula.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""SVOActionGetLocation"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/region/region.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""AnnouncementTxt"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/announcement/announcement.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""region"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/login/login.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""login"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/actions/login/login.jsp"" /> 
    
                                                   <Data dataType=""DATA"" name=""announcementsURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/announcements/News.jsp"" /> 
                                                   <Data dataType=""DATA"" name=""leaderboardsListURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/leaderboard/Stat_Leaderboard.jsp"" /> 
    
    
                                                   <DATA dataType=""URI"" name=""ticketLogin"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_Login.jsp"" /> 
    
                                                  <!-- Normal URIs--> 
                                                   <DATA dataType=""URI"" name=""login"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_Login.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""spUpdateTicketURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_UpdateTicket.jsp"" /> 
    
                                                   <DATA dataType=""URI"" name=""entryURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/Account_Login.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""loginEncryptedURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/Account_Encrypted_Login_Submit.jsp"" />    
                                                   <DATA dataType=""URI"" name=""loginEncryptedURL"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/Account_Encrypted_Login_Submit.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""createGameURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/game/Game_Create.jsp?gameMode=%d"" /> 
                                                   <DATA dataType=""URI"" name=""createGameSubmitURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/game/Game_Create_Submit.jsp"" />  
                                                   <DATA dataType=""URI"" name=""finishGameURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/game/Game_Finish_Submit.jsp"" /> 
    
                                                   <DATA dataType=""URI"" name=""gamePostBinaryStatsURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/game/Game_PostBinaryStats_Submit.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""createGamePlayerURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/game/Game_Create_Player_Submit.jsp?SVOGameID=%d&amp;playerSide=%d"" /> 
                                                   <DATA dataType=""URI"" name=""mediusAccountLoginURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Account_Login.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""mediusAccountCreateURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Account_Create.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""mediusLobbyListURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Lobby_List.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""mediusChatLobbyURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Chat_Lobby.jsp"" />   
                                                   <DATA dataType=""URI"" name=""mediusChallengePopupURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Challenge_Popup.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""mediusAcceptPopupURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/medius/Medius_Accept_Popup.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""tickerStrURL"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/ticker/TickerStr.jsp"" /> 
                                                   <DATA dataType=""DATA"" name=""getLadderMatchDataURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/ladder/Ladder_GetMatchData.jsp?ladderMatchID=%d"" /> 
                                                   <DATA dataType=""URI"" name=""getForfeitLadderMatchURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/ladder/Ladder_Forfeit_Submit.jsp?ladderMatchID=%d&amp;clanID=%d"" /> 
                                                   <DATA dataType=""URI"" name=""downloadPatch"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/download/patchDownload.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""playerStatsURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/stats/Stats_GetPlayerStats.jsp?PlayerID=%d&amp;gameMode=%d"" /> 
                                                   <DATA dataType=""URI"" name=""playerProfileURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/profile/Profile_GetPlayerProfile.jsp?PlayerID=%d"" /> 
                                                   <DATA dataType=""URI"" name=""rankInfoURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/stats/Stats_CareerRankInfo.jsp?playerList=""  />  
                                                   <DATA dataType=""URI"" name=""downloadVerificationURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/commerce/Commerce_VerifySubmit.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""purchaseListURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/commerce/Commerce_PurchaseList.jsp?categoryID=default"" /> 
                                                   <DATA dataType=""URI"" name=""createVerifiedFileGameURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/commerce/Commerce_GameCreatorFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;spectatorPassword=$3"" /> 
                                                   <DATA dataType=""URI"" name=""joinVerifiedFileGameURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/commerce/Commerce_GameJoinerFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;ticket=$3"" /> 
                                                   <DATA dataType=""URI"" name=""spectateVerifiedFileGameURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/commerce/Commerce_GameSpectatorFileVerification.jsp?fileList=$1&amp;spectatorPassword=$2&amp;ticket=$3"" /> 
                                                   <DATA dataType=""URI"" name=""TicketLoginURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_Login_Submit.jsp"" /> 
    
                                                   <DATA dataType=""URI"" name=""SetBuddyListURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_UpdateBuddyList_Submit.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""SetIgnoreListURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_UpdateIgnoreList_Submit.jsp"" /> 
                                                   <DATA dataType=""URI"" name=""SetUniversePasswordURI"" value=""https://{domain}:10060/MOTORSTORM2PS3_XML/account/SP_SetPassword_Submit.jsp=%d?passWord=%d"" /> 
 
 
                                                   <DATA dataType=""URI"" name=""homeURI"" value=""http://{domain}:10060/MOTORSTORM2PS3_XML/home.jsp"" />     
                                                 </URL_List> 
                                                <XML>"
                                                );
                                            }

                                            resp.OutputStream.Write(uriStore);
#if DEBUG
                                            LoggerAccessor.LogInfo($"Start URIStore for Resistance: Fall of Man SENT!");
#endif
                                        }
                                        break;
                                }
                                break;
                            }


                        case "/MOTORSTORM2PS3_XML/account/SP_Login.jsp":
                            switch (request.HttpMethod)
                            {
                                case "POST":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        int appId = Convert.ToInt32(HttpUtility.ParseQueryString(request.Url.Query).Get("applicationID"));

                                        if (!request.HasEntityBody)
                                        {
                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }

                                        response.Headers.Set("Content-Type", "text/xml");

                                        string s = string.Empty;

                                        // Get the data from the HTTP stream
                                        using (StreamReader reader = new(request.InputStream, request.ContentEncoding))
                                        {
                                            // Convert the data to a string and display it on the console.
                                            s = reader.ReadToEnd();
                                            reader.Close();
                                        }

                                        byte[] bytes = Encoding.ASCII.GetBytes(s);
                                        int AcctNameLen = Convert.ToInt32(bytes.GetValue(81));

                                        string acctName = s.Substring(82, 32);

                                        string acctNameREX = Regex.Replace(acctName, @"[^a-zA-Z0-9]+", string.Empty);

                                        LoggerAccessor.LogInfo($"Logging user {acctNameREX} into SVO...\n");

                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? sig = HttpUtility.ParseQueryString(request.Url.Query).Get("sig");

                                        int accountId = -1;

                                        string langId = "0";

                                        try
                                        {
                                            await SVOServerConfiguration.Database?.GetAccountByName(acctNameREX, appId).ContinueWith((r) =>
                                            {
                                                //Found in database so keep.
                                                langId = request.Url.Query[94..];
                                                if (r.Result != null)
                                                    accountId = r.Result.AccountId;
                                            });
                                        }
                                        catch (Exception)
                                        {
                                            langId = request.Url.Query[94..];
                                            accountId = 0;
                                        }

                                        response.AddHeader("Set-Cookie", $"LangID={langId}; Path=/");
                                        response.AppendHeader("Set-Cookie", $"AcctID={accountId}; Path=/");
                                        response.AppendHeader("Set-Cookie", $"NPCountry=us; Path=/");
                                        response.AppendHeader("Set-Cookie", $"ClanID=-1; Path=/");
                                        response.AppendHeader("Set-Cookie", $"AuthKeyTime=03-31-2023 16:03:41; Path=/");
                                        response.AppendHeader("Set-Cookie", $"NPLang=1; Path=/");
                                        response.AppendHeader("Set-Cookie", $"ModerateMode=false; Path=/");
                                        response.AppendHeader("Set-Cookie", $"TimeZone=PST; Path=/");
                                        response.AppendHeader("Set-Cookie", $"ClanID=-1; Path=/");
                                        response.AppendHeader("Set-Cookie", $"NPContentRating=201326592; Path=/");
                                        response.AppendHeader("Set-Cookie", $"AuthKey=nRqnf97f~UaSANLErurJIzq9GXGWqWCADdA3TfqUIVXXisJyMnHsQ34kA&C^0R#&~JULZ7xUOY*rXW85slhQF&P&Eq$7kSB&VBtf`V8rb^BC`53jGCgIT; Path=/");
                                        response.AppendHeader("Set-Cookie", $"AcctName={acctNameREX}; Path=/");
                                        response.AppendHeader("Set-Cookie", $"OwnerID=-255; Path=/");
                                        response.AppendHeader("Set-Cookie", $"Sig={sig}==; Path=/");

                                        byte[] sp_Login = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<XML>\r\n" +
                                            "    <SP_Login>\r\n" +
                                            "        <status>\r\n" +
                                            "            <id>20600</id>\r\n" +
                                            "            <message>ACCT_LOGIN_SUCCESS</message>\r\n" +
                                            "        </status>\r\n" +
                                            $"       <accountID>{accountId}</accountID>\r\n" +
                                            "        <userContext>0</userContext>\r\n" +
                                            "    </SP_Login>\r\n" +
                                            "</XML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = sp_Login.Length;
                                                response.OutputStream.Write(sp_Login, 0, sp_Login.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }

                                    break;
                            }

                            break;


                        case "/MOTORSTORM2PS3_SVML/getEula":
                            {
                                switch (request.HttpMethod)
                                {
                                    case "GET":
                                        HttpListenerRequest req = request;
                                        HttpListenerResponse resp = response;
                                        resp.Headers.Set("Content-Type", "text/xml");

                                        string? clientMac = req.Headers.Get("X-SVOMac");

                                        string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                        if (string.IsNullOrEmpty(serverMac))
                                        {
                                            response.StatusCode = (int)HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                        {
                                            resp.Headers.Set("X-SVOMac", serverMac);

                                            byte[] getEula = Encoding.UTF8.GetBytes(@"<?xml version=""1.0"" encoding=""UTF-8""?> 
                                                 <XML>
	                                                <eula>
		                                                <text>Test</text>
		                                                <accepted>true</accepted>
	                                                </eula>
                                                </XML>");

                                            resp.OutputStream.Write(getEula);
#if DEBUG
                                            LoggerAccessor.LogInfo($"Start getEula for Resistance: Fall of Man SENT!");
#endif
                                        }
                                        break;
                                }
                                break;
                            }

                        case "/MOTORSTORM2PS3_SVML/getAnnouncement":
                            {
                                switch (request.HttpMethod)
                                {
                                    case "GET":
                                        HttpListenerRequest req = request;
                                        HttpListenerResponse resp = response;
                                        resp.Headers.Set("Content-Type", "text/xml");

                                        string? clientMac = req.Headers.Get("X-SVOMac");

                                        string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                        if (string.IsNullOrEmpty(serverMac))
                                        {
                                            response.StatusCode = (int)HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                        {
                                            resp.Headers.Set("X-SVOMac", serverMac);

                                            byte[] getAnnouncement = Encoding.UTF8.GetBytes(@"<?xml version=""1.0"" encoding=""UTF-8""?> 
                                                 <XML>
	                                                <AnnouncementTxt>
		                                                <msg>Test Announcement</msg>
	                                                </AnnouncementTxt>
                                                </XML>");

                                            resp.OutputStream.Write(getAnnouncement);
#if DEBUG
                                            LoggerAccessor.LogInfo($"Start getEula for Resistance: Fall of Man SENT!");
#endif
                                        }
                                        break;
                                }
                                break;
                            }

                        default:
                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                            break;

                            #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SVO] - Starhawk_SVO thrown an assertion - {ex}");
                response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }
    }
}
