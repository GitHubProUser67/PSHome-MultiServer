using CustomLogger;

namespace QuazalServer.QNetZ
{
	public class DO
	{
        public enum METHOD
        {
            JoinRequest = 0x0,
            JoinResponse = 0x1,
            Update = 0x2,
            Delete = 0x4,
            Action = 0x5,
            CallOutcome = 0x8,
            RMCCall = 0xA,
            RMCResponse = 0xB,
            FetchRequest = 0xD,
            Bundle = 0xF,
            Migration = 0x11,
            CreateDuplicate = 0x12,
            CreateAndPromoteDuplicate = 0x13,
            GetParticipantsRequest = 0x14,
            GetParticipantsResponse = 0x15,
            NotHandledProtocol = 0xFE,
            EOS = 0xFF
        }
        
        public static void HandlePacket(QPacketHandlerPRUDP handler, QPacket p, QClient client)
		{
            client.SessionID = p.m_bySessionID;

            if (p.uiSeqId > client.SeqCounter)
                client.SeqCounter = p.uiSeqId;
            
            LoggerAccessor.LogError("[DO] - Duplicated objects are not implemented on this server.");
		}
	}
}
