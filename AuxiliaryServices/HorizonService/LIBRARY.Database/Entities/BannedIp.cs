using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class BannedIp
    {
        public int Id { get; set; }
        public string IpAddress { get; set; }
        public DateTime FromDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime? ToDt { get; set; }
    }
}
