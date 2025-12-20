using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class ClanMember
    {
        public int Id { get; set; }
        public int ClanId { get; set; }
        public int AccountId { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime? ModifiedDt { get; set; }
        public int? ModifiedBy { get; set; }
        public bool? IsActive { get; set; } = true;

        public virtual Account Account { get; set; }
        public virtual Clan Clan { get; set; }
    }
}
