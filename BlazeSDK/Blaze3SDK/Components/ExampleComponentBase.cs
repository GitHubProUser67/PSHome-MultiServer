using Blaze3SDK.Blaze.Example;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class ExampleComponentBase
    {
        public const ushort Id = 3;
        public const string Name = "ExampleComponent";
        
        public class Server : BlazeServerComponent<ExampleComponentCommand, ExampleComponentNotification, Blaze3RpcError>
        {
            public Server() : base(ExampleComponentBase.Id, ExampleComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)ExampleComponentCommand.poke)]
            public virtual Task<ExampleResponse> PokeAsync(ExampleRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(ExampleComponentCommand command) => ExampleComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(ExampleComponentCommand command) => ExampleComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(ExampleComponentCommand command) => ExampleComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(ExampleComponentNotification notification) => ExampleComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<ExampleComponentCommand, ExampleComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(ExampleComponentBase.Id, ExampleComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public ExampleResponse Poke(ExampleRequest request)
            {
                return Connection.SendRequest<ExampleRequest, ExampleResponse, ExampleError>(this, (ushort)ExampleComponentCommand.poke, request);
            }
            public Task<ExampleResponse> PokeAsync(ExampleRequest request)
            {
                return Connection.SendRequestAsync<ExampleRequest, ExampleResponse, ExampleError>(this, (ushort)ExampleComponentCommand.poke, request);
            }
            
            
            public override Type GetCommandRequestType(ExampleComponentCommand command) => ExampleComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(ExampleComponentCommand command) => ExampleComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(ExampleComponentCommand command) => ExampleComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(ExampleComponentNotification notification) => ExampleComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<ExampleComponentCommand, ExampleComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(ExampleComponentBase.Id, ExampleComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)ExampleComponentCommand.poke)]
            public virtual Task<ExampleResponse> PokeAsync(ExampleRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ExampleRequest, ExampleResponse, ExampleError>(this, (ushort)ExampleComponentCommand.poke, request);
            }
            
            
            public override Type GetCommandRequestType(ExampleComponentCommand command) => ExampleComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(ExampleComponentCommand command) => ExampleComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(ExampleComponentCommand command) => ExampleComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(ExampleComponentNotification notification) => ExampleComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(ExampleComponentCommand command) => command switch
        {
            ExampleComponentCommand.poke => typeof(ExampleRequest),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(ExampleComponentCommand command) => command switch
        {
            ExampleComponentCommand.poke => typeof(ExampleResponse),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(ExampleComponentCommand command) => command switch
        {
            ExampleComponentCommand.poke => typeof(ExampleError),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(ExampleComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum ExampleComponentCommand : ushort
        {
            poke = 1,
        }
        
        public enum ExampleComponentNotification : ushort
        {
        }
        
    }
}
