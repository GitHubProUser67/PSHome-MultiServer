using Blaze3SDK.Blaze.Rsp;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class RspComponentBase
    {
        public const ushort Id = 2049;
        public const string Name = "RspComponent";
        
        public class Server : BlazeServerComponent<RspComponentCommand, RspComponentNotification, Blaze3RpcError>
        {
            public Server() : base(RspComponentBase.Id, RspComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)RspComponentCommand.startPurchase)]
            public virtual Task<NullStruct> StartPurchaseAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updatePurchase)]
            public virtual Task<NullStruct> UpdatePurchaseAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.finishPurchase)]
            public virtual Task<NullStruct> FinishPurchaseAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.listPurchases)]
            public virtual Task<NullStruct> ListPurchasesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.listServers)]
            public virtual Task<NullStruct> ListServersAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.getServerDetails)]
            public virtual Task<NullStruct> GetServerDetailsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.restartServer)]
            public virtual Task<NullStruct> RestartServerAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updateServerBanner)]
            public virtual Task<NullStruct> UpdateServerBannerAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updateServerSettings)]
            public virtual Task<NullStruct> UpdateServerSettingsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updateServerPreset)]
            public virtual Task<NullStruct> UpdateServerPresetAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updateServerMapRotation)]
            public virtual Task<NullStruct> UpdateServerMapRotationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.addServerAdmin)]
            public virtual Task<NullStruct> AddServerAdminAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.removeServerAdmin)]
            public virtual Task<NullStruct> RemoveServerAdminAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.addServerBan)]
            public virtual Task<NullStruct> AddServerBanAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.removeServerBan)]
            public virtual Task<NullStruct> RemoveServerBanAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.addServerVip)]
            public virtual Task<NullStruct> AddServerVipAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.removeServerVip)]
            public virtual Task<NullStruct> RemoveServerVipAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.getConfig)]
            public virtual Task<GetConfigResponse> GetConfigAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.getPingSites)]
            public virtual Task<NullStruct> GetPingSitesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.getGameData)]
            public virtual Task<NullStruct> GetGameDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.addGameBan)]
            public virtual Task<NullStruct> AddGameBanAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.createServer)]
            public virtual Task<NullStruct> CreateServerAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updateServer)]
            public virtual Task<NullStruct> UpdateServerAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.listAllServers)]
            public virtual Task<NullStruct> ListAllServersAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.startMatch)]
            public virtual Task<NullStruct> StartMatchAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.abortMatch)]
            public virtual Task<NullStruct> AbortMatchAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.endMatch)]
            public virtual Task<NullStruct> EndMatchAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(RspComponentCommand command) => RspComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RspComponentCommand command) => RspComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RspComponentCommand command) => RspComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RspComponentNotification notification) => RspComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<RspComponentCommand, RspComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(RspComponentBase.Id, RspComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct StartPurchase()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.startPurchase, new NullStruct());
            }
            public Task<NullStruct> StartPurchaseAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.startPurchase, new NullStruct());
            }
            
            public NullStruct UpdatePurchase()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updatePurchase, new NullStruct());
            }
            public Task<NullStruct> UpdatePurchaseAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updatePurchase, new NullStruct());
            }
            
            public NullStruct FinishPurchase()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.finishPurchase, new NullStruct());
            }
            public Task<NullStruct> FinishPurchaseAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.finishPurchase, new NullStruct());
            }
            
            public NullStruct ListPurchases()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.listPurchases, new NullStruct());
            }
            public Task<NullStruct> ListPurchasesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.listPurchases, new NullStruct());
            }
            
            public NullStruct ListServers()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.listServers, new NullStruct());
            }
            public Task<NullStruct> ListServersAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.listServers, new NullStruct());
            }
            
            public NullStruct GetServerDetails()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.getServerDetails, new NullStruct());
            }
            public Task<NullStruct> GetServerDetailsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.getServerDetails, new NullStruct());
            }
            
            public NullStruct RestartServer()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.restartServer, new NullStruct());
            }
            public Task<NullStruct> RestartServerAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.restartServer, new NullStruct());
            }
            
            public NullStruct UpdateServerBanner()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerBanner, new NullStruct());
            }
            public Task<NullStruct> UpdateServerBannerAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerBanner, new NullStruct());
            }
            
            public NullStruct UpdateServerSettings()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerSettings, new NullStruct());
            }
            public Task<NullStruct> UpdateServerSettingsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerSettings, new NullStruct());
            }
            
            public NullStruct UpdateServerPreset()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerPreset, new NullStruct());
            }
            public Task<NullStruct> UpdateServerPresetAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerPreset, new NullStruct());
            }
            
            public NullStruct UpdateServerMapRotation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerMapRotation, new NullStruct());
            }
            public Task<NullStruct> UpdateServerMapRotationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerMapRotation, new NullStruct());
            }
            
            public NullStruct AddServerAdmin()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addServerAdmin, new NullStruct());
            }
            public Task<NullStruct> AddServerAdminAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addServerAdmin, new NullStruct());
            }
            
            public NullStruct RemoveServerAdmin()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.removeServerAdmin, new NullStruct());
            }
            public Task<NullStruct> RemoveServerAdminAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.removeServerAdmin, new NullStruct());
            }
            
            public NullStruct AddServerBan()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addServerBan, new NullStruct());
            }
            public Task<NullStruct> AddServerBanAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addServerBan, new NullStruct());
            }
            
            public NullStruct RemoveServerBan()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.removeServerBan, new NullStruct());
            }
            public Task<NullStruct> RemoveServerBanAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.removeServerBan, new NullStruct());
            }
            
            public NullStruct AddServerVip()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addServerVip, new NullStruct());
            }
            public Task<NullStruct> AddServerVipAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addServerVip, new NullStruct());
            }
            
            public NullStruct RemoveServerVip()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.removeServerVip, new NullStruct());
            }
            public Task<NullStruct> RemoveServerVipAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.removeServerVip, new NullStruct());
            }
            
            public GetConfigResponse GetConfig()
            {
                return Connection.SendRequest<NullStruct, GetConfigResponse, NullStruct>(this, (ushort)RspComponentCommand.getConfig, new NullStruct());
            }
            public Task<GetConfigResponse> GetConfigAsync()
            {
                return Connection.SendRequestAsync<NullStruct, GetConfigResponse, NullStruct>(this, (ushort)RspComponentCommand.getConfig, new NullStruct());
            }
            
            public NullStruct GetPingSites()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.getPingSites, new NullStruct());
            }
            public Task<NullStruct> GetPingSitesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.getPingSites, new NullStruct());
            }
            
            public NullStruct GetGameData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.getGameData, new NullStruct());
            }
            public Task<NullStruct> GetGameDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.getGameData, new NullStruct());
            }
            
            public NullStruct AddGameBan()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addGameBan, new NullStruct());
            }
            public Task<NullStruct> AddGameBanAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addGameBan, new NullStruct());
            }
            
            public NullStruct CreateServer()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.createServer, new NullStruct());
            }
            public Task<NullStruct> CreateServerAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.createServer, new NullStruct());
            }
            
            public NullStruct UpdateServer()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServer, new NullStruct());
            }
            public Task<NullStruct> UpdateServerAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServer, new NullStruct());
            }
            
            public NullStruct ListAllServers()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.listAllServers, new NullStruct());
            }
            public Task<NullStruct> ListAllServersAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.listAllServers, new NullStruct());
            }
            
            public NullStruct StartMatch()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.startMatch, new NullStruct());
            }
            public Task<NullStruct> StartMatchAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.startMatch, new NullStruct());
            }
            
            public NullStruct AbortMatch()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.abortMatch, new NullStruct());
            }
            public Task<NullStruct> AbortMatchAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.abortMatch, new NullStruct());
            }
            
            public NullStruct EndMatch()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.endMatch, new NullStruct());
            }
            public Task<NullStruct> EndMatchAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.endMatch, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(RspComponentCommand command) => RspComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RspComponentCommand command) => RspComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RspComponentCommand command) => RspComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RspComponentNotification notification) => RspComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<RspComponentCommand, RspComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(RspComponentBase.Id, RspComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)RspComponentCommand.startPurchase)]
            public virtual Task<NullStruct> StartPurchaseAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.startPurchase, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updatePurchase)]
            public virtual Task<NullStruct> UpdatePurchaseAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updatePurchase, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.finishPurchase)]
            public virtual Task<NullStruct> FinishPurchaseAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.finishPurchase, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.listPurchases)]
            public virtual Task<NullStruct> ListPurchasesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.listPurchases, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.listServers)]
            public virtual Task<NullStruct> ListServersAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.listServers, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.getServerDetails)]
            public virtual Task<NullStruct> GetServerDetailsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.getServerDetails, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.restartServer)]
            public virtual Task<NullStruct> RestartServerAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.restartServer, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updateServerBanner)]
            public virtual Task<NullStruct> UpdateServerBannerAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerBanner, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updateServerSettings)]
            public virtual Task<NullStruct> UpdateServerSettingsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerSettings, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updateServerPreset)]
            public virtual Task<NullStruct> UpdateServerPresetAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerPreset, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updateServerMapRotation)]
            public virtual Task<NullStruct> UpdateServerMapRotationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServerMapRotation, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.addServerAdmin)]
            public virtual Task<NullStruct> AddServerAdminAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addServerAdmin, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.removeServerAdmin)]
            public virtual Task<NullStruct> RemoveServerAdminAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.removeServerAdmin, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.addServerBan)]
            public virtual Task<NullStruct> AddServerBanAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addServerBan, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.removeServerBan)]
            public virtual Task<NullStruct> RemoveServerBanAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.removeServerBan, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.addServerVip)]
            public virtual Task<NullStruct> AddServerVipAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addServerVip, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.removeServerVip)]
            public virtual Task<NullStruct> RemoveServerVipAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.removeServerVip, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.getConfig)]
            public virtual Task<GetConfigResponse> GetConfigAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, GetConfigResponse, NullStruct>(this, (ushort)RspComponentCommand.getConfig, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.getPingSites)]
            public virtual Task<NullStruct> GetPingSitesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.getPingSites, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.getGameData)]
            public virtual Task<NullStruct> GetGameDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.getGameData, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.addGameBan)]
            public virtual Task<NullStruct> AddGameBanAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.addGameBan, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.createServer)]
            public virtual Task<NullStruct> CreateServerAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.createServer, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.updateServer)]
            public virtual Task<NullStruct> UpdateServerAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.updateServer, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.listAllServers)]
            public virtual Task<NullStruct> ListAllServersAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.listAllServers, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.startMatch)]
            public virtual Task<NullStruct> StartMatchAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.startMatch, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.abortMatch)]
            public virtual Task<NullStruct> AbortMatchAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.abortMatch, request);
            }
            
            [BlazeCommand((ushort)RspComponentCommand.endMatch)]
            public virtual Task<NullStruct> EndMatchAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RspComponentCommand.endMatch, request);
            }
            
            
            public override Type GetCommandRequestType(RspComponentCommand command) => RspComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RspComponentCommand command) => RspComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RspComponentCommand command) => RspComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RspComponentNotification notification) => RspComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(RspComponentCommand command) => command switch
        {
            RspComponentCommand.startPurchase => typeof(NullStruct),
            RspComponentCommand.updatePurchase => typeof(NullStruct),
            RspComponentCommand.finishPurchase => typeof(NullStruct),
            RspComponentCommand.listPurchases => typeof(NullStruct),
            RspComponentCommand.listServers => typeof(NullStruct),
            RspComponentCommand.getServerDetails => typeof(NullStruct),
            RspComponentCommand.restartServer => typeof(NullStruct),
            RspComponentCommand.updateServerBanner => typeof(NullStruct),
            RspComponentCommand.updateServerSettings => typeof(NullStruct),
            RspComponentCommand.updateServerPreset => typeof(NullStruct),
            RspComponentCommand.updateServerMapRotation => typeof(NullStruct),
            RspComponentCommand.addServerAdmin => typeof(NullStruct),
            RspComponentCommand.removeServerAdmin => typeof(NullStruct),
            RspComponentCommand.addServerBan => typeof(NullStruct),
            RspComponentCommand.removeServerBan => typeof(NullStruct),
            RspComponentCommand.addServerVip => typeof(NullStruct),
            RspComponentCommand.removeServerVip => typeof(NullStruct),
            RspComponentCommand.getConfig => typeof(NullStruct),
            RspComponentCommand.getPingSites => typeof(NullStruct),
            RspComponentCommand.getGameData => typeof(NullStruct),
            RspComponentCommand.addGameBan => typeof(NullStruct),
            RspComponentCommand.createServer => typeof(NullStruct),
            RspComponentCommand.updateServer => typeof(NullStruct),
            RspComponentCommand.listAllServers => typeof(NullStruct),
            RspComponentCommand.startMatch => typeof(NullStruct),
            RspComponentCommand.abortMatch => typeof(NullStruct),
            RspComponentCommand.endMatch => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(RspComponentCommand command) => command switch
        {
            RspComponentCommand.startPurchase => typeof(NullStruct),
            RspComponentCommand.updatePurchase => typeof(NullStruct),
            RspComponentCommand.finishPurchase => typeof(NullStruct),
            RspComponentCommand.listPurchases => typeof(NullStruct),
            RspComponentCommand.listServers => typeof(NullStruct),
            RspComponentCommand.getServerDetails => typeof(NullStruct),
            RspComponentCommand.restartServer => typeof(NullStruct),
            RspComponentCommand.updateServerBanner => typeof(NullStruct),
            RspComponentCommand.updateServerSettings => typeof(NullStruct),
            RspComponentCommand.updateServerPreset => typeof(NullStruct),
            RspComponentCommand.updateServerMapRotation => typeof(NullStruct),
            RspComponentCommand.addServerAdmin => typeof(NullStruct),
            RspComponentCommand.removeServerAdmin => typeof(NullStruct),
            RspComponentCommand.addServerBan => typeof(NullStruct),
            RspComponentCommand.removeServerBan => typeof(NullStruct),
            RspComponentCommand.addServerVip => typeof(NullStruct),
            RspComponentCommand.removeServerVip => typeof(NullStruct),
            RspComponentCommand.getConfig => typeof(GetConfigResponse),
            RspComponentCommand.getPingSites => typeof(NullStruct),
            RspComponentCommand.getGameData => typeof(NullStruct),
            RspComponentCommand.addGameBan => typeof(NullStruct),
            RspComponentCommand.createServer => typeof(NullStruct),
            RspComponentCommand.updateServer => typeof(NullStruct),
            RspComponentCommand.listAllServers => typeof(NullStruct),
            RspComponentCommand.startMatch => typeof(NullStruct),
            RspComponentCommand.abortMatch => typeof(NullStruct),
            RspComponentCommand.endMatch => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(RspComponentCommand command) => command switch
        {
            RspComponentCommand.startPurchase => typeof(NullStruct),
            RspComponentCommand.updatePurchase => typeof(NullStruct),
            RspComponentCommand.finishPurchase => typeof(NullStruct),
            RspComponentCommand.listPurchases => typeof(NullStruct),
            RspComponentCommand.listServers => typeof(NullStruct),
            RspComponentCommand.getServerDetails => typeof(NullStruct),
            RspComponentCommand.restartServer => typeof(NullStruct),
            RspComponentCommand.updateServerBanner => typeof(NullStruct),
            RspComponentCommand.updateServerSettings => typeof(NullStruct),
            RspComponentCommand.updateServerPreset => typeof(NullStruct),
            RspComponentCommand.updateServerMapRotation => typeof(NullStruct),
            RspComponentCommand.addServerAdmin => typeof(NullStruct),
            RspComponentCommand.removeServerAdmin => typeof(NullStruct),
            RspComponentCommand.addServerBan => typeof(NullStruct),
            RspComponentCommand.removeServerBan => typeof(NullStruct),
            RspComponentCommand.addServerVip => typeof(NullStruct),
            RspComponentCommand.removeServerVip => typeof(NullStruct),
            RspComponentCommand.getConfig => typeof(NullStruct),
            RspComponentCommand.getPingSites => typeof(NullStruct),
            RspComponentCommand.getGameData => typeof(NullStruct),
            RspComponentCommand.addGameBan => typeof(NullStruct),
            RspComponentCommand.createServer => typeof(NullStruct),
            RspComponentCommand.updateServer => typeof(NullStruct),
            RspComponentCommand.listAllServers => typeof(NullStruct),
            RspComponentCommand.startMatch => typeof(NullStruct),
            RspComponentCommand.abortMatch => typeof(NullStruct),
            RspComponentCommand.endMatch => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(RspComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum RspComponentCommand : ushort
        {
            startPurchase = 10,
            updatePurchase = 11,
            finishPurchase = 12,
            listPurchases = 15,
            listServers = 20,
            getServerDetails = 21,
            restartServer = 23,
            updateServerBanner = 24,
            updateServerSettings = 25,
            updateServerPreset = 26,
            updateServerMapRotation = 27,
            addServerAdmin = 31,
            removeServerAdmin = 32,
            addServerBan = 33,
            removeServerBan = 34,
            addServerVip = 35,
            removeServerVip = 36,
            getConfig = 50,
            getPingSites = 51,
            getGameData = 60,
            addGameBan = 61,
            createServer = 70,
            updateServer = 71,
            listAllServers = 72,
            startMatch = 80,
            abortMatch = 81,
            endMatch = 82,
        }
        
        public enum RspComponentNotification : ushort
        {
        }
        
    }
}
