using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FileUploadServerReq)]
    public class MediusFileUploadServerRequest : BaseLobbyMessage, IMediusRequest
    {

        public override byte PacketType => (byte)MediusLobbyMessageIds.FileUploadServerReq;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }

        public int iReqStartByteIndex;
        public int iPacketNumber;
        public MediusFileXferStatus iXferStatus;
        public MediusCallbackStatus StatusCode;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            iReqStartByteIndex = reader.ReadInt32();
            iPacketNumber = reader.ReadInt32();
            iXferStatus = reader.Read<MediusFileXferStatus>();
            StatusCode = reader.Read<MediusCallbackStatus>();

            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(iReqStartByteIndex);
            writer.Write(iPacketNumber);
            writer.Write(iXferStatus);
            writer.Write(StatusCode);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"iReqStartByteIndex: {iReqStartByteIndex} " +
                $"iPacketNumber: {iPacketNumber} " +
                $"iXferStatus: {iXferStatus} " +
                $"StatusCode: {StatusCode}";
        }
    }
}
