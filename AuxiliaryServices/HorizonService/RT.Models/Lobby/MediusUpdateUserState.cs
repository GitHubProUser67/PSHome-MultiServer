using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Update State Level of this session
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.UpdateUserState)]
    public class MediusUpdateUserState : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.UpdateUserState;

        public MessageId MessageID { get; set; }

        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// Change to in chat channel, left game, or left party
        /// </summary>
        public MediusUserAction UserAction;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(3);
            UserAction = reader.Read<MediusUserAction>();
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[3]);
            writer.Write(UserAction);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
             $"SessionKey: {SessionKey} " +
             $"UserAction: {UserAction}";
        }
    }
}
