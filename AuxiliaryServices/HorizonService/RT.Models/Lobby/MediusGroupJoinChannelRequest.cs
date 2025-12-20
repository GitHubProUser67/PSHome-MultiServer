using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.GroupJoinChannelRequest)]
    public class MediusGroupJoinChannelRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.GroupJoinChannelRequest;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public int MediusLobbyWorldID;
        public int MediusPartyWorldID;
        public string LobbyPassword;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);

            //
            MediusLobbyWorldID = reader.ReadInt32();
            MediusPartyWorldID = reader.ReadInt32();
            LobbyPassword = reader.ReadString(Constants.LOBBYPASSWORD_MAXLEN);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[2]);

            //
            writer.Write(MediusLobbyWorldID);
            writer.Write(MediusPartyWorldID);
            writer.Write(LobbyPassword);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey}" +
                $"MediusLobbyWorldID: {MediusLobbyWorldID} " +
                $"MediusPartyWorldID: {MediusPartyWorldID} " +
                $"LobbyPassword: {LobbyPassword}";
        }
    }
}
