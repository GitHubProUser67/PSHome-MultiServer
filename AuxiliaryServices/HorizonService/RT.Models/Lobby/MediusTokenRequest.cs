using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Request structure to add/update/remove MediusToken
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.TokenRequest)]
    public class MediusTokenRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.TokenRequest;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Token action to take: Add, update, clear.
        /// </summary>
        public MediusTokenActionType TokenAction;
        /// <summary>
        /// Token category.
        /// </summary>
        public MediusTokenCategoryType TokenCategory;
        /// <summary>
        /// Entity ID of the token.
        /// </summary>
        public uint EntityID;
        /// <summary>
        /// Token to replace.
        /// </summary>
        public string TokenToReplace;
        /// <summary>
        /// New token to replace with, or create.
        /// </summary>
        public string Token;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3); // padding
            TokenAction = reader.Read<MediusTokenActionType>();
            TokenCategory = reader.Read<MediusTokenCategoryType>();
            EntityID = reader.ReadUInt32();
            TokenToReplace = reader.ReadString(Constants.MEDIUS_TOKEN_MAXSIZE);
            Token = reader.ReadString(Constants.MEDIUS_TOKEN_MAXSIZE);
            reader.ReadBytes(3); // padding
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
            writer.Write(TokenAction);
            writer.Write(TokenCategory);
            writer.Write(EntityID);
            writer.Write(TokenToReplace, Constants.MEDIUS_TOKEN_MAXSIZE);
            writer.Write(Token, Constants.MEDIUS_TOKEN_MAXSIZE);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"TokenAction: {TokenAction} " + 
                $"TokenCategory: {TokenCategory}" +
                $"EntityID: {EntityID} " +
                $"TokenToReplace: {string.Join("", TokenToReplace)} " +
                $"Token: {string.Join("", Token)}";
        }
    }
}
