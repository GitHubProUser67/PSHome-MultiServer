using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.UtilAddLobbyWorld)]
    public class MediusUtilAddLobbyWorldRequest : BaseLobbyMessage, IMediusRequest
    {
        public override NetMessageClass PacketClass => NetMessageClass.MessageClassLobby;

        public override byte PacketType => (byte)MediusLobbyMessageIds.UtilAddLobbyWorld;

        public MessageId MessageID { get; set; }

        public int ApplicationID;
        public int MaxPlayers;
        public string LobbyName; // WORLDNAME_MAXLEN
        public string LobbyPassword; // PASSWORD_MAXLEN
        public MediusUtilTypeWorldPersistence Persistence;
        public int GenericField1;
        public int GenericField2;
        public int GenericField3;
        public int GenericField4;
        public MediusWorldGenericFieldLevelType GenericFieldLevelType;
        public int MABNotificationThreshold;
        public int MABNotificationStep;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);

            // 
            ApplicationID = reader.ReadInt32();
            LobbyName = reader.ReadString(Constants.WORLDNAME_MAXLEN);
            LobbyPassword = reader.ReadString(Constants.PASSWORD_MAXLEN);
            Persistence = reader.Read<MediusUtilTypeWorldPersistence>();
            GenericField1 = reader.ReadInt32();
            GenericField2 = reader.ReadInt32();
            GenericField3 = reader.ReadInt32();
            GenericField4 = reader.ReadInt32();
            GenericFieldLevelType = reader.Read<MediusWorldGenericFieldLevelType>();
            MABNotificationThreshold = reader.ReadInt32();
            MABNotificationStep = reader.ReadInt32();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(ApplicationID);
            writer.Write(LobbyName, Constants.WORLDNAME_MAXLEN);
            writer.Write(LobbyPassword, Constants.PASSWORD_MAXLEN);
            writer.Write(Persistence);
            writer.Write(GenericField1);
            writer.Write(GenericField2);
            writer.Write(GenericField3);
            writer.Write(GenericField4);
            writer.Write(GenericFieldLevelType);
            writer.Write(MABNotificationThreshold);
            writer.Write(MABNotificationStep);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"ApplicationID: {ApplicationID} " +
                $"LobbyName: {LobbyName} " +
                $"LobbyPassword: {LobbyPassword} " +
                $"Persistence: {Persistence} " +
                $"GenericField1: {GenericField1} " +
                $"GenericField2: {GenericField2} " +
                $"GenericField3: {GenericField3} " +
                $"GenericField4: {GenericField4} " +
                $"GenericFieldLevelType: {GenericFieldLevelType} " +
                $"MABNotificationThreshold: {MABNotificationThreshold} " +
                $"MABNotificationStep: {MABNotificationStep} ";
        }
    }
}
