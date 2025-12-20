using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.BuddySetListRequest)]
    public class MediusBuddySetListRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.BuddySetListRequest;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public int NumEntries;
        public string[] List;

        public byte NAME_LEN;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            //reader.ReadBytes(2);

            NumEntries = reader.ReadInt32();
            
            List = new string[NumEntries];
            
            for (int i = 0; i < NumEntries; i++)
            {
                NAME_LEN = reader.ReadByte();
                List[i] = reader.ReadString(NAME_LEN);
            }
            
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            //writer.Write(new byte[2]);

            writer.Write(NumEntries);
            
            for (int i = 0; i < NumEntries; i++)
            {
                writer.Write(NAME_LEN);
                writer.Write(List[i]);
            }
            
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"NumEntries: {NumEntries} " +
                $"List: {string.Join(" ", List)}";
        }
    }
}
