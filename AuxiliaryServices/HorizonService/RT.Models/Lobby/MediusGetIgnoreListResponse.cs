using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Collections.Generic;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Introduced in Medius 1.42
    /// </summary>
	[MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetIgnoreListResponse)]
    public class MediusGetIgnoreListResponse : BaseLobbyMessage, IMediusResponse
    {
        public class MediusGetIgnoreListResponseItem
        {
            public int IgnoreAccountID;
            public string IgnoreAccountName; // ACCOUNTNAME_MAXLEN
            public MediusPlayerStatus PlayerStatus;
        }

        public override byte PacketType => (byte)MediusLobbyMessageIds.GetIgnoreListResponse;

        public bool IsSuccess => StatusCode >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCode;
        public int IgnoreAccountID;
        public string IgnoreAccountName; // ACCOUNTNAME_MAXLEN
        public MediusPlayerStatus PlayerStatus;
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
            IgnoreAccountID = reader.ReadInt32();
            IgnoreAccountName = reader.ReadString(Constants.ACCOUNTNAME_MAXLEN);
            PlayerStatus = reader.Read<MediusPlayerStatus>();
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
            writer.Write(IgnoreAccountID);
            writer.Write(IgnoreAccountName, Constants.ACCOUNTNAME_MAXLEN);
            writer.Write(PlayerStatus);
            writer.Write(EndOfList);
            writer.Write(new byte[3]);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID:{MessageID} " +
                $"StatusCode:{StatusCode} " +
                $"IgnoreAccountID:{IgnoreAccountID} " +
                $"IgnoreAccountName:{IgnoreAccountName} " +
                $"PlayerStatus:{PlayerStatus} " +
                $"EndOfList:{EndOfList}";
        }

        public static List<MediusGetIgnoreListResponse> FromCollection(MessageId messageId, IEnumerable<MediusGetIgnoreListResponseItem> items)
        {
            List<MediusGetIgnoreListResponse> ignoreList = new List<MediusGetIgnoreListResponse>();

            foreach (var item in items)
            {
                ignoreList.Add(new MediusGetIgnoreListResponse()
                {
                    MessageID = messageId,
                    StatusCode = MediusCallbackStatus.MediusSuccess,
                    IgnoreAccountID = item.IgnoreAccountID,
                    IgnoreAccountName = item.IgnoreAccountName,
                    PlayerStatus = item.PlayerStatus
                });
            }

            // Set end of list
            ignoreList[ignoreList.Count - 1].EndOfList = true;

            return ignoreList;
        }
    }
}
