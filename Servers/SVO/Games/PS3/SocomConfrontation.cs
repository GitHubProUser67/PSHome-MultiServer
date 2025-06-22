using CustomLogger;
using NetworkLibrary.Extension;
using SpaceWizards.HttpListener;
using System.Text;
using System.Web;
using XI5;

namespace SVO.Games.PS3
{
    public class SocomConfrontation
    {
        public static async Task SocomConfrontation_SVO(HttpListenerRequest request, HttpListenerResponse response)
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
                        #region SOCOMCONFRONTATION
                        case "/CONFRONTATION_XML/index.jsp":

                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        string? language = request.Headers.Get("Accept-Language");

                                        int languageID = 1;

                                        try
                                        {
                                            string? parsed = HttpUtility.ParseQueryString(request.Url.Query).Get("languageID");

                                            if (parsed != null)
                                                languageID = int.Parse(parsed);
                                        }
                                        catch (Exception)
                                        {
                                            languageID = 1;
                                        }

                                        response.Headers.Set("Content-Type", "text/xml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("X-Powered-By", "MultiServer");
                                        response.Headers.Set("Date", DateTime.Now.ToString("r"));

                                        byte[]? index = null;

                                        if (SVOServerConfiguration.SVOHTTPSBypass)
                                            index = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n<XML>\r\n    " +
                                                $"<SET name=\"IP\" IPAddress=\"{request.RemoteEndPoint.Address}\" />     \r\n    " +
                                                "<DATA dataType=\"URI\" name=\"entryURI\" value=\"http://killzoneps3.svo.online.scee.com:10060/SOCOMCF_SVML/account/Account_Login.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"homeURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/home.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"menuURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"logoutURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/account/Logout.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"syncprofileURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"clanCreateURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"mailboxURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"clanTournamentURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"leaderboardClanURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"leaderboardClanMembersURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"personalStatsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"friendsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"leaderboardURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"leaderboardFriendsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"clanURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"viewtimezonesURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"buildInfoURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"clanUniverseURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"loginEncryptedURI\" value=\"http://killzoneps3.svo.online.scee.com:10060/SOCOMCF_SVML/account/Account_Encrypted_Login_Submit.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"personalStatsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/stats/personalStats.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"DATA\" name=\"gameCreateURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/tempgame.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"createGameURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/Game_Create.jsp?gameMode=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"createGameSubmitURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/Game_Create_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"finishGameURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/Game_Finish_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"gamePostBinaryStatsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/Game_PostBinaryStats_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"createGamePlayerURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/Game_Create_Player_Submit.jsp?SVOGameID=%d&amp;playerSide=%d\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusAccountLoginURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Account_Login.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusAccountCreateURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Account_Create.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusLobbyListURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Lobby_List.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusChatLobbyURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Chat_Lobby.jsp\" />  \r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusChallengePopupURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Challenge_Popup.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusAcceptPopupURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Accept_Popup.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"tickerStrURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/ticker/TickerStr.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"rankingsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/ranks/rankings.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"tourLaunchPopupURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/tourney/Tourney_AutoLaunch.jsp\" /> \r\n   " +
                                                " <DATA dataType=\"DATA\" name=\"tourDataURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/tourney/Tourney_CheckIn.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"teamTourneyMatchDataURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/teamtourney/TeamTourney_MatchData.jsp?teamTourID=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"teamTourneyForfeitURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/teamtourney/TeamTourney_ForfeitTeam_Submit.jsp?teamTourTeamID=%d&amp;teamTourBracketID=%d&amp;teamTourID=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"tourForfeitURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/tourney/Tourney_Forfeit_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"getLadderMatchDataURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/ladder/Ladder_GetMatchData.jsp?ladderMatchID=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"getForfeitLadderMatchURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/ladder/Ladder_Forfeit_Submit.jsp?ladderMatchID=%d&amp;clanID=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"downloadPatch\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/download/patchDownload.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"playerStatsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/stats/Stats_GetPlayerStats.jsp?PlayerID=%d&amp;gameMode=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"playerProfileURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/profile/Profile_GetPlayerProfile.jsp?PlayerID=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"rankInfoURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/stats/Stats_CareerRankInfo.jsp?playerList=\"  /> \r\n    " +
                                                "<DATA dataType=\"URI\" name=\"downloadVerificationURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/commerce/Commerce_VerifySubmit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"purchaseListURI\" value=\"http://killzoneps3.svo.online.scee.com:10060/SOCOMCF_SVML/commerce/Commerce_PurchaseList.jsp?categoryID=default\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"createVerifiedFileGameURI\" value=\"http://killzoneps3.svo.online.scee.com:10060/SOCOMCF_SVML/commerce/Commerce_GameCreatorFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;spectatorPassword=$3\" " +
                                                "/>\r\n    <DATA dataType=\"DATA\" name=\"joinVerifiedFileGameURI\" value=\"http://killzoneps3.svo.online.scee.com:10060/SOCOMCF_SVML/commerce/Commerce_GameJoinerFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;ticket=$3\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"spectateVerifiedFileGameURI\" value=\"http://killzoneps3.svo.online.scee.com:10060/SOCOMCF_SVML/commerce/Commerce_GameSpectatorFileVerification.jsp?fileList=$1&amp;spectatorPassword=$2&amp;ticket=$3\" " +
                                                "/>\r\n    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/account/SP_Login_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"SetIgnoreListURI\" value=\"http://killzoneps3.svo.online.scee.com:10060/SOCOMCF_SVML/account/SP_UpdateIgnoreList_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"SetUniversePasswordURI\" value=\"http://killzoneps3.svo.online.scee.com:10060/SOCOMCF_SVML/account/SP_SetPassword_Submit.jsp\" />\r\n\r\n    " +
                                                "<BROWSER_INIT name=\"init\" />\r\n</XML>");
                                        else
                                            index = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n<XML>\r\n    " +
                                                $"<SET name=\"IP\" IPAddress=\"{request.RemoteEndPoint.Address}\" />     \r\n    " +
                                                "<DATA dataType=\"URI\" name=\"entryURI\" value=\"https://killzoneps3.svo.online.scee.com:10061/SOCOMCF_SVML/account/Account_Login.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"homeURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/home.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"menuURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"logoutURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/account/Logout.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"syncprofileURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"clanCreateURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"mailboxURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"clanTournamentURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"leaderboardClanURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"leaderboardClanMembersURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"personalStatsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"friendsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"leaderboardURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"leaderboardFriendsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"clanURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"viewtimezonesURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"buildInfoURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"clanUniverseURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/menu.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"loginEncryptedURI\" value=\"https://killzoneps3.svo.online.scee.com:10061/SOCOMCF_SVML/account/Account_Encrypted_Login_Submit.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"personalStatsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/stats/personalStats.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"DATA\" name=\"gameCreateURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/tempgame.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"createGameURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/Game_Create.jsp?gameMode=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"createGameSubmitURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/Game_Create_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"finishGameURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/Game_Finish_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"gamePostBinaryStatsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/Game_PostBinaryStats_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"createGamePlayerURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/game/Game_Create_Player_Submit.jsp?SVOGameID=%d&amp;playerSide=%d\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusAccountLoginURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Account_Login.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusAccountCreateURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Account_Create.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusLobbyListURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Lobby_List.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusChatLobbyURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Chat_Lobby.jsp\" />  \r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusChallengePopupURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Challenge_Popup.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"mediusAcceptPopupURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/medius/Medius_Accept_Popup.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"tickerStrURL\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/ticker/TickerStr.jsp\" />\r\n\t" +
                                                "<DATA dataType=\"URI\" name=\"rankingsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/ranks/rankings.jsp\" />\r\n    " +
                                                "<DATA dataType=\"URI\" name=\"tourLaunchPopupURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/tourney/Tourney_AutoLaunch.jsp\" /> \r\n   " +
                                                " <DATA dataType=\"DATA\" name=\"tourDataURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/tourney/Tourney_CheckIn.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"teamTourneyMatchDataURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/teamtourney/TeamTourney_MatchData.jsp?teamTourID=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"teamTourneyForfeitURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/teamtourney/TeamTourney_ForfeitTeam_Submit.jsp?teamTourTeamID=%d&amp;teamTourBracketID=%d&amp;teamTourID=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"tourForfeitURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/tourney/Tourney_Forfeit_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"getLadderMatchDataURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/ladder/Ladder_GetMatchData.jsp?ladderMatchID=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"getForfeitLadderMatchURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/ladder/Ladder_Forfeit_Submit.jsp?ladderMatchID=%d&amp;clanID=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"downloadPatch\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/download/patchDownload.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"playerStatsURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/stats/Stats_GetPlayerStats.jsp?PlayerID=%d&amp;gameMode=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"playerProfileURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/profile/Profile_GetPlayerProfile.jsp?PlayerID=%d\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"rankInfoURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/stats/Stats_CareerRankInfo.jsp?playerList=\"  /> \r\n    " +
                                                "<DATA dataType=\"URI\" name=\"downloadVerificationURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/commerce/Commerce_VerifySubmit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"purchaseListURI\" value=\"https://killzoneps3.svo.online.scee.com:10061/SOCOMCF_SVML/commerce/Commerce_PurchaseList.jsp?categoryID=default\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"createVerifiedFileGameURI\" value=\"https://killzoneps3.svo.online.scee.com:10061/SOCOMCF_SVML/commerce/Commerce_GameCreatorFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;spectatorPassword=$3\" " +
                                                "/>\r\n    <DATA dataType=\"DATA\" name=\"joinVerifiedFileGameURI\" value=\"https://killzoneps3.svo.online.scee.com:10061/SOCOMCF_SVML/commerce/Commerce_GameJoinerFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;ticket=$3\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"spectateVerifiedFileGameURI\" value=\"https://killzoneps3.svo.online.scee.com:10061/SOCOMCF_SVML/commerce/Commerce_GameSpectatorFileVerification.jsp?fileList=$1&amp;spectatorPassword=$2&amp;ticket=$3\" " +
                                                "/>\r\n    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"http://killzoneps3.svo.online.scee.com:10058/SOCOMCF_SVML/account/SP_Login_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"SetIgnoreListURI\" value=\"https://killzoneps3.svo.online.scee.com:10061/SOCOMCF_SVML/account/SP_UpdateIgnoreList_Submit.jsp\" />\r\n    " +
                                                "<DATA dataType=\"DATA\" name=\"SetUniversePasswordURI\" value=\"https://killzoneps3.svo.online.scee.com:10061/SOCOMCF_SVML/account/SP_SetPassword_Submit.jsp\" />\r\n\r\n    " +
                                                "<BROWSER_INIT name=\"init\" />\r\n</XML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = index.Length;
                                                response.OutputStream.Write(index, 0, index.Length);
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

