using System;
using System.Collections.Generic;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class Clan
    {
        public Clan()
        {
            ClanInvitation = new HashSet<ClanInvitation>();
            ClanMember = new HashSet<ClanMember>();
            ClanMessage = new HashSet<ClanMessage>();
            ClanStat = new HashSet<ClanStat>();
            ClanCustomStat = new HashSet<ClanCustomStat>();
        }

        public int ClanId { get; set; }
        public string ClanName { get; set; }
        public int ClanLeaderAccountId { get; set; }
        public int AppId { get; set; }
        public bool? IsActive { get; set; } = true;
        public string MediusStats { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public int CreatedBy { get; set; }
        public DateTime? ModifiedDt { get; set; }
        public int? ModifiedBy { get; set; }

        public virtual Account ClanLeaderAccount { get; set; }
        public virtual ICollection<ClanInvitation> ClanInvitation { get; set; }
        public virtual ICollection<ClanMember> ClanMember { get; set; }
        public virtual ICollection<ClanMessage> ClanMessage { get; set; }
        public virtual ICollection<ClanStat> ClanStat { get; set; }
        public virtual ICollection<ClanCustomStat> ClanCustomStat { get; set; }
    }
}
