using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class Roles
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public bool? IsActive { get; set; } = true;
    }
}
