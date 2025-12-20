using Blaze3SDK.Blaze.Clubs;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class ClubsComponentBase
    {
        public const ushort Id = 11;
        public const string Name = "ClubsComponent";
        
        public class Server : BlazeServerComponent<ClubsComponentCommand, ClubsComponentNotification, Blaze3RpcError>
        {
            public Server() : base(ClubsComponentBase.Id, ClubsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.createClub)]
            public virtual Task<NullStruct> CreateClubAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubs)]
            public virtual Task<NullStruct> GetClubsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.findClubs)]
            public virtual Task<NullStruct> FindClubsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.findClubs2)]
            public virtual Task<NullStruct> FindClubs2Async(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.removeMember)]
            public virtual Task<NullStruct> RemoveMemberAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.sendInvitation)]
            public virtual Task<NullStruct> SendInvitationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getInvitations)]
            public virtual Task<NullStruct> GetInvitationsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.revokeInvitation)]
            public virtual Task<NullStruct> RevokeInvitationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.acceptInvitation)]
            public virtual Task<NullStruct> AcceptInvitationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.declineInvitation)]
            public virtual Task<NullStruct> DeclineInvitationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getMembers)]
            public virtual Task<NullStruct> GetMembersAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.promoteToGM)]
            public virtual Task<NullStruct> PromoteToGMAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.demoteToMember)]
            public virtual Task<NullStruct> DemoteToMemberAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.updateClubSettings)]
            public virtual Task<NullStruct> UpdateClubSettingsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.postNews)]
            public virtual Task<NullStruct> PostNewsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getNews)]
            public virtual Task<NullStruct> GetNewsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.setNewsItemHidden)]
            public virtual Task<NullStruct> SetNewsItemHiddenAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.setMetadata)]
            public virtual Task<NullStruct> SetMetadataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.setMetadata2)]
            public virtual Task<NullStruct> SetMetadata2Async(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubsComponentSettings)]
            public virtual Task<NullStruct> GetClubsComponentSettingsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.transferOwnership)]
            public virtual Task<NullStruct> TransferOwnershipAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubMembershipForUsers)]
            public virtual Task<NullStruct> GetClubMembershipForUsersAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.sendPetition)]
            public virtual Task<NullStruct> SendPetitionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getPetitions)]
            public virtual Task<NullStruct> GetPetitionsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.acceptPetition)]
            public virtual Task<NullStruct> AcceptPetitionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.declinePetition)]
            public virtual Task<NullStruct> DeclinePetitionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.revokePetition)]
            public virtual Task<NullStruct> RevokePetitionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.joinClub)]
            public virtual Task<NullStruct> JoinClubAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubRecordbook)]
            public virtual Task<NullStruct> GetClubRecordbookAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.resetClubRecords)]
            public virtual Task<NullStruct> ResetClubRecordsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.updateMemberOnlineStatus)]
            public virtual Task<NullStruct> UpdateMemberOnlineStatusAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubAwards)]
            public virtual Task<NullStruct> GetClubAwardsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.updateMemberMetadata)]
            public virtual Task<NullStruct> UpdateMemberMetadataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.findClubsAsync)]
            public virtual Task<NullStruct> FindClubsAsyncAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.findClubs2Async)]
            public virtual Task<NullStruct> FindClubs2AsyncAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.listRivals)]
            public virtual Task<NullStruct> ListRivalsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubTickerMessages)]
            public virtual Task<NullStruct> GetClubTickerMessagesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.setClubTickerMessagesSubscription)]
            public virtual Task<NullStruct> SetClubTickerMessagesSubscriptionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.changeClubStrings)]
            public virtual Task<NullStruct> ChangeClubStringsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.countMessages)]
            public virtual Task<NullStruct> CountMessagesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getMembersAsync)]
            public virtual Task<NullStruct> GetMembersAsyncAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubBans)]
            public virtual Task<NullStruct> GetClubBansAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getUserBans)]
            public virtual Task<NullStruct> GetUserBansAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.banMember)]
            public virtual Task<NullStruct> BanMemberAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.unbanMember)]
            public virtual Task<NullStruct> UnbanMemberAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.GetClubsComponentInfo)]
            public virtual Task<NullStruct> GetClubsComponentInfoAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.disbandClub)]
            public virtual Task<NullStruct> DisbandClubAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getNewsForClubs)]
            public virtual Task<NullStruct> GetNewsForClubsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getPetitionsForClubs)]
            public virtual Task<NullStruct> GetPetitionsForClubsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyFindClubsAsyncNotificationAsync(BlazeServerConnection connection, FindClubsAsyncResult notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(ClubsComponentBase.Id, (ushort)ClubsComponentNotification.FindClubsAsyncNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyNewClubTickerMessageNotificationAsync(BlazeServerConnection connection, ClubTickerMessage notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(ClubsComponentBase.Id, (ushort)ClubsComponentNotification.NewClubTickerMessageNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyGetMembersAsyncNotificationAsync(BlazeServerConnection connection, GetMembersAsyncResult notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(ClubsComponentBase.Id, (ushort)ClubsComponentNotification.GetMembersAsyncNotification, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(ClubsComponentCommand command) => ClubsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(ClubsComponentCommand command) => ClubsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(ClubsComponentCommand command) => ClubsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(ClubsComponentNotification notification) => ClubsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<ClubsComponentCommand, ClubsComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(ClubsComponentBase.Id, ClubsComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct CreateClub()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.createClub, new NullStruct());
            }
            public Task<NullStruct> CreateClubAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.createClub, new NullStruct());
            }
            
            public NullStruct GetClubs()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubs, new NullStruct());
            }
            public Task<NullStruct> GetClubsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubs, new NullStruct());
            }
            
            public NullStruct FindClubs()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubs, new NullStruct());
            }
            public Task<NullStruct> FindClubsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubs, new NullStruct());
            }
            
            public NullStruct FindClubs2()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubs2, new NullStruct());
            }
            public Task<NullStruct> FindClubs2Async()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubs2, new NullStruct());
            }
            
            public NullStruct RemoveMember()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.removeMember, new NullStruct());
            }
            public Task<NullStruct> RemoveMemberAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.removeMember, new NullStruct());
            }
            
            public NullStruct SendInvitation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.sendInvitation, new NullStruct());
            }
            public Task<NullStruct> SendInvitationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.sendInvitation, new NullStruct());
            }
            
            public NullStruct GetInvitations()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getInvitations, new NullStruct());
            }
            public Task<NullStruct> GetInvitationsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getInvitations, new NullStruct());
            }
            
            public NullStruct RevokeInvitation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.revokeInvitation, new NullStruct());
            }
            public Task<NullStruct> RevokeInvitationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.revokeInvitation, new NullStruct());
            }
            
            public NullStruct AcceptInvitation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.acceptInvitation, new NullStruct());
            }
            public Task<NullStruct> AcceptInvitationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.acceptInvitation, new NullStruct());
            }
            
            public NullStruct DeclineInvitation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.declineInvitation, new NullStruct());
            }
            public Task<NullStruct> DeclineInvitationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.declineInvitation, new NullStruct());
            }
            
            public NullStruct GetMembers()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getMembers, new NullStruct());
            }
            public Task<NullStruct> GetMembersAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getMembers, new NullStruct());
            }
            
            public NullStruct PromoteToGM()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.promoteToGM, new NullStruct());
            }
            public Task<NullStruct> PromoteToGMAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.promoteToGM, new NullStruct());
            }
            
            public NullStruct DemoteToMember()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.demoteToMember, new NullStruct());
            }
            public Task<NullStruct> DemoteToMemberAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.demoteToMember, new NullStruct());
            }
            
            public NullStruct UpdateClubSettings()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.updateClubSettings, new NullStruct());
            }
            public Task<NullStruct> UpdateClubSettingsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.updateClubSettings, new NullStruct());
            }
            
            public NullStruct PostNews()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.postNews, new NullStruct());
            }
            public Task<NullStruct> PostNewsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.postNews, new NullStruct());
            }
            
            public NullStruct GetNews()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getNews, new NullStruct());
            }
            public Task<NullStruct> GetNewsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getNews, new NullStruct());
            }
            
            public NullStruct SetNewsItemHidden()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setNewsItemHidden, new NullStruct());
            }
            public Task<NullStruct> SetNewsItemHiddenAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setNewsItemHidden, new NullStruct());
            }
            
            public NullStruct SetMetadata()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setMetadata, new NullStruct());
            }
            public Task<NullStruct> SetMetadataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setMetadata, new NullStruct());
            }
            
            public NullStruct SetMetadata2()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setMetadata2, new NullStruct());
            }
            public Task<NullStruct> SetMetadata2Async()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setMetadata2, new NullStruct());
            }
            
            public NullStruct GetClubsComponentSettings()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubsComponentSettings, new NullStruct());
            }
            public Task<NullStruct> GetClubsComponentSettingsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubsComponentSettings, new NullStruct());
            }
            
            public NullStruct TransferOwnership()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.transferOwnership, new NullStruct());
            }
            public Task<NullStruct> TransferOwnershipAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.transferOwnership, new NullStruct());
            }
            
            public NullStruct GetClubMembershipForUsers()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubMembershipForUsers, new NullStruct());
            }
            public Task<NullStruct> GetClubMembershipForUsersAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubMembershipForUsers, new NullStruct());
            }
            
            public NullStruct SendPetition()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.sendPetition, new NullStruct());
            }
            public Task<NullStruct> SendPetitionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.sendPetition, new NullStruct());
            }
            
            public NullStruct GetPetitions()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getPetitions, new NullStruct());
            }
            public Task<NullStruct> GetPetitionsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getPetitions, new NullStruct());
            }
            
            public NullStruct AcceptPetition()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.acceptPetition, new NullStruct());
            }
            public Task<NullStruct> AcceptPetitionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.acceptPetition, new NullStruct());
            }
            
            public NullStruct DeclinePetition()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.declinePetition, new NullStruct());
            }
            public Task<NullStruct> DeclinePetitionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.declinePetition, new NullStruct());
            }
            
            public NullStruct RevokePetition()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.revokePetition, new NullStruct());
            }
            public Task<NullStruct> RevokePetitionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.revokePetition, new NullStruct());
            }
            
            public NullStruct JoinClub()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.joinClub, new NullStruct());
            }
            public Task<NullStruct> JoinClubAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.joinClub, new NullStruct());
            }
            
            public NullStruct GetClubRecordbook()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubRecordbook, new NullStruct());
            }
            public Task<NullStruct> GetClubRecordbookAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubRecordbook, new NullStruct());
            }
            
            public NullStruct ResetClubRecords()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.resetClubRecords, new NullStruct());
            }
            public Task<NullStruct> ResetClubRecordsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.resetClubRecords, new NullStruct());
            }
            
            public NullStruct UpdateMemberOnlineStatus()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.updateMemberOnlineStatus, new NullStruct());
            }
            public Task<NullStruct> UpdateMemberOnlineStatusAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.updateMemberOnlineStatus, new NullStruct());
            }
            
            public NullStruct GetClubAwards()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubAwards, new NullStruct());
            }
            public Task<NullStruct> GetClubAwardsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubAwards, new NullStruct());
            }
            
            public NullStruct UpdateMemberMetadata()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.updateMemberMetadata, new NullStruct());
            }
            public Task<NullStruct> UpdateMemberMetadataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.updateMemberMetadata, new NullStruct());
            }
            
            public NullStruct FindClubsAsynchronously()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubsAsync, new NullStruct());
            }
            public Task<NullStruct> FindClubsAsynchronouslyAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubsAsync, new NullStruct());
            }
            
            public NullStruct FindClubs2Asynchronously()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubs2Async, new NullStruct());
            }
            public Task<NullStruct> FindClubs2AsynchronouslyAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubs2Async, new NullStruct());
            }
            
            public NullStruct ListRivals()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.listRivals, new NullStruct());
            }
            public Task<NullStruct> ListRivalsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.listRivals, new NullStruct());
            }
            
            public NullStruct GetClubTickerMessages()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubTickerMessages, new NullStruct());
            }
            public Task<NullStruct> GetClubTickerMessagesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubTickerMessages, new NullStruct());
            }
            
            public NullStruct SetClubTickerMessagesSubscription()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setClubTickerMessagesSubscription, new NullStruct());
            }
            public Task<NullStruct> SetClubTickerMessagesSubscriptionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setClubTickerMessagesSubscription, new NullStruct());
            }
            
            public NullStruct ChangeClubStrings()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.changeClubStrings, new NullStruct());
            }
            public Task<NullStruct> ChangeClubStringsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.changeClubStrings, new NullStruct());
            }
            
            public NullStruct CountMessages()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.countMessages, new NullStruct());
            }
            public Task<NullStruct> CountMessagesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.countMessages, new NullStruct());
            }
            
            public NullStruct GetMembersAsynchronously()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getMembersAsync, new NullStruct());
            }
            public Task<NullStruct> GetMembersAsynchronouslyAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getMembersAsync, new NullStruct());
            }
            
            public NullStruct GetClubBans()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubBans, new NullStruct());
            }
            public Task<NullStruct> GetClubBansAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubBans, new NullStruct());
            }
            
            public NullStruct GetUserBans()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getUserBans, new NullStruct());
            }
            public Task<NullStruct> GetUserBansAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getUserBans, new NullStruct());
            }
            
            public NullStruct BanMember()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.banMember, new NullStruct());
            }
            public Task<NullStruct> BanMemberAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.banMember, new NullStruct());
            }
            
            public NullStruct UnbanMember()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.unbanMember, new NullStruct());
            }
            public Task<NullStruct> UnbanMemberAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.unbanMember, new NullStruct());
            }
            
            public NullStruct GetClubsComponentInfo()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.GetClubsComponentInfo, new NullStruct());
            }
            public Task<NullStruct> GetClubsComponentInfoAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.GetClubsComponentInfo, new NullStruct());
            }
            
            public NullStruct DisbandClub()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.disbandClub, new NullStruct());
            }
            public Task<NullStruct> DisbandClubAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.disbandClub, new NullStruct());
            }
            
            public NullStruct GetNewsForClubs()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getNewsForClubs, new NullStruct());
            }
            public Task<NullStruct> GetNewsForClubsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getNewsForClubs, new NullStruct());
            }
            
            public NullStruct GetPetitionsForClubs()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getPetitionsForClubs, new NullStruct());
            }
            public Task<NullStruct> GetPetitionsForClubsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getPetitionsForClubs, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)ClubsComponentNotification.FindClubsAsyncNotification)]
            public virtual Task OnFindClubsAsyncNotificationAsync(FindClubsAsyncResult notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnFindClubsAsyncNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)ClubsComponentNotification.NewClubTickerMessageNotification)]
            public virtual Task OnNewClubTickerMessageNotificationAsync(ClubTickerMessage notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNewClubTickerMessageNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)ClubsComponentNotification.GetMembersAsyncNotification)]
            public virtual Task OnGetMembersAsyncNotificationAsync(GetMembersAsyncResult notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnGetMembersAsyncNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(ClubsComponentCommand command) => ClubsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(ClubsComponentCommand command) => ClubsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(ClubsComponentCommand command) => ClubsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(ClubsComponentNotification notification) => ClubsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<ClubsComponentCommand, ClubsComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(ClubsComponentBase.Id, ClubsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.createClub)]
            public virtual Task<NullStruct> CreateClubAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.createClub, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubs)]
            public virtual Task<NullStruct> GetClubsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubs, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.findClubs)]
            public virtual Task<NullStruct> FindClubsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubs, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.findClubs2)]
            public virtual Task<NullStruct> FindClubs2Async(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubs2, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.removeMember)]
            public virtual Task<NullStruct> RemoveMemberAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.removeMember, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.sendInvitation)]
            public virtual Task<NullStruct> SendInvitationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.sendInvitation, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getInvitations)]
            public virtual Task<NullStruct> GetInvitationsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getInvitations, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.revokeInvitation)]
            public virtual Task<NullStruct> RevokeInvitationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.revokeInvitation, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.acceptInvitation)]
            public virtual Task<NullStruct> AcceptInvitationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.acceptInvitation, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.declineInvitation)]
            public virtual Task<NullStruct> DeclineInvitationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.declineInvitation, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getMembers)]
            public virtual Task<NullStruct> GetMembersAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getMembers, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.promoteToGM)]
            public virtual Task<NullStruct> PromoteToGMAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.promoteToGM, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.demoteToMember)]
            public virtual Task<NullStruct> DemoteToMemberAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.demoteToMember, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.updateClubSettings)]
            public virtual Task<NullStruct> UpdateClubSettingsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.updateClubSettings, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.postNews)]
            public virtual Task<NullStruct> PostNewsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.postNews, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getNews)]
            public virtual Task<NullStruct> GetNewsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getNews, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.setNewsItemHidden)]
            public virtual Task<NullStruct> SetNewsItemHiddenAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setNewsItemHidden, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.setMetadata)]
            public virtual Task<NullStruct> SetMetadataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setMetadata, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.setMetadata2)]
            public virtual Task<NullStruct> SetMetadata2Async(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setMetadata2, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubsComponentSettings)]
            public virtual Task<NullStruct> GetClubsComponentSettingsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubsComponentSettings, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.transferOwnership)]
            public virtual Task<NullStruct> TransferOwnershipAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.transferOwnership, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubMembershipForUsers)]
            public virtual Task<NullStruct> GetClubMembershipForUsersAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubMembershipForUsers, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.sendPetition)]
            public virtual Task<NullStruct> SendPetitionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.sendPetition, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getPetitions)]
            public virtual Task<NullStruct> GetPetitionsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getPetitions, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.acceptPetition)]
            public virtual Task<NullStruct> AcceptPetitionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.acceptPetition, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.declinePetition)]
            public virtual Task<NullStruct> DeclinePetitionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.declinePetition, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.revokePetition)]
            public virtual Task<NullStruct> RevokePetitionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.revokePetition, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.joinClub)]
            public virtual Task<NullStruct> JoinClubAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.joinClub, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubRecordbook)]
            public virtual Task<NullStruct> GetClubRecordbookAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubRecordbook, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.resetClubRecords)]
            public virtual Task<NullStruct> ResetClubRecordsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.resetClubRecords, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.updateMemberOnlineStatus)]
            public virtual Task<NullStruct> UpdateMemberOnlineStatusAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.updateMemberOnlineStatus, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubAwards)]
            public virtual Task<NullStruct> GetClubAwardsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubAwards, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.updateMemberMetadata)]
            public virtual Task<NullStruct> UpdateMemberMetadataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.updateMemberMetadata, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.findClubsAsync)]
            public virtual Task<NullStruct> FindClubsAsyncAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubsAsync, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.findClubs2Async)]
            public virtual Task<NullStruct> FindClubs2AsyncAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.findClubs2Async, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.listRivals)]
            public virtual Task<NullStruct> ListRivalsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.listRivals, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubTickerMessages)]
            public virtual Task<NullStruct> GetClubTickerMessagesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubTickerMessages, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.setClubTickerMessagesSubscription)]
            public virtual Task<NullStruct> SetClubTickerMessagesSubscriptionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.setClubTickerMessagesSubscription, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.changeClubStrings)]
            public virtual Task<NullStruct> ChangeClubStringsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.changeClubStrings, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.countMessages)]
            public virtual Task<NullStruct> CountMessagesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.countMessages, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getMembersAsync)]
            public virtual Task<NullStruct> GetMembersAsyncAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getMembersAsync, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getClubBans)]
            public virtual Task<NullStruct> GetClubBansAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getClubBans, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getUserBans)]
            public virtual Task<NullStruct> GetUserBansAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getUserBans, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.banMember)]
            public virtual Task<NullStruct> BanMemberAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.banMember, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.unbanMember)]
            public virtual Task<NullStruct> UnbanMemberAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.unbanMember, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.GetClubsComponentInfo)]
            public virtual Task<NullStruct> GetClubsComponentInfoAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.GetClubsComponentInfo, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.disbandClub)]
            public virtual Task<NullStruct> DisbandClubAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.disbandClub, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getNewsForClubs)]
            public virtual Task<NullStruct> GetNewsForClubsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getNewsForClubs, request);
            }
            
            [BlazeCommand((ushort)ClubsComponentCommand.getPetitionsForClubs)]
            public virtual Task<NullStruct> GetPetitionsForClubsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)ClubsComponentCommand.getPetitionsForClubs, request);
            }
            
            
            [BlazeNotification((ushort)ClubsComponentNotification.FindClubsAsyncNotification)]
            public virtual Task<FindClubsAsyncResult> OnFindClubsAsyncNotificationAsync(FindClubsAsyncResult notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)ClubsComponentNotification.NewClubTickerMessageNotification)]
            public virtual Task<ClubTickerMessage> OnNewClubTickerMessageNotificationAsync(ClubTickerMessage notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)ClubsComponentNotification.GetMembersAsyncNotification)]
            public virtual Task<GetMembersAsyncResult> OnGetMembersAsyncNotificationAsync(GetMembersAsyncResult notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(ClubsComponentCommand command) => ClubsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(ClubsComponentCommand command) => ClubsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(ClubsComponentCommand command) => ClubsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(ClubsComponentNotification notification) => ClubsComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(ClubsComponentCommand command) => command switch
        {
            ClubsComponentCommand.createClub => typeof(NullStruct),
            ClubsComponentCommand.getClubs => typeof(NullStruct),
            ClubsComponentCommand.findClubs => typeof(NullStruct),
            ClubsComponentCommand.findClubs2 => typeof(NullStruct),
            ClubsComponentCommand.removeMember => typeof(NullStruct),
            ClubsComponentCommand.sendInvitation => typeof(NullStruct),
            ClubsComponentCommand.getInvitations => typeof(NullStruct),
            ClubsComponentCommand.revokeInvitation => typeof(NullStruct),
            ClubsComponentCommand.acceptInvitation => typeof(NullStruct),
            ClubsComponentCommand.declineInvitation => typeof(NullStruct),
            ClubsComponentCommand.getMembers => typeof(NullStruct),
            ClubsComponentCommand.promoteToGM => typeof(NullStruct),
            ClubsComponentCommand.demoteToMember => typeof(NullStruct),
            ClubsComponentCommand.updateClubSettings => typeof(NullStruct),
            ClubsComponentCommand.postNews => typeof(NullStruct),
            ClubsComponentCommand.getNews => typeof(NullStruct),
            ClubsComponentCommand.setNewsItemHidden => typeof(NullStruct),
            ClubsComponentCommand.setMetadata => typeof(NullStruct),
            ClubsComponentCommand.setMetadata2 => typeof(NullStruct),
            ClubsComponentCommand.getClubsComponentSettings => typeof(NullStruct),
            ClubsComponentCommand.transferOwnership => typeof(NullStruct),
            ClubsComponentCommand.getClubMembershipForUsers => typeof(NullStruct),
            ClubsComponentCommand.sendPetition => typeof(NullStruct),
            ClubsComponentCommand.getPetitions => typeof(NullStruct),
            ClubsComponentCommand.acceptPetition => typeof(NullStruct),
            ClubsComponentCommand.declinePetition => typeof(NullStruct),
            ClubsComponentCommand.revokePetition => typeof(NullStruct),
            ClubsComponentCommand.joinClub => typeof(NullStruct),
            ClubsComponentCommand.getClubRecordbook => typeof(NullStruct),
            ClubsComponentCommand.resetClubRecords => typeof(NullStruct),
            ClubsComponentCommand.updateMemberOnlineStatus => typeof(NullStruct),
            ClubsComponentCommand.getClubAwards => typeof(NullStruct),
            ClubsComponentCommand.updateMemberMetadata => typeof(NullStruct),
            ClubsComponentCommand.findClubsAsync => typeof(NullStruct),
            ClubsComponentCommand.findClubs2Async => typeof(NullStruct),
            ClubsComponentCommand.listRivals => typeof(NullStruct),
            ClubsComponentCommand.getClubTickerMessages => typeof(NullStruct),
            ClubsComponentCommand.setClubTickerMessagesSubscription => typeof(NullStruct),
            ClubsComponentCommand.changeClubStrings => typeof(NullStruct),
            ClubsComponentCommand.countMessages => typeof(NullStruct),
            ClubsComponentCommand.getMembersAsync => typeof(NullStruct),
            ClubsComponentCommand.getClubBans => typeof(NullStruct),
            ClubsComponentCommand.getUserBans => typeof(NullStruct),
            ClubsComponentCommand.banMember => typeof(NullStruct),
            ClubsComponentCommand.unbanMember => typeof(NullStruct),
            ClubsComponentCommand.GetClubsComponentInfo => typeof(NullStruct),
            ClubsComponentCommand.disbandClub => typeof(NullStruct),
            ClubsComponentCommand.getNewsForClubs => typeof(NullStruct),
            ClubsComponentCommand.getPetitionsForClubs => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(ClubsComponentCommand command) => command switch
        {
            ClubsComponentCommand.createClub => typeof(NullStruct),
            ClubsComponentCommand.getClubs => typeof(NullStruct),
            ClubsComponentCommand.findClubs => typeof(NullStruct),
            ClubsComponentCommand.findClubs2 => typeof(NullStruct),
            ClubsComponentCommand.removeMember => typeof(NullStruct),
            ClubsComponentCommand.sendInvitation => typeof(NullStruct),
            ClubsComponentCommand.getInvitations => typeof(NullStruct),
            ClubsComponentCommand.revokeInvitation => typeof(NullStruct),
            ClubsComponentCommand.acceptInvitation => typeof(NullStruct),
            ClubsComponentCommand.declineInvitation => typeof(NullStruct),
            ClubsComponentCommand.getMembers => typeof(NullStruct),
            ClubsComponentCommand.promoteToGM => typeof(NullStruct),
            ClubsComponentCommand.demoteToMember => typeof(NullStruct),
            ClubsComponentCommand.updateClubSettings => typeof(NullStruct),
            ClubsComponentCommand.postNews => typeof(NullStruct),
            ClubsComponentCommand.getNews => typeof(NullStruct),
            ClubsComponentCommand.setNewsItemHidden => typeof(NullStruct),
            ClubsComponentCommand.setMetadata => typeof(NullStruct),
            ClubsComponentCommand.setMetadata2 => typeof(NullStruct),
            ClubsComponentCommand.getClubsComponentSettings => typeof(NullStruct),
            ClubsComponentCommand.transferOwnership => typeof(NullStruct),
            ClubsComponentCommand.getClubMembershipForUsers => typeof(NullStruct),
            ClubsComponentCommand.sendPetition => typeof(NullStruct),
            ClubsComponentCommand.getPetitions => typeof(NullStruct),
            ClubsComponentCommand.acceptPetition => typeof(NullStruct),
            ClubsComponentCommand.declinePetition => typeof(NullStruct),
            ClubsComponentCommand.revokePetition => typeof(NullStruct),
            ClubsComponentCommand.joinClub => typeof(NullStruct),
            ClubsComponentCommand.getClubRecordbook => typeof(NullStruct),
            ClubsComponentCommand.resetClubRecords => typeof(NullStruct),
            ClubsComponentCommand.updateMemberOnlineStatus => typeof(NullStruct),
            ClubsComponentCommand.getClubAwards => typeof(NullStruct),
            ClubsComponentCommand.updateMemberMetadata => typeof(NullStruct),
            ClubsComponentCommand.findClubsAsync => typeof(NullStruct),
            ClubsComponentCommand.findClubs2Async => typeof(NullStruct),
            ClubsComponentCommand.listRivals => typeof(NullStruct),
            ClubsComponentCommand.getClubTickerMessages => typeof(NullStruct),
            ClubsComponentCommand.setClubTickerMessagesSubscription => typeof(NullStruct),
            ClubsComponentCommand.changeClubStrings => typeof(NullStruct),
            ClubsComponentCommand.countMessages => typeof(NullStruct),
            ClubsComponentCommand.getMembersAsync => typeof(NullStruct),
            ClubsComponentCommand.getClubBans => typeof(NullStruct),
            ClubsComponentCommand.getUserBans => typeof(NullStruct),
            ClubsComponentCommand.banMember => typeof(NullStruct),
            ClubsComponentCommand.unbanMember => typeof(NullStruct),
            ClubsComponentCommand.GetClubsComponentInfo => typeof(NullStruct),
            ClubsComponentCommand.disbandClub => typeof(NullStruct),
            ClubsComponentCommand.getNewsForClubs => typeof(NullStruct),
            ClubsComponentCommand.getPetitionsForClubs => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(ClubsComponentCommand command) => command switch
        {
            ClubsComponentCommand.createClub => typeof(NullStruct),
            ClubsComponentCommand.getClubs => typeof(NullStruct),
            ClubsComponentCommand.findClubs => typeof(NullStruct),
            ClubsComponentCommand.findClubs2 => typeof(NullStruct),
            ClubsComponentCommand.removeMember => typeof(NullStruct),
            ClubsComponentCommand.sendInvitation => typeof(NullStruct),
            ClubsComponentCommand.getInvitations => typeof(NullStruct),
            ClubsComponentCommand.revokeInvitation => typeof(NullStruct),
            ClubsComponentCommand.acceptInvitation => typeof(NullStruct),
            ClubsComponentCommand.declineInvitation => typeof(NullStruct),
            ClubsComponentCommand.getMembers => typeof(NullStruct),
            ClubsComponentCommand.promoteToGM => typeof(NullStruct),
            ClubsComponentCommand.demoteToMember => typeof(NullStruct),
            ClubsComponentCommand.updateClubSettings => typeof(NullStruct),
            ClubsComponentCommand.postNews => typeof(NullStruct),
            ClubsComponentCommand.getNews => typeof(NullStruct),
            ClubsComponentCommand.setNewsItemHidden => typeof(NullStruct),
            ClubsComponentCommand.setMetadata => typeof(NullStruct),
            ClubsComponentCommand.setMetadata2 => typeof(NullStruct),
            ClubsComponentCommand.getClubsComponentSettings => typeof(NullStruct),
            ClubsComponentCommand.transferOwnership => typeof(NullStruct),
            ClubsComponentCommand.getClubMembershipForUsers => typeof(NullStruct),
            ClubsComponentCommand.sendPetition => typeof(NullStruct),
            ClubsComponentCommand.getPetitions => typeof(NullStruct),
            ClubsComponentCommand.acceptPetition => typeof(NullStruct),
            ClubsComponentCommand.declinePetition => typeof(NullStruct),
            ClubsComponentCommand.revokePetition => typeof(NullStruct),
            ClubsComponentCommand.joinClub => typeof(NullStruct),
            ClubsComponentCommand.getClubRecordbook => typeof(NullStruct),
            ClubsComponentCommand.resetClubRecords => typeof(NullStruct),
            ClubsComponentCommand.updateMemberOnlineStatus => typeof(NullStruct),
            ClubsComponentCommand.getClubAwards => typeof(NullStruct),
            ClubsComponentCommand.updateMemberMetadata => typeof(NullStruct),
            ClubsComponentCommand.findClubsAsync => typeof(NullStruct),
            ClubsComponentCommand.findClubs2Async => typeof(NullStruct),
            ClubsComponentCommand.listRivals => typeof(NullStruct),
            ClubsComponentCommand.getClubTickerMessages => typeof(NullStruct),
            ClubsComponentCommand.setClubTickerMessagesSubscription => typeof(NullStruct),
            ClubsComponentCommand.changeClubStrings => typeof(NullStruct),
            ClubsComponentCommand.countMessages => typeof(NullStruct),
            ClubsComponentCommand.getMembersAsync => typeof(NullStruct),
            ClubsComponentCommand.getClubBans => typeof(NullStruct),
            ClubsComponentCommand.getUserBans => typeof(NullStruct),
            ClubsComponentCommand.banMember => typeof(NullStruct),
            ClubsComponentCommand.unbanMember => typeof(NullStruct),
            ClubsComponentCommand.GetClubsComponentInfo => typeof(NullStruct),
            ClubsComponentCommand.disbandClub => typeof(NullStruct),
            ClubsComponentCommand.getNewsForClubs => typeof(NullStruct),
            ClubsComponentCommand.getPetitionsForClubs => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(ClubsComponentNotification notification) => notification switch
        {
            ClubsComponentNotification.FindClubsAsyncNotification => typeof(FindClubsAsyncResult),
            ClubsComponentNotification.NewClubTickerMessageNotification => typeof(ClubTickerMessage),
            ClubsComponentNotification.GetMembersAsyncNotification => typeof(GetMembersAsyncResult),
            _ => typeof(NullStruct)
        };
        
        public enum ClubsComponentCommand : ushort
        {
            createClub = 1100,
            getClubs = 1200,
            findClubs = 1300,
            findClubs2 = 1310,
            removeMember = 1400,
            sendInvitation = 1500,
            getInvitations = 1600,
            revokeInvitation = 1700,
            acceptInvitation = 1800,
            declineInvitation = 1900,
            getMembers = 2000,
            promoteToGM = 2100,
            demoteToMember = 2150,
            updateClubSettings = 2200,
            postNews = 2300,
            getNews = 2400,
            setNewsItemHidden = 2450,
            setMetadata = 2500,
            setMetadata2 = 2510,
            getClubsComponentSettings = 2600,
            transferOwnership = 2650,
            getClubMembershipForUsers = 2700,
            sendPetition = 2800,
            getPetitions = 2900,
            acceptPetition = 3000,
            declinePetition = 3100,
            revokePetition = 3200,
            joinClub = 3300,
            getClubRecordbook = 3400,
            resetClubRecords = 3410,
            updateMemberOnlineStatus = 3500,
            getClubAwards = 3600,
            updateMemberMetadata = 3700,
            findClubsAsync = 3800,
            findClubs2Async = 3810,
            listRivals = 3900,
            getClubTickerMessages = 4000,
            setClubTickerMessagesSubscription = 4100,
            changeClubStrings = 4200,
            countMessages = 4300,
            getMembersAsync = 4400,
            getClubBans = 4500,
            getUserBans = 4600,
            banMember = 4700,
            unbanMember = 4800,
            GetClubsComponentInfo = 4900,
            disbandClub = 5000,
            getNewsForClubs = 5100,
            getPetitionsForClubs = 5200,
        }
        
        public enum ClubsComponentNotification : ushort
        {
            FindClubsAsyncNotification = 0,
            NewClubTickerMessageNotification = 1,
            GetMembersAsyncNotification = 2,
        }
        
    }
}