                        case "/CONFRONTATION_XML/account/SP_Login_Submit.jsp":
                            switch (request.HttpMethod)
                            {
                                case "POST":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        string psnname = string.Empty;

                                        string? language = request.Headers.Get("Accept-Language");

                                        int languageID = 1;

                                        if (!request.HasEntityBody)
                                        {
                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }

                                        try
                                        {
                                            string? parsed = HttpUtility.ParseQueryString(request.Url.Query).Get("languageID");

                                            if (parsed != null)
                                                languageID = int.Parse(parsed);
                                        }
                                        catch (Exception)
                                        {
                                            languageID = 1;
                                        }

                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");

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

                                            const string RPCNSigner = "RPCN";

                                            // get ticket
                                            XI5Ticket ticket = XI5Ticket.ReadFromBytes(buffer);

                                            // setup username
                                            psnname = ticket.Username;

                                            // invalid ticket
                                            if (!ticket.Valid)
                                            {
                                                // log to console
                                                LoggerAccessor.LogWarn($"[SVO] - User {psnname} tried to alter their ticket data");

                                                response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                                return;
                                            }

                                            // RPCN
                                            if (ticket.SignatureIdentifier == RPCNSigner)
                                            {
                                                LoggerAccessor.LogInfo($"[SVO] - User {psnname} connected at: {DateTime.Now} and is on RPCN");

                                                psnname += $"@{RPCNSigner}";
                                            }
                                            else if (psnname.EndsWith($"@{RPCNSigner}"))
                                            {
                                                LoggerAccessor.LogError($"[SVO] - User {psnname} was caught using a RPCN suffix while not on it!");

                                                response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                                return;
                                            }
                                            else
                                                LoggerAccessor.LogInfo($"[SVO] - User {psnname} connected at: {DateTime.Now} and is on PSN");

                                            ms.Flush();
                                        }

                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? sig = HttpUtility.ParseQueryString(request.Url.Query).Get("sig");

