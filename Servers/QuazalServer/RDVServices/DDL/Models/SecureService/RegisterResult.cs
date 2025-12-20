using QuazalServer.QNetZ.DDL;

namespace QuazalServer.RDVServices.DDL.Models
{
	public class RegisterResult
	{
		public uint retval { get; set; }
		public uint pidConnectionID { get; set; }
		public StationURL? urlPublic { get; set; }
	}
}