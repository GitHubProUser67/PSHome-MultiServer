using Blaze3SDK.Blaze.GameReporting;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class GameReportingComponentBase
    {
        public const ushort Id = 28;
        public const string Name = "GameReportingComponent";
        
        public class Server : BlazeServerComponent<GameReportingComponentCommand, GameReportingComponentNotification, Blaze3RpcError>
        {
            public Server() : base(GameReportingComponentBase.Id, GameReportingComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitGameReport)]
            public virtual Task<NullStruct> SubmitGameReportAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitOfflineGameReport)]
            public virtual Task<NullStruct> SubmitOfflineGameReportAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitGameEvents)]
            public virtual Task<NullStruct> SubmitGameEventsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportQuery)]
            public virtual Task<NullStruct> GetGameReportQueryAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportQueriesList)]
            public virtual Task<GameReportQueriesList> GetGameReportQueriesListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReports)]
            public virtual Task<GameReportsList> GetGameReportsAsync(GetGameReports request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportView)]
            public virtual Task<GetGameReportViewResponse> GetGameReportViewAsync(GetGameReportViewRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportViewInfo)]
            public virtual Task<GameReportViewInfo> GetGameReportViewInfoAsync(GetGameReportViewInfo request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportViewInfoList)]
            public virtual Task<GameReportViewInfosList> GetGameReportViewInfoListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportTypes)]
            public virtual Task<GetGameReportTypesResponse> GetGameReportTypesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.updateMetric)]
            public virtual Task<NullStruct> UpdateMetricAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportColumnInfo)]
            public virtual Task<GameReportColumnInfoResponse> GetGameReportColumnInfoAsync(GetGameReportColumnInfo request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportColumnValues)]
            public virtual Task<GetGameReportColumnValuesResponse> GetGameReportColumnValuesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitTrustedMidGameReport)]
            public virtual Task<NullStruct> SubmitTrustedMidGameReportAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitTrustedEndGameReport)]
            public virtual Task<NullStruct> SubmitTrustedEndGameReportAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyResultNotificationAsync(BlazeServerConnection connection, ResultNotification notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameReportingComponentBase.Id, (ushort)GameReportingComponentNotification.ResultNotification, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameReportingComponentNotification notification) => GameReportingComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<GameReportingComponentCommand, GameReportingComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(GameReportingComponentBase.Id, GameReportingComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct SubmitGameReport()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameReport, new NullStruct());
            }
            public Task<NullStruct> SubmitGameReportAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameReport, new NullStruct());
            }
            
            public NullStruct SubmitOfflineGameReport()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitOfflineGameReport, new NullStruct());
            }
            public Task<NullStruct> SubmitOfflineGameReportAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitOfflineGameReport, new NullStruct());
            }
            
            public NullStruct SubmitGameEvents()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameEvents, new NullStruct());
            }
            public Task<NullStruct> SubmitGameEventsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameEvents, new NullStruct());
            }
            
            public NullStruct GetGameReportQuery()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportQuery, new NullStruct());
            }
            public Task<NullStruct> GetGameReportQueryAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportQuery, new NullStruct());
            }
            
            public GameReportQueriesList GetGameReportQueriesList()
            {
                return Connection.SendRequest<NullStruct, GameReportQueriesList, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportQueriesList, new NullStruct());
            }
            public Task<GameReportQueriesList> GetGameReportQueriesListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, GameReportQueriesList, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportQueriesList, new NullStruct());
            }
            
            public GameReportsList GetGameReports(GetGameReports request)
            {
                return Connection.SendRequest<GetGameReports, GameReportsList, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReports, request);
            }
            public Task<GameReportsList> GetGameReportsAsync(GetGameReports request)
            {
                return Connection.SendRequestAsync<GetGameReports, GameReportsList, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReports, request);
            }
            
            public GetGameReportViewResponse GetGameReportView(GetGameReportViewRequest request)
            {
                return Connection.SendRequest<GetGameReportViewRequest, GetGameReportViewResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportView, request);
            }
            public Task<GetGameReportViewResponse> GetGameReportViewAsync(GetGameReportViewRequest request)
            {
                return Connection.SendRequestAsync<GetGameReportViewRequest, GetGameReportViewResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportView, request);
            }
            
            public GameReportViewInfo GetGameReportViewInfo(GetGameReportViewInfo request)
            {
                return Connection.SendRequest<GetGameReportViewInfo, GameReportViewInfo, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfo, request);
            }
            public Task<GameReportViewInfo> GetGameReportViewInfoAsync(GetGameReportViewInfo request)
            {
                return Connection.SendRequestAsync<GetGameReportViewInfo, GameReportViewInfo, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfo, request);
            }
            
            public GameReportViewInfosList GetGameReportViewInfoList()
            {
                return Connection.SendRequest<NullStruct, GameReportViewInfosList, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfoList, new NullStruct());
            }
            public Task<GameReportViewInfosList> GetGameReportViewInfoListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, GameReportViewInfosList, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfoList, new NullStruct());
            }
            
            public GetGameReportTypesResponse GetGameReportTypes()
            {
                return Connection.SendRequest<NullStruct, GetGameReportTypesResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportTypes, new NullStruct());
            }
            public Task<GetGameReportTypesResponse> GetGameReportTypesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, GetGameReportTypesResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportTypes, new NullStruct());
            }
            
            public NullStruct UpdateMetric()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.updateMetric, new NullStruct());
            }
            public Task<NullStruct> UpdateMetricAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.updateMetric, new NullStruct());
            }
            
            public GameReportColumnInfoResponse GetGameReportColumnInfo(GetGameReportColumnInfo request)
            {
                return Connection.SendRequest<GetGameReportColumnInfo, GameReportColumnInfoResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportColumnInfo, request);
            }
            public Task<GameReportColumnInfoResponse> GetGameReportColumnInfoAsync(GetGameReportColumnInfo request)
            {
                return Connection.SendRequestAsync<GetGameReportColumnInfo, GameReportColumnInfoResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportColumnInfo, request);
            }
            
            public GetGameReportColumnValuesResponse GetGameReportColumnValues()
            {
                return Connection.SendRequest<NullStruct, GetGameReportColumnValuesResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportColumnValues, new NullStruct());
            }
            public Task<GetGameReportColumnValuesResponse> GetGameReportColumnValuesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, GetGameReportColumnValuesResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportColumnValues, new NullStruct());
            }
            
            public NullStruct SubmitTrustedMidGameReport()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedMidGameReport, new NullStruct());
            }
            public Task<NullStruct> SubmitTrustedMidGameReportAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedMidGameReport, new NullStruct());
            }
            
            public NullStruct SubmitTrustedEndGameReport()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedEndGameReport, new NullStruct());
            }
            public Task<NullStruct> SubmitTrustedEndGameReportAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedEndGameReport, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)GameReportingComponentNotification.ResultNotification)]
            public virtual Task OnResultNotificationAsync(ResultNotification notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnResultNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameReportingComponentNotification notification) => GameReportingComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<GameReportingComponentCommand, GameReportingComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(GameReportingComponentBase.Id, GameReportingComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitGameReport)]
            public virtual Task<NullStruct> SubmitGameReportAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameReport, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitOfflineGameReport)]
            public virtual Task<NullStruct> SubmitOfflineGameReportAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitOfflineGameReport, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitGameEvents)]
            public virtual Task<NullStruct> SubmitGameEventsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameEvents, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportQuery)]
            public virtual Task<NullStruct> GetGameReportQueryAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportQuery, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportQueriesList)]
            public virtual Task<GameReportQueriesList> GetGameReportQueriesListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, GameReportQueriesList, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportQueriesList, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReports)]
            public virtual Task<GameReportsList> GetGameReportsAsync(GetGameReports request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetGameReports, GameReportsList, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReports, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportView)]
            public virtual Task<GetGameReportViewResponse> GetGameReportViewAsync(GetGameReportViewRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetGameReportViewRequest, GetGameReportViewResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportView, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportViewInfo)]
            public virtual Task<GameReportViewInfo> GetGameReportViewInfoAsync(GetGameReportViewInfo request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetGameReportViewInfo, GameReportViewInfo, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfo, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportViewInfoList)]
            public virtual Task<GameReportViewInfosList> GetGameReportViewInfoListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, GameReportViewInfosList, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfoList, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportTypes)]
            public virtual Task<GetGameReportTypesResponse> GetGameReportTypesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, GetGameReportTypesResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportTypes, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.updateMetric)]
            public virtual Task<NullStruct> UpdateMetricAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.updateMetric, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportColumnInfo)]
            public virtual Task<GameReportColumnInfoResponse> GetGameReportColumnInfoAsync(GetGameReportColumnInfo request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetGameReportColumnInfo, GameReportColumnInfoResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportColumnInfo, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportColumnValues)]
            public virtual Task<GetGameReportColumnValuesResponse> GetGameReportColumnValuesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, GetGameReportColumnValuesResponse, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportColumnValues, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitTrustedMidGameReport)]
            public virtual Task<NullStruct> SubmitTrustedMidGameReportAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedMidGameReport, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitTrustedEndGameReport)]
            public virtual Task<NullStruct> SubmitTrustedEndGameReportAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedEndGameReport, request);
            }
            
            
            [BlazeNotification((ushort)GameReportingComponentNotification.ResultNotification)]
            public virtual Task<ResultNotification> OnResultNotificationAsync(ResultNotification notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameReportingComponentNotification notification) => GameReportingComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(GameReportingComponentCommand command) => command switch
        {
            GameReportingComponentCommand.submitGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitOfflineGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitGameEvents => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportQuery => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportQueriesList => typeof(NullStruct),
            GameReportingComponentCommand.getGameReports => typeof(GetGameReports),
            GameReportingComponentCommand.getGameReportView => typeof(GetGameReportViewRequest),
            GameReportingComponentCommand.getGameReportViewInfo => typeof(GetGameReportViewInfo),
            GameReportingComponentCommand.getGameReportViewInfoList => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportTypes => typeof(NullStruct),
            GameReportingComponentCommand.updateMetric => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportColumnInfo => typeof(GetGameReportColumnInfo),
            GameReportingComponentCommand.getGameReportColumnValues => typeof(NullStruct),
            GameReportingComponentCommand.submitTrustedMidGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitTrustedEndGameReport => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(GameReportingComponentCommand command) => command switch
        {
            GameReportingComponentCommand.submitGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitOfflineGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitGameEvents => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportQuery => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportQueriesList => typeof(GameReportQueriesList),
            GameReportingComponentCommand.getGameReports => typeof(GameReportsList),
            GameReportingComponentCommand.getGameReportView => typeof(GetGameReportViewResponse),
            GameReportingComponentCommand.getGameReportViewInfo => typeof(GameReportViewInfo),
            GameReportingComponentCommand.getGameReportViewInfoList => typeof(GameReportViewInfosList),
            GameReportingComponentCommand.getGameReportTypes => typeof(GetGameReportTypesResponse),
            GameReportingComponentCommand.updateMetric => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportColumnInfo => typeof(GameReportColumnInfoResponse),
            GameReportingComponentCommand.getGameReportColumnValues => typeof(GetGameReportColumnValuesResponse),
            GameReportingComponentCommand.submitTrustedMidGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitTrustedEndGameReport => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(GameReportingComponentCommand command) => command switch
        {
            GameReportingComponentCommand.submitGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitOfflineGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitGameEvents => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportQuery => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportQueriesList => typeof(NullStruct),
            GameReportingComponentCommand.getGameReports => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportView => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportViewInfo => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportViewInfoList => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportTypes => typeof(NullStruct),
            GameReportingComponentCommand.updateMetric => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportColumnInfo => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportColumnValues => typeof(NullStruct),
            GameReportingComponentCommand.submitTrustedMidGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitTrustedEndGameReport => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(GameReportingComponentNotification notification) => notification switch
        {
            GameReportingComponentNotification.ResultNotification => typeof(ResultNotification),
            _ => typeof(NullStruct)
        };
        
        public enum GameReportingComponentCommand : ushort
        {
            submitGameReport = 1,
            submitOfflineGameReport = 2,
            submitGameEvents = 3,
            getGameReportQuery = 4,
            getGameReportQueriesList = 5,
            getGameReports = 6,
            getGameReportView = 7,
            getGameReportViewInfo = 8,
            getGameReportViewInfoList = 9,
            getGameReportTypes = 10,
            updateMetric = 11,
            getGameReportColumnInfo = 12,
            getGameReportColumnValues = 13,
            submitTrustedMidGameReport = 100,
            submitTrustedEndGameReport = 101,
        }
        
        public enum GameReportingComponentNotification : ushort
        {
            ResultNotification = 114,
        }
        
    }
}
