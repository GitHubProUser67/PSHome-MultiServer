using QuazalServer.RDVServices.RMC;

namespace QuazalServer.QNetZ.Attributes
{
    /// <summary>
    /// RMC class attribute identifying class as a service/protocol hanlder
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
	public class RMCServiceAttribute : Attribute
	{
		public readonly ushort ProtocolId;
		public RMCServiceAttribute(ushort protocolId)
		{
			ProtocolId = protocolId;
		}
	}
}