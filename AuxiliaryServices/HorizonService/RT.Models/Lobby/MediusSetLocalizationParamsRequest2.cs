using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.SetLocalizationParamsRequest2)]
    public class MediusSetLocalizationParamsRequest2 : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.SetLocalizationParamsRequest2;

        public MessageId MessageID { get; set; }

        public string SessionKey; // SESSIONKEY_MAXLEN
        public MediusCharacterEncodingType CharacterEncoding;
        public MediusLanguageType Language;
        public MediusTimeZone TimeZone;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(2);
            CharacterEncoding = reader.Read<MediusCharacterEncodingType>();
            Language = reader.Read<MediusLanguageType>();
            TimeZone = reader.Read<MediusTimeZone>();
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
            writer.Write(CharacterEncoding);
            writer.Write(Language);
            writer.Write(TimeZone);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                $"CharacterEncoding: {CharacterEncoding} " +
                $"Language: {Language} " +
                $"TimeZone: {TimeZone}";
        }
    }
}
