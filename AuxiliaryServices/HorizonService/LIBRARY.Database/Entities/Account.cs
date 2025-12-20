using System;
using System.Collections.Generic;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class Account
    {
        public Account()
        {
            AccountFriend = new HashSet<AccountFriend>();
            AccountIgnored = new HashSet<AccountIgnored>();
            AccountStat = new HashSet<AccountStat>();
            AccountCustomStat = new HashSet<AccountCustomStat>();
            Clan = new HashSet<Clan>();
            ClanInvitation = new HashSet<ClanInvitation>();
            ClanMember = new HashSet<ClanMember>();
       }

        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string AccountPassword { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime? ModifiedDt { get; set; }
        public DateTime? LastSignInDt { get; set; }
        public string MachineId { get; set; }
        public bool? IsActive { get; set; } = true;
        public int? AppId { get; set; }
        public string MediusStats { get; set; }
        public string LastSignInIp { get; set; }
        public string Metadata { get; set; }
        public bool ResetPasswordOnNextLogin { get; set; }

        public virtual ICollection<AccountFriend> AccountFriend { get; set; }
        public virtual ICollection<AccountIgnored> AccountIgnored { get; set; }
        public virtual ICollection<AccountStat> AccountStat { get; set; }
        public virtual ICollection<AccountCustomStat> AccountCustomStat { get; set; }
        public virtual ICollection<Clan> Clan { get; set; }
        public virtual ICollection<ClanInvitation> ClanInvitation { get; set; }
        public virtual ICollection<ClanMember> ClanMember { get; set; }
    }
}
