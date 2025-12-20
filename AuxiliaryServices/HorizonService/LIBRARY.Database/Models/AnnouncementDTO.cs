using System;

namespace Horizon.LIBRARY.Database.Models
{
    public class AddAnnouncementDTO
    {
        public string AnnouncementTitle { get; set; }
        public string AnnouncementBody { get; set; }
        public DateTime? FromDt { get; set; }
        public DateTime? ToDt { get; set; }
        public int AppId { get; set; }
    }

    public class ChangeAnnouncementDTO
    {
        public int Id { get; set; }
        public string AnnouncementTitle { get; set; }
        public string AnnouncementBody { get; set; }
        public DateTime? FromDt { get; set; }
        public DateTime? ToDt { get; set; }
        public int AppId { get; set; }
    }
}
