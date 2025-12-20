using Blaze3SDK.Blaze.GameManager;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class GameManagerBase
    {
        public const ushort Id = 4;
        public const string Name = "GameManager";
        
        public class Server : BlazeServerComponent<GameManagerCommand, GameManagerNotification, Blaze3RpcError>
        {
            public Server() : base(GameManagerBase.Id, GameManagerBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)GameManagerCommand.createGame)]
            public virtual Task<CreateGameResponse> CreateGameAsync(CreateGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.destroyGame)]
            public virtual Task<DestroyGameResponse> DestroyGameAsync(DestroyGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.advanceGameState)]
            public virtual Task<NullStruct> AdvanceGameStateAsync(AdvanceGameStateRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setGameSettings)]
            public virtual Task<NullStruct> SetGameSettingsAsync(SetGameSettingsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerCapacity)]
            public virtual Task<NullStruct> SetPlayerCapacityAsync(SetPlayerCapacityRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPresenceMode)]
            public virtual Task<NullStruct> SetPresenceModeAsync(SetPresenceModeRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setGameAttributes)]
            public virtual Task<NullStruct> SetGameAttributesAsync(SetGameAttributesRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerAttributes)]
            public virtual Task<NullStruct> SetPlayerAttributesAsync(SetPlayerAttributesRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.joinGame)]
            public virtual Task<JoinGameResponse> JoinGameAsync(JoinGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.removePlayer)]
            public virtual Task<NullStruct> RemovePlayerAsync(RemovePlayerRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.startMatchmaking)]
            public virtual Task<StartMatchmakingResponse> StartMatchmakingAsync(StartMatchmakingRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.cancelMatchmaking)]
            public virtual Task<NullStruct> CancelMatchmakingAsync(CancelMatchmakingRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.finalizeGameCreation)]
            public virtual Task<NullStruct> FinalizeGameCreationAsync(UpdateGameSessionRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.listGames)]
            public virtual Task<ListGamesResponse> ListGamesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerCustomData)]
            public virtual Task<NullStruct> SetPlayerCustomDataAsync(SetPlayerCustomDataRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.replayGame)]
            public virtual Task<NullStruct> ReplayGameAsync(ReplayGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.returnDedicatedServerToPool)]
            public virtual Task<NullStruct> ReturnDedicatedServerToPoolAsync(ReturnDedicatedServerToPoolRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.joinGameByGroup)]
            public virtual Task<NullStruct> JoinGameByGroupAsync(JoinGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.leaveGameByGroup)]
            public virtual Task<NullStruct> LeaveGameByGroupAsync(RemovePlayerRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.migrateGame)]
            public virtual Task<NullStruct> MigrateGameAsync(MigrateHostRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateGameHostMigrationStatus)]
            public virtual Task<NullStruct> UpdateGameHostMigrationStatusAsync(UpdateGameHostMigrationStatusRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.resetDedicatedServer)]
            public virtual Task<NullStruct> ResetDedicatedServerAsync(CreateGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateGameSession)]
            public virtual Task<NullStruct> UpdateGameSessionAsync(UpdateGameSessionRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.banPlayer)]
            public virtual Task<NullStruct> BanPlayerAsync(BanPlayerRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateMeshConnection)]
            public virtual Task<NullStruct> UpdateMeshConnectionAsync(UpdateMeshConnectionRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.removePlayerFromBannedList)]
            public virtual Task<NullStruct> RemovePlayerFromBannedListAsync(RemovePlayerFromBannedListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.clearBannedList)]
            public virtual Task<NullStruct> ClearBannedListAsync(BannedListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getBannedList)]
            public virtual Task<BannedListResponse> GetBannedListAsync(BannedListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.addQueuedPlayerToGame)]
            public virtual Task<NullStruct> AddQueuedPlayerToGameAsync(AddQueuedPlayerToGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateGameName)]
            public virtual Task<NullStruct> UpdateGameNameAsync(UpdateGameNameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.ejectHost)]
            public virtual Task<NullStruct> EjectHostAsync(EjectHostRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameListSnapshot)]
            public virtual Task<GetGameListResponse> GetGameListSnapshotAsync(GetGameListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameListSubscription)]
            public virtual Task<GetGameListResponse> GetGameListSubscriptionAsync(GetGameListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.destroyGameList)]
            public virtual Task<NullStruct> DestroyGameListAsync(DestroyGameListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getFullGameData)]
            public virtual Task<GetFullGameDataResponse> GetFullGameDataAsync(GetFullGameDataRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getMatchmakingConfig)]
            public virtual Task<GetMatchmakingConfigResponse> GetMatchmakingConfigAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameDataFromId)]
            public virtual Task<NullStruct> GetGameDataFromIdAsync(GetGameDataFromIdRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.addAdminPlayer)]
            public virtual Task<NullStruct> AddAdminPlayerAsync(UpdateAdminListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.removeAdminPlayer)]
            public virtual Task<NullStruct> RemoveAdminPlayerAsync(UpdateAdminListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerTeam)]
            public virtual Task<NullStruct> SetPlayerTeamAsync(SetPlayerTeamRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.changeGameTeamId)]
            public virtual Task<NullStruct> ChangeGameTeamIdAsync(ChangeTeamIdRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.migrateAdminPlayer)]
            public virtual Task<NullStruct> MigrateAdminPlayerAsync(UpdateAdminListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getUserSetGameListSubscription)]
            public virtual Task<NullStruct> GetUserSetGameListSubscriptionAsync(GetUserSetGameListSubscriptionRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.swapPlayersTeam)]
            public virtual Task<NullStruct> SwapPlayersTeamAsync(SwapPlayersTeamRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.registerDynamicDedicatedServerCreator)]
            public virtual Task<RegisterDynamicDedicatedServerCreatorResponse> RegisterDynamicDedicatedServerCreatorAsync(RegisterDynamicDedicatedServerCreatorRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.unregisterDynamicDedicatedServerCreator)]
            public virtual Task<UnregisterDynamicDedicatedServerCreatorResponse> UnregisterDynamicDedicatedServerCreatorAsync(UnregisterDynamicDedicatedServerCreatorRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyMatchmakingFailedAsync(BlazeServerConnection connection, NotifyMatchmakingFailed notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyMatchmakingFailed, notification, waitUntilFree);
            }
            
            public static Task NotifyMatchmakingAsyncStatusAsync(BlazeServerConnection connection, NotifyMatchmakingAsyncStatus notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyMatchmakingAsyncStatus, notification, waitUntilFree);
            }
            
            public static Task NotifyGameCreatedAsync(BlazeServerConnection connection, NotifyGameCreated notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameCreated, notification, waitUntilFree);
            }
            
            public static Task NotifyGameRemovedAsync(BlazeServerConnection connection, NotifyGameRemoved notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameRemoved, notification, waitUntilFree);
            }
            
            public static Task NotifyGameSetupAsync(BlazeServerConnection connection, NotifyGameSetup notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameSetup, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerJoiningAsync(BlazeServerConnection connection, NotifyPlayerJoining notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerJoining, notification, waitUntilFree);
            }
            
            public static Task NotifyJoiningPlayerInitiateConnectionsAsync(BlazeServerConnection connection, NotifyGameSetup notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyJoiningPlayerInitiateConnections, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerJoiningQueueAsync(BlazeServerConnection connection, NotifyPlayerJoining notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerJoiningQueue, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerPromotedFromQueueAsync(BlazeServerConnection connection, NotifyPlayerJoining notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerPromotedFromQueue, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerClaimingReservationAsync(BlazeServerConnection connection, NotifyPlayerJoining notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerClaimingReservation, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerJoinCompletedAsync(BlazeServerConnection connection, NotifyPlayerJoinCompleted notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerJoinCompleted, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerRemovedAsync(BlazeServerConnection connection, NotifyPlayerRemoved notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerRemoved, notification, waitUntilFree);
            }
            
            public static Task NotifyHostMigrationFinishedAsync(BlazeServerConnection connection, NotifyHostMigrationFinished notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyHostMigrationFinished, notification, waitUntilFree);
            }
            
            public static Task NotifyHostMigrationStartAsync(BlazeServerConnection connection, NotifyHostMigrationStart notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyHostMigrationStart, notification, waitUntilFree);
            }
            
            public static Task NotifyPlatformHostInitializedAsync(BlazeServerConnection connection, NotifyPlatformHostInitialized notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlatformHostInitialized, notification, waitUntilFree);
            }
            
            public static Task NotifyGameAttribChangeAsync(BlazeServerConnection connection, NotifyGameAttribChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameAttribChange, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerAttribChangeAsync(BlazeServerConnection connection, NotifyPlayerAttribChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerAttribChange, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerCustomDataChangeAsync(BlazeServerConnection connection, NotifyPlayerCustomDataChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerCustomDataChange, notification, waitUntilFree);
            }
            
            public static Task NotifyGameStateChangeAsync(BlazeServerConnection connection, NotifyGameStateChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameStateChange, notification, waitUntilFree);
            }
            
            public static Task NotifyGameSettingsChangeAsync(BlazeServerConnection connection, NotifyGameSettingsChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameSettingsChange, notification, waitUntilFree);
            }
            
            public static Task NotifyGameCapacityChangeAsync(BlazeServerConnection connection, NotifyGameCapacityChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameCapacityChange, notification, waitUntilFree);
            }
            
            public static Task NotifyGameResetAsync(BlazeServerConnection connection, NotifyGameReset notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameReset, notification, waitUntilFree);
            }
            
            public static Task NotifyGameReportingIdChangeAsync(BlazeServerConnection connection, NotifyGameReportingIdChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameReportingIdChange, notification, waitUntilFree);
            }
            
            public static Task NotifyGameSessionUpdatedAsync(BlazeServerConnection connection, GameSessionUpdatedNotification notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameSessionUpdated, notification, waitUntilFree);
            }
            
            public static Task NotifyGamePlayerStateChangeAsync(BlazeServerConnection connection, NotifyGamePlayerStateChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGamePlayerStateChange, notification, waitUntilFree);
            }
            
            public static Task NotifyGamePlayerTeamChangeAsync(BlazeServerConnection connection, NotifyGamePlayerTeamChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGamePlayerTeamChange, notification, waitUntilFree);
            }
            
            public static Task NotifyGameTeamIdChangeAsync(BlazeServerConnection connection, NotifyGameTeamIdChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameTeamIdChange, notification, waitUntilFree);
            }
            
            public static Task NotifyProcessQueueAsync(BlazeServerConnection connection, NotifyProcessQueue notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyProcessQueue, notification, waitUntilFree);
            }
            
            public static Task NotifyPresenceModeChangedAsync(BlazeServerConnection connection, NotifyPresenceModeChanged notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPresenceModeChanged, notification, waitUntilFree);
            }
            
            public static Task NotifyGamePlayerQueuePositionChangeAsync(BlazeServerConnection connection, NotifyGamePlayerQueuePositionChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGamePlayerQueuePositionChange, notification, waitUntilFree);
            }
            
            public static Task NotifyGameListUpdateAsync(BlazeServerConnection connection, NotifyGameListUpdate notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameListUpdate, notification, waitUntilFree);
            }
            
            public static Task NotifyAdminListChangeAsync(BlazeServerConnection connection, NotifyAdminListChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyAdminListChange, notification, waitUntilFree);
            }
            
            public static Task NotifyCreateDynamicDedicatedServerGameAsync(BlazeServerConnection connection, NotifyCreateDynamicDedicatedServerGame notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyCreateDynamicDedicatedServerGame, notification, waitUntilFree);
            }
            
            public static Task NotifyGameNameChangeAsync(BlazeServerConnection connection, NotifyGameNameChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGameNameChange, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(GameManagerCommand command) => GameManagerBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameManagerCommand command) => GameManagerBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameManagerCommand command) => GameManagerBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameManagerNotification notification) => GameManagerBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<GameManagerCommand, GameManagerNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(GameManagerBase.Id, GameManagerBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public CreateGameResponse CreateGame(CreateGameRequest request)
            {
                return Connection.SendRequest<CreateGameRequest, CreateGameResponse, NullStruct>(this, (ushort)GameManagerCommand.createGame, request);
            }
            public Task<CreateGameResponse> CreateGameAsync(CreateGameRequest request)
            {
                return Connection.SendRequestAsync<CreateGameRequest, CreateGameResponse, NullStruct>(this, (ushort)GameManagerCommand.createGame, request);
            }
            
            public DestroyGameResponse DestroyGame(DestroyGameRequest request)
            {
                return Connection.SendRequest<DestroyGameRequest, DestroyGameResponse, NullStruct>(this, (ushort)GameManagerCommand.destroyGame, request);
            }
            public Task<DestroyGameResponse> DestroyGameAsync(DestroyGameRequest request)
            {
                return Connection.SendRequestAsync<DestroyGameRequest, DestroyGameResponse, NullStruct>(this, (ushort)GameManagerCommand.destroyGame, request);
            }
            
            public NullStruct AdvanceGameState(AdvanceGameStateRequest request)
            {
                return Connection.SendRequest<AdvanceGameStateRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.advanceGameState, request);
            }
            public Task<NullStruct> AdvanceGameStateAsync(AdvanceGameStateRequest request)
            {
                return Connection.SendRequestAsync<AdvanceGameStateRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.advanceGameState, request);
            }
            
            public NullStruct SetGameSettings(SetGameSettingsRequest request)
            {
                return Connection.SendRequest<SetGameSettingsRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setGameSettings, request);
            }
            public Task<NullStruct> SetGameSettingsAsync(SetGameSettingsRequest request)
            {
                return Connection.SendRequestAsync<SetGameSettingsRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setGameSettings, request);
            }
            
            public NullStruct SetPlayerCapacity(SetPlayerCapacityRequest request)
            {
                return Connection.SendRequest<SetPlayerCapacityRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerCapacity, request);
            }
            public Task<NullStruct> SetPlayerCapacityAsync(SetPlayerCapacityRequest request)
            {
                return Connection.SendRequestAsync<SetPlayerCapacityRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerCapacity, request);
            }
            
            public NullStruct SetPresenceMode(SetPresenceModeRequest request)
            {
                return Connection.SendRequest<SetPresenceModeRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPresenceMode, request);
            }
            public Task<NullStruct> SetPresenceModeAsync(SetPresenceModeRequest request)
            {
                return Connection.SendRequestAsync<SetPresenceModeRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPresenceMode, request);
            }
            
            public NullStruct SetGameAttributes(SetGameAttributesRequest request)
            {
                return Connection.SendRequest<SetGameAttributesRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setGameAttributes, request);
            }
            public Task<NullStruct> SetGameAttributesAsync(SetGameAttributesRequest request)
            {
                return Connection.SendRequestAsync<SetGameAttributesRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setGameAttributes, request);
            }
            
            public NullStruct SetPlayerAttributes(SetPlayerAttributesRequest request)
            {
                return Connection.SendRequest<SetPlayerAttributesRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerAttributes, request);
            }
            public Task<NullStruct> SetPlayerAttributesAsync(SetPlayerAttributesRequest request)
            {
                return Connection.SendRequestAsync<SetPlayerAttributesRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerAttributes, request);
            }
            
            public JoinGameResponse JoinGame(JoinGameRequest request)
            {
                return Connection.SendRequest<JoinGameRequest, JoinGameResponse, NullStruct>(this, (ushort)GameManagerCommand.joinGame, request);
            }
            public Task<JoinGameResponse> JoinGameAsync(JoinGameRequest request)
            {
                return Connection.SendRequestAsync<JoinGameRequest, JoinGameResponse, NullStruct>(this, (ushort)GameManagerCommand.joinGame, request);
            }
            
            public NullStruct RemovePlayer(RemovePlayerRequest request)
            {
                return Connection.SendRequest<RemovePlayerRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.removePlayer, request);
            }
            public Task<NullStruct> RemovePlayerAsync(RemovePlayerRequest request)
            {
                return Connection.SendRequestAsync<RemovePlayerRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.removePlayer, request);
            }
            
            public StartMatchmakingResponse StartMatchmaking(StartMatchmakingRequest request)
            {
                return Connection.SendRequest<StartMatchmakingRequest, StartMatchmakingResponse, NullStruct>(this, (ushort)GameManagerCommand.startMatchmaking, request);
            }
            public Task<StartMatchmakingResponse> StartMatchmakingAsync(StartMatchmakingRequest request)
            {
                return Connection.SendRequestAsync<StartMatchmakingRequest, StartMatchmakingResponse, NullStruct>(this, (ushort)GameManagerCommand.startMatchmaking, request);
            }
            
            public NullStruct CancelMatchmaking(CancelMatchmakingRequest request)
            {
                return Connection.SendRequest<CancelMatchmakingRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.cancelMatchmaking, request);
            }
            public Task<NullStruct> CancelMatchmakingAsync(CancelMatchmakingRequest request)
            {
                return Connection.SendRequestAsync<CancelMatchmakingRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.cancelMatchmaking, request);
            }
            
            public NullStruct FinalizeGameCreation(UpdateGameSessionRequest request)
            {
                return Connection.SendRequest<UpdateGameSessionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.finalizeGameCreation, request);
            }
            public Task<NullStruct> FinalizeGameCreationAsync(UpdateGameSessionRequest request)
            {
                return Connection.SendRequestAsync<UpdateGameSessionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.finalizeGameCreation, request);
            }
            
            public ListGamesResponse ListGames()
            {
                return Connection.SendRequest<NullStruct, ListGamesResponse, NullStruct>(this, (ushort)GameManagerCommand.listGames, new NullStruct());
            }
            public Task<ListGamesResponse> ListGamesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, ListGamesResponse, NullStruct>(this, (ushort)GameManagerCommand.listGames, new NullStruct());
            }
            
            public NullStruct SetPlayerCustomData(SetPlayerCustomDataRequest request)
            {
                return Connection.SendRequest<SetPlayerCustomDataRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerCustomData, request);
            }
            public Task<NullStruct> SetPlayerCustomDataAsync(SetPlayerCustomDataRequest request)
            {
                return Connection.SendRequestAsync<SetPlayerCustomDataRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerCustomData, request);
            }
            
            public NullStruct ReplayGame(ReplayGameRequest request)
            {
                return Connection.SendRequest<ReplayGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.replayGame, request);
            }
            public Task<NullStruct> ReplayGameAsync(ReplayGameRequest request)
            {
                return Connection.SendRequestAsync<ReplayGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.replayGame, request);
            }
            
            public NullStruct ReturnDedicatedServerToPool(ReturnDedicatedServerToPoolRequest request)
            {
                return Connection.SendRequest<ReturnDedicatedServerToPoolRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.returnDedicatedServerToPool, request);
            }
            public Task<NullStruct> ReturnDedicatedServerToPoolAsync(ReturnDedicatedServerToPoolRequest request)
            {
                return Connection.SendRequestAsync<ReturnDedicatedServerToPoolRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.returnDedicatedServerToPool, request);
            }
            
            public NullStruct JoinGameByGroup(JoinGameRequest request)
            {
                return Connection.SendRequest<JoinGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.joinGameByGroup, request);
            }
            public Task<NullStruct> JoinGameByGroupAsync(JoinGameRequest request)
            {
                return Connection.SendRequestAsync<JoinGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.joinGameByGroup, request);
            }
            
            public NullStruct LeaveGameByGroup(RemovePlayerRequest request)
            {
                return Connection.SendRequest<RemovePlayerRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.leaveGameByGroup, request);
            }
            public Task<NullStruct> LeaveGameByGroupAsync(RemovePlayerRequest request)
            {
                return Connection.SendRequestAsync<RemovePlayerRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.leaveGameByGroup, request);
            }
            
            public NullStruct MigrateGame(MigrateHostRequest request)
            {
                return Connection.SendRequest<MigrateHostRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.migrateGame, request);
            }
            public Task<NullStruct> MigrateGameAsync(MigrateHostRequest request)
            {
                return Connection.SendRequestAsync<MigrateHostRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.migrateGame, request);
            }
            
            public NullStruct UpdateGameHostMigrationStatus(UpdateGameHostMigrationStatusRequest request)
            {
                return Connection.SendRequest<UpdateGameHostMigrationStatusRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateGameHostMigrationStatus, request);
            }
            public Task<NullStruct> UpdateGameHostMigrationStatusAsync(UpdateGameHostMigrationStatusRequest request)
            {
                return Connection.SendRequestAsync<UpdateGameHostMigrationStatusRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateGameHostMigrationStatus, request);
            }
            
            public NullStruct ResetDedicatedServer(CreateGameRequest request)
            {
                return Connection.SendRequest<CreateGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.resetDedicatedServer, request);
            }
            public Task<NullStruct> ResetDedicatedServerAsync(CreateGameRequest request)
            {
                return Connection.SendRequestAsync<CreateGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.resetDedicatedServer, request);
            }
            
            public NullStruct UpdateGameSession(UpdateGameSessionRequest request)
            {
                return Connection.SendRequest<UpdateGameSessionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateGameSession, request);
            }
            public Task<NullStruct> UpdateGameSessionAsync(UpdateGameSessionRequest request)
            {
                return Connection.SendRequestAsync<UpdateGameSessionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateGameSession, request);
            }
            
            public NullStruct BanPlayer(BanPlayerRequest request)
            {
                return Connection.SendRequest<BanPlayerRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.banPlayer, request);
            }
            public Task<NullStruct> BanPlayerAsync(BanPlayerRequest request)
            {
                return Connection.SendRequestAsync<BanPlayerRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.banPlayer, request);
            }
            
            public NullStruct UpdateMeshConnection(UpdateMeshConnectionRequest request)
            {
                return Connection.SendRequest<UpdateMeshConnectionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateMeshConnection, request);
            }
            public Task<NullStruct> UpdateMeshConnectionAsync(UpdateMeshConnectionRequest request)
            {
                return Connection.SendRequestAsync<UpdateMeshConnectionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateMeshConnection, request);
            }
            
            public NullStruct RemovePlayerFromBannedList(RemovePlayerFromBannedListRequest request)
            {
                return Connection.SendRequest<RemovePlayerFromBannedListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.removePlayerFromBannedList, request);
            }
            public Task<NullStruct> RemovePlayerFromBannedListAsync(RemovePlayerFromBannedListRequest request)
            {
                return Connection.SendRequestAsync<RemovePlayerFromBannedListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.removePlayerFromBannedList, request);
            }
            
            public NullStruct ClearBannedList(BannedListRequest request)
            {
                return Connection.SendRequest<BannedListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.clearBannedList, request);
            }
            public Task<NullStruct> ClearBannedListAsync(BannedListRequest request)
            {
                return Connection.SendRequestAsync<BannedListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.clearBannedList, request);
            }
            
            public BannedListResponse GetBannedList(BannedListRequest request)
            {
                return Connection.SendRequest<BannedListRequest, BannedListResponse, NullStruct>(this, (ushort)GameManagerCommand.getBannedList, request);
            }
            public Task<BannedListResponse> GetBannedListAsync(BannedListRequest request)
            {
                return Connection.SendRequestAsync<BannedListRequest, BannedListResponse, NullStruct>(this, (ushort)GameManagerCommand.getBannedList, request);
            }
            
            public NullStruct AddQueuedPlayerToGame(AddQueuedPlayerToGameRequest request)
            {
                return Connection.SendRequest<AddQueuedPlayerToGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.addQueuedPlayerToGame, request);
            }
            public Task<NullStruct> AddQueuedPlayerToGameAsync(AddQueuedPlayerToGameRequest request)
            {
                return Connection.SendRequestAsync<AddQueuedPlayerToGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.addQueuedPlayerToGame, request);
            }
            
            public NullStruct UpdateGameName(UpdateGameNameRequest request)
            {
                return Connection.SendRequest<UpdateGameNameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateGameName, request);
            }
            public Task<NullStruct> UpdateGameNameAsync(UpdateGameNameRequest request)
            {
                return Connection.SendRequestAsync<UpdateGameNameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateGameName, request);
            }
            
            public NullStruct EjectHost(EjectHostRequest request)
            {
                return Connection.SendRequest<EjectHostRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.ejectHost, request);
            }
            public Task<NullStruct> EjectHostAsync(EjectHostRequest request)
            {
                return Connection.SendRequestAsync<EjectHostRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.ejectHost, request);
            }
            
            public GetGameListResponse GetGameListSnapshot(GetGameListRequest request)
            {
                return Connection.SendRequest<GetGameListRequest, GetGameListResponse, NullStruct>(this, (ushort)GameManagerCommand.getGameListSnapshot, request);
            }
            public Task<GetGameListResponse> GetGameListSnapshotAsync(GetGameListRequest request)
            {
                return Connection.SendRequestAsync<GetGameListRequest, GetGameListResponse, NullStruct>(this, (ushort)GameManagerCommand.getGameListSnapshot, request);
            }
            
            public GetGameListResponse GetGameListSubscription(GetGameListRequest request)
            {
                return Connection.SendRequest<GetGameListRequest, GetGameListResponse, NullStruct>(this, (ushort)GameManagerCommand.getGameListSubscription, request);
            }
            public Task<GetGameListResponse> GetGameListSubscriptionAsync(GetGameListRequest request)
            {
                return Connection.SendRequestAsync<GetGameListRequest, GetGameListResponse, NullStruct>(this, (ushort)GameManagerCommand.getGameListSubscription, request);
            }
            
            public NullStruct DestroyGameList(DestroyGameListRequest request)
            {
                return Connection.SendRequest<DestroyGameListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.destroyGameList, request);
            }
            public Task<NullStruct> DestroyGameListAsync(DestroyGameListRequest request)
            {
                return Connection.SendRequestAsync<DestroyGameListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.destroyGameList, request);
            }
            
            public GetFullGameDataResponse GetFullGameData(GetFullGameDataRequest request)
            {
                return Connection.SendRequest<GetFullGameDataRequest, GetFullGameDataResponse, NullStruct>(this, (ushort)GameManagerCommand.getFullGameData, request);
            }
            public Task<GetFullGameDataResponse> GetFullGameDataAsync(GetFullGameDataRequest request)
            {
                return Connection.SendRequestAsync<GetFullGameDataRequest, GetFullGameDataResponse, NullStruct>(this, (ushort)GameManagerCommand.getFullGameData, request);
            }
            
            public GetMatchmakingConfigResponse GetMatchmakingConfig()
            {
                return Connection.SendRequest<NullStruct, GetMatchmakingConfigResponse, NullStruct>(this, (ushort)GameManagerCommand.getMatchmakingConfig, new NullStruct());
            }
            public Task<GetMatchmakingConfigResponse> GetMatchmakingConfigAsync()
            {
                return Connection.SendRequestAsync<NullStruct, GetMatchmakingConfigResponse, NullStruct>(this, (ushort)GameManagerCommand.getMatchmakingConfig, new NullStruct());
            }
            
            public NullStruct GetGameDataFromId(GetGameDataFromIdRequest request)
            {
                return Connection.SendRequest<GetGameDataFromIdRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.getGameDataFromId, request);
            }
            public Task<NullStruct> GetGameDataFromIdAsync(GetGameDataFromIdRequest request)
            {
                return Connection.SendRequestAsync<GetGameDataFromIdRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.getGameDataFromId, request);
            }
            
            public NullStruct AddAdminPlayer(UpdateAdminListRequest request)
            {
                return Connection.SendRequest<UpdateAdminListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.addAdminPlayer, request);
            }
            public Task<NullStruct> AddAdminPlayerAsync(UpdateAdminListRequest request)
            {
                return Connection.SendRequestAsync<UpdateAdminListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.addAdminPlayer, request);
            }
            
            public NullStruct RemoveAdminPlayer(UpdateAdminListRequest request)
            {
                return Connection.SendRequest<UpdateAdminListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.removeAdminPlayer, request);
            }
            public Task<NullStruct> RemoveAdminPlayerAsync(UpdateAdminListRequest request)
            {
                return Connection.SendRequestAsync<UpdateAdminListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.removeAdminPlayer, request);
            }
            
            public NullStruct SetPlayerTeam(SetPlayerTeamRequest request)
            {
                return Connection.SendRequest<SetPlayerTeamRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerTeam, request);
            }
            public Task<NullStruct> SetPlayerTeamAsync(SetPlayerTeamRequest request)
            {
                return Connection.SendRequestAsync<SetPlayerTeamRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerTeam, request);
            }
            
            public NullStruct ChangeGameTeamId(ChangeTeamIdRequest request)
            {
                return Connection.SendRequest<ChangeTeamIdRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.changeGameTeamId, request);
            }
            public Task<NullStruct> ChangeGameTeamIdAsync(ChangeTeamIdRequest request)
            {
                return Connection.SendRequestAsync<ChangeTeamIdRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.changeGameTeamId, request);
            }
            
            public NullStruct MigrateAdminPlayer(UpdateAdminListRequest request)
            {
                return Connection.SendRequest<UpdateAdminListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.migrateAdminPlayer, request);
            }
            public Task<NullStruct> MigrateAdminPlayerAsync(UpdateAdminListRequest request)
            {
                return Connection.SendRequestAsync<UpdateAdminListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.migrateAdminPlayer, request);
            }
            
            public NullStruct GetUserSetGameListSubscription(GetUserSetGameListSubscriptionRequest request)
            {
                return Connection.SendRequest<GetUserSetGameListSubscriptionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.getUserSetGameListSubscription, request);
            }
            public Task<NullStruct> GetUserSetGameListSubscriptionAsync(GetUserSetGameListSubscriptionRequest request)
            {
                return Connection.SendRequestAsync<GetUserSetGameListSubscriptionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.getUserSetGameListSubscription, request);
            }
            
            public NullStruct SwapPlayersTeam(SwapPlayersTeamRequest request)
            {
                return Connection.SendRequest<SwapPlayersTeamRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.swapPlayersTeam, request);
            }
            public Task<NullStruct> SwapPlayersTeamAsync(SwapPlayersTeamRequest request)
            {
                return Connection.SendRequestAsync<SwapPlayersTeamRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.swapPlayersTeam, request);
            }
            
            public RegisterDynamicDedicatedServerCreatorResponse RegisterDynamicDedicatedServerCreator(RegisterDynamicDedicatedServerCreatorRequest request)
            {
                return Connection.SendRequest<RegisterDynamicDedicatedServerCreatorRequest, RegisterDynamicDedicatedServerCreatorResponse, NullStruct>(this, (ushort)GameManagerCommand.registerDynamicDedicatedServerCreator, request);
            }
            public Task<RegisterDynamicDedicatedServerCreatorResponse> RegisterDynamicDedicatedServerCreatorAsync(RegisterDynamicDedicatedServerCreatorRequest request)
            {
                return Connection.SendRequestAsync<RegisterDynamicDedicatedServerCreatorRequest, RegisterDynamicDedicatedServerCreatorResponse, NullStruct>(this, (ushort)GameManagerCommand.registerDynamicDedicatedServerCreator, request);
            }
            
            public UnregisterDynamicDedicatedServerCreatorResponse UnregisterDynamicDedicatedServerCreator(UnregisterDynamicDedicatedServerCreatorRequest request)
            {
                return Connection.SendRequest<UnregisterDynamicDedicatedServerCreatorRequest, UnregisterDynamicDedicatedServerCreatorResponse, NullStruct>(this, (ushort)GameManagerCommand.unregisterDynamicDedicatedServerCreator, request);
            }
            public Task<UnregisterDynamicDedicatedServerCreatorResponse> UnregisterDynamicDedicatedServerCreatorAsync(UnregisterDynamicDedicatedServerCreatorRequest request)
            {
                return Connection.SendRequestAsync<UnregisterDynamicDedicatedServerCreatorRequest, UnregisterDynamicDedicatedServerCreatorResponse, NullStruct>(this, (ushort)GameManagerCommand.unregisterDynamicDedicatedServerCreator, request);
            }
            
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyMatchmakingFailed)]
            public virtual Task OnNotifyMatchmakingFailedAsync(NotifyMatchmakingFailed notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyMatchmakingFailedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyMatchmakingAsyncStatus)]
            public virtual Task OnNotifyMatchmakingAsyncStatusAsync(NotifyMatchmakingAsyncStatus notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyMatchmakingAsyncStatusAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameCreated)]
            public virtual Task OnNotifyGameCreatedAsync(NotifyGameCreated notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameCreatedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameRemoved)]
            public virtual Task OnNotifyGameRemovedAsync(NotifyGameRemoved notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameRemovedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameSetup)]
            public virtual Task OnNotifyGameSetupAsync(NotifyGameSetup notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameSetupAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerJoining)]
            public virtual Task OnNotifyPlayerJoiningAsync(NotifyPlayerJoining notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPlayerJoiningAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyJoiningPlayerInitiateConnections)]
            public virtual Task OnNotifyJoiningPlayerInitiateConnectionsAsync(NotifyGameSetup notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyJoiningPlayerInitiateConnectionsAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerJoiningQueue)]
            public virtual Task OnNotifyPlayerJoiningQueueAsync(NotifyPlayerJoining notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPlayerJoiningQueueAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerPromotedFromQueue)]
            public virtual Task OnNotifyPlayerPromotedFromQueueAsync(NotifyPlayerJoining notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPlayerPromotedFromQueueAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerClaimingReservation)]
            public virtual Task OnNotifyPlayerClaimingReservationAsync(NotifyPlayerJoining notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPlayerClaimingReservationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerJoinCompleted)]
            public virtual Task OnNotifyPlayerJoinCompletedAsync(NotifyPlayerJoinCompleted notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPlayerJoinCompletedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerRemoved)]
            public virtual Task OnNotifyPlayerRemovedAsync(NotifyPlayerRemoved notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPlayerRemovedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyHostMigrationFinished)]
            public virtual Task OnNotifyHostMigrationFinishedAsync(NotifyHostMigrationFinished notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyHostMigrationFinishedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyHostMigrationStart)]
            public virtual Task OnNotifyHostMigrationStartAsync(NotifyHostMigrationStart notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyHostMigrationStartAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlatformHostInitialized)]
            public virtual Task OnNotifyPlatformHostInitializedAsync(NotifyPlatformHostInitialized notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPlatformHostInitializedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameAttribChange)]
            public virtual Task OnNotifyGameAttribChangeAsync(NotifyGameAttribChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameAttribChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerAttribChange)]
            public virtual Task OnNotifyPlayerAttribChangeAsync(NotifyPlayerAttribChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPlayerAttribChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerCustomDataChange)]
            public virtual Task OnNotifyPlayerCustomDataChangeAsync(NotifyPlayerCustomDataChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPlayerCustomDataChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameStateChange)]
            public virtual Task OnNotifyGameStateChangeAsync(NotifyGameStateChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameStateChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameSettingsChange)]
            public virtual Task OnNotifyGameSettingsChangeAsync(NotifyGameSettingsChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameSettingsChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameCapacityChange)]
            public virtual Task OnNotifyGameCapacityChangeAsync(NotifyGameCapacityChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameCapacityChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameReset)]
            public virtual Task OnNotifyGameResetAsync(NotifyGameReset notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameResetAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameReportingIdChange)]
            public virtual Task OnNotifyGameReportingIdChangeAsync(NotifyGameReportingIdChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameReportingIdChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameSessionUpdated)]
            public virtual Task OnNotifyGameSessionUpdatedAsync(GameSessionUpdatedNotification notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameSessionUpdatedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGamePlayerStateChange)]
            public virtual Task OnNotifyGamePlayerStateChangeAsync(NotifyGamePlayerStateChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGamePlayerStateChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGamePlayerTeamChange)]
            public virtual Task OnNotifyGamePlayerTeamChangeAsync(NotifyGamePlayerTeamChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGamePlayerTeamChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameTeamIdChange)]
            public virtual Task OnNotifyGameTeamIdChangeAsync(NotifyGameTeamIdChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameTeamIdChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyProcessQueue)]
            public virtual Task OnNotifyProcessQueueAsync(NotifyProcessQueue notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyProcessQueueAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPresenceModeChanged)]
            public virtual Task OnNotifyPresenceModeChangedAsync(NotifyPresenceModeChanged notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPresenceModeChangedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGamePlayerQueuePositionChange)]
            public virtual Task OnNotifyGamePlayerQueuePositionChangeAsync(NotifyGamePlayerQueuePositionChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGamePlayerQueuePositionChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameListUpdate)]
            public virtual Task OnNotifyGameListUpdateAsync(NotifyGameListUpdate notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameListUpdateAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyAdminListChange)]
            public virtual Task OnNotifyAdminListChangeAsync(NotifyAdminListChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyAdminListChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyCreateDynamicDedicatedServerGame)]
            public virtual Task OnNotifyCreateDynamicDedicatedServerGameAsync(NotifyCreateDynamicDedicatedServerGame notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyCreateDynamicDedicatedServerGameAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameNameChange)]
            public virtual Task OnNotifyGameNameChangeAsync(NotifyGameNameChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyGameNameChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(GameManagerCommand command) => GameManagerBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameManagerCommand command) => GameManagerBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameManagerCommand command) => GameManagerBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameManagerNotification notification) => GameManagerBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<GameManagerCommand, GameManagerNotification, Blaze3RpcError>
        {
            public Proxy() : base(GameManagerBase.Id, GameManagerBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)GameManagerCommand.createGame)]
            public virtual Task<CreateGameResponse> CreateGameAsync(CreateGameRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<CreateGameRequest, CreateGameResponse, NullStruct>(this, (ushort)GameManagerCommand.createGame, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.destroyGame)]
            public virtual Task<DestroyGameResponse> DestroyGameAsync(DestroyGameRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<DestroyGameRequest, DestroyGameResponse, NullStruct>(this, (ushort)GameManagerCommand.destroyGame, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.advanceGameState)]
            public virtual Task<NullStruct> AdvanceGameStateAsync(AdvanceGameStateRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<AdvanceGameStateRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.advanceGameState, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setGameSettings)]
            public virtual Task<NullStruct> SetGameSettingsAsync(SetGameSettingsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SetGameSettingsRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setGameSettings, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerCapacity)]
            public virtual Task<NullStruct> SetPlayerCapacityAsync(SetPlayerCapacityRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SetPlayerCapacityRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerCapacity, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPresenceMode)]
            public virtual Task<NullStruct> SetPresenceModeAsync(SetPresenceModeRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SetPresenceModeRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPresenceMode, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setGameAttributes)]
            public virtual Task<NullStruct> SetGameAttributesAsync(SetGameAttributesRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SetGameAttributesRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setGameAttributes, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerAttributes)]
            public virtual Task<NullStruct> SetPlayerAttributesAsync(SetPlayerAttributesRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SetPlayerAttributesRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerAttributes, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.joinGame)]
            public virtual Task<JoinGameResponse> JoinGameAsync(JoinGameRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<JoinGameRequest, JoinGameResponse, NullStruct>(this, (ushort)GameManagerCommand.joinGame, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.removePlayer)]
            public virtual Task<NullStruct> RemovePlayerAsync(RemovePlayerRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<RemovePlayerRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.removePlayer, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.startMatchmaking)]
            public virtual Task<StartMatchmakingResponse> StartMatchmakingAsync(StartMatchmakingRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<StartMatchmakingRequest, StartMatchmakingResponse, NullStruct>(this, (ushort)GameManagerCommand.startMatchmaking, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.cancelMatchmaking)]
            public virtual Task<NullStruct> CancelMatchmakingAsync(CancelMatchmakingRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<CancelMatchmakingRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.cancelMatchmaking, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.finalizeGameCreation)]
            public virtual Task<NullStruct> FinalizeGameCreationAsync(UpdateGameSessionRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateGameSessionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.finalizeGameCreation, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.listGames)]
            public virtual Task<ListGamesResponse> ListGamesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, ListGamesResponse, NullStruct>(this, (ushort)GameManagerCommand.listGames, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerCustomData)]
            public virtual Task<NullStruct> SetPlayerCustomDataAsync(SetPlayerCustomDataRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SetPlayerCustomDataRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerCustomData, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.replayGame)]
            public virtual Task<NullStruct> ReplayGameAsync(ReplayGameRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ReplayGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.replayGame, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.returnDedicatedServerToPool)]
            public virtual Task<NullStruct> ReturnDedicatedServerToPoolAsync(ReturnDedicatedServerToPoolRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ReturnDedicatedServerToPoolRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.returnDedicatedServerToPool, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.joinGameByGroup)]
            public virtual Task<NullStruct> JoinGameByGroupAsync(JoinGameRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<JoinGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.joinGameByGroup, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.leaveGameByGroup)]
            public virtual Task<NullStruct> LeaveGameByGroupAsync(RemovePlayerRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<RemovePlayerRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.leaveGameByGroup, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.migrateGame)]
            public virtual Task<NullStruct> MigrateGameAsync(MigrateHostRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<MigrateHostRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.migrateGame, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateGameHostMigrationStatus)]
            public virtual Task<NullStruct> UpdateGameHostMigrationStatusAsync(UpdateGameHostMigrationStatusRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateGameHostMigrationStatusRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateGameHostMigrationStatus, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.resetDedicatedServer)]
            public virtual Task<NullStruct> ResetDedicatedServerAsync(CreateGameRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<CreateGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.resetDedicatedServer, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateGameSession)]
            public virtual Task<NullStruct> UpdateGameSessionAsync(UpdateGameSessionRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateGameSessionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateGameSession, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.banPlayer)]
            public virtual Task<NullStruct> BanPlayerAsync(BanPlayerRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<BanPlayerRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.banPlayer, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateMeshConnection)]
            public virtual Task<NullStruct> UpdateMeshConnectionAsync(UpdateMeshConnectionRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateMeshConnectionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateMeshConnection, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.removePlayerFromBannedList)]
            public virtual Task<NullStruct> RemovePlayerFromBannedListAsync(RemovePlayerFromBannedListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<RemovePlayerFromBannedListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.removePlayerFromBannedList, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.clearBannedList)]
            public virtual Task<NullStruct> ClearBannedListAsync(BannedListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<BannedListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.clearBannedList, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getBannedList)]
            public virtual Task<BannedListResponse> GetBannedListAsync(BannedListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<BannedListRequest, BannedListResponse, NullStruct>(this, (ushort)GameManagerCommand.getBannedList, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.addQueuedPlayerToGame)]
            public virtual Task<NullStruct> AddQueuedPlayerToGameAsync(AddQueuedPlayerToGameRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<AddQueuedPlayerToGameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.addQueuedPlayerToGame, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateGameName)]
            public virtual Task<NullStruct> UpdateGameNameAsync(UpdateGameNameRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateGameNameRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateGameName, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.ejectHost)]
            public virtual Task<NullStruct> EjectHostAsync(EjectHostRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<EjectHostRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.ejectHost, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameListSnapshot)]
            public virtual Task<GetGameListResponse> GetGameListSnapshotAsync(GetGameListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetGameListRequest, GetGameListResponse, NullStruct>(this, (ushort)GameManagerCommand.getGameListSnapshot, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameListSubscription)]
            public virtual Task<GetGameListResponse> GetGameListSubscriptionAsync(GetGameListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetGameListRequest, GetGameListResponse, NullStruct>(this, (ushort)GameManagerCommand.getGameListSubscription, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.destroyGameList)]
            public virtual Task<NullStruct> DestroyGameListAsync(DestroyGameListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<DestroyGameListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.destroyGameList, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getFullGameData)]
            public virtual Task<GetFullGameDataResponse> GetFullGameDataAsync(GetFullGameDataRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetFullGameDataRequest, GetFullGameDataResponse, NullStruct>(this, (ushort)GameManagerCommand.getFullGameData, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getMatchmakingConfig)]
            public virtual Task<GetMatchmakingConfigResponse> GetMatchmakingConfigAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, GetMatchmakingConfigResponse, NullStruct>(this, (ushort)GameManagerCommand.getMatchmakingConfig, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameDataFromId)]
            public virtual Task<NullStruct> GetGameDataFromIdAsync(GetGameDataFromIdRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetGameDataFromIdRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.getGameDataFromId, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.addAdminPlayer)]
            public virtual Task<NullStruct> AddAdminPlayerAsync(UpdateAdminListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateAdminListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.addAdminPlayer, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.removeAdminPlayer)]
            public virtual Task<NullStruct> RemoveAdminPlayerAsync(UpdateAdminListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateAdminListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.removeAdminPlayer, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerTeam)]
            public virtual Task<NullStruct> SetPlayerTeamAsync(SetPlayerTeamRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SetPlayerTeamRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.setPlayerTeam, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.changeGameTeamId)]
            public virtual Task<NullStruct> ChangeGameTeamIdAsync(ChangeTeamIdRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ChangeTeamIdRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.changeGameTeamId, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.migrateAdminPlayer)]
            public virtual Task<NullStruct> MigrateAdminPlayerAsync(UpdateAdminListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateAdminListRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.migrateAdminPlayer, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getUserSetGameListSubscription)]
            public virtual Task<NullStruct> GetUserSetGameListSubscriptionAsync(GetUserSetGameListSubscriptionRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetUserSetGameListSubscriptionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.getUserSetGameListSubscription, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.swapPlayersTeam)]
            public virtual Task<NullStruct> SwapPlayersTeamAsync(SwapPlayersTeamRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SwapPlayersTeamRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.swapPlayersTeam, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.registerDynamicDedicatedServerCreator)]
            public virtual Task<RegisterDynamicDedicatedServerCreatorResponse> RegisterDynamicDedicatedServerCreatorAsync(RegisterDynamicDedicatedServerCreatorRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<RegisterDynamicDedicatedServerCreatorRequest, RegisterDynamicDedicatedServerCreatorResponse, NullStruct>(this, (ushort)GameManagerCommand.registerDynamicDedicatedServerCreator, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.unregisterDynamicDedicatedServerCreator)]
            public virtual Task<UnregisterDynamicDedicatedServerCreatorResponse> UnregisterDynamicDedicatedServerCreatorAsync(UnregisterDynamicDedicatedServerCreatorRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UnregisterDynamicDedicatedServerCreatorRequest, UnregisterDynamicDedicatedServerCreatorResponse, NullStruct>(this, (ushort)GameManagerCommand.unregisterDynamicDedicatedServerCreator, request);
            }
            
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyMatchmakingFailed)]
            public virtual Task<NotifyMatchmakingFailed> OnNotifyMatchmakingFailedAsync(NotifyMatchmakingFailed notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyMatchmakingAsyncStatus)]
            public virtual Task<NotifyMatchmakingAsyncStatus> OnNotifyMatchmakingAsyncStatusAsync(NotifyMatchmakingAsyncStatus notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameCreated)]
            public virtual Task<NotifyGameCreated> OnNotifyGameCreatedAsync(NotifyGameCreated notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameRemoved)]
            public virtual Task<NotifyGameRemoved> OnNotifyGameRemovedAsync(NotifyGameRemoved notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameSetup)]
            public virtual Task<NotifyGameSetup> OnNotifyGameSetupAsync(NotifyGameSetup notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerJoining)]
            public virtual Task<NotifyPlayerJoining> OnNotifyPlayerJoiningAsync(NotifyPlayerJoining notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyJoiningPlayerInitiateConnections)]
            public virtual Task<NotifyGameSetup> OnNotifyJoiningPlayerInitiateConnectionsAsync(NotifyGameSetup notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerJoiningQueue)]
            public virtual Task<NotifyPlayerJoining> OnNotifyPlayerJoiningQueueAsync(NotifyPlayerJoining notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerPromotedFromQueue)]
            public virtual Task<NotifyPlayerJoining> OnNotifyPlayerPromotedFromQueueAsync(NotifyPlayerJoining notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerClaimingReservation)]
            public virtual Task<NotifyPlayerJoining> OnNotifyPlayerClaimingReservationAsync(NotifyPlayerJoining notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerJoinCompleted)]
            public virtual Task<NotifyPlayerJoinCompleted> OnNotifyPlayerJoinCompletedAsync(NotifyPlayerJoinCompleted notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerRemoved)]
            public virtual Task<NotifyPlayerRemoved> OnNotifyPlayerRemovedAsync(NotifyPlayerRemoved notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyHostMigrationFinished)]
            public virtual Task<NotifyHostMigrationFinished> OnNotifyHostMigrationFinishedAsync(NotifyHostMigrationFinished notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyHostMigrationStart)]
            public virtual Task<NotifyHostMigrationStart> OnNotifyHostMigrationStartAsync(NotifyHostMigrationStart notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlatformHostInitialized)]
            public virtual Task<NotifyPlatformHostInitialized> OnNotifyPlatformHostInitializedAsync(NotifyPlatformHostInitialized notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameAttribChange)]
            public virtual Task<NotifyGameAttribChange> OnNotifyGameAttribChangeAsync(NotifyGameAttribChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerAttribChange)]
            public virtual Task<NotifyPlayerAttribChange> OnNotifyPlayerAttribChangeAsync(NotifyPlayerAttribChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerCustomDataChange)]
            public virtual Task<NotifyPlayerCustomDataChange> OnNotifyPlayerCustomDataChangeAsync(NotifyPlayerCustomDataChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameStateChange)]
            public virtual Task<NotifyGameStateChange> OnNotifyGameStateChangeAsync(NotifyGameStateChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameSettingsChange)]
            public virtual Task<NotifyGameSettingsChange> OnNotifyGameSettingsChangeAsync(NotifyGameSettingsChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameCapacityChange)]
            public virtual Task<NotifyGameCapacityChange> OnNotifyGameCapacityChangeAsync(NotifyGameCapacityChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameReset)]
            public virtual Task<NotifyGameReset> OnNotifyGameResetAsync(NotifyGameReset notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameReportingIdChange)]
            public virtual Task<NotifyGameReportingIdChange> OnNotifyGameReportingIdChangeAsync(NotifyGameReportingIdChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameSessionUpdated)]
            public virtual Task<GameSessionUpdatedNotification> OnNotifyGameSessionUpdatedAsync(GameSessionUpdatedNotification notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGamePlayerStateChange)]
            public virtual Task<NotifyGamePlayerStateChange> OnNotifyGamePlayerStateChangeAsync(NotifyGamePlayerStateChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGamePlayerTeamChange)]
            public virtual Task<NotifyGamePlayerTeamChange> OnNotifyGamePlayerTeamChangeAsync(NotifyGamePlayerTeamChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameTeamIdChange)]
            public virtual Task<NotifyGameTeamIdChange> OnNotifyGameTeamIdChangeAsync(NotifyGameTeamIdChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyProcessQueue)]
            public virtual Task<NotifyProcessQueue> OnNotifyProcessQueueAsync(NotifyProcessQueue notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPresenceModeChanged)]
            public virtual Task<NotifyPresenceModeChanged> OnNotifyPresenceModeChangedAsync(NotifyPresenceModeChanged notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGamePlayerQueuePositionChange)]
            public virtual Task<NotifyGamePlayerQueuePositionChange> OnNotifyGamePlayerQueuePositionChangeAsync(NotifyGamePlayerQueuePositionChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameListUpdate)]
            public virtual Task<NotifyGameListUpdate> OnNotifyGameListUpdateAsync(NotifyGameListUpdate notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyAdminListChange)]
            public virtual Task<NotifyAdminListChange> OnNotifyAdminListChangeAsync(NotifyAdminListChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyCreateDynamicDedicatedServerGame)]
            public virtual Task<NotifyCreateDynamicDedicatedServerGame> OnNotifyCreateDynamicDedicatedServerGameAsync(NotifyCreateDynamicDedicatedServerGame notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameNameChange)]
            public virtual Task<NotifyGameNameChange> OnNotifyGameNameChangeAsync(NotifyGameNameChange notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(GameManagerCommand command) => GameManagerBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameManagerCommand command) => GameManagerBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameManagerCommand command) => GameManagerBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameManagerNotification notification) => GameManagerBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(GameManagerCommand command) => command switch
        {
            GameManagerCommand.createGame => typeof(CreateGameRequest),
            GameManagerCommand.destroyGame => typeof(DestroyGameRequest),
            GameManagerCommand.advanceGameState => typeof(AdvanceGameStateRequest),
            GameManagerCommand.setGameSettings => typeof(SetGameSettingsRequest),
            GameManagerCommand.setPlayerCapacity => typeof(SetPlayerCapacityRequest),
            GameManagerCommand.setPresenceMode => typeof(SetPresenceModeRequest),
            GameManagerCommand.setGameAttributes => typeof(SetGameAttributesRequest),
            GameManagerCommand.setPlayerAttributes => typeof(SetPlayerAttributesRequest),
            GameManagerCommand.joinGame => typeof(JoinGameRequest),
            GameManagerCommand.removePlayer => typeof(RemovePlayerRequest),
            GameManagerCommand.startMatchmaking => typeof(StartMatchmakingRequest),
            GameManagerCommand.cancelMatchmaking => typeof(CancelMatchmakingRequest),
            GameManagerCommand.finalizeGameCreation => typeof(UpdateGameSessionRequest),
            GameManagerCommand.listGames => typeof(NullStruct),
            GameManagerCommand.setPlayerCustomData => typeof(SetPlayerCustomDataRequest),
            GameManagerCommand.replayGame => typeof(ReplayGameRequest),
            GameManagerCommand.returnDedicatedServerToPool => typeof(ReturnDedicatedServerToPoolRequest),
            GameManagerCommand.joinGameByGroup => typeof(JoinGameRequest),
            GameManagerCommand.leaveGameByGroup => typeof(RemovePlayerRequest),
            GameManagerCommand.migrateGame => typeof(MigrateHostRequest),
            GameManagerCommand.updateGameHostMigrationStatus => typeof(UpdateGameHostMigrationStatusRequest),
            GameManagerCommand.resetDedicatedServer => typeof(CreateGameRequest),
            GameManagerCommand.updateGameSession => typeof(UpdateGameSessionRequest),
            GameManagerCommand.banPlayer => typeof(BanPlayerRequest),
            GameManagerCommand.updateMeshConnection => typeof(UpdateMeshConnectionRequest),
            GameManagerCommand.removePlayerFromBannedList => typeof(RemovePlayerFromBannedListRequest),
            GameManagerCommand.clearBannedList => typeof(BannedListRequest),
            GameManagerCommand.getBannedList => typeof(BannedListRequest),
            GameManagerCommand.addQueuedPlayerToGame => typeof(AddQueuedPlayerToGameRequest),
            GameManagerCommand.updateGameName => typeof(UpdateGameNameRequest),
            GameManagerCommand.ejectHost => typeof(EjectHostRequest),
            GameManagerCommand.getGameListSnapshot => typeof(GetGameListRequest),
            GameManagerCommand.getGameListSubscription => typeof(GetGameListRequest),
            GameManagerCommand.destroyGameList => typeof(DestroyGameListRequest),
            GameManagerCommand.getFullGameData => typeof(GetFullGameDataRequest),
            GameManagerCommand.getMatchmakingConfig => typeof(NullStruct),
            GameManagerCommand.getGameDataFromId => typeof(GetGameDataFromIdRequest),
            GameManagerCommand.addAdminPlayer => typeof(UpdateAdminListRequest),
            GameManagerCommand.removeAdminPlayer => typeof(UpdateAdminListRequest),
            GameManagerCommand.setPlayerTeam => typeof(SetPlayerTeamRequest),
            GameManagerCommand.changeGameTeamId => typeof(ChangeTeamIdRequest),
            GameManagerCommand.migrateAdminPlayer => typeof(UpdateAdminListRequest),
            GameManagerCommand.getUserSetGameListSubscription => typeof(GetUserSetGameListSubscriptionRequest),
            GameManagerCommand.swapPlayersTeam => typeof(SwapPlayersTeamRequest),
            GameManagerCommand.registerDynamicDedicatedServerCreator => typeof(RegisterDynamicDedicatedServerCreatorRequest),
            GameManagerCommand.unregisterDynamicDedicatedServerCreator => typeof(UnregisterDynamicDedicatedServerCreatorRequest),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(GameManagerCommand command) => command switch
        {
            GameManagerCommand.createGame => typeof(CreateGameResponse),
            GameManagerCommand.destroyGame => typeof(DestroyGameResponse),
            GameManagerCommand.advanceGameState => typeof(NullStruct),
            GameManagerCommand.setGameSettings => typeof(NullStruct),
            GameManagerCommand.setPlayerCapacity => typeof(NullStruct),
            GameManagerCommand.setPresenceMode => typeof(NullStruct),
            GameManagerCommand.setGameAttributes => typeof(NullStruct),
            GameManagerCommand.setPlayerAttributes => typeof(NullStruct),
            GameManagerCommand.joinGame => typeof(JoinGameResponse),
            GameManagerCommand.removePlayer => typeof(NullStruct),
            GameManagerCommand.startMatchmaking => typeof(StartMatchmakingResponse),
            GameManagerCommand.cancelMatchmaking => typeof(NullStruct),
            GameManagerCommand.finalizeGameCreation => typeof(NullStruct),
            GameManagerCommand.listGames => typeof(ListGamesResponse),
            GameManagerCommand.setPlayerCustomData => typeof(NullStruct),
            GameManagerCommand.replayGame => typeof(NullStruct),
            GameManagerCommand.returnDedicatedServerToPool => typeof(NullStruct),
            GameManagerCommand.joinGameByGroup => typeof(NullStruct),
            GameManagerCommand.leaveGameByGroup => typeof(NullStruct),
            GameManagerCommand.migrateGame => typeof(NullStruct),
            GameManagerCommand.updateGameHostMigrationStatus => typeof(NullStruct),
            GameManagerCommand.resetDedicatedServer => typeof(NullStruct),
            GameManagerCommand.updateGameSession => typeof(NullStruct),
            GameManagerCommand.banPlayer => typeof(NullStruct),
            GameManagerCommand.updateMeshConnection => typeof(NullStruct),
            GameManagerCommand.removePlayerFromBannedList => typeof(NullStruct),
            GameManagerCommand.clearBannedList => typeof(NullStruct),
            GameManagerCommand.getBannedList => typeof(BannedListResponse),
            GameManagerCommand.addQueuedPlayerToGame => typeof(NullStruct),
            GameManagerCommand.updateGameName => typeof(NullStruct),
            GameManagerCommand.ejectHost => typeof(NullStruct),
            GameManagerCommand.getGameListSnapshot => typeof(GetGameListResponse),
            GameManagerCommand.getGameListSubscription => typeof(GetGameListResponse),
            GameManagerCommand.destroyGameList => typeof(NullStruct),
            GameManagerCommand.getFullGameData => typeof(GetFullGameDataResponse),
            GameManagerCommand.getMatchmakingConfig => typeof(GetMatchmakingConfigResponse),
            GameManagerCommand.getGameDataFromId => typeof(NullStruct),
            GameManagerCommand.addAdminPlayer => typeof(NullStruct),
            GameManagerCommand.removeAdminPlayer => typeof(NullStruct),
            GameManagerCommand.setPlayerTeam => typeof(NullStruct),
            GameManagerCommand.changeGameTeamId => typeof(NullStruct),
            GameManagerCommand.migrateAdminPlayer => typeof(NullStruct),
            GameManagerCommand.getUserSetGameListSubscription => typeof(NullStruct),
            GameManagerCommand.swapPlayersTeam => typeof(NullStruct),
            GameManagerCommand.registerDynamicDedicatedServerCreator => typeof(RegisterDynamicDedicatedServerCreatorResponse),
            GameManagerCommand.unregisterDynamicDedicatedServerCreator => typeof(UnregisterDynamicDedicatedServerCreatorResponse),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(GameManagerCommand command) => command switch
        {
            GameManagerCommand.createGame => typeof(NullStruct),
            GameManagerCommand.destroyGame => typeof(NullStruct),
            GameManagerCommand.advanceGameState => typeof(NullStruct),
            GameManagerCommand.setGameSettings => typeof(NullStruct),
            GameManagerCommand.setPlayerCapacity => typeof(NullStruct),
            GameManagerCommand.setPresenceMode => typeof(NullStruct),
            GameManagerCommand.setGameAttributes => typeof(NullStruct),
            GameManagerCommand.setPlayerAttributes => typeof(NullStruct),
            GameManagerCommand.joinGame => typeof(NullStruct),
            GameManagerCommand.removePlayer => typeof(NullStruct),
            GameManagerCommand.startMatchmaking => typeof(NullStruct),
            GameManagerCommand.cancelMatchmaking => typeof(NullStruct),
            GameManagerCommand.finalizeGameCreation => typeof(NullStruct),
            GameManagerCommand.listGames => typeof(NullStruct),
            GameManagerCommand.setPlayerCustomData => typeof(NullStruct),
            GameManagerCommand.replayGame => typeof(NullStruct),
            GameManagerCommand.returnDedicatedServerToPool => typeof(NullStruct),
            GameManagerCommand.joinGameByGroup => typeof(NullStruct),
            GameManagerCommand.leaveGameByGroup => typeof(NullStruct),
            GameManagerCommand.migrateGame => typeof(NullStruct),
            GameManagerCommand.updateGameHostMigrationStatus => typeof(NullStruct),
            GameManagerCommand.resetDedicatedServer => typeof(NullStruct),
            GameManagerCommand.updateGameSession => typeof(NullStruct),
            GameManagerCommand.banPlayer => typeof(NullStruct),
            GameManagerCommand.updateMeshConnection => typeof(NullStruct),
            GameManagerCommand.removePlayerFromBannedList => typeof(NullStruct),
            GameManagerCommand.clearBannedList => typeof(NullStruct),
            GameManagerCommand.getBannedList => typeof(NullStruct),
            GameManagerCommand.addQueuedPlayerToGame => typeof(NullStruct),
            GameManagerCommand.updateGameName => typeof(NullStruct),
            GameManagerCommand.ejectHost => typeof(NullStruct),
            GameManagerCommand.getGameListSnapshot => typeof(NullStruct),
            GameManagerCommand.getGameListSubscription => typeof(NullStruct),
            GameManagerCommand.destroyGameList => typeof(NullStruct),
            GameManagerCommand.getFullGameData => typeof(NullStruct),
            GameManagerCommand.getMatchmakingConfig => typeof(NullStruct),
            GameManagerCommand.getGameDataFromId => typeof(NullStruct),
            GameManagerCommand.addAdminPlayer => typeof(NullStruct),
            GameManagerCommand.removeAdminPlayer => typeof(NullStruct),
            GameManagerCommand.setPlayerTeam => typeof(NullStruct),
            GameManagerCommand.changeGameTeamId => typeof(NullStruct),
            GameManagerCommand.migrateAdminPlayer => typeof(NullStruct),
            GameManagerCommand.getUserSetGameListSubscription => typeof(NullStruct),
            GameManagerCommand.swapPlayersTeam => typeof(NullStruct),
            GameManagerCommand.registerDynamicDedicatedServerCreator => typeof(NullStruct),
            GameManagerCommand.unregisterDynamicDedicatedServerCreator => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(GameManagerNotification notification) => notification switch
        {
            GameManagerNotification.NotifyMatchmakingFailed => typeof(NotifyMatchmakingFailed),
            GameManagerNotification.NotifyMatchmakingAsyncStatus => typeof(NotifyMatchmakingAsyncStatus),
            GameManagerNotification.NotifyGameCreated => typeof(NotifyGameCreated),
            GameManagerNotification.NotifyGameRemoved => typeof(NotifyGameRemoved),
            GameManagerNotification.NotifyGameSetup => typeof(NotifyGameSetup),
            GameManagerNotification.NotifyPlayerJoining => typeof(NotifyPlayerJoining),
            GameManagerNotification.NotifyJoiningPlayerInitiateConnections => typeof(NotifyGameSetup),
            GameManagerNotification.NotifyPlayerJoiningQueue => typeof(NotifyPlayerJoining),
            GameManagerNotification.NotifyPlayerPromotedFromQueue => typeof(NotifyPlayerJoining),
            GameManagerNotification.NotifyPlayerClaimingReservation => typeof(NotifyPlayerJoining),
            GameManagerNotification.NotifyPlayerJoinCompleted => typeof(NotifyPlayerJoinCompleted),
            GameManagerNotification.NotifyPlayerRemoved => typeof(NotifyPlayerRemoved),
            GameManagerNotification.NotifyHostMigrationFinished => typeof(NotifyHostMigrationFinished),
            GameManagerNotification.NotifyHostMigrationStart => typeof(NotifyHostMigrationStart),
            GameManagerNotification.NotifyPlatformHostInitialized => typeof(NotifyPlatformHostInitialized),
            GameManagerNotification.NotifyGameAttribChange => typeof(NotifyGameAttribChange),
            GameManagerNotification.NotifyPlayerAttribChange => typeof(NotifyPlayerAttribChange),
            GameManagerNotification.NotifyPlayerCustomDataChange => typeof(NotifyPlayerCustomDataChange),
            GameManagerNotification.NotifyGameStateChange => typeof(NotifyGameStateChange),
            GameManagerNotification.NotifyGameSettingsChange => typeof(NotifyGameSettingsChange),
            GameManagerNotification.NotifyGameCapacityChange => typeof(NotifyGameCapacityChange),
            GameManagerNotification.NotifyGameReset => typeof(NotifyGameReset),
            GameManagerNotification.NotifyGameReportingIdChange => typeof(NotifyGameReportingIdChange),
            GameManagerNotification.NotifyGameSessionUpdated => typeof(GameSessionUpdatedNotification),
            GameManagerNotification.NotifyGamePlayerStateChange => typeof(NotifyGamePlayerStateChange),
            GameManagerNotification.NotifyGamePlayerTeamChange => typeof(NotifyGamePlayerTeamChange),
            GameManagerNotification.NotifyGameTeamIdChange => typeof(NotifyGameTeamIdChange),
            GameManagerNotification.NotifyProcessQueue => typeof(NotifyProcessQueue),
            GameManagerNotification.NotifyPresenceModeChanged => typeof(NotifyPresenceModeChanged),
            GameManagerNotification.NotifyGamePlayerQueuePositionChange => typeof(NotifyGamePlayerQueuePositionChange),
            GameManagerNotification.NotifyGameListUpdate => typeof(NotifyGameListUpdate),
            GameManagerNotification.NotifyAdminListChange => typeof(NotifyAdminListChange),
            GameManagerNotification.NotifyCreateDynamicDedicatedServerGame => typeof(NotifyCreateDynamicDedicatedServerGame),
            GameManagerNotification.NotifyGameNameChange => typeof(NotifyGameNameChange),
            _ => typeof(NullStruct)
        };
        
        public enum GameManagerCommand : ushort
        {
            createGame = 1,
            destroyGame = 2,
            advanceGameState = 3,
            setGameSettings = 4,
            setPlayerCapacity = 5,
            setPresenceMode = 6,
            setGameAttributes = 7,
            setPlayerAttributes = 8,
            joinGame = 9,
            removePlayer = 11,
            startMatchmaking = 13,
            cancelMatchmaking = 14,
            finalizeGameCreation = 15,
            listGames = 17,
            setPlayerCustomData = 18,
            replayGame = 19,
            returnDedicatedServerToPool = 20,
            joinGameByGroup = 21,
            leaveGameByGroup = 22,
            migrateGame = 23,
            updateGameHostMigrationStatus = 24,
            resetDedicatedServer = 25,
            updateGameSession = 26,
            banPlayer = 27,
            updateMeshConnection = 29,
            removePlayerFromBannedList = 31,
            clearBannedList = 32,
            getBannedList = 33,
            addQueuedPlayerToGame = 38,
            updateGameName = 39,
            ejectHost = 40,
            getGameListSnapshot = 100,
            getGameListSubscription = 101,
            destroyGameList = 102,
            getFullGameData = 103,
            getMatchmakingConfig = 104,
            getGameDataFromId = 105,
            addAdminPlayer = 106,
            removeAdminPlayer = 107,
            setPlayerTeam = 108,
            changeGameTeamId = 109,
            migrateAdminPlayer = 110,
            getUserSetGameListSubscription = 111,
            swapPlayersTeam = 112,
            registerDynamicDedicatedServerCreator = 150,
            unregisterDynamicDedicatedServerCreator = 151,
        }
        
        public enum GameManagerNotification : ushort
        {
            NotifyMatchmakingFailed = 10,
            NotifyMatchmakingAsyncStatus = 12,
            NotifyGameCreated = 15,
            NotifyGameRemoved = 16,
            NotifyGameSetup = 20,
            NotifyPlayerJoining = 21,
            NotifyJoiningPlayerInitiateConnections = 22,
            NotifyPlayerJoiningQueue = 23,
            NotifyPlayerPromotedFromQueue = 24,
            NotifyPlayerClaimingReservation = 25,
            NotifyPlayerJoinCompleted = 30,
            NotifyPlayerRemoved = 40,
            NotifyHostMigrationFinished = 60,
            NotifyHostMigrationStart = 70,
            NotifyPlatformHostInitialized = 71,
            NotifyGameAttribChange = 80,
            NotifyPlayerAttribChange = 90,
            NotifyPlayerCustomDataChange = 95,
            NotifyGameStateChange = 100,
            NotifyGameSettingsChange = 110,
            NotifyGameCapacityChange = 111,
            NotifyGameReset = 112,
            NotifyGameReportingIdChange = 113,
            NotifyGameSessionUpdated = 115,
            NotifyGamePlayerStateChange = 116,
            NotifyGamePlayerTeamChange = 117,
            NotifyGameTeamIdChange = 118,
            NotifyProcessQueue = 119,
            NotifyPresenceModeChanged = 120,
            NotifyGamePlayerQueuePositionChange = 121,
            NotifyGameListUpdate = 201,
            NotifyAdminListChange = 202,
            NotifyCreateDynamicDedicatedServerGame = 220,
            NotifyGameNameChange = 230,
        }
        
    }
}
