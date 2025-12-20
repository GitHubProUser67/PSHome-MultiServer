using CustomLogger;
using MultiServerLibrary.Extension;
using SpaceWizards.HttpListener;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SVO.Games.PS3
{
    public class Warhawk
    {
        public static async Task Warhawk_SVO(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                if (request.Url == null)
                {
                    response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    return;
                }

                string method = request.HttpMethod;

                using (response)
                {
                    switch (request.Url.AbsolutePath)
                    {
                        #region WARHAWK
                        case "/WARHAWK_SVML/index.jsp":

                            switch (method)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        string? languageID = string.Empty;

                                        try
                                        {
                                            languageID = request.QueryString["languageID"];
                                        }
                                        catch (Exception)
                                        {
                                            languageID = "1";
                                        }

                                        string domain = "{domain}";

                                        if (!SVOServerConfiguration.PreferDNSUrls)
                                            await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("Set-Cookie", $"LangID={languageID}; Path=/");
                                        response.Headers.Set("Location", $"http://{domain}:10060/WARHAWK_SVML/index1.jsp");
                                        response.Headers.Set("Content-Type", "text/svml;charset=UTF-8");
                                        response.Headers.Set("Content-Length", "0");
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));

                                        response.StatusCode = (int)System.Net.HttpStatusCode.Redirect;
                                    }
                                    break;
                            }
                            break;

                        case "/WARHAWK_SVML/index1.jsp":

                            switch (method)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml;charset=UTF-8");
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string domain = "warhawk-prod3.svo.online.scea.com";

                                        if (!SVOServerConfiguration.PreferDNSUrls)
                                            await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                        byte[]? uriStore = null;

                                        if (SVOServerConfiguration.SVOHTTPSBypass)
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n" +
                                                "<SVML>\r\n" +
                                                $"    <SET name=\"IP\" IPAddress=\"{request.RemoteEndPoint.Address}\" />    \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"entryURI\" value=\"http://{domain}:10060/WARHAWK_SVML/account/Account_Login.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"homeURI\" value=\"http://{domain}:10060/WARHAWK_SVML/home.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"loginEncryptedURI\" value=\"http://{domain}:10060/WARHAWK_SVML/account/Account_Encrypted_Login_Submit.jsp\" />    \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGameURI\" value=\"http://{domain}:10060/WARHAWK_SVML/game/Game_Create.jsp?gameMode=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGameSubmitURI\" value=\"http://{domain}:10060/WARHAWK_SVML/game/Game_Create_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"finishGameURI\" value=\"http://{domain}:10060/WARHAWK_SVML/game/Game_Finish_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"gamePostBinaryStatsURI\" value=\"http://{domain}:10060/WARHAWK_SVML/game/Game_PostBinaryStats_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGamePlayerURI\" value=\"http://{domain}:10060/WARHAWK_SVML/game/Game_Create_Player_Submit.jsp?SVOGameID=%d&amp;playerSide=%d\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAccountLoginURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Account_Login.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAccountCreateURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Account_Create.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusLobbyListURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Lobby_List.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusChatLobbyURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Chat_Lobby.jsp\" />  \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusChallengePopupURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Challenge_Popup.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAcceptPopupURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Accept_Popup.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tickerStrURL\" value=\"http://{domain}:10060/WARHAWK_SVML/ticker/TickerStr.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"tourLaunchPopupURI\" value=\"http://{domain}:10060/WARHAWK_SVML/tourney/Tourney_AutoLaunch.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tourDataURI\" value=\"http://{domain}:10060/WARHAWK_SVML/tourney/Tourney_CheckIn.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"teamTourneyMatchDataURI\" value=\"http://{domain}:10060/WARHAWK_SVML/teamtourney/TeamTourney_MatchData.jsp?teamTourID=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"teamTourneyForfeitURI\" value=\"http://{domain}:10060/WARHAWK_SVML/teamtourney/TeamTourney_ForfeitTeam_Submit.jsp?teamTourTeamID=%d&amp;teamTourBracketID=%d&amp;teamTourID=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tourForfeitURI\" value=\"http://{domain}:10060/WARHAWK_SVML/tourney/Tourney_Forfeit_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"getLadderMatchDataURI\" value=\"http://{domain}:10060/WARHAWK_SVML/ladder/Ladder_GetMatchData.jsp?ladderMatchID=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"getForfeitLadderMatchURI\" value=\"http://{domain}:10060/WARHAWK_SVML/ladder/Ladder_Forfeit_Submit.jsp?ladderMatchID=%d&amp;clanID=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"downloadPatch\" value=\"http://{domain}:10060/WARHAWK_SVML/download/patchDownload.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"playerStatsURI\" value=\"http://{domain}:10060/WARHAWK_SVML/stats/Stats_GetPlayerStats.jsp?PlayerID=%d&amp;gameMode=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"playerProfileURI\" value=\"http://{domain}:10060/WARHAWK_SVML/profile/Profile_GetPlayerProfile.jsp?PlayerID=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"rankInfoURI\" value=\"http://{domain}:10060/WARHAWK_SVML/stats/Stats_CareerRankInfo.jsp?playerList=\"  /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"downloadVerificationURI\" value=\"http://{domain}:10060/WARHAWK_SVML/commerce/Commerce_VerifySubmit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"purchaseListURI\" value=\"http://{domain}:10060/WARHAWK_SVML/commerce/Commerce_PurchaseList.jsp?categoryID=default\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createVerifiedFileGameURI\" value=\"http://{domain}:10060/WARHAWK_SVML/commerce/Commerce_GameCreatorFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;spectatorPassword=$3\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"joinVerifiedFileGameURI\" value=\"http://{domain}:10060/WARHAWK_SVML/commerce/Commerce_GameJoinerFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;ticket=$3\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"spectateVerifiedFileGameURI\" value=\"http://{domain}:10060/WARHAWK_SVML/commerce/Commerce_GameSpectatorFileVerification.jsp?fileList=$1&amp;spectatorPassword=$2&amp;ticket=$3\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"http://{domain}:10060/WARHAWK_SVML/account/SP_Login_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"SetIgnoreListURI\" value=\"http://{domain}:10060/WARHAWK_SVML/account/SP_UpdateIgnoreList_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"SetUniversePasswordURI\" value=\"http://{domain}:10060/WARHAWK_SVML/account/SP_SetPassword_Submit.jsp\" />\r\n" +
                                                $"    \r\n    <DATA dataType=\"DATA\" name=\"fileServicesGetMetaDataURI\" value=\"http://{domain}:10060/WARHAWK_SVML/fileservices/FileServices_MetaData.jsp\">\r\n" +
                                                "          <ServerParams>\r\n" +
                                                "                        <Param name=\"fileID\" type=\"integer\" />\r\n" +
                                                "                        <Param name=\"fileNameBeginsWith\" type=\"string\" />\r\n" +
                                                "                        <Param name=\"fileMetaDataKey\" type=\"string\" />\r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"fileServicesUpdateMetaDataURI\" value=\"http://{domain}:10060/WARHAWK_SVML/fileservices/FileServices_UpdateMetaData.jsp\">\r\n" +
                                                "          <ServerParams>\r\n" +
                                                "                        <Param name=\"fileID\" type=\"integer\" />\r\n" +
                                                "                        <Param name=\"fileNameBeginsWith\" type=\"string\" />\r\n" +
                                                "                        <Param name=\"fileMetaDataKey\" type=\"string\" />\r\n" +
                                                "                        <Param name=\"fileMetaDataValue\" type=\"string\" />  \r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"fileServicesStatusURI\" value=\"http://{domain}:10060/WARHAWK_SVML/fileservices/FileServices_Status.jsp\"> \r\n" +
                                                "          <ServerParams>\r\n" +
                                                "                        <Param name=\"statusCode\" type=\"integer\" />\r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"fileServicesUploadServletURI\" value=\"http://{domain}:10060/WARHAWK_SVML/fileservices/UploadFileServlet\"> \r\n" +
                                                "          <ServerParams>\r\n" +
                                                "                        <Param name=\"fileNameBeginsWith\" type=\"string\" />\r\n" +
                                                "                        <Param name=\"fileDescription\" type=\"string\" />\r\n" +
                                                "                        <Param name=\"filePermission\" type=\"integer\" />  \r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"fileServicesDownloadServletURI\" value=\"http://{domain}:10060/WARHAWK_SVML/fileservices/DownloadFileServlet\"> \r\n" +
                                                "          <ServerParams>\r\n" +
                                                "                        <Param name=\"fileID\" type=\"integer\" />\r\n" +
                                                "                        <Param name=\"fileNameBeginsWith\" type=\"string\" />\r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                "    \r\n" +
                                                "    \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"awardInsertURI\" value=\"http://{domain}:10060/WARHAWK_SVML/awards/Awards_Insert_Submit.jsp?awardID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"gameeventsEventInfoURI\" value=\"http://{domain}:10060/WARHAWK_SVML/gameevents/GameEvents_EventInfo.jsp?eventID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"spUpdateTicketURI\" value=\"http://{domain}:10060/WARHAWK_SVML/account/SP_UpdateTicket.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"vulgarityFilterURI\" value=\"http://{domain}:10060/WARHAWK_SVML/filter/VulgarityFilter.jsp\">\r\n" +
                                                "          <ServerParams>\r\n" +
                                                "              <Param name=\"text\" type=\"string\" />\r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"fileMetadata\" value=\"http://{domain}:10060/WARHAWK_SVML/commerce/Commerce_FileMetadata.jsp\"/>\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"drmSignatureURI\" value=\"http://{domain}:10060/WARHAWK_SVML/commerce/Commerce_BufferedSignature.jsp\"/>\r\n" +
                                                "    \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"blankSVMLURI\" value=\"http://{domain}:10060/WARHAWK_SVML/test/Test_BlankSVML.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"blankXMLURI\" value=\"http://{domain}:10060/WARHAWK_SVML/test/Test_BlankXML.jsp\"/>\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"statsBlobDownloadURI\" value=\"http://{domain}:10060/WARHAWK_SVML/stats/Stats_BinaryStatsDownload_Submit.jsp\" >\r\n" +
                                                "         <ServerParams>\r\n" +
                                                "            <Param name=\"gameMode\" type=\"integer\" />\r\n" +
                                                "            <Param name=\"accountID\" type=\"integer\" />\r\n" +
                                                "            <Param name=\"blobType\" type=\"integer\" />\r\n" +
                                                "            <Param name=\"start\" type=\"integer\" optional=\"true\" />\r\n" +
                                                "            <Param name=\"length\" type=\"integer\" optional=\"true\" />\r\n" +
                                                "        </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                "    \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"careerLeaderboardURI\" value=\"http://{domain}:10060/WARHAWK_SVML/stats/Stats_CareerLeaderboard.jsp\"> \r\n" +
                                                "          <ServerParams>\r\n\t\t\t" +
                                                "<Param name=\"gameMode\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"start\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"end\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"statsStart\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"statsEnd\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"sortCol\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"sortOrder\" type=\"integer\" />\r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"PSNGameLongDescriptionURI\" value=\"http://{domain}:10060/WARHAWK_SVML/ilovesony/\" />\r\n" +
                                                "    \r\n " +
                                                $"   <DATA dataType=\"DATA\" name=\"heartbeatURI\" value=\"http://{domain}:10060/WARHAWK_SVML/heartbeat/heartbeat.jsp\" />\r\n" +
                                                "    \r\n" +
                                                "    <BROWSER_INIT name=\"init\" />\r\n" +
                                                "</SVML>");
                                        else
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n" +
                                                "<SVML>\r\n" +
                                                $"    <SET name=\"IP\" IPAddress=\"{request.RemoteEndPoint.Address}\" />    \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"entryURI\" value=\"https://{domain}:10061/WARHAWK_SVML/account/Account_Login.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"homeURI\" value=\"http://{domain}:10060/WARHAWK_SVML/home.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"loginEncryptedURI\" value=\"https://{domain}:10061/WARHAWK_SVML/account/Account_Encrypted_Login_Submit.jsp\" />    \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGameURI\" value=\"http://{domain}:10060/WARHAWK_SVML/game/Game_Create.jsp?gameMode=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGameSubmitURI\" value=\"http://{domain}:10060/WARHAWK_SVML/game/Game_Create_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"finishGameURI\" value=\"http://{domain}:10060/WARHAWK_SVML/game/Game_Finish_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"gamePostBinaryStatsURI\" value=\"http://{domain}:10060/WARHAWK_SVML/game/Game_PostBinaryStats_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGamePlayerURI\" value=\"http://{domain}:10060/WARHAWK_SVML/game/Game_Create_Player_Submit.jsp?SVOGameID=%d&amp;playerSide=%d\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAccountLoginURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Account_Login.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAccountCreateURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Account_Create.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusLobbyListURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Lobby_List.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusChatLobbyURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Chat_Lobby.jsp\" />  \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusChallengePopupURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Challenge_Popup.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAcceptPopupURL\" value=\"http://{domain}:10060/WARHAWK_SVML/medius/Medius_Accept_Popup.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tickerStrURL\" value=\"http://{domain}:10060/WARHAWK_SVML/ticker/TickerStr.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"tourLaunchPopupURI\" value=\"http://{domain}:10060/WARHAWK_SVML/tourney/Tourney_AutoLaunch.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tourDataURI\" value=\"http://{domain}:10060/WARHAWK_SVML/tourney/Tourney_CheckIn.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"teamTourneyMatchDataURI\" value=\"http://{domain}:10060/WARHAWK_SVML/teamtourney/TeamTourney_MatchData.jsp?teamTourID=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"teamTourneyForfeitURI\" value=\"http://{domain}:10060/WARHAWK_SVML/teamtourney/TeamTourney_ForfeitTeam_Submit.jsp?teamTourTeamID=%d&amp;teamTourBracketID=%d&amp;teamTourID=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tourForfeitURI\" value=\"http://{domain}:10060/WARHAWK_SVML/tourney/Tourney_Forfeit_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"getLadderMatchDataURI\" value=\"http://{domain}:10060/WARHAWK_SVML/ladder/Ladder_GetMatchData.jsp?ladderMatchID=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"getForfeitLadderMatchURI\" value=\"http://{domain}:10060/WARHAWK_SVML/ladder/Ladder_Forfeit_Submit.jsp?ladderMatchID=%d&amp;clanID=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"downloadPatch\" value=\"http://{domain}:10060/WARHAWK_SVML/download/patchDownload.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"playerStatsURI\" value=\"http://{domain}:10060/WARHAWK_SVML/stats/Stats_GetPlayerStats.jsp?PlayerID=%d&amp;gameMode=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"playerProfileURI\" value=\"http://{domain}:10060/WARHAWK_SVML/profile/Profile_GetPlayerProfile.jsp?PlayerID=%d\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"rankInfoURI\" value=\"http://{domain}:10060/WARHAWK_SVML/stats/Stats_CareerRankInfo.jsp?playerList=\"  /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"downloadVerificationURI\" value=\"http://{domain}:10060/WARHAWK_SVML/commerce/Commerce_VerifySubmit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"purchaseListURI\" value=\"https://{domain}:10061/WARHAWK_SVML/commerce/Commerce_PurchaseList.jsp?categoryID=default\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createVerifiedFileGameURI\" value=\"https://{domain}:10061/WARHAWK_SVML/commerce/Commerce_GameCreatorFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;spectatorPassword=$3\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"joinVerifiedFileGameURI\" value=\"https://{domain}:10061/WARHAWK_SVML/commerce/Commerce_GameJoinerFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;ticket=$3\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"spectateVerifiedFileGameURI\" value=\"https://{domain}:10061/WARHAWK_SVML/commerce/Commerce_GameSpectatorFileVerification.jsp?fileList=$1&amp;spectatorPassword=$2&amp;ticket=$3\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"https://{domain}:10061/WARHAWK_SVML/account/SP_Login_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"SetIgnoreListURI\" value=\"https://{domain}:10061/WARHAWK_SVML/account/SP_UpdateIgnoreList_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"SetUniversePasswordURI\" value=\"https://{domain}:10061/WARHAWK_SVML/account/SP_SetPassword_Submit.jsp\" />\r\n" +
                                                $"    \r\n    <DATA dataType=\"DATA\" name=\"fileServicesGetMetaDataURI\" value=\"http://{domain}:10060/WARHAWK_SVML/fileservices/FileServices_MetaData.jsp\">\r\n" +
                                                "          <ServerParams>\r\n" +
                                                "                        <Param name=\"fileID\" type=\"integer\" />\r\n" +
                                                "                        <Param name=\"fileNameBeginsWith\" type=\"string\" />\r\n" +
                                                "                        <Param name=\"fileMetaDataKey\" type=\"string\" />\r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"fileServicesUpdateMetaDataURI\" value=\"http://{domain}:10060/WARHAWK_SVML/fileservices/FileServices_UpdateMetaData.jsp\">\r\n" +
                                                "          <ServerParams>\r\n" +
                                                "                        <Param name=\"fileID\" type=\"integer\" />\r\n" +
                                                "                        <Param name=\"fileNameBeginsWith\" type=\"string\" />\r\n" +
                                                "                        <Param name=\"fileMetaDataKey\" type=\"string\" />\r\n" +
                                                "                        <Param name=\"fileMetaDataValue\" type=\"string\" />  \r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"fileServicesStatusURI\" value=\"http://{domain}:10060/WARHAWK_SVML/fileservices/FileServices_Status.jsp\"> \r\n" +
                                                "          <ServerParams>\r\n" +
                                                "                        <Param name=\"statusCode\" type=\"integer\" />\r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"fileServicesUploadServletURI\" value=\"http://{domain}:10060/WARHAWK_SVML/fileservices/UploadFileServlet\"> \r\n" +
                                                "          <ServerParams>\r\n" +
                                                "                        <Param name=\"fileNameBeginsWith\" type=\"string\" />\r\n" +
                                                "                        <Param name=\"fileDescription\" type=\"string\" />\r\n" +
                                                "                        <Param name=\"filePermission\" type=\"integer\" />  \r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"fileServicesDownloadServletURI\" value=\"http://{domain}:10060/WARHAWK_SVML/fileservices/DownloadFileServlet\"> \r\n" +
                                                "          <ServerParams>\r\n" +
                                                "                        <Param name=\"fileID\" type=\"integer\" />\r\n" +
                                                "                        <Param name=\"fileNameBeginsWith\" type=\"string\" />\r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                "    \r\n" +
                                                "    \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"awardInsertURI\" value=\"http://{domain}:10060/WARHAWK_SVML/awards/Awards_Insert_Submit.jsp?awardID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"gameeventsEventInfoURI\" value=\"http://{domain}:10060/WARHAWK_SVML/gameevents/GameEvents_EventInfo.jsp?eventID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"spUpdateTicketURI\" value=\"https://{domain}:10061/WARHAWK_SVML/account/SP_UpdateTicket.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"vulgarityFilterURI\" value=\"http://{domain}:10060/WARHAWK_SVML/filter/VulgarityFilter.jsp\">\r\n" +
                                                "          <ServerParams>\r\n" +
                                                "              <Param name=\"text\" type=\"string\" />\r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"fileMetadata\" value=\"https://{domain}:10061/WARHAWK_SVML/commerce/Commerce_FileMetadata.jsp\"/>\r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"drmSignatureURI\" value=\"https://{domain}:10061/WARHAWK_SVML/commerce/Commerce_BufferedSignature.jsp\"/>\r\n" +
                                                "    \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"blankSVMLURI\" value=\"http://{domain}:10060/WARHAWK_SVML/test/Test_BlankSVML.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"blankXMLURI\" value=\"http://{domain}:10060/WARHAWK_SVML/test/Test_BlankXML.jsp\"/>\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"statsBlobDownloadURI\" value=\"http://{domain}:10060/WARHAWK_SVML/stats/Stats_BinaryStatsDownload_Submit.jsp\" >\r\n" +
                                                "         <ServerParams>\r\n" +
                                                "            <Param name=\"gameMode\" type=\"integer\" />\r\n" +
                                                "            <Param name=\"accountID\" type=\"integer\" />\r\n" +
                                                "            <Param name=\"blobType\" type=\"integer\" />\r\n" +
                                                "            <Param name=\"start\" type=\"integer\" optional=\"true\" />\r\n" +
                                                "            <Param name=\"length\" type=\"integer\" optional=\"true\" />\r\n" +
                                                "        </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                "    \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"careerLeaderboardURI\" value=\"http://{domain}:10060/WARHAWK_SVML/stats/Stats_CareerLeaderboard.jsp\"> \r\n" +
                                                "          <ServerParams>\r\n\t\t\t" +
                                                "<Param name=\"gameMode\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"start\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"end\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"statsStart\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"statsEnd\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"sortCol\" type=\"integer\" />\r\n\t\t\t" +
                                                "<Param name=\"sortOrder\" type=\"integer\" />\r\n" +
                                                "          </ServerParams>\r\n" +
                                                "    </DATA>\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"PSNGameLongDescriptionURI\" value=\"http://{domain}:10060/WARHAWK_SVML/ilovesony/\" />\r\n" +
                                                "    \r\n " +
                                                $"   <DATA dataType=\"DATA\" name=\"heartbeatURI\" value=\"http://{domain}:10060/WARHAWK_SVML/heartbeat/heartbeat.jsp\" />\r\n" +
                                                "    \r\n" +
                                                "    <BROWSER_INIT name=\"init\" />\r\n" +
                                                "</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = uriStore.Length;
                                                response.OutputStream.Write(uriStore, 0, uriStore.Length);
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

                        case "/WARHAWK_SVML/account/SP_Login_Submit.jsp":
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
                                            await SVOServerConfiguration.Database.GetAccountByName(acctNameREX, appId).ContinueWith((r) =>
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

                        case "/WARHAWK_SVML/account/Account_Login_Submit.jsp":
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
                                            await SVOServerConfiguration.Database.GetAccountByName(acctNameREX, appId).ContinueWith((r) =>
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
                                        response.AppendHeader("Set-Cookie", $"Sig={sig}; Path=/");


                                        byte[] xmlMessage = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<SVML>\r\n\t" +
                                            "<Login>\r\n\t\t" +
                                            "<status>\r\n\t\t\t" +
                                            "<message>ACCT_LOGIN_SUCCESS</message>\r\n\t\t" +
                                            "</status>\r\n\t\t" +
                                            $"<userName>{acctNameREX}</userName>\r\n\t\t" +
                                            $"<maxLength>{acctNameREX.Length}</maxLength>\r\n\t\t" +
                                            $"<accountID>{accountId}</accountID>\r\n\t" +
                                            $"</Login>\r\n" +
                                            $"</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = xmlMessage.Length;
                                                response.OutputStream.Write(xmlMessage, 0, xmlMessage.Length);
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

                        case "/WARHAWK_SVML/account/Account_Login.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml;charset=UTF-8");
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string domain = "warhawk-prod3.svo.online.scea.com";

                                        if (!SVOServerConfiguration.PreferDNSUrls)
                                            await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                        byte[] sp_Login = Encoding.UTF8.GetBytes($"<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n<SVML>       \r\n\r\n" +
                                            $"    <TEXT name=\"text\" x=\"320\" y=\"30\" align=\"center\" \r\n" +
                                            $"     class=\"TEXT1\">LOGIN</TEXT>\r\n" +
                                            $"             \r\n" +
                                            $"    <RECTANGLE name=\"rect\" x=\"70\" y=\"70\" width=\"500\" height=\"225\" zValue=\"200000.0\"\r\n" +
                                            $"     class=\"RECT1\"/> \r\n" +
                                            $"     \r\n" +
                                            $"    <TEXT name=\"text\" x=\"320\" y=\"50\" align=\"center\" class=\"TEXT2\"></TEXT>\r\n" +
                                            $"    \r\n" +
                                            $"    <FORM name=\"form1\" action=\"http://{domain}:10060/WARHAWK_SVML/account/Account_Login_Submit.jsp\" method=\"POST\">\r\n" +
                                            $"        \r\n" +
                                            $"        <TEXT name=\"text\" x=\"315\" y=\"105\" align=\"right\" class=\"TEXT3\">USER NAME:</TEXT>\r\n" +
                                            $"        \r\n" +
                                            $"        <INPUT type=\"text\" x=\"320\" y=\"100\" width=\"200\" \r\n" +
                                            $"            name=\"userName\" \r\n" +
                                            $"            maxLength=\"16\" \r\n" +
                                            $"            value=\"pheino\"            \r\n" +
                                            $"            class=\"INPUT1\" height=\"25\" selectable=\"default\"/>            \r\n" +
                                            $"        <TEXT name=\"text\" x=\"315\" y=\"145\" align=\"right\" class=\"TEXT3\">PASSWORD:</TEXT>\r\n" +
                                            $"        \r\n" +
                                            $"        <INPUT type=\"password\" x=\"320\" y=\"140\" width=\"200\" height=\"25\" \r\n" +
                                            $"         name=\"passWord\" \r\n" +
                                            $"         maxLength=\"16\"\r\n" +
                                            $"         value=\"aaaaa\"                  \r\n" +
                                            $"         class=\"INPUT1\" selectable=\"true\" down=\"submit1\"/>         \r\n" +
                                            $"        <INPUT type=\"submit\" name=\"submit1\" value=\"LOGIN\" x=\"275\" y=\"180\" width=\"100\" height=\"25\"\r\n" +
                                            $"         fontSize=\"12\" class=\"SUBMIT1\" />\r\n" +
                                            $"        \r\n" +
                                            $"    </FORM>\r\n" +
                                            $"    \r\n" +
                                            $"    <TEXT name=\"text\" x=\"325\" y=\"325\" align=\"center\" \r\n" +
                                            $"     class=\"TEXT1\">IP Address = {request.RemoteEndPoint.Address}</TEXT>\r\n" +
                                            $"     \r\n" +
                                            $"    </SVML>");

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

                        case "/WARHAWK_SVML/medius/Medius_Account_Login.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":
                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));

                                        string domain = "warhawk-prod3.svo.online.scea.com";

                                        if (!SVOServerConfiguration.PreferDNSUrls)
                                            await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                        byte[] xmlMessage = Encoding.UTF8.GetBytes($"<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n<SVML>       \r\n\r\n" +
                                            $"    <PAGEID name=\"pageID_Medius_Account_Login\"/>\r\n    \r\n" +
                                            $"    <TEXT name=\"text\" x=\"320\" y=\"30\" align=\"center\" \r\n" +
                                            $"     class=\"TEXT1\">LOGIN</TEXT>\r\n" +
                                            $"             \r\n" +
                                            $"    <RECTANGLE name=\"rect\" x=\"70\" y=\"70\" width=\"500\" height=\"225\" zValue=\"200000.0\"\r\n" +
                                            $"     class=\"RECT1\"/> \r\n" +
                                            $"    \r\n" +
                                            $"    <TEXT name=\"statusAccountLogin\" x=\"320\" y=\"80\" align=\"center\" \r\n" +
                                            $"    class=\"TEXT1\">statusAccountLogin</TEXT>\r\n" +
                                            $"    \r\n" +
                                            $"    <TEXT name=\"text\" x=\"315\" y=\"105\" align=\"right\" class=\"TEXT3\">USER NAME:</TEXT>\r\n" +
                                            $"    \r\n" +
                                            $"    <INPUT type=\"text\" x=\"320\" y=\"100\" width=\"200\" name=\"userName\" maxLength=\"16\" \r\n" +
                                            $"     class=\"INPUT1\" height=\"25\" value=\"\" selectable=\"default\"/>\r\n" +
                                            $"    \r\n" +
                                            $"    <TEXT name=\"text\" x=\"315\" y=\"145\" align=\"right\" class=\"TEXT3\">PASSWORD:</TEXT>\r\n" +
                                            $"    \r\n" +
                                            $"    <INPUT type=\"password\" x=\"320\" y=\"140\" width=\"200\" height=\"25\" name=\"passWord\" \r\n" +
                                            $"     value=\"\" maxLength=\"16\"\r\n" +
                                            $"     class=\"INPUT1\" selectable=\"true\" down=\"login\"/>   \r\n" +
                                            $"   \r\n" +
                                            $"    <BUTTON name=\"login\" x=\"275\" y=\"180\" width=\"100\" height=\"25\"\r\n" +
                                            $"     fontSize=\"12\" class=\"SUBMIT1\" selectable=\"true\" href=\"\">LOGIN</BUTTON>     \r\n" +
                                            $"    \r\n" +
                                            $"    <MEDIUS_PLUGIN name=\"mediusInitialize\" type=\"INITIALIZE\" authServerIP=\"warhawk-prod3.online.scea.com\" port=\"10075\"/>\r\n\r\n" +
                                            $"    <MEDIUS_PLUGIN name=\"mediusLogin\" type=\"LOGIN\" \r\n" +
                                            $"     userNameField=\"passWord\" userNameLength=\"16\"\r\n" +
                                            $"     passWordField=\"passWord\" passWordLength=\"16\" loginButton=\"login\"/>  \r\n" +
                                            $"    \r\n" +
                                            $"    <BUTTON name=\"button1\" x=\"225\" y=\"250\" width=\"200\" height=\"25\"\r\n" +
                                            $"     align=\"center\" class=\"BUTTON1\" href=\"http://{domain}:10060/WARHAWK_SVML/account/Account_AgeVerification.jsp\">Did this change....CREATE ACCOUNT</BUTTON>\r\n" +
                                            $"    \r\n" +
                                            $"</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = xmlMessage.Length;
                                                response.OutputStream.Write(xmlMessage, 0, xmlMessage.Length);
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

                        case "/WARHAWK_SVML/game/Game_Create_Player_Submit.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));

                                        byte[] GameCreateResponse = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<XML>\r\n\t" +
                                            "<GamePlayer>\r\n\t\t" +
                                            "<status>\r\n\t\t\t" +
                                            "<id>20422</id>\r\n\t\t\t" +
                                            "<message>GAME_CREATE_PLAYER_SUCCESS</message>\r\n\t\t" +
                                            "</status>\r\n\t" +
                                            "</GamePlayer>\r\n" +
                                            "</XML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = GameCreateResponse.Length;
                                                response.OutputStream.Write(GameCreateResponse, 0, GameCreateResponse.Length);
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

                        case "/WARHAWK_SVML/whstats.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));

                                        byte[] whstatsResponse = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<SVML>\r\n\t" +
                                            "<GROUP name=\"group1\" topmostGroup=\"true\">\r\n\t\t" +
                                            "<CHILD tagName=\"groupButton1\" />\r\n\t" +
                                            "</GROUP>\r\n\t" +
                                            "<GROUP name=\"groupButton1\">\r\n\t\t" +
                                            "<CHILD tagName=\"p1button1\" />\r\n\t\t" +
                                            "<CHILD tagName=\"groupOverview\" />\r\n\t" +
                                            "</GROUP>\r\n\t" +
                                            "<GROUP name=\"groupOverview\">\r\n\t\t" +
                                            "<CHILD tagName='p1header1Rect'  />\r\n\t\t" +
                                            "<CHILD tagName='p1header1N'  />\r\n\t" +
                                            "</GROUP>\r\n\t" +
                                            "<TEXT name=\"pagetitle\">1288</TEXT>\r\n\t" +
                                            "<BUTTON name=\"p1button1\" y=\"0\" href=\"NONE\" selectable=\"true\">1011</BUTTON>\r\n\t" +
                                            "<RECTANGLE name='p1header1Rect' x='0'/>\r\n\t" +
                                            "<TEXT name='p1header1N' class='locnamevalue' >1011</TEXT>\r\n" +
                                            "</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = whstatsResponse.Length;
                                                response.OutputStream.Write(whstatsResponse, 0, whstatsResponse.Length);
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

                        default:
                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                            break;

                            #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SVO] - Warhawk_SVO thrown an assertion - {ex}");
                response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }
    }
}
