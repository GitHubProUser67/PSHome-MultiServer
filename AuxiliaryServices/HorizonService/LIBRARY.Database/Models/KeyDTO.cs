using System;

namespace Horizon.LIBRARY.Database.Models
{
    public class ServerFlagsDTO
    {
        public MaintenanceDTO MaintenanceMode { get; set; }
    }

    public class MaintenanceDTO
    {
        public bool IsActive { get; set; }
        public DateTime? FromDt { get; set; }
        public DateTime? ToDt { get; set; }
    }
}