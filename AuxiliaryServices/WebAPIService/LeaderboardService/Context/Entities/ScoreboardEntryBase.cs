using System;

namespace WebAPIService.LeaderboardService.Context.Entities
{
    public abstract class ScoreboardEntryBase
    {
        public int Id { get; set; } // Primary Key
        public string PlayerId { get; set; }
        public float Score { get; set; }
        public string ExtraData1 { get; set; }
        public string ExtraData2 { get; set; }
        public string ExtraData3 { get; set; }
        public string ExtraData4 { get; set; }
        public string ExtraData5 { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
