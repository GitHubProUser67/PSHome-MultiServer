using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace WebAPIService.GameServices.PSHOME.OHS
{
    public class OHSClass
    {
        private static readonly Dictionary<string, Func<byte[], string, string, int, string>> _handlers = new Dictionary<string, Func<byte[], string, string, int, string>>
        {
            { "/batch/", (post, ct, dir, game) => Batch.Batch_Process(post, ct, dir, game) },
            { "/community/getscore/", (post, ct, dir, game) => Community.Community_Getscore(post, ct, dir, string.Empty, game) },
            { "/community/updatescore/", (post, ct, dir, game) => Community.Community_UpdateScore(post, ct, dir, string.Empty, game) },
            { "/global/set/", (post, ct, dir, game) => User.Set(post, ct, dir, string.Empty, true, game) },
            { "/global/getall/", (post, ct, dir, game) => User.Get_All(post, ct, dir, string.Empty, true, game) },
            { "/global/get/", (post, ct, dir, game) => User.Get(post, ct, dir, string.Empty, true, game) },
            { "/userid/", (post, ct, dir, game) => User.User_Id(post, ct, string.Empty, game) },
            { "/user/getwritekey/", (post, ct, dir, game) => User.User_GetWritekey(post, ct, string.Empty, game) },
            { "/user/set/", (post, ct, dir, game) => User.Set(post, ct, dir, string.Empty, false, game) },
            { "/user/getall/", (post, ct, dir, game) => User.Get_All(post, ct, dir, string.Empty, false, game) },
            { "/user/get/", (post, ct, dir, game) => User.Get(post, ct, dir, string.Empty, false, game) },
            { "/user/gets/", (post, ct, dir, game) => User.Gets(post, ct, dir, string.Empty, false, game) },
            { "/user/getmany/", (post, ct, dir, game) => User.GetMany(post, ct, dir, string.Empty, false, game) },
            { "/usercounter/set/", (post, ct, dir, game) => UserCounter.Set(post, ct, dir, string.Empty, game) },
            { "/usercounter/getall/", (post, ct, dir, game) => UserCounter.Get_All(post, ct, dir, string.Empty, game) },
            { "usercounter/getmany/", (post, ct, dir, game) => UserCounter.Get_Many(post, ct, dir, string.Empty, game) },
            { "/usercounter/get/", (post, ct, dir, game) => UserCounter.Get(post, ct, dir, string.Empty, game) },
            { "/usercounter/increment/", (post, ct, dir, game) => UserCounter.Increment(post, ct, dir, string.Empty, game, false) },
            { "/userinventory/addglobalitems/", (post, ct, dir, game) => UserInventory.AddGlobalItems(post, ct, dir, string.Empty, game) },
            { "/userinventory/getglobalitems/", (post, ct, dir, game) => UserInventory.GetGlobalItems(post, ct, dir, string.Empty, game) },
            { "/userinventory/getuserinventory/", (post, ct, dir, game) => UserInventory.GetUserInventory(post, ct, dir, string.Empty, game) },
            { "/leaderboard/requestbyusers/", (post, ct, dir, game) => Leaderboard.Leaderboard_RequestByUsers(dir, post, ct, Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), string.Empty, game) },
            { "/leaderboard/requestbyrank/", (post, ct, dir, game) => Leaderboard.Leaderboard_RequestByRank(dir, post, ct, Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), string.Empty, game) },
            { "/leaderboard/update/", (post, ct, dir, game) => Leaderboard.Leaderboard_Update(dir, post, ct, Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), string.Empty, game, false) },
            { "/leaderboard/updatessameentry/", (post, ct, dir, game) => Leaderboard.Leaderboard_UpdatesSameEntry(dir, post, ct, Path.GetFileName(dir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)), string.Empty, game, false) },
            { "/statistic/set/", (post, ct, dir, game) => Statistic.Set(post, ct) },
            { "/heatmap/tracker/", (post, ct, dir, game) => Statistic.HeatmapTracker(post, ct) },
            { "/points/tracker/", (post, ct, dir, game) => Statistic.PointsTracker(post, ct) },
        };

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
            if (string.IsNullOrEmpty(absolutepath) || method != "POST" || string.IsNullOrEmpty(directoryPath) || ContentType == null || !ContentType.Contains("multipart/form-data"))
                return null;

            directoryPath = RemoveCommands(directoryPath);

            foreach (var route in _handlers)
            {
                if (absolutepath.Contains(route.Key))
                    return route.Value(PostData, ContentType, directoryPath, game);
            }

            return null;
        }

        private static string RemoveCommands(string input)
        {
            string modifiedInput = input;

            foreach (string pattern in _handlers.Keys)
                modifiedInput = Regex.Replace(modifiedInput, Regex.Escape(pattern), string.Empty);

            return modifiedInput;
        }
    }
}
