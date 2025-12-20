using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class GpsContentControllerComponentBase
    {
        public const ushort Id = 27;
        public const string Name = "GpsContentControllerComponent";
        
        public class Server : BlazeServerComponent<GpsContentControllerComponentCommand, GpsContentControllerComponentNotification, Blaze2RpcError>
        {
            public Server() : base(GpsContentControllerComponentBase.Id, GpsContentControllerComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)GpsContentControllerComponentCommand.filePetition)]
            public virtual Task<NullStruct> FilePetitionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GpsContentControllerComponentCommand.fetchContent)]
            public virtual Task<NullStruct> FetchContentAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GpsContentControllerComponentCommand.showContent)]
            public virtual Task<NullStruct> ShowContentAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(GpsContentControllerComponentCommand command) => GpsContentControllerComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GpsContentControllerComponentCommand command) => GpsContentControllerComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GpsContentControllerComponentCommand command) => GpsContentControllerComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GpsContentControllerComponentNotification notification) => GpsContentControllerComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<GpsContentControllerComponentCommand, GpsContentControllerComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(GpsContentControllerComponentBase.Id, GpsContentControllerComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct FilePetition()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GpsContentControllerComponentCommand.filePetition, new NullStruct());
            }
            public Task<NullStruct> FilePetitionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GpsContentControllerComponentCommand.filePetition, new NullStruct());
            }
            
            public NullStruct FetchContent()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GpsContentControllerComponentCommand.fetchContent, new NullStruct());
            }
            public Task<NullStruct> FetchContentAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GpsContentControllerComponentCommand.fetchContent, new NullStruct());
            }
            
            public NullStruct ShowContent()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GpsContentControllerComponentCommand.showContent, new NullStruct());
            }
            public Task<NullStruct> ShowContentAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GpsContentControllerComponentCommand.showContent, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(GpsContentControllerComponentCommand command) => GpsContentControllerComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GpsContentControllerComponentCommand command) => GpsContentControllerComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GpsContentControllerComponentCommand command) => GpsContentControllerComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GpsContentControllerComponentNotification notification) => GpsContentControllerComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<GpsContentControllerComponentCommand, GpsContentControllerComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(GpsContentControllerComponentBase.Id, GpsContentControllerComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)GpsContentControllerComponentCommand.filePetition)]
            public virtual Task<NullStruct> FilePetitionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GpsContentControllerComponentCommand.filePetition, request);
            }
            
            [BlazeCommand((ushort)GpsContentControllerComponentCommand.fetchContent)]
            public virtual Task<NullStruct> FetchContentAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GpsContentControllerComponentCommand.fetchContent, request);
            }
            
            [BlazeCommand((ushort)GpsContentControllerComponentCommand.showContent)]
            public virtual Task<NullStruct> ShowContentAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GpsContentControllerComponentCommand.showContent, request);
            }
            
            
            public override Type GetCommandRequestType(GpsContentControllerComponentCommand command) => GpsContentControllerComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GpsContentControllerComponentCommand command) => GpsContentControllerComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GpsContentControllerComponentCommand command) => GpsContentControllerComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GpsContentControllerComponentNotification notification) => GpsContentControllerComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(GpsContentControllerComponentCommand command) => command switch
        {
            GpsContentControllerComponentCommand.filePetition => typeof(NullStruct),
            GpsContentControllerComponentCommand.fetchContent => typeof(NullStruct),
            GpsContentControllerComponentCommand.showContent => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(GpsContentControllerComponentCommand command) => command switch
        {
            GpsContentControllerComponentCommand.filePetition => typeof(NullStruct),
            GpsContentControllerComponentCommand.fetchContent => typeof(NullStruct),
            GpsContentControllerComponentCommand.showContent => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(GpsContentControllerComponentCommand command) => command switch
        {
            GpsContentControllerComponentCommand.filePetition => typeof(NullStruct),
            GpsContentControllerComponentCommand.fetchContent => typeof(NullStruct),
            GpsContentControllerComponentCommand.showContent => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(GpsContentControllerComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum GpsContentControllerComponentCommand : ushort
        {
            filePetition = 1,
            fetchContent = 2,
            showContent = 3,
        }
        
        public enum GpsContentControllerComponentNotification : ushort
        {
        }
        
    }
}
