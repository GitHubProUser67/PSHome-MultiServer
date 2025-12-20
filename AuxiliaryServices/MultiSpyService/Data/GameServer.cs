using System;

namespace MultiSpyService.Data
{
	public class NonFilterAttribute : Attribute
	{

	}

	public class GameServer
	{
		[NonFilter]
		public bool Valid { get; set; }

		[NonFilter]
		public string IPAddress { get; set; }

		[NonFilter]
		public int QueryPort { get; set; }

		[NonFilter]
		public DateTime LastRefreshed { get; set; }

		[NonFilter]
		public DateTime LastPing { get; set; }


		[NonFilter]
		public string localip0 { get; set; }

		[NonFilter]
		public string localip1 { get; set; }

		[NonFilter]
		public int localport { get; set; }

		[NonFilter]
		public bool natneg { get; set; }

		[NonFilter]
		public int statechanged { get; set; }

        public string groupid { get; set; }
        public string country { get; set; }
		public string hostname { get; set; }
		public string gamename { get; set; }
		public string gamever { get; set; }
		public string mapname { get; set; }
		public string gametype { get; set; }
		public string gamevariant { get; set; }
		public int numplayers { get; set; }
		public int maxplayers { get; set; }
		public string gamemode { get; set; }
		public bool password { get; set; }
		public int timelimit { get; set; }
		public int roundtime { get; set; }
		public int hostport { get; set; }
		public bool bf2_dedicated { get; set; }
		public bool bf2_ranked { get; set; }
		public bool bf2_anticheat { get; set; }
		public string bf2_os { get; set; }
		public bool bf2_autorec { get; set; }
		public string bf2_d_idx { get; set; }
		public string bf2_d_dl { get; set; }
		public bool bf2_voip { get; set; }
		public bool bf2_autobalanced { get; set; }
		public bool bf2_friendlyfire { get; set; }
		public string bf2_tkmode { get; set; }
		public double bf2_startdelay { get; set; }
		public double bf2_spawntime { get; set; }
		public string bf2_sponsortext { get; set; }
		public string bf2_sponsorlogo_url { get; set; }
		public string bf2_communitylogo_url { get; set; }
		public int bf2_scorelimit { get; set; }
		public double bf2_ticketratio { get; set; }
		public double bf2_teamratio { get; set; }
		public string bf2_team1 { get; set; }
		public string bf2_team2 { get; set; }
		public bool bf2_bots { get; set; }
		public bool bf2_pure { get; set; }
		public int bf2_mapsize { get; set; }
		public bool bf2_globalunlocks { get; set; }
		public double bf2_fps { get; set; }
		public bool bf2_plasma { get; set; }
		public int bf2_reservedslots { get; set; }
		public double bf2_coopbotratio { get; set; }
		public int bf2_coopbotcount { get; set; }
		public int bf2_coopbotdiff { get; set; }
		public bool bf2_novehicles { get; set; }
	}
}
