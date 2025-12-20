namespace QuazalServer.RDVServices.DDL.Models
{
	public class PresenceElement
	{
		public PresenceElement()
		{
			argument = new QNetZ.DDL.qBuffer();
		}

		public uint principalId { get; set; }
		public bool isConnected { get; set; }
		public int phraseId { get; set; }
		public QNetZ.DDL.qBuffer argument { get; set; }
	}
}
