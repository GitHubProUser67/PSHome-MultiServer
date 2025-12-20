using System.Collections.Generic;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class DimClanStats
    {
        public DimClanStats()
        {
            ClanStat = new HashSet<ClanStat>();
        }

        public int StatId { get; set; }
        public string StatName { get; set; }
        public int DefaultValue { get; set; }

        public virtual ICollection<ClanStat> ClanStat { get; set; }
    }
}
