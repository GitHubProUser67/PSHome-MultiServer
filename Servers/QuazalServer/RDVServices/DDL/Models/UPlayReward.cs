namespace QuazalServer.RDVServices.DDL.Models
{
	public class UPlayRewardPlatform
	{
		public string? m_platformCode { get; set; }
		public bool m_purchased { get; set; }
	}

	public class UPlayReward
	{
		public string? m_code { get; set; }
		public string? m_name { get; set; }
		public string? m_description { get; set; }
		public int m_value { get; set; }
		public string? m_rewardTypeName { get; set; }
		public string? m_gameCode { get; set; }
		public IEnumerable<UPlayRewardPlatform>? m_platforms { get; set; }
	}
}
