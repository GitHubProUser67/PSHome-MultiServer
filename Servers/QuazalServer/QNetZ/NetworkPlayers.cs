using NetHasher.CRC;
using CustomLogger;
using System.Text;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.QNetZ
{
    public static class NetworkPlayers
	{
		public static uint RVCIDCounter = 0xBB98E;

        private static readonly object _Lock = new();

        public static readonly List<PlayerInfo> Players = new();

        public static PlayerInfo? GetPlayerInfoByPID(uint pid)
		{
            lock (_Lock)
                return Players.Where(pl => pl.PID == pid).FirstOrDefault();
        }

		public static PlayerInfo? GetPlayerInfoByUsername(string userName)
		{
            lock (_Lock)
                return Players.Where(pl => pl.Name == userName).FirstOrDefault();
        }

		public static PlayerInfo CreatePlayerInfo(RMCContext ctx)
		{
            PlayerInfo plInfo = new()
            {
                Client = ctx.Client,
                PID = 0,
                RVCID = RVCIDCounter++,
                AccessKey = ctx.Handler.AccessKey
            };

            lock (_Lock)
                Players.Add(plInfo);

            return plInfo;
        }

        public static void PurgeAllPlayers()
		{
            lock (_Lock)
                Players.Clear();
		}

		public static void DropPlayerInfo(PlayerInfo playerInfo)
		{
            lock (_Lock)
            {
                LoggerAccessor.LogWarn($"[Quazal NetworkPlayers] - dropping player: {playerInfo.Name}");

                playerInfo.OnDropped();
                Players.Remove(playerInfo);
            }
        }

		public static void DropPlayers()
		{
			lock (_Lock)
			{
                Players.RemoveAll(playerInfo => {
                    if (playerInfo.Client != null)
                    {
                        if (playerInfo.Client.State != QClient.StateType.Dropped)
                            return false;
                        if (playerInfo.Client.TimeSinceLastPacket < Constants.ClientTimeoutSeconds)
                            return false;
                    }

                    LoggerAccessor.LogWarn($"[Quazal NetworkPlayers] - auto-dropping player: {playerInfo.Name}");

                    playerInfo.OnDropped();
                    return true;
                });
            }
		}

        public static uint GenerateUniqueUint(string input)
        {
            uint result = 0;

            do
            {
                result = CRC32.Create(Encoding.UTF8.GetBytes(input + "QnetZM$3")) ^ result;

                // If below or equal 1000, modify the input slightly and recalculate.
                if (result <= 1000)
                    input += "_retry";
            }
            while (result <= 1000);

            return result;
        }
    }
}
