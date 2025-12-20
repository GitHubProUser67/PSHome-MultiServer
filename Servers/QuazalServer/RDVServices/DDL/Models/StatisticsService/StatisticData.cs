namespace QuazalServer.RDVServices.DDL.Models
{
	public enum VariantType
	{
		None = 0,
		int32 = 1,
		int64 = 2,
		Float = 3,
		Double = 4,
		String = 5,
	}
	public enum StatisticPolicy
	{
		Add = 0,
		Sub = 1,
		Overwrite = 2,
		ReplaceIfMin = 3,
		ReplaceIfMax = 4,
	};

	public enum RankingOrder
	{
		NotRanked = -1,

		Ascending = 0,
		Descending = 1,
	}

	public class StatisticData
	{
		public StatisticData()
		{
		}

		public int boardId { get; set; }
		public IEnumerable<int>? propertyIds { get; set; }
	}

	public class StatisticValueVariant
	{
		public StatisticValueVariant()
		{
			typeValue = (byte)VariantType.None;
			valueString = "";
			valueInt32 = 0;
			valueInt64 = 0;
			valueDouble = 0.0;
		}

		public int valueInt32 { get; set; }
		public long valueInt64 { get; set; }
		public double valueDouble { get; set; }
		public string valueString { get; set; }
		public byte typeValue { get; set; }         // VariantType

		public bool Compare(StatisticValueVariant other)
        {
			switch ((VariantType)typeValue)
			{
				case VariantType.int32:
					return (valueInt32 == other.valueInt32);
				case VariantType.int64:
					return (valueInt64 == other.valueInt64);
				case VariantType.Float:
				case VariantType.Double:
					return (valueDouble == other.valueDouble);
				case VariantType.String:
					return (valueString == other.valueString);
			}
			return false;
		}

		public bool Assign(StatisticValueVariant newValue)
		{
			if (Compare(newValue))
				return false;

			switch((VariantType)typeValue)
			{
				case VariantType.int32:
					valueInt32 = newValue.valueInt32;
					return true;
				case VariantType.int64:
					valueInt64 = newValue.valueInt64;
					return true;
				case VariantType.Float:
				case VariantType.Double:
					valueDouble = newValue.valueDouble;
					return true;
				case VariantType.String:
					valueString = newValue.valueString;
					return true;
			}
			return false;
		}

		public bool Add(StatisticValueVariant newValue)
		{
			switch ((VariantType)typeValue)
			{
				case VariantType.int32:
					valueInt32 += newValue.valueInt32;
					break;
				case VariantType.int64:
					valueInt64 += newValue.valueInt64;
					break;
				case VariantType.Float:
				case VariantType.Double:
					valueDouble += newValue.valueDouble;
					break;
				case VariantType.String:
					// not supported
				default:
					return false;
			}
			return true;
		}

		public bool Subtract(StatisticValueVariant newValue)
		{
			switch ((VariantType)typeValue)
			{
				case VariantType.int32:
					valueInt32 -= newValue.valueInt32;
					break;
				case VariantType.int64:
					valueInt64 -= newValue.valueInt64;
					break;
				case VariantType.Float:
				case VariantType.Double:
					valueDouble -= newValue.valueDouble;
					break;
				case VariantType.String:
					// not supported
				default:
					return false;
			}
			return true;
		}

		public bool ReplaceIfMin(StatisticValueVariant newValue)
		{
			// FIXME: might be incorrect and flipped!
			switch ((VariantType)typeValue)
			{
				case VariantType.int32:
					if(newValue.valueInt32 < valueInt32)
                    {
						valueInt32 = newValue.valueInt32;
						return true;
					}
					break;
				case VariantType.int64:
					if (newValue.valueInt64 < valueInt64)
                    {
						valueInt64 = newValue.valueInt64;
						return true;
					}
					break;
				case VariantType.Float:
				case VariantType.Double:
					if (newValue.valueDouble < valueDouble)
                    {
						valueDouble = newValue.valueDouble;
						return true;
					}
					break;
				case VariantType.String:
					// not supported
					break;
			}
			return false;
		}

		public bool ReplaceIfMax(StatisticValueVariant newValue)
		{
			// FIXME: might be incorrect and flipped!
			switch ((VariantType)typeValue)
			{
				case VariantType.int32:
					if (newValue.valueInt32 > valueInt32)
                    {
						valueInt32 = newValue.valueInt32;
						return true;
					}
					break;
				case VariantType.int64:
					if (newValue.valueInt64 > valueInt64)
                    {
						valueInt64 = newValue.valueInt64;
						return true;
					}
					break;
				case VariantType.Float:
				case VariantType.Double:
					if (newValue.valueDouble > valueDouble)
                    {
						valueDouble = newValue.valueDouble;
						return true;
					}
					break;
				case VariantType.String:
					// not supported
					break;
			}
			return false;
		}

		public double GetAsFloat()
		{
			switch ((VariantType)typeValue)
			{
				case VariantType.int32:
					return valueInt32;
				case VariantType.int64:
					return valueInt64;
				case VariantType.Float:
				case VariantType.Double:
					return valueDouble;
				case VariantType.String:
					return 0.0;
			}
			return 0.0;
		}

		public override string? ToString()
        {
			switch ((VariantType)typeValue)
			{
				case VariantType.int32:
					return valueInt32.ToString();
				case VariantType.int64:
					return valueInt64.ToString();
				case VariantType.Float:
				case VariantType.Double:
					return valueDouble.ToString();
				case VariantType.String:
					return valueString.ToString();
			}
			return "<invalid variant type>";
		}
	}

	public class StatisticWriteValue
	{
		public StatisticWriteValue()
		{
		}

		public byte propertyId { get; set; }
		public StatisticValueVariant? value { get; set; }
		public byte writePolicy { get; set; } // StatisticPolicy
		public bool friendComparison { get; set; }
	}

	public class StatisticWriteWithBoard
	{
		public StatisticWriteWithBoard()
		{

		}

		public int boardId { get; set; }
		public IEnumerable<StatisticWriteValue>? statisticList { get; set; }
	}

	public class StatisticReadValue
	{
		public StatisticReadValue()
		{
		}

		public StatisticReadValue(byte _propertyId, StatisticsBoardValue boardValue)
		{
			propertyId = _propertyId;
			value = boardValue.value;
			rankingCriterionIndex = boardValue.rankingCriterionIndex;
			sliceScore = boardValue.sliceScore;
			scoreLostForNextSlice = boardValue.scoreLostForNextSlice;
		}

		public byte propertyId { get; set; }
		public StatisticValueVariant? value { get; set; }
		public byte rankingCriterionIndex { get; set; }
		public StatisticValueVariant? sliceScore { get; set; }
		public StatisticValueVariant? scoreLostForNextSlice { get; set; }
	}

	public class StatisticReadValueByBoard
	{
		public StatisticReadValueByBoard()
		{
			scores = new List<StatisticReadValue>();
		}

		public int boardId { get; set; }
		public int rank { get; set; }
		public float score { get; set; }
		public ICollection<StatisticReadValue> scores { get; set; }
		public DateTime lastUpdate { get; set; }
	}

	public class ScoreListRead
	{
		public ScoreListRead()
		{
			scoresByBoard = new List<StatisticReadValueByBoard>();
		}

		public uint pid { get; set; }
		public string? pname { get; set; }
		public ICollection<StatisticReadValueByBoard> scoresByBoard { get; set; }
	}

	public class LeaderboardData
	{
		public LeaderboardData()
		{

		}

		public int boardId { get; set; }
		public int columnId { get; set; }
	}

	//---------------------------------------------------------------
	// Database stuff

	public class StatisticsBoardValue
	{
		public StatisticsBoardValue()
		{
			value = new StatisticValueVariant();
			sliceScore = new StatisticValueVariant();
			scoreLostForNextSlice = new StatisticValueVariant();
			rankingCriterionIndex = 1;
		}

		public StatisticsBoardValue(StatisticDesc desc) : this()
		{
			value.typeValue = sliceScore.typeValue = scoreLostForNextSlice.typeValue = (byte)desc.statType;
		}

		public StatisticsBoardValue(StatisticValueVariant initialValue) : this()
		{
			value = initialValue;
		}

		public bool UpdateValueWithPolicy(StatisticValueVariant newValue, StatisticPolicy policy)
		{
			value.typeValue = newValue.typeValue;
			switch (policy)
			{
				case StatisticPolicy.Overwrite:
					return value.Assign(newValue);
				case StatisticPolicy.Add:
					return value.Add(newValue);
				case StatisticPolicy.Sub:
					return value.Subtract(newValue);
				case StatisticPolicy.ReplaceIfMin:
					return value.ReplaceIfMin(newValue);
				case StatisticPolicy.ReplaceIfMax:
					return value.ReplaceIfMax(newValue);
			}

			// scoreLostForNextSlice is diff?
			// sliceScore hmmm?
			return false;
		}

		public StatisticValueVariant value { get; set; }
		public byte rankingCriterionIndex { get; set; }
		public StatisticValueVariant sliceScore { get; set; }
		public StatisticValueVariant scoreLostForNextSlice { get; set; }
	}
}
