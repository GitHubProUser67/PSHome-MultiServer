using System.IO;
namespace Horizon.RT.Models
{

    /*
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.UnkRequestTwistedMetalX)]
    public class MediusUnkTMRequest1 : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.UnkRequestTwistedMetalX;

        public MessageId MessageID { get; set; }
        public long Unk1;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            Unk1 = reader.ReadUInt32();

        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            //writer.Write(MessageID ?? MessageId.Empty);

            writer.Write(Unk1);

        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"Unk1: {Unk1}";
                //$"MessageID:{MessageID} ";
        }
    }
    */
}
