using System;

namespace Horizon.LIBRARY.Database.Models
{
    
    public class MatchmakingSupersetDTO
    {
        public int Id { get; set; }
        public int AppId { get; set; }
        public int SupersetID { get; set; }
        public string SupersetName { get; set; }
        public string SupersetDescription { get; set; }
        public string SupersetExtraInfo { get; set; }
        public DateTime CreateDt { get; set; }
        public DateTime ModifiedDt { get; set; }
    }
}