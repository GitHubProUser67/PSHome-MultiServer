using Microsoft.EntityFrameworkCore;
using MultiServerLibrary.HTTP;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using WebAPIService.GameServices.PSHOME.RCHOME;
using WebAPIService.LeaderboardService;

namespace WebAPIService.GameServices.PSHOME.NDREAMS.AnsadaPhone
{
    public static class AnsadaScoreHandler
    {
        private const string EncryptionKey = "78TYUK1NF43IJA281";
        private const string EncryptionKey2 = "3C1UHK3AB59VEB916";
        private readonly static char[] EncodingTable = new char[] { 'Z', 'a', 'D', 'i', 'h', 'S', 'u', 'J', 'e', 'g' };

        private static readonly Dictionary<string, AnsadaScoreBoardData> _leaderboards = new Dictionary<string, AnsadaScoreBoardData>();

        public static string ProcessScore(byte[] PostData, string ContentType)
        {
            float score = 0;
            string func = string.Empty;
            string playername = string.Empty;
            string gameid = string.Empty;
            string order = string.Empty;
            string region = string.Empty;
            string time = string.Empty;
            string checksum = string.Empty;
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            if (!string.IsNullOrEmpty(boundary) && PostData != null)
            {
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var data = RawMultipartFormDataParser.Parse(ms, boundary);

                    func = Encoding.UTF8.GetString(data.First(f => f.Name == "func").Data);
                    playername = Encoding.UTF8.GetString(data.First(f => f.Name == "playername").Data);
                    gameid = DecryptData(data.First(f => f.Name == "gameid").Data, false);
                    order = Encoding.UTF8.GetString(data.First(f => f.Name == "order").Data);
                    if (data.Any(f => f.Name == "region"))
                        region = Encoding.UTF8.GetString(data.First(f => f.Name == "region").Data);
                    if (data.Any(f => f.Name == "score"))
                        score = DecodeNumber(DecryptData(data.First(f => f.Name == "score").Data, true));
                    if (data.Any(f => f.Name == "time"))
                        time = Encoding.UTF8.GetString(data.First(f => f.Name == "time").Data);
                    if (data.Any(f => f.Name == "checksum"))
                        checksum = Encoding.UTF8.GetString(data.First(f => f.Name == "checksum").Data);
                }

                switch (func)
                {
                    case "set":
                        string calcChecksum = CheckSum(playername, gameid, ((int)score).ToString());

                        if (checksum != calcChecksum)
                        {
                            CustomLogger.LoggerAccessor.LogWarn($"[AnsadaPhone] - ProcessScore: Checksum:{checksum} doesn't match expected result! (Expected:{calcChecksum})");
                            return null;
                        }

                        if (!string.IsNullOrEmpty(gameid))
                        {
                            lock (_leaderboards)
                            {
                                if (!_leaderboards.ContainsKey(gameid))
                                    _leaderboards.Add(gameid, new AnsadaScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options, gameid));

                                _ = _leaderboards[gameid].UpdateScoreAsync(playername, score, new List<object> { time });
                            }
                        }

                        return "<xml></xml>";
                    case "gethigh":

                        if (!string.IsNullOrEmpty(gameid))
                        {
                            lock (_leaderboards)
                            {
                                if (!_leaderboards.ContainsKey(gameid))
                                    _leaderboards.Add(gameid, new AnsadaScoreBoardData(LeaderboardDbContext.OnContextBuilding(new DbContextOptionsBuilder<LeaderboardDbContext>(), 0, $"Data Source={LeaderboardDbContext.GetDefaultDbPath()}").Options, gameid));

                                return _leaderboards[gameid].SerializeToString("highscores").Result;
                            }
                        }

                        break;
                }
            }

            return null;
        }

        private static string DecryptData(byte[] value, bool decodeNumber)
        {
            int keyIndex = 0;

            string key = decodeNumber ? EncryptionKey2 : EncryptionKey;

            StringBuilder output = new StringBuilder(value.Length);

            for (int i = 0; i < value.Length; i++)
            {
                byte keyByte = (byte)key[keyIndex];

                output.Append((char)(byte)BinXor(value[i], keyByte));

                keyIndex++;
                if (keyIndex >= key.Length)
                    keyIndex = 0;
            }

            return output.ToString();
        }

        private static float DecodeNumber(string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
                throw new Exception("[AnsadaPhone] - ProcessScore: Invalid encoded number.");

            // First char = initial shift index
            int shift = Array.IndexOf(EncodingTable, encoded[0]);
            if (shift < 0)
                throw new Exception("[AnsadaPhone] - ProcessScore: Invalid encoding table index.");

            var result = new StringBuilder();

            // For each encoded digit
            for (int i = 1; i < encoded.Length; i++)
            {
                int charIndex = Array.IndexOf(EncodingTable, encoded[i]);
                if (charIndex < 0)
                    throw new Exception("[AnsadaPhone] - ProcessScore: Invalid encoded digit.");

                int digit = charIndex - shift;
                if (digit < 0)
                    digit += 10;

                result.Append(digit);

                shift++;
                if (shift >= 10)
                    shift = 0;
            }

            return float.Parse(result.ToString(), CultureInfo.InvariantCulture);
        }

        private static int BinXor(int x, int y)
        {
            int z = 0;

            for (int i = 0; i < 32; i++)
            {
                int xb = x % 2;   // bit of x
                int yb = y % 2;   // bit of y

                if (xb == 0)
                {
                    if (yb == 1)
                    {
                        y -= 1;             // clear that bit
                        z += (1 << i);      // set bit i
                    }
                }
                else
                {
                    x -= 1;                 // clear that bit
                    if (yb == 0)
                        z += (1 << i);      // set bit i
                    else
                        y -= 1;
                }

                y /= 2;
                x /= 2;
            }

            return z;
        }

        private static string CheckSum(string player, string game, string score)
        {
            int playerVal = 0, gameVal = 0, scoreVal = 0;

            for (int i = 0; i < player.Length; i++)
                playerVal += (int)player[i] + (i + 1);

            for (int i = 0; i < game.Length; i++)
                gameVal += (int)game[i];

            for (int i = 0; i < score.Length; i++)
                scoreVal += (int)score[i];

            int value = (gameVal * 2) - (playerVal + scoreVal);

            if (value < 0)
                value = -((-value) % 10000);
            else
                value %= 10000;

            return value.ToString();
        }
    }
}
