using System;

namespace Horizon.LIBRARY.Database.Models
{
    public class LogDTO
    {
        public DateTime Timestamp { get; set; }
        public int? AccountId { get; set; }
        public string MethodName { get; set; }
        public string LogTitle { get; set; }
        public string LogMsg { get; set; }
        public string LogStacktrace { get; set; }
        public string Payload { get; set; }
    }
}
