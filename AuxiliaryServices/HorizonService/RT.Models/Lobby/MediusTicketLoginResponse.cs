using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.TicketLoginResponse)]
    public class MediusTicketLoginResponse : BaseLobbyExtMessage, IMediusResponse
    {

        public override byte PacketType => (byte)MediusLobbyExtMessageIds.TicketLoginResponse;

        public bool IsSuccess => StatusCodeTicketLogin >= 0;

        public MessageId MessageID { get; set; }

        public MediusCallbackStatus StatusCodeTicketLogin;
        public MediusPasswordType PasswordType;

        //Wrapped Account Login
        public MessageId MessageID2 { get; set; }
        public MediusCallbackStatus StatusCodeAccountLogin;
        public int AccountID;
        public MediusAccountType AccountType;
        public int MediusWorldID;
        public NetConnectionInfo ConnectInfo;

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            reader.ReadBytes(3);
            StatusCodeTicketLogin = reader.Read<MediusCallbackStatus>();
            PasswordType = reader.Read<MediusPasswordType>();

            //AccountLoginResponse Wrapped
            MessageID2 = reader.Read<MessageId>();
            StatusCodeAccountLogin = reader.Read<MediusCallbackStatus>();
            AccountID = reader.ReadInt32();
            AccountType = reader.Read<MediusAccountType>();
            MediusWorldID = reader.ReadInt32();
            ConnectInfo = reader.Read<NetConnectionInfo>();
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(new byte[3]);
            writer.Write(StatusCodeTicketLogin);
            writer.Write(PasswordType);

            //AccountLoginResponse Wrapped
            writer.Write(MessageID2);
            writer.Write(StatusCodeAccountLogin);
            writer.Write(AccountID);
            writer.Write(AccountType);
            writer.Write(MediusWorldID);
            writer.Write(ConnectInfo);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                    $"MessageIDTicketLogin: {MessageID} " +
                    $"StatusCodeTicketLogin: {StatusCodeTicketLogin} " +
                    $"PasswordType: {PasswordType} " +
                    $"MessageIDAccountLogin: {MessageID2} " +
                    $"StatusCodeAccountLogin: {StatusCodeAccountLogin} " +
                    $"AccountID: {AccountID} " +
                    $"AccountType: {AccountType} " +
                    $"MediusWorldID: {MediusWorldID} " +
                    $"ConnectInfo: {ConnectInfo}";
        }
    }
}
