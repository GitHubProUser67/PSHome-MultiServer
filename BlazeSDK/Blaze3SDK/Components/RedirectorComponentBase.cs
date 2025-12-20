using Blaze3SDK.Blaze.Redirector;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class RedirectorComponentBase
    {
        public const ushort Id = 5;
        public const string Name = "RedirectorComponent";
        
        public class Server : BlazeServerComponent<RedirectorComponentCommand, RedirectorComponentNotification, Blaze3RpcError>
        {
            public Server() : base(RedirectorComponentBase.Id, RedirectorComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.getServerInstance)]
            public virtual Task<ServerInstanceInfo> GetServerInstanceAsync(ServerInstanceRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RedirectorComponentNotification notification) => RedirectorComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<RedirectorComponentCommand, RedirectorComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(RedirectorComponentBase.Id, RedirectorComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public ServerInstanceInfo GetServerInstance(ServerInstanceRequest request)
            {
                return Connection.SendRequest<ServerInstanceRequest, ServerInstanceInfo, ServerInstanceError>(this, (ushort)RedirectorComponentCommand.getServerInstance, request);
            }
            public Task<ServerInstanceInfo> GetServerInstanceAsync(ServerInstanceRequest request)
            {
                return Connection.SendRequestAsync<ServerInstanceRequest, ServerInstanceInfo, ServerInstanceError>(this, (ushort)RedirectorComponentCommand.getServerInstance, request);
            }
            
            
            public override Type GetCommandRequestType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RedirectorComponentNotification notification) => RedirectorComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<RedirectorComponentCommand, RedirectorComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(RedirectorComponentBase.Id, RedirectorComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.getServerInstance)]
            public virtual Task<ServerInstanceInfo> GetServerInstanceAsync(ServerInstanceRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ServerInstanceRequest, ServerInstanceInfo, ServerInstanceError>(this, (ushort)RedirectorComponentCommand.getServerInstance, request);
            }
            
            
            public override Type GetCommandRequestType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RedirectorComponentNotification notification) => RedirectorComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(RedirectorComponentCommand command) => command switch
        {
            RedirectorComponentCommand.getServerInstance => typeof(ServerInstanceRequest),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(RedirectorComponentCommand command) => command switch
        {
            RedirectorComponentCommand.getServerInstance => typeof(ServerInstanceInfo),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(RedirectorComponentCommand command) => command switch
        {
            RedirectorComponentCommand.getServerInstance => typeof(ServerInstanceError),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(RedirectorComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum RedirectorComponentCommand : ushort
        {
            getServerInstance = 1,
        }
        
        public enum RedirectorComponentNotification : ushort
        {
        }
        
    }
}
