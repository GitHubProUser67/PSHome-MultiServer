namespace QuazalServer.RDVServices.DDL.Models
{
	public class NewsRecipient
	{
		public uint m_recipientID { get; set; }
		public uint m_recipientType { get; set; }
	}

	public class NewsHeader
	{
		public uint m_ID { get; set; }
		public uint m_recipientID { get; set; }
		public uint m_recipientType { get; set; }
		public uint m_publisherPID { get; set; }
		public string? m_publisherName { get; set; }
		public DateTime m_publicationTime { get; set; }
		public DateTime m_displayTime { get; set; }
		public DateTime m_expirationTime { get; set; }
		public string? m_title { get; set; }
		public string? m_link { get; set; }
	}
}
