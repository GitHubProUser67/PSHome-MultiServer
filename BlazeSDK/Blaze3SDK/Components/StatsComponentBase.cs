using Blaze3SDK.Blaze.Stats;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class StatsComponentBase
    {
        public const ushort Id = 7;
        public const string Name = "StatsComponent";
        
        public class Server : BlazeServerComponent<StatsComponentCommand, StatsComponentNotification, Blaze3RpcError>
        {
            public Server() : base(StatsComponentBase.Id, StatsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatDescs)]
            public virtual Task<StatDescs> GetStatDescsAsync(GetStatDescsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStats)]
            public virtual Task<NullStruct> GetStatsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatGroupList)]
            public virtual Task<StatGroupList> GetStatGroupListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatGroup)]
            public virtual Task<StatGroupResponse> GetStatGroupAsync(GetStatGroupRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatsByGroup)]
            public virtual Task<NullStruct> GetStatsByGroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getDateRange)]
            public virtual Task<NullStruct> GetDateRangeAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getEntityCount)]
            public virtual Task<NullStruct> GetEntityCountAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboardGroup)]
            public virtual Task<LeaderboardGroupResponse> GetLeaderboardGroupAsync(LeaderboardGroupRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboardFolderGroup)]
            public virtual Task<LeaderboardFolderGroup> GetLeaderboardFolderGroupAsync(LeaderboardFolderGroupRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboard)]
            public virtual Task<NullStruct> GetLeaderboardAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getCenteredLeaderboard)]
            public virtual Task<NullStruct> GetCenteredLeaderboardAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getFilteredLeaderboard)]
            public virtual Task<NullStruct> GetFilteredLeaderboardAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getKeyScopesMap)]
            public virtual Task<KeyScopes> GetKeyScopesMapAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatsByGroupAsync)]
            public virtual Task<NullStruct> GetStatsByGroupAsyncAsync(GetStatsByGroupRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboardTreeAsync)]
            public virtual Task<NullStruct> GetLeaderboardTreeAsyncAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboardEntityCount)]
            public virtual Task<NullStruct> GetLeaderboardEntityCountAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatCategoryList)]
            public virtual Task<StatCategoryList> GetStatCategoryListAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getPeriodIds)]
            public virtual Task<PeriodIds> GetPeriodIdsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboardRaw)]
            public virtual Task<NullStruct> GetLeaderboardRawAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getCenteredLeaderboardRaw)]
            public virtual Task<NullStruct> GetCenteredLeaderboardRawAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getFilteredLeaderboardRaw)]
            public virtual Task<NullStruct> GetFilteredLeaderboardRawAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.changeKeyscopeValue)]
            public virtual Task<NullStruct> ChangeKeyscopeValueAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyGetStatsAsyncNotificationAsync(BlazeServerConnection connection, KeyScopedStatValues notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(StatsComponentBase.Id, (ushort)StatsComponentNotification.GetStatsAsyncNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyGetLeaderboardTreeNotificationAsync(BlazeServerConnection connection, LeaderboardTreeNode notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(StatsComponentBase.Id, (ushort)StatsComponentNotification.GetLeaderboardTreeNotification, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(StatsComponentCommand command) => StatsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(StatsComponentCommand command) => StatsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(StatsComponentCommand command) => StatsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(StatsComponentNotification notification) => StatsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<StatsComponentCommand, StatsComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(StatsComponentBase.Id, StatsComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public StatDescs GetStatDescs(GetStatDescsRequest request)
            {
                return Connection.SendRequest<GetStatDescsRequest, StatDescs, NullStruct>(this, (ushort)StatsComponentCommand.getStatDescs, request);
            }
            public Task<StatDescs> GetStatDescsAsync(GetStatDescsRequest request)
            {
                return Connection.SendRequestAsync<GetStatDescsRequest, StatDescs, NullStruct>(this, (ushort)StatsComponentCommand.getStatDescs, request);
            }
            
            public NullStruct GetStats()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getStats, new NullStruct());
            }
            public Task<NullStruct> GetStatsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getStats, new NullStruct());
            }
            
            public StatGroupList GetStatGroupList()
            {
                return Connection.SendRequest<NullStruct, StatGroupList, NullStruct>(this, (ushort)StatsComponentCommand.getStatGroupList, new NullStruct());
            }
            public Task<StatGroupList> GetStatGroupListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, StatGroupList, NullStruct>(this, (ushort)StatsComponentCommand.getStatGroupList, new NullStruct());
            }
            
            public StatGroupResponse GetStatGroup(GetStatGroupRequest request)
            {
                return Connection.SendRequest<GetStatGroupRequest, StatGroupResponse, NullStruct>(this, (ushort)StatsComponentCommand.getStatGroup, request);
            }
            public Task<StatGroupResponse> GetStatGroupAsync(GetStatGroupRequest request)
            {
                return Connection.SendRequestAsync<GetStatGroupRequest, StatGroupResponse, NullStruct>(this, (ushort)StatsComponentCommand.getStatGroup, request);
            }
            
            public NullStruct GetStatsByGroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getStatsByGroup, new NullStruct());
            }
            public Task<NullStruct> GetStatsByGroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getStatsByGroup, new NullStruct());
            }
            
            public NullStruct GetDateRange()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getDateRange, new NullStruct());
            }
            public Task<NullStruct> GetDateRangeAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getDateRange, new NullStruct());
            }
            
            public NullStruct GetEntityCount()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getEntityCount, new NullStruct());
            }
            public Task<NullStruct> GetEntityCountAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getEntityCount, new NullStruct());
            }
            
            public LeaderboardGroupResponse GetLeaderboardGroup(LeaderboardGroupRequest request)
            {
                return Connection.SendRequest<LeaderboardGroupRequest, LeaderboardGroupResponse, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardGroup, request);
            }
            public Task<LeaderboardGroupResponse> GetLeaderboardGroupAsync(LeaderboardGroupRequest request)
            {
                return Connection.SendRequestAsync<LeaderboardGroupRequest, LeaderboardGroupResponse, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardGroup, request);
            }
            
            public LeaderboardFolderGroup GetLeaderboardFolderGroup(LeaderboardFolderGroupRequest request)
            {
                return Connection.SendRequest<LeaderboardFolderGroupRequest, LeaderboardFolderGroup, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardFolderGroup, request);
            }
            public Task<LeaderboardFolderGroup> GetLeaderboardFolderGroupAsync(LeaderboardFolderGroupRequest request)
            {
                return Connection.SendRequestAsync<LeaderboardFolderGroupRequest, LeaderboardFolderGroup, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardFolderGroup, request);
            }
            
            public NullStruct GetLeaderboard()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboard, new NullStruct());
            }
            public Task<NullStruct> GetLeaderboardAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboard, new NullStruct());
            }
            
            public NullStruct GetCenteredLeaderboard()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getCenteredLeaderboard, new NullStruct());
            }
            public Task<NullStruct> GetCenteredLeaderboardAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getCenteredLeaderboard, new NullStruct());
            }
            
            public NullStruct GetFilteredLeaderboard()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getFilteredLeaderboard, new NullStruct());
            }
            public Task<NullStruct> GetFilteredLeaderboardAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getFilteredLeaderboard, new NullStruct());
            }
            
            public KeyScopes GetKeyScopesMap()
            {
                return Connection.SendRequest<NullStruct, KeyScopes, NullStruct>(this, (ushort)StatsComponentCommand.getKeyScopesMap, new NullStruct());
            }
            public Task<KeyScopes> GetKeyScopesMapAsync()
            {
                return Connection.SendRequestAsync<NullStruct, KeyScopes, NullStruct>(this, (ushort)StatsComponentCommand.getKeyScopesMap, new NullStruct());
            }
            
            public NullStruct GetStatsByGroupAsynchronously(GetStatsByGroupRequest request)
            {
                return Connection.SendRequest<GetStatsByGroupRequest, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getStatsByGroupAsync, request);
            }
            public Task<NullStruct> GetStatsByGroupAsynchronouslyAsync(GetStatsByGroupRequest request)
            {
                return Connection.SendRequestAsync<GetStatsByGroupRequest, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getStatsByGroupAsync, request);
            }
            
            public NullStruct GetLeaderboardTreeAsynchronously()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardTreeAsync, new NullStruct());
            }
            public Task<NullStruct> GetLeaderboardTreeAsynchronouslyAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardTreeAsync, new NullStruct());
            }
            
            public NullStruct GetLeaderboardEntityCount()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardEntityCount, new NullStruct());
            }
            public Task<NullStruct> GetLeaderboardEntityCountAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardEntityCount, new NullStruct());
            }
            
            public StatCategoryList GetStatCategoryList()
            {
                return Connection.SendRequest<NullStruct, StatCategoryList, NullStruct>(this, (ushort)StatsComponentCommand.getStatCategoryList, new NullStruct());
            }
            public Task<StatCategoryList> GetStatCategoryListAsync()
            {
                return Connection.SendRequestAsync<NullStruct, StatCategoryList, NullStruct>(this, (ushort)StatsComponentCommand.getStatCategoryList, new NullStruct());
            }
            
            public PeriodIds GetPeriodIds()
            {
                return Connection.SendRequest<NullStruct, PeriodIds, NullStruct>(this, (ushort)StatsComponentCommand.getPeriodIds, new NullStruct());
            }
            public Task<PeriodIds> GetPeriodIdsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, PeriodIds, NullStruct>(this, (ushort)StatsComponentCommand.getPeriodIds, new NullStruct());
            }
            
            public NullStruct GetLeaderboardRaw()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardRaw, new NullStruct());
            }
            public Task<NullStruct> GetLeaderboardRawAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardRaw, new NullStruct());
            }
            
            public NullStruct GetCenteredLeaderboardRaw()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getCenteredLeaderboardRaw, new NullStruct());
            }
            public Task<NullStruct> GetCenteredLeaderboardRawAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getCenteredLeaderboardRaw, new NullStruct());
            }
            
            public NullStruct GetFilteredLeaderboardRaw()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getFilteredLeaderboardRaw, new NullStruct());
            }
            public Task<NullStruct> GetFilteredLeaderboardRawAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getFilteredLeaderboardRaw, new NullStruct());
            }
            
            public NullStruct ChangeKeyscopeValue()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.changeKeyscopeValue, new NullStruct());
            }
            public Task<NullStruct> ChangeKeyscopeValueAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.changeKeyscopeValue, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)StatsComponentNotification.GetStatsAsyncNotification)]
            public virtual Task OnGetStatsAsyncNotificationAsync(KeyScopedStatValues notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnGetStatsAsyncNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)StatsComponentNotification.GetLeaderboardTreeNotification)]
            public virtual Task OnGetLeaderboardTreeNotificationAsync(LeaderboardTreeNode notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnGetLeaderboardTreeNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(StatsComponentCommand command) => StatsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(StatsComponentCommand command) => StatsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(StatsComponentCommand command) => StatsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(StatsComponentNotification notification) => StatsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<StatsComponentCommand, StatsComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(StatsComponentBase.Id, StatsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatDescs)]
            public virtual Task<StatDescs> GetStatDescsAsync(GetStatDescsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetStatDescsRequest, StatDescs, NullStruct>(this, (ushort)StatsComponentCommand.getStatDescs, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStats)]
            public virtual Task<NullStruct> GetStatsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getStats, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatGroupList)]
            public virtual Task<StatGroupList> GetStatGroupListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, StatGroupList, NullStruct>(this, (ushort)StatsComponentCommand.getStatGroupList, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatGroup)]
            public virtual Task<StatGroupResponse> GetStatGroupAsync(GetStatGroupRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetStatGroupRequest, StatGroupResponse, NullStruct>(this, (ushort)StatsComponentCommand.getStatGroup, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatsByGroup)]
            public virtual Task<NullStruct> GetStatsByGroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getStatsByGroup, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getDateRange)]
            public virtual Task<NullStruct> GetDateRangeAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getDateRange, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getEntityCount)]
            public virtual Task<NullStruct> GetEntityCountAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getEntityCount, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboardGroup)]
            public virtual Task<LeaderboardGroupResponse> GetLeaderboardGroupAsync(LeaderboardGroupRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<LeaderboardGroupRequest, LeaderboardGroupResponse, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardGroup, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboardFolderGroup)]
            public virtual Task<LeaderboardFolderGroup> GetLeaderboardFolderGroupAsync(LeaderboardFolderGroupRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<LeaderboardFolderGroupRequest, LeaderboardFolderGroup, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardFolderGroup, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboard)]
            public virtual Task<NullStruct> GetLeaderboardAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboard, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getCenteredLeaderboard)]
            public virtual Task<NullStruct> GetCenteredLeaderboardAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getCenteredLeaderboard, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getFilteredLeaderboard)]
            public virtual Task<NullStruct> GetFilteredLeaderboardAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getFilteredLeaderboard, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getKeyScopesMap)]
            public virtual Task<KeyScopes> GetKeyScopesMapAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, KeyScopes, NullStruct>(this, (ushort)StatsComponentCommand.getKeyScopesMap, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatsByGroupAsync)]
            public virtual Task<NullStruct> GetStatsByGroupAsyncAsync(GetStatsByGroupRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetStatsByGroupRequest, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getStatsByGroupAsync, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboardTreeAsync)]
            public virtual Task<NullStruct> GetLeaderboardTreeAsyncAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardTreeAsync, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboardEntityCount)]
            public virtual Task<NullStruct> GetLeaderboardEntityCountAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardEntityCount, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getStatCategoryList)]
            public virtual Task<StatCategoryList> GetStatCategoryListAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, StatCategoryList, NullStruct>(this, (ushort)StatsComponentCommand.getStatCategoryList, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getPeriodIds)]
            public virtual Task<PeriodIds> GetPeriodIdsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, PeriodIds, NullStruct>(this, (ushort)StatsComponentCommand.getPeriodIds, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getLeaderboardRaw)]
            public virtual Task<NullStruct> GetLeaderboardRawAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getLeaderboardRaw, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getCenteredLeaderboardRaw)]
            public virtual Task<NullStruct> GetCenteredLeaderboardRawAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getCenteredLeaderboardRaw, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.getFilteredLeaderboardRaw)]
            public virtual Task<NullStruct> GetFilteredLeaderboardRawAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.getFilteredLeaderboardRaw, request);
            }
            
            [BlazeCommand((ushort)StatsComponentCommand.changeKeyscopeValue)]
            public virtual Task<NullStruct> ChangeKeyscopeValueAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)StatsComponentCommand.changeKeyscopeValue, request);
            }
            
            
            [BlazeNotification((ushort)StatsComponentNotification.GetStatsAsyncNotification)]
            public virtual Task<KeyScopedStatValues> OnGetStatsAsyncNotificationAsync(KeyScopedStatValues notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)StatsComponentNotification.GetLeaderboardTreeNotification)]
            public virtual Task<LeaderboardTreeNode> OnGetLeaderboardTreeNotificationAsync(LeaderboardTreeNode notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(StatsComponentCommand command) => StatsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(StatsComponentCommand command) => StatsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(StatsComponentCommand command) => StatsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(StatsComponentNotification notification) => StatsComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(StatsComponentCommand command) => command switch
        {
            StatsComponentCommand.getStatDescs => typeof(GetStatDescsRequest),
            StatsComponentCommand.getStats => typeof(NullStruct),
            StatsComponentCommand.getStatGroupList => typeof(NullStruct),
            StatsComponentCommand.getStatGroup => typeof(GetStatGroupRequest),
            StatsComponentCommand.getStatsByGroup => typeof(NullStruct),
            StatsComponentCommand.getDateRange => typeof(NullStruct),
            StatsComponentCommand.getEntityCount => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardGroup => typeof(LeaderboardGroupRequest),
            StatsComponentCommand.getLeaderboardFolderGroup => typeof(LeaderboardFolderGroupRequest),
            StatsComponentCommand.getLeaderboard => typeof(NullStruct),
            StatsComponentCommand.getCenteredLeaderboard => typeof(NullStruct),
            StatsComponentCommand.getFilteredLeaderboard => typeof(NullStruct),
            StatsComponentCommand.getKeyScopesMap => typeof(NullStruct),
            StatsComponentCommand.getStatsByGroupAsync => typeof(GetStatsByGroupRequest),
            StatsComponentCommand.getLeaderboardTreeAsync => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardEntityCount => typeof(NullStruct),
            StatsComponentCommand.getStatCategoryList => typeof(NullStruct),
            StatsComponentCommand.getPeriodIds => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardRaw => typeof(NullStruct),
            StatsComponentCommand.getCenteredLeaderboardRaw => typeof(NullStruct),
            StatsComponentCommand.getFilteredLeaderboardRaw => typeof(NullStruct),
            StatsComponentCommand.changeKeyscopeValue => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(StatsComponentCommand command) => command switch
        {
            StatsComponentCommand.getStatDescs => typeof(StatDescs),
            StatsComponentCommand.getStats => typeof(NullStruct),
            StatsComponentCommand.getStatGroupList => typeof(StatGroupList),
            StatsComponentCommand.getStatGroup => typeof(StatGroupResponse),
            StatsComponentCommand.getStatsByGroup => typeof(NullStruct),
            StatsComponentCommand.getDateRange => typeof(NullStruct),
            StatsComponentCommand.getEntityCount => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardGroup => typeof(LeaderboardGroupResponse),
            StatsComponentCommand.getLeaderboardFolderGroup => typeof(LeaderboardFolderGroup),
            StatsComponentCommand.getLeaderboard => typeof(NullStruct),
            StatsComponentCommand.getCenteredLeaderboard => typeof(NullStruct),
            StatsComponentCommand.getFilteredLeaderboard => typeof(NullStruct),
            StatsComponentCommand.getKeyScopesMap => typeof(KeyScopes),
            StatsComponentCommand.getStatsByGroupAsync => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardTreeAsync => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardEntityCount => typeof(NullStruct),
            StatsComponentCommand.getStatCategoryList => typeof(StatCategoryList),
            StatsComponentCommand.getPeriodIds => typeof(PeriodIds),
            StatsComponentCommand.getLeaderboardRaw => typeof(NullStruct),
            StatsComponentCommand.getCenteredLeaderboardRaw => typeof(NullStruct),
            StatsComponentCommand.getFilteredLeaderboardRaw => typeof(NullStruct),
            StatsComponentCommand.changeKeyscopeValue => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(StatsComponentCommand command) => command switch
        {
            StatsComponentCommand.getStatDescs => typeof(NullStruct),
            StatsComponentCommand.getStats => typeof(NullStruct),
            StatsComponentCommand.getStatGroupList => typeof(NullStruct),
            StatsComponentCommand.getStatGroup => typeof(NullStruct),
            StatsComponentCommand.getStatsByGroup => typeof(NullStruct),
            StatsComponentCommand.getDateRange => typeof(NullStruct),
            StatsComponentCommand.getEntityCount => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardGroup => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardFolderGroup => typeof(NullStruct),
            StatsComponentCommand.getLeaderboard => typeof(NullStruct),
            StatsComponentCommand.getCenteredLeaderboard => typeof(NullStruct),
            StatsComponentCommand.getFilteredLeaderboard => typeof(NullStruct),
            StatsComponentCommand.getKeyScopesMap => typeof(NullStruct),
            StatsComponentCommand.getStatsByGroupAsync => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardTreeAsync => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardEntityCount => typeof(NullStruct),
            StatsComponentCommand.getStatCategoryList => typeof(NullStruct),
            StatsComponentCommand.getPeriodIds => typeof(NullStruct),
            StatsComponentCommand.getLeaderboardRaw => typeof(NullStruct),
            StatsComponentCommand.getCenteredLeaderboardRaw => typeof(NullStruct),
            StatsComponentCommand.getFilteredLeaderboardRaw => typeof(NullStruct),
            StatsComponentCommand.changeKeyscopeValue => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(StatsComponentNotification notification) => notification switch
        {
            StatsComponentNotification.GetStatsAsyncNotification => typeof(KeyScopedStatValues),
            StatsComponentNotification.GetLeaderboardTreeNotification => typeof(LeaderboardTreeNode),
            _ => typeof(NullStruct)
        };
        
        public enum StatsComponentCommand : ushort
        {
            getStatDescs = 1,
            getStats = 2,
            getStatGroupList = 3,
            getStatGroup = 4,
            getStatsByGroup = 5,
            getDateRange = 6,
            getEntityCount = 7,
            getLeaderboardGroup = 10,
            getLeaderboardFolderGroup = 11,
            getLeaderboard = 12,
            getCenteredLeaderboard = 13,
            getFilteredLeaderboard = 14,
            getKeyScopesMap = 15,
            getStatsByGroupAsync = 16,
            getLeaderboardTreeAsync = 17,
            getLeaderboardEntityCount = 18,
            getStatCategoryList = 19,
            getPeriodIds = 20,
            getLeaderboardRaw = 21,
            getCenteredLeaderboardRaw = 22,
            getFilteredLeaderboardRaw = 23,
            changeKeyscopeValue = 24,
        }
        
        public enum StatsComponentNotification : ushort
        {
            GetStatsAsyncNotification = 50,
            GetLeaderboardTreeNotification = 51,
        }
        
    }
}
