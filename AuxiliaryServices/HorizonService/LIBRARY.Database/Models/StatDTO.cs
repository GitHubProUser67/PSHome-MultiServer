namespace Horizon.LIBRARY.Database.Models
{
    public class LeaderboardDTO
    {
        /// <summary>
        /// Ladder ranking of stat (0-index)
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Unique account id of player.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Account name.
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// Value of stat.
        /// </summary>
        public int StatValue { get; set; }

        /// <summary>
        /// Application specific stats encoded as Base64 string.
        /// </summary>
        public string MediusStats { get; set; }

        /// <summary>
        /// Total number of players ranked for the given column.
        /// </summary>
        public int TotalRankedAccounts { get; set; }
    }

    public class ClanLeaderboardDTO
    {
        /// <summary>
        /// Ladder ranking of stat (0-index)
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Unique id of clan.
        /// </summary>
        public int ClanId { get; set; }

        /// <summary>
        /// Clan name.
        /// </summary>
        public string ClanName { get; set; }

        /// <summary>
        /// Value of stat.
        /// </summary>
        public int StatValue { get; set; }

        /// <summary>
        /// Application specific stats encoded as Base64 string.
        /// </summary>
        public string MediusStats { get; set; }

        /// <summary>
        /// Total number of clans ranked for the given column.
        /// </summary>
        public int TotalRankedClans { get; set; }
    }

    public class StatPostDTO
    {
        /// <summary>
        /// Unique account id of player.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Collection of ladder stats to be saved to the database.
        /// </summary>
        public int[] Stats { get; set; }
    }
    public class ClanStatPostDTO
    {
        /// <summary>
        /// Unique clan id of clan.
        /// </summary>
        public int ClanId { get; set; }

        /// <summary>
        /// Collection of ladder stats to be saved to the database.
        /// </summary>
        public int[] Stats { get; set; }
    }
}