using Blaze2SDK.Blaze;
using Blaze2SDK.Blaze.Util;
using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class UtilComponentBase
    {
        public const ushort Id = 9;
        public const string Name = "UtilComponent";
        
        public class Server : BlazeServerComponent<UtilComponentCommand, UtilComponentNotification, Blaze2RpcError>
        {
            public Server() : base(UtilComponentBase.Id, UtilComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.fetchClientConfig)]
            public virtual Task<FetchConfigResponse> FetchClientConfigAsync(FetchClientConfigRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.ping)]
            public virtual Task<PingResponse> PingAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.setClientData)]
            public virtual Task<NullStruct> SetClientDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.localizeStrings)]
            public virtual Task<NullStruct> LocalizeStringsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.getTelemetryServer)]
            public virtual Task<GetTelemetryServerResponse> GetTelemetryServerAsync(GetTelemetryServerRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.getTickerServer)]
            public virtual Task<NullStruct> GetTickerServerAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.preAuth)]
            public virtual Task<PreAuthResponse> PreAuthAsync(PreAuthRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.postAuth)]
            public virtual Task<PostAuthResponse> PostAuthAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.userSettingsLoad)]
            public virtual Task<NullStruct> UserSettingsLoadAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.userSettingsSave)]
            public virtual Task<NullStruct> UserSettingsSaveAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.userSettingsLoadAll)]
            public virtual Task<NullStruct> UserSettingsLoadAllAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.userSettingsLoadAllForUserId)]
            public virtual Task<NullStruct> UserSettingsLoadAllForUserIdAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.filterForProfanity)]
            public virtual Task<FilterUserTextResponse> FilterForProfanityAsync(FilterUserTextResponse request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.fetchQosConfig)]
            public virtual Task<NullStruct> FetchQosConfigAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.setClientMetrics)]
            public virtual Task<NullStruct> SetClientMetricsAsync(ClientMetrics request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.setConnectionState)]
            public virtual Task<NullStruct> SetConnectionStateAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(UtilComponentCommand command) => UtilComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(UtilComponentCommand command) => UtilComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(UtilComponentCommand command) => UtilComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(UtilComponentNotification notification) => UtilComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<UtilComponentCommand, UtilComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(UtilComponentBase.Id, UtilComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public FetchConfigResponse FetchClientConfig(FetchClientConfigRequest request)
            {
                return Connection.SendRequest<FetchClientConfigRequest, FetchConfigResponse, NullStruct>(this, (ushort)UtilComponentCommand.fetchClientConfig, request);
            }
            public Task<FetchConfigResponse> FetchClientConfigAsync(FetchClientConfigRequest request)
            {
                return Connection.SendRequestAsync<FetchClientConfigRequest, FetchConfigResponse, NullStruct>(this, (ushort)UtilComponentCommand.fetchClientConfig, request);
            }
            
            public PingResponse Ping()
            {
                return Connection.SendRequest<NullStruct, PingResponse, NullStruct>(this, (ushort)UtilComponentCommand.ping, new NullStruct());
            }
            public Task<PingResponse> PingAsync()
            {
                return Connection.SendRequestAsync<NullStruct, PingResponse, NullStruct>(this, (ushort)UtilComponentCommand.ping, new NullStruct());
            }
            
            public NullStruct SetClientData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientData, new NullStruct());
            }
            public Task<NullStruct> SetClientDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientData, new NullStruct());
            }
            
            public NullStruct LocalizeStrings()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.localizeStrings, new NullStruct());
            }
            public Task<NullStruct> LocalizeStringsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.localizeStrings, new NullStruct());
            }
            
            public GetTelemetryServerResponse GetTelemetryServer(GetTelemetryServerRequest request)
            {
                return Connection.SendRequest<GetTelemetryServerRequest, GetTelemetryServerResponse, NullStruct>(this, (ushort)UtilComponentCommand.getTelemetryServer, request);
            }
            public Task<GetTelemetryServerResponse> GetTelemetryServerAsync(GetTelemetryServerRequest request)
            {
                return Connection.SendRequestAsync<GetTelemetryServerRequest, GetTelemetryServerResponse, NullStruct>(this, (ushort)UtilComponentCommand.getTelemetryServer, request);
            }
            
            public NullStruct GetTickerServer()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.getTickerServer, new NullStruct());
            }
            public Task<NullStruct> GetTickerServerAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.getTickerServer, new NullStruct());
            }
            
            public PreAuthResponse PreAuth(PreAuthRequest request)
            {
                return Connection.SendRequest<PreAuthRequest, PreAuthResponse, NullStruct>(this, (ushort)UtilComponentCommand.preAuth, request);
            }
            public Task<PreAuthResponse> PreAuthAsync(PreAuthRequest request)
            {
                return Connection.SendRequestAsync<PreAuthRequest, PreAuthResponse, NullStruct>(this, (ushort)UtilComponentCommand.preAuth, request);
            }
            
            public PostAuthResponse PostAuth()
            {
                return Connection.SendRequest<NullStruct, PostAuthResponse, NullStruct>(this, (ushort)UtilComponentCommand.postAuth, new NullStruct());
            }
            public Task<PostAuthResponse> PostAuthAsync()
            {
                return Connection.SendRequestAsync<NullStruct, PostAuthResponse, NullStruct>(this, (ushort)UtilComponentCommand.postAuth, new NullStruct());
            }
            
            public NullStruct UserSettingsLoad()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoad, new NullStruct());
            }
            public Task<NullStruct> UserSettingsLoadAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoad, new NullStruct());
            }
            
            public NullStruct UserSettingsSave()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsSave, new NullStruct());
            }
            public Task<NullStruct> UserSettingsSaveAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsSave, new NullStruct());
            }
            
            public NullStruct UserSettingsLoadAll()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoadAll, new NullStruct());
            }
            public Task<NullStruct> UserSettingsLoadAllAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoadAll, new NullStruct());
            }
            
            public NullStruct UserSettingsLoadAllForUserId()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoadAllForUserId, new NullStruct());
            }
            public Task<NullStruct> UserSettingsLoadAllForUserIdAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoadAllForUserId, new NullStruct());
            }
            
            public FilterUserTextResponse FilterForProfanity(FilterUserTextResponse request)
            {
                return Connection.SendRequest<FilterUserTextResponse, FilterUserTextResponse, NullStruct>(this, (ushort)UtilComponentCommand.filterForProfanity, request);
            }
            public Task<FilterUserTextResponse> FilterForProfanityAsync(FilterUserTextResponse request)
            {
                return Connection.SendRequestAsync<FilterUserTextResponse, FilterUserTextResponse, NullStruct>(this, (ushort)UtilComponentCommand.filterForProfanity, request);
            }
            
            public NullStruct FetchQosConfig()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.fetchQosConfig, new NullStruct());
            }
            public Task<NullStruct> FetchQosConfigAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.fetchQosConfig, new NullStruct());
            }
            
            public NullStruct SetClientMetrics(ClientMetrics request)
            {
                return Connection.SendRequest<ClientMetrics, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientMetrics, request);
            }
            public Task<NullStruct> SetClientMetricsAsync(ClientMetrics request)
            {
                return Connection.SendRequestAsync<ClientMetrics, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientMetrics, request);
            }
            
            public NullStruct SetConnectionState()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setConnectionState, new NullStruct());
            }
            public Task<NullStruct> SetConnectionStateAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setConnectionState, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(UtilComponentCommand command) => UtilComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(UtilComponentCommand command) => UtilComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(UtilComponentCommand command) => UtilComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(UtilComponentNotification notification) => UtilComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<UtilComponentCommand, UtilComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(UtilComponentBase.Id, UtilComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.fetchClientConfig)]
            public virtual Task<FetchConfigResponse> FetchClientConfigAsync(FetchClientConfigRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<FetchClientConfigRequest, FetchConfigResponse, NullStruct>(this, (ushort)UtilComponentCommand.fetchClientConfig, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.ping)]
            public virtual Task<PingResponse> PingAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, PingResponse, NullStruct>(this, (ushort)UtilComponentCommand.ping, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.setClientData)]
            public virtual Task<NullStruct> SetClientDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientData, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.localizeStrings)]
            public virtual Task<NullStruct> LocalizeStringsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.localizeStrings, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.getTelemetryServer)]
            public virtual Task<GetTelemetryServerResponse> GetTelemetryServerAsync(GetTelemetryServerRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetTelemetryServerRequest, GetTelemetryServerResponse, NullStruct>(this, (ushort)UtilComponentCommand.getTelemetryServer, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.getTickerServer)]
            public virtual Task<NullStruct> GetTickerServerAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.getTickerServer, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.preAuth)]
            public virtual Task<PreAuthResponse> PreAuthAsync(PreAuthRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<PreAuthRequest, PreAuthResponse, NullStruct>(this, (ushort)UtilComponentCommand.preAuth, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.postAuth)]
            public virtual Task<PostAuthResponse> PostAuthAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, PostAuthResponse, NullStruct>(this, (ushort)UtilComponentCommand.postAuth, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.userSettingsLoad)]
            public virtual Task<NullStruct> UserSettingsLoadAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoad, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.userSettingsSave)]
            public virtual Task<NullStruct> UserSettingsSaveAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsSave, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.userSettingsLoadAll)]
            public virtual Task<NullStruct> UserSettingsLoadAllAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoadAll, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.userSettingsLoadAllForUserId)]
            public virtual Task<NullStruct> UserSettingsLoadAllForUserIdAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoadAllForUserId, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.filterForProfanity)]
            public virtual Task<FilterUserTextResponse> FilterForProfanityAsync(FilterUserTextResponse request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<FilterUserTextResponse, FilterUserTextResponse, NullStruct>(this, (ushort)UtilComponentCommand.filterForProfanity, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.fetchQosConfig)]
            public virtual Task<NullStruct> FetchQosConfigAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.fetchQosConfig, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.setClientMetrics)]
            public virtual Task<NullStruct> SetClientMetricsAsync(ClientMetrics request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ClientMetrics, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientMetrics, request);
            }
            
            [BlazeCommand((ushort)UtilComponentCommand.setConnectionState)]
            public virtual Task<NullStruct> SetConnectionStateAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setConnectionState, request);
            }
            
            
            public override Type GetCommandRequestType(UtilComponentCommand command) => UtilComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(UtilComponentCommand command) => UtilComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(UtilComponentCommand command) => UtilComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(UtilComponentNotification notification) => UtilComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(UtilComponentCommand command) => command switch
        {
            UtilComponentCommand.fetchClientConfig => typeof(FetchClientConfigRequest),
            UtilComponentCommand.ping => typeof(NullStruct),
            UtilComponentCommand.setClientData => typeof(NullStruct),
            UtilComponentCommand.localizeStrings => typeof(NullStruct),
            UtilComponentCommand.getTelemetryServer => typeof(GetTelemetryServerRequest),
            UtilComponentCommand.getTickerServer => typeof(NullStruct),
            UtilComponentCommand.preAuth => typeof(PreAuthRequest),
            UtilComponentCommand.postAuth => typeof(NullStruct),
            UtilComponentCommand.userSettingsLoad => typeof(NullStruct),
            UtilComponentCommand.userSettingsSave => typeof(NullStruct),
            UtilComponentCommand.userSettingsLoadAll => typeof(NullStruct),
            UtilComponentCommand.userSettingsLoadAllForUserId => typeof(NullStruct),
            UtilComponentCommand.filterForProfanity => typeof(FilterUserTextResponse),
            UtilComponentCommand.fetchQosConfig => typeof(NullStruct),
            UtilComponentCommand.setClientMetrics => typeof(ClientMetrics),
            UtilComponentCommand.setConnectionState => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(UtilComponentCommand command) => command switch
        {
            UtilComponentCommand.fetchClientConfig => typeof(FetchConfigResponse),
            UtilComponentCommand.ping => typeof(PingResponse),
            UtilComponentCommand.setClientData => typeof(NullStruct),
            UtilComponentCommand.localizeStrings => typeof(NullStruct),
            UtilComponentCommand.getTelemetryServer => typeof(GetTelemetryServerResponse),
            UtilComponentCommand.getTickerServer => typeof(NullStruct),
            UtilComponentCommand.preAuth => typeof(PreAuthResponse),
            UtilComponentCommand.postAuth => typeof(PostAuthResponse),
            UtilComponentCommand.userSettingsLoad => typeof(NullStruct),
            UtilComponentCommand.userSettingsSave => typeof(NullStruct),
            UtilComponentCommand.userSettingsLoadAll => typeof(NullStruct),
            UtilComponentCommand.userSettingsLoadAllForUserId => typeof(NullStruct),
            UtilComponentCommand.filterForProfanity => typeof(FilterUserTextResponse),
            UtilComponentCommand.fetchQosConfig => typeof(NullStruct),
            UtilComponentCommand.setClientMetrics => typeof(NullStruct),
            UtilComponentCommand.setConnectionState => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(UtilComponentCommand command) => command switch
        {
            UtilComponentCommand.fetchClientConfig => typeof(NullStruct),
            UtilComponentCommand.ping => typeof(NullStruct),
            UtilComponentCommand.setClientData => typeof(NullStruct),
            UtilComponentCommand.localizeStrings => typeof(NullStruct),
            UtilComponentCommand.getTelemetryServer => typeof(NullStruct),
            UtilComponentCommand.getTickerServer => typeof(NullStruct),
            UtilComponentCommand.preAuth => typeof(NullStruct),
            UtilComponentCommand.postAuth => typeof(NullStruct),
            UtilComponentCommand.userSettingsLoad => typeof(NullStruct),
            UtilComponentCommand.userSettingsSave => typeof(NullStruct),
            UtilComponentCommand.userSettingsLoadAll => typeof(NullStruct),
            UtilComponentCommand.userSettingsLoadAllForUserId => typeof(NullStruct),
            UtilComponentCommand.filterForProfanity => typeof(NullStruct),
            UtilComponentCommand.fetchQosConfig => typeof(NullStruct),
            UtilComponentCommand.setClientMetrics => typeof(NullStruct),
            UtilComponentCommand.setConnectionState => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(UtilComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum UtilComponentCommand : ushort
        {
            fetchClientConfig = 1,
            ping = 2,
            setClientData = 3,
            localizeStrings = 4,
            getTelemetryServer = 5,
            getTickerServer = 6,
            preAuth = 7,
            postAuth = 8,
            userSettingsLoad = 10,
            userSettingsSave = 11,
            userSettingsLoadAll = 12,
            userSettingsLoadAllForUserId = 13,
            filterForProfanity = 20,
            fetchQosConfig = 21,
            setClientMetrics = 22,
            setConnectionState = 23,
        }
        
        public enum UtilComponentNotification : ushort
        {
        }
        
    }
}
