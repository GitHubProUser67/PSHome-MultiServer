using Alcatraz.Context.Entities;
using Newtonsoft.Json;

namespace QuazalServer.RDVServices.DDL.Models
{
	public class StatisticDesc
	{
		public StatisticDesc()
		{
			statInstantBroadcast = false;
			statFriendComparison = false;
			statRankingOrder = RankingOrder.NotRanked;
			statDisplayType = VariantType.None;
		}
		public int statBoard;
		public int statInBoardId;
		public VariantType statType;
		public VariantType statDisplayType;
		public StatisticPolicy statWritePolicy;
		public bool statInstantBroadcast;
		public bool statFriendComparison;
		public RankingOrder statRankingOrder;
		public int statID;
		public string? statName;
	};

	public static class SeedStatistics
	{

		public static PlayerStatisticsBoard GeneratePlayerBoard(int boardId, uint playerId)
		{
			var playerBoard = new PlayerStatisticsBoard()
			{
				BoardId = boardId,
				PlayerId = playerId,
				Rank = 0,
				Score = 0.0f,
			};

			playerBoard.Values = new List<PlayerStatisticsBoardValue>();

			foreach (var statPropertyDesc in AllStatisticDescriptions.Where(x => x.statBoard == boardId))
			{
				var value = new StatisticsBoardValue(statPropertyDesc);
				playerBoard.Values.Add(new PlayerStatisticsBoardValue()
				{
					PropertyId = statPropertyDesc.statInBoardId,
					RankingCriterionIndex = value.rankingCriterionIndex,
					ValueJSON = JsonConvert.SerializeObject(value.value),
					ScoreLostForNextSliceJSON = JsonConvert.SerializeObject(value.scoreLostForNextSlice),
					SliceScoreJSON = JsonConvert.SerializeObject(value.sliceScore)
				});
			}

			return playerBoard;
		}

		public static StatisticsBoardValue GetStatisticBoardValueByPropertyId(int boardId, int propertyId)
		{
			var statPropertyDesc = AllStatisticDescriptions.FirstOrDefault(x => x.statBoard == boardId && x.statInBoardId == propertyId);
			return new StatisticsBoardValue(statPropertyDesc);
		}

