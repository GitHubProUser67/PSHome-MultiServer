using CustomLogger;
using System.IO;

namespace WebAPIService.GameServices.PSHOME.CDM
{
    public class Leaderboards
    {
        public static string handleLeaderboards(byte[] PostData, string ContentType, string workpath, string absolutePath)
        {
            string pubListPath = $"{workpath}/CDM/Leaderboards/";

            Directory.CreateDirectory(pubListPath);
            string filePath = $"{pubListPath}/TestLeaderboard.xml";
            if (File.Exists(filePath))
            {
                LoggerAccessor.LogInfo($"[CDM] - Leaderboard found and sent! (TEMP IMPLEMENTATION)!");
                string res = File.ReadAllText(filePath);

                return $"{res}";
            }
            else
                LoggerAccessor.LogWarn($"[CDM] - Failed to find Leaderboard with expected path {filePath}! (TEMP IMPLEMENTATION)");

            return "<xml><Leaderboard NAME=\"Player 1\" COINS=\"99999\" /></xml>";
        }
    }
}
