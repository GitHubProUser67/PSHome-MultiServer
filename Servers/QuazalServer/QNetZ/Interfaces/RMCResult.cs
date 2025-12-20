using QuazalServer.RDVServices.RMC;

namespace QuazalServer.QNetZ.Interfaces
{
    public class RMCResult
	{
		public RMCResult(RMCPResponse response, bool compression = true, uint error = 0)
		{
			Response = response;
			Compression = compression;
			Error = error;
		}

		public readonly RMCPResponse Response;
		public readonly bool Compression;
		public readonly uint Error;
	}
}
