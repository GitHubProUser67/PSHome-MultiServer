using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class Game
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        public int AppId { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int PlayerCount { get; set; }
        public string PlayerListCurrent { get; set; }
        public string PlayerListStart { get; set; }
        public int GameLevel { get; set; }
        public int PlayerSkillLevel { get; set; }
        public byte[] GameStats { get; set; }
        public string GameName { get; set; }
        public int RuleSet { get; set; }
        public int? GenericField1 { get; set; }
        public int? GenericField2 { get; set; }
        public int? GenericField3 { get; set; }
        public int? GenericField4 { get; set; }
        public int? GenericField5 { get; set; }
        public int? GenericField6 { get; set; }
        public int? GenericField7 { get; set; }
        public int? GenericField8 { get; set; }
        public string WorldStatus { get; set; }
        public string GameHostType { get; set; }
        public string Metadata { get; set; }
        public DateTime? GameCreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime? GameStartDt { get; set; }
        public string DatabaseUser { get; set; }
    }
}
