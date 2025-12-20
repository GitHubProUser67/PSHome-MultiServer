using Blaze3SDK.Blaze.Messaging;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class MessagingComponentBase
    {
        public const ushort Id = 15;
        public const string Name = "MessagingComponent";
        
        public class Server : BlazeServerComponent<MessagingComponentCommand, MessagingComponentNotification, Blaze3RpcError>
        {
            public Server() : base(MessagingComponentBase.Id, MessagingComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)MessagingComponentCommand.sendMessage)]
            public virtual Task<NullStruct> SendMessageAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)MessagingComponentCommand.fetchMessages)]
            public virtual Task<NullStruct> FetchMessagesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)MessagingComponentCommand.purgeMessages)]
            public virtual Task<NullStruct> PurgeMessagesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)MessagingComponentCommand.touchMessages)]
            public virtual Task<NullStruct> TouchMessagesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)MessagingComponentCommand.getMessages)]
            public virtual Task<NullStruct> GetMessagesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyMessageAsync(BlazeServerConnection connection, ServerMessage notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(MessagingComponentBase.Id, (ushort)MessagingComponentNotification.NotifyMessage, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(MessagingComponentCommand command) => MessagingComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(MessagingComponentCommand command) => MessagingComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(MessagingComponentCommand command) => MessagingComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(MessagingComponentNotification notification) => MessagingComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<MessagingComponentCommand, MessagingComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(MessagingComponentBase.Id, MessagingComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct SendMessage()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.sendMessage, new NullStruct());
            }
            public Task<NullStruct> SendMessageAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.sendMessage, new NullStruct());
            }
            
            public NullStruct FetchMessages()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.fetchMessages, new NullStruct());
            }
            public Task<NullStruct> FetchMessagesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.fetchMessages, new NullStruct());
            }
            
            public NullStruct PurgeMessages()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.purgeMessages, new NullStruct());
            }
            public Task<NullStruct> PurgeMessagesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.purgeMessages, new NullStruct());
            }
            
            public NullStruct TouchMessages()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.touchMessages, new NullStruct());
            }
            public Task<NullStruct> TouchMessagesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.touchMessages, new NullStruct());
            }
            
            public NullStruct GetMessages()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.getMessages, new NullStruct());
            }
            public Task<NullStruct> GetMessagesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.getMessages, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)MessagingComponentNotification.NotifyMessage)]
            public virtual Task OnNotifyMessageAsync(ServerMessage notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyMessageAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(MessagingComponentCommand command) => MessagingComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(MessagingComponentCommand command) => MessagingComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(MessagingComponentCommand command) => MessagingComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(MessagingComponentNotification notification) => MessagingComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<MessagingComponentCommand, MessagingComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(MessagingComponentBase.Id, MessagingComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)MessagingComponentCommand.sendMessage)]
            public virtual Task<NullStruct> SendMessageAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.sendMessage, request);
            }
            
            [BlazeCommand((ushort)MessagingComponentCommand.fetchMessages)]
            public virtual Task<NullStruct> FetchMessagesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.fetchMessages, request);
            }
            
            [BlazeCommand((ushort)MessagingComponentCommand.purgeMessages)]
            public virtual Task<NullStruct> PurgeMessagesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.purgeMessages, request);
            }
            
            [BlazeCommand((ushort)MessagingComponentCommand.touchMessages)]
            public virtual Task<NullStruct> TouchMessagesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.touchMessages, request);
            }
            
            [BlazeCommand((ushort)MessagingComponentCommand.getMessages)]
            public virtual Task<NullStruct> GetMessagesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)MessagingComponentCommand.getMessages, request);
            }
            
            
            [BlazeNotification((ushort)MessagingComponentNotification.NotifyMessage)]
            public virtual Task<ServerMessage> OnNotifyMessageAsync(ServerMessage notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(MessagingComponentCommand command) => MessagingComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(MessagingComponentCommand command) => MessagingComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(MessagingComponentCommand command) => MessagingComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(MessagingComponentNotification notification) => MessagingComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(MessagingComponentCommand command) => command switch
        {
            MessagingComponentCommand.sendMessage => typeof(NullStruct),
            MessagingComponentCommand.fetchMessages => typeof(NullStruct),
            MessagingComponentCommand.purgeMessages => typeof(NullStruct),
            MessagingComponentCommand.touchMessages => typeof(NullStruct),
            MessagingComponentCommand.getMessages => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(MessagingComponentCommand command) => command switch
        {
            MessagingComponentCommand.sendMessage => typeof(NullStruct),
            MessagingComponentCommand.fetchMessages => typeof(NullStruct),
            MessagingComponentCommand.purgeMessages => typeof(NullStruct),
            MessagingComponentCommand.touchMessages => typeof(NullStruct),
            MessagingComponentCommand.getMessages => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(MessagingComponentCommand command) => command switch
        {
            MessagingComponentCommand.sendMessage => typeof(NullStruct),
            MessagingComponentCommand.fetchMessages => typeof(NullStruct),
            MessagingComponentCommand.purgeMessages => typeof(NullStruct),
            MessagingComponentCommand.touchMessages => typeof(NullStruct),
            MessagingComponentCommand.getMessages => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(MessagingComponentNotification notification) => notification switch
        {
            MessagingComponentNotification.NotifyMessage => typeof(ServerMessage),
            _ => typeof(NullStruct)
        };
        
        public enum MessagingComponentCommand : ushort
        {
            sendMessage = 1,
            fetchMessages = 2,
            purgeMessages = 3,
            touchMessages = 4,
            getMessages = 5,
        }
        
        public enum MessagingComponentNotification : ushort
        {
            NotifyMessage = 1,
        }
        
    }
}
