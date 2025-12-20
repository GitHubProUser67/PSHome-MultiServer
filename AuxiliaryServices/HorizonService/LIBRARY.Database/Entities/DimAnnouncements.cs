using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class DimAnnouncements
    {
        public int Id { get; set; }
        public string AnnouncementTitle { get; set; }
        public string AnnouncementBody { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime? ModifiedDt { get; set; }
        public DateTime FromDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime? ToDt { get; set; }
        public int? AppId { get; set; }
    }
}
