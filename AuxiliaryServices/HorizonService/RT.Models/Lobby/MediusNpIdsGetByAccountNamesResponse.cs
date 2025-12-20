using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.MediusNpIdsGetByAccountNamesResponse)]
    public class MediusNpIdsGetByAccountNamesResponse : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.MediusNpIdsGetByAccountNamesResponse;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public string AccountName; //ACCOUNTNAME_MAXLEN
        public string SceNpId; //SCE_NPID_MAXLEN

        public byte[] data;
        public byte term;
        public byte[] dummy;

        public byte[] opt;
        public byte[] reserved;

        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            StatusCode = reader.Read<MediusCallbackStatus>();

            // 
            AccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            //SceNpId = reader.ReadString(Constants.SCE_NPID_MAXLEN);

            data = reader.ReadBytes(16);
            term = reader.ReadByte();
            dummy = reader.ReadBytes(3);

            //
            opt = reader.ReadBytes(8);
            reserved = reader.ReadBytes(8);

            EndOfList = reader.ReadBoolean();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(StatusCode);
            writer.Write(AccountName);
            //writer.Write(SceNpId);

            writer.Write(data);
            writer.Write(term);
            writer.Write(dummy);

            //
            writer.Write(opt);
            writer.Write(reserved);

            writer.Write(EndOfList);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"AccountName: {AccountName} " +
                $"SceNpId: {SceNpId} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
