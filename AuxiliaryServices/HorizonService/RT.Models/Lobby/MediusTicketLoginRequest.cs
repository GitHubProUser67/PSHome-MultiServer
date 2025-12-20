using EndianTools;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;
using System;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.TicketLogin)]
    public class MediusTicketLoginRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.TicketLogin;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN

        public uint UNK0;
        public uint Version;
        public uint Size;
        public byte[] TicketData;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            UNK0 = reader.ReadUInt32();
            Version = EndianUtils.ReverseUint(reader.ReadUInt32());
            Size = EndianUtils.ReverseUint(reader.ReadUInt32());
            TicketData = reader.ReadBytes((int)Size);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(UNK0);
            writer.Write(EndianUtils.ReverseUint(Version));
            writer.Write(EndianUtils.ReverseUint(Size));
            writer.Write(TicketData ?? new byte[Size]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"SessionKey:{SessionKey} " +
                $"UNK0: {UNK0} " +
                $"TicketData: {(TicketData != null ? BitConverter.ToString(TicketData) : string.Empty)} ";
        }
    }
}
