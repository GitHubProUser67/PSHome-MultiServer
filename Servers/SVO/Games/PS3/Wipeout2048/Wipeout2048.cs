using CastleLibrary.Sony.XI5;
using CustomLogger;
using MultiServerLibrary.Extension;
using SpaceWizards.HttpListener;
using System.Text;
using System.Text.RegularExpressions;

namespace SVO.Games.PS3
{
    public class Wipeout2048
    {
        public static async Task Wipeout2048_OTG(HttpListenerRequest request, HttpListenerResponse response)
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
                        #region Wipeout2048
                        case "/wox_ws/rest/main/Start":

                            switch (method)
                            {
                                case "GET":
                                    response.SendChunked = true;

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    string domain = "wipeout2048.online.scee.com";

                                    if (!SVOServerConfiguration.PreferDNSUrls)
                                        await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                    byte[] start = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<Start>" +
                                            $"<DATA dataType=\"URI\" name=\"startURL\" value=\"https://{domain}:10062/wox_ws/rest/main/Start\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"ticketLoginURL\" value=\"https://{domain}:10062/wox_ws/rest/account/TicketLogin\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"friendsUploadURL\" value=\"https://{domain}:10062/wox_ws/rest/account/Friends\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"friendsDownloadURL\" value=\"https://{domain}:10062/wox_ws/rest/account/Friends\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"binaryUploadURL\" value=\"https://{domain}:10062/wox_ws/rest/binary/Upload\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"binaryDownloadURL\" value=\"https://{domain}:10062/wox_ws/rest/binary/Download\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"eulaURL\" value=\"https://{domain}:10062/wox_ws/rest/legal/Eula\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"announcementsURL\" value=\"https://{domain}:10062/wox_ws/rest/legal/Announcements\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"postScoreURL\" value=\"https://{domain}:10062/wox_ws/rest/games/PostScore\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"postWO2048ScoreURL\" value=\"https://{domain}:10062/wox_ws/rest/games/PostWO2048Score\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"leaderboardsURL\" value=\"https://{domain}:10062/wox_ws/rest/lb/{{leaderboardId}}\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"leaderboardsListURL\" value=\"https://{domain}:10062/wox_ws/rest/lb/GetList\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"leaderboardsPageURL\" value=\"https://{domain}:10062/wox_ws/rest/lb/GetPage\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"mediusStatsURL\" value=\"https://{domain}:10062/wox_ws/rest/lb/GetMediusStats\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"playerTimesURL\" value=\"https://{domain}:10062/wox_ws/rest/lb/GetPlayerTimes\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"guessRankingURL\" value=\"https://{domain}:10062/wox_ws/rest/lb/GuessRanking\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"rankedConfigURL\" value=\"https://{domain}:10062/wox_ws/rest/lb/GetRankedConfig\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"leaderboardScoreRangeURL\" value=\"https://{domain}:10062/wox_ws/rest/lbNGP/GetLeaderboardRange\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"leaderboards2048URL\" value=\"https://{domain}:10062/wox_ws/rest/lbNGP/{{leaderboardId}}\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"leaderboards2048PageURL\" value=\"https://{domain}:10062/wox_ws/rest/lbNGP/GetPage\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"leaderboards2048XPURL\" value=\"https://{domain}:10062/wox_ws/rest/lbNGP/GetXPLeaderboard\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"ghostUploadURL\" value=\"https://{domain}:10062/wox_ws/rest/ghost/Upload\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"ghostDownloadURL\" value=\"https://{domain}:10062/wox_ws/rest/ghost/Download\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"friendActivitiesURL\" value=\"https://{domain}:10062/wox_ws/rest/activities/FriendActivities\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"uploadEventsURL\" value=\"https://{domain}:10062/wox_ws/rest/activities/UploadEvents\"/>" +
                                            $"<DATA dataType=\"URI\" name=\"frameDataUploadURL\" value=\"https://{domain}:10062/FrameRateAnalizer/gui/main/uploadFrameData\"/>" +
                                            "</Start>");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = start.Length;
                                            response.OutputStream.Write(start, 0, start.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        case "/wox_ws/rest/lb/GetRankedConfig":

                            switch (method)
                            {
                                case "GET":
                                    response.SendChunked = true;

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    byte[] RankedConfig = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<GetRankedConfig>\r\n\t" +
                                            "<stage1>\r\n\t\t" +
                                            "<modes size=\"8\">\r\n\t\t\t" +
                                            "<mode name=\"Multiplayer Single Race\" minplayer=\"5\" maxplayer=\"8\">\r\n\t\t\t\t" +
                                            "<points position=\"1\" value=\"16\"/>\r\n\t\t\t\t" +
                                            "<points position=\"2\" value=\"14\"/>\r\n\t\t\t\t" +
                                            "<points position=\"3\" value=\"12\"/>\r\n\t\t\t\t" +
                                            "<points position=\"4\" value=\"10\"/>\r\n\t\t\t\t" +
                                            "<points position=\"5\" value=\"8\"/>\r\n\t\t\t\t" +
                                            "<points position=\"6\" value=\"6\"/>\r\n\t\t\t\t" +
                                            "<points position=\"7\" value=\"4\"/>\r\n\t\t\t\t" +
                                            "<points position=\"8\" value=\"2\"/>\r\n\t\t\t" +
                                            "</mode>\r\n\r\n\t\t\t" +
                                            "<mode name=\"Multiplayer Single Race\" minplayer=\"2\" maxplayer=\"4\">\r\n\t\t\t\t" +
                                            "<points position=\"1\" value=\"10\"/>\r\n\t\t\t\t" +
                                            "<points position=\"2\" value=\"8\"/>\r\n\t\t\t\t" +
                                            "<points position=\"3\" value=\"6\"/>\r\n\t\t\t\t" +
                                            "<points position=\"4\" value=\"4\"/>\r\n\t\t\t" +
                                            "</mode>\r\n\r\n\t\t\t" +
                                            "<mode name=\"Multiplayer Tournament\" minplayer=\"5\" maxplayer=\"8\">\r\n\t\t\t\t" +
                                            "<points position=\"1\" value=\"16\"/>\r\n\t\t\t\t" +
                                            "<points position=\"2\" value=\"14\"/>\r\n\t\t\t\t" +
                                            "<points position=\"3\" value=\"12\"/>\r\n\t\t\t\t" +
                                            "<points position=\"4\" value=\"10\"/>\r\n\t\t\t\t" +
                                            "<points position=\"5\" value=\"8\"/>\r\n\t\t\t\t" +
                                            "<points position=\"6\" value=\"6\"/>\r\n\t\t\t\t" +
                                            "<points position=\"7\" value=\"4\"/>\r\n\t\t\t\t" +
                                            "<points position=\"8\" value=\"2\"/>\r\n\t\t\t" +
                                            "</mode>\r\n\r\n\t\t\t" +
                                            "<mode name=\"Multiplayer Tournament\" minplayer=\"2\" maxplayer=\"4\">\r\n\t\t\t\t" +
                                            "<points position=\"1\" value=\"10\"/>\r\n\t\t\t\t" +
                                            "<points position=\"2\" value=\"8\"/>\r\n\t\t\t\t" +
                                            "<points position=\"3\" value=\"6\"/>\r\n\t\t\t\t" +
                                            "<points position=\"4\" value=\"4\"/>\r\n\t\t\t" +
                                            "</mode>\r\n\r\n\t\t\t" +
                                            "<mode name=\"Multiplayer Elimination\" minplayer=\"5\" maxplayer=\"8\">\r\n\t\t\t\t" +
                                            "<points position=\"1\" value=\"16\"/>\r\n\t\t\t\t" +
                                            "<points position=\"2\" value=\"14\"/>\r\n\t\t\t\t" +
                                            "<points position=\"3\" value=\"12\"/>\r\n\t\t\t\t" +
                                            "<points position=\"4\" value=\"10\"/>\r\n\t\t\t\t" +
                                            "<points position=\"5\" value=\"8\"/>\r\n\t\t\t\t" +
                                            "<points position=\"6\" value=\"6\"/>\r\n\t\t\t\t" +
                                            "<points position=\"7\" value=\"4\"/>\r\n\t\t\t\t" +
                                            "<points position=\"8\" value=\"2\"/>\r\n\t\t\t" +
                                            "</mode>\r\n\r\n\t\t\t" +
                                            "<mode name=\"Multiplayer Elimination\" minplayer=\"2\" maxplayer=\"4\">\r\n\t\t\t\t" +
                                            "<points position=\"1\" value=\"10\"/>\r\n\t\t\t\t" +
                                            "<points position=\"2\" value=\"8\"/>\r\n\t\t\t\t" +
                                            "<points position=\"3\" value=\"6\"/>\r\n\t\t\t\t" +
                                            "<points position=\"4\" value=\"4\"/>\r\n\t\t\t" +
                                            "</mode>\r\n\r\n\t\t\t" +
                                            "<mode name=\"Multiplayer Nitro Battle\" minplayer=\"5\" maxplayer=\"8\">\r\n\t\t\t\t" +
                                            "<points position=\"1\" value=\"16\"/>\r\n\t\t\t\t" +
                                            "<points position=\"2\" value=\"14\"/>\r\n\t\t\t\t" +
                                            "<points position=\"3\" value=\"12\"/>\r\n\t\t\t\t" +
                                            "<points position=\"4\" value=\"10\"/>\r\n\t\t\t\t" +
                                            "<points position=\"5\" value=\"8\"/>\r\n\t\t\t\t" +
                                            "<points position=\"6\" value=\"6\"/>\r\n\t\t\t\t" +
                                            "<points position=\"7\" value=\"4\"/>\r\n\t\t\t\t" +
                                            "<points position=\"8\" value=\"2\"/>\r\n\t\t\t" +
                                            "</mode>\r\n\r\n\t\t\t" +
                                            "<mode name=\"Multiplayer Nitro Battle\" minplayer=\"2\" maxplayer=\"4\">\r\n\t\t\t\t" +
                                            "<points position=\"1\" value=\"10\"/>\r\n\t\t\t\t" +
                                            "<points position=\"2\" value=\"8\"/>\r\n\t\t\t\t" +
                                            "<points position=\"3\" value=\"6\"/>\r\n\t\t\t\t" +
                                            "<points position=\"4\" value=\"4\"/>\r\n\t\t\t" +
                                            "</mode>\r\n\t\t" +
                                            "</modes>\r\n\r\n\t" +
                                            "</stage1>\r\n\r\n\r\n\t" +
                                            "<stage2>\r\n\t\t" +
                                            "<basepoints>10</basepoints>\r\n\t\t" +
                                            "<quitpoints>100</quitpoints>\r\n\t\t" +
                                            "<tournypoints>50</tournypoints>\r\n\t\t" +
                                            "<tournyraces>4</tournyraces>\r\n\t\t" +
                                            "<relrankpoints>10</relrankpoints>\r\n\t" +
                                            "</stage2>\r\n\r\n\t" +
                                            "<stage3 awards=\"5\" badges=\"0\">\r\n\t\t" +
                                            "<table type=\"award\" category=\"weapons\" size=\"18\">\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_QUAKER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_ROCKETEER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_MISSILER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_MINER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_BOMBER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_CANNONEER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_PLASMAIFIER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_LEECHER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_QUAKE_WIPEOUT\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_ROCKET_WIPEOUT\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_MISSILE_WIPEOUT\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_MINE_WIPEOUT\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_BOMB_WIPEOUT\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_CANNON_WIPEOUT\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_PLASMA_WIPEOUT\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_LEECH_WIPEOUT\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_FURY\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_ANNIHILATOR\" value=\"0\"/>\r\n\t\t" +
                                            "</table>\r\n\r\n\t\t" +
                                            "<table type=\"award\" category=\"onlineplayteams\" size=\"5\">\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_RECRUIT\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_REGULAR\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_VETERAN\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_LEGEND\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_ZONER\" value=\"0\"/>\r\n\t\t" +
                                            "</table>\r\n\r\n\r\n\r\n\t\t" +
                                            "<table type=\"award\" category=\"tournaments\" size=\"10\">\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_POISONOUS\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_SHINY\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_SHARP\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_SPOOKY\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_POISHINARPOOKY\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_DEADLY\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_DAZZLER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_BARBED\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_NIGHTMARE\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_DEZZARMARE\" value=\"0\"/>\r\n\t\t" +
                                            "</table>\r\n\r\n\t\t" +
                                            "<table type=\"award\" category=\"tracks\" size=\"8\">\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_WINDOW_SHOPPER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_ESKIMO\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_PASSENGER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_CITYSLICKER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_SCUBA_DIVER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_BEACH_BUM\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_HIGH_ROLLER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_AIR_CADET\" value=\"0\"/>\r\n\t\t" +
                                            "</table>\r\n\r\n\r\n\t\t" +
                                            "<table type=\"award\" category=\"badges\" size=\"31\">\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_COME_BACK\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_SURVIVOR\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_GLOBETROTTER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_SPIROGYRA\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_YEAR_TO_REMEMBER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_DEFENDER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_PROTECTOR\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_LAZY\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_SONIC_BOOM\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_WARP\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_UNQUAKABLE\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_UNSTOPPABLE\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_UNBREAKABLE\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_INDESTRUCTIBLE\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_UNBEATABLE\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_NEAR_DEATH_EXPERIENCE\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_MINE_SWEEPER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_BOMB_DISPOSER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_FEROCIOUS\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_PHOTO_FINISH\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_PRISTINE\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_AIRTIME\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_REFLEX\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_TRANSONIC\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_CLOSE_CALL\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_RUTHLESS\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_DESTRUCTION_DEALER\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_DESTRUCTION_DEVA\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_TORTURE\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_SUBSONIC\" value=\"0\"/>\r\n\t\t\t" +
                                            "<entry type=\"BADGE_NAME_JUST_IN_TIME\" value=\"0\"/>\r\n\t\t" +
                                            "</table>\r\n\r\n\t" +
                                            "</stage3>\r\n\r\n\t" +
                                            "<stage4>\r\n\t\t" +
                                            "<scaler size=\"4\">\r\n\t\t\t" +
                                            "<multiplier class=\"Venom\" value=\"1\"/>\r\n\t\t\t" +
                                            "<multiplier class=\"Flash\" value=\"1\"/>\r\n\t\t\t" +
                                            "<multiplier class=\"Rapier\" value=\"1.2\"/>\r\n\t\t\t" +
                                            "<multiplier class=\"Phantom\" value=\"1.2\"/>\r\n\t\t" +
                                            "</scaler>\r\n\t" +
                                            "</stage4>\r\n\r\n\t" +
                                            "<stage5 maxlevels=\"101\" maxpoints=\"2199999\">\r\n\t\t" +
                                            "<table>\r\n\t\t\t" +
                                            "<rank level=\"1\" minpoints=\"1\" maxpoints=\"199\" name=\"RANK_TRAINEE_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"2\" minpoints=\"200\" maxpoints=\"399\" name=\"RANK_TRAINEE_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"3\" minpoints=\"400\" maxpoints=\"599\" name=\"RANK_TRAINEE_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"4\" minpoints=\"600\" maxpoints=\"799\" name=\"RANK_TRAINEE_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"5\" minpoints=\"800\" maxpoints=\"999\" name=\"RANK_TRAINEE_5\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"6\" minpoints=\"1000\" maxpoints=\"1499\" name=\"RANK_ROOKIE_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"7\" minpoints=\"1500\" maxpoints=\"1999\" name=\"RANK_ROOKIE_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"8\" minpoints=\"2000\" maxpoints=\"2499\" name=\"RANK_ROOKIE_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"9\" minpoints=\"2500\" maxpoints=\"2999\" name=\"RANK_ROOKIE_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"10\" minpoints=\"3000\" maxpoints=\"3499\" name=\"RANK_ROOKIE_5\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"11\" minpoints=\"3500\" maxpoints=\"4499\" name=\"RANK_AMATEUR_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"12\" minpoints=\"4500\" maxpoints=\"5499\" name=\"RANK_AMATEUR_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"13\" minpoints=\"5500\" maxpoints=\"6499\" name=\"RANK_AMATEUR_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"14\" minpoints=\"6500\" maxpoints=\"7499\" name=\"RANK_AMATEUR_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"15\" minpoints=\"7500\" maxpoints=\"8499\" name=\"RANK_AMATEUR_5\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"16\" minpoints=\"8500\" maxpoints=\"9999\" name=\"RANK_PROFESSIONAL_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"17\" minpoints=\"10000\" maxpoints=\"11499\" name=\"RANK_PROFESSIONAL_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"18\" minpoints=\"11500\" maxpoints=\"12999\" name=\"RANK_PROFESSIONAL_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"19\" minpoints=\"13000\" maxpoints=\"14499\" name=\"RANK_PROFESSIONAL_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"20\" minpoints=\"14500\" maxpoints=\"15999\" name=\"RANK_PROFESSIONAL_5\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"21\" minpoints=\"16000\" maxpoints=\"17999\" name=\"RANK_EXPERT_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"22\" minpoints=\"18000\" maxpoints=\"19999\" name=\"RANK_EXPERT_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"23\" minpoints=\"20000\" maxpoints=\"21999\" name=\"RANK_EXPERT_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"24\" minpoints=\"22000\" maxpoints=\"23999\" name=\"RANK_EXPERT_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"25\" minpoints=\"24000\" maxpoints=\"25999\" name=\"RANK_EXPERT_5\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"26\" minpoints=\"26000\" maxpoints=\"29999\" name=\"RANK_VETERAN_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"27\" minpoints=\"30000\" maxpoints=\"33999\" name=\"RANK_VETERAN_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"28\" minpoints=\"34000\" maxpoints=\"37999\" name=\"RANK_VETERAN_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"29\" minpoints=\"38000\" maxpoints=\"41999\" name=\"RANK_VETERAN_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"30\" minpoints=\"42000\" maxpoints=\"45999\" name=\"RANK_VETERAN_5\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"31\" minpoints=\"46000\" maxpoints=\"51999\" name=\"RANK_MASTER_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"32\" minpoints=\"52000\" maxpoints=\"57999\" name=\"RANK_MASTER_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"33\" minpoints=\"58000\" maxpoints=\"63999\" name=\"RANK_MASTER_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"34\" minpoints=\"64000\" maxpoints=\"69999\" name=\"RANK_MASTER_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"35\" minpoints=\"70000\" maxpoints=\"75999\" name=\"RANK_MASTER_5\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"36\" minpoints=\"76000\" maxpoints=\"83999\" name=\"RANK_ELITE_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"37\" minpoints=\"84000\" maxpoints=\"91999\" name=\"RANK_ELITE_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"38\" minpoints=\"92000\" maxpoints=\"99999\" name=\"RANK_ELITE_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"39\" minpoints=\"100000\" maxpoints=\"107999\" name=\"RANK_ELITE_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"40\" minpoints=\"108000\" maxpoints=\"115999\" name=\"RANK_ELITE_5\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"41\" minpoints=\"116000\" maxpoints=\"125999\" name=\"RANK_HERO_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"42\" minpoints=\"126000\" maxpoints=\"135999\" name=\"RANK_HERO_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"43\" minpoints=\"136000\" maxpoints=\"145999\" name=\"RANK_HERO_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"44\" minpoints=\"146000\" maxpoints=\"155999\" name=\"RANK_HERO_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"45\" minpoints=\"156000\" maxpoints=\"165999\" name=\"RANK_HERO_5\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"46\" minpoints=\"166000\" maxpoints=\"177999\" name=\"RANK_LEGEND_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"47\" minpoints=\"178000\" maxpoints=\"189999\" name=\"RANK_LEGEND_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"48\" minpoints=\"190000\" maxpoints=\"201999\" name=\"RANK_LEGEND_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"49\" minpoints=\"202000\" maxpoints=\"213999\" name=\"RANK_LEGEND_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"50\" minpoints=\"214000\" maxpoints=\"225999\" name=\"RANK_LEGEND_5\"/>\r\n\r\n\r\n\t\t\t" +
                                            "<rank level=\"51\" minpoints=\"226000\" maxpoints=\"237999\" name=\"RANK_CONQUEROR_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"52\" minpoints=\"238000\" maxpoints=\"249999\" name=\"RANK_CONQUEROR_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"53\" minpoints=\"250000\" maxpoints=\"261999\" name=\"RANK_CONQUEROR_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"54\" minpoints=\"262000\" maxpoints=\"273999\" name=\"RANK_CONQUEROR_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"55\" minpoints=\"274000\" maxpoints=\"285999\" name=\"RANK_CONQUEROR_5\"/>\r\n\t\t\t" +
                                            "<rank level=\"56\" minpoints=\"286000\" maxpoints=\"297999\" name=\"RANK_CONQUEROR_6\"/>\r\n\t\t\t" +
                                            "<rank level=\"57\" minpoints=\"298000\" maxpoints=\"309999\" name=\"RANK_CONQUEROR_7\"/>\r\n\t\t\t" +
                                            "<rank level=\"58\" minpoints=\"310000\" maxpoints=\"321999\" name=\"RANK_CONQUEROR_8\"/>\r\n\t\t\t" +
                                            "<rank level=\"59\" minpoints=\"322000\" maxpoints=\"333999\" name=\"RANK_CONQUEROR_9\"/>\r\n\t\t\t" +
                                            "<rank level=\"60\" minpoints=\"334000\" maxpoints=\"345999\" name=\"RANK_CONQUEROR_10\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"61\" minpoints=\"346000\" maxpoints=\"357999\" name=\"RANK_GURU_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"62\" minpoints=\"358000\" maxpoints=\"369999\" name=\"RANK_GURU_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"63\" minpoints=\"370000\" maxpoints=\"381999\" name=\"RANK_GURU_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"64\" minpoints=\"382000\" maxpoints=\"393999\" name=\"RANK_GURU_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"65\" minpoints=\"394000\" maxpoints=\"405999\" name=\"RANK_GURU_5\"/>\r\n\t\t\t" +
                                            "<rank level=\"66\" minpoints=\"406000\" maxpoints=\"417999\" name=\"RANK_GURU_6\"/>\r\n\t\t\t" +
                                            "<rank level=\"67\" minpoints=\"418000\" maxpoints=\"429999\" name=\"RANK_GURU_7\"/>\r\n\t\t\t" +
                                            "<rank level=\"68\" minpoints=\"430000\" maxpoints=\"441999\" name=\"RANK_GURU_8\"/>\r\n\t\t\t" +
                                            "<rank level=\"69\" minpoints=\"442000\" maxpoints=\"453999\" name=\"RANK_GURU_9\"/>\r\n\t\t\t" +
                                            "<rank level=\"70\" minpoints=\"454000\" maxpoints=\"465999\" name=\"RANK_GURU_10\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"71\" minpoints=\"466000\" maxpoints=\"477999\" name=\"RANK_SAGE_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"72\" minpoints=\"478000\" maxpoints=\"489999\" name=\"RANK_SAGE_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"73\" minpoints=\"490000\" maxpoints=\"501999\" name=\"RANK_SAGE_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"74\" minpoints=\"502000\" maxpoints=\"513999\" name=\"RANK_SAGE_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"75\" minpoints=\"514000\" maxpoints=\"525999\" name=\"RANK_SAGE_5\"/>\r\n\t\t\t" +
                                            "<rank level=\"76\" minpoints=\"526000\" maxpoints=\"537999\" name=\"RANK_SAGE_6\"/>\r\n\t\t\t" +
                                            "<rank level=\"77\" minpoints=\"538000\" maxpoints=\"549999\" name=\"RANK_SAGE_7\"/>\r\n\t\t\t" +
                                            "<rank level=\"78\" minpoints=\"550000\" maxpoints=\"561999\" name=\"RANK_SAGE_8\"/>\r\n\t\t\t" +
                                            "<rank level=\"79\" minpoints=\"562000\" maxpoints=\"573999\" name=\"RANK_SAGE_9\"/>\r\n\t\t\t" +
                                            "<rank level=\"80\" minpoints=\"574000\" maxpoints=\"585999\" name=\"RANK_SAGE_10\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"81\" minpoints=\"586000\" maxpoints=\"597999\" name=\"RANK_SAVANT_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"82\" minpoints=\"598000\" maxpoints=\"609999\" name=\"RANK_SAVANT_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"83\" minpoints=\"610000\" maxpoints=\"621999\" name=\"RANK_SAVANT_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"84\" minpoints=\"622000\" maxpoints=\"633999\" name=\"RANK_SAVANT_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"85\" minpoints=\"634000\" maxpoints=\"645999\" name=\"RANK_SAVANT_5\"/>\r\n\t\t\t" +
                                            "<rank level=\"86\" minpoints=\"646000\" maxpoints=\"657999\" name=\"RANK_SAVANT_6\"/>\r\n\t\t\t" +
                                            "<rank level=\"87\" minpoints=\"658000\" maxpoints=\"669999\" name=\"RANK_SAVANT_7\"/>\r\n\t\t\t" +
                                            "<rank level=\"88\" minpoints=\"670000\" maxpoints=\"681999\" name=\"RANK_SAVANT_8\"/>\r\n\t\t\t" +
                                            "<rank level=\"89\" minpoints=\"682000\" maxpoints=\"693999\" name=\"RANK_SAVANT_9\"/>\r\n\t\t\t" +
                                            "<rank level=\"90\" minpoints=\"694000\" maxpoints=\"705999\" name=\"RANK_SAVANT_10\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"91\" minpoints=\"706000\" maxpoints=\"717999\" name=\"RANK_IMMORTAL_1\"/>\r\n\t\t\t" +
                                            "<rank level=\"92\" minpoints=\"718000\" maxpoints=\"729999\" name=\"RANK_IMMORTAL_2\"/>\r\n\t\t\t" +
                                            "<rank level=\"93\" minpoints=\"730000\" maxpoints=\"741999\" name=\"RANK_IMMORTAL_3\"/>\r\n\t\t\t" +
                                            "<rank level=\"94\" minpoints=\"742000\" maxpoints=\"753999\" name=\"RANK_IMMORTAL_4\"/>\r\n\t\t\t" +
                                            "<rank level=\"95\" minpoints=\"754000\" maxpoints=\"765999\" name=\"RANK_IMMORTAL_5\"/>\r\n\t\t\t" +
                                            "<rank level=\"96\" minpoints=\"766000\" maxpoints=\"777999\" name=\"RANK_IMMORTAL_6\"/>\r\n\t\t\t" +
                                            "<rank level=\"97\" minpoints=\"778000\" maxpoints=\"789999\" name=\"RANK_IMMORTAL_7\"/>\r\n\t\t\t" +
                                            "<rank level=\"98\" minpoints=\"790000\" maxpoints=\"801999\" name=\"RANK_IMMORTAL_8\"/>\r\n\t\t\t" +
                                            "<rank level=\"99\" minpoints=\"802000\" maxpoints=\"813999\" name=\"RANK_IMMORTAL_9\"/>\r\n\t\t\t" +
                                            "<rank level=\"100\" minpoints=\"814000\" maxpoints=\"873999\" name=\"RANK_IMMORTAL_10\"/>\r\n\r\n\t\t\t" +
                                            "<rank level=\"101\" minpoints=\"874000\" maxpoints=\"899999\" name=\"RANK_OVERLORD_1\"/>\r\n\r\n\t\t" +
                                            "</table>\r\n\r\n\t" +
                                            "</stage5>\r\n" +
                                            "</GetRankedConfig>");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = RankedConfig.Length;
                                            response.OutputStream.Write(RankedConfig, 0, RankedConfig.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        case "/wox_ws/rest/legal/Eula":

                            switch (method)
                            {
                                case "GET":
                                    response.SendChunked = true;

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    byte[] Eula = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                        "<Eula>" +
                                        "Online Interactions Not Rated by the ESRB\r\n\r\n" +
                                        "ONLINE USER AGREEMENT\r\n\r\n" +
                                        "PLEASE READ THE ENTIRE AGREEMENT AND INDICATE WHETHER OR NOT YOU AGREE TO ITS TERMS BY CLICKING THE 'ACCEPT' OR 'DECLINE' BUTTON AT THE BOTTOM OF THIS SCREEN. " +
                                        "IF YOU CLICK THE 'DECLINE' BUTTON YOU WILL NOT BE ABLE TO PLAY THE ONLINE VERSION OF THIS GAME. THERE ARE NO REFUNDS OF THE ONLINE VERSION OF THIS GAME, SO PLEASE READ THE USER AGREEMENT PRIOR TO PURCHASE. FOR THE PACKAGED VERSION OF THE GAME, CHECK WITH SONY COMPUTER ENTERTAINMENT AMERICA INC. ('SCEA') CONSUMER SERVICES AT 1-800-345-7669 FOR REFUND OR RETURN INFORMATION. PLEASE HAVE YOUR PURCHASE RECEIPT AVAILABLE.\r\n\r\n" +
                                        "1. ACCEPTANCE OF AGREEMENT. This Agreement can be accepted only by an adult 18 years or older. By clicking the 'ACCEPT' button, you affirm that you are over 18 years old and you are accepting this Agreement on your own behalf or on behalf of your minor child (under 18).\r\n\r\n" +
                                        "2. GRANT OF LICENSE. SCEA grants you a non-exclusive right to use this software for personal, non-commercial play on a PlayStation.. computer entertainment system only. You may not (i) rent, lease or sublicense the software, (ii) modify, adapt, translate, reverse engineer, decompile or disassemble the software, (iii) attempt to create the source code from the object code for the software, or (iv) download game content for any purpose other than game play. You have no proprietary rights in any game content including game play statistics. SCEA may modify such content at any time for any reason. This software may include time and use restrictions. For time and use restrictions regarding this software, visit www.us.playstation.comsupportuseragreement.\r\n\r\n" +
                                        "3. AUTHENTICATION/SERVICE. SCEA may retrieve information about a user's hardware and software for authentication, copy protection, account blocking, system monitoring/diagnostics, rule enforcements, game management and other purposes. SCEA does not guarantee the continuous operation of the game servers and shall not be responsible for any delay or failure of the game servers to perform.\r\n\r\n" +
                                        "4. MAINTENANCE AND UPGRADES. Certain versions of hardware operating system software or other firmware ('Firmware') may be necessary in order for this game to play on your PlayStation.. computer entertainment system. The game software on this disc may check for the appropriate Firmware and if it does not find the correct Firmware versions, it may automatically update your Firmware. Without limitation, such automatic updates or upgrades may change your current operating system, cause a loss of data, content, functionalities or utilities. It is recommended that you regularly back up any data located on the hard disk that is of a type that can be backed up. Loss of data is the user's responsibility.\r\n\r\n" +
                                        "5. SEPARATE USER ACCOUNTS/COLLECTION OF INFORMATION. Some games/services may ask you to create an account with a user, player or other game name ('Game Name') and password. You may also be asked to select or provide additional information for a game profile. This information may be provided to any tournament website established by SCEA or its partners in connection with this game. When you choose a Game Name, choose an alias to protect your identity. When you choose a password, choose a unique combination of letters and numbers unrelated to your Game Name or to any information you may share with other players in the game. If your account is inactive for an extended period your account may be deactivated. To inquire about a deactivated account, please contact SCEA Consumer Services at 1-800-345-7669.\r\n\r\n" +
                                        "6. PROTECTION OF IDENTITY/NO EXPECTATION OF PRIVACY. You have no expectation of privacy or confidentiality in the personal information or User Material you may intentionally or unintentionally disclose. You should avoid saying anything personally identifying in chat.\r\n\r\n" +
                                        "7. ONLINE CONDUCT. When you play, you agree to be respectful of your fellow players and never to engage in any behavior that would be abusive or offensive to other players, disruptive of the game experience, fraudulent or otherwise illegal. This includes but is not limited to:\r\n\r\n" +
                                        "(a) Harassing or intimidating other players;\r\n\r\n" +
                                        "(b) Using language, selecting user, character, clan or team names or creating any other content that may be racially, ethnically or religiously offensive, sexually abusive, obscene or defamatory;\r\n\r\n" +
                                        "(c) Selecting as a user, character, clan or team name any word, symbol or combination of words and symbols which is identical to or substantially similar to any character, team, weapon, vehicle or other element which appears in this game;\r\n\r\n" +
                                        "(d) Using content that is commercial in nature such as advertisements, solicitations and promotions for goods or services;\r\n\r\n" +
                                        "(e) Falsely representing that you are an employee of Sony Corporation, SCEA, or any other affiliated or related company;\r\n\r\n" +
                                        "(f) Disrupting the normal flow of chat;\r\n\r\n" +
                                        "(g) Making a false report of user abuse to SCEA Consumer Services;\r\n\r\n" +
                                        "(h) Violating any local, state or national law;\r\n\r\n" +
                                        "(i) Using a cheat code, cheat device or any device that modifies the executable game code or data. For a detailed explanation of the SCEA policy on cheating, visit www.us.playstation.com/onlinecheating.\r\n\r\n" +
                                        "(j) Any attempt to deliberately alter, damage or undermine the legitimate operation of this game, including but not limited to exploiting the ranking system by creating 'dummy accounts'.\r\n\r\n" +
                                        "8. REPORTING ABUSE. To report violations of this Agreement or to inquire about a blocked account, call SCEA Consumer Services at 1-800-345-7669.\r\n\r\n" +
                                        "9. AGREEMENT VIOLATIONS. If you violate this Agreement in any manner, SCEA may, at its discretion and without notice, temporarily or permanently block your account and/or reset your stats and/or rankings in this game and any related games.\r\n\r\n" +
                                        "10. HOTSPOT AND INTERNET SERVICE PROVIDERS. SCEA and its affiliated companies are not associated with any of the Internet service providers (ISPs) including hotspot operators (collectively 'ISPs'). SCEA is not responsible for any damages or injury arising from or related to your use of these ISP services. When you access the Internet, you are providing information directly to the ISP and not to SCEA. This information is collected by the ISP and is not shared with SCEA. Before accessing the Internet, you should carefully read the Service Provider's user agreement/terms and conditions and privacy policy carefully.\r\n\r\n" +
                                        "11. WARRANTY/DISCLAIMER/LIABILITY LIMITATIONS. EXCEPT AS PROVIDED HEREIN, THE SOFTWARE AND ALL RELATED SERVICES ARE PROVIDED 'AS IS' AND, TO THE MAXIMUM EXTENT PROVIDED UNDER LAW, SCEA DISCLAIMS ALL WARRANTIES OF ANY KIND, WHETHER EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO ANY WARRANTIES OF MERCHANTABILITY OR FITNESS FOR A PARTICULAR PURPOSE. Without limiting the foregoing, SCEA does not promise that this software will work properly with all memory card storage or other peripheral devices. From time to time, there may be problems related to availability, access, delay and failure to perform that are beyond the immediate and reasonable control of SCEA. In the event of a dispute regarding the online functionality of this software, you agree that the sole liability of SCEA and its affiliated companies will be limited to repair or replacement of the game software at SCEA's option. SCEA may, at its sole discretion, discontinue hosting the game server at any time. SCEA has no liability for such discontinuance. SCEA has no liability for any violation of this Agreement by you or by any other player.\r\n\r\n" +
                                        "12. MODIFICATION. SCEA at its sole discretion may modify the terms of this Agreement at any time. You are responsible for reviewing the terms of this Agreement each time you log in to play. By accepting this Agreement and by playing the game online, you agree to be bound by all current terms of the Agreement. To print out a current copy of this Agreement using your computer, go to www.us.playstation.com/support/useragreement.\r\n\r\n" +
                                        "13. USER GENERATED CONTENT.\r\n\r\n" +
                                        "This game may include tools that give you the ability to communicate with other players and to post pictures, photographs, videos, game-related materials and other information, either on particular versions of the game you play or in community areas (individually and collectively, 'User Generated Content'). When using these tools, you are solely responsible for your own User Generated Content. By posting or submitting User Generated Content in any manner, you grant SCEA, our affiliates, licensors, and distributors a worldwide, perpetual, non-exclusive, sublicenseable and transferable license to use, copy, display, perform, distribute, adapt, modify and promote the User Generated Content in any medium. You represent and warrant that you own or have all necessary rights and licenses to use and authorize SCEA to use all patent, trademark, trade secret, copyright or other proprietary rights in any and all User Generated Content to enable inclusion and use of the User Generated Content in the manner contemplated by this Agreement.\r\n\r\n" +
                                        "To comply with the terms of this Agreement, you will not submit any User Generated Content that (a) is protected by copyright, patent, trademark, trade secret or otherwise subject to third party proprietary rights, including rights of privacy and publicity (unless you are or have permission from the rightful owner); (b) is fraudulent or a misrepresentation that could damage SCEA or any third party; (c) is unlawful, obscene, defamatory, threatening, harassing, predatory, pornographic, hateful, racially or ethnically offensive, or encourages conduct that would violate any law, or is otherwise inappropriate; (d) is an advertisement or solicitation of business, (e) is an impersonation of another person; or (e) violates any of the rules of Online Conduct, other terms of this Agreement or any other terms related to this game.\r\n\r\n" +
                                        "You agree that SCEA is not responsible or liable for User Generated Content submitted or posted by you or by others. SCEA does not claim ownership of any User Generated Content that you submit or make available as part of the game, and SCEA expressly disclaims any and all liability in connection with any User Generated Content. User Generated Content will belong to you or your licensors. SCEA has no duty to pre-screen User Generated Content. SCEA has the right to edit, remove, or refuse to post any submitted User Generated Content for any reason without prior notice, but is not responsible for any failure or delay in doing so.\r\n\r\n" +
                                        "14. MISCELLANEOUS. This Agreement shall be construed and interpreted in accordance with the laws of the State of California applying to contracts fully executed and performed within the State of California. Both parties submit to personal jurisdiction in California and further agree that any dispute arising from or relating to this Agreement shall be brought in a court within San Mateo County, California. If any provision of this Agreement shall be held invalid or unenforceable, in whole or in part, such provision shall be modified to the minimum extent necessary to make it valid and enforceable, and the validity and enforceability of all other provisions of this Agreement shall not be affected thereby. This Agreement constitutes the entire agreement between the parties related to the subject matter hereof and supersedes all prior oral and written and all contemporaneous oral negotiations, commitments, and understandings of the parties, all of which are merged herein.\r\n\r\n" +
                                        "</Eula>");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = Eula.Length;
                                            response.OutputStream.Write(Eula, 0, Eula.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        case "/wox_ws/rest/legal/Announcements":

                            switch (method)
                            {
                                case "GET":
                                    response.SendChunked = true;

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    byte[] Announcements = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<Announcements><Announcement>WipEout.. HD Patch 2.51 information\r\n\r\n" +
                                            "- Spectate - Zone Battle - Fix crash when cycling through the ships.\r\n" +
                                            "- Restores Online Rank Progress\r\n\r\n" +
                                            "</Announcement></Announcements>");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = Announcements.Length;
                                            response.OutputStream.Write(Announcements, 0, Announcements.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        case "/wox_ws/rest/lb/GetList":

                            switch (method)
                            {
                                case "GET":
                                    response.SendChunked = true;

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    byte[] GetList = Encoding.UTF8.GetBytes(Leaderboard2048Lookup.SerializeLeaderboard(Leaderboard2048Lookup.leaderboard2048_Data));

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = GetList.Length;
                                            response.OutputStream.Write(GetList, 0, GetList.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        case "/wox_ws/rest/lb/GetMediusStats":

                            switch (method)
                            {
                                case "GET":

                                    string name = string.Empty;

                                    string authKey = string.Empty;

                                    string timeZone = string.Empty;

                                    string signature = string.Empty;
                                    string id = string.Empty;

                                    string? cookieHeader = request.Headers.Get("Cookie");

                                    if (cookieHeader != null)
                                    {
                                        response.SendChunked = true;

                                        string[] cookies = cookieHeader.Split(';');

                                        foreach (string cookie in cookies)
                                        {
                                            string[] parts = cookie.Trim().Split('=');

                                            string key = parts[0];
                                            string value = parts[1];

                                            if (key == "name")
                                                name = value;
                                            else if (key == "authKey")
                                                authKey = value;
                                            else if (key == "timeZone")
                                                timeZone = value;
                                            else if (key == "signature")
                                                signature = value;
                                            else if (key == "id")
                                                id = value;
                                        }

                                        response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                        response.Headers.Set("Content-Language", string.Empty);

                                        byte[] MediusStats = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            $"<MediusStats accountId=\"{id}\" accountName=\"{name}\"/>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = MediusStats.Length;
                                                response.OutputStream.Write(MediusStats, 0, MediusStats.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }
                                    else
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                    break;
                            }
                            break;

                        case "/wox_ws/rest/account/Friends":

                            switch (method)
                            {
                                case "POST":

                                    string name = string.Empty;

                                    string authKey = string.Empty;

                                    string timeZone = string.Empty;

                                    string signature = string.Empty;
                                    string id = string.Empty;

                                    string? cookieHeader = request.Headers.Get("Cookie");

                                    if (cookieHeader != null)
                                    {
                                        response.SendChunked = true;

                                        string[] cookies = cookieHeader.Split(';');

                                        foreach (string cookie in cookies)
                                        {
                                            string[] parts = cookie.Trim().Split('=');

                                            string key = parts[0];
                                            string value = parts[1];

                                            if (key == "name")
                                                name = value;
                                            else if (key == "authKey")
                                                authKey = value;
                                            else if (key == "timeZone")
                                                timeZone = value;
                                            else if (key == "signature")
                                                signature = value;
                                            else if (key == "id")
                                                id = value;
                                        }

                                        string contentType = request.ContentType;

                                        if (!string.IsNullOrEmpty(contentType))
                                            response.Headers.Set("Content-Type", contentType);
                                        else
                                            response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");

                                        response.Headers.Set("Content-Language", string.Empty);

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

                                            Directory.CreateDirectory($"{SVOServerConfiguration.SVOStaticFolder}/wox_ws/rest/account");

                                            using (FileStream fs = new($"{SVOServerConfiguration.SVOStaticFolder}/wox_ws/rest/account/Friends.{name}", FileMode.OpenOrCreate))
                                            {
                                                fs.Write(buffer, 0, buffer.Length);
                                                fs.Flush();
                                            }

                                            byte[] Friends = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                               "<Friends><status value=\"0\"/></Friends>");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                            if (response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    response.ContentLength64 = Friends.Length;
                                                    response.OutputStream.Write(Friends, 0, Friends.Length);
                                                }
                                                catch (Exception)
                                                {
                                                    // Not Important;
                                                }
                                            }
                                        }
                                    }
                                    else
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                    break;
                            }
                            break;

                        case "/wox_ws/rest/account/TicketLogin":

                            switch (method)
                            {
                                case "POST":
                                    response.SendChunked = true;

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
                                            LoggerAccessor.LogWarn($"[OTG] - Wipeout2048 : User {psnname} tried to alter their ticket data");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }

                                        // RPCN
                                        if (ticket.IsSignedByRPCN)
                                        {
                                            LoggerAccessor.LogInfo($"[OTG] - Wipeout2048 : User {psnname} connected at: {DateTime.Now} and is on RPCN");

                                            psnname += $"@{XI5Ticket.RPCNSigner}";
                                        }
                                        else if (psnname.EndsWith($"@{XI5Ticket.RPCNSigner}"))
                                        {
                                            LoggerAccessor.LogError($"[OTG] - Wipeout2048 : User {psnname} was caught using a RPCN suffix while not on it!");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                            LoggerAccessor.LogInfo($"[OTG] - Wipeout2048 : User {psnname} connected at: {DateTime.Now} and is on PSN");

                                        ms.Flush();
                                    }

                                    string langId = "0";

                                    try
                                    {
                                        await SVOServerConfiguration.Database.GetAccountByName(psnname, 23360).ContinueWith((r) =>
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
                                        $"   <accountID>{accountId}</accountID>\r\n\t" +
                                        $"   <languageID>{languageId}</languageID>\r\n" +
                                        $"   <userContext>{userContext}</userContext> \r\n" +
                                        "</SP_Login>");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;

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

                        case "/wox_ws/rest/lb/GetPlayerTimes":

                            switch (method)
                            {
                                case "GET":
                                    response.SendChunked = true;

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    byte[] GetPlayerTimes = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<GetPlayerTimes><status value=\"0\"/></GetPlayerTimes>");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = GetPlayerTimes.Length;
                                            response.OutputStream.Write(GetPlayerTimes, 0, GetPlayerTimes.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        case "/wox_ws/rest/lb/GetPage":

                            switch (method)
                            {
                                case "GET":
                                    response.SendChunked = true;

                                    string leaderboardId = string.Empty;

                                    string row = string.Empty;

                                    string pageSize = string.Empty;

                                    string accountName = string.Empty;

                                    string accountId = string.Empty;

                                    string filterMode = string.Empty;

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

                                        if (key == "leaderboardId")
                                            leaderboardId = value;
                                        else if (key == "row")
                                            row = value;
                                        else if (key == "pageSize")
                                            pageSize = value;
                                        else if (key == "accountName")
                                            accountName = value;
                                        else if (key == "accountId")
                                            accountId = value;
                                        else if (key == "filterMode")
                                            filterMode = value;
                                    }

                                    response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                    response.Headers.Set("Content-Language", string.Empty);

                                    // TODO, implements real leaderboard function.
                                    byte[] GetPageData = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<Leaderboard>" +
                                            "<GetPage>" +
                                            $"{Leaderboard2048Lookup.SerializeLeaderboardEntryById(Leaderboard2048Lookup.leaderboard2048_Data, leaderboardId)}" +
                                            "<Stats isFirst=\"true\" isLast=\"true\" row=\"0\" page=\"1\" size=\"1\" leaderboardSize=\"114585\" totalEntries=\"0\">" +
                                            "</Stats>" +
                                            "</GetPage>" +
                                            "</Leaderboard>");

                                    response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                    if (response.OutputStream.CanWrite)
                                    {
                                        try
                                        {
                                            response.ContentLength64 = GetPageData.Length;
                                            response.OutputStream.Write(GetPageData, 0, GetPageData.Length);
                                        }
                                        catch (Exception)
                                        {
                                            // Not Important;
                                        }
                                    }

                                    break;
                            }
                            break;

                        case "/wox_ws/rest/lbNGP/GetXPLeaderboard":

                            switch (method)
                            {
                                case "GET":

                                    string name = string.Empty;

                                    string authKey = string.Empty;

                                    string timeZone = string.Empty;

                                    string signature = string.Empty;
                                    string id = string.Empty;

                                    string? cookieHeader = request.Headers.Get("Cookie");

                                    if (cookieHeader != null)
                                    {
                                        response.SendChunked = true;

                                        string[] cookies = cookieHeader.Split(';');

                                        foreach (string cookie in cookies)
                                        {
                                            string[] parts = cookie.Trim().Split('=');

                                            string key = parts[0];
                                            string value = parts[1];

                                            if (key == "name")
                                                name = value;
                                            else if (key == "authKey")
                                                authKey = value;
                                            else if (key == "timeZone")
                                                timeZone = value;
                                            else if (key == "signature")
                                                signature = value;
                                            else if (key == "id")
                                                id = value;
                                        }

                                        response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                        response.Headers.Set("Content-Language", string.Empty);

                                        byte[] XPLeaderboard = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            $"<XPLeaderboard/>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = XPLeaderboard.Length;
                                                response.OutputStream.Write(XPLeaderboard, 0, XPLeaderboard.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }

                                        break;
                                    }
                                    else
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                    break;
                            }
                            break;

                        case "/wox_ws/rest/games/PostScore":

                            switch (method)
                            {
                                case "GET":

                                    string name = string.Empty;

                                    string authKey = string.Empty;

                                    string timeZone = string.Empty;

                                    string signature = string.Empty;
                                    string id = string.Empty;

                                    string? cookieHeader = request.Headers.Get("Cookie");

                                    if (cookieHeader != null)
                                    {
                                        response.SendChunked = true;

                                        string[] cookies = cookieHeader.Split(';');

                                        foreach (string cookie in cookies)
                                        {
                                            string[] parts = cookie.Trim().Split('=');

                                            string key = parts[0];
                                            string value = parts[1];

                                            if (key == "name")
                                                name = value;
                                            else if (key == "authKey")
                                                authKey = value;
                                            else if (key == "timeZone")
                                                timeZone = value;
                                            else if (key == "signature")
                                                signature = value;
                                            else if (key == "id")
                                                id = value;
                                        }

                                        Dictionary<string, string> parameterDictionary = new(); // Query sent are dynamic for this request.

                                        int questionMarkIndex = request.RawUrl.IndexOf("?");
                                        if (questionMarkIndex != -1) // If '?' is found
                                        {
                                            string trimmedurl = request.RawUrl[(questionMarkIndex + 1)..];
                                            foreach (string? UrlArg in System.Web.HttpUtility.ParseQueryString(trimmedurl).AllKeys) // Thank you WebOne.
                                            {
                                                if (!string.IsNullOrEmpty(UrlArg))
                                                    parameterDictionary[UrlArg] = System.Web.HttpUtility.ParseQueryString(trimmedurl)[UrlArg] ?? string.Empty;
                                            }
                                        }

                                        response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                        response.Headers.Set("Content-Language", string.Empty);

                                        byte[] PostScore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<PostScore>" +
                                            "</PostScore>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = PostScore.Length;
                                                response.OutputStream.Write(PostScore, 0, PostScore.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }

                                        break;
                                    }
                                    else
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                    break;
                            }
                            break;

                        case "/wox_ws/rest/lb/GuessRanking":

                            switch (method)
                            {
                                case "GET":

                                    string name = string.Empty;

                                    string authKey = string.Empty;

                                    string timeZone = string.Empty;

                                    string signature = string.Empty;
                                    string id = string.Empty;

                                    string? cookieHeader = request.Headers.Get("Cookie");

                                    if (cookieHeader != null)
                                    {
                                        response.SendChunked = true;

                                        string[] cookies = cookieHeader.Split(';');

                                        foreach (string cookie in cookies)
                                        {
                                            string[] parts = cookie.Trim().Split('=');

                                            string key = parts[0];
                                            string value = parts[1];

                                            if (key == "name")
                                                name = value;
                                            else if (key == "authKey")
                                                authKey = value;
                                            else if (key == "timeZone")
                                                timeZone = value;
                                            else if (key == "signature")
                                                signature = value;
                                            else if (key == "id")
                                                id = value;
                                        }

                                        string leaderboardId = request.QueryString["leaderboardId"] ?? "-1";

                                        response.Headers.Set("Content-Type", "application/xml;charset=UTF-8");
                                        response.Headers.Set("Content-Language", string.Empty);

                                        // TODO, implements real leaderboard function.
                                        byte[] GuessRankingData = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<Leaderboard>" +
                                            "<GuessRanking>" +
                                            $"{Leaderboard2048Lookup.SerializeLeaderboardEntryById(Leaderboard2048Lookup.leaderboard2048_Data, leaderboardId)}" +
                                            "</GuessRanking>" +
                                            "</Leaderboard>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = GuessRankingData.Length;
                                                response.OutputStream.Write(GuessRankingData, 0, GuessRankingData.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }
                                    else
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                    break;
                            }
                            break;

                        case "/wox_ws/rest/binary/Download":

                            switch (method)
                            {
                                case "GET":
                                    string name = string.Empty;

                                    string authKey = string.Empty;

                                    string timeZone = string.Empty;

                                    string signature = string.Empty;
                                    string id = string.Empty;

                                    string? cookieHeader = request.Headers.Get("Cookie");

                                    if (cookieHeader != null)
                                    {
                                        response.SendChunked = true;

                                        string[] cookies = cookieHeader.Split(';');

                                        foreach (string cookie in cookies)
                                        {
                                            string[] parts = cookie.Trim().Split('=');

                                            string key = parts[0];
                                            string value = parts[1];

                                            if (key == "name")
                                                name = value;
                                            else if (key == "authKey")
                                                authKey = value;
                                            else if (key == "timeZone")
                                                timeZone = value;
                                            else if (key == "signature")
                                                signature = value;
                                            else if (key == "id")
                                                id = value;
                                        }

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                                        response.ContentType = "text/xml";
                                        response.Headers.Set("Content-Language", string.Empty);

                                        string? fileName = request.QueryString["filename"];

                                        if (string.IsNullOrEmpty(fileName))
                                        {
                                            response.StatusCode = (int)System.Net.HttpStatusCode.BadRequest;
                                            break;
                                        }

                                        byte[] fileData;

                                        if (!new Regex(@"\.[^.]+$").Match(fileName).Success) // We give a default extension if none found.
                                            fileName += ".bin";

                                        if (File.Exists($"{SVOServerConfiguration.SVOStaticFolder}/wox_ws/rest/fileservices/{name}/{fileName}"))
                                            fileData = Encoding.UTF8.GetBytes($"<BinaryDownload checksum=\"{CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateOTGSecuredHash("m4nT15")}\">\n" +
                                                    $"        <Data>{File.ReadAllText($"{SVOServerConfiguration.SVOStaticFolder}/wox_ws/rest/fileservices/{name}/{fileName}")}</Data>\n" +
                                                    $"    </BinaryDownload>");
                                        else
                                            fileData = Encoding.UTF8.GetBytes($"<BinaryDownload checksum=\"{CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateOTGSecuredHash("m4nT15")}\">\n" +
                                                    "        <Data></Data>\n" +
                                                    $"    </BinaryDownload>");

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = fileData.Length;
                                                response.OutputStream.Write(fileData, 0, fileData.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }
                                    else
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                    break;
                            }
                            break;

                        case "/wox_ws/rest/activities/FriendActivities":

                            switch (method)
                            {
                                case "GET":
                                    string name = string.Empty;

                                    string authKey = string.Empty;

                                    string timeZone = string.Empty;

                                    string signature = string.Empty;
                                    string id = string.Empty;

                                    string? cookieHeader = request.Headers.Get("Cookie");

                                    if (cookieHeader != null)
                                    {
                                        response.SendChunked = true;

                                        string[] cookies = cookieHeader.Split(';');

                                        foreach (string cookie in cookies)
                                        {
                                            string[] parts = cookie.Trim().Split('=');

                                            string key = parts[0];
                                            string value = parts[1];

                                            if (key == "name")
                                                name = value;
                                            else if (key == "authKey")
                                                authKey = value;
                                            else if (key == "timeZone")
                                                timeZone = value;
                                            else if (key == "signature")
                                                signature = value;
                                            else if (key == "id")
                                                id = value;
                                        }

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                                        response.ContentType = "text/xml";
                                        response.Headers.Set("Content-Language", string.Empty);

                                        string? filter = request.QueryString["filter"];

                                        byte[] FriendActivities = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                           "<FriendActivities>" +
                                           "</FriendActivities>");

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = FriendActivities.Length;
                                                response.OutputStream.Write(FriendActivities, 0, FriendActivities.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }
                                    else
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
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
                LoggerAccessor.LogError($"[OTG] - Wipeout2048_OTG thrown an assertion - {ex}");
                response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }
    }
}
