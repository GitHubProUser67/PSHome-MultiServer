using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.ClanLadderListResponse)]
    public class MediusClanLadderListResponse : BaseLobbyMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.ClanLadderListResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public int ClanID;
        public string ClanName;
        public uint LadderPosition;
        public MediusCallbackStatus StatusCode;
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            reader.ReadBytes(3);
            ClanID = reader.ReadInt32();
            ClanName = reader.ReadString(Constants.CLANNAME_MAXLEN);
            LadderPosition = reader.ReadUInt32();
            StatusCode = reader.Read<MediusCallbackStatus>();
            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(new byte[3]);
            writer.Write(ClanID);
            writer.Write(ClanName, Constants.CLANNAME_MAXLEN);
            writer.Write(LadderPosition);
            writer.Write(StatusCode);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"ClanID:{ClanID} " +
                $"ClanName:{ClanName} " +
                $"LadderPosition:{LadderPosition} " +
                $"StatusCode:{StatusCode} " +
                $"EndOfList:{EndOfList}";
        }
    }
}
