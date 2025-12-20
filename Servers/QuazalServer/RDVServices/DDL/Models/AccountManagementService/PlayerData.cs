namespace QuazalServer.RDVServices.DDL.Models
{
    public class PlayerData
    {
        public float RidingSkill { get; set; }
        public float ObedienceSkill { get; set; }
        public float JumpingSkill { get; set; }
        public bool AvatarSettingsInitialized { get; set; }
        public string? AvatarApperance { get; set; }
        public bool HorseSettingsInitialized { get; set; }
        public string? HorseApperance { get; set; }
        public string? HorseName { get; set; }
        public uint StableID { get; set; }
        public uint StablePosition { get; set; }
        public string? TagLine { get; set; }
        public string? Bio { get; set; }
        public uint Locale { get; set; }
        public double GlobalRankScore { get; set; }
        public uint CachedGlobalRank { get; set; }
        public uint CachedPreviousGlobalRank { get; set; }
        public string? Accomplishments { get; set; }
        public uint Wins { get; set; }
        public uint Losses { get; set; }
        public string? Status { get; set; }
    }
}
