using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class ServerLog
    {
        public int Id { get; set; }
        public DateTime LogDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public int? AccountId { get; set; }
        public string MethodName { get; set; }
        public string LogTitle { get; set; }
        public string LogMsg { get; set; }
        public string LogStacktrace { get; set; }
        public string Payload { get; set; }
    }
}
