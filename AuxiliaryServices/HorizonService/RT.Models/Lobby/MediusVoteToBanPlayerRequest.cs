using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Request structure to add/remove a vote to ban another player
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.VoteToBanPlayer)]
    public class MediusVoteToBanPlayerRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.VoteToBanPlayer;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Vote addor remove
        /// </summary>
        public MediusVoteActionType VoteAction;
        /// <summary>
        /// Reason for vote: vulgarity, cheating, other.
        /// </summary>
        public MediusBanReasonType BanReason;
        /// <summary>
        /// Medius ID of game world to ban player from.
        /// </summary>
        public int MediusWorldID;
        /// <summary>
        /// DME Client index of player to vote off.
        /// </summary>
        public int DmeClientIndex;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            VoteAction = reader.Read<MediusVoteActionType>();
            BanReason = reader.Read<MediusBanReasonType>();
            MediusWorldID = reader.ReadInt32();
            DmeClientIndex = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(VoteAction);
            writer.Write(BanReason);
            writer.Write(MediusWorldID);
            writer.Write(DmeClientIndex);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"VoteAction: {VoteAction} " +
                $"BanReason: {BanReason} " +
                $"MediusWorldID: {MediusWorldID} " +
                $"DmeClientIndex: {DmeClientIndex}";
        }
    }
}
