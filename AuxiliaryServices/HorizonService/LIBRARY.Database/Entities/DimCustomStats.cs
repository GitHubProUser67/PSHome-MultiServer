using System.Collections.Generic;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class DimCustomStats
    {
        public DimCustomStats()
        {
            AccountCustomStat = new HashSet<AccountCustomStat>();
        }

        public int StatId { get; set; }
        public string StatName { get; set; }
        public int DefaultValue { get; set; }
        public int AppId { get; set; }

        public virtual ICollection<AccountCustomStat> AccountCustomStat { get; set; }
    }
}
