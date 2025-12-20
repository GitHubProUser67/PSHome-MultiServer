using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.MatchCreateGameResponse)]
    public class MediusMatchCreateGameResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.MatchCreateGameResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int MediusWorldID;
        public int SystemSpecificStatusCode;
        public string RequestData; // REQUESTDATA_MAXLEN
        public int ApplicationDataSize;
        public string ApplicationData;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            StatusCode = reader.Read<MediusCallbackStatus>();
            MediusWorldID = reader.ReadInt32();
            SystemSpecificStatusCode = reader.ReadInt32();
            RequestData = reader.ReadString(Constants.REQUESTDATA_MAXLEN);
            ApplicationDataSize = reader.ReadInt32();
            ApplicationData = reader.ReadString(ApplicationDataSize);

        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);
            //writer.Write(new byte[3]);

            writer.Write(StatusCode);
            writer.Write(MediusWorldID);
            writer.Write(SystemSpecificStatusCode);
            writer.Write(RequestData, Constants.REQUESTDATA_MAXLEN);
            writer.Write(ApplicationDataSize);
            writer.Write(ApplicationData, ApplicationDataSize);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"SystemSpecificStatusCode: {SystemSpecificStatusCode} " +
                $"RequestData: {RequestData} " +
                $"ApplicationDataSize: {ApplicationDataSize} " +
                $"ApplicationData: {ApplicationData}";
        }
    }
}
