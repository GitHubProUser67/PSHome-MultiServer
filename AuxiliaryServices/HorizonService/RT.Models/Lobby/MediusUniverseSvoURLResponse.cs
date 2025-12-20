using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.UniverseSvoURLResponse)]
    public class MediusUniverseSvoURLResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.UniverseSvoURLResponse;

        public bool IsSuccess => true;

        public MessageId MessageID { get; set; }

        public string URL { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            MessageID = reader.Read<MessageId>();

            // read URL
            if (reader.MediusVersion >= 109)
            {
                // 1 byte length prefixed url
                byte len = reader.ReadByte();
                URL = reader.ReadString(len + 1);
            }
            else
            {
                // fixed size url
                URL = reader.ReadString(128);
            }
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(MessageID ?? MessageId.Empty);

            if (writer.MediusVersion >= 109)
            {
                // 1 byte length prefixed url
                if (URL == null)
                {
                    writer.Write((byte)0);
                }
                else
                {
                    writer.Write((byte)(URL.Length + 1));
                    writer.Write(URL, URL.Length);
                }
            }
            else
            {
                // fixed size url
                writer.Write(URL, 128);
            }
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SVOURL: {URL}";
        }

    }
}
