using Blaze2SDK.Blaze.GameReporting;
using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class GameReportingComponentBase
    {
        public const ushort Id = 12;
        public const string Name = "GameReportingComponent";
        
        public class Server : BlazeServerComponent<GameReportingComponentCommand, GameReportingComponentNotification, Blaze2RpcError>
        {
            public Server() : base(GameReportingComponentBase.Id, GameReportingComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitGameReport)]
            public virtual Task<NullStruct> SubmitGameReportAsync(GameReport request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitOfflineGameReport)]
            public virtual Task<NullStruct> SubmitOfflineGameReportAsync(GameReport request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitGameEvents)]
            public virtual Task<NullStruct> SubmitGameEventsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReports)]
            public virtual Task<NullStruct> GetGameReportsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportView)]
            public virtual Task<NullStruct> GetGameReportViewAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportViewInfo)]
            public virtual Task<NullStruct> GetGameReportViewInfoAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportViewInfoList)]
            public virtual Task<NullStruct> GetGameReportViewInfoListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportTypes)]
            public virtual Task<NullStruct> GetGameReportTypesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitTrustedMidGameReport)]
            public virtual Task<NullStruct> SubmitTrustedMidGameReportAsync(GameReport request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitTrustedEndGameReport)]
            public virtual Task<NullStruct> SubmitTrustedEndGameReportAsync(GameReport request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
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
        
        public class Client : BlazeClientComponent<GameReportingComponentCommand, GameReportingComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(GameReportingComponentBase.Id, GameReportingComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct SubmitGameReport(GameReport request)
            {
                return Connection.SendRequest<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameReport, request);
            }
            public Task<NullStruct> SubmitGameReportAsync(GameReport request)
            {
                return Connection.SendRequestAsync<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameReport, request);
            }
            
            public NullStruct SubmitOfflineGameReport(GameReport request)
            {
                return Connection.SendRequest<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitOfflineGameReport, request);
            }
            public Task<NullStruct> SubmitOfflineGameReportAsync(GameReport request)
            {
                return Connection.SendRequestAsync<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitOfflineGameReport, request);
            }
            
            public NullStruct SubmitGameEvents()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameEvents, new NullStruct());
            }
            public Task<NullStruct> SubmitGameEventsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameEvents, new NullStruct());
            }
            
            public NullStruct GetGameReports()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReports, new NullStruct());
            }
            public Task<NullStruct> GetGameReportsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReports, new NullStruct());
            }
            
            public NullStruct GetGameReportView()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportView, new NullStruct());
            }
            public Task<NullStruct> GetGameReportViewAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportView, new NullStruct());
            }
            
            public NullStruct GetGameReportViewInfo()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfo, new NullStruct());
            }
            public Task<NullStruct> GetGameReportViewInfoAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfo, new NullStruct());
            }
            
            public NullStruct GetGameReportViewInfoList()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfoList, new NullStruct());
            }
            public Task<NullStruct> GetGameReportViewInfoListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfoList, new NullStruct());
            }
            
            public NullStruct GetGameReportTypes()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportTypes, new NullStruct());
            }
            public Task<NullStruct> GetGameReportTypesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportTypes, new NullStruct());
            }
            
            public NullStruct SubmitTrustedMidGameReport(GameReport request)
            {
                return Connection.SendRequest<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedMidGameReport, request);
            }
            public Task<NullStruct> SubmitTrustedMidGameReportAsync(GameReport request)
            {
                return Connection.SendRequestAsync<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedMidGameReport, request);
            }
            
            public NullStruct SubmitTrustedEndGameReport(GameReport request)
            {
                return Connection.SendRequest<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedEndGameReport, request);
            }
            public Task<NullStruct> SubmitTrustedEndGameReportAsync(GameReport request)
            {
                return Connection.SendRequestAsync<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedEndGameReport, request);
            }
            
            
            [BlazeNotification((ushort)GameReportingComponentNotification.ResultNotification)]
            public virtual Task OnResultNotificationAsync(ResultNotification notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnResultNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameReportingComponentCommand command) => GameReportingComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameReportingComponentNotification notification) => GameReportingComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<GameReportingComponentCommand, GameReportingComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(GameReportingComponentBase.Id, GameReportingComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitGameReport)]
            public virtual Task<NullStruct> SubmitGameReportAsync(GameReport request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameReport, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitOfflineGameReport)]
            public virtual Task<NullStruct> SubmitOfflineGameReportAsync(GameReport request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitOfflineGameReport, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitGameEvents)]
            public virtual Task<NullStruct> SubmitGameEventsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitGameEvents, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReports)]
            public virtual Task<NullStruct> GetGameReportsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReports, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportView)]
            public virtual Task<NullStruct> GetGameReportViewAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportView, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportViewInfo)]
            public virtual Task<NullStruct> GetGameReportViewInfoAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfo, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportViewInfoList)]
            public virtual Task<NullStruct> GetGameReportViewInfoListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportViewInfoList, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.getGameReportTypes)]
            public virtual Task<NullStruct> GetGameReportTypesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.getGameReportTypes, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitTrustedMidGameReport)]
            public virtual Task<NullStruct> SubmitTrustedMidGameReportAsync(GameReport request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedMidGameReport, request);
            }
            
            [BlazeCommand((ushort)GameReportingComponentCommand.submitTrustedEndGameReport)]
            public virtual Task<NullStruct> SubmitTrustedEndGameReportAsync(GameReport request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GameReport, NullStruct, NullStruct>(this, (ushort)GameReportingComponentCommand.submitTrustedEndGameReport, request);
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
            GameReportingComponentCommand.submitGameReport => typeof(GameReport),
            GameReportingComponentCommand.submitOfflineGameReport => typeof(GameReport),
            GameReportingComponentCommand.submitGameEvents => typeof(NullStruct),
            GameReportingComponentCommand.getGameReports => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportView => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportViewInfo => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportViewInfoList => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportTypes => typeof(NullStruct),
            GameReportingComponentCommand.submitTrustedMidGameReport => typeof(GameReport),
            GameReportingComponentCommand.submitTrustedEndGameReport => typeof(GameReport),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(GameReportingComponentCommand command) => command switch
        {
            GameReportingComponentCommand.submitGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitOfflineGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitGameEvents => typeof(NullStruct),
            GameReportingComponentCommand.getGameReports => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportView => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportViewInfo => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportViewInfoList => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportTypes => typeof(NullStruct),
            GameReportingComponentCommand.submitTrustedMidGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitTrustedEndGameReport => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(GameReportingComponentCommand command) => command switch
        {
            GameReportingComponentCommand.submitGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitOfflineGameReport => typeof(NullStruct),
            GameReportingComponentCommand.submitGameEvents => typeof(NullStruct),
            GameReportingComponentCommand.getGameReports => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportView => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportViewInfo => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportViewInfoList => typeof(NullStruct),
            GameReportingComponentCommand.getGameReportTypes => typeof(NullStruct),
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
            getGameReports = 4,
            getGameReportView = 5,
            getGameReportViewInfo = 6,
            getGameReportViewInfoList = 7,
            getGameReportTypes = 8,
            submitTrustedMidGameReport = 100,
            submitTrustedEndGameReport = 101,
        }
        
        public enum GameReportingComponentNotification : ushort
        {
            ResultNotification = 114,
        }
        
    }
}
