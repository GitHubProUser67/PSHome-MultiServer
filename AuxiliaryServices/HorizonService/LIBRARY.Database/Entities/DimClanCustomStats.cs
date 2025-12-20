using System.Collections.Generic;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class DimClanCustomStats
    {
        public DimClanCustomStats()
        {
            ClanCustomStat = new HashSet<ClanCustomStat>();
        }

        public int StatId { get; set; }
        public string StatName { get; set; }
        public int DefaultValue { get; set; }
        public int AppId { get; set; }

        public virtual ICollection<ClanCustomStat> ClanCustomStat { get; set; }
    }
}
