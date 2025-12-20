namespace QuazalServer.RDVServices.DDL.Models
{
	public class RelaySignatureKey
	{
		public int relayMode { get; set; }
		public DateTime currentUTCTime { get; set; }
		public string? address { get; set; }
		public ushort port { get; set; }
		public int relayAddressType { get; set; }
		public uint gameServerID { get; set; }
	}
}
