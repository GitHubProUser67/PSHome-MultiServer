using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class CommerceInfoComponentBase
    {
        public const ushort Id = 24;
        public const string Name = "CommerceInfoComponent";
        
        public class Server : BlazeServerComponent<CommerceInfoComponentCommand, CommerceInfoComponentNotification, Blaze2RpcError>
        {
            public Server() : base(CommerceInfoComponentBase.Id, CommerceInfoComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.getCatalogMap)]
            public virtual Task<NullStruct> GetCatalogMapAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.getCategoryMap)]
            public virtual Task<NullStruct> GetCategoryMapAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.getProductList)]
            public virtual Task<NullStruct> GetProductListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.refreshOfbCache)]
            public virtual Task<NullStruct> RefreshOfbCacheAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.getProductAssociation)]
            public virtual Task<NullStruct> GetProductAssociationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.getWalletBalance)]
            public virtual Task<NullStruct> GetWalletBalanceAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.checkoutProducts)]
            public virtual Task<NullStruct> CheckoutProductsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(CommerceInfoComponentCommand command) => CommerceInfoComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(CommerceInfoComponentCommand command) => CommerceInfoComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(CommerceInfoComponentCommand command) => CommerceInfoComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(CommerceInfoComponentNotification notification) => CommerceInfoComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<CommerceInfoComponentCommand, CommerceInfoComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(CommerceInfoComponentBase.Id, CommerceInfoComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct GetCatalogMap()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getCatalogMap, new NullStruct());
            }
            public Task<NullStruct> GetCatalogMapAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getCatalogMap, new NullStruct());
            }
            
            public NullStruct GetCategoryMap()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getCategoryMap, new NullStruct());
            }
            public Task<NullStruct> GetCategoryMapAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getCategoryMap, new NullStruct());
            }
            
            public NullStruct GetProductList()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getProductList, new NullStruct());
            }
            public Task<NullStruct> GetProductListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getProductList, new NullStruct());
            }
            
            public NullStruct RefreshOfbCache()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.refreshOfbCache, new NullStruct());
            }
            public Task<NullStruct> RefreshOfbCacheAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.refreshOfbCache, new NullStruct());
            }
            
            public NullStruct GetProductAssociation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getProductAssociation, new NullStruct());
            }
            public Task<NullStruct> GetProductAssociationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getProductAssociation, new NullStruct());
            }
            
            public NullStruct GetWalletBalance()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getWalletBalance, new NullStruct());
            }
            public Task<NullStruct> GetWalletBalanceAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getWalletBalance, new NullStruct());
            }
            
            public NullStruct CheckoutProducts()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.checkoutProducts, new NullStruct());
            }
            public Task<NullStruct> CheckoutProductsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.checkoutProducts, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(CommerceInfoComponentCommand command) => CommerceInfoComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(CommerceInfoComponentCommand command) => CommerceInfoComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(CommerceInfoComponentCommand command) => CommerceInfoComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(CommerceInfoComponentNotification notification) => CommerceInfoComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<CommerceInfoComponentCommand, CommerceInfoComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(CommerceInfoComponentBase.Id, CommerceInfoComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.getCatalogMap)]
            public virtual Task<NullStruct> GetCatalogMapAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getCatalogMap, request);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.getCategoryMap)]
            public virtual Task<NullStruct> GetCategoryMapAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getCategoryMap, request);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.getProductList)]
            public virtual Task<NullStruct> GetProductListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getProductList, request);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.refreshOfbCache)]
            public virtual Task<NullStruct> RefreshOfbCacheAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.refreshOfbCache, request);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.getProductAssociation)]
            public virtual Task<NullStruct> GetProductAssociationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getProductAssociation, request);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.getWalletBalance)]
            public virtual Task<NullStruct> GetWalletBalanceAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.getWalletBalance, request);
            }
            
            [BlazeCommand((ushort)CommerceInfoComponentCommand.checkoutProducts)]
            public virtual Task<NullStruct> CheckoutProductsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CommerceInfoComponentCommand.checkoutProducts, request);
            }
            
            
            public override Type GetCommandRequestType(CommerceInfoComponentCommand command) => CommerceInfoComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(CommerceInfoComponentCommand command) => CommerceInfoComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(CommerceInfoComponentCommand command) => CommerceInfoComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(CommerceInfoComponentNotification notification) => CommerceInfoComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(CommerceInfoComponentCommand command) => command switch
        {
            CommerceInfoComponentCommand.getCatalogMap => typeof(NullStruct),
            CommerceInfoComponentCommand.getCategoryMap => typeof(NullStruct),
            CommerceInfoComponentCommand.getProductList => typeof(NullStruct),
            CommerceInfoComponentCommand.refreshOfbCache => typeof(NullStruct),
            CommerceInfoComponentCommand.getProductAssociation => typeof(NullStruct),
            CommerceInfoComponentCommand.getWalletBalance => typeof(NullStruct),
            CommerceInfoComponentCommand.checkoutProducts => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(CommerceInfoComponentCommand command) => command switch
        {
            CommerceInfoComponentCommand.getCatalogMap => typeof(NullStruct),
            CommerceInfoComponentCommand.getCategoryMap => typeof(NullStruct),
            CommerceInfoComponentCommand.getProductList => typeof(NullStruct),
            CommerceInfoComponentCommand.refreshOfbCache => typeof(NullStruct),
            CommerceInfoComponentCommand.getProductAssociation => typeof(NullStruct),
            CommerceInfoComponentCommand.getWalletBalance => typeof(NullStruct),
            CommerceInfoComponentCommand.checkoutProducts => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(CommerceInfoComponentCommand command) => command switch
        {
            CommerceInfoComponentCommand.getCatalogMap => typeof(NullStruct),
            CommerceInfoComponentCommand.getCategoryMap => typeof(NullStruct),
            CommerceInfoComponentCommand.getProductList => typeof(NullStruct),
            CommerceInfoComponentCommand.refreshOfbCache => typeof(NullStruct),
            CommerceInfoComponentCommand.getProductAssociation => typeof(NullStruct),
            CommerceInfoComponentCommand.getWalletBalance => typeof(NullStruct),
            CommerceInfoComponentCommand.checkoutProducts => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(CommerceInfoComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum CommerceInfoComponentCommand : ushort
        {
            getCatalogMap = 1,
            getCategoryMap = 2,
            getProductList = 3,
            refreshOfbCache = 4,
            getProductAssociation = 5,
            getWalletBalance = 6,
            checkoutProducts = 7,
        }
        
        public enum CommerceInfoComponentNotification : ushort
        {
        }
        
    }
}
