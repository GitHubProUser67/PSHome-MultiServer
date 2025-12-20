using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alcatraz.Context.Entities
{
	public class PlayerStatisticsBoard
	{
		public uint Id { get; set; }
		public uint PlayerId { get; set; }
		public int BoardId { get; set; }
		public int Rank { get; set; }
		public float Score { get; set; }
		public DateTime LastUpdate { get; set; }

		public virtual ICollection<PlayerStatisticsBoardValue> Values { get; set; }
	}

	public class PlayerStatisticsBoardValue
	{
		public int Id { get; set; }

		public PlayerStatisticsBoard PlayerBoard { get; set; }
		public uint PlayerBoardId { get; set; }

		public int PropertyId { get; set; }

		public string ValueJSON { get; set; }
		public int RankingCriterionIndex { get; set; }
		public string SliceScoreJSON { get; set; }
		public string ScoreLostForNextSliceJSON { get; set; }
	}
}
