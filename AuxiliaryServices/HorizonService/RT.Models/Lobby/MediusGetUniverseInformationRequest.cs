using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Request information about a universe
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.GetUniverseInformation)]
    public class MediusGetUniverseInformationRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.GetUniverseInformation;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Bitfield to determine the type of information to retrieve
        /// </summary>
        public MediusUniverseVariableInformationInfoFilter InfoType;
        /// <summary>
        /// Character encoding: ISO-8859-1 or UTF-8
        /// </summary>
        public MediusCharacterEncodingType CharacterEncoding;
        /// <summary>
        /// Language Setting
        /// </summary>
        public MediusLanguageType Language;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            InfoType = reader.Read<MediusUniverseVariableInformationInfoFilter>();
            CharacterEncoding = reader.Read<MediusCharacterEncodingType>();
            Language = reader.Read<MediusLanguageType>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(InfoType);
            writer.Write(CharacterEncoding);
            writer.Write(Language);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"InfoType: {System.Convert.ToInt32(InfoType)}:{InfoType} " +
                $"CharacterEncoding: {CharacterEncoding} " +
                $"Language: {Language}";
        }
    }
}
