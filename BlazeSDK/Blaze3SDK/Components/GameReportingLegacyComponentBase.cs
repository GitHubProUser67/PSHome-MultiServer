using Blaze3SDK.Blaze.GameReportingLegacy;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class GameReportingLegacyComponentBase
    {
        public const ushort Id = 12;
        public const string Name = "GameReportingLegacyComponent";

        public class Server : BlazeServerComponent<GameReportingLegacyComponentCommand, GameReportingLegacyComponentNotification, Blaze3RpcError>
        {
            public Server() : base(GameReportingLegacyComponentBase.Id, GameReportingLegacyComponentBase.Name)
            {

            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.submitGameReport)]
            public virtual Task<NullStruct> SubmitGameReportAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.submitOfflineGameReport)]
            public virtual Task<NullStruct> SubmitOfflineGameReportAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.submitGameEvents)]
            public virtual Task<NullStruct> SubmitGameEventsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.getGameReports)]
            public virtual Task<NullStruct> GetGameReportsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.getGameReportView)]
            public virtual Task<NullStruct> GetGameReportViewAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.getGameReportViewInfo)]
            public virtual Task<NullStruct> GetGameReportViewInfoAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.getGameReportViewInfoList)]
            public virtual Task<NullStruct> GetGameReportViewInfoListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.getGameReportTypes)]
            public virtual Task<NullStruct> GetGameReportTypesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.submitTrustedMidGameReport)]
            public virtual Task<NullStruct> SubmitTrustedMidGameReportAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.submitTrustedEndGameReport)]
            public virtual Task<NullStruct> SubmitTrustedEndGameReportAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }


            public static Task NotifyResultNotificationAsync(BlazeServerConnection connection, ResultNotification notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameReportingLegacyComponentBase.Id, (ushort)GameReportingLegacyComponentNotification.ResultNotification, notification, waitUntilFree);
            }

            public override Type GetCommandRequestType(GameReportingLegacyComponentCommand command) => GameReportingLegacyComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameReportingLegacyComponentCommand command) => GameReportingLegacyComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameReportingLegacyComponentCommand command) => GameReportingLegacyComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameReportingLegacyComponentNotification notification) => GameReportingLegacyComponentBase.GetNotificationType(notification);

        }

        public class Client : BlazeClientComponent<GameReportingLegacyComponentCommand, GameReportingLegacyComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }

            public Client(BlazeClientConnection connection) : base(GameReportingLegacyComponentBase.Id, GameReportingLegacyComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }


            public NullStruct SubmitGameReport()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitGameReport, new NullStruct());
            }
            public Task<NullStruct> SubmitGameReportAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitGameReport, new NullStruct());
            }

            public NullStruct SubmitOfflineGameReport()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitOfflineGameReport, new NullStruct());
            }
            public Task<NullStruct> SubmitOfflineGameReportAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitOfflineGameReport, new NullStruct());
            }

            public NullStruct SubmitGameEvents()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitGameEvents, new NullStruct());
            }
            public Task<NullStruct> SubmitGameEventsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitGameEvents, new NullStruct());
            }

            public NullStruct GetGameReports()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReports, new NullStruct());
            }
            public Task<NullStruct> GetGameReportsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReports, new NullStruct());
            }

            public NullStruct GetGameReportView()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportView, new NullStruct());
            }
            public Task<NullStruct> GetGameReportViewAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportView, new NullStruct());
            }

            public NullStruct GetGameReportViewInfo()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportViewInfo, new NullStruct());
            }
            public Task<NullStruct> GetGameReportViewInfoAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportViewInfo, new NullStruct());
            }

            public NullStruct GetGameReportViewInfoList()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportViewInfoList, new NullStruct());
            }
            public Task<NullStruct> GetGameReportViewInfoListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportViewInfoList, new NullStruct());
            }

            public NullStruct GetGameReportTypes()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportTypes, new NullStruct());
            }
            public Task<NullStruct> GetGameReportTypesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportTypes, new NullStruct());
            }

            public NullStruct SubmitTrustedMidGameReport()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitTrustedMidGameReport, new NullStruct());
            }
            public Task<NullStruct> SubmitTrustedMidGameReportAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitTrustedMidGameReport, new NullStruct());
            }

            public NullStruct SubmitTrustedEndGameReport()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitTrustedEndGameReport, new NullStruct());
            }
            public Task<NullStruct> SubmitTrustedEndGameReportAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitTrustedEndGameReport, new NullStruct());
            }


            [BlazeNotification((ushort)GameReportingLegacyComponentNotification.ResultNotification)]
            public virtual Task OnResultNotificationAsync(ResultNotification notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnResultNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }

            public override Type GetCommandRequestType(GameReportingLegacyComponentCommand command) => GameReportingLegacyComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameReportingLegacyComponentCommand command) => GameReportingLegacyComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameReportingLegacyComponentCommand command) => GameReportingLegacyComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameReportingLegacyComponentNotification notification) => GameReportingLegacyComponentBase.GetNotificationType(notification);

        }

        public class Proxy : BlazeProxyComponent<GameReportingLegacyComponentCommand, GameReportingLegacyComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(GameReportingLegacyComponentBase.Id, GameReportingLegacyComponentBase.Name)
            {

            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.submitGameReport)]
            public virtual Task<NullStruct> SubmitGameReportAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitGameReport, request);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.submitOfflineGameReport)]
            public virtual Task<NullStruct> SubmitOfflineGameReportAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitOfflineGameReport, request);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.submitGameEvents)]
            public virtual Task<NullStruct> SubmitGameEventsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitGameEvents, request);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.getGameReports)]
            public virtual Task<NullStruct> GetGameReportsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReports, request);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.getGameReportView)]
            public virtual Task<NullStruct> GetGameReportViewAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportView, request);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.getGameReportViewInfo)]
            public virtual Task<NullStruct> GetGameReportViewInfoAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportViewInfo, request);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.getGameReportViewInfoList)]
            public virtual Task<NullStruct> GetGameReportViewInfoListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportViewInfoList, request);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.getGameReportTypes)]
            public virtual Task<NullStruct> GetGameReportTypesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.getGameReportTypes, request);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.submitTrustedMidGameReport)]
            public virtual Task<NullStruct> SubmitTrustedMidGameReportAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitTrustedMidGameReport, request);
            }

            [BlazeCommand((ushort)GameReportingLegacyComponentCommand.submitTrustedEndGameReport)]
            public virtual Task<NullStruct> SubmitTrustedEndGameReportAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameReportingLegacyComponentCommand.submitTrustedEndGameReport, request);
            }


            [BlazeNotification((ushort)GameReportingLegacyComponentNotification.ResultNotification)]
            public virtual Task<ResultNotification> OnResultNotificationAsync(ResultNotification notification)
            {
                return Task.FromResult(notification);
            }

            public override Type GetCommandRequestType(GameReportingLegacyComponentCommand command) => GameReportingLegacyComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameReportingLegacyComponentCommand command) => GameReportingLegacyComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameReportingLegacyComponentCommand command) => GameReportingLegacyComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameReportingLegacyComponentNotification notification) => GameReportingLegacyComponentBase.GetNotificationType(notification);

        }

        public static Type GetCommandRequestType(GameReportingLegacyComponentCommand command) => command switch
        {
            GameReportingLegacyComponentCommand.submitGameReport => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitOfflineGameReport => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitGameEvents => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReports => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportView => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportViewInfo => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportViewInfoList => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportTypes => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitTrustedMidGameReport => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitTrustedEndGameReport => typeof(NullStruct),
            _ => typeof(NullStruct)
        };

        public static Type GetCommandResponseType(GameReportingLegacyComponentCommand command) => command switch
        {
            GameReportingLegacyComponentCommand.submitGameReport => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitOfflineGameReport => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitGameEvents => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReports => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportView => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportViewInfo => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportViewInfoList => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportTypes => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitTrustedMidGameReport => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitTrustedEndGameReport => typeof(NullStruct),
            _ => typeof(NullStruct)
        };

        public static Type GetCommandErrorResponseType(GameReportingLegacyComponentCommand command) => command switch
        {
            GameReportingLegacyComponentCommand.submitGameReport => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitOfflineGameReport => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitGameEvents => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReports => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportView => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportViewInfo => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportViewInfoList => typeof(NullStruct),
            GameReportingLegacyComponentCommand.getGameReportTypes => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitTrustedMidGameReport => typeof(NullStruct),
            GameReportingLegacyComponentCommand.submitTrustedEndGameReport => typeof(NullStruct),
            _ => typeof(NullStruct)
        };

        public static Type GetNotificationType(GameReportingLegacyComponentNotification notification) => notification switch
        {
            GameReportingLegacyComponentNotification.ResultNotification => typeof(ResultNotification),
            _ => typeof(NullStruct)
        };

        public enum GameReportingLegacyComponentCommand : ushort
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

        public enum GameReportingLegacyComponentNotification : ushort
        {
            ResultNotification = 114,
        }

    }
}
