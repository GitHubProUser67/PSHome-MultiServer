namespace QuazalServer.RDVServices.DDL.Models
{
	public class UbiAuthenticationLoginCustomData
	{
		public string? username { get; set; }
		public string? onlineKey { get; set; }
		public string? password { get; set; }
	}

    public class SonyNPTicket
    {
        public uint uiTicketSize { get; set; }
        public qBuffer? ticket { get; set; }
    }

    public class qBuffer
    {
        public ushort length { get; set; }
        public byte[]? data { get; set; }
    }
}
