using System.Net;

namespace QuazalServer.QNetZ
{
	// client connection info on particular server
	public class QClient
	{
		public enum StateType
		{
			PreCONNECT = 0,
			Active,
			Dropped
		}

		public QClient(uint recvId, IPEndPoint ep)
		{
			IDrecv = recvId;
			LastPacketTime = DateTime.UtcNow;
			Endpoint = ep;
			State = StateType.PreCONNECT;
		}

		public StateType State;

		public string? WIIFriendCode; // If WII, store this Variable
        public uint sPID; // server PID
		public ushort sPort; // server port
		public IPEndPoint Endpoint; // client endpoint
		public DateTime LastPacketTime;

		public byte SessionID;

		public uint IDrecv;	// connection signature for recieving
		public uint IDsend;	// connection signature for sending

		public ushort SeqCounter;
		public ushort SeqCounterOut;
		public uint CallCounterRMC;

		public PlayerInfo? PlayerInfo; // unique player info instance

        public double TimeSinceLastPacket
        {
            get
            {
                return (DateTime.UtcNow - LastPacketTime).TotalSeconds;
            }
        }
    }
}
