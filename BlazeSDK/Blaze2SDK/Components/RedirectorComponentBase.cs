using Blaze2SDK.Blaze.Redirector;
using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class RedirectorComponentBase
    {
        public const ushort Id = 5;
        public const string Name = "RedirectorComponent";
        
        public class Server : BlazeServerComponent<RedirectorComponentCommand, RedirectorComponentNotification, Blaze2RpcError>
        {
            public Server() : base(RedirectorComponentBase.Id, RedirectorComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.getServerInstance)]
            public virtual Task<ServerInstanceInfo> GetServerInstanceAsync(ServerInstanceRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.getServerList)]
            public virtual Task<NullStruct> GetServerListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.scheduleServerDowntime)]
            public virtual Task<NullStruct> ScheduleServerDowntimeAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.cancelServerDowntime)]
            public virtual Task<NullStruct> CancelServerDowntimeAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.getDowntimeList)]
            public virtual Task<NullStruct> GetDowntimeListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.getDowntimeMessageTypes)]
            public virtual Task<NullStruct> GetDowntimeMessageTypesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RedirectorComponentNotification notification) => RedirectorComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<RedirectorComponentCommand, RedirectorComponentNotification, Blaze2RpcError>
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
            
            public NullStruct GetServerList()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.getServerList, new NullStruct());
            }
            public Task<NullStruct> GetServerListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.getServerList, new NullStruct());
            }
            
            public NullStruct ScheduleServerDowntime()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.scheduleServerDowntime, new NullStruct());
            }
            public Task<NullStruct> ScheduleServerDowntimeAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.scheduleServerDowntime, new NullStruct());
            }
            
            public NullStruct CancelServerDowntime()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.cancelServerDowntime, new NullStruct());
            }
            public Task<NullStruct> CancelServerDowntimeAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.cancelServerDowntime, new NullStruct());
            }
            
            public NullStruct GetDowntimeList()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.getDowntimeList, new NullStruct());
            }
            public Task<NullStruct> GetDowntimeListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.getDowntimeList, new NullStruct());
            }
            
            public NullStruct GetDowntimeMessageTypes()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.getDowntimeMessageTypes, new NullStruct());
            }
            public Task<NullStruct> GetDowntimeMessageTypesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.getDowntimeMessageTypes, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RedirectorComponentNotification notification) => RedirectorComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<RedirectorComponentCommand, RedirectorComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(RedirectorComponentBase.Id, RedirectorComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.getServerInstance)]
            public virtual Task<ServerInstanceInfo> GetServerInstanceAsync(ServerInstanceRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ServerInstanceRequest, ServerInstanceInfo, ServerInstanceError>(this, (ushort)RedirectorComponentCommand.getServerInstance, request);
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.getServerList)]
            public virtual Task<NullStruct> GetServerListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.getServerList, request);
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.scheduleServerDowntime)]
            public virtual Task<NullStruct> ScheduleServerDowntimeAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.scheduleServerDowntime, request);
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.cancelServerDowntime)]
            public virtual Task<NullStruct> CancelServerDowntimeAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.cancelServerDowntime, request);
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.getDowntimeList)]
            public virtual Task<NullStruct> GetDowntimeListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.getDowntimeList, request);
            }
            
            [BlazeCommand((ushort)RedirectorComponentCommand.getDowntimeMessageTypes)]
            public virtual Task<NullStruct> GetDowntimeMessageTypesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RedirectorComponentCommand.getDowntimeMessageTypes, request);
            }
            
            
            public override Type GetCommandRequestType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RedirectorComponentCommand command) => RedirectorComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RedirectorComponentNotification notification) => RedirectorComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(RedirectorComponentCommand command) => command switch
        {
            RedirectorComponentCommand.getServerInstance => typeof(ServerInstanceRequest),
            RedirectorComponentCommand.getServerList => typeof(NullStruct),
            RedirectorComponentCommand.scheduleServerDowntime => typeof(NullStruct),
            RedirectorComponentCommand.cancelServerDowntime => typeof(NullStruct),
            RedirectorComponentCommand.getDowntimeList => typeof(NullStruct),
            RedirectorComponentCommand.getDowntimeMessageTypes => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(RedirectorComponentCommand command) => command switch
        {
            RedirectorComponentCommand.getServerInstance => typeof(ServerInstanceInfo),
            RedirectorComponentCommand.getServerList => typeof(NullStruct),
            RedirectorComponentCommand.scheduleServerDowntime => typeof(NullStruct),
            RedirectorComponentCommand.cancelServerDowntime => typeof(NullStruct),
            RedirectorComponentCommand.getDowntimeList => typeof(NullStruct),
            RedirectorComponentCommand.getDowntimeMessageTypes => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(RedirectorComponentCommand command) => command switch
        {
            RedirectorComponentCommand.getServerInstance => typeof(ServerInstanceError),
            RedirectorComponentCommand.getServerList => typeof(NullStruct),
            RedirectorComponentCommand.scheduleServerDowntime => typeof(NullStruct),
            RedirectorComponentCommand.cancelServerDowntime => typeof(NullStruct),
            RedirectorComponentCommand.getDowntimeList => typeof(NullStruct),
            RedirectorComponentCommand.getDowntimeMessageTypes => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(RedirectorComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum RedirectorComponentCommand : ushort
        {
            getServerInstance = 1,
            getServerList = 2,
            scheduleServerDowntime = 3,
            cancelServerDowntime = 4,
            getDowntimeList = 5,
            getDowntimeMessageTypes = 6,
        }
        
        public enum RedirectorComponentNotification : ushort
        {
        }
        
    }
}
