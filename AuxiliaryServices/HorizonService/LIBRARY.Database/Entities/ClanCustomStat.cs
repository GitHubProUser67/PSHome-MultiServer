using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class ClanCustomStat
    {
        public int Id { get; set; }
        public int ClanId { get; set; }
        public int StatId { get; set; }
        public int StatValue { get; set; }
        public DateTime? ModifiedDt { get; set; }

        public virtual Clan Clan { get; set; }
        public virtual DimClanCustomStats Stat { get; set; }
    }
}
