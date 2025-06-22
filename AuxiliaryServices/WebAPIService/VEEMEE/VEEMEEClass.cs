using NetworkLibrary.HTTP;
using System;
using System.Text;
using WebAPIService.VEEMEE.audi_sled;
using WebAPIService.VEEMEE.audi_vrun;
using WebAPIService.VEEMEE.goalie_sfrgbt;
using WebAPIService.VEEMEE.gofish;
using WebAPIService.VEEMEE.meta;
using WebAPIService.VEEMEE.nml;
using WebAPIService.VEEMEE.olm;
using WebAPIService.VEEMEE.player_profiles;
using WebAPIService.VEEMEE.wardrobe_wars;

namespace WebAPIService.VEEMEE
{
    public class VEEMEEClass
    {
        private string absolutepath;
        private string method;

        public VEEMEEClass(string method, string absolutepath)
        {
            this.absolutepath = absolutepath;
            this.method = method;
        }

        public (byte[], string) ProcessRequest(byte[] postData, string contentType, string apiPath)
        {
            if (string.IsNullOrEmpty(apiPath))
                return (null, null);

            string result = null;
            string resultContentType = null;
#if NET6_0_OR_GREATER
            if(absolutepath.Contains("//WardrobeWars/Images") && method == "GET")
            {
                byte[] resultImage = Podium.RequestWWImage(contentType, apiPath, absolutepath);
                resultContentType = "image/jpeg";
                return (resultImage, resultContentType);
            }
#endif
            switch (method)
            {
                case "POST":
                    switch (absolutepath)
                    {
                        case "/audisledmp/audisledmpHSSetUserData.php":
                            result = SledMpScoreProcessor.SetUserDataPOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audisledmp/audisledmpHSGetUserData.php":
                            result = SledMpScoreProcessor.GetUserDataPOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audisledmp/audisledmpHSGetTopUser.php":
                            result = SledMpScoreProcessor.GetHigherUserScorePOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audisledmp/audisledmpHSGlobalTable.php":
                            result = SledMpScoreProcessor.GetGlobalTablePOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audisled/audisledHSSetUserData.php":
                            result = SledScoreProcessor.SetUserDataPOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audisled/audisledHSGetUserData.php":
                            result = SledScoreProcessor.GetUserDataPOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audisled/audisledHSGetTopUser.php":
                            result = SledScoreProcessor.GetHigherUserScorePOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audisled/audisledHSGlobalTable.php":
                            result = SledScoreProcessor.GetGlobalTablePOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audiHSSetUserData.php":
                            result = VrunScoreProcessor.SetUserDataPOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audiHSGetUserData.php":
                            result = VrunScoreProcessor.GetUserDataPOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audiHSGetTopUser.php":
                            result = VrunScoreProcessor.GetHigherUserScorePOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/audiHSGlobalTable.php":
                            result = VrunScoreProcessor.GetGlobalTablePOST(postData, HTTPProcessor.ExtractBoundary(contentType), apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/MetaScores/setScore.php":
                            result = MetaScores.SetUserDataPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/MetaScores/getScore.php":
                            result = MetaScores.GetUserDataPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/MetaScores/getTrophies.php":
                            result = Trophy.GetUserTrophyDataPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/MetaScores/setTrophy.php":
                            result = Trophy.SetUserTrophyDataPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/MetaScores/getHighScores.php":
                        case "/MetaScores/getHighScoresToday.php":
                        case "/MetaScores/getHighScoresYesterday.php":
                        case "/MetaScores/getHighScoresFriends.php":
                            // TODO: Implements MetaScores dynamic leaderboards!
                            result = "<leaderboard></leaderboard>";
                            resultContentType = "text/xml";
                            break;
                        case "/player_profiles/setPlayerProfile.php":
                            result = Profile_Handler.SetPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/player_profiles/getPlayerProfile.php":
                            result = Profile_Handler.GetPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/goalie/goalieHSSetUserData.php":
                            result = UserData.SetUserDataPOST(postData, contentType, true, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/sfrgbt/sfrgbtHSSetUserData.php":
                            result = UserData.SetUserDataPOST(postData, contentType, false, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/goali/goalieHSGetUserData.php":
                            result = UserData.GetUserDataPOST(postData, contentType, true, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/sfrgbt/sfrgbtHSGetUserData.php":
                            result = UserData.GetUserDataPOST(postData, contentType, false, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/goalie/goalieHSGetLeaderboard.php":
                            result = GSLeaderboard.GetLeaderboardPOST(postData, contentType, true, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/sfrgbt/sfrgbtHSGetLeaderboard.php":
                            result = GSLeaderboard.GetLeaderboardPOST(postData, contentType, false, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/gofish/goFishHSGetFishCaught.php":
                            result = FishCaughtProcess.GetPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/gofish/goFishHSSetFishCaught.php":
                            result = FishCaughtProcess.SetPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/gofish/goFishHSSetUserData.php":
                            result = GFUserData.SetUserDataPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/gofish/goFishHSGetUserData.php":
                            result = GFUserData.GetUserDataPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/gofish/goFishHSGetLeaderboard.php":
                            result = GFLeaderboard.GetLeaderboardPOST(postData, contentType, 2, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/gofish/goFishHSGetLeaderboardToday.php":
                            result = GFLeaderboard.GetLeaderboardPOST(postData, contentType, 0, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/gofish/goFishHSGetLeaderboardYesterday.php":
                            result = GFLeaderboard.GetLeaderboardPOST(postData, contentType, 1, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/olm/olmHSSetUserData.php":
                            result = olmUserData.SetUserDataPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/olm/olmHSGetUserData.php":
                            result = olmUserData.GetUserDataPOST(postData, contentType, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/olm/olmHSGetLeaderboard.php":
                            result = OLMLeaderboard.GetLeaderboardPOST(postData, contentType, 0, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/olm/olmHSGetWeekly.php":
                            result = OLMLeaderboard.GetLeaderboardPOST(postData, contentType, 1, apiPath);
                            resultContentType = "text/xml";
                            break;
                        case "/commerce/get_count.php":
                            result = Commerce.Get_Count();
                            break;
                        case "/commerce/get_ownership.php":
                            result = Commerce.Get_Ownership();
                            break;
                        case "/data/parkChallenges.php":
                            result = Data.ParkChallenges(apiPath);
                            break;
                        case "/data/parkTasks.php":
                            result = Data.ParkTasks(apiPath);
                            break;
                        case "/slot-management/getobjectspace.php":
                            result = Slot.GetObjectSpace(postData, contentType);
                            break;
                        case "/slot-management/getobjectslot.php":
                            result = Slot.GetObjectSlot(postData, contentType);
                            break;
                        case "/slot-management/remove.php":
                            result = Slot.RemoveSlot(postData, contentType);
                            break;
                        case "/slot-management/heartbeat.php":
                            result = Slot.HeartBeat(postData, contentType);
                            break;
                        case "/stats/getconfig.php":
                            result = Stats.GetConfig(false, postData, contentType, apiPath);
                            break;
                        case "/stats/crash.php":
                            result = Stats.Crash(postData, contentType, apiPath);
                            break;
                        case "/stats_tracking/usage.php":
                            result = Stats.Usage(postData, contentType);
                            break;
                        case "/storage/readconfig.php":
                            result = Storage.ReadConfig(postData, contentType, apiPath);
                            break;
                        case "/storage/readtable.php":
                            result = Storage.ReadTable(postData, contentType, apiPath);
                            break;
                        case "/storage/writetable.php":
                            result = Storage.WriteTable(postData, contentType, apiPath);
                            break;
                        case "/nml/rc2/profile/verify.php":
                            result = Profile.Verify(postData, contentType);
                            break;
                        case "/nml/rc2/profile/reward.php":
                            result = Profile.Reward(postData, contentType);
                            break;
                        case "/nml/rc2/profile/get.php":
                            result = Profile.Get(postData, contentType, apiPath);
                            break;
                        case "/nml/rc2/profile/set.php":
                            result = Profile.Set(postData, contentType, apiPath);
                            break;
                        case "/nml/rc2/stats/getConfig.php":
                            result = Stats.GetConfig(false, postData, contentType, apiPath);
                            break;
                        case "/audi_tech/getprofile.php":
                            result = audi_tech.Profile.GetProfile(postData, contentType, apiPath);
                            break;
                        case "/audi_tech/setprofile.php":
                            result = audi_tech.Profile.SetProfile(postData, contentType, apiPath);
                            break;
                        case "/audi_tech/getFriendsGhostTimes.php":
                            result = audi_tech.Ghost.getFriendsGhostTimes(postData, contentType, apiPath);
                            break;
                        case "/audi_tech/getghost.php":
                            return (audi_tech.Ghost.getGhost(postData, contentType, apiPath), null);
                        case "/WardrobeWars/verify.php":
                            result = Podium.Verify(postData, contentType, apiPath);
                            break;
                        case "/WardrobeWars/podium.php":
                            result = Podium.CheckPodium(postData, contentType, apiPath);
                            break;
                        case "/WardrobeWars/podium_vote.php":
                            result = Podium.RequestVote(postData, contentType, apiPath);
                            break;
                        case "/WardrobeWars/podium_score.php":
                            result = Podium.RequestScore(postData, contentType, apiPath);
                            break;
                        case "/WardrobeWars/reward.php":
                            result = Podium.RequestRewards(postData, contentType, apiPath);
                            break;
                        case "/WardrobeWars/screen.php":
                            result = Podium.RequestScreens(postData, contentType, apiPath);
                            break;
                        //fullsecure endpoint 
                        case "//WardrobeWars/photo.php":
                            result = Podium.PostPhotoPart2(postData, contentType, apiPath, true);
                            break;
                            //part 1
                        case "//WardrobeWars/photo-p1.php":
                            result = Podium.PostPhotoPart1(postData, contentType);
                            break;
                            //part 2
                        case "//WardrobeWars/photo-p2.php":
                            result = Podium.PostPhotoPart2(postData, contentType, apiPath, false);
                            break;
                        default:

                            break;
                    }
                    break;
                case "GET":
                    switch (absolutepath)
                    {
                        case "/audi_tech/hiscores.xml":
                            result = audi_tech.Scoreboard.MockedScoreBoard; // TODO: implement real scoreboard.
                            resultContentType = "text/xml";
                            break;
                        case "/stats/getconfig.php":
                            result = Stats.GetConfig(true, postData, contentType, apiPath);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            if (!string.IsNullOrEmpty(result))
                return (Encoding.UTF8.GetBytes(result), resultContentType);

            return (null, null);
        }
    }
}
