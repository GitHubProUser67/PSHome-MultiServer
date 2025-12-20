using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.LadderList_ExtraInfoResponse)]
    public class MediusLadderList_ExtraInfoResponse : BaseLobbyMessage, IMediusResponse
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.LadderList_ExtraInfoResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public uint LadderPosition;
        public int LadderStat;
        public int AccountID;
        public string AccountName; // ACCOUNTNAME_MAXLEN
        public byte[] AccountStats = new byte[Constants.ACCOUNTSTATS_MAXLEN];
        public MediusPlayerOnlineState OnlineState;
        public bool EndOfList;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            StatusCode = reader.Read<MediusCallbackStatus>();
            LadderPosition = reader.ReadUInt32();
            LadderStat = reader.ReadInt32();
            AccountID = reader.ReadInt32();
            AccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            AccountStats = reader.ReadBytes(Constants.ACCOUNTSTATS_MAXLEN);
            OnlineState = reader.Read<MediusPlayerOnlineState>();
            EndOfList = reader.ReadBoolean();
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
            writer.Write(LadderPosition);
            writer.Write(LadderStat);
            writer.Write(AccountID);
            writer.Write(AccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(AccountStats, Constants.ACCOUNTSTATS_MAXLEN);
            writer.Write(OnlineState);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"StatusCode: {StatusCode} " +
                $"LadderPosition: {LadderPosition} " +
                $"LadderStat: {LadderStat} " +
                $"AccountID: {AccountID} " +
                $"AccountName: {AccountName} " +
                $"AccountStats: {System.BitConverter.ToString(AccountStats)} " +
                $"OnlineState: {OnlineState} " +
                $"EndOfList: {EndOfList}";
        }
    }
}
