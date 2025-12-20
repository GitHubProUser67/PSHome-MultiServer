using Blaze3SDK.Blaze;
using Blaze3SDK.Blaze.Util;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class UtilComponentBase
    {
        public const ushort Id = 9;
        public const string Name = "UtilComponent";

        public class Server : BlazeServerComponent<UtilComponentCommand, UtilComponentNotification, Blaze3RpcError>
        {
            public Server() : base(UtilComponentBase.Id, UtilComponentBase.Name)
            {

            }

            [BlazeCommand((ushort)UtilComponentCommand.fetchClientConfig)]
            public virtual Task<FetchConfigResponse> FetchClientConfigAsync(FetchClientConfigRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.ping)]
            public virtual Task<PingResponse> PingAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.setClientData)]
            public virtual Task<NullStruct> SetClientDataAsync(ClientData request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.localizeStrings)]
            public virtual Task<LocalizeStringsResponse> LocalizeStringsAsync(LocalizeStringsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.getTelemetryServer)]
            public virtual Task<GetTelemetryServerResponse> GetTelemetryServerAsync(GetTelemetryServerRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.getTickerServer)]
            public virtual Task<GetTickerServerResponse> GetTickerServerAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.preAuth)]
            public virtual Task<PreAuthResponse> PreAuthAsync(PreAuthRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.postAuth)]
            public virtual Task<PostAuthResponse> PostAuthAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.userSettingsLoad)]
            public virtual Task<UserSettingsResponse> UserSettingsLoadAsync(UserSettingsLoadRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.userSettingsSave)]
            public virtual Task<NullStruct> UserSettingsSaveAsync(UserSettingsSaveRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.userSettingsLoadAll)]
            public virtual Task<UserSettingsLoadAllResponse> UserSettingsLoadAllAsync(UserSettingsLoadAllRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.deleteUserSettings)]
            public virtual Task<NullStruct> DeleteUserSettingsAsync(DeleteUserSettingsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.filterForProfanity)]
            public virtual Task<FilterUserTextResponse> FilterForProfanityAsync(FilterUserTextResponse request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.fetchQosConfig)]
            public virtual Task<QosConfigInfo> FetchQosConfigAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.setClientMetrics)]
            public virtual Task<NullStruct> SetClientMetricsAsync(ClientMetrics request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.setConnectionState)]
            public virtual Task<NullStruct> SetConnectionStateAsync(SetConnectionStateRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.getPssConfig)]
            public virtual Task<PssConfig> GetPssConfigAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.getUserOptions)]
            public virtual Task<UserOptions> GetUserOptionsAsync(GetUserOptionsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.setUserOptions)]
            public virtual Task<NullStruct> SetUserOptionsAsync(UserOptions request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)UtilComponentCommand.suspendUserPing)]
            public virtual Task<NullStruct> SuspendUserPingAsync(SuspendUserPingRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }


            public override Type GetCommandRequestType(UtilComponentCommand command) => UtilComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(UtilComponentCommand command) => UtilComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(UtilComponentCommand command) => UtilComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(UtilComponentNotification notification) => UtilComponentBase.GetNotificationType(notification);

        }

        public class Client : BlazeClientComponent<UtilComponentCommand, UtilComponentNotification, Blaze3RpcError>
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

            public NullStruct SetClientData(ClientData request)
            {
                return Connection.SendRequest<ClientData, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientData, request);
            }
            public Task<NullStruct> SetClientDataAsync(ClientData request)
            {
                return Connection.SendRequestAsync<ClientData, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientData, request);
            }

            public LocalizeStringsResponse LocalizeStrings(LocalizeStringsRequest request)
            {
                return Connection.SendRequest<LocalizeStringsRequest, LocalizeStringsResponse, NullStruct>(this, (ushort)UtilComponentCommand.localizeStrings, request);
            }
            public Task<LocalizeStringsResponse> LocalizeStringsAsync(LocalizeStringsRequest request)
            {
                return Connection.SendRequestAsync<LocalizeStringsRequest, LocalizeStringsResponse, NullStruct>(this, (ushort)UtilComponentCommand.localizeStrings, request);
            }

            public GetTelemetryServerResponse GetTelemetryServer(GetTelemetryServerRequest request)
            {
                return Connection.SendRequest<GetTelemetryServerRequest, GetTelemetryServerResponse, NullStruct>(this, (ushort)UtilComponentCommand.getTelemetryServer, request);
            }
            public Task<GetTelemetryServerResponse> GetTelemetryServerAsync(GetTelemetryServerRequest request)
            {
                return Connection.SendRequestAsync<GetTelemetryServerRequest, GetTelemetryServerResponse, NullStruct>(this, (ushort)UtilComponentCommand.getTelemetryServer, request);
            }

            public GetTickerServerResponse GetTickerServer()
            {
                return Connection.SendRequest<NullStruct, GetTickerServerResponse, NullStruct>(this, (ushort)UtilComponentCommand.getTickerServer, new NullStruct());
            }
            public Task<GetTickerServerResponse> GetTickerServerAsync()
            {
                return Connection.SendRequestAsync<NullStruct, GetTickerServerResponse, NullStruct>(this, (ushort)UtilComponentCommand.getTickerServer, new NullStruct());
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

            public UserSettingsResponse UserSettingsLoad(UserSettingsLoadRequest request)
            {
                return Connection.SendRequest<UserSettingsLoadRequest, UserSettingsResponse, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoad, request);
            }
            public Task<UserSettingsResponse> UserSettingsLoadAsync(UserSettingsLoadRequest request)
            {
                return Connection.SendRequestAsync<UserSettingsLoadRequest, UserSettingsResponse, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoad, request);
            }

            public NullStruct UserSettingsSave(UserSettingsSaveRequest request)
            {
                return Connection.SendRequest<UserSettingsSaveRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsSave, request);
            }
            public Task<NullStruct> UserSettingsSaveAsync(UserSettingsSaveRequest request)
            {
                return Connection.SendRequestAsync<UserSettingsSaveRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsSave, request);
            }

            public UserSettingsLoadAllResponse UserSettingsLoadAll(UserSettingsLoadAllRequest request)
            {
                return Connection.SendRequest<UserSettingsLoadAllRequest, UserSettingsLoadAllResponse, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoadAll, request);
            }
            public Task<UserSettingsLoadAllResponse> UserSettingsLoadAllAsync(UserSettingsLoadAllRequest request)
            {
                return Connection.SendRequestAsync<UserSettingsLoadAllRequest, UserSettingsLoadAllResponse, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoadAll, request);
            }

            public NullStruct DeleteUserSettings(DeleteUserSettingsRequest request)
            {
                return Connection.SendRequest<DeleteUserSettingsRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.deleteUserSettings, request);
            }
            public Task<NullStruct> DeleteUserSettingsAsync(DeleteUserSettingsRequest request)
            {
                return Connection.SendRequestAsync<DeleteUserSettingsRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.deleteUserSettings, request);
            }

            public FilterUserTextResponse FilterForProfanity(FilterUserTextResponse request)
            {
                return Connection.SendRequest<FilterUserTextResponse, FilterUserTextResponse, NullStruct>(this, (ushort)UtilComponentCommand.filterForProfanity, request);
            }
            public Task<FilterUserTextResponse> FilterForProfanityAsync(FilterUserTextResponse request)
            {
                return Connection.SendRequestAsync<FilterUserTextResponse, FilterUserTextResponse, NullStruct>(this, (ushort)UtilComponentCommand.filterForProfanity, request);
            }

            public QosConfigInfo FetchQosConfig()
            {
                return Connection.SendRequest<NullStruct, QosConfigInfo, NullStruct>(this, (ushort)UtilComponentCommand.fetchQosConfig, new NullStruct());
            }
            public Task<QosConfigInfo> FetchQosConfigAsync()
            {
                return Connection.SendRequestAsync<NullStruct, QosConfigInfo, NullStruct>(this, (ushort)UtilComponentCommand.fetchQosConfig, new NullStruct());
            }

            public NullStruct SetClientMetrics(ClientMetrics request)
            {
                return Connection.SendRequest<ClientMetrics, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientMetrics, request);
            }
            public Task<NullStruct> SetClientMetricsAsync(ClientMetrics request)
            {
                return Connection.SendRequestAsync<ClientMetrics, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientMetrics, request);
            }

            public NullStruct SetConnectionState(SetConnectionStateRequest request)
            {
                return Connection.SendRequest<SetConnectionStateRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setConnectionState, request);
            }
            public Task<NullStruct> SetConnectionStateAsync(SetConnectionStateRequest request)
            {
                return Connection.SendRequestAsync<SetConnectionStateRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setConnectionState, request);
            }

            public PssConfig GetPssConfig()
            {
                return Connection.SendRequest<NullStruct, PssConfig, NullStruct>(this, (ushort)UtilComponentCommand.getPssConfig, new NullStruct());
            }
            public Task<PssConfig> GetPssConfigAsync()
            {
                return Connection.SendRequestAsync<NullStruct, PssConfig, NullStruct>(this, (ushort)UtilComponentCommand.getPssConfig, new NullStruct());
            }

            public UserOptions GetUserOptions(GetUserOptionsRequest request)
            {
                return Connection.SendRequest<GetUserOptionsRequest, UserOptions, NullStruct>(this, (ushort)UtilComponentCommand.getUserOptions, request);
            }
            public Task<UserOptions> GetUserOptionsAsync(GetUserOptionsRequest request)
            {
                return Connection.SendRequestAsync<GetUserOptionsRequest, UserOptions, NullStruct>(this, (ushort)UtilComponentCommand.getUserOptions, request);
            }

            public NullStruct SetUserOptions(UserOptions request)
            {
                return Connection.SendRequest<UserOptions, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setUserOptions, request);
            }
            public Task<NullStruct> SetUserOptionsAsync(UserOptions request)
            {
                return Connection.SendRequestAsync<UserOptions, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setUserOptions, request);
            }

            public NullStruct SuspendUserPing(SuspendUserPingRequest request)
            {
                return Connection.SendRequest<SuspendUserPingRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.suspendUserPing, request);
            }
            public Task<NullStruct> SuspendUserPingAsync(SuspendUserPingRequest request)
            {
                return Connection.SendRequestAsync<SuspendUserPingRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.suspendUserPing, request);
            }


            public override Type GetCommandRequestType(UtilComponentCommand command) => UtilComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(UtilComponentCommand command) => UtilComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(UtilComponentCommand command) => UtilComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(UtilComponentNotification notification) => UtilComponentBase.GetNotificationType(notification);

        }

        public class Proxy : BlazeProxyComponent<UtilComponentCommand, UtilComponentNotification, Blaze3RpcError>
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
            public virtual Task<NullStruct> SetClientDataAsync(ClientData request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ClientData, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientData, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.localizeStrings)]
            public virtual Task<LocalizeStringsResponse> LocalizeStringsAsync(LocalizeStringsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<LocalizeStringsRequest, LocalizeStringsResponse, NullStruct>(this, (ushort)UtilComponentCommand.localizeStrings, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.getTelemetryServer)]
            public virtual Task<GetTelemetryServerResponse> GetTelemetryServerAsync(GetTelemetryServerRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetTelemetryServerRequest, GetTelemetryServerResponse, NullStruct>(this, (ushort)UtilComponentCommand.getTelemetryServer, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.getTickerServer)]
            public virtual Task<GetTickerServerResponse> GetTickerServerAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, GetTickerServerResponse, NullStruct>(this, (ushort)UtilComponentCommand.getTickerServer, request);
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
            public virtual Task<UserSettingsResponse> UserSettingsLoadAsync(UserSettingsLoadRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UserSettingsLoadRequest, UserSettingsResponse, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoad, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.userSettingsSave)]
            public virtual Task<NullStruct> UserSettingsSaveAsync(UserSettingsSaveRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UserSettingsSaveRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsSave, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.userSettingsLoadAll)]
            public virtual Task<UserSettingsLoadAllResponse> UserSettingsLoadAllAsync(UserSettingsLoadAllRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UserSettingsLoadAllRequest, UserSettingsLoadAllResponse, NullStruct>(this, (ushort)UtilComponentCommand.userSettingsLoadAll, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.deleteUserSettings)]
            public virtual Task<NullStruct> DeleteUserSettingsAsync(DeleteUserSettingsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<DeleteUserSettingsRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.deleteUserSettings, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.filterForProfanity)]
            public virtual Task<FilterUserTextResponse> FilterForProfanityAsync(FilterUserTextResponse request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<FilterUserTextResponse, FilterUserTextResponse, NullStruct>(this, (ushort)UtilComponentCommand.filterForProfanity, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.fetchQosConfig)]
            public virtual Task<QosConfigInfo> FetchQosConfigAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, QosConfigInfo, NullStruct>(this, (ushort)UtilComponentCommand.fetchQosConfig, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.setClientMetrics)]
            public virtual Task<NullStruct> SetClientMetricsAsync(ClientMetrics request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ClientMetrics, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setClientMetrics, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.setConnectionState)]
            public virtual Task<NullStruct> SetConnectionStateAsync(SetConnectionStateRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SetConnectionStateRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setConnectionState, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.getPssConfig)]
            public virtual Task<PssConfig> GetPssConfigAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, PssConfig, NullStruct>(this, (ushort)UtilComponentCommand.getPssConfig, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.getUserOptions)]
            public virtual Task<UserOptions> GetUserOptionsAsync(GetUserOptionsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetUserOptionsRequest, UserOptions, NullStruct>(this, (ushort)UtilComponentCommand.getUserOptions, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.setUserOptions)]
            public virtual Task<NullStruct> SetUserOptionsAsync(UserOptions request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UserOptions, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.setUserOptions, request);
            }

            [BlazeCommand((ushort)UtilComponentCommand.suspendUserPing)]
            public virtual Task<NullStruct> SuspendUserPingAsync(SuspendUserPingRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SuspendUserPingRequest, NullStruct, NullStruct>(this, (ushort)UtilComponentCommand.suspendUserPing, request);
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
            UtilComponentCommand.setClientData => typeof(ClientData),
            UtilComponentCommand.localizeStrings => typeof(LocalizeStringsRequest),
            UtilComponentCommand.getTelemetryServer => typeof(GetTelemetryServerRequest),
            UtilComponentCommand.getTickerServer => typeof(NullStruct),
            UtilComponentCommand.preAuth => typeof(PreAuthRequest),
            UtilComponentCommand.postAuth => typeof(NullStruct),
            UtilComponentCommand.userSettingsLoad => typeof(UserSettingsLoadRequest),
            UtilComponentCommand.userSettingsSave => typeof(UserSettingsSaveRequest),
            UtilComponentCommand.userSettingsLoadAll => typeof(UserSettingsLoadAllRequest),
            UtilComponentCommand.deleteUserSettings => typeof(DeleteUserSettingsRequest),
            UtilComponentCommand.filterForProfanity => typeof(FilterUserTextResponse),
            UtilComponentCommand.fetchQosConfig => typeof(NullStruct),
            UtilComponentCommand.setClientMetrics => typeof(ClientMetrics),
            UtilComponentCommand.setConnectionState => typeof(SetConnectionStateRequest),
            UtilComponentCommand.getPssConfig => typeof(NullStruct),
            UtilComponentCommand.getUserOptions => typeof(GetUserOptionsRequest),
            UtilComponentCommand.setUserOptions => typeof(UserOptions),
            UtilComponentCommand.suspendUserPing => typeof(SuspendUserPingRequest),
            _ => typeof(NullStruct)
        };

        public static Type GetCommandResponseType(UtilComponentCommand command) => command switch
        {
            UtilComponentCommand.fetchClientConfig => typeof(FetchConfigResponse),
            UtilComponentCommand.ping => typeof(PingResponse),
            UtilComponentCommand.setClientData => typeof(NullStruct),
            UtilComponentCommand.localizeStrings => typeof(LocalizeStringsResponse),
            UtilComponentCommand.getTelemetryServer => typeof(GetTelemetryServerResponse),
            UtilComponentCommand.getTickerServer => typeof(GetTickerServerResponse),
            UtilComponentCommand.preAuth => typeof(PreAuthResponse),
            UtilComponentCommand.postAuth => typeof(PostAuthResponse),
            UtilComponentCommand.userSettingsLoad => typeof(UserSettingsResponse),
            UtilComponentCommand.userSettingsSave => typeof(NullStruct),
            UtilComponentCommand.userSettingsLoadAll => typeof(UserSettingsLoadAllResponse),
            UtilComponentCommand.deleteUserSettings => typeof(NullStruct),
            UtilComponentCommand.filterForProfanity => typeof(FilterUserTextResponse),
            UtilComponentCommand.fetchQosConfig => typeof(QosConfigInfo),
            UtilComponentCommand.setClientMetrics => typeof(NullStruct),
            UtilComponentCommand.setConnectionState => typeof(NullStruct),
            UtilComponentCommand.getPssConfig => typeof(PssConfig),
            UtilComponentCommand.getUserOptions => typeof(UserOptions),
            UtilComponentCommand.setUserOptions => typeof(NullStruct),
            UtilComponentCommand.suspendUserPing => typeof(NullStruct),
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
            UtilComponentCommand.deleteUserSettings => typeof(NullStruct),
            UtilComponentCommand.filterForProfanity => typeof(NullStruct),
            UtilComponentCommand.fetchQosConfig => typeof(NullStruct),
            UtilComponentCommand.setClientMetrics => typeof(NullStruct),
            UtilComponentCommand.setConnectionState => typeof(NullStruct),
            UtilComponentCommand.getPssConfig => typeof(NullStruct),
            UtilComponentCommand.getUserOptions => typeof(NullStruct),
            UtilComponentCommand.setUserOptions => typeof(NullStruct),
            UtilComponentCommand.suspendUserPing => typeof(NullStruct),
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
            deleteUserSettings = 14,
            filterForProfanity = 20,
            fetchQosConfig = 21,
            setClientMetrics = 22,
            setConnectionState = 23,
            getPssConfig = 24,
            getUserOptions = 25,
            setUserOptions = 26,
            suspendUserPing = 27,
        }

        public enum UtilComponentNotification : ushort
        {
        }

    }
}
