using System;
using System.IO;
using System.Security;
using System.Text.RegularExpressions;

namespace WebAPIService.GameServices.PSHOME.OHS
{
    public class OHSClass
    {
        private static string[] Commands = { "/batch/", "/community/getscore/", "/community/updatescore/",
            "/global/set/", "/global/getall/", "/global/get/",
            "/userid/", "/user/getwritekey/", "/user/set/",
            "/user/getall/", "/user/get/", "/user/gets/",  "/user/getmany/", "/usercounter/set/",
            "/usercounter/getall/", "usercounter/getmany/", "/usercounter/get/", "/usercounter/increment/",
            "/userinventory/addglobalitems/", "/userinventory/getglobalitems/",
            "/userinventory/getuserinventory/", "/leaderboard/requestbyusers/", "/leaderboard/requestbyrank/",
            "/leaderboard/update/", "/leaderboard/updatessameentry/", 
            "/statistic/set/", "/heatmap/tracker/", "/points/tracker/"};
        private string absolutepath;
        private string method;
        private int game;

        public OHSClass(string method, string absolutepath, int game)
        {
            this.absolutepath = absolutepath;
            this.method = method;
            this.game = game;
        }

        public string ProcessRequest(byte[] PostData, string ContentType, string directoryPath)
        {
            if (string.IsNullOrEmpty(absolutepath) || string.IsNullOrEmpty(directoryPath))
                return null;

            string res = null;

            directoryPath = RemoveCommands(directoryPath);

            switch (method)
            {
                case "POST":
                    if (ContentType != null && ContentType.Contains("multipart/form-data"))
                    {
                        if (absolutepath.Contains(Commands[0]))
                            res = Batch.Batch_Process(PostData, ContentType, directoryPath, game);
                        else if (absolutepath.Contains(Commands[1]))
                            res = Community.Community_Getscore(PostData, ContentType, directoryPath, string.Empty, game);
                        else if (absolutepath.Contains(Commands[2]))
                            res = Community.Community_UpdateScore(PostData, ContentType, directoryPath, string.Empty, game);
                        else if (absolutepath.Contains(Commands[3]))
                            res = User.Set(PostData, ContentType, directoryPath, string.Empty, true, game);
                        else if (absolutepath.Contains(Commands[4]))
                            res = User.Get_All(PostData, ContentType, directoryPath, string.Empty, true, game);
                        else if (absolutepath.Contains(Commands[5]))
                            res = User.Get(PostData, ContentType, directoryPath, string.Empty, true, game);
                        else if (absolutepath.Contains(Commands[6]))
                            res = User.User_Id(PostData, ContentType, string.Empty, game);
                        else if (absolutepath.Contains(Commands[7]))
                            res = User.User_GetWritekey(PostData, ContentType, string.Empty, game);
                        else if (absolutepath.Contains(Commands[8]))
                            res = User.Set(PostData, ContentType, directoryPath, string.Empty, false, game);
                        else if (absolutepath.Contains(Commands[9]))
                            res = User.Get_All(PostData, ContentType, directoryPath, string.Empty, false, game);
                        else if (absolutepath.Contains(Commands[10]))
                            res = User.Get(PostData, ContentType, directoryPath, string.Empty, false, game);
                        else if (absolutepath.Contains(Commands[11]))
                            res = User.Gets(PostData, ContentType, directoryPath, string.Empty, false, game);
                        else if (absolutepath.Contains(Commands[12]))
                            res = User.GetMany(PostData, ContentType, directoryPath, string.Empty, false, game);
                        else if (absolutepath.Contains(Commands[13]))
                            res = UserCounter.Set(PostData, ContentType, directoryPath, string.Empty, game);
                        else if (absolutepath.Contains(Commands[14]))
                            res = UserCounter.Get_All(PostData, ContentType, directoryPath, string.Empty, game);
                        else if (absolutepath.Contains(Commands[15]))
                            res = UserCounter.Get_Many(PostData, ContentType, directoryPath, string.Empty, game);
                        else if (absolutepath.Contains(Commands[16]))
                            res = UserCounter.Get(PostData, ContentType, directoryPath, string.Empty, game);
                        else if (absolutepath.Contains(Commands[17]))
                            res = UserCounter.Increment(PostData, ContentType, directoryPath, string.Empty, game, false);
                        else if (absolutepath.Contains(Commands[18]))
                            res = UserInventory.AddGlobalItems(PostData, ContentType, directoryPath, string.Empty, game);
                        else if (absolutepath.Contains(Commands[19]))
                            res = UserInventory.GetGlobalItems(PostData, ContentType, directoryPath, string.Empty, game);
                        else if (absolutepath.Contains(Commands[20]))
                            res = UserInventory.GetUserInventory(PostData, ContentType, directoryPath, string.Empty, game);
                        else if (absolutepath.Contains(Commands[21]))
                            res = Leaderboard.Leaderboard_RequestByUsers(directoryPath, PostData, ContentType, Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), string.Empty, game);
                        else if (absolutepath.Contains(Commands[22]))
                            res = Leaderboard.Leaderboard_RequestByRank(directoryPath, PostData, ContentType, Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), string.Empty, game);
                        else if (absolutepath.Contains(Commands[23]))
                            res = Leaderboard.Leaderboard_Update(directoryPath, PostData, ContentType, Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), string.Empty, game, false);
                        else if (absolutepath.Contains(Commands[24]))
                            res = Leaderboard.Leaderboard_UpdatesSameEntry(directoryPath, PostData, ContentType, Path.GetFileName(directoryPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), string.Empty, game, false);
                        else if (absolutepath.Contains(Commands[25]))
                            res = Statistic.Set(PostData, ContentType);
                        else if (absolutepath.Contains(Commands[26]))
                            res = Statistic.HeatmapTracker(PostData, ContentType);
                        else if (absolutepath.Contains(Commands[27]))
                            res = Statistic.PointsTracker(PostData, ContentType);
                    }
                    break;
                default:
                    break;
            }

            return SecurityElement.Escape(res);
        }

        private static string RemoveCommands(string input)
        {
            string modifiedInput = input;

            foreach (string pattern in Commands)
            {
                modifiedInput = Regex.Replace(modifiedInput, Regex.Escape(pattern), string.Empty);
            }

            return modifiedInput;
        }
    }
}
