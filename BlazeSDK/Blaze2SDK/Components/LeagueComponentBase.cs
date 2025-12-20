using Blaze2SDK.Blaze.League;
using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class LeagueComponentBase
    {
        public const ushort Id = 13;
        public const string Name = "LeagueComponent";
        
        public class Server : BlazeServerComponent<LeagueComponentCommand, LeagueComponentNotification, Blaze2RpcError>
        {
            public Server() : base(LeagueComponentBase.Id, LeagueComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.createLeague)]
            public virtual Task<NullStruct> CreateLeagueAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.joinLeague)]
            public virtual Task<NullStruct> JoinLeagueAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getLeague)]
            public virtual Task<NullStruct> GetLeagueAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getLeaguesByUser)]
            public virtual Task<NullStruct> GetLeaguesByUserAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.deleteLeague)]
            public virtual Task<NullStruct> DeleteLeagueAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.promoteToGM)]
            public virtual Task<NullStruct> PromoteToGMAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.findLeagues)]
            public virtual Task<NullStruct> FindLeaguesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.findLeaguesAsync)]
            public virtual Task<NullStruct> FindLeaguesAsyncAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.removeMember)]
            public virtual Task<NullStruct> RemoveMemberAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.resetLeague)]
            public virtual Task<NullStruct> ResetLeagueAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.updateLeagueSettings)]
            public virtual Task<NullStruct> UpdateLeagueSettingsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.setMetadata)]
            public virtual Task<NullStruct> SetMetadataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.postNews)]
            public virtual Task<NullStruct> PostNewsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getNews)]
            public virtual Task<NullStruct> GetNewsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.setRoster)]
            public virtual Task<NullStruct> SetRosterAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.sendInvitation)]
            public virtual Task<NullStruct> SendInvitationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getInvitations)]
            public virtual Task<NullStruct> GetInvitationsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.processInvitation)]
            public virtual Task<NullStruct> ProcessInvitationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.proposeTrade)]
            public virtual Task<NullStruct> ProposeTradeAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.processTrade)]
            public virtual Task<NullStruct> ProcessTradeAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getTrades)]
            public virtual Task<NullStruct> GetTradesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getMembers)]
            public virtual Task<NullStruct> GetMembersAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.submitStatistics)]
            public virtual Task<NullStruct> SubmitStatisticsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getRecentGames)]
            public virtual Task<NullStruct> GetRecentGamesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.submitScores)]
            public virtual Task<NullStruct> SubmitScoresAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getRoster)]
            public virtual Task<NullStruct> GetRosterAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.runDraft)]
            public virtual Task<NullStruct> RunDraftAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getDraftProfile)]
            public virtual Task<NullStruct> GetDraftProfileAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.setDraftProfile)]
            public virtual Task<NullStruct> SetDraftProfileAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getPlayoffSeries)]
            public virtual Task<NullStruct> GetPlayoffSeriesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyFindLeaguesAsyncNotificationAsync(BlazeServerConnection connection, FindLeaguesAsyncNotification notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(LeagueComponentBase.Id, (ushort)LeagueComponentNotification.FindLeaguesAsyncNotification, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(LeagueComponentCommand command) => LeagueComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(LeagueComponentCommand command) => LeagueComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(LeagueComponentCommand command) => LeagueComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(LeagueComponentNotification notification) => LeagueComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<LeagueComponentCommand, LeagueComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(LeagueComponentBase.Id, LeagueComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct CreateLeague()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.createLeague, new NullStruct());
            }
            public Task<NullStruct> CreateLeagueAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.createLeague, new NullStruct());
            }
            
            public NullStruct JoinLeague()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.joinLeague, new NullStruct());
            }
            public Task<NullStruct> JoinLeagueAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.joinLeague, new NullStruct());
            }
            
            public NullStruct GetLeague()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getLeague, new NullStruct());
            }
            public Task<NullStruct> GetLeagueAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getLeague, new NullStruct());
            }
            
            public NullStruct GetLeaguesByUser()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getLeaguesByUser, new NullStruct());
            }
            public Task<NullStruct> GetLeaguesByUserAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getLeaguesByUser, new NullStruct());
            }
            
            public NullStruct DeleteLeague()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.deleteLeague, new NullStruct());
            }
            public Task<NullStruct> DeleteLeagueAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.deleteLeague, new NullStruct());
            }
            
            public NullStruct PromoteToGM()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.promoteToGM, new NullStruct());
            }
            public Task<NullStruct> PromoteToGMAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.promoteToGM, new NullStruct());
            }
            
            public NullStruct FindLeagues()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.findLeagues, new NullStruct());
            }
            public Task<NullStruct> FindLeaguesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.findLeagues, new NullStruct());
            }
            
            public NullStruct FindLeaguesAsynchronously()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.findLeaguesAsync, new NullStruct());
            }
            public Task<NullStruct> FindLeaguesAsynchronouslyAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.findLeaguesAsync, new NullStruct());
            }
            
            public NullStruct RemoveMember()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.removeMember, new NullStruct());
            }
            public Task<NullStruct> RemoveMemberAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.removeMember, new NullStruct());
            }
            
            public NullStruct ResetLeague()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.resetLeague, new NullStruct());
            }
            public Task<NullStruct> ResetLeagueAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.resetLeague, new NullStruct());
            }
            
            public NullStruct UpdateLeagueSettings()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.updateLeagueSettings, new NullStruct());
            }
            public Task<NullStruct> UpdateLeagueSettingsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.updateLeagueSettings, new NullStruct());
            }
            
            public NullStruct SetMetadata()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.setMetadata, new NullStruct());
            }
            public Task<NullStruct> SetMetadataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.setMetadata, new NullStruct());
            }
            
            public NullStruct PostNews()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.postNews, new NullStruct());
            }
            public Task<NullStruct> PostNewsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.postNews, new NullStruct());
            }
            
            public NullStruct GetNews()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getNews, new NullStruct());
            }
            public Task<NullStruct> GetNewsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getNews, new NullStruct());
            }
            
            public NullStruct SetRoster()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.setRoster, new NullStruct());
            }
            public Task<NullStruct> SetRosterAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.setRoster, new NullStruct());
            }
            
            public NullStruct SendInvitation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.sendInvitation, new NullStruct());
            }
            public Task<NullStruct> SendInvitationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.sendInvitation, new NullStruct());
            }
            
            public NullStruct GetInvitations()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getInvitations, new NullStruct());
            }
            public Task<NullStruct> GetInvitationsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getInvitations, new NullStruct());
            }
            
            public NullStruct ProcessInvitation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.processInvitation, new NullStruct());
            }
            public Task<NullStruct> ProcessInvitationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.processInvitation, new NullStruct());
            }
            
            public NullStruct ProposeTrade()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.proposeTrade, new NullStruct());
            }
            public Task<NullStruct> ProposeTradeAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.proposeTrade, new NullStruct());
            }
            
            public NullStruct ProcessTrade()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.processTrade, new NullStruct());
            }
            public Task<NullStruct> ProcessTradeAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.processTrade, new NullStruct());
            }
            
            public NullStruct GetTrades()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getTrades, new NullStruct());
            }
            public Task<NullStruct> GetTradesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getTrades, new NullStruct());
            }
            
            public NullStruct GetMembers()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getMembers, new NullStruct());
            }
            public Task<NullStruct> GetMembersAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getMembers, new NullStruct());
            }
            
            public NullStruct SubmitStatistics()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.submitStatistics, new NullStruct());
            }
            public Task<NullStruct> SubmitStatisticsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.submitStatistics, new NullStruct());
            }
            
            public NullStruct GetRecentGames()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getRecentGames, new NullStruct());
            }
            public Task<NullStruct> GetRecentGamesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getRecentGames, new NullStruct());
            }
            
            public NullStruct SubmitScores()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.submitScores, new NullStruct());
            }
            public Task<NullStruct> SubmitScoresAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.submitScores, new NullStruct());
            }
            
            public NullStruct GetRoster()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getRoster, new NullStruct());
            }
            public Task<NullStruct> GetRosterAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getRoster, new NullStruct());
            }
            
            public NullStruct RunDraft()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.runDraft, new NullStruct());
            }
            public Task<NullStruct> RunDraftAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.runDraft, new NullStruct());
            }
            
            public NullStruct GetDraftProfile()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getDraftProfile, new NullStruct());
            }
            public Task<NullStruct> GetDraftProfileAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getDraftProfile, new NullStruct());
            }
            
            public NullStruct SetDraftProfile()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.setDraftProfile, new NullStruct());
            }
            public Task<NullStruct> SetDraftProfileAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.setDraftProfile, new NullStruct());
            }
            
            public NullStruct GetPlayoffSeries()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getPlayoffSeries, new NullStruct());
            }
            public Task<NullStruct> GetPlayoffSeriesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getPlayoffSeries, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)LeagueComponentNotification.FindLeaguesAsyncNotification)]
            public virtual Task OnFindLeaguesAsyncNotificationAsync(FindLeaguesAsyncNotification notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnFindLeaguesAsyncNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(LeagueComponentCommand command) => LeagueComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(LeagueComponentCommand command) => LeagueComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(LeagueComponentCommand command) => LeagueComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(LeagueComponentNotification notification) => LeagueComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<LeagueComponentCommand, LeagueComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(LeagueComponentBase.Id, LeagueComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.createLeague)]
            public virtual Task<NullStruct> CreateLeagueAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.createLeague, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.joinLeague)]
            public virtual Task<NullStruct> JoinLeagueAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.joinLeague, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getLeague)]
            public virtual Task<NullStruct> GetLeagueAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getLeague, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getLeaguesByUser)]
            public virtual Task<NullStruct> GetLeaguesByUserAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getLeaguesByUser, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.deleteLeague)]
            public virtual Task<NullStruct> DeleteLeagueAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.deleteLeague, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.promoteToGM)]
            public virtual Task<NullStruct> PromoteToGMAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.promoteToGM, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.findLeagues)]
            public virtual Task<NullStruct> FindLeaguesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.findLeagues, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.findLeaguesAsync)]
            public virtual Task<NullStruct> FindLeaguesAsyncAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.findLeaguesAsync, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.removeMember)]
            public virtual Task<NullStruct> RemoveMemberAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.removeMember, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.resetLeague)]
            public virtual Task<NullStruct> ResetLeagueAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.resetLeague, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.updateLeagueSettings)]
            public virtual Task<NullStruct> UpdateLeagueSettingsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.updateLeagueSettings, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.setMetadata)]
            public virtual Task<NullStruct> SetMetadataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.setMetadata, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.postNews)]
            public virtual Task<NullStruct> PostNewsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.postNews, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getNews)]
            public virtual Task<NullStruct> GetNewsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getNews, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.setRoster)]
            public virtual Task<NullStruct> SetRosterAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.setRoster, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.sendInvitation)]
            public virtual Task<NullStruct> SendInvitationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.sendInvitation, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getInvitations)]
            public virtual Task<NullStruct> GetInvitationsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getInvitations, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.processInvitation)]
            public virtual Task<NullStruct> ProcessInvitationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.processInvitation, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.proposeTrade)]
            public virtual Task<NullStruct> ProposeTradeAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.proposeTrade, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.processTrade)]
            public virtual Task<NullStruct> ProcessTradeAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.processTrade, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getTrades)]
            public virtual Task<NullStruct> GetTradesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getTrades, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getMembers)]
            public virtual Task<NullStruct> GetMembersAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getMembers, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.submitStatistics)]
            public virtual Task<NullStruct> SubmitStatisticsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.submitStatistics, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getRecentGames)]
            public virtual Task<NullStruct> GetRecentGamesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getRecentGames, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.submitScores)]
            public virtual Task<NullStruct> SubmitScoresAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.submitScores, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getRoster)]
            public virtual Task<NullStruct> GetRosterAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getRoster, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.runDraft)]
            public virtual Task<NullStruct> RunDraftAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.runDraft, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getDraftProfile)]
            public virtual Task<NullStruct> GetDraftProfileAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getDraftProfile, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.setDraftProfile)]
            public virtual Task<NullStruct> SetDraftProfileAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.setDraftProfile, request);
            }
            
            [BlazeCommand((ushort)LeagueComponentCommand.getPlayoffSeries)]
            public virtual Task<NullStruct> GetPlayoffSeriesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LeagueComponentCommand.getPlayoffSeries, request);
            }
            
            
            [BlazeNotification((ushort)LeagueComponentNotification.FindLeaguesAsyncNotification)]
            public virtual Task<FindLeaguesAsyncNotification> OnFindLeaguesAsyncNotificationAsync(FindLeaguesAsyncNotification notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(LeagueComponentCommand command) => LeagueComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(LeagueComponentCommand command) => LeagueComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(LeagueComponentCommand command) => LeagueComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(LeagueComponentNotification notification) => LeagueComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(LeagueComponentCommand command) => command switch
        {
            LeagueComponentCommand.createLeague => typeof(NullStruct),
            LeagueComponentCommand.joinLeague => typeof(NullStruct),
            LeagueComponentCommand.getLeague => typeof(NullStruct),
            LeagueComponentCommand.getLeaguesByUser => typeof(NullStruct),
            LeagueComponentCommand.deleteLeague => typeof(NullStruct),
            LeagueComponentCommand.promoteToGM => typeof(NullStruct),
            LeagueComponentCommand.findLeagues => typeof(NullStruct),
            LeagueComponentCommand.findLeaguesAsync => typeof(NullStruct),
            LeagueComponentCommand.removeMember => typeof(NullStruct),
            LeagueComponentCommand.resetLeague => typeof(NullStruct),
            LeagueComponentCommand.updateLeagueSettings => typeof(NullStruct),
            LeagueComponentCommand.setMetadata => typeof(NullStruct),
            LeagueComponentCommand.postNews => typeof(NullStruct),
            LeagueComponentCommand.getNews => typeof(NullStruct),
            LeagueComponentCommand.setRoster => typeof(NullStruct),
            LeagueComponentCommand.sendInvitation => typeof(NullStruct),
            LeagueComponentCommand.getInvitations => typeof(NullStruct),
            LeagueComponentCommand.processInvitation => typeof(NullStruct),
            LeagueComponentCommand.proposeTrade => typeof(NullStruct),
            LeagueComponentCommand.processTrade => typeof(NullStruct),
            LeagueComponentCommand.getTrades => typeof(NullStruct),
            LeagueComponentCommand.getMembers => typeof(NullStruct),
            LeagueComponentCommand.submitStatistics => typeof(NullStruct),
            LeagueComponentCommand.getRecentGames => typeof(NullStruct),
            LeagueComponentCommand.submitScores => typeof(NullStruct),
            LeagueComponentCommand.getRoster => typeof(NullStruct),
            LeagueComponentCommand.runDraft => typeof(NullStruct),
            LeagueComponentCommand.getDraftProfile => typeof(NullStruct),
            LeagueComponentCommand.setDraftProfile => typeof(NullStruct),
            LeagueComponentCommand.getPlayoffSeries => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(LeagueComponentCommand command) => command switch
        {
            LeagueComponentCommand.createLeague => typeof(NullStruct),
            LeagueComponentCommand.joinLeague => typeof(NullStruct),
            LeagueComponentCommand.getLeague => typeof(NullStruct),
            LeagueComponentCommand.getLeaguesByUser => typeof(NullStruct),
            LeagueComponentCommand.deleteLeague => typeof(NullStruct),
            LeagueComponentCommand.promoteToGM => typeof(NullStruct),
            LeagueComponentCommand.findLeagues => typeof(NullStruct),
            LeagueComponentCommand.findLeaguesAsync => typeof(NullStruct),
            LeagueComponentCommand.removeMember => typeof(NullStruct),
            LeagueComponentCommand.resetLeague => typeof(NullStruct),
            LeagueComponentCommand.updateLeagueSettings => typeof(NullStruct),
            LeagueComponentCommand.setMetadata => typeof(NullStruct),
            LeagueComponentCommand.postNews => typeof(NullStruct),
            LeagueComponentCommand.getNews => typeof(NullStruct),
            LeagueComponentCommand.setRoster => typeof(NullStruct),
            LeagueComponentCommand.sendInvitation => typeof(NullStruct),
            LeagueComponentCommand.getInvitations => typeof(NullStruct),
            LeagueComponentCommand.processInvitation => typeof(NullStruct),
            LeagueComponentCommand.proposeTrade => typeof(NullStruct),
            LeagueComponentCommand.processTrade => typeof(NullStruct),
            LeagueComponentCommand.getTrades => typeof(NullStruct),
            LeagueComponentCommand.getMembers => typeof(NullStruct),
            LeagueComponentCommand.submitStatistics => typeof(NullStruct),
            LeagueComponentCommand.getRecentGames => typeof(NullStruct),
            LeagueComponentCommand.submitScores => typeof(NullStruct),
            LeagueComponentCommand.getRoster => typeof(NullStruct),
            LeagueComponentCommand.runDraft => typeof(NullStruct),
            LeagueComponentCommand.getDraftProfile => typeof(NullStruct),
            LeagueComponentCommand.setDraftProfile => typeof(NullStruct),
            LeagueComponentCommand.getPlayoffSeries => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(LeagueComponentCommand command) => command switch
        {
            LeagueComponentCommand.createLeague => typeof(NullStruct),
            LeagueComponentCommand.joinLeague => typeof(NullStruct),
            LeagueComponentCommand.getLeague => typeof(NullStruct),
            LeagueComponentCommand.getLeaguesByUser => typeof(NullStruct),
            LeagueComponentCommand.deleteLeague => typeof(NullStruct),
            LeagueComponentCommand.promoteToGM => typeof(NullStruct),
            LeagueComponentCommand.findLeagues => typeof(NullStruct),
            LeagueComponentCommand.findLeaguesAsync => typeof(NullStruct),
            LeagueComponentCommand.removeMember => typeof(NullStruct),
            LeagueComponentCommand.resetLeague => typeof(NullStruct),
            LeagueComponentCommand.updateLeagueSettings => typeof(NullStruct),
            LeagueComponentCommand.setMetadata => typeof(NullStruct),
            LeagueComponentCommand.postNews => typeof(NullStruct),
            LeagueComponentCommand.getNews => typeof(NullStruct),
            LeagueComponentCommand.setRoster => typeof(NullStruct),
            LeagueComponentCommand.sendInvitation => typeof(NullStruct),
            LeagueComponentCommand.getInvitations => typeof(NullStruct),
            LeagueComponentCommand.processInvitation => typeof(NullStruct),
            LeagueComponentCommand.proposeTrade => typeof(NullStruct),
            LeagueComponentCommand.processTrade => typeof(NullStruct),
            LeagueComponentCommand.getTrades => typeof(NullStruct),
            LeagueComponentCommand.getMembers => typeof(NullStruct),
            LeagueComponentCommand.submitStatistics => typeof(NullStruct),
            LeagueComponentCommand.getRecentGames => typeof(NullStruct),
            LeagueComponentCommand.submitScores => typeof(NullStruct),
            LeagueComponentCommand.getRoster => typeof(NullStruct),
            LeagueComponentCommand.runDraft => typeof(NullStruct),
            LeagueComponentCommand.getDraftProfile => typeof(NullStruct),
            LeagueComponentCommand.setDraftProfile => typeof(NullStruct),
            LeagueComponentCommand.getPlayoffSeries => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(LeagueComponentNotification notification) => notification switch
        {
            LeagueComponentNotification.FindLeaguesAsyncNotification => typeof(FindLeaguesAsyncNotification),
            _ => typeof(NullStruct)
        };
        
        public enum LeagueComponentCommand : ushort
        {
            createLeague = 1,
            joinLeague = 2,
            getLeague = 3,
            getLeaguesByUser = 4,
            deleteLeague = 5,
            promoteToGM = 7,
            findLeagues = 8,
            findLeaguesAsync = 9,
            removeMember = 10,
            resetLeague = 11,
            updateLeagueSettings = 12,
            setMetadata = 13,
            postNews = 15,
            getNews = 16,
            setRoster = 20,
            sendInvitation = 22,
            getInvitations = 23,
            processInvitation = 24,
            proposeTrade = 26,
            processTrade = 27,
            getTrades = 29,
            getMembers = 31,
            submitStatistics = 103,
            getRecentGames = 104,
            submitScores = 105,
            getRoster = 107,
            runDraft = 109,
            getDraftProfile = 112,
            setDraftProfile = 113,
            getPlayoffSeries = 116,
        }
        
        public enum LeagueComponentNotification : ushort
        {
            FindLeaguesAsyncNotification = 118,
        }
        
    }
}
