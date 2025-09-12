using CustomLogger;
using Microsoft.Extensions.Logging;
using MultiServerLibrary.Extension;
using Org.BouncyCastle.Asn1.Ocsp;
using SVO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SVO.Games.PS3
{
    public class Killzone2
    {

        public static async Task Killzone2_SVOAsync(HttpListenerContext context)
        {
            using (var response = context.Response)
            {
                try
                {
                    switch (context.Request.Url.AbsolutePath)
                    {
                        case "/KILLZONEPS3_SVML/start.jsp":
                            {
                                switch (context.Request.HttpMethod)
                                {
                                    case "GET":

                                        HttpListenerRequest req = context.Request;
                                        HttpListenerResponse resp = context.Response;
                                        resp.Headers.Set("Content-Type", "text/svml");

                                        string clientMac = req.Headers.Get("X-SVOMac");
                                        string serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);
                                        if (string.IsNullOrEmpty(serverMac))
                                        {
                                            response.StatusCode = (int)HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                        {
                                            if (!req.HasEntityBody)
                                            {
                                                response.StatusCode = (int)HttpStatusCode.Forbidden;
                                                return;
                                            }
                                            resp.Headers.Set("X-SVOMac", serverMac);

                                            string domain = "killzoneps3.svo.online.scee.com";

                                            if (!SVOServerConfiguration.PreferDNSUrls)
                                                await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                            byte[] uriStore = null;
                                            if (SVOServerConfiguration.SVOHTTPSBypass)
                                            {
                                                uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<SVML>\r\n" +
                                                $"    <SET name=\"IP\" IPAddress=\"{req.RemoteEndPoint.Address}\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"entryURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/account/Account_Login.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"homeURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/home.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"menuURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"logoutURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/account/Logout.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"syncprofileURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/Sync_Profiles.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"clanCreateURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/Clan_Create.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mailboxURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/Mail_Box.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"clanTournamentURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"leaderboardClanURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"leaderboardClanMembersURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"personalStatsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"friendsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"leaderboardURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/Stats_CareerLeaderboard.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"leaderboardFriendsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/Stats_LeaderboardFriends.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"clanURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/clan.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"viewtimezonesURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/buildinfo.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"buildInfoURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"clanUniverseURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"loginEncryptedURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/account/Account_Encrypted_Login_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"personalStatsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/stats/personalStats.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"gameCreateURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/tempgame.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGameURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/Game_Create.jsp?gameMode=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGameSubmitURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/Game_Create_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"finishGameURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/Game_Finish_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"gamePostBinaryStatsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/Game_PostBinaryStats_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGamePlayerURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/Game_Create_Player_Submit.jsp?SVOGameID=%d&amp;playerSide=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAccountLoginURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Account_Login.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAccountCreateURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Account_Create.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusLobbyListURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Lobby_List.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusChatLobbyURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Chat_Lobby.jsp\" />   \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusChallengePopupURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Challenge_Popup.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAcceptPopupURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Accept_Popup.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tickerStrURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/ticker/TickerStr.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"rankingsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/ranks/rankings.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"tourLaunchPopupURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/tourney/Tourney_AutoLaunch.jsp\" />  \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tourDataURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/tourney/Tourney_CheckIn.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"teamTourneyMatchDataURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/teamtourney/TeamTourney_MatchData.jsp?teamTourID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"teamTourneyForfeitURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/teamtourney/TeamTourney_ForfeitTeam_Submit.jsp?teamTourTeamID=%d&amp;teamTourBracketID=%d&amp;teamTourID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tourForfeitURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/tourney/Tourney_Forfeit_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"getLadderMatchDataURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/ladder/Ladder_GetMatchData.jsp?ladderMatchID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"getForfeitLadderMatchURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/ladder/Ladder_Forfeit_Submit.jsp?ladderMatchID=%d&amp;clanID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"downloadPatch\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/download/patchDownload.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"playerStatsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/stats/Stats_GetPlayerStats.jsp?PlayerID=%d&amp;gameMode=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"playerProfileURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/profile/Profile_GetPlayerProfile.jsp?PlayerID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"rankInfoURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/stats/Stats_CareerRankInfo.jsp?playerList=\"  />  \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"downloadVerificationURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/commerce/Commerce_VerifySubmit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"purchaseListURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/commerce/Commerce_PurchaseList.jsp?categoryID=default\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createVerifiedFileGameURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/commerce/Commerce_GameCreatorFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;spectatorPassword=$3\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"joinVerifiedFileGameURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/commerce/Commerce_GameJoinerFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;ticket=$3\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"spectateVerifiedFileGameURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/commerce/Commerce_GameSpectatorFileVerification.jsp?fileList=$1&amp;spectatorPassword=$2&amp;ticket=$3\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/account/SP_Login_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"SetIgnoreListURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/account/SP_UpdateIgnoreList_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"SetUniversePasswordURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/account/SP_SetPassword_Submit.jsp\" /> \r\n \r\n" +
                                                "    <BROWSER_INIT name=\"init\" /> \r\n" +
                                                "</SVML>");
                                            } else
                                            {
                                                uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<SVML>\r\n" +
                                                $"    <SET name=\"IP\" IPAddress=\"{req.RemoteEndPoint.Address}\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"entryURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/account/Account_Login.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"homeURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/home.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"menuURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"logoutURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/account/Logout.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"syncprofileURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/Sync_Profiles.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"clanCreateURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/Clan_Create.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mailboxURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/Mail_Box.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"clanTournamentURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"leaderboardClanURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"leaderboardClanMembersURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"personalStatsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"friendsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"leaderboardURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/Stats_CareerLeaderboard.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"leaderboardFriendsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/Stats_LeaderboardFriends.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"clanURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/clan.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"viewtimezonesURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/buildinfo.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"buildInfoURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"clanUniverseURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/menu.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"loginEncryptedURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/account/Account_Encrypted_Login_Submit.jsp\" />\r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"personalStatsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/stats/personalStats.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"gameCreateURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/tempgame.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGameURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/Game_Create.jsp?gameMode=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGameSubmitURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/Game_Create_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"finishGameURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/Game_Finish_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"gamePostBinaryStatsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/Game_PostBinaryStats_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createGamePlayerURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/game/Game_Create_Player_Submit.jsp?SVOGameID=%d&amp;playerSide=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAccountLoginURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Account_Login.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAccountCreateURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Account_Create.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusLobbyListURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Lobby_List.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusChatLobbyURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Chat_Lobby.jsp\" />   \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusChallengePopupURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Challenge_Popup.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"mediusAcceptPopupURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/medius/Medius_Accept_Popup.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tickerStrURL\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/ticker/TickerStr.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"rankingsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/ranks/rankings.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"tourLaunchPopupURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/tourney/Tourney_AutoLaunch.jsp\" />  \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tourDataURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/tourney/Tourney_CheckIn.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"teamTourneyMatchDataURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/teamtourney/TeamTourney_MatchData.jsp?teamTourID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"teamTourneyForfeitURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/teamtourney/TeamTourney_ForfeitTeam_Submit.jsp?teamTourTeamID=%d&amp;teamTourBracketID=%d&amp;teamTourID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"tourForfeitURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/tourney/Tourney_Forfeit_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"getLadderMatchDataURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/ladder/Ladder_GetMatchData.jsp?ladderMatchID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"getForfeitLadderMatchURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/ladder/Ladder_Forfeit_Submit.jsp?ladderMatchID=%d&amp;clanID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"downloadPatch\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/download/patchDownload.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"playerStatsURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/stats/Stats_GetPlayerStats.jsp?PlayerID=%d&amp;gameMode=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"playerProfileURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/profile/Profile_GetPlayerProfile.jsp?PlayerID=%d\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"rankInfoURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/stats/Stats_CareerRankInfo.jsp?playerList=\"  />  \r\n" +
                                                $"    <DATA dataType=\"URI\" name=\"downloadVerificationURI\" value=\"http://{domain}:10060/KILLZONEPS3_SVML/commerce/Commerce_VerifySubmit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"purchaseListURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/commerce/Commerce_PurchaseList.jsp?categoryID=default\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"createVerifiedFileGameURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/commerce/Commerce_GameCreatorFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;spectatorPassword=$3\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"joinVerifiedFileGameURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/commerce/Commerce_GameJoinerFileVerification.jsp?fileList=$1&amp;userPassword=$2&amp;ticket=$3\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"spectateVerifiedFileGameURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/commerce/Commerce_GameSpectatorFileVerification.jsp?fileList=$1&amp;spectatorPassword=$2&amp;ticket=$3\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/account/SP_Login_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"SetIgnoreListURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/account/SP_UpdateIgnoreList_Submit.jsp\" /> \r\n" +
                                                $"    <DATA dataType=\"DATA\" name=\"SetUniversePasswordURI\" value=\"https://{domain}:10061/KILLZONEPS3_SVML/account/SP_SetPassword_Submit.jsp\" /> \r\n \r\n" +
                                                "    <BROWSER_INIT name=\"init\" /> \r\n" +
                                                "</SVML>");
                                            }

                                            resp.OutputStream.Write(uriStore);
#if DEBUG
                                            LoggerAccessor.LogInfo($"Start URIStore for Killzone 2 SENT!");
#endif
                                        }
                                        break;
                                }

                                break;
                            }
                            

                        case "/KILLZONEPS3_SVML/home.jsp":
                            {
                                switch (context.Request.HttpMethod)
                                {
                                    case "GET":
                                        HttpListenerRequest req = context.Request;
                                        HttpListenerResponse resp = context.Response;
                                        resp.Headers.Set("Content-Type", "text/svml");

                                        string clientMac = req.Headers.Get("X-SVOMac");
                                        string serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);
                                        if (string.IsNullOrEmpty(serverMac))
                                        {
                                            response.StatusCode = (int)HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                        {
                                            if (!req.HasEntityBody)
                                            {
                                                response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                                return;
                                            }
                                            resp.Headers.Set("X-SVOMac", serverMac);


                                            string xmlMessage = "";

                                            xmlMessage = "<SVML> <_s name=\"firstpage\" >\r\n" +
                                                "function OnEnterPage()\r\n\r\n" +
                                                "gPlayerProfile:SetHonorRank(3)\r\n" +
                                                "gPlayerProfile:SetMPScore(3000)\r\n\r\n\r\n" +
                                                "gPlayerProfile:SavePlayerProfile()\r\n" +
                                                "gBrowser:OpenPage(\"file:///kin/menu/online/menu.jsp\")\r\n" +
                                                "end \r\n</_s> </SVML>";

                                            resp.OutputStream.Write(Encoding.ASCII.GetBytes(xmlMessage));
#if DEBUG
                                            LoggerAccessor.LogInfo($"Start home for Killzone 2 SENT!");
#endif
                                        }
                                        break;
                                }

                                break;
                            }

                        case "/KILLZONEPS3_SVML/menu.jsp":
                            {
                                switch (context.Request.HttpMethod)
                                {
                                    case "GET":
                                        HttpListenerRequest req = context.Request;
                                        HttpListenerResponse resp = context.Response;
                                        resp.Headers.Set("Content-Type", "text/svml");

                                        string clientMac = req.Headers.Get("X-SVOMac");
                                        string serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);
                                        if (string.IsNullOrEmpty(serverMac))
                                        {
                                            response.StatusCode = (int)HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                        {
                                            if (!req.HasEntityBody)
                                            {
                                                response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                                return;
                                            }
                                            resp.Headers.Set("X-SVOMac", serverMac);

                                            string xmlMessage = "<SVML>\r\n" +
                                                "<PAGEID name=\"kin-preparsed_menu_online_menu\" />\r\n" +
                                                "<_c name=\"onlinemenuAssets\" location=\"SVO/SVOMainMenu\" />\r\n" +
                                                "<_s name=\"musicscript\" >\r\n" +
                                                "function OnEnterPage()\r\n" +
                                                "gModule:PlayMusic(\"MainMenuMusic\")\r\n" +
                                                "end </_s>\r\n" +
                                                "<_u name=\"panelHeader1\" c=\"_SingleColor\" p=\"196,71,956,97\" l=\"#80808080\" />\r\n" +
                                                "<_u name=\"panelHeader2\" c=\"_SingleColor\" p=\"196,71,956,90\" l=\"#40404080\" />\r\n" +
                                                "<_u name=\"panelHeader3\" c=\"_SingleColor\" p=\"196,71,600,61\" l=\"#FFFFFF70\" />\r\n" +
                                                "<_t name=\"pageTitle\" c=\"_Large\" p=\"210,94,600,36\" l=\"#000000FF\" >WARZONE</_t>\r\n" +
                                                "<_t name=\"pageSubTitle\" c=\"_Small\" p=\"210,129,928,29\" ah=\"left\" av=\"bottom\" l=\"#FFFFFFC0\" >Welcome to Killzone &#174;2 Online</_t>\r\n" +
                                                "<_t name=\"pageSubTitleRight\" c=\"_Tiny\" p=\"210,129,928,29\" a=\"true\" ah=\"right\" av=\"bottom\" l=\"#FFFFFFC0\" />\r\n" +
                                                "<_u name=\"panelHeader4\" c=\"_SingleColor\" p=\"543,,609,71\" l=\"#000000C0\" />\r\n" +
                                                "<_u name=\"panelHeader5\" c=\"_SingleColor\" p=\"-1000,61,1545,10\" l=\"#000000C0\" />\r\n" +
                                                "<_u name=\"panelHeaderLeftRedStripe\" c=\"_SingleColor\" p=\"-1000,71,1196,10\" l=\"#990000FF\" />\r\n" +
                                                "<_t name=\"techyVersionNumber\" c=\"_Tiny\" p=\"1040,22,100,20\" l=\"#00000030\" >KIN_MNU_SCR</_t>\r\n" +
                                                "<_u name=\"panelSide1\" c=\"_SingleColor\" p=\"1165,71,8,1000\" l=\"#00000070\" />\r\n" +
                                                "<_u name=\"panelSide2\" c=\"_SingleColor\" p=\"1180,71,28,1000\" l=\"#00000070\" />\r\n" +
                                                "<_u name=\"sideCutMark1\" c=\"_SideStuffCutMark\" p=\"1,1,1,1\" l=\"#909090FF\" />\r\n" +
                                                "<_u name=\"sideCutMark2\" c=\"_SideStuffCutMarkLeft\" p=\"32,240,196,24\" l=\"#909090FF\" />\r\n" +
                                                "<_u name=\"sideCutMark3\" c=\"_SideStuffCutMarkLeft2\" p=\"122,550,48,24\" l=\"#909090FF\" />\r\n" +
                                                "<_u name=\"sideDeco2\" c=\"_SideStuff2\" p=\"1210,583,32,128\" l=\"#909090FF\" />\r\n" +
                                                "<_u name=\"sideDeco1\" c=\"_SideStuff1\" p=\"1226,80,32,128\" l=\"#909090FF\" />" +
                                                "<_u name=\"sideDeco3\" c=\"_SideStuff3\" p=\"1250,200,32,512\" l=\"#909090FF\" />\r\n" +
                                                "<_p name=\"panelMenu\" c=\"_Normal\" p=\"196,196,400,250\" a=\"default\" g=\"0\" m=\"true\" />\r\n" +
                                                "<_l name=\"optionsList\" c=\"_Normal\" p=\"14,14,400,220\" a=\"true\" bn=\"DOWN_BUTTON\" bp=\"UP_BUTTON\" g=\"1\" />\r\n" +
                                                "<_i name=\"itemGame\" c=\"_Medium\" p=\"29\" g=\"2\" h=\"static://script.menuscript.SetFocusSubMenu()\" />\r\n" +
                                                "<_i: name=\"!\" g=\"2\" />\r\n" +
                                                "<_i name=\"itemCommunicate\" c=\"_Medium\" p=\",29,,29\" g=\"3\" h=\"static://script.menuscript.SetFocusSubMenu()\" />\r\n" +
                                                "<_i: name=\"!\" g=\"3\" />\r\n" +
                                                "<_i name=\"itemClan\" c=\"_Medium\" p=\",58,,29\" g=\"4\" h=\"static://script.menuscript.SetFocusSubMenu()\" />\r\n" +
                                                "<_i: name=\"!\" g=\"4\" />\r\n" +
                                                "<_i name=\"itemStats\" c=\"_Medium\" p=\",87,,29\" g=\"5\" h=\"static://script.menuscript.SetFocusSubMenu()\" />\r\n" +
                                                "<_i: name=\"!\" g=\"5\" /> <_i name=\"itemSettings\" c=\"_Medium\" p=\",116,,29\" g=\"6\" h=\"static://script.menuscript.SetFocusSubMenu()\" />\r\n" +
                                                "<_i: name=\"!\" g=\"6\" /> <_l: name=\"!\" g=\"1\" />\r\n" +
                                                "<_p: name=\"!\" g=\"0\" /> <_p name=\"main_subpanel\" c=\"_Normal\" p=\"598,196,554,300\" a=\"default\" bn=\"DOWN_BUTTON\" bp=\"UP_BUTTON\" g=\"7\" m=\"true\" />\r\n" +
                                                "<_l name=\"subpaneloptions\" c=\"_Normal\" p=\"16,14,540,270\" a=\"true\" bn=\"DOWN_BUTTON\" bp=\"UP_BUTTON\" g=\"8\" /> <_i name=\"item0\" c=\"_Medium\" p=\",,,29\" g=\"9\" />\r\n" +
                                                "<_i: name=\"!\" g=\"9\" />\r\n" +
                                                "<_i name=\"item1\" c=\"_Medium\" p=\",29,,29\" g=\"10\" /><_i: name=\"!\" g=\"10\" />" +
                                                "<_i name=\"item2\" c=\"_Medium\" p=\",58,,29\" g=\"11\" /><_i: name=\"!\" g=\"11\" />\r\n" +
                                                "<_i name=\"item3\" c=\"_Medium\" p=\",87,,29\" g=\"12\" /><_i: name=\"!\" g=\"12\" />" +
                                                "<_i name=\"item4\" c=\"_Medium\" p=\",116,,29\" g=\"13\" /><_i: name=\"!\" g=\"13\" />" +
                                                "<_l: name=\"!\" g=\"8\" /> <_p: name=\"!\" g=\"7\" />\r\n" +
                                                "<_s name=\"menuscript\" >\r\n" +
                                                "<![CDATA[ \r\n" +
                                                "global gMenu = { { title = \"PLAY\", description = \"Enter the warzone now!\", \r\n" +
                                                "links = { { title = \"JOIN GAME\" , link = \"file:///kin/menu/online/joingame/joinsearch.jsp\", description = \"Join an existing game\"}, \r\n" +
                                                "{ title = \"CREATE GAME\" , link = \"file:///kin/menu/online/creategame/creategamepresetmenu.jsp\", description = \"Create your own custom game\"} \r\n" +
                                                "} \r\n}," +
                                                " \r\n" +
                                                "{ title = \"COMMUNICATION\", description = \"See who's online and read your game messages\", \r\n" +
                                                "links = { { title = \"MESSAGES\", link = \"mailboxURI\", description = \"Manage your messages from friends and clan members\"}, \r\n" +
                                                "{ title = \"FRIENDS\", link = \"friendsURI\", description = \"See which friends are online\" } \r\n" +
                                                "} \r\n}, \r\n{ title = \"CLAN\", \r\ndescription = \"Enter the clan warzones\", \r\n" +
                                                "links = { { title = \"\", link = \"\", description = \"\"}, { title = \"UNIVERSE\", link = \"clanUniverseURI\", description = \"Clan ranking and event information\"}, \r\n" +
                                                "{ title = \"TOURNAMENTS\", link = \"clanTournamentURI\", description = \"Manage your clans schedule\"}, \r\n{ title = \"RANKINGS\", link = \"leaderboardClanURI\", description = \"Check your clans overall ranking\"}, \r\n{ title = \"MEMBER RANKINGS\", link = \"leaderboardClanMembersURI\", description = \"See how your buddies are doing\"} \r\n} \r\n}, \r\n{ title = \"STATISTICS\", description = \"Your unlockables, rankings and statistics\", \r\nlinks = { { title = \"MY STATISTICS\", link = \"personalStatsURI\", description = \"Your personal statistics, rewards and score overview\"}, \r\n{ title = \"RANKINGS\", link = \"leaderboardURI\", description = \"Take a look at the overall rankings\"}, \r\n" +
                                                "{ title = \"FRIEND RANKINGS\", link = \"leaderboardFriendsURI\", description = \"See where your friends rank in the universe\"} \r\n} \r\n}, \r\n{ title = \"OPTIONS\", description = \"Change region and timezone settings\", \r\nlinks = { { title = \"CHANGE REGION [\" .. gPlayerProfile:GetLocation() .. \"]\", link = \"file:///kin/menu/online/connect/regionreselect.jsp\", description = \"Change your region settings\"}, \r\n{ title = \"CHANGE TIMEZONE\", link = \"\", description = \"Change your timezone settings\"} \r\n} \r\n} \r\n} \r\nglobal gPanel = nil \r\nglobal gList = nil \r\n" +
                                                "global gDescription = nil \r\n" +
                                                "global gMenuSelectChange = -1 \r\n" +
                                                "global gMenuDelay = 0.3 \r\n" +
                                                "global gIsFirstFrame = true \r\n" +
                                                "function OnEnterPage() \r\nif( gBrowser:IsPopupOpened() ) then \r\ngBrowser:ClosePopup() \r\nend \r\ngBrowserContext:SetPageValue(\"buttonlegend.ok\", \"SELECT\") \r\ngBrowserContext:SetPageValue(\"buttonlegend.cancel\", \"BACK\") \r\ngBrowserContext:SetPageValue(\"buttonlegend.updown\", \"NAVIGATE\") \r\ngPanel = gBrowser:GetTagByName(\"main_subpanel\") \r\ngList = gBrowser:GetTagByName(\"optionsList\") \r\ngPanel:SetOnClick(CROSS_BUTTON, \"menuscript.ClickAccept()\") \r\ngPanel:SetOnClick(CIRCLE_BUTTON, \"menuscript.GoBack()\") \r\ngList:SetOnClick(CIRCLE_BUTTON, \"menuscript.GoBack()\") \r\ngList:SetOnChange(\"menuscript.ChangeMainSelection\") \r\n" +
                                                "gPanel:GetItem(0):SetOnChange(\"menuscript.ChangeSubSelection\") \r\n" +
                                                "if (gPlayerProfile:GetClanID() <= 0) then \r\ngMenu[3].links[1].title = \"CREATE CLAN\" \r\ngMenu[3].links[1].link = \"static://script.menuscript.CreateClan\" \r\ngMenu[3].links[1].description = \"Create and manage your own clan\" \r\ngMenu[3].links[5].disabled = true \r\nelse \r\ngMenu[3].links[1].title = \"MY CLAN\" \r\ngMenu[3].links[1].link = \"clanURI\" \r\ngMenu[3].links[1].description = \"See everything about your clan\" \r\ngMenu[3].links[5].disabled = false \r\nend \r\nif (gPlayerProfile:ShowBuildInfo()) then \r\ngMenu[5].links[3] = { title = \"ABOUT\", link = \"static://script.menuscript.ShowBuildInfo\", sub_menu_list=nil, description = \"ABOUT\", links = {}} \r\n" +
                                                "end \r\n" +
                                                "for ix =1, getn(gMenu) \r\n" +
                                                "do \r\n" +
                                                "gList:GetItem(ix-1):SetCaption(gMenu[ix].title) \r\nend \r\nRenderSubMenu(gList:GetSelectedIndex()+1) \r\ngDescription = gBrowser:GetTagByName(\"pageSubTitleRight\") \r\n" +
                                                "gMenuDelay = 0.0 \r\ngPanel:SetMarked(true) SetFocusMainMenu() local timezones_uri = gBrowser:ResolveURI(\"viewtimezonesURI\")..\"?lang=\"..gModule:URLEncode(gBrowser:GetLanguageShortName()) \r\ngMenu[5].links[2].link = timezones_uri \r\nlocal chat_restrictions = gBrowserContext:GetApplicationValue(\"display.chatrestrictions\") \r\nif (chat_restrictions and chat_restrictions ~= \"\") then \r\ngBrowserContext:SetApplicationValue(\"display.chatrestrictions\", \"\") \r\n" +
                                                "gBrowserContext:SetPageValue(\"popup.message\", \"Chat is disabled on your <verbatim>PlayStation····Network</verbatim> account due to parental control restrictions.\") \r\n" +
                                                "gBrowserContext:SetPageValue(\"popup.ok\", \"OK\") \r\ngBrowser:OpenPopup(\"file:///kin/menu/popups/popupgeneric.jsp\") \r\n" +
                                                "end \r\n" +
                                                "end \r\n" +
                                                "function OnUpdatePage() \r\n" +
                                                "if (gMenuSelectChange ~= -1 and (gMenuSelectChange + gMenuDelay) < gBrowser:GetTime()) then \r\n" +
                                                "local ix = 1 + gList:GetSelectedIndex() \r\nRenderSubMenu(ix) \r\n" +
                                                "gPanel:SetMarked(true) \r\n" +
                                                "gMenuSelectChange = -1 \r\n" +
                                                "if (not gPanel:GetItem(0):IsSelected()) then \r\n" +
                                                "gPanel:GetItem(0):SetSelectedIndex(-1) \r\n" +
                                                "end \r\n" +
                                                "gMenuDelay = 0.3 \r\nend \r\n" +
                                                "gIsFirstFrame = false \r\n" +
                                                "end \r\n" +
                                                "function RenderSubMenu(id) \r\nlocal menu = gPanel:GetItem(0) \r\n" +
                                                "menu:Clear() \r\n" +
                                                "for ix =1, getn(gMenu[id].links) \r\ndo \r\nlocal item = menu:AddItem() \r\n" +
                                                "item:SetCaption(gMenu[id].links[ix].title) \r\nitem:SetSelectable(not gMenu[id].links[ix].disabled) \r\nend \r\nmenu:Refresh() \r\nend \r\n" +
                                                "function OnPopupOpen(id) \r\nend \r\nfunction OnPopupClose(id, value) \r\n" +
                                                "if (id == \"back\" and value == \"ok\") then \r\n" +
                                                "gBrowser:OpenPage(\"logoutURI\") \r\nend \r\nend \r\nfunction Back() \r\ngBrowserContext:SetPageValue(\"popup.scriptname\", \"menuscript\") \r\n" +
                                                "gBrowserContext:SetPageValue(\"popup.message\", \"Do you want to logout and return to the main menu?\") \r\ngBrowserContext:SetPageValue(\"popup.id\", \"back\") \r\n" +
                                                "gBrowserContext:SetPageValue(\"popup.ok\", \"OK\") \r\ngBrowserContext:SetPageValue(\"popup.cancel\", \"CANCEL\") \r\n" +
                                                "gBrowser:OpenPopup(\"file:///kin/menu/popups/popupgeneric.jsp\") \r\nend \r\nfunction SetFocusSubMenu() \r\nceTrace(\"SetFocusSubMenu\") \r\n" +
                                                "gList:SetSelected(false) \r\ngPanel:GetItem(0):SetSelected(true) \r\ngPanel:GetItem(0):SetSelectedIndex(0) \r\nend \r\nfunction SetFocusMainMenu() \r\n" +
                                                "gList:SetSelected(true) \r\ngPanel:GetItem(0):SetSelectedIndex(-1) \r\ngPanel:GetItem(0):SetSelected(false) \r\nRenderDescription() \r\n" +
                                                "end \r\nfunction GoBack() \r\n" +
                                                "if (gPanel:GetItem(0):GetSelectedIndex()>-1) then \r\nSetFocusMainMenu() \r\nelse \r\nBack() \r\nend \r\nend \r\nfunction ClickAccept() \r\n" +
                                                "if (gPanel:GetSelectedIndex()>-1) then \r\n" +
                                                "local link = gMenu[1+gList:GetSelectedIndex()].links[gPanel:GetItem(0):GetSelectedIndex()+1].link \r\ngBrowser:OpenPage(link) \r\n" +
                                                "end \r\nend \r\nfunction ChangeMainSelection() \r\n" +
                                                "if (not gIsFirstFrame) then \r\ngPanel:SetMarked(false) \r\n" +
                                                "end \r\n" +
                                                "gMenuSelectChange = gBrowser:GetTime() \r\nRenderDescription() \r\n" +
                                                "end \r\nfunction ChangeSubSelection() \r\nRenderDescription() \r\n" +
                                                "end \r\nfunction RenderDescription() \r\nlocal ix = 1 + gList:GetSelectedIndex() \r\n" +
                                                "local id = 1 + gPanel:GetItem(0):GetSelectedIndex() \r\nif (id == 0) then \r\ngDescription:SetValue(gMenu[ix].description) \r\n" +
                                                "else \r\ngDescription:SetValue(gMenu[ix].links[id].description) \r\n" +
                                                "end \r\n" +
                                                "end \r\nfunction CreateClan() \r\n" +
                                                "local ranks_resource = gModule:GetPlayerRanksResource() \r\n" +
                                                "local has_required_rank = ranks_resource and gPlayerProfile:GetMPScore() >= ranks_resource:GetScoreForRank(2) \r\n" +
                                                "if (has_required_rank) then \r\n" +
                                                "gBrowserContext:SetApplicationValue(\"createclan.back\", \"file:///kin/clan/myclan.do\") \r\n" +
                                                "gBrowser:OpenPage(\"clanCreateURI\") \r\n" +
                                                "else \r\n" +
                                                "local rank_title = ranks_resource:GetPlayerRankTitleByID(2) \r\n" +
                                                "local rank_name = gLocalizer:GetUTF8StringByInGameID(rank_title) \r\n" +
                                                "local msg = \"Rank up to [RANK] to be able to create a clan.\" \r\nmsg = gsub( msg, \"%[RANK%]\", rank_name ) \r\ngBrowserContext:SetPageValue(\"popup.message\", msg) \r\ngBrowserContext:SetPageValue(\"popup.ok\", \"OK\") \r\n" +
                                                "gBrowser:OpenPopup(\"file:///kin/menu/popups/popupgeneric.jsp\") \r\nend \r\nend \r\nfunction ShowBuildInfo() \r\ngBrowser:OpenPage(\"buildInfoURI\") \r\nend \r\n]]></_s> <_g name=\"ButtonLegend\" c=\"Legend\" p=\"196,620,956,100\" />" +
                                                " <_s name=\"buttonlegend\" ><![CDATA[ \r\nglobal gButtonList = { \r\n{ \r\nkey = \"buttonlegend.select\", \r\nimage = \"<buttonimage name=select>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.start\", \r\nimage = \"<buttonimage name=start>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.L1\", \r\nimage = \"<buttonimage name=L1>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.R1\", \r\nimage = \"<buttonimage name=R1>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.L2R2\", \r\nimage = \"<buttonimage name=L2><buttonimage name=R2>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.L2\", \r\nimage = \"<buttonimage name=L2>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.R2\", \r\nimage = \"<buttonimage name=R2>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.L3\", \r\nimage = \"<buttonimage name=L3>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.R3\", \r\nimage = \"<buttonimage name=R3>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.LAnalog\", \r\nimage = \"<buttonimage name=LAnalog>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.RAnalog\", \r\nimage = \"<buttonimage name=RAnalog>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.down\", \r\nimage = \"<buttonimage name=down>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.up\", \r\nimage = \"<buttonimage name=up>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.leftrightupdown\", \r\nimage = \"<buttonimage name=leftrightupdown>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.updown\", \r\nimage = \"<buttonimage name=updown>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.leftright\", \r\nimage = \"<buttonimage name=leftright>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.triangle\", \r\nimage = \"<buttonimage name=triangle>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.square\", \r\nimage = \"<buttonimage name=square>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.L1R1\", \r\nimage = \"<buttonimage name=L1><buttonimage name=R1>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.ok\", \r\nimage = \"<buttonimage name=ok>\" \r\n}, \r\n{ \r\nkey = \"buttonlegend.cancel\", \r\nimage = \"<buttonimage name=cancel>\" \r\n} \r\n} \r\nglobal gButtonLegend = nil \r\nfunction OnEnterPage() \r\ngButtonLegend = gBrowser:GetTagByName(\"ButtonLegend\") \r\nRenderButtons() \r\nend \r\nfunction OnUpdatePage() \r\nRenderButtons() \r\nend \r\nfunction RenderButtons() \r\nlocal button_string = \"\" \r\nlocal description \r\nfor i=1, getn(gButtonList) \r\ndo \r\nbutton_string = \"\" \r\ndescription = gBrowserContext:GetPageValue(gButtonList[i].key) \r\nif (description) then \r\nbutton_string = gButtonList[i].image .. \" \" .. description \r\nend \r\ngButtonLegend:SetButton(i-1, button_string) \r\nend \r\nend \r\n]]>" +
                                                "</_s>" +
                                                " </SVML>";

                                            resp.OutputStream.Write(Encoding.ASCII.GetBytes(xmlMessage));
#if DEBUG
                                            LoggerAccessor.LogInfo($"Start menu for Killzone 2 SENT!");
#endif
                                        }
                                        break;
                                }

                                break;
                            }

                        case "/KILLZONEPS3_SVML/account/SP_Login_Submit.jsp":
                            switch (context.Request.HttpMethod)
                            {
                                case "POST":
                                    HttpListenerRequest req = context.Request;
                                    HttpListenerResponse resp = context.Response;

                                    string psnname = string.Empty;

                                    resp.Headers.Set("Content-Type", "text/svml;charset=UTF-8");
                                    string clientMac = req.Headers.Get("X-SVOMac");

                                    string sig = HttpUtility.ParseQueryString(req.Url.Query).Get("sig");
                                    string serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    resp.Headers.Set("X-SVOMac", serverMac);
                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        if (!req.HasEntityBody)
                                        {
                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        int appId = Convert.ToInt32(HttpUtility.ParseQueryString(req.Url.Query).Get("applicationID"));
                                        string s = string.Empty;

                                        // Get the data from the HTTP stream
                                        using (StreamReader reader = new(req.InputStream, req.ContentEncoding))
                                        {
                                            // Convert the data to a string and display it on the console.
                                            s = reader.ReadToEnd();
                                            reader.Close();
                                        }

                                        byte[] bytes = Encoding.ASCII.GetBytes(s);
                                        int AcctNameLen = Convert.ToInt32(bytes.GetValue(81));

                                        string acctName = s.Substring(82, 32);

                                        string acctNameREX = Regex.Replace(acctName, @"[^a-zA-Z0-9]+", string.Empty);

#if DEBUG
                                        LoggerAccessor.LogInfo($"Logging user {acctNameREX} into SVO...\n");
#endif
                                        int accountId = -1;
                                        int clanId = -1;
                                        string langId = string.Empty;

                                        try
                                        {
                                            await SVOServerConfiguration.Database.GetAccountByName(acctNameREX, appId).ContinueWith((r) =>
                                            {
                                                //Found in database so keep.
                                                langId = req.Url.Query.Substring(94, req.Url.Query.Length - 94);
                                                if (r.Result != null)
                                                    accountId = r.Result.AccountId;
                                            });
                                        }
                                        catch (Exception)
                                        {
                                            langId = req.Url.Query.Substring(94, req.Url.Query.Length - 94);
                                            accountId = 0;
                                        }

                                        
                                        response.AppendHeader("Set-Cookie", $"AcctID={accountId}");
                                        response.AppendHeader("Set-Cookie", $"NPCountry=us");
                                        response.AppendHeader("Set-Cookie", $"ClanID={clanId}");
                                        response.AppendHeader("Set-Cookie", $"AuthKeyTime=03-11-2023 16:03:41");
                                        response.AppendHeader("Set-Cookie", $"NPLang=1");
                                        response.AppendHeader("Set-Cookie", $"ModerateMode=false");
                                        response.AppendHeader("Set-Cookie", $"TimeZone=PST");
                                        response.AppendHeader("Set-Cookie", $"AssocID=-1");
                                        response.AppendHeader("Set-Cookie", $"NPContentRating=201326592");
                                        response.AppendHeader("Set-Cookie", $"AuthKey=nRqnf97f~UaSANLErurJIzq9GXGWqWCADdA3TfqUIVXXisJyMnHsQ34kA&C^0R#&~JULZ7xUOY*rXW85slhQF&P&Eq$7kSB&VBtf`V8rb^BC`53jGCgIT");
                                        response.AppendHeader("Set-Cookie", $"AcctName={psnname}");
                                        response.AppendHeader("Set-Cookie", $"OwnerID=-255");
                                        response.AppendHeader("Set-Cookie", $"Sig={sig}==");
                                        response.AppendHeader("Set-Cookie", $"AppId={appId}");

                                        string sp_Login = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<SVML>\r\n" +
                                            "    <SP_Login>\r\n" +
                                            "        <status>\r\n" +
                                            "            <id>20600</id>\r\n" +
                                            "            <message>ACCT_LOGIN_SUCCESS</message>\r\n" +
                                            "        </status>\r\n" +
                                           $"        <accountID>{accountId}</accountID>\r\n" +
                                            "        <userContext>0</userContext>\r\n" +
                                            "    </SP_Login>\r\n\t\r\n" +
                                            "</SVML>";

                                        resp.OutputStream.Write(Encoding.ASCII.GetBytes(sp_Login));
#if DEBUG
                                        LoggerAccessor.LogInfo($"Player {psnname} successfully logged into SVO!\n");
#endif
                                    }
                                    break;
                            }
                            break;

                        case "/KILLZONEPS3_SVML/game/Game_Create.jsp":
                            {
                                switch (context.Request.HttpMethod)
                                {
                                    case "GET":
                                        HttpListenerRequest req = context.Request;
                                        HttpListenerResponse resp = context.Response;
                                        resp.Headers.Set("Content-Type", "text/svml");

                                        string clientMac = req.Headers.Get("X-SVOMac");
                                        string serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);
                                        if (string.IsNullOrEmpty(serverMac))
                                        {
                                            response.StatusCode = (int)HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                        {
                                            if (!req.HasEntityBody)
                                            {
                                                response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                                return;
                                            }
                                            resp.Headers.Set("X-SVOMac", serverMac);


                                            string xmlMessage = "<SVML>" +
                                                "<_s name=\"firstpage\" >\r\n\r\n" +
                                                "function OnEnterPage() \r\n" +
                                                "end \r\n \r\n" +
                                                "function ToNextPage()\r\n\r\n" +
                                                "gBrowser:EnableNavigation()\r\n" +
                                                "gBrowser:GetTagByName(\"pregamelobby_popup_body\"):SetVisible(false)\r\n" +
                                                "gBrowser:ClosePopup() \r\n" +
                                                "gSPState = 2 \r\n" +
                                                "gBrowser:OpenPage(\"file:///kin/menu/online/game/spawnselect_1.jsp\")\r\n" +
                                                "end\r\n" +
                                                "</_s>\r\n" +
                                                "</SVML>";

                                            resp.OutputStream.Write(Encoding.ASCII.GetBytes(xmlMessage));
#if DEBUG
                                            LoggerAccessor.LogInfo($"Start game create for Killzone 2 SENT!");
#endif
                                        }
                                        break;
                                }

                                break;
                            }

                        case "/KILLZONEPS3_SVML/account/Logout.jsp":
                            {
                                switch (context.Request.HttpMethod)
                                {
                                    case "GET":
                                        HttpListenerRequest req = context.Request;
                                        HttpListenerResponse resp = context.Response;
                                        resp.Headers.Set("Content-Type", "text/svml");

                                        string clientMac = req.Headers.Get("X-SVOMac");
                                        string serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);
                                        if (serverMac == null)
                                        {
                                            string forbidden = "500 Forbidden";
                                            LoggerAccessor.LogWarn("Client connected abnormally... returning 500 Forbidden");

                                            resp.StatusCode = 500;
                                            resp.OutputStream.Write(Encoding.ASCII.GetBytes(forbidden));
                                        }
                                        else
                                        {
                                            resp.Headers.Set("X-SVOMac", serverMac);

                                            string xmlMessage = "<SVML> <_s name=\"firstpage\" >\r\n" +
                                                "function OnEnterPage()\r\n\r\n" +
                                                "gModule:ExitAll(\"file:///kin/menu/startscreen.jsp\")\r\n" +
                                                "end\r\n" +
                                                "</_s>\r\n" +
                                                " </SVML>";

                                            resp.OutputStream.Write(Encoding.ASCII.GetBytes(xmlMessage));
#if DEBUG
                                            LoggerAccessor.LogInfo($"Start URIStore for Killzone 2 SENT!");
#endif
                                        }
                                        break;
                                }

                                break;
                            }
                    }
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[SVO] - Killzone2_SVOAsync thrown an assertion - {ex}");
                    response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                }
            }
        }
    }

}