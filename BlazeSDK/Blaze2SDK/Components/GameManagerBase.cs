using Blaze2SDK.Blaze;
using Blaze2SDK.Blaze.GameManager;
using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class GameManagerBase
    {
        public const ushort Id = 4;
        public const string Name = "GameManager";
        
        public class Server : BlazeServerComponent<GameManagerCommand, GameManagerNotification, Blaze2RpcError>
        {
            public Server() : base(GameManagerBase.Id, GameManagerBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)GameManagerCommand.createGame)]
            public virtual Task<CreateGameResponse> CreateGameAsync(CreateGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.destroyGame)]
            public virtual Task<DestroyGameResponse> DestroyGameAsync(DestroyGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.advanceGameState)]
            public virtual Task<NullStruct> AdvanceGameStateAsync(AdvanceGameStateRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setGameSettings)]
            public virtual Task<NullStruct> SetGameSettingsAsync(SetGameSettingsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerCapacity)]
            public virtual Task<NullStruct> SetPlayerCapacityAsync(SetPlayerCapacityRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setGameAttributes)]
            public virtual Task<NullStruct> SetGameAttributesAsync(SetGameAttributesRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerAttributes)]
            public virtual Task<NullStruct> SetPlayerAttributesAsync(SetPlayerAttributesRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.joinGame)]
            public virtual Task<JoinGameResponse> JoinGameAsync(JoinGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updatePlayerConnection)]
            public virtual Task<NullStruct> UpdatePlayerConnectionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.removePlayer)]
            public virtual Task<NullStruct> RemovePlayerAsync(RemovePlayerRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.startMatchmaking)]
            public virtual Task<StartMatchmakingResponse> StartMatchmakingAsync(StartMatchmakingRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.cancelMatchmaking)]
            public virtual Task<NullStruct> CancelMatchmakingAsync(CancelMatchmakingRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.finalizeGameCreation)]
            public virtual Task<NullStruct> FinalizeGameCreationAsync(UpdateGameSessionRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateHostConnection)]
            public virtual Task<NullStruct> UpdateHostConnectionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.listGames)]
            public virtual Task<NullStruct> ListGamesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerCustomData)]
            public virtual Task<NullStruct> SetPlayerCustomDataAsync(SetPlayerCustomDataRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.replayGame)]
            public virtual Task<NullStruct> ReplayGameAsync(ReplayGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.returnDedicatedServerToPool)]
            public virtual Task<NullStruct> ReturnDedicatedServerToPoolAsync(ReturnDedicatedServerToPoolRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.joinGameByGroup)]
            public virtual Task<JoinGameResponse> JoinGameByGroupAsync(JoinGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.leaveGameByGroup)]
            public virtual Task<NullStruct> LeaveGameByGroupAsync(RemovePlayerRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.migrateGame)]
            public virtual Task<NullStruct> MigrateGameAsync(MigrateHostRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.resetDedicatedServer)]
            public virtual Task<JoinGameResponse> ResetDedicatedServerAsync(CreateGameRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateGameSession)]
            public virtual Task<NullStruct> UpdateGameSessionAsync(UpdateGameSessionRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.banPlayer)]
            public virtual Task<NullStruct> BanPlayerAsync(BanPlayerRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.matchmakingDedicatedServerOverride)]
            public virtual Task<NullStruct> MatchmakingDedicatedServerOverrideAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateMeshConnection)]
            public virtual Task<NullStruct> UpdateMeshConnectionAsync(UpdateMeshConnectionRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.joinGameByUserList)]
            public virtual Task<NullStruct> JoinGameByUserListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameListSnapshot)]
            public virtual Task<GetGameListResponse> GetGameListSnapshotAsync(GetGameListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameListSubscription)]
            public virtual Task<GetGameListResponse> GetGameListSubscriptionAsync(GetGameListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.destroyGameList)]
            public virtual Task<NullStruct> DestroyGameListAsync(DestroyGameListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getFullGameData)]
            public virtual Task<GetFullGameDataResponse> GetFullGameDataAsync(GetFullGameDataRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getMatchmakingConfig)]
            public virtual Task<GetMatchmakingConfigResponse> GetMatchmakingConfigAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameDataFromId)]
            public virtual Task<GameBrowserDataList> GetGameDataFromIdAsync(GetGameDataFromIdRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.addAdminPlayer)]
            public virtual Task<NullStruct> AddAdminPlayerAsync(UpdateAdminListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.removeAdminPlayer)]
            public virtual Task<NullStruct> RemoveAdminPlayerAsync(UpdateAdminListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.setPlayerTeam)]
            public virtual Task<NullStruct> SetPlayerTeamAsync(SetPlayerTeamRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.changeGameTeamId)]
            public virtual Task<NullStruct> ChangeGameTeamIdAsync(ChangeTeamIdRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.migrateAdminPlayer)]
            public virtual Task<NullStruct> MigrateAdminPlayerAsync(UpdateAdminListRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.registerDynamicDedicatedServerCreator)]
            public virtual Task<NullStruct> RegisterDynamicDedicatedServerCreatorAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.unregisterDynamicDedicatedServerCreator)]
            public virtual Task<NullStruct> UnregisterDynamicDedicatedServerCreatorAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyMatchmakingFinishedAsync(BlazeServerConnection connection, NotifyMatchmakingFinished notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyMatchmakingFinished, notification, waitUntilFree);
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
            
            public static Task NotifyJoinGameAsync(BlazeServerConnection connection, NotifyJoinGame notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyJoinGame, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerJoiningAsync(BlazeServerConnection connection, NotifyPlayerJoining notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerJoining, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerJoinCompletedAsync(BlazeServerConnection connection, NotifyPlayerJoinCompleted notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerJoinCompleted, notification, waitUntilFree);
            }
            
            public static Task NotifyGroupPreJoinedGameAsync(BlazeServerConnection connection, NotifyGroupPreJoinedGame notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyGroupPreJoinedGame, notification, waitUntilFree);
            }
            
            public static Task NotifyPlayerRemovedAsync(BlazeServerConnection connection, NotifyPlayerRemoved notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyPlayerRemoved, notification, waitUntilFree);
            }
            
            public static Task NotifyQueueChangedAsync(BlazeServerConnection connection, NotifyQueueChanged notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(GameManagerBase.Id, (ushort)GameManagerNotification.NotifyQueueChanged, notification, waitUntilFree);
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
            
            public override Type GetCommandRequestType(GameManagerCommand command) => GameManagerBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameManagerCommand command) => GameManagerBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameManagerCommand command) => GameManagerBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameManagerNotification notification) => GameManagerBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<GameManagerCommand, GameManagerNotification, Blaze2RpcError>
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
                return Connection.SendRequest<JoinGameRequest, JoinGameResponse, EntryCriteriaError>(this, (ushort)GameManagerCommand.joinGame, request);
            }
            public Task<JoinGameResponse> JoinGameAsync(JoinGameRequest request)
            {
                return Connection.SendRequestAsync<JoinGameRequest, JoinGameResponse, EntryCriteriaError>(this, (ushort)GameManagerCommand.joinGame, request);
            }
            
            public NullStruct UpdatePlayerConnection()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updatePlayerConnection, new NullStruct());
            }
            public Task<NullStruct> UpdatePlayerConnectionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updatePlayerConnection, new NullStruct());
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
                return Connection.SendRequest<StartMatchmakingRequest, StartMatchmakingResponse, MatchmakingCriteriaError>(this, (ushort)GameManagerCommand.startMatchmaking, request);
            }
            public Task<StartMatchmakingResponse> StartMatchmakingAsync(StartMatchmakingRequest request)
            {
                return Connection.SendRequestAsync<StartMatchmakingRequest, StartMatchmakingResponse, MatchmakingCriteriaError>(this, (ushort)GameManagerCommand.startMatchmaking, request);
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
            
            public NullStruct UpdateHostConnection()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateHostConnection, new NullStruct());
            }
            public Task<NullStruct> UpdateHostConnectionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateHostConnection, new NullStruct());
            }
            
            public NullStruct ListGames()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.listGames, new NullStruct());
            }
            public Task<NullStruct> ListGamesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.listGames, new NullStruct());
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
            
            public JoinGameResponse JoinGameByGroup(JoinGameRequest request)
            {
                return Connection.SendRequest<JoinGameRequest, JoinGameResponse, EntryCriteriaError>(this, (ushort)GameManagerCommand.joinGameByGroup, request);
            }
            public Task<JoinGameResponse> JoinGameByGroupAsync(JoinGameRequest request)
            {
                return Connection.SendRequestAsync<JoinGameRequest, JoinGameResponse, EntryCriteriaError>(this, (ushort)GameManagerCommand.joinGameByGroup, request);
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
            
            public JoinGameResponse ResetDedicatedServer(CreateGameRequest request)
            {
                return Connection.SendRequest<CreateGameRequest, JoinGameResponse, NullStruct>(this, (ushort)GameManagerCommand.resetDedicatedServer, request);
            }
            public Task<JoinGameResponse> ResetDedicatedServerAsync(CreateGameRequest request)
            {
                return Connection.SendRequestAsync<CreateGameRequest, JoinGameResponse, NullStruct>(this, (ushort)GameManagerCommand.resetDedicatedServer, request);
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
            
            public NullStruct MatchmakingDedicatedServerOverride()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.matchmakingDedicatedServerOverride, new NullStruct());
            }
            public Task<NullStruct> MatchmakingDedicatedServerOverrideAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.matchmakingDedicatedServerOverride, new NullStruct());
            }
            
            public NullStruct UpdateMeshConnection(UpdateMeshConnectionRequest request)
            {
                return Connection.SendRequest<UpdateMeshConnectionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateMeshConnection, request);
            }
            public Task<NullStruct> UpdateMeshConnectionAsync(UpdateMeshConnectionRequest request)
            {
                return Connection.SendRequestAsync<UpdateMeshConnectionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateMeshConnection, request);
            }
            
            public NullStruct JoinGameByUserList()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.joinGameByUserList, new NullStruct());
            }
            public Task<NullStruct> JoinGameByUserListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.joinGameByUserList, new NullStruct());
            }
            
            public GetGameListResponse GetGameListSnapshot(GetGameListRequest request)
            {
                return Connection.SendRequest<GetGameListRequest, GetGameListResponse, MatchmakingCriteriaError>(this, (ushort)GameManagerCommand.getGameListSnapshot, request);
            }
            public Task<GetGameListResponse> GetGameListSnapshotAsync(GetGameListRequest request)
            {
                return Connection.SendRequestAsync<GetGameListRequest, GetGameListResponse, MatchmakingCriteriaError>(this, (ushort)GameManagerCommand.getGameListSnapshot, request);
            }
            
            public GetGameListResponse GetGameListSubscription(GetGameListRequest request)
            {
                return Connection.SendRequest<GetGameListRequest, GetGameListResponse, MatchmakingCriteriaError>(this, (ushort)GameManagerCommand.getGameListSubscription, request);
            }
            public Task<GetGameListResponse> GetGameListSubscriptionAsync(GetGameListRequest request)
            {
                return Connection.SendRequestAsync<GetGameListRequest, GetGameListResponse, MatchmakingCriteriaError>(this, (ushort)GameManagerCommand.getGameListSubscription, request);
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
            
            public GameBrowserDataList GetGameDataFromId(GetGameDataFromIdRequest request)
            {
                return Connection.SendRequest<GetGameDataFromIdRequest, GameBrowserDataList, NullStruct>(this, (ushort)GameManagerCommand.getGameDataFromId, request);
            }
            public Task<GameBrowserDataList> GetGameDataFromIdAsync(GetGameDataFromIdRequest request)
            {
                return Connection.SendRequestAsync<GetGameDataFromIdRequest, GameBrowserDataList, NullStruct>(this, (ushort)GameManagerCommand.getGameDataFromId, request);
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
            
            public NullStruct RegisterDynamicDedicatedServerCreator()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.registerDynamicDedicatedServerCreator, new NullStruct());
            }
            public Task<NullStruct> RegisterDynamicDedicatedServerCreatorAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.registerDynamicDedicatedServerCreator, new NullStruct());
            }
            
            public NullStruct UnregisterDynamicDedicatedServerCreator()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.unregisterDynamicDedicatedServerCreator, new NullStruct());
            }
            public Task<NullStruct> UnregisterDynamicDedicatedServerCreatorAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.unregisterDynamicDedicatedServerCreator, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyMatchmakingFinished)]
            public virtual Task OnNotifyMatchmakingFinishedAsync(NotifyMatchmakingFinished notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyMatchmakingFinishedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyMatchmakingAsyncStatus)]
            public virtual Task OnNotifyMatchmakingAsyncStatusAsync(NotifyMatchmakingAsyncStatus notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyMatchmakingAsyncStatusAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameCreated)]
            public virtual Task OnNotifyGameCreatedAsync(NotifyGameCreated notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameCreatedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameRemoved)]
            public virtual Task OnNotifyGameRemovedAsync(NotifyGameRemoved notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameRemovedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyJoinGame)]
            public virtual Task OnNotifyJoinGameAsync(NotifyJoinGame notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyJoinGameAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerJoining)]
            public virtual Task OnNotifyPlayerJoiningAsync(NotifyPlayerJoining notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyPlayerJoiningAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerJoinCompleted)]
            public virtual Task OnNotifyPlayerJoinCompletedAsync(NotifyPlayerJoinCompleted notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyPlayerJoinCompletedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGroupPreJoinedGame)]
            public virtual Task OnNotifyGroupPreJoinedGameAsync(NotifyGroupPreJoinedGame notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGroupPreJoinedGameAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerRemoved)]
            public virtual Task OnNotifyPlayerRemovedAsync(NotifyPlayerRemoved notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyPlayerRemovedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyQueueChanged)]
            public virtual Task OnNotifyQueueChangedAsync(NotifyQueueChanged notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyQueueChangedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyHostMigrationFinished)]
            public virtual Task OnNotifyHostMigrationFinishedAsync(NotifyHostMigrationFinished notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyHostMigrationFinishedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyHostMigrationStart)]
            public virtual Task OnNotifyHostMigrationStartAsync(NotifyHostMigrationStart notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyHostMigrationStartAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlatformHostInitialized)]
            public virtual Task OnNotifyPlatformHostInitializedAsync(NotifyPlatformHostInitialized notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyPlatformHostInitializedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameAttribChange)]
            public virtual Task OnNotifyGameAttribChangeAsync(NotifyGameAttribChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameAttribChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerAttribChange)]
            public virtual Task OnNotifyPlayerAttribChangeAsync(NotifyPlayerAttribChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyPlayerAttribChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerCustomDataChange)]
            public virtual Task OnNotifyPlayerCustomDataChangeAsync(NotifyPlayerCustomDataChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyPlayerCustomDataChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameStateChange)]
            public virtual Task OnNotifyGameStateChangeAsync(NotifyGameStateChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameStateChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameSettingsChange)]
            public virtual Task OnNotifyGameSettingsChangeAsync(NotifyGameSettingsChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameSettingsChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameCapacityChange)]
            public virtual Task OnNotifyGameCapacityChangeAsync(NotifyGameCapacityChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameCapacityChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameReset)]
            public virtual Task OnNotifyGameResetAsync(NotifyGameReset notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameResetAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameReportingIdChange)]
            public virtual Task OnNotifyGameReportingIdChangeAsync(NotifyGameReportingIdChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameReportingIdChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameSessionUpdated)]
            public virtual Task OnNotifyGameSessionUpdatedAsync(GameSessionUpdatedNotification notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameSessionUpdatedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGamePlayerStateChange)]
            public virtual Task OnNotifyGamePlayerStateChangeAsync(NotifyGamePlayerStateChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGamePlayerStateChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGamePlayerTeamChange)]
            public virtual Task OnNotifyGamePlayerTeamChangeAsync(NotifyGamePlayerTeamChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGamePlayerTeamChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameTeamIdChange)]
            public virtual Task OnNotifyGameTeamIdChangeAsync(NotifyGameTeamIdChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameTeamIdChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGameListUpdate)]
            public virtual Task OnNotifyGameListUpdateAsync(NotifyGameListUpdate notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyGameListUpdateAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyAdminListChange)]
            public virtual Task OnNotifyAdminListChangeAsync(NotifyAdminListChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyAdminListChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyCreateDynamicDedicatedServerGame)]
            public virtual Task OnNotifyCreateDynamicDedicatedServerGameAsync(NotifyCreateDynamicDedicatedServerGame notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyCreateDynamicDedicatedServerGameAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(GameManagerCommand command) => GameManagerBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(GameManagerCommand command) => GameManagerBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(GameManagerCommand command) => GameManagerBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(GameManagerNotification notification) => GameManagerBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<GameManagerCommand, GameManagerNotification, Blaze2RpcError>
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
                return context.ClientConnection.SendRequestAsync<JoinGameRequest, JoinGameResponse, EntryCriteriaError>(this, (ushort)GameManagerCommand.joinGame, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updatePlayerConnection)]
            public virtual Task<NullStruct> UpdatePlayerConnectionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updatePlayerConnection, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.removePlayer)]
            public virtual Task<NullStruct> RemovePlayerAsync(RemovePlayerRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<RemovePlayerRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.removePlayer, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.startMatchmaking)]
            public virtual Task<StartMatchmakingResponse> StartMatchmakingAsync(StartMatchmakingRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<StartMatchmakingRequest, StartMatchmakingResponse, MatchmakingCriteriaError>(this, (ushort)GameManagerCommand.startMatchmaking, request);
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
            
            [BlazeCommand((ushort)GameManagerCommand.updateHostConnection)]
            public virtual Task<NullStruct> UpdateHostConnectionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateHostConnection, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.listGames)]
            public virtual Task<NullStruct> ListGamesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.listGames, request);
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
            public virtual Task<JoinGameResponse> JoinGameByGroupAsync(JoinGameRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<JoinGameRequest, JoinGameResponse, EntryCriteriaError>(this, (ushort)GameManagerCommand.joinGameByGroup, request);
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
            
            [BlazeCommand((ushort)GameManagerCommand.resetDedicatedServer)]
            public virtual Task<JoinGameResponse> ResetDedicatedServerAsync(CreateGameRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<CreateGameRequest, JoinGameResponse, NullStruct>(this, (ushort)GameManagerCommand.resetDedicatedServer, request);
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
            
            [BlazeCommand((ushort)GameManagerCommand.matchmakingDedicatedServerOverride)]
            public virtual Task<NullStruct> MatchmakingDedicatedServerOverrideAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.matchmakingDedicatedServerOverride, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.updateMeshConnection)]
            public virtual Task<NullStruct> UpdateMeshConnectionAsync(UpdateMeshConnectionRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateMeshConnectionRequest, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.updateMeshConnection, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.joinGameByUserList)]
            public virtual Task<NullStruct> JoinGameByUserListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.joinGameByUserList, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameListSnapshot)]
            public virtual Task<GetGameListResponse> GetGameListSnapshotAsync(GetGameListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetGameListRequest, GetGameListResponse, MatchmakingCriteriaError>(this, (ushort)GameManagerCommand.getGameListSnapshot, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.getGameListSubscription)]
            public virtual Task<GetGameListResponse> GetGameListSubscriptionAsync(GetGameListRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetGameListRequest, GetGameListResponse, MatchmakingCriteriaError>(this, (ushort)GameManagerCommand.getGameListSubscription, request);
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
            public virtual Task<GameBrowserDataList> GetGameDataFromIdAsync(GetGameDataFromIdRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetGameDataFromIdRequest, GameBrowserDataList, NullStruct>(this, (ushort)GameManagerCommand.getGameDataFromId, request);
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
            
            [BlazeCommand((ushort)GameManagerCommand.registerDynamicDedicatedServerCreator)]
            public virtual Task<NullStruct> RegisterDynamicDedicatedServerCreatorAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.registerDynamicDedicatedServerCreator, request);
            }
            
            [BlazeCommand((ushort)GameManagerCommand.unregisterDynamicDedicatedServerCreator)]
            public virtual Task<NullStruct> UnregisterDynamicDedicatedServerCreatorAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)GameManagerCommand.unregisterDynamicDedicatedServerCreator, request);
            }
            
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyMatchmakingFinished)]
            public virtual Task<NotifyMatchmakingFinished> OnNotifyMatchmakingFinishedAsync(NotifyMatchmakingFinished notification)
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
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyJoinGame)]
            public virtual Task<NotifyJoinGame> OnNotifyJoinGameAsync(NotifyJoinGame notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerJoining)]
            public virtual Task<NotifyPlayerJoining> OnNotifyPlayerJoiningAsync(NotifyPlayerJoining notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerJoinCompleted)]
            public virtual Task<NotifyPlayerJoinCompleted> OnNotifyPlayerJoinCompletedAsync(NotifyPlayerJoinCompleted notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyGroupPreJoinedGame)]
            public virtual Task<NotifyGroupPreJoinedGame> OnNotifyGroupPreJoinedGameAsync(NotifyGroupPreJoinedGame notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyPlayerRemoved)]
            public virtual Task<NotifyPlayerRemoved> OnNotifyPlayerRemovedAsync(NotifyPlayerRemoved notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)GameManagerNotification.NotifyQueueChanged)]
            public virtual Task<NotifyQueueChanged> OnNotifyQueueChangedAsync(NotifyQueueChanged notification)
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
            GameManagerCommand.setGameAttributes => typeof(SetGameAttributesRequest),
            GameManagerCommand.setPlayerAttributes => typeof(SetPlayerAttributesRequest),
            GameManagerCommand.joinGame => typeof(JoinGameRequest),
            GameManagerCommand.updatePlayerConnection => typeof(NullStruct),
            GameManagerCommand.removePlayer => typeof(RemovePlayerRequest),
            GameManagerCommand.startMatchmaking => typeof(StartMatchmakingRequest),
            GameManagerCommand.cancelMatchmaking => typeof(CancelMatchmakingRequest),
            GameManagerCommand.finalizeGameCreation => typeof(UpdateGameSessionRequest),
            GameManagerCommand.updateHostConnection => typeof(NullStruct),
            GameManagerCommand.listGames => typeof(NullStruct),
            GameManagerCommand.setPlayerCustomData => typeof(SetPlayerCustomDataRequest),
            GameManagerCommand.replayGame => typeof(ReplayGameRequest),
            GameManagerCommand.returnDedicatedServerToPool => typeof(ReturnDedicatedServerToPoolRequest),
            GameManagerCommand.joinGameByGroup => typeof(JoinGameRequest),
            GameManagerCommand.leaveGameByGroup => typeof(RemovePlayerRequest),
            GameManagerCommand.migrateGame => typeof(MigrateHostRequest),
            GameManagerCommand.resetDedicatedServer => typeof(CreateGameRequest),
            GameManagerCommand.updateGameSession => typeof(UpdateGameSessionRequest),
            GameManagerCommand.banPlayer => typeof(BanPlayerRequest),
            GameManagerCommand.matchmakingDedicatedServerOverride => typeof(NullStruct),
            GameManagerCommand.updateMeshConnection => typeof(UpdateMeshConnectionRequest),
            GameManagerCommand.joinGameByUserList => typeof(NullStruct),
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
            GameManagerCommand.registerDynamicDedicatedServerCreator => typeof(NullStruct),
            GameManagerCommand.unregisterDynamicDedicatedServerCreator => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(GameManagerCommand command) => command switch
        {
            GameManagerCommand.createGame => typeof(CreateGameResponse),
            GameManagerCommand.destroyGame => typeof(DestroyGameResponse),
            GameManagerCommand.advanceGameState => typeof(NullStruct),
            GameManagerCommand.setGameSettings => typeof(NullStruct),
            GameManagerCommand.setPlayerCapacity => typeof(NullStruct),
            GameManagerCommand.setGameAttributes => typeof(NullStruct),
            GameManagerCommand.setPlayerAttributes => typeof(NullStruct),
            GameManagerCommand.joinGame => typeof(JoinGameResponse),
            GameManagerCommand.updatePlayerConnection => typeof(NullStruct),
            GameManagerCommand.removePlayer => typeof(NullStruct),
            GameManagerCommand.startMatchmaking => typeof(StartMatchmakingResponse),
            GameManagerCommand.cancelMatchmaking => typeof(NullStruct),
            GameManagerCommand.finalizeGameCreation => typeof(NullStruct),
            GameManagerCommand.updateHostConnection => typeof(NullStruct),
            GameManagerCommand.listGames => typeof(NullStruct),
            GameManagerCommand.setPlayerCustomData => typeof(NullStruct),
            GameManagerCommand.replayGame => typeof(NullStruct),
            GameManagerCommand.returnDedicatedServerToPool => typeof(NullStruct),
            GameManagerCommand.joinGameByGroup => typeof(JoinGameResponse),
            GameManagerCommand.leaveGameByGroup => typeof(NullStruct),
            GameManagerCommand.migrateGame => typeof(NullStruct),
            GameManagerCommand.resetDedicatedServer => typeof(JoinGameResponse),
            GameManagerCommand.updateGameSession => typeof(NullStruct),
            GameManagerCommand.banPlayer => typeof(NullStruct),
            GameManagerCommand.matchmakingDedicatedServerOverride => typeof(NullStruct),
            GameManagerCommand.updateMeshConnection => typeof(NullStruct),
            GameManagerCommand.joinGameByUserList => typeof(NullStruct),
            GameManagerCommand.getGameListSnapshot => typeof(GetGameListResponse),
            GameManagerCommand.getGameListSubscription => typeof(GetGameListResponse),
            GameManagerCommand.destroyGameList => typeof(NullStruct),
            GameManagerCommand.getFullGameData => typeof(GetFullGameDataResponse),
            GameManagerCommand.getMatchmakingConfig => typeof(GetMatchmakingConfigResponse),
            GameManagerCommand.getGameDataFromId => typeof(GameBrowserDataList),
            GameManagerCommand.addAdminPlayer => typeof(NullStruct),
            GameManagerCommand.removeAdminPlayer => typeof(NullStruct),
            GameManagerCommand.setPlayerTeam => typeof(NullStruct),
            GameManagerCommand.changeGameTeamId => typeof(NullStruct),
            GameManagerCommand.migrateAdminPlayer => typeof(NullStruct),
            GameManagerCommand.registerDynamicDedicatedServerCreator => typeof(NullStruct),
            GameManagerCommand.unregisterDynamicDedicatedServerCreator => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(GameManagerCommand command) => command switch
        {
            GameManagerCommand.createGame => typeof(NullStruct),
            GameManagerCommand.destroyGame => typeof(NullStruct),
            GameManagerCommand.advanceGameState => typeof(NullStruct),
            GameManagerCommand.setGameSettings => typeof(NullStruct),
            GameManagerCommand.setPlayerCapacity => typeof(NullStruct),
            GameManagerCommand.setGameAttributes => typeof(NullStruct),
            GameManagerCommand.setPlayerAttributes => typeof(NullStruct),
            GameManagerCommand.joinGame => typeof(EntryCriteriaError),
            GameManagerCommand.updatePlayerConnection => typeof(NullStruct),
            GameManagerCommand.removePlayer => typeof(NullStruct),
            GameManagerCommand.startMatchmaking => typeof(MatchmakingCriteriaError),
            GameManagerCommand.cancelMatchmaking => typeof(NullStruct),
            GameManagerCommand.finalizeGameCreation => typeof(NullStruct),
            GameManagerCommand.updateHostConnection => typeof(NullStruct),
            GameManagerCommand.listGames => typeof(NullStruct),
            GameManagerCommand.setPlayerCustomData => typeof(NullStruct),
            GameManagerCommand.replayGame => typeof(NullStruct),
            GameManagerCommand.returnDedicatedServerToPool => typeof(NullStruct),
            GameManagerCommand.joinGameByGroup => typeof(EntryCriteriaError),
            GameManagerCommand.leaveGameByGroup => typeof(NullStruct),
            GameManagerCommand.migrateGame => typeof(NullStruct),
            GameManagerCommand.resetDedicatedServer => typeof(NullStruct),
            GameManagerCommand.updateGameSession => typeof(NullStruct),
            GameManagerCommand.banPlayer => typeof(NullStruct),
            GameManagerCommand.matchmakingDedicatedServerOverride => typeof(NullStruct),
            GameManagerCommand.updateMeshConnection => typeof(NullStruct),
            GameManagerCommand.joinGameByUserList => typeof(NullStruct),
            GameManagerCommand.getGameListSnapshot => typeof(MatchmakingCriteriaError),
            GameManagerCommand.getGameListSubscription => typeof(MatchmakingCriteriaError),
            GameManagerCommand.destroyGameList => typeof(NullStruct),
            GameManagerCommand.getFullGameData => typeof(NullStruct),
            GameManagerCommand.getMatchmakingConfig => typeof(NullStruct),
            GameManagerCommand.getGameDataFromId => typeof(NullStruct),
            GameManagerCommand.addAdminPlayer => typeof(NullStruct),
            GameManagerCommand.removeAdminPlayer => typeof(NullStruct),
            GameManagerCommand.setPlayerTeam => typeof(NullStruct),
            GameManagerCommand.changeGameTeamId => typeof(NullStruct),
            GameManagerCommand.migrateAdminPlayer => typeof(NullStruct),
            GameManagerCommand.registerDynamicDedicatedServerCreator => typeof(NullStruct),
            GameManagerCommand.unregisterDynamicDedicatedServerCreator => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(GameManagerNotification notification) => notification switch
        {
            GameManagerNotification.NotifyMatchmakingFinished => typeof(NotifyMatchmakingFinished),
            GameManagerNotification.NotifyMatchmakingAsyncStatus => typeof(NotifyMatchmakingAsyncStatus),
            GameManagerNotification.NotifyGameCreated => typeof(NotifyGameCreated),
            GameManagerNotification.NotifyGameRemoved => typeof(NotifyGameRemoved),
            GameManagerNotification.NotifyJoinGame => typeof(NotifyJoinGame),
            GameManagerNotification.NotifyPlayerJoining => typeof(NotifyPlayerJoining),
            GameManagerNotification.NotifyPlayerJoinCompleted => typeof(NotifyPlayerJoinCompleted),
            GameManagerNotification.NotifyGroupPreJoinedGame => typeof(NotifyGroupPreJoinedGame),
            GameManagerNotification.NotifyPlayerRemoved => typeof(NotifyPlayerRemoved),
            GameManagerNotification.NotifyQueueChanged => typeof(NotifyQueueChanged),
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
            GameManagerNotification.NotifyGameListUpdate => typeof(NotifyGameListUpdate),
            GameManagerNotification.NotifyAdminListChange => typeof(NotifyAdminListChange),
            GameManagerNotification.NotifyCreateDynamicDedicatedServerGame => typeof(NotifyCreateDynamicDedicatedServerGame),
            _ => typeof(NullStruct)
        };
        
        public enum GameManagerCommand : ushort
        {
            createGame = 1,
            destroyGame = 2,
            advanceGameState = 3,
            setGameSettings = 4,
            setPlayerCapacity = 5,
            setGameAttributes = 7,
            setPlayerAttributes = 8,
            joinGame = 9,
            updatePlayerConnection = 10,
            removePlayer = 11,
            startMatchmaking = 13,
            cancelMatchmaking = 14,
            finalizeGameCreation = 15,
            updateHostConnection = 16,
            listGames = 17,
            setPlayerCustomData = 18,
            replayGame = 19,
            returnDedicatedServerToPool = 20,
            joinGameByGroup = 21,
            leaveGameByGroup = 22,
            migrateGame = 23,
            resetDedicatedServer = 25,
            updateGameSession = 26,
            banPlayer = 27,
            matchmakingDedicatedServerOverride = 28,
            updateMeshConnection = 29,
            joinGameByUserList = 30,
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
            registerDynamicDedicatedServerCreator = 150,
            unregisterDynamicDedicatedServerCreator = 151,
        }
        
        public enum GameManagerNotification : ushort
        {
            NotifyMatchmakingFinished = 10,
            NotifyMatchmakingAsyncStatus = 12,
            NotifyGameCreated = 15,
            NotifyGameRemoved = 16,
            NotifyJoinGame = 20,
            NotifyPlayerJoining = 21,
            NotifyPlayerJoinCompleted = 30,
            NotifyGroupPreJoinedGame = 35,
            NotifyPlayerRemoved = 40,
            NotifyQueueChanged = 41,
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
            NotifyGameListUpdate = 201,
            NotifyAdminListChange = 202,
            NotifyCreateDynamicDedicatedServerGame = 220,
        }
        
    }
}
