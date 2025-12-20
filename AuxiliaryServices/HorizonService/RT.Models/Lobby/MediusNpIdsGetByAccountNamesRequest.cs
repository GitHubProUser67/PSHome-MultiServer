using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.MediusNpIdsGetByAccountNamesRequest)]
    public class MediusNpIdsGetByAccountNamesRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.MediusNpIdsGetByAccountNamesRequest;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public uint NumNames;
        public string[] AccountNames;

        public byte NAME_LEN;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            NumNames = reader.ReadUInt32();

            AccountNames = new string[NumNames];

            for (int i = 0; i < NumNames; i++)
            {
                NAME_LEN = reader.ReadByte();
                AccountNames[i] = reader.ReadString(NAME_LEN);
            }
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(NumNames);
            for (int i = 0; i < NumNames; i++)
            {
                writer.Write(NAME_LEN);
                writer.Write(AccountNames[i]);
            }
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"NumNames: {NumNames} " +
                $"AccountNames: {string.Join(" ", AccountNames)}";
        }
    }
}
