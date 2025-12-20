using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class BannedMac
    {
        public int Id { get; set; }
        public string MacAddress { get; set; }
        public DateTime FromDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime? ToDt { get; set; }
    }
}
