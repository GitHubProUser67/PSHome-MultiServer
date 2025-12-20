using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Applies a lobby list filter to this session
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.SetLobbyWorldFilter1)]
    public class MediusSetLobbyWorldFilterRequest1 : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.SetLobbyWorldFilter1;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Filter Mask 1
        /// </summary>
        public uint FilterMask1;
        /// <summary>
        /// Filter Mask 2
        /// </summary>
        public uint FilterMask2;
        /// <summary>
        /// Filter Mask 3
        /// </summary>
        public uint FilterMask3;
        /// <summary>
        /// Filter Mask 4
        /// </summary>
        public uint FilterMask4;
        /// <summary>
        /// Filter type = AND mask = mask; AND mask = lobby field
        /// </summary>
        public MediusLobbyFilterType LobbyFilterType;
        /// <summary>
        /// Filter level, must correspond to the lobby world's filter level
        /// </summary>
        public MediusLobbyFilterMaskLevelType FilterMaskLevel;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            FilterMask1 = reader.ReadUInt32();
            FilterMask2 = reader.ReadUInt32();
            FilterMask3 = reader.ReadUInt32();
            FilterMask4 = reader.ReadUInt32();
            LobbyFilterType = reader.Read<MediusLobbyFilterType>();
            FilterMaskLevel = reader.Read<MediusLobbyFilterMaskLevelType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            //
            writer.Write(new byte[3]);
            writer.Write(FilterMask1);
            writer.Write(FilterMask2);
            writer.Write(FilterMask3);
            writer.Write(FilterMask4);
            writer.Write(LobbyFilterType);
            writer.Write(FilterMaskLevel);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"FilterMask1: {FilterMask1} " +
                $"FilterMask2: {FilterMask2} " +
                $"FilterMask3: {FilterMask3} " +
                $"FilterMask4: {FilterMask4} " +
                $"LobbyFilterType: {LobbyFilterType} " +
                $"FilterMaskLevel: {FilterMaskLevel}";
        }
    }
}