                                        int accountId = -1;

                                        try
                                        {
                                            await SVOServerConfiguration.Database.GetAccountByName(psnname, 21784).ContinueWith((r) =>
                                            {
                                                //Found in database so keep.
                                                if (r.Result != null)
                                                    accountId = r.Result.AccountId;
                                            });
                                        }
                                        catch (Exception)
                                        {
                                            accountId = 0;
                                        }

                                        response.AddHeader("Set-Cookie", "id=hmM2M88=");
                                        response.Headers.Set("X-Powered-By", "MultiServer");
                                        response.Headers.Set("Date", DateTime.Now.ToString("r"));

                                        byte[] sp_Login = Encoding.UTF8.GetBytes("<SVML>\r\n" +
                                            "    <SP_Login>\r\n" +
                                            "        <status>\r\n" +
                                            "            <id>20600</id>\r\n" +
                                            "            <message>Hello World</message>\r\n" +
                                            "        </status>\r\n" +
                                            $"        <accountID>{accountId}</accountID>\r\n" +
                                            "        <userContext>0</userContext>\r\n" +
                                            "    </SP_Login>\r\n" +
                                            "</SVML>");

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

                        case "/CONFRONTATION_XML/home.jsp":

                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        string? language = request.Headers.Get("Accept-Language");

                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("X-Powered-By", "MultiServer");
                                        response.Headers.Set("Date", DateTime.Now.ToString("r"));

