using System;

namespace Horizon.LIBRARY.Database.Models
{
    public partial class UniverseDTO
    {
        public int AppId { get; set; }
        public uint UniverseID { get; set; }
        public string UniverseName { get; set; }
        public string UniverseDescription { get; set; }
        public string DNS { get; set; }
        public int Port { get; set; }
        public int Status { get; set; }
        public int UserCount { get; set; }
        public int MaxUsers { get; set; }
        public string UniverseBilling { get; set; }
        public string BillingSystemName { get; set; }
        public string ExtendedInfo { get; set; }
        public string SvoURL { get; set; }

        public DateTime CreateDt { get; set; }
        public DateTime ModifiedDt { get; set; }
    }
}