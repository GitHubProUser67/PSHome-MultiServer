using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class RegistrationComponentBase
    {
        public const ushort Id = 22;
        public const string Name = "RegistrationComponent";
        
        public class Server : BlazeServerComponent<RegistrationComponentCommand, RegistrationComponentNotification, Blaze2RpcError>
        {
            public Server() : base(RegistrationComponentBase.Id, RegistrationComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.check)]
            public virtual Task<NullStruct> CheckAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.addrow)]
            public virtual Task<NullStruct> AddrowAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.ban)]
            public virtual Task<NullStruct> BanAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.remrow)]
            public virtual Task<NullStruct> RemrowAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.returnusers)]
            public virtual Task<NullStruct> ReturnusersAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.numusers)]
            public virtual Task<NullStruct> NumusersAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.updatenote)]
            public virtual Task<NullStruct> UpdatenoteAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.wipestats)]
            public virtual Task<NullStruct> WipestatsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.updateflags)]
            public virtual Task<NullStruct> UpdateflagsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.getDbCredentials)]
            public virtual Task<NullStruct> GetDbCredentialsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(RegistrationComponentCommand command) => RegistrationComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RegistrationComponentCommand command) => RegistrationComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RegistrationComponentCommand command) => RegistrationComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RegistrationComponentNotification notification) => RegistrationComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<RegistrationComponentCommand, RegistrationComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(RegistrationComponentBase.Id, RegistrationComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct Check()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.check, new NullStruct());
            }
            public Task<NullStruct> CheckAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.check, new NullStruct());
            }
            
            public NullStruct Addrow()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.addrow, new NullStruct());
            }
            public Task<NullStruct> AddrowAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.addrow, new NullStruct());
            }
            
            public NullStruct Ban()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.ban, new NullStruct());
            }
            public Task<NullStruct> BanAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.ban, new NullStruct());
            }
            
            public NullStruct Remrow()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.remrow, new NullStruct());
            }
            public Task<NullStruct> RemrowAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.remrow, new NullStruct());
            }
            
            public NullStruct Returnusers()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.returnusers, new NullStruct());
            }
            public Task<NullStruct> ReturnusersAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.returnusers, new NullStruct());
            }
            
            public NullStruct Numusers()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.numusers, new NullStruct());
            }
            public Task<NullStruct> NumusersAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.numusers, new NullStruct());
            }
            
            public NullStruct Updatenote()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.updatenote, new NullStruct());
            }
            public Task<NullStruct> UpdatenoteAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.updatenote, new NullStruct());
            }
            
            public NullStruct Wipestats()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.wipestats, new NullStruct());
            }
            public Task<NullStruct> WipestatsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.wipestats, new NullStruct());
            }
            
            public NullStruct Updateflags()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.updateflags, new NullStruct());
            }
            public Task<NullStruct> UpdateflagsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.updateflags, new NullStruct());
            }
            
            public NullStruct GetDbCredentials()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.getDbCredentials, new NullStruct());
            }
            public Task<NullStruct> GetDbCredentialsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.getDbCredentials, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(RegistrationComponentCommand command) => RegistrationComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RegistrationComponentCommand command) => RegistrationComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RegistrationComponentCommand command) => RegistrationComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RegistrationComponentNotification notification) => RegistrationComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<RegistrationComponentCommand, RegistrationComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(RegistrationComponentBase.Id, RegistrationComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.check)]
            public virtual Task<NullStruct> CheckAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.check, request);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.addrow)]
            public virtual Task<NullStruct> AddrowAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.addrow, request);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.ban)]
            public virtual Task<NullStruct> BanAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.ban, request);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.remrow)]
            public virtual Task<NullStruct> RemrowAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.remrow, request);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.returnusers)]
            public virtual Task<NullStruct> ReturnusersAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.returnusers, request);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.numusers)]
            public virtual Task<NullStruct> NumusersAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.numusers, request);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.updatenote)]
            public virtual Task<NullStruct> UpdatenoteAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.updatenote, request);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.wipestats)]
            public virtual Task<NullStruct> WipestatsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.wipestats, request);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.updateflags)]
            public virtual Task<NullStruct> UpdateflagsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.updateflags, request);
            }
            
            [BlazeCommand((ushort)RegistrationComponentCommand.getDbCredentials)]
            public virtual Task<NullStruct> GetDbCredentialsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RegistrationComponentCommand.getDbCredentials, request);
            }
            
            
            public override Type GetCommandRequestType(RegistrationComponentCommand command) => RegistrationComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RegistrationComponentCommand command) => RegistrationComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RegistrationComponentCommand command) => RegistrationComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RegistrationComponentNotification notification) => RegistrationComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(RegistrationComponentCommand command) => command switch
        {
            RegistrationComponentCommand.check => typeof(NullStruct),
            RegistrationComponentCommand.addrow => typeof(NullStruct),
            RegistrationComponentCommand.ban => typeof(NullStruct),
            RegistrationComponentCommand.remrow => typeof(NullStruct),
            RegistrationComponentCommand.returnusers => typeof(NullStruct),
            RegistrationComponentCommand.numusers => typeof(NullStruct),
            RegistrationComponentCommand.updatenote => typeof(NullStruct),
            RegistrationComponentCommand.wipestats => typeof(NullStruct),
            RegistrationComponentCommand.updateflags => typeof(NullStruct),
            RegistrationComponentCommand.getDbCredentials => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(RegistrationComponentCommand command) => command switch
        {
            RegistrationComponentCommand.check => typeof(NullStruct),
            RegistrationComponentCommand.addrow => typeof(NullStruct),
            RegistrationComponentCommand.ban => typeof(NullStruct),
            RegistrationComponentCommand.remrow => typeof(NullStruct),
            RegistrationComponentCommand.returnusers => typeof(NullStruct),
            RegistrationComponentCommand.numusers => typeof(NullStruct),
            RegistrationComponentCommand.updatenote => typeof(NullStruct),
            RegistrationComponentCommand.wipestats => typeof(NullStruct),
            RegistrationComponentCommand.updateflags => typeof(NullStruct),
            RegistrationComponentCommand.getDbCredentials => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(RegistrationComponentCommand command) => command switch
        {
            RegistrationComponentCommand.check => typeof(NullStruct),
            RegistrationComponentCommand.addrow => typeof(NullStruct),
            RegistrationComponentCommand.ban => typeof(NullStruct),
            RegistrationComponentCommand.remrow => typeof(NullStruct),
            RegistrationComponentCommand.returnusers => typeof(NullStruct),
            RegistrationComponentCommand.numusers => typeof(NullStruct),
            RegistrationComponentCommand.updatenote => typeof(NullStruct),
            RegistrationComponentCommand.wipestats => typeof(NullStruct),
            RegistrationComponentCommand.updateflags => typeof(NullStruct),
            RegistrationComponentCommand.getDbCredentials => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(RegistrationComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum RegistrationComponentCommand : ushort
        {
            check = 2,
            addrow = 3,
            ban = 4,
            remrow = 5,
            returnusers = 9,
            numusers = 10,
            updatenote = 13,
            wipestats = 14,
            updateflags = 15,
            getDbCredentials = 16,
        }
        
        public enum RegistrationComponentNotification : ushort
        {
        }
        
    }
}