                                        byte[] homesvml = Encoding.UTF8.GetBytes("<SVML> " +
                                            "<_s name=\"firstpage\" >\r\nfunction OnEnterPage()\r\n\r\ngPlayerProfile:SetHonorRank(3)\r\ngPlayerProfile:SetMPScore(3000)\r\n\r\n\r\ngPlayerProfile:" +
                                            "SavePlayerProfile()\r\ngBrowser:OpenPage(\"file:///kin/menu/online/menu.jsp\")\r\nend\r\n</_s> </SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = homesvml.Length;
                                                response.OutputStream.Write(homesvml, 0, homesvml.Length);
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

                        case "/CONFRONTATION_XML/account/Logout.jsp":

                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        string? language = request.Headers.Get("Accept-Language");

                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("X-Powered-By", "MultiServer");
                                        response.Headers.Set("Date", DateTime.Now.ToString("r"));

                                        byte[] homesvml = Encoding.UTF8.GetBytes("<SVML> <_s name=\"firstpage\" >" +
                                            "\r\nfunction OnEnterPage()\r\n\r\ngModule:ExitAll(\"file:///kin/menu/startscreen.jsp\")\r\nend\r\n</_s> </SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = homesvml.Length;
                                                response.OutputStream.Write(homesvml, 0, homesvml.Length);
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

                        case "/SOCOMCF_SVML/menu.jsp":

                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        string? language = request.Headers.Get("Accept-Language");

                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("X-Powered-By", "MultiServer");
                                        response.Headers.Set("Date", DateTime.Now.ToString("r"));

                                        byte[] menusvml = Encoding.UTF8.GetBytes("<SVML> <PAGEID name=\"kin-preparsed_menu_online_menu\" /> <_c name=\"onlinemenuAssets\" location=\"SVO/SVOMainMenu\" /> " +
                                            "<_s name=\"musicscript\" >\r\nfunction OnEnterPage()\r\ngModule:PlayMusic(\"MainMenuMusic\")\r\nend </_s> <_u name=\"panelHeader1\" c=\"_SingleColor\" p=\"196,71,956,97\" l=\"#80808080\" /> " +
                                            "<_u name=\"panelHeader2\" c=\"_SingleColor\" p=\"196,71,956,90\" l=\"#40404080\" /> " +
                                            "<_u name=\"panelHeader3\" c=\"_SingleColor\" p=\"196,71,600,61\" l=\"#FFFFFF70\" /> " +
                                            "<_t name=\"pageTitle\" c=\"_Large\" p=\"210,94,600,36\" l=\"#000000FF\" >WARZONE</_t> " +
                                            "<_t name=\"pageSubTitle\" c=\"_Small\" p=\"210,129,928,29\" ah=\"left\" av=\"bottom\" l=\"#FFFFFFC0\" >Welcome to Killzone revival online</_t> " +
                                            "<_t name=\"pageSubTitleRight\" c=\"_Tiny\" p=\"210,129,928,29\" a=\"true\" ah=\"right\" av=\"bottom\" l=\"#FFFFFFC0\" /> <_u name=\"panelHeader4\" c=\"_SingleColor\" p=\"543,,609,71\" l=\"#000000C0\" /> " +
                                            "<_u name=\"panelHeader5\" c=\"_SingleColor\" p=\"-1000,61,1545,10\" l=\"#000000C0\" /> <_u name=\"panelHeaderLeftRedStripe\" c=\"_SingleColor\" p=\"-1000,71,1196,10\" l=\"#990000FF\" /> " +
                                            "<_t name=\"techyVersionNumber\" c=\"_Tiny\" p=\"1040,22,100,20\" l=\"#00000030\" >KIN_MNU_SCR</_t> <_u name=\"panelSide1\" c=\"_SingleColor\" p=\"1165,71,8,1000\" l=\"#00000070\" /> " +
                                            "<_u name=\"panelSide2\" c=\"_SingleColor\" p=\"1180,71,28,1000\" l=\"#00000070\" /> <_u name=\"sideCutMark1\" c=\"_SideStuffCutMark\" p=\"1,1,1,1\" l=\"#909090FF\" /> " +
                                            "<_u name=\"sideCutMark2\" c=\"_SideStuffCutMarkLeft\" p=\"32,240,196,24\" l=\"#909090FF\" /> <_u name=\"sideCutMark3\" c=\"_SideStuffCutMarkLeft2\" p=\"122,550,48,24\" l=\"#909090FF\" /> " +
                                            "<_u name=\"sideDeco2\" c=\"_SideStuff2\" p=\"1210,583,32,128\" l=\"#909090FF\" /> <_u name=\"sideDeco1\" c=\"_SideStuff1\" p=\"1226,80,32,128\" l=\"#909090FF\" /> " +
                                            "<_u name=\"sideDeco3\" c=\"_SideStuff3\" p=\"1250,200,32,512\" l=\"#909090FF\" /> <_p name=\"panelMenu\" c=\"_Normal\" p=\"196,196,400,250\" a=\"default\" g=\"0\" m=\"true\" /> " +
                                            "<_l name=\"optionsList\" c=\"_Normal\" p=\"14,14,400,220\" a=\"true\" bn=\"DOWN_BUTTON\" bp=\"UP_BUTTON\" g=\"1\" /> " +
                                            "<_i name=\"itemGame\" c=\"_Medium\" p=\",,,29\" g=\"2\" h=\"static://script.menuscript.SetFocusSubMenu()\" /><_i: name=\"!\" g=\"2\" /> " +
                                            "<_i name=\"itemCommunicate\" c=\"_Medium\" p=\",29,,29\" g=\"3\" h=\"static://script.menuscript.SetFocusSubMenu()\" /><_i: name=\"!\" g=\"3\" /> " +
                                            "<_i name=\"itemClan\" c=\"_Medium\" p=\",58,,29\" g=\"4\" h=\"static://script.menuscript.SetFocusSubMenu()\" /><_i: name=\"!\" g=\"4\" /> " +
                                            "<_i name=\"itemStats\" c=\"_Medium\" p=\",87,,29\" g=\"5\" h=\"static://script.menuscript.SetFocusSubMenu()\" /><_i: name=\"!\" g=\"5\" /> " +
                                            "<_i name=\"itemSettings\" c=\"_Medium\" p=\",116,,29\" g=\"6\" h=\"static://script.menuscript.SetFocusSubMenu()\" /><_i: name=\"!\" g=\"6\" /> <_l: name=\"!\" g=\"1\" /> " +
                                            "<_p: name=\"!\" g=\"0\" /> <_p name=\"main_subpanel\" c=\"_Normal\" p=\"598,196,554,300\" a=\"default\" bn=\"DOWN_BUTTON\" bp=\"UP_BUTTON\" g=\"7\" m=\"true\" /> " +
                                            "<_l name=\"subpaneloptions\" c=\"_Normal\" p=\"16,14,540,270\" a=\"true\" bn=\"DOWN_BUTTON\" bp=\"UP_BUTTON\" g=\"8\" /> <_i name=\"item0\" c=\"_Medium\" p=\",,,29\" g=\"9\" />" +
                                            "<_i: name=\"!\" g=\"9\" /> <_i name=\"item1\" c=\"_Medium\" p=\",29,,29\" g=\"10\" /><_i: name=\"!\" g=\"10\" /> <_i name=\"item2\" c=\"_Medium\" p=\",58,,29\" g=\"11\" />" +
                                            "<_i: name=\"!\" g=\"11\" /> <_i name=\"item3\" c=\"_Medium\" p=\",87,,29\" g=\"12\" /><_i: name=\"!\" g=\"12\" /> <_i name=\"item4\" c=\"_Medium\" p=\",116,,29\" g=\"13\" />" +
                                            "<_i: name=\"!\" g=\"13\" /> <_l: name=\"!\" g=\"8\" /> <_p: name=\"!\" g=\"7\" /> <_s name=\"menuscript\" >" +
                                            "<![CDATA[\r\nglobal gMenu = { { title = \"PLAY\", description = \"Enter the warzone now!\",\r\nlinks = { { title = \"JOIN GAME\" , link = \"file:///kin/menu/online/joingame/joinsearch.jsp\", " +
                                            "description = \"Join an existing game\"},\r\n{ title = \"CREATE GAME\" , link = \"file:///kin/menu/online/creategame/creategamepresetmenu.jsp\", description = \"Create your own custom game\"}\r\n}\r\n}," +
                                            "\r\n{ title = \"COMMUNICATION\", description = \"See who's online and read your game messages\",\r\nlinks = { { title = \"MESSAGES\", link = \"mailboxURI\", description " +
                                            "= \"Manage your messages from friends and clan members\"},\r\n{ title = \"FRIENDS\", link = \"friendsURI\", description = \"See which friends are online\" }\r\n}\r\n},\r\n{ title = \"CLAN\",\r\ndescription " +
                                            "= \"Enter the clan warzones\",\r\nlinks = { { title = \"\", link = \"\", description = \"\"}, { title = \"UNIVERSE\", link = \"clanUniverseURI\", description = \"Clan ranking and event information\"},\r\n{ " +
                                            "title = \"TOURNAMENTS\", link = \"clanTournamentURI\", description = \"Manage your clans schedule\"},\r\n{ title = \"RANKINGS\", link = \"leaderboardClanURI\", description = \"Check your clans overall" +
                                            " ranking\"},\r\n{ title = \"MEMBER RANKINGS\", link = \"leaderboardClanMembersURI\", description = \"See how your buddies are doing\"}\r\n}\r\n},\r\n{ title = \"STATISTICS\", description = \"Your unlockables," +
                                            " rankings and statistics\",\r\nlinks = { { title = \"MY STATISTICS\", link = \"personalStatsURI\", description = \"Your personal statistics, rewards and score overview\"},\r\n{ title = \"RANKINGS\", " +
                                            "link = \"leaderboardURI\", description = \"Take a look at the overall rankings\"},\r\n{ title = \"FRIEND RANKINGS\", link = \"leaderboardFriendsURI\", description = \"See where your friends rank in the " +
                                            "universe\"}\r\n}\r\n},\r\n{ title = \"OPTIONS\", description = \"Change region and timezone settings\",\r\nlinks = { { title = \"CHANGE REGION [\" .. gPlayerProfile:GetLocation() .. \"]\", link " +
                                            "= \"file:///kin/menu/online/connect/regionreselect.jsp\", description = \"Change your region settings\"},\r\n{ title = \"CHANGE TIMEZONE\", link = \"\", description = \"Change your timezone settings\"}\r\n}\r\n}\r\n}\r\nglobal " +
                                            "gPanel = nil\r\nglobal gList = nil\r\nglobal gDescription = nil\r\nglobal gMenuSelectChange = -1\r\nglobal gMenuDelay = 0.3\r\nglobal gIsFirstFrame = true\r\nfunction OnEnterPage()\r\nif( gBrowser:IsPopupOpened() ) " +
                                            "then\r\ngBrowser:ClosePopup()\r\nend\r\ngBrowserContext:SetPageValue(\"buttonlegend.ok\", \"SELECT\")\r\ngBrowserContext:SetPageValue(\"buttonlegend.cancel\", " +
                                            "\"BACK\")\r\ngBrowserContext:SetPageValue(\"buttonlegend.updown\", \"NAVIGATE\")\r\ngPanel = gBrowser:GetTagByName(\"main_subpanel\")\r\ngList = gBrowser:GetTagByName(\"optionsList\")\r\ngPanel:SetOnClick(CROSS_BUTTON, " +
                                            "\"menuscript.ClickAccept()\")\r\ngPanel:SetOnClick(CIRCLE_BUTTON, \"menuscript.GoBack()\")\r\ngList:SetOnClick(CIRCLE_BUTTON, " +
                                            "\"menuscript.GoBack()\")\r\ngList:SetOnChange(\"menuscript.ChangeMainSelection\")\r\ngPanel:GetItem(0):SetOnChange(\"menuscript.ChangeSubSelection\")\r\nif " +
                                            "(gPlayerProfile:GetClanID() <= 0) then\r\ngMenu[3].links[1].title = \"CREATE CLAN\"\r\ngMenu[3].links[1].link = \"static://script.menuscript.CreateClan\"\r\ngMenu[3].links[1].description = " +
                                            "\"Create and manage your own clan\"\r\ngMenu[3].links[5].disabled = true\r\nelse\r\ngMenu[3].links[1].title = \"MY CLAN\"\r\ngMenu[3].links[1].link = \"clanURI\"\r\ngMenu[3].links[1].description =" +
                                            " \"See everything about your clan\"\r\ngMenu[3].links[5].disabled = false\r\nend\r\nif (gPlayerProfile:ShowBuildInfo()) then\r\ngMenu[5].links[3] = { title = \"ABOUT\", link = " +
                                            "\"static://script.menuscript.ShowBuildInfo\", sub_menu_list=nil, description = \"ABOUT\", links = {}}\r\nend\r\nfor ix =1, " +
                                            "getn(gMenu)\r\ndo\r\ngList:GetItem(ix-1):SetCaption(gMenu[ix].title)\r\nend\r\nRenderSubMenu(gList:GetSelectedIndex()+1)\r\ngDescription = " +
                                            "gBrowser:GetTagByName(\"pageSubTitleRight\")\r\ngMenuDelay = 0.0\r\ngPanel:SetMarked(true) SetFocusMainMenu() local timezones_uri = gBrowser:ResolveURI(\"viewtimezonesURI\")." +
                                            ".\"?lang=\"..gModule:URLEncode(gBrowser:GetLanguageShortName())\r\ngMenu[5].links[2].link = timezones_uri\r\nlocal chat_restrictions = " +
                                            "gBrowserContext:GetApplicationValue(\"display.chatrestrictions\")\r\nif (chat_restrictions and chat_restrictions ~= \"\") then\r\ngBrowserContext:SetApplicationValue(\"display.chatrestrictions\", " +
                                            "\"\")\r\ngBrowserContext:SetPageValue(\"popup.message\", \"Chat is disabled on your <verbatim>PlayStation....Network</verbatim> account due to parental control " +
                                            "restrictions.\")\r\ngBrowserContext:SetPageValue(\"popup.ok\", \"OK\")\r\ngBrowser:OpenPopup(\"file:///kin/menu/popups/popupgeneric.jsp\")\r\nend\r\nend\r\nfunction " +
                                            "OnUpdatePage()\r\nif (gMenuSelectChange ~= -1 and (gMenuSelectChange + gMenuDelay) < gBrowser:GetTime()) then\r\nlocal ix = 1 + gList:GetSelectedIndex()\r\nRenderSubMenu(ix)\r\ngPanel:SetMarked(true)\r\ngMenuSelectChange = " +
                                            "-1\r\nif (not gPanel:GetItem(0):IsSelected()) then\r\ngPanel:GetItem(0):SetSelectedIndex(-1)\r\nend\r\ngMenuDelay = 0.3\r\nend\r\ngIsFirstFrame = false\r\nend\r\nfunction RenderSubMenu(id)\r\nlocal menu = " +
                                            "gPanel:GetItem(0)\r\nmenu:Clear()\r\nfor ix =1, getn(gMenu[id].links)\r\ndo\r\nlocal item = " +
                                            "menu:AddItem()\r\nitem:SetCaption(gMenu[id].links[ix].title)\r\nitem:SetSelectable(not gMenu[id].links[ix].disabled)\r\nend\r\nmenu:Refresh()\r\nend\r\nfunction OnPopupOpen(id)\r\nend\r\nfunction OnPopupClose(id," +
                                            " value)\r\nif (id == \"back\" and value == \"ok\") then\r\ngBrowser:OpenPage(\"logoutURI\")\r\nend\r\nend\r\nfunction Back()\r\ngBrowserContext:SetPageValue(\"popup.scriptname\", " +
                                            "\"menuscript\")\r\ngBrowserContext:SetPageValue(\"popup.message\", \"Do you want to logout and return to the main menu?\")\r\ngBrowserContext:SetPageValue(\"popup.id\", " +
                                            "\"back\")\r\ngBrowserContext:SetPageValue(\"popup.ok\", \"OK\")\r\ngBrowserContext:SetPageValue(\"popup.cancel\", " +
                                            "\"CANCEL\")\r\ngBrowser:OpenPopup(\"file:///kin/menu/popups/popupgeneric.jsp\")\r\nend\r\nfunction SetFocusSubMenu()\r\nceTrace(\"SetFocusSubMenu\")\r\ngList:SetSelected(false)\r\ngPanel:GetItem(0)" +
                                            ":SetSelected(true)\r\ngPanel:GetItem(0):SetSelectedIndex(0)\r\nend\r\nfunction SetFocusMainMenu()\r\ngList:SetSelected(true)\r\ngPanel:GetItem(0):SetSelectedIndex(-1)\r\ngPanel:GetItem(0)" +
                                            ":SetSelected(false)\r\nRenderDescription()\r\nend\r\nfunction GoBack()\r\nif (gPanel:GetItem(0):GetSelectedIndex()>-1) then\r\nSetFocusMainMenu()\r\nelse\r\nBack()\r\nend\r\nend\r\nfunction " +
                                            "ClickAccept()\r\nif (gPanel:GetSelectedIndex()>-1) then\r\nlocal link = gMenu[1+gList:GetSelectedIndex()].links[gPanel:GetItem(0):GetSelectedIndex()+1]." +
                                            "link\r\ngBrowser:OpenPage(link)\r\nend\r\nend\r\nfunction ChangeMainSelection()\r\nif (not gIsFirstFrame) then\r\ngPanel:SetMarked(false)\r\nend\r\ngMenuSelectChange = " +
                                            "gBrowser:GetTime()\r\nRenderDescription()\r\nend\r\nfunction ChangeSubSelection()\r\nRenderDescription()\r\nend\r\nfunction RenderDescription()\r\nlocal ix = 1 + " +
                                            "gList:GetSelectedIndex()\r\nlocal id = 1 + gPanel:GetItem(0):GetSelectedIndex()\r\nif (id == 0) then\r\ngDescription:SetValue(gMenu[ix].description)\r\nelse\r\ngDescription:" +
                                            "SetValue(gMenu[ix].links[id].description)\r\nend\r\nend\r\nfunction CreateClan()\r\nlocal ranks_resource = gModule:GetPlayerRanksResource()\r\nlocal has_required_rank = " +
                                            "ranks_resource and gPlayerProfile:GetMPScore() >= ranks_resource:GetScoreForRank(2)\r\nif (has_required_rank) then\r\ngBrowserContext:SetApplicationValue(\"createclan.back\", " +
                                            "\"file:///kin/clan/myclan.do\")\r\ngBrowser:OpenPage(\"clanCreateURI\")\r\nelse\r\nlocal rank_title = ranks_resource:GetPlayerRankTitleByID(2)\r\nlocal rank_name = gLocalizer:" +
                                            "GetUTF8StringByInGameID(rank_title)\r\nlocal msg = \"Rank up to [RANK] to be able to create a clan.\"\r\nmsg = gsub( msg, \"%[RANK%]\", rank_name )\r\ngBrowserContext:SetPageValue(\"popup." +
                                            "message\", msg)\r\ngBrowserContext:SetPageValue(\"popup.ok\", \"OK\")\r\ngBrowser:OpenPopup(\"file:///kin/menu/popups/popupgeneric.jsp\")\r\nend\r\nend\r\nfunction ShowBuildInfo()\r\ngBrowser:" +
                                            "OpenPage(\"buildInfoURI\")\r\nend\r\n]]></_s> <_g name=\"ButtonLegend\" c=\"Legend\" p=\"196,620,956,100\" /> <_s name=\"buttonlegend\" ><![CDATA[\r\nglobal gButtonList = {\r\n{\r\nkey = " +
                                            "\"buttonlegend.select\",\r\nimage = \"<buttonimage name=select>\"\r\n},\r\n{\r\nkey = \"buttonlegend.start\",\r\nimage = \"<buttonimage name=start>\"\r\n},\r\n{\r\nkey = \"buttonlegend.L1\",\r\nimage = " +
                                            "\"<buttonimage name=L1>\"\r\n},\r\n{\r\nkey = \"buttonlegend.R1\",\r\nimage = \"<buttonimage name=R1>\"\r\n},\r\n{\r\nkey = \"buttonlegend.L2R2\",\r\nimage = " +
                                            "\"<buttonimage name=L2><buttonimage name=R2>\"\r\n},\r\n{\r\nkey = \"buttonlegend.L2\",\r\nimage = \"<buttonimage name=L2>\"\r\n},\r\n{\r\nkey = \"buttonlegend.R2\",\r\nimage = " +
                                            "\"<buttonimage name=R2>\"\r\n},\r\n{\r\nkey = \"buttonlegend.L3\",\r\nimage = \"<buttonimage name=L3>\"\r\n},\r\n{\r\nkey = \"buttonlegend.R3\",\r\nimage = " +
                                            "\"<buttonimage name=R3>\"\r\n},\r\n{\r\nkey = \"buttonlegend.LAnalog\",\r\nimage = \"<buttonimage name=LAnalog>\"\r\n},\r\n{\r\nkey = \"buttonlegend.RAnalog\",\r\nimage = " +
                                            "\"<buttonimage name=RAnalog>\"\r\n},\r\n{\r\nkey = \"buttonlegend.down\",\r\nimage = \"<buttonimage name=down>\"\r\n},\r\n{\r\nkey = \"buttonlegend.up\",\r\nimage = " +
                                            "\"<buttonimage name=up>\"\r\n},\r\n{\r\nkey = \"buttonlegend.leftrightupdown\",\r\nimage = \"<buttonimage name=leftrightupdown>\"\r\n},\r\n{\r\nkey = " +
                                            "\"buttonlegend.updown\",\r\nimage = \"<buttonimage name=updown>\"\r\n},\r\n{\r\nkey = \"buttonlegend.leftright\",\r\nimage = \"<buttonimage name=leftright>\"\r\n},\r\n{\r\nkey = " +
                                            "\"buttonlegend.triangle\",\r\nimage = \"<buttonimage name=triangle>\"\r\n},\r\n{\r\nkey = \"buttonlegend.square\",\r\nimage = \"<buttonimage name=square>\"\r\n},\r\n{\r\nkey = " +
                                            "\"buttonlegend.L1R1\",\r\nimage = \"<buttonimage name=L1><buttonimage name=R1>\"\r\n},\r\n{\r\nkey = \"buttonlegend.ok\",\r\nimage = \"<buttonimage name=ok>\"\r\n},\r\n{\r\nkey = " +
                                            "\"buttonlegend.cancel\",\r\nimage = \"<buttonimage name=cancel>\"\r\n}\r\n}\r\nglobal gButtonLegend = nil\r\nfunction OnEnterPage()\r\ngButtonLegend = " +
                                            "gBrowser:GetTagByName(\"ButtonLegend\")\r\nRenderButtons()\r\nend\r\nfunction OnUpdatePage()\r\nRenderButtons()\r\nend\r\nfunction RenderButtons()\r\nlocal button_string = " +
                                            "\"\"\r\nlocal description\r\nfor i=1, getn(gButtonList)\r\ndo\r\nbutton_string = \"\"\r\ndescription = gBrowserContext:GetPageValue(gButtonList[i].key)\r\nif (description) " +
                                            "then\r\nbutton_string = gButtonList[i].image .. \" \" .. description\r\nend\r\ngButtonLegend:SetButton(i-1, button_string)\r\nend\r\nend\r\n]]></_s> " +
                                            "</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = menusvml.Length;
                                                response.OutputStream.Write(menusvml, 0, menusvml.Length);
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
                LoggerAccessor.LogError($"[SVO] - SocomConfrontation_SVO thrown an assertion - {ex}");
                response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }
    }
}
