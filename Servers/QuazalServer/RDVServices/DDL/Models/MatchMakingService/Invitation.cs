namespace QuazalServer.RDVServices.DDL.Models
{
	public class Invitation
	{
		public uint m_idGathering { get; set; }
		public uint m_idGuest { get; set; }
		public string? m_strMessage { get; set; }
	}

	public class SentInvitation
	{
		public uint SentById { get; set; }
		public uint GatheringId { get; set; }
		public uint GuestId { get; set; }
		public string? Message { get; set; }
	}
}
