using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class ClanMessage
    {
        public int Id { get; set; }
        public int ClanId { get; set; }
        public string Message { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public int CreatedBy { get; set; }
        public bool? IsActive { get; set; } = true;

        public virtual Clan Clan { get; set; }
    }
}
