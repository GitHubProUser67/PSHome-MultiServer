using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.DnasSignaturePost)]
    public class MediusDnasSignaturePost : BaseLobbyExtMessage
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.DnasSignaturePost;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public MediusDnasCategory DnasSignatureType;
        public byte DnasSignatureLength;
        public byte[] DnasSignature = new byte[Constants.DNASSIGNATURE_MAXLEN];

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            DnasSignatureType = reader.Read<MediusDnasCategory>();
            DnasSignatureLength = reader.ReadByte();
            DnasSignature = reader.ReadBytes(Constants.DNASSIGNATURE_MAXLEN);
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);
            writer.Write(DnasSignatureType);
            writer.Write(DnasSignatureLength);
            writer.Write(DnasSignature, Constants.DNASSIGNATURE_MAXLEN);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"DnasSignatureType: {DnasSignatureType} " +
                $"SignatureLength: {DnasSignatureLength} " +
                $"DnasSignature: {System.BitConverter.ToString(DnasSignature)}";
        }
    }
}
