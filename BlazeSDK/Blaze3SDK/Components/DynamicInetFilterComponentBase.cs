using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class DynamicInetFilterComponentBase
    {
        public const ushort Id = 2000;
        public const string Name = "DynamicInetFilterComponent";
        
        public class Server : BlazeServerComponent<DynamicInetFilterComponentCommand, DynamicInetFilterComponentNotification, Blaze3RpcError>
        {
            public Server() : base(DynamicInetFilterComponentBase.Id, DynamicInetFilterComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)DynamicInetFilterComponentCommand.add)]
            public virtual Task<NullStruct> AddAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)DynamicInetFilterComponentCommand.update)]
            public virtual Task<NullStruct> UpdateAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)DynamicInetFilterComponentCommand.remove)]
            public virtual Task<NullStruct> RemoveAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)DynamicInetFilterComponentCommand.listInfo)]
            public virtual Task<NullStruct> ListInfoAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(DynamicInetFilterComponentCommand command) => DynamicInetFilterComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(DynamicInetFilterComponentCommand command) => DynamicInetFilterComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(DynamicInetFilterComponentCommand command) => DynamicInetFilterComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(DynamicInetFilterComponentNotification notification) => DynamicInetFilterComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<DynamicInetFilterComponentCommand, DynamicInetFilterComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(DynamicInetFilterComponentBase.Id, DynamicInetFilterComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct Add()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.add, new NullStruct());
            }
            public Task<NullStruct> AddAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.add, new NullStruct());
            }
            
            public NullStruct Update()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.update, new NullStruct());
            }
            public Task<NullStruct> UpdateAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.update, new NullStruct());
            }
            
            public NullStruct Remove()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.remove, new NullStruct());
            }
            public Task<NullStruct> RemoveAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.remove, new NullStruct());
            }
            
            public NullStruct ListInfo()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.listInfo, new NullStruct());
            }
            public Task<NullStruct> ListInfoAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.listInfo, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(DynamicInetFilterComponentCommand command) => DynamicInetFilterComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(DynamicInetFilterComponentCommand command) => DynamicInetFilterComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(DynamicInetFilterComponentCommand command) => DynamicInetFilterComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(DynamicInetFilterComponentNotification notification) => DynamicInetFilterComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<DynamicInetFilterComponentCommand, DynamicInetFilterComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(DynamicInetFilterComponentBase.Id, DynamicInetFilterComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)DynamicInetFilterComponentCommand.add)]
            public virtual Task<NullStruct> AddAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.add, request);
            }
            
            [BlazeCommand((ushort)DynamicInetFilterComponentCommand.update)]
            public virtual Task<NullStruct> UpdateAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.update, request);
            }
            
            [BlazeCommand((ushort)DynamicInetFilterComponentCommand.remove)]
            public virtual Task<NullStruct> RemoveAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.remove, request);
            }
            
            [BlazeCommand((ushort)DynamicInetFilterComponentCommand.listInfo)]
            public virtual Task<NullStruct> ListInfoAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)DynamicInetFilterComponentCommand.listInfo, request);
            }
            
            
            public override Type GetCommandRequestType(DynamicInetFilterComponentCommand command) => DynamicInetFilterComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(DynamicInetFilterComponentCommand command) => DynamicInetFilterComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(DynamicInetFilterComponentCommand command) => DynamicInetFilterComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(DynamicInetFilterComponentNotification notification) => DynamicInetFilterComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(DynamicInetFilterComponentCommand command) => command switch
        {
            DynamicInetFilterComponentCommand.add => typeof(NullStruct),
            DynamicInetFilterComponentCommand.update => typeof(NullStruct),
            DynamicInetFilterComponentCommand.remove => typeof(NullStruct),
            DynamicInetFilterComponentCommand.listInfo => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(DynamicInetFilterComponentCommand command) => command switch
        {
            DynamicInetFilterComponentCommand.add => typeof(NullStruct),
            DynamicInetFilterComponentCommand.update => typeof(NullStruct),
            DynamicInetFilterComponentCommand.remove => typeof(NullStruct),
            DynamicInetFilterComponentCommand.listInfo => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(DynamicInetFilterComponentCommand command) => command switch
        {
            DynamicInetFilterComponentCommand.add => typeof(NullStruct),
            DynamicInetFilterComponentCommand.update => typeof(NullStruct),
            DynamicInetFilterComponentCommand.remove => typeof(NullStruct),
            DynamicInetFilterComponentCommand.listInfo => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(DynamicInetFilterComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum DynamicInetFilterComponentCommand : ushort
        {
            add = 1,
            update = 2,
            remove = 3,
            listInfo = 4,
        }
        
        public enum DynamicInetFilterComponentNotification : ushort
        {
        }
        
    }
}
