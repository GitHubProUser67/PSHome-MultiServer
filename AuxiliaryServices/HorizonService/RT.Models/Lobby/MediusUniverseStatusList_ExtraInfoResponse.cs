using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.UniverseStatusList_ExtraInfoResponse)]
    public class MediusUniverseStatusList_ExtraInfoResponse : BaseLobbyExtMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.UniverseStatusList_ExtraInfoResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public string UniverseName;
        public string DNS;
        public int Port;
        public string UniverseDescription;
        public int Status;
        public int UserCount;
        public int MaxUsers;
        public string UniverseBilling;
        public string BillingSystemName;
        public bool EndOfList;
        public string ExtendedInfo;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            //
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            UniverseName = reader.ReadString(Constants.UNIVERSENAME_MAXLEN);
            DNS = reader.ReadString(Constants.UNIVERSEDNS_MAXLEN);
            Port = reader.ReadInt32();
            UniverseDescription = reader.ReadString(Constants.UNIVERSEDESCRIPTION_MAXLEN);
            Status = reader.ReadInt32();
            UserCount = reader.ReadInt32();
            MaxUsers = reader.ReadInt32();
            UniverseBilling = reader.ReadString(Constants.UNIVERSE_BSP_MAXLEN);
            BillingSystemName = reader.ReadString(Constants.UNIVERSE_BSP_NAME_MAXLEN);
            EndOfList = reader.ReadBoolean();
            ExtendedInfo = reader.ReadString(Constants.UNIVERSE_EXTENDED_INFO_MAXLEN);
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            //
            writer.Write(new byte[3]);
            writer.Write(StatusCode);
            writer.Write(UniverseName, Constants.UNIVERSENAME_MAXLEN);
            writer.Write(DNS, Constants.UNIVERSEDNS_MAXLEN);
            writer.Write(Port);
            writer.Write(UniverseDescription, Constants.UNIVERSEDESCRIPTION_MAXLEN);
            writer.Write(Status);
            writer.Write(UserCount);
            writer.Write(MaxUsers);
            writer.Write(UniverseBilling, Constants.UNIVERSE_BSP_MAXLEN);
            writer.Write(BillingSystemName, Constants.UNIVERSE_BSP_NAME_MAXLEN);
            writer.Write(EndOfList);
            writer.Write(ExtendedInfo, Constants.UNIVERSE_EXTENDED_INFO_MAXLEN);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"UniverseName: {UniverseName} " +
                $"DNS: {DNS} " +
                $"Port: {Port} " +
                $"UniverseDescription: {UniverseDescription} " +
                $"Status: {Status} " +
                $"UserCount: {UserCount} " +
                $"MaxUsers: {MaxUsers} " +
                $"UniverseBilling: {UniverseBilling} " +
                $"BillingSystemName: {BillingSystemName} " +
                $"EndOfList: {EndOfList}" +
                $"ExtendedInfo: {ExtendedInfo}";
        }
    }
}
