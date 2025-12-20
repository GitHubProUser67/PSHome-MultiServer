namespace QuazalServer.RDVServices.DDL.Models
{
	public class FriendData
	{
		public uint m_pid { get; set; }
		public string? m_strName { get; set; }
		public byte m_byRelationship { get; set; }
		public uint m_uiDetails { get; set; }
		public string? m_strStatus { get; set; }
	}

	public class RelationshipData
	{
		public uint m_pid { get; set; }
		public string? m_strName { get; set; }
		public byte m_byRelationship { get; set; }
		public uint m_uiDetails { get; set; }
		public byte m_byStatus { get; set; }
	}

	public class RelationshipsResult
	{
		public RelationshipsResult()
		{
			uiTotalCount = 0;
			lstRelationshipsList = new List<RelationshipData>();
		}

		public uint uiTotalCount { get; set; }
		public IEnumerable<RelationshipData> lstRelationshipsList { get; set; }
	}
}
