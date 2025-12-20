using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class MailComponentBase
    {
        public const ushort Id = 14;
        public const string Name = "MailComponent";
        
        public class Server : BlazeServerComponent<MailComponentCommand, MailComponentNotification, Blaze2RpcError>
        {
            public Server() : base(MailComponentBase.Id, MailComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)MailComponentCommand.updateMailSettings)]
            public virtual Task<NullStruct> UpdateMailSettingsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)MailComponentCommand.sendMailToSelf)]
            public virtual Task<NullStruct> SendMailToSelfAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(MailComponentCommand command) => MailComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(MailComponentCommand command) => MailComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(MailComponentCommand command) => MailComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(MailComponentNotification notification) => MailComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<MailComponentCommand, MailComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(MailComponentBase.Id, MailComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct UpdateMailSettings()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.updateMailSettings, new NullStruct());
            }
            public Task<NullStruct> UpdateMailSettingsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.updateMailSettings, new NullStruct());
            }
            
            public NullStruct SendMailToSelf()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.sendMailToSelf, new NullStruct());
            }
            public Task<NullStruct> SendMailToSelfAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.sendMailToSelf, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(MailComponentCommand command) => MailComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(MailComponentCommand command) => MailComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(MailComponentCommand command) => MailComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(MailComponentNotification notification) => MailComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<MailComponentCommand, MailComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(MailComponentBase.Id, MailComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)MailComponentCommand.updateMailSettings)]
            public virtual Task<NullStruct> UpdateMailSettingsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.updateMailSettings, request);
            }
            
            [BlazeCommand((ushort)MailComponentCommand.sendMailToSelf)]
            public virtual Task<NullStruct> SendMailToSelfAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.sendMailToSelf, request);
            }
            
            
            public override Type GetCommandRequestType(MailComponentCommand command) => MailComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(MailComponentCommand command) => MailComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(MailComponentCommand command) => MailComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(MailComponentNotification notification) => MailComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(MailComponentCommand command) => command switch
        {
            MailComponentCommand.updateMailSettings => typeof(NullStruct),
            MailComponentCommand.sendMailToSelf => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(MailComponentCommand command) => command switch
        {
            MailComponentCommand.updateMailSettings => typeof(NullStruct),
            MailComponentCommand.sendMailToSelf => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(MailComponentCommand command) => command switch
        {
            MailComponentCommand.updateMailSettings => typeof(NullStruct),
            MailComponentCommand.sendMailToSelf => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(MailComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum MailComponentCommand : ushort
        {
            updateMailSettings = 1,
            sendMailToSelf = 2,
        }
        
        public enum MailComponentNotification : ushort
        {
        }
        
    }
}
