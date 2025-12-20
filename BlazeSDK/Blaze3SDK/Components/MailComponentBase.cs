using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class MailComponentBase
    {
        public const ushort Id = 14;
        public const string Name = "MailComponent";
        
        public class Server : BlazeServerComponent<MailComponentCommand, MailComponentNotification, Blaze3RpcError>
        {
            public Server() : base(MailComponentBase.Id, MailComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)MailComponentCommand.updateMailSettings)]
            public virtual Task<NullStruct> UpdateMailSettingsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)MailComponentCommand.sendMailToSelf)]
            public virtual Task<NullStruct> SendMailToSelfAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)MailComponentCommand.setMailOptInFlags)]
            public virtual Task<NullStruct> SetMailOptInFlagsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)MailComponentCommand.setMailPref)]
            public virtual Task<NullStruct> SetMailPrefAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)MailComponentCommand.getMailSettings)]
            public virtual Task<NullStruct> GetMailSettingsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(MailComponentCommand command) => MailComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(MailComponentCommand command) => MailComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(MailComponentCommand command) => MailComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(MailComponentNotification notification) => MailComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<MailComponentCommand, MailComponentNotification, Blaze3RpcError>
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
            
            public NullStruct SetMailOptInFlags()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.setMailOptInFlags, new NullStruct());
            }
            public Task<NullStruct> SetMailOptInFlagsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.setMailOptInFlags, new NullStruct());
            }
            
            public NullStruct SetMailPref()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.setMailPref, new NullStruct());
            }
            public Task<NullStruct> SetMailPrefAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.setMailPref, new NullStruct());
            }
            
            public NullStruct GetMailSettings()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.getMailSettings, new NullStruct());
            }
            public Task<NullStruct> GetMailSettingsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.getMailSettings, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(MailComponentCommand command) => MailComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(MailComponentCommand command) => MailComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(MailComponentCommand command) => MailComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(MailComponentNotification notification) => MailComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<MailComponentCommand, MailComponentNotification, Blaze3RpcError>
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
            
            [BlazeCommand((ushort)MailComponentCommand.setMailOptInFlags)]
            public virtual Task<NullStruct> SetMailOptInFlagsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.setMailOptInFlags, request);
            }
            
            [BlazeCommand((ushort)MailComponentCommand.setMailPref)]
            public virtual Task<NullStruct> SetMailPrefAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.setMailPref, request);
            }
            
            [BlazeCommand((ushort)MailComponentCommand.getMailSettings)]
            public virtual Task<NullStruct> GetMailSettingsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MailComponentCommand.getMailSettings, request);
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
            MailComponentCommand.setMailOptInFlags => typeof(NullStruct),
            MailComponentCommand.setMailPref => typeof(NullStruct),
            MailComponentCommand.getMailSettings => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(MailComponentCommand command) => command switch
        {
            MailComponentCommand.updateMailSettings => typeof(NullStruct),
            MailComponentCommand.sendMailToSelf => typeof(NullStruct),
            MailComponentCommand.setMailOptInFlags => typeof(NullStruct),
            MailComponentCommand.setMailPref => typeof(NullStruct),
            MailComponentCommand.getMailSettings => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(MailComponentCommand command) => command switch
        {
            MailComponentCommand.updateMailSettings => typeof(NullStruct),
            MailComponentCommand.sendMailToSelf => typeof(NullStruct),
            MailComponentCommand.setMailOptInFlags => typeof(NullStruct),
            MailComponentCommand.setMailPref => typeof(NullStruct),
            MailComponentCommand.getMailSettings => typeof(NullStruct),
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
            setMailOptInFlags = 3,
            setMailPref = 4,
            getMailSettings = 5,
        }
        
        public enum MailComponentNotification : ushort
        {
        }
        
    }
}
