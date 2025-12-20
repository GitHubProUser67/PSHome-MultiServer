using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.UniverseNewsResponse)]
    public class MediusUniverseNewsResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.UniverseNewsResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public string News; // NEWS_MAXLEN
        public MediusCallbackStatus StatusCode;
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();

            // 
            News = reader.ReadString(Constants.NEWS_MAXLEN);
            EndOfList = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(StatusCode);

            // 
            writer.Write(News, Constants.NEWS_MAXLEN);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);

        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"News: {News} " +
                $"EndOfList: {EndOfList} ";
        }
    }
}
