using QuazalServer.RDVServices.RMC;

namespace QuazalServer.QNetZ.Interfaces
{
    public class RMCContext
	{
		public RMCContext(RMCPacket rmc, QPacketHandlerPRUDP handler, QClient clientInfo, QPacket packet)
		{
			RMC = rmc;
			Handler = handler;
			Client = clientInfo;
			Packet = packet;
		}

		public readonly RMCPacket RMC;
		public readonly QPacketHandlerPRUDP Handler;
		public readonly QClient Client;
		public readonly QPacket Packet;
	}
}
