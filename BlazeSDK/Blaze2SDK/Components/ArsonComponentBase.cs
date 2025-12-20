using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class ArsonComponentBase
    {
        public const ushort Id = 32;
        public const string Name = "ArsonComponent";
        
        public class Server : BlazeServerComponent<ArsonComponentCommand, ArsonComponentNotification, Blaze2RpcError>
        {
            public Server() : base(ArsonComponentBase.Id, ArsonComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.getUserExtendedData)]
            public virtual Task<NullStruct> GetUserExtendedDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.updateUserExtendedData)]
            public virtual Task<NullStruct> UpdateUserExtendedDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.reportTournamentResult)]
            public virtual Task<NullStruct> ReportTournamentResultAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.updateRegistrationGameIncrement)]
            public virtual Task<NullStruct> UpdateRegistrationGameIncrementAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.joinGameByUserList)]
            public virtual Task<NullStruct> JoinGameByUserListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.reconfigure)]
            public virtual Task<NullStruct> ReconfigureAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.getTournamentMemberStatus)]
            public virtual Task<NullStruct> GetTournamentMemberStatusAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.setRoomCategoryClientMetaData)]
            public virtual Task<NullStruct> SetRoomCategoryClientMetaDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.getRoomCategoryClientMetaData)]
            public virtual Task<NullStruct> GetRoomCategoryClientMetaDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.setRoomAttributes)]
            public virtual Task<NullStruct> SetRoomAttributesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.getRoomAttributes)]
            public virtual Task<NullStruct> GetRoomAttributesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.getRoomCategory)]
            public virtual Task<NullStruct> GetRoomCategoryAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.setComponentState)]
            public virtual Task<NullStruct> SetComponentStateAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.addPointsToWallet)]
            public virtual Task<NullStruct> AddPointsToWalletAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyReconfigureCompletedAsync(BlazeServerConnection connection, NullStruct notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(ArsonComponentBase.Id, (ushort)ArsonComponentNotification.NotifyReconfigureCompleted, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(ArsonComponentCommand command) => ArsonComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(ArsonComponentCommand command) => ArsonComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(ArsonComponentCommand command) => ArsonComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(ArsonComponentNotification notification) => ArsonComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<ArsonComponentCommand, ArsonComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(ArsonComponentBase.Id, ArsonComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct GetUserExtendedData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getUserExtendedData, new NullStruct());
            }
            public Task<NullStruct> GetUserExtendedDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getUserExtendedData, new NullStruct());
            }
            
            public NullStruct UpdateUserExtendedData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.updateUserExtendedData, new NullStruct());
            }
            public Task<NullStruct> UpdateUserExtendedDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.updateUserExtendedData, new NullStruct());
            }
            
            public NullStruct ReportTournamentResult()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.reportTournamentResult, new NullStruct());
            }
            public Task<NullStruct> ReportTournamentResultAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.reportTournamentResult, new NullStruct());
            }
            
            public NullStruct UpdateRegistrationGameIncrement()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.updateRegistrationGameIncrement, new NullStruct());
            }
            public Task<NullStruct> UpdateRegistrationGameIncrementAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.updateRegistrationGameIncrement, new NullStruct());
            }
            
            public NullStruct JoinGameByUserList()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.joinGameByUserList, new NullStruct());
            }
            public Task<NullStruct> JoinGameByUserListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.joinGameByUserList, new NullStruct());
            }
            
            public NullStruct Reconfigure()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.reconfigure, new NullStruct());
            }
            public Task<NullStruct> ReconfigureAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.reconfigure, new NullStruct());
            }
            
            public NullStruct GetTournamentMemberStatus()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getTournamentMemberStatus, new NullStruct());
            }
            public Task<NullStruct> GetTournamentMemberStatusAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getTournamentMemberStatus, new NullStruct());
            }
            
            public NullStruct SetRoomCategoryClientMetaData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.setRoomCategoryClientMetaData, new NullStruct());
            }
            public Task<NullStruct> SetRoomCategoryClientMetaDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.setRoomCategoryClientMetaData, new NullStruct());
            }
            
            public NullStruct GetRoomCategoryClientMetaData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getRoomCategoryClientMetaData, new NullStruct());
            }
            public Task<NullStruct> GetRoomCategoryClientMetaDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getRoomCategoryClientMetaData, new NullStruct());
            }
            
            public NullStruct SetRoomAttributes()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.setRoomAttributes, new NullStruct());
            }
            public Task<NullStruct> SetRoomAttributesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.setRoomAttributes, new NullStruct());
            }
            
            public NullStruct GetRoomAttributes()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getRoomAttributes, new NullStruct());
            }
            public Task<NullStruct> GetRoomAttributesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getRoomAttributes, new NullStruct());
            }
            
            public NullStruct GetRoomCategory()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getRoomCategory, new NullStruct());
            }
            public Task<NullStruct> GetRoomCategoryAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getRoomCategory, new NullStruct());
            }
            
            public NullStruct SetComponentState()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.setComponentState, new NullStruct());
            }
            public Task<NullStruct> SetComponentStateAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.setComponentState, new NullStruct());
            }
            
            public NullStruct AddPointsToWallet()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.addPointsToWallet, new NullStruct());
            }
            public Task<NullStruct> AddPointsToWalletAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.addPointsToWallet, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)ArsonComponentNotification.NotifyReconfigureCompleted)]
            public virtual Task OnNotifyReconfigureCompletedAsync()
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyReconfigureCompletedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(ArsonComponentCommand command) => ArsonComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(ArsonComponentCommand command) => ArsonComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(ArsonComponentCommand command) => ArsonComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(ArsonComponentNotification notification) => ArsonComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<ArsonComponentCommand, ArsonComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(ArsonComponentBase.Id, ArsonComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.getUserExtendedData)]
            public virtual Task<NullStruct> GetUserExtendedDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getUserExtendedData, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.updateUserExtendedData)]
            public virtual Task<NullStruct> UpdateUserExtendedDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.updateUserExtendedData, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.reportTournamentResult)]
            public virtual Task<NullStruct> ReportTournamentResultAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.reportTournamentResult, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.updateRegistrationGameIncrement)]
            public virtual Task<NullStruct> UpdateRegistrationGameIncrementAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.updateRegistrationGameIncrement, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.joinGameByUserList)]
            public virtual Task<NullStruct> JoinGameByUserListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.joinGameByUserList, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.reconfigure)]
            public virtual Task<NullStruct> ReconfigureAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.reconfigure, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.getTournamentMemberStatus)]
            public virtual Task<NullStruct> GetTournamentMemberStatusAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getTournamentMemberStatus, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.setRoomCategoryClientMetaData)]
            public virtual Task<NullStruct> SetRoomCategoryClientMetaDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.setRoomCategoryClientMetaData, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.getRoomCategoryClientMetaData)]
            public virtual Task<NullStruct> GetRoomCategoryClientMetaDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getRoomCategoryClientMetaData, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.setRoomAttributes)]
            public virtual Task<NullStruct> SetRoomAttributesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.setRoomAttributes, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.getRoomAttributes)]
            public virtual Task<NullStruct> GetRoomAttributesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getRoomAttributes, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.getRoomCategory)]
            public virtual Task<NullStruct> GetRoomCategoryAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.getRoomCategory, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.setComponentState)]
            public virtual Task<NullStruct> SetComponentStateAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.setComponentState, request);
            }
            
            [BlazeCommand((ushort)ArsonComponentCommand.addPointsToWallet)]
            public virtual Task<NullStruct> AddPointsToWalletAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ArsonComponentCommand.addPointsToWallet, request);
            }
            
            
            [BlazeNotification((ushort)ArsonComponentNotification.NotifyReconfigureCompleted)]
            public virtual Task<NullStruct> OnNotifyReconfigureCompletedAsync(NullStruct notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(ArsonComponentCommand command) => ArsonComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(ArsonComponentCommand command) => ArsonComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(ArsonComponentCommand command) => ArsonComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(ArsonComponentNotification notification) => ArsonComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(ArsonComponentCommand command) => command switch
        {
            ArsonComponentCommand.getUserExtendedData => typeof(NullStruct),
            ArsonComponentCommand.updateUserExtendedData => typeof(NullStruct),
            ArsonComponentCommand.reportTournamentResult => typeof(NullStruct),
            ArsonComponentCommand.updateRegistrationGameIncrement => typeof(NullStruct),
            ArsonComponentCommand.joinGameByUserList => typeof(NullStruct),
            ArsonComponentCommand.reconfigure => typeof(NullStruct),
            ArsonComponentCommand.getTournamentMemberStatus => typeof(NullStruct),
            ArsonComponentCommand.setRoomCategoryClientMetaData => typeof(NullStruct),
            ArsonComponentCommand.getRoomCategoryClientMetaData => typeof(NullStruct),
            ArsonComponentCommand.setRoomAttributes => typeof(NullStruct),
            ArsonComponentCommand.getRoomAttributes => typeof(NullStruct),
            ArsonComponentCommand.getRoomCategory => typeof(NullStruct),
            ArsonComponentCommand.setComponentState => typeof(NullStruct),
            ArsonComponentCommand.addPointsToWallet => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(ArsonComponentCommand command) => command switch
        {
            ArsonComponentCommand.getUserExtendedData => typeof(NullStruct),
            ArsonComponentCommand.updateUserExtendedData => typeof(NullStruct),
            ArsonComponentCommand.reportTournamentResult => typeof(NullStruct),
            ArsonComponentCommand.updateRegistrationGameIncrement => typeof(NullStruct),
            ArsonComponentCommand.joinGameByUserList => typeof(NullStruct),
            ArsonComponentCommand.reconfigure => typeof(NullStruct),
            ArsonComponentCommand.getTournamentMemberStatus => typeof(NullStruct),
            ArsonComponentCommand.setRoomCategoryClientMetaData => typeof(NullStruct),
            ArsonComponentCommand.getRoomCategoryClientMetaData => typeof(NullStruct),
            ArsonComponentCommand.setRoomAttributes => typeof(NullStruct),
            ArsonComponentCommand.getRoomAttributes => typeof(NullStruct),
            ArsonComponentCommand.getRoomCategory => typeof(NullStruct),
            ArsonComponentCommand.setComponentState => typeof(NullStruct),
            ArsonComponentCommand.addPointsToWallet => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(ArsonComponentCommand command) => command switch
        {
            ArsonComponentCommand.getUserExtendedData => typeof(NullStruct),
            ArsonComponentCommand.updateUserExtendedData => typeof(NullStruct),
            ArsonComponentCommand.reportTournamentResult => typeof(NullStruct),
            ArsonComponentCommand.updateRegistrationGameIncrement => typeof(NullStruct),
            ArsonComponentCommand.joinGameByUserList => typeof(NullStruct),
            ArsonComponentCommand.reconfigure => typeof(NullStruct),
            ArsonComponentCommand.getTournamentMemberStatus => typeof(NullStruct),
            ArsonComponentCommand.setRoomCategoryClientMetaData => typeof(NullStruct),
            ArsonComponentCommand.getRoomCategoryClientMetaData => typeof(NullStruct),
            ArsonComponentCommand.setRoomAttributes => typeof(NullStruct),
            ArsonComponentCommand.getRoomAttributes => typeof(NullStruct),
            ArsonComponentCommand.getRoomCategory => typeof(NullStruct),
            ArsonComponentCommand.setComponentState => typeof(NullStruct),
            ArsonComponentCommand.addPointsToWallet => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(ArsonComponentNotification notification) => notification switch
        {
            ArsonComponentNotification.NotifyReconfigureCompleted => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public enum ArsonComponentCommand : ushort
        {
            getUserExtendedData = 1,
            updateUserExtendedData = 2,
            reportTournamentResult = 3,
            updateRegistrationGameIncrement = 4,
            joinGameByUserList = 5,
            reconfigure = 6,
            getTournamentMemberStatus = 7,
            setRoomCategoryClientMetaData = 8,
            getRoomCategoryClientMetaData = 9,
            setRoomAttributes = 10,
            getRoomAttributes = 11,
            getRoomCategory = 12,
            setComponentState = 13,
            addPointsToWallet = 14,
        }
        
        public enum ArsonComponentNotification : ushort
        {
            NotifyReconfigureCompleted = 1,
        }
        
    }
}
