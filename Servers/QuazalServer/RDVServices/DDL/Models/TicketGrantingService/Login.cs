using QuazalServer.QNetZ.DDL;

namespace QuazalServer.RDVServices.DDL.Models
{
	public class RVConnectionData
	{
		public RVConnectionData()
		{
			m_urlRegularProtocols = new StationURL();
			m_lstSpecialProtocols = Array.Empty<byte>();
			m_urlSpecialProtocols = new StationURL();
		}
		public StationURL m_urlRegularProtocols { get; set; }
        public byte[] m_lstSpecialProtocols { get; set; }
        public StationURL m_urlSpecialProtocols { get; set; }
    }

    public class Login
	{
		public Login()
		{

		}

		public Login(uint pid)
		{
			retVal =  0x10001;
			pidPrincipal = pid;
			pConnectionData = new RVConnectionData();
		}

		public uint retVal { get; set; }
        public uint pidPrincipal { get; set; }
        public byte[]? pbufResponse { get; set; }
        public RVConnectionData? pConnectionData { get; set; }
        public string? strReturnMsg { get; set; }
    }

	public class TicketData
	{
		public uint retVal { get; set; }
		public byte[]? pbufResponse { get; set; }
	}
}