		public static StatisticDesc[] AllStatisticDescriptions = {
			 new StatisticDesc() {
					statBoard = 1,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = true,
					statFriendComparison = false,
					statID = 1,
					statName = "XP"
			 },
			 new StatisticDesc() {
					statBoard = 1,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = true,
					statFriendComparison = false,
					statID = 2,
					statName = "Icon"
			 },
			 new StatisticDesc() {
					statBoard = 1,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = true,
					statFriendComparison = false,
					statID = 3,
					statName = "ZapSpawnCars"
			 },
			 new StatisticDesc() {
					statBoard = 1,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 4,
					statName = "TutorialCompleteFlags"
			 },
			 new StatisticDesc() {
					statBoard = 2,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 20,
					statName = "Tag Score"
			 },
			 new StatisticDesc() {
					statBoard = 2,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 21,
					statName = "Tag Wins"
			 },
			 new StatisticDesc() {
					statBoard = 2,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 22,
					statName = "Tag Losses"
			 },
			 new StatisticDesc() {
					statBoard = 2,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 23,
					statName = "Tag Time Tagged"
			 },
			 new StatisticDesc() {
					statBoard = 3,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 24,
					statName = "Takedown Score"
			 },
			 new StatisticDesc() {
					statBoard = 3,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 25,
					statName = "Takedown Wins"
			 },
			 new StatisticDesc() {
					statBoard = 3,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 26,
					statName = "Takedown Losses"
			 },
			 new StatisticDesc() {
					statBoard = 3,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 27,
					statName = "Takedown Drop Offs"
			 },
			 new StatisticDesc() {
					statBoard = 4,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 28,
					statName = "Blazer Score"
			 },
			 new StatisticDesc() {
					statBoard = 4,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 29,
					statName = "Blazer Wins"
			 },
			 new StatisticDesc() {
					statBoard = 4,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 30,
					statName = "Blazer Losses"
			 },
			 new StatisticDesc() {
					statBoard = 4,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 31,
					statName = "Blazer Time in Trails"
			 },
			 new StatisticDesc() {
					statBoard = 5,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 32,
					statName = "Tug Score"
			 },
			 new StatisticDesc() {
					statBoard = 5,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 33,
					statName = "Tug Wins"
			 },
			 new StatisticDesc() {
					statBoard = 5,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 34,
					statName = "Tug Losses"
			 },
			 new StatisticDesc() {
					statBoard = 5,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 35,
					statName = "Tug Flags Captured"
			 },
			 new StatisticDesc() {
					statBoard = 6,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 36,
					statName = "Burning Score"
			 },
			 new StatisticDesc() {
					statBoard = 6,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 37,
					statName = "Burning Wins"
			 },
			 new StatisticDesc() {
					statBoard = 6,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 38,
					statName = "Burning Losses"
			 },
			 new StatisticDesc() {
					statBoard = 6,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 39,
					statName = "Burning Checkpoints Crossed"
			 },
			 new StatisticDesc() {
					statBoard = 7,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 40,
					statName = "Rushdown Score"
			 },
			 new StatisticDesc() {
					statBoard = 7,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 41,
					statName = "Rushdown Wins"
			 },
			 new StatisticDesc() {
					statBoard = 7,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 42,
					statName = "Rushdown Losses"
			 },
			 new StatisticDesc() {
					statBoard = 7,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 43,
					statName = "Rushdown Objectives Destroyed"
			 },
			 new StatisticDesc() {
					statBoard = 8,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 44,
					statName = "Sprint Score"
			 },
			 new StatisticDesc() {
					statBoard = 8,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 45,
					statName = "Sprint Wins"
			 },
			 new StatisticDesc() {
					statBoard = 8,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 46,
					statName = "Sprint Losses"
			 },
			 new StatisticDesc() {
					statBoard = 8,
					statInBoardId = 4,
					statType = VariantType.Float,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 47,
					statName = "Sprint Average Placing",
					statDisplayType = VariantType.int32
			 },
			 new StatisticDesc() {
					statBoard = 9,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 48,
					statName = "Circuit Score"
			 },
			 new StatisticDesc() {
					statBoard = 9,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 49,
					statName = "Circuit Wins"
			 },
			 new StatisticDesc() {
					statBoard = 9,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 50,
					statName = "Circuit Losses"
			 },
			 new StatisticDesc() {
					statBoard = 9,
					statInBoardId = 4,
					statType = VariantType.Float,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 51,
					statName = "Circuit Average Placing",
					statDisplayType = VariantType.int32
			 },
			 new StatisticDesc() {
					statBoard = 10,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 52,
					statName = "Pure Score"
			 },
			 new StatisticDesc() {
					statBoard = 10,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 53,
					statName = "Pure Wins"
			 },
			 new StatisticDesc() {
					statBoard = 10,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 54,
					statName = "Pure Losses"
			 },
			 new StatisticDesc() {
					statBoard = 10,
					statInBoardId = 4,
					statType = VariantType.Float,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 55,
					statName = "Pure Average Placing",
					statDisplayType = VariantType.int32
			 },
			 new StatisticDesc() {
					statBoard = 11,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 56,
					statName = "Team Circuit Race Score"
			 },
			 new StatisticDesc() {
					statBoard = 11,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 57,
					statName = "Team Circuit Race Wins"
			 },
			 new StatisticDesc() {
					statBoard = 11,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 58,
					statName = "Team Circuit Race Losses"
			 },
			 new StatisticDesc() {
					statBoard = 11,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 59,
					statName = "Team Circuit Race Checkpoints Crossed"
			 },
			 new StatisticDesc() {
					statBoard = 12,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 60,
					statName = "Overall Score"
			 },
			 new StatisticDesc() {
					statBoard = 12,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 61,
					statName = "Overall Wins"
			 },
			 new StatisticDesc() {
					statBoard = 12,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 62,
					statName = "Overall Losses"
			 },
			 new StatisticDesc() {
					statBoard = 12,
					statInBoardId = 4,
					statType = VariantType.Float,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 63,
					statName = "Overall Win/Loss Ratio"
			 },
			 new StatisticDesc() {
					statBoard = 13,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 64,
					statName = "Rush Score"
			 },
			 new StatisticDesc() {
					statBoard = 13,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 65,
					statName = "Rush Wins"
			 },
			 new StatisticDesc() {
					statBoard = 13,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 66,
					statName = "Rush Losses"
			 },
			 new StatisticDesc() {
					statBoard = 13,
					statInBoardId = 4,
					statType = VariantType.Float,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 67,
					statName = "Rush Average Placing",
					statDisplayType = VariantType.int32
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 150,
					statName = "TheFreewayRun_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 151,
					statName = "TheFreewayRun_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 152,
					statName = "TheFreewayRun_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 153,
					statName = "TheFreewayRun_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 154,
					statName = "TheFreewayRun_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 155,
					statName = "TheFreewayRun_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 156,
					statName = "TheFreewayRun_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 157,
					statName = "TheFreewayRun_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 158,
					statName = "TheFreewayRun_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 159,
					statName = "TheFreewayRun_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 160,
					statName = "TheFreewayRun_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 161,
					statName = "TheFreewayRun_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 162,
					statName = "TheFreewayRun_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 163,
					statName = "TheFreewayRun_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 164,
					statName = "TheFreewayRun_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 165,
					statName = "TheFreewayRun_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 26,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 166,
					statName = "TheFreewayRun_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 27,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 167,
					statName = "TheFreewayRun"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 200,
					statName = "DowntownSprint_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 201,
					statName = "DowntownSprint_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 202,
					statName = "DowntownSprint_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 203,
					statName = "DowntownSprint_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 204,
					statName = "DowntownSprint_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 205,
					statName = "DowntownSprint_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 206,
					statName = "DowntownSprint_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 207,
					statName = "DowntownSprint_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 208,
					statName = "DowntownSprint_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 209,
					statName = "DowntownSprint_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 210,
					statName = "DowntownSprint_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 211,
					statName = "DowntownSprint_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 28,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 212,
					statName = "DowntownSprint_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 29,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 213,
					statName = "DowntownSprint"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 250,
					statName = "Escape_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 251,
					statName = "Escape_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 252,
					statName = "Escape_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 253,
					statName = "Escape_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 254,
					statName = "Escape_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 255,
					statName = "Escape_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 256,
					statName = "Escape_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 257,
					statName = "Escape_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 258,
					statName = "Escape_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 259,
					statName = "Escape_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 260,
					statName = "Escape_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 261,
					statName = "Escape_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 262,
					statName = "Escape_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 263,
					statName = "Escape_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 264,
					statName = "Escape_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 265,
					statName = "Escape_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 266,
					statName = "Escape_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 267,
					statName = "Escape_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 268,
					statName = "Escape_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 269,
					statName = "Escape_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 21,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 270,
					statName = "Escape_SplitTime_21"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 22,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 271,
					statName = "Escape_SplitTime_22"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 23,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 272,
					statName = "Escape_SplitTime_23"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 24,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 273,
					statName = "Escape_SplitTime_24"
			 },
			 new StatisticDesc() {
					statBoard = 30,
					statInBoardId = 25,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 274,
					statName = "Escape_SplitTime_25"
			 },
			 new StatisticDesc() {
					statBoard = 31,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 275,
					statName = "Escape"
			 },
			 new StatisticDesc() {
					statBoard = 32,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMax,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 300,
					statName = "ChinatownDrift"
			 },
			 new StatisticDesc() {
					statBoard = 33,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 350,
					statName = "TheGetaway"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 400,
					statName = "GoldenGateCircuit_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 401,
					statName = "GoldenGateCircuit_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 402,
					statName = "GoldenGateCircuit_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 403,
					statName = "GoldenGateCircuit_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 404,
					statName = "GoldenGateCircuit_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 405,
					statName = "GoldenGateCircuit_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 406,
					statName = "GoldenGateCircuit_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 407,
					statName = "GoldenGateCircuit_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 408,
					statName = "GoldenGateCircuit_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 409,
					statName = "GoldenGateCircuit_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 410,
					statName = "GoldenGateCircuit_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 411,
					statName = "GoldenGateCircuit_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 412,
					statName = "GoldenGateCircuit_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 413,
					statName = "GoldenGateCircuit_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 414,
					statName = "GoldenGateCircuit_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 415,
					statName = "GoldenGateCircuit_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 416,
					statName = "GoldenGateCircuit_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 417,
					statName = "GoldenGateCircuit_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 418,
					statName = "GoldenGateCircuit_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 419,
					statName = "GoldenGateCircuit_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 21,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 420,
					statName = "GoldenGateCircuit_SplitTime_21"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 22,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 421,
					statName = "GoldenGateCircuit_SplitTime_22"
			 },
			 new StatisticDesc() {
					statBoard = 34,
					statInBoardId = 23,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 422,
					statName = "GoldenGateCircuit_SplitTime_23"
			 },
			 new StatisticDesc() {
					statBoard = 35,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 423,
					statName = "GoldenGateCircuit"
			 },
			 new StatisticDesc() {
					statBoard = 36,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 483,
					statName = "RussianHillTakedown"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 500,
					statName = "Offroad_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 501,
					statName = "Offroad_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 502,
					statName = "Offroad_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 503,
					statName = "Offroad_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 504,
					statName = "Offroad_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 505,
					statName = "Offroad_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 506,
					statName = "Offroad_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 507,
					statName = "Offroad_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 508,
					statName = "Offroad_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 509,
					statName = "Offroad_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 37,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 510,
					statName = "Offroad_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 38,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 511,
					statName = "Offroad"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 550,
					statName = "LuckyEscape_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 551,
					statName = "LuckyEscape_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 552,
					statName = "LuckyEscape_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 553,
					statName = "LuckyEscape_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 554,
					statName = "LuckyEscape_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 555,
					statName = "LuckyEscape_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 556,
					statName = "LuckyEscape_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 557,
					statName = "LuckyEscape_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 558,
					statName = "LuckyEscape_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 559,
					statName = "LuckyEscape_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 560,
					statName = "LuckyEscape_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 561,
					statName = "LuckyEscape_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 562,
					statName = "LuckyEscape_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 563,
					statName = "LuckyEscape_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 39,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 564,
					statName = "LuckyEscape_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 40,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 565,
					statName = "LuckyEscape"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 600,
					statName = "Speed_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 601,
					statName = "Speed_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 602,
					statName = "Speed_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 603,
					statName = "Speed_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 604,
					statName = "Speed_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 605,
					statName = "Speed_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 606,
					statName = "Speed_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 607,
					statName = "Speed_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 608,
					statName = "Speed_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 609,
					statName = "Speed_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 610,
					statName = "Speed_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 611,
					statName = "Speed_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 612,
					statName = "Speed_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 613,
					statName = "Speed_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 41,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 614,
					statName = "Speed_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 42,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 615,
					statName = "Speed"
			 },
			 new StatisticDesc() {
					statBoard = 43,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 650,
					statName = "MarinCopRun_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 43,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 651,
					statName = "MarinCopRun_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 43,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 652,
					statName = "MarinCopRun_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 43,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 653,
					statName = "MarinCopRun_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 43,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 654,
					statName = "MarinCopRun_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 43,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 655,
					statName = "MarinCopRun_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 43,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 656,
					statName = "MarinCopRun_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 43,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 657,
					statName = "MarinCopRun_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 43,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 658,
					statName = "MarinCopRun_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 44,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 659,
					statName = "MarinCopRun"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 750,
					statName = "RallyFaceOff_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 751,
					statName = "RallyFaceOff_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 752,
					statName = "RallyFaceOff_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 753,
					statName = "RallyFaceOff_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 754,
					statName = "RallyFaceOff_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 755,
					statName = "RallyFaceOff_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 756,
					statName = "RallyFaceOff_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 757,
					statName = "RallyFaceOff_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 758,
					statName = "RallyFaceOff_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 759,
					statName = "RallyFaceOff_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 760,
					statName = "RallyFaceOff_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 46,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 761,
					statName = "RallyFaceOff_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 47,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 762,
					statName = "RallyFaceOff"
			 },
			 new StatisticDesc() {
					statBoard = 48,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 800,
					statName = "TheDriver"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 850,
					statName = "FreewayFaceoff_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 851,
					statName = "FreewayFaceoff_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 852,
					statName = "FreewayFaceoff_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 853,
					statName = "FreewayFaceoff_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 854,
					statName = "FreewayFaceoff_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 855,
					statName = "FreewayFaceoff_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 856,
					statName = "FreewayFaceoff_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 857,
					statName = "FreewayFaceoff_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 858,
					statName = "FreewayFaceoff_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 859,
					statName = "FreewayFaceoff_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 860,
					statName = "FreewayFaceoff_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 861,
					statName = "FreewayFaceoff_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 862,
					statName = "FreewayFaceoff_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 863,
					statName = "FreewayFaceoff_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 864,
					statName = "FreewayFaceoff_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 865,
					statName = "FreewayFaceoff_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 866,
					statName = "FreewayFaceoff_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 867,
					statName = "FreewayFaceoff_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 868,
					statName = "FreewayFaceoff_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 869,
					statName = "FreewayFaceoff_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 21,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 870,
					statName = "FreewayFaceoff_SplitTime_21"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 22,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 871,
					statName = "FreewayFaceoff_SplitTime_22"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 23,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 872,
					statName = "FreewayFaceoff_SplitTime_23"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 24,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 873,
					statName = "FreewayFaceoff_SplitTime_24"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 25,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 874,
					statName = "FreewayFaceoff_SplitTime_25"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 26,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 875,
					statName = "FreewayFaceoff_SplitTime_26"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 27,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 876,
					statName = "FreewayFaceoff_SplitTime_27"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 28,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 877,
					statName = "FreewayFaceoff_SplitTime_28"
			 },
			 new StatisticDesc() {
					statBoard = 49,
					statInBoardId = 29,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 878,
					statName = "FreewayFaceoff_SplitTime_29"
			 },
			 new StatisticDesc() {
					statBoard = 50,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 879,
					statName = "FreewayFaceoff"
			 },
			 new StatisticDesc() {
					statBoard = 51,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMax,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 900,
					statName = "Survival"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1000,
					statName = "MarinEscape_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1001,
					statName = "MarinEscape_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1002,
					statName = "MarinEscape_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1003,
					statName = "MarinEscape_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1004,
					statName = "MarinEscape_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1005,
					statName = "MarinEscape_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1006,
					statName = "MarinEscape_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1007,
					statName = "MarinEscape_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1008,
					statName = "MarinEscape_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1009,
					statName = "MarinEscape_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1010,
					statName = "MarinEscape_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1011,
					statName = "MarinEscape_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1012,
					statName = "MarinEscape_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1013,
					statName = "MarinEscape_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 53,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1014,
					statName = "MarinEscape_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 54,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1015,
					statName = "MarinEscape"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1050,
					statName = "ItalianJob_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1051,
					statName = "ItalianJob_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1052,
					statName = "ItalianJob_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1053,
					statName = "ItalianJob_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1054,
					statName = "ItalianJob_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1055,
					statName = "ItalianJob_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1056,
					statName = "ItalianJob_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1057,
					statName = "ItalianJob_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1058,
					statName = "ItalianJob_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1059,
					statName = "ItalianJob_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1060,
					statName = "ItalianJob_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1061,
					statName = "ItalianJob_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1062,
					statName = "ItalianJob_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1063,
					statName = "ItalianJob_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1064,
					statName = "ItalianJob_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1065,
					statName = "ItalianJob_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1066,
					statName = "ItalianJob_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1067,
					statName = "ItalianJob_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1068,
					statName = "ItalianJob_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1069,
					statName = "ItalianJob_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 21,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1070,
					statName = "ItalianJob_SplitTime_21"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 22,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1071,
					statName = "ItalianJob_SplitTime_22"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 23,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1072,
					statName = "ItalianJob_SplitTime_23"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 24,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1073,
					statName = "ItalianJob_SplitTime_24"
			 },
			 new StatisticDesc() {
					statBoard = 55,
					statInBoardId = 25,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1074,
					statName = "ItalianJob_SplitTime_25"
			 },
			 new StatisticDesc() {
					statBoard = 56,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1080,
					statName = "ItalianJob"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1100,
					statName = "Uplaych1_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1101,
					statName = "Uplaych1_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1102,
					statName = "Uplaych1_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1103,
					statName = "Uplaych1_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1104,
					statName = "Uplaych1_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1105,
					statName = "Uplaych1_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1106,
					statName = "Uplaych1_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1107,
					statName = "Uplaych1_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1108,
					statName = "Uplaych1_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1109,
					statName = "Uplaych1_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1110,
					statName = "Uplaych1_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1111,
					statName = "Uplaych1_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1112,
					statName = "Uplaych1_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1113,
					statName = "Uplaych1_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1114,
					statName = "Uplaych1_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1115,
					statName = "Uplaych1_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1116,
					statName = "Uplaych1_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1117,
					statName = "Uplaych1_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1118,
					statName = "Uplaych1_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1119,
					statName = "Uplaych1_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 21,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1120,
					statName = "Uplaych1_SplitTime_21"
			 },
			 new StatisticDesc() {
					statBoard = 57,
					statInBoardId = 22,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1121,
					statName = "Uplaych1_SplitTime_22"
			 },
			 new StatisticDesc() {
					statBoard = 58,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1122,
					statName = "Uplaych1"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1150,
					statName = "Uplaych2_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1151,
					statName = "Uplaych2_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1152,
					statName = "Uplaych2_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1153,
					statName = "Uplaych2_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1154,
					statName = "Uplaych2_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1155,
					statName = "Uplaych2_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1156,
					statName = "Uplaych2_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1157,
					statName = "Uplaych2_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1158,
					statName = "Uplaych2_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1159,
					statName = "Uplaych2_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1160,
					statName = "Uplaych2_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1161,
					statName = "Uplaych2_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1162,
					statName = "Uplaych2_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1163,
					statName = "Uplaych2_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1164,
					statName = "Uplaych2_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1165,
					statName = "Uplaych2_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1166,
					statName = "Uplaych2_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1167,
					statName = "Uplaych2_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 59,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1168,
					statName = "Uplaych2_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 60,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1169,
					statName = "Uplaych2"
			 },
			 new StatisticDesc() {
					statBoard = 61,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1200,
					statName = "Uplaych3"
			 },
			 new StatisticDesc() {
					statBoard = 62,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMax,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 1230,
					statName = "Uplaych4"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1250,
					statName = "Uplaych5_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1251,
					statName = "Uplaych5_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1252,
					statName = "Uplaych5_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1253,
					statName = "Uplaych5_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1254,
					statName = "Uplaych5_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1255,
					statName = "Uplaych5_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1256,
					statName = "Uplaych5_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1257,
					statName = "Uplaych5_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1258,
					statName = "Uplaych5_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1259,
					statName = "Uplaych5_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1260,
					statName = "Uplaych5_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1261,
					statName = "Uplaych5_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1262,
					statName = "Uplaych5_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1263,
					statName = "Uplaych5_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1264,
					statName = "Uplaych5_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1265,
					statName = "Uplaych5_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1266,
					statName = "Uplaych5_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1267,
					statName = "Uplaych5_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1268,
					statName = "Uplaych5_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1269,
					statName = "Uplaych5_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 21,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1280,
					statName = "Uplaych5_SplitTime_21"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 22,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1281,
					statName = "Uplaych5_SplitTime_22"
			 },
			 new StatisticDesc() {
					statBoard = 63,
					statInBoardId = 23,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1282,
					statName = "Uplaych5_SplitTime_23"
			 },
			 new StatisticDesc() {
					statBoard = 64,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1283,
					statName = "Uplaych5"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1300,
					statName = "RelayRace_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1301,
					statName = "RelayRace_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1302,
					statName = "RelayRace_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1303,
					statName = "RelayRace_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1304,
					statName = "RelayRace_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1305,
					statName = "RelayRace_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1306,
					statName = "RelayRace_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1307,
					statName = "RelayRace_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1308,
					statName = "RelayRace_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1309,
					statName = "RelayRace_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1310,
					statName = "RelayRace_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1311,
					statName = "RelayRace_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1312,
					statName = "RelayRace_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1313,
					statName = "RelayRace_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1314,
					statName = "RelayRace_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1315,
					statName = "RelayRace_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1316,
					statName = "RelayRace_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1317,
					statName = "RelayRace_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1318,
					statName = "RelayRace_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1319,
					statName = "RelayRace_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 21,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1320,
					statName = "RelayRace_SplitTime_21"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 22,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1321,
					statName = "RelayRace_SplitTime_22"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 23,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1322,
					statName = "RelayRace_SplitTime_23"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 24,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1323,
					statName = "RelayRace_SplitTime_24"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 25,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1324,
					statName = "RelayRace_SplitTime_25"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 26,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1325,
					statName = "RelayRace_SplitTime_26"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 27,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1326,
					statName = "RelayRace_SplitTime_27"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 28,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1327,
					statName = "RelayRace_SplitTime_28"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 29,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1328,
					statName = "RelayRace_SplitTime_29"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 30,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1329,
					statName = "RelayRace_SplitTime_30"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 31,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1330,
					statName = "RelayRace_SplitTime_31"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 32,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1331,
					statName = "RelayRace_SplitTime_32"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 33,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1332,
					statName = "RelayRace_SplitTime_33"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 34,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1333,
					statName = "RelayRace_SplitTime_34"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 35,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1334,
					statName = "RelayRace_SplitTime_35"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 36,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1335,
					statName = "RelayRace_SplitTime_36"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 37,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1336,
					statName = "RelayRace_SplitTime_37"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 38,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1337,
					statName = "RelayRace_SplitTime_38"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 39,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1338,
					statName = "RelayRace_SplitTime_39"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 40,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1339,
					statName = "RelayRace_SplitTime_40"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 41,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1340,
					statName = "RelayRace_SplitTime_41"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 42,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1341,
					statName = "RelayRace_SplitTime_42"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 43,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1342,
					statName = "RelayRace_SplitTime_43"
			 },
			 new StatisticDesc() {
					statBoard = 65,
					statInBoardId = 44,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1343,
					statName = "RelayRace_SplitTime_44"
			 },
			 new StatisticDesc() {
					statBoard = 66,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1344,
					statName = "RelayRace"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1350,
					statName = "Team colours tutorial_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1351,
					statName = "Team colours tutorial_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1352,
					statName = "Team colours tutorial_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1353,
					statName = "Team colours tutorial_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1354,
					statName = "Team colours tutorial_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1355,
					statName = "Team colours tutorial_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1356,
					statName = "Team colours tutorial_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1357,
					statName = "Team colours tutorial_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1358,
					statName = "Team colours tutorial_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1359,
					statName = "Team colours tutorial_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1360,
					statName = "Team colours tutorial_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1361,
					statName = "Team colours tutorial_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1362,
					statName = "Team colours tutorial_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1363,
					statName = "Team colours tutorial_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1364,
					statName = "Team colours tutorial_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1365,
					statName = "Team colours tutorial_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1366,
					statName = "Team colours tutorial_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1367,
					statName = "Team colours tutorial_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1368,
					statName = "Team colours tutorial_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1369,
					statName = "Team colours tutorial_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 21,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1370,
					statName = "Team colours tutorial_SplitTime_21"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 22,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1371,
					statName = "Team colours tutorial_SplitTime_22"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 23,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1372,
					statName = "Team colours tutorial_SplitTime_23"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 24,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1373,
					statName = "Team colours tutorial_SplitTime_24"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 25,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1374,
					statName = "Team colours tutorial_SplitTime_25"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 26,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1375,
					statName = "Team colours tutorial_SplitTime_26"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 27,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1376,
					statName = "Team colours tutorial_SplitTime_27"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 28,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1377,
					statName = "Team colours tutorial_SplitTime_28"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 29,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1378,
					statName = "Team colours tutorial_SplitTime_29"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 30,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1379,
					statName = "Team colours tutorial_SplitTime_30"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 31,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1380,
					statName = "Team colours tutorial_SplitTime_31"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 32,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1381,
					statName = "Team colours tutorial_SplitTime_32"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 33,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1382,
					statName = "Team colours tutorial_SplitTime_33"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 34,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1383,
					statName = "Team colours tutorial_SplitTime_34"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 35,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1384,
					statName = "Team colours tutorial_SplitTime_35"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 36,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1385,
					statName = "Team colours tutorial_SplitTime_36"
			 },
			 new StatisticDesc() {
					statBoard = 67,
					statInBoardId = 37,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1386,
					statName = "Team colours tutorial_SplitTime_37"
			 },
			 new StatisticDesc() {
					statBoard = 68,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1387,
					statName = "Team colours tutorial"
			 },
			 new StatisticDesc() {
					statBoard = 69,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1400,
					statName = "Mass Chase 2"
			 },
			 new StatisticDesc() {
					statBoard = 70,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1450,
					statName = "Smoketrail_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 70,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1451,
					statName = "Smoketrail_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 70,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1452,
					statName = "Smoketrail_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 70,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1453,
					statName = "Smoketrail_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 70,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1454,
					statName = "Smoketrail_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 70,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1455,
					statName = "Smoketrail_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 70,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1456,
					statName = "Smoketrail_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 71,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1457,
					statName = "Smoketrail"
			 },
			 new StatisticDesc() {
					statBoard = 72,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1500,
					statName = "Car park"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1550,
					statName = "CopOut_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1551,
					statName = "CopOut_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1552,
					statName = "CopOut_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1553,
					statName = "CopOut_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1554,
					statName = "CopOut_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1555,
					statName = "CopOut_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1556,
					statName = "CopOut_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1557,
					statName = "CopOut_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1558,
					statName = "CopOut_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1559,
					statName = "CopOut_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1560,
					statName = "CopOut_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1561,
					statName = "CopOut_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1562,
					statName = "CopOut_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1563,
					statName = "CopOut_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1564,
					statName = "CopOut_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1565,
					statName = "CopOut_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1566,
					statName = "CopOut_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1567,
					statName = "CopOut_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1568,
					statName = "CopOut_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 73,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1569,
					statName = "CopOut_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 74,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1570,
					statName = "CopOut"
			 },
			 new StatisticDesc() {
					statBoard = 75,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1600,
					statName = "Handle challenge"
			 },
			 new StatisticDesc() {
					statBoard = 76,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMax,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 1650,
					statName = "DriveToSurvive2"
			 },
			 new StatisticDesc() {
					statBoard = 77,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1700,
					statName = "Charity 2"
			 },
			 new StatisticDesc() {
					statBoard = 78,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1750,
					statName = "Deactivating bombs 2"
			 },
			 new StatisticDesc() {
					statBoard = 79,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMax,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Descending,
					statID = 1800,
					statName = "Big break 2"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 700,
					statName = "DownhillDrift_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 701,
					statName = "DownhillDrift_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 702,
					statName = "DownhillDrift_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 703,
					statName = "DownhillDrift_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 704,
					statName = "DownhillDrift_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 705,
					statName = "DownhillDrift_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 706,
					statName = "DownhillDrift_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 707,
					statName = "DownhillDrift_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 708,
					statName = "DownhillDrift_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 709,
					statName = "DownhillDrift_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 710,
					statName = "DownhillDrift_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 711,
					statName = "DownhillDrift_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 712,
					statName = "DownhillDrift_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 713,
					statName = "DownhillDrift_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 714,
					statName = "DownhillDrift_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 715,
					statName = "DownhillDrift_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 716,
					statName = "DownhillDrift_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 717,
					statName = "DownhillDrift_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 718,
					statName = "DownhillDrift_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 80,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 719,
					statName = "DownhillDrift_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 81,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 720,
					statName = "DownhillDrift"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1850,
					statName = "HardcoreChallenge_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1851,
					statName = "HardcoreChallenge_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1852,
					statName = "HardcoreChallenge_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1853,
					statName = "HardcoreChallenge_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1854,
					statName = "HardcoreChallenge_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1855,
					statName = "HardcoreChallenge_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1856,
					statName = "HardcoreChallenge_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1857,
					statName = "HardcoreChallenge_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1858,
					statName = "HardcoreChallenge_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1859,
					statName = "HardcoreChallenge_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1860,
					statName = "HardcoreChallenge_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1861,
					statName = "HardcoreChallenge_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1862,
					statName = "HardcoreChallenge_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1863,
					statName = "HardcoreChallenge_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1864,
					statName = "HardcoreChallenge_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1865,
					statName = "HardcoreChallenge_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1866,
					statName = "HardcoreChallenge_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1867,
					statName = "HardcoreChallenge_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1868,
					statName = "HardcoreChallenge_SplitTime_19"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1869,
					statName = "HardcoreChallenge_SplitTime_20"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 21,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1870,
					statName = "HardcoreChallenge_SplitTime_21"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 22,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1871,
					statName = "HardcoreChallenge_SplitTime_22"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 23,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1872,
					statName = "HardcoreChallenge_SplitTime_23"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 24,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1873,
					statName = "HardcoreChallenge_SplitTime_24"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 25,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1874,
					statName = "HardcoreChallenge_SplitTime_25"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 26,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1875,
					statName = "HardcoreChallenge_SplitTime_26"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 27,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1876,
					statName = "HardcoreChallenge_SplitTime_27"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 28,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1877,
					statName = "HardcoreChallenge_SplitTime_28"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 29,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1878,
					statName = "HardcoreChallenge_SplitTime_29"
			 },
			 new StatisticDesc() {
					statBoard = 82,
					statInBoardId = 30,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 1879,
					statName = "HardcoreChallenge_SplitTime_30"
			 },
			 new StatisticDesc() {
					statBoard = 83,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 1880,
					statName = "HardcoreChallenge"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 950,
					statName = "SutroDrift_SplitTime_1"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 951,
					statName = "SutroDrift_SplitTime_2"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 952,
					statName = "SutroDrift_SplitTime_3"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 953,
					statName = "SutroDrift_SplitTime_4"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 954,
					statName = "SutroDrift_SplitTime_5"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 955,
					statName = "SutroDrift_SplitTime_6"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 956,
					statName = "SutroDrift_SplitTime_7"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 957,
					statName = "SutroDrift_SplitTime_8"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 958,
					statName = "SutroDrift_SplitTime_9"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 959,
					statName = "SutroDrift_SplitTime_10"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 960,
					statName = "SutroDrift_SplitTime_11"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 961,
					statName = "SutroDrift_SplitTime_12"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 962,
					statName = "SutroDrift_SplitTime_13"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 963,
					statName = "SutroDrift_SplitTime_14"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 964,
					statName = "SutroDrift_SplitTime_15"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 965,
					statName = "SutroDrift_SplitTime_16"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 17,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 966,
					statName = "SutroDrift_SplitTime_17"
			 },
			 new StatisticDesc() {
					statBoard = 84,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 967,
					statName = "SutroDrift_SplitTime_18"
			 },
			 new StatisticDesc() {
					statBoard = 85,
					statInBoardId = 1,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.ReplaceIfMin,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statRankingOrder = RankingOrder.Ascending,
					statID = 968,
					statName = "SutroDrift"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 1,
					statType = VariantType.Float,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2544,
					statName = "GetTimePlayed"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 2,
					statType = VariantType.Float,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2545,
					statName = "Vehicle_GetTimeInAnyCar"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 3,
					statType = VariantType.Float,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2546,
					statName = "Vehicle_GetAverageSpeed"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2547,
					statName = "Favourite car"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2548,
					statName = "Vehicle_GetDistanceForward"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2549,
					statName = "HitMiss_GetNumCarsHit"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2550,
					statName = "HitMiss_GetNumNearMisses"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2551,
					statName = "HitMiss_GetNumOvertakes"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2552,
					statName = "HitMiss_GetLongestOvertakeChain"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2553,
					statName = "Manoeuvres_GetNumHandbrakeTurns"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2554,
					statName = "Manoeuvres_GetNum180s"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 12,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2555,
					statName = "Manoeuvres_GetNum360s"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2556,
					statName = "Manoeuvres_GetNumJTurns"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 14,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2557,
					statName = "Manoeuvres_GetNumTrailersDrivenUnder"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 15,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2558,
					statName = "Manoeuvres_GetNumVehicleTowed"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 16,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2559,
					statName = "Zap_GetNumZaps"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 17,
					statType = VariantType.Float,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2560,
					statName = "Zap_GetTimeInZap"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 18,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2561,
					statName = "Felony_GetNumFeloniesTriggered"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 19,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2562,
					statName = "Chase_GetCriminalsAprehended"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 20,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2563,
					statName = "Chase_GetCriminalsEscaped"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 21,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2564,
					statName = "Getaway_GetTimesTriggered"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 22,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2565,
					statName = "Getaway_GetTimesEscaped"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 23,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2566,
					statName = "Getaway_GetTimesAprehended"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 24,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2567,
					statName = "Jumps_GetNumJumps"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 25,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2568,
					statName = "Jumps_GetTotalJumpDistance"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 26,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2569,
					statName = "Jumps_GetLongestJumpDistance"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 27,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2570,
					statName = "Jumps_GetAverageJumpDistance"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 28,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2571,
					statName = "Jumps_GetNumVehiclesJumpedOver"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 29,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2572,
					statName = "Jumps_GetNumRampTruckJumped"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 30,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2573,
					statName = "Drift_GetNumDrifts"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 31,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2574,
					statName = "Drift_GetTotalDistance"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 32,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2575,
					statName = "Drift_GetLongestSingleDistance"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 33,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2576,
					statName = "Boost_GetNumBoosts"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 34,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2577,
					statName = "Boost_GetLongest"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 35,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2578,
					statName = "Ram_GetNumRams"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 36,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2579,
					statName = "Ram_PercentHit"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 37,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2580,
					statName = "SP_GetActivitiesCompleted"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 38,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2581,
					statName = "SP_GetDaresCompleted"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 39,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2582,
					statName = "SP_GetMovieTokensCollected"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 40,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2583,
					statName = "iMissionsPassed"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 41,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2584,
					statName = "MP_GetNumCarSwaps"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 42,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2585,
					statName = "MP_GetNumSuccessfulZapAttacks"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 43,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2586,
					statName = "Challenges_GetNumCompleted"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 44,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2587,
					statName = "Challenges_GetFavouriteChallengeID"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 45,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2588,
					statName = "SP_GetSWillpowerEarned"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 46,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2589,
					statName = "SP_GetSpentWillpower"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 47,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2590,
					statName = "SP_GetWillpowerFromMissions"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 48,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2591,
					statName = "SP_GetWillpowerFromDriving"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 49,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2592,
					statName = "SP_GetWillpowerFromGarages"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 50,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2593,
					statName = "Garages_GetGaragesOwned"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 51,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2594,
					statName = "Garages_GetVehiclesOwned"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 52,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2595,
					statName = "Garages_GarageUpgrades"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 53,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2596,
					statName = "Chase_GetTimesTriggered"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 54,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2597,
					statName = "SP_GetMostPlayedActivity"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 55,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2598,
					statName = "MP_LEVEL"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 56,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2599,
					statName = "SP_GetGameCompletitionPercentage"
			 },
			 new StatisticDesc() {
					statBoard = 100,
					statInBoardId = 57,
					statType = VariantType.Float,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2600,
					statName = "Manoeuvres_GetNumBarrelRolls"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 2,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2601,
					statName = "Tag Last Rank"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 3,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2602,
					statName = "Takedown Last Rank"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 4,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2603,
					statName = "Blazer Last Rank"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 5,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2604,
					statName = "Tug Rank"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 6,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2605,
					statName = "Burning Rank"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 7,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2606,
					statName = "Rushdown Rank"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 8,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2607,
					statName = "Sprint Rank"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 9,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2608,
					statName = "Circuit Rank"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 10,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2609,
					statName = "Pure Rank"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 11,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2610,
					statName = "Team Circuit Rank"
			 },
			 new StatisticDesc() {
					statBoard = 101,
					statInBoardId = 13,
					statType = VariantType.int32,
					statWritePolicy = StatisticPolicy.Overwrite,
					statInstantBroadcast = false,
					statFriendComparison = false,
					statID = 2611,
					statName = "Rush Last Rank"
			  }
		};
	}
}
