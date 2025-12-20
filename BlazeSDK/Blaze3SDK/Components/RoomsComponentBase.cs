using Blaze3SDK.Blaze.Rooms;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class RoomsComponentBase
    {
        public const ushort Id = 21;
        public const string Name = "RoomsComponent";
        
        public class Server : BlazeServerComponent<RoomsComponentCommand, RoomsComponentNotification, Blaze3RpcError>
        {
            public Server() : base(RoomsComponentBase.Id, RoomsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.selectViewUpdates)]
            public virtual Task<NullStruct> SelectViewUpdatesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.selectCategoryUpdates)]
            public virtual Task<NullStruct> SelectCategoryUpdatesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.joinRoom)]
            public virtual Task<NullStruct> JoinRoomAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.leaveRoom)]
            public virtual Task<NullStruct> LeaveRoomAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.kickUser)]
            public virtual Task<NullStruct> KickUserAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.transferRoomHost)]
            public virtual Task<NullStruct> TransferRoomHostAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.createRoomCategory)]
            public virtual Task<NullStruct> CreateRoomCategoryAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.removeRoomCategory)]
            public virtual Task<NullStruct> RemoveRoomCategoryAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.createRoom)]
            public virtual Task<NullStruct> CreateRoomAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.removeRoom)]
            public virtual Task<NullStruct> RemoveRoomAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.clearBannedUsers)]
            public virtual Task<NullStruct> ClearBannedUsersAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.unbanUser)]
            public virtual Task<NullStruct> UnbanUserAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.getViews)]
            public virtual Task<NullStruct> GetViewsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.createScheduledCategory)]
            public virtual Task<NullStruct> CreateScheduledCategoryAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.deleteScheduledCategory)]
            public virtual Task<NullStruct> DeleteScheduledCategoryAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.getScheduledCategories)]
            public virtual Task<NullStruct> GetScheduledCategoriesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.lookupRoomData)]
            public virtual Task<NullStruct> LookupRoomDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.listBannedUsers)]
            public virtual Task<NullStruct> ListBannedUsersAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.setRoomAttributes)]
            public virtual Task<NullStruct> SetRoomAttributesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.checkEntryCriteria)]
            public virtual Task<NullStruct> CheckEntryCriteriaAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.toggleJoinedRoomNotifications)]
            public virtual Task<NullStruct> ToggleJoinedRoomNotificationsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.selectPseudoRoomUpdates)]
            public virtual Task<NullStruct> SelectPseudoRoomUpdatesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.setMemberAttributes)]
            public virtual Task<NullStruct> SetMemberAttributesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyRoomViewUpdatedNotificationAsync(BlazeServerConnection connection, RoomViewData notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomViewUpdatedNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomViewAddedNotificationAsync(BlazeServerConnection connection, RoomViewData notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomViewAddedNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomViewRemovedNotificationAsync(BlazeServerConnection connection, RoomViewRemoved notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomViewRemovedNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomCategoryUpdatedNotificationAsync(BlazeServerConnection connection, RoomCategoryData notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomCategoryUpdatedNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomCategoryAddedNotificationAsync(BlazeServerConnection connection, RoomCategoryData notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomCategoryAddedNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomCategoryRemovedNotificationAsync(BlazeServerConnection connection, RoomCategoryRemoved notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomCategoryRemovedNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomUpdatedNotificationAsync(BlazeServerConnection connection, RoomData notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomUpdatedNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomAddedNotificationAsync(BlazeServerConnection connection, RoomData notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomAddedNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomRemovedNotificationAsync(BlazeServerConnection connection, RoomRemoved notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomRemovedNotification, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomPopulationUpdatedAsync(BlazeServerConnection connection, RoomsPopulationUpdate notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomPopulationUpdated, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomMemberJoinedAsync(BlazeServerConnection connection, RoomMemberData notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomMemberJoined, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomMemberLeftAsync(BlazeServerConnection connection, RoomMemberRemoved notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomMemberLeft, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomMemberUpdatedAsync(BlazeServerConnection connection, RoomMemberData notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomMemberUpdated, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomKickAsync(BlazeServerConnection connection, RoomMemberKicked notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomKick, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomHostTransferAsync(BlazeServerConnection connection, RoomHostTransfered notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomHostTransfer, notification, waitUntilFree);
            }
            
            public static Task NotifyRoomAttributesSetAsync(BlazeServerConnection connection, RoomAttributesSet notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.RoomAttributesSet, notification, waitUntilFree);
            }
            
            public static Task NotifyMemberAttributesSetAsync(BlazeServerConnection connection, MemberAttributesSet notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(RoomsComponentBase.Id, (ushort)RoomsComponentNotification.MemberAttributesSet, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(RoomsComponentCommand command) => RoomsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RoomsComponentCommand command) => RoomsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RoomsComponentCommand command) => RoomsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RoomsComponentNotification notification) => RoomsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<RoomsComponentCommand, RoomsComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(RoomsComponentBase.Id, RoomsComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct SelectViewUpdates()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.selectViewUpdates, new NullStruct());
            }
            public Task<NullStruct> SelectViewUpdatesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.selectViewUpdates, new NullStruct());
            }
            
            public NullStruct SelectCategoryUpdates()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.selectCategoryUpdates, new NullStruct());
            }
            public Task<NullStruct> SelectCategoryUpdatesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.selectCategoryUpdates, new NullStruct());
            }
            
            public NullStruct JoinRoom()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.joinRoom, new NullStruct());
            }
            public Task<NullStruct> JoinRoomAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.joinRoom, new NullStruct());
            }
            
            public NullStruct LeaveRoom()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.leaveRoom, new NullStruct());
            }
            public Task<NullStruct> LeaveRoomAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.leaveRoom, new NullStruct());
            }
            
            public NullStruct KickUser()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.kickUser, new NullStruct());
            }
            public Task<NullStruct> KickUserAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.kickUser, new NullStruct());
            }
            
            public NullStruct TransferRoomHost()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.transferRoomHost, new NullStruct());
            }
            public Task<NullStruct> TransferRoomHostAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.transferRoomHost, new NullStruct());
            }
            
            public NullStruct CreateRoomCategory()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.createRoomCategory, new NullStruct());
            }
            public Task<NullStruct> CreateRoomCategoryAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.createRoomCategory, new NullStruct());
            }
            
            public NullStruct RemoveRoomCategory()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.removeRoomCategory, new NullStruct());
            }
            public Task<NullStruct> RemoveRoomCategoryAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.removeRoomCategory, new NullStruct());
            }
            
            public NullStruct CreateRoom()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.createRoom, new NullStruct());
            }
            public Task<NullStruct> CreateRoomAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.createRoom, new NullStruct());
            }
            
            public NullStruct RemoveRoom()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.removeRoom, new NullStruct());
            }
            public Task<NullStruct> RemoveRoomAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.removeRoom, new NullStruct());
            }
            
            public NullStruct ClearBannedUsers()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.clearBannedUsers, new NullStruct());
            }
            public Task<NullStruct> ClearBannedUsersAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.clearBannedUsers, new NullStruct());
            }
            
            public NullStruct UnbanUser()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.unbanUser, new NullStruct());
            }
            public Task<NullStruct> UnbanUserAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.unbanUser, new NullStruct());
            }
            
            public NullStruct GetViews()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.getViews, new NullStruct());
            }
            public Task<NullStruct> GetViewsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.getViews, new NullStruct());
            }
            
            public NullStruct CreateScheduledCategory()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.createScheduledCategory, new NullStruct());
            }
            public Task<NullStruct> CreateScheduledCategoryAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.createScheduledCategory, new NullStruct());
            }
            
            public NullStruct DeleteScheduledCategory()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.deleteScheduledCategory, new NullStruct());
            }
            public Task<NullStruct> DeleteScheduledCategoryAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.deleteScheduledCategory, new NullStruct());
            }
            
            public NullStruct GetScheduledCategories()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.getScheduledCategories, new NullStruct());
            }
            public Task<NullStruct> GetScheduledCategoriesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.getScheduledCategories, new NullStruct());
            }
            
            public NullStruct LookupRoomData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.lookupRoomData, new NullStruct());
            }
            public Task<NullStruct> LookupRoomDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.lookupRoomData, new NullStruct());
            }
            
            public NullStruct ListBannedUsers()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.listBannedUsers, new NullStruct());
            }
            public Task<NullStruct> ListBannedUsersAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.listBannedUsers, new NullStruct());
            }
            
            public NullStruct SetRoomAttributes()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.setRoomAttributes, new NullStruct());
            }
            public Task<NullStruct> SetRoomAttributesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.setRoomAttributes, new NullStruct());
            }
            
            public NullStruct CheckEntryCriteria()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.checkEntryCriteria, new NullStruct());
            }
            public Task<NullStruct> CheckEntryCriteriaAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.checkEntryCriteria, new NullStruct());
            }
            
            public NullStruct ToggleJoinedRoomNotifications()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.toggleJoinedRoomNotifications, new NullStruct());
            }
            public Task<NullStruct> ToggleJoinedRoomNotificationsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.toggleJoinedRoomNotifications, new NullStruct());
            }
            
            public NullStruct SelectPseudoRoomUpdates()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.selectPseudoRoomUpdates, new NullStruct());
            }
            public Task<NullStruct> SelectPseudoRoomUpdatesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.selectPseudoRoomUpdates, new NullStruct());
            }
            
            public NullStruct SetMemberAttributes()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.setMemberAttributes, new NullStruct());
            }
            public Task<NullStruct> SetMemberAttributesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.setMemberAttributes, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomViewUpdatedNotification)]
            public virtual Task OnRoomViewUpdatedNotificationAsync(RoomViewData notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomViewUpdatedNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomViewAddedNotification)]
            public virtual Task OnRoomViewAddedNotificationAsync(RoomViewData notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomViewAddedNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomViewRemovedNotification)]
            public virtual Task OnRoomViewRemovedNotificationAsync(RoomViewRemoved notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomViewRemovedNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomCategoryUpdatedNotification)]
            public virtual Task OnRoomCategoryUpdatedNotificationAsync(RoomCategoryData notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomCategoryUpdatedNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomCategoryAddedNotification)]
            public virtual Task OnRoomCategoryAddedNotificationAsync(RoomCategoryData notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomCategoryAddedNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomCategoryRemovedNotification)]
            public virtual Task OnRoomCategoryRemovedNotificationAsync(RoomCategoryRemoved notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomCategoryRemovedNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomUpdatedNotification)]
            public virtual Task OnRoomUpdatedNotificationAsync(RoomData notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomUpdatedNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomAddedNotification)]
            public virtual Task OnRoomAddedNotificationAsync(RoomData notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomAddedNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomRemovedNotification)]
            public virtual Task OnRoomRemovedNotificationAsync(RoomRemoved notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomRemovedNotificationAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomPopulationUpdated)]
            public virtual Task OnRoomPopulationUpdatedAsync(RoomsPopulationUpdate notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomPopulationUpdatedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomMemberJoined)]
            public virtual Task OnRoomMemberJoinedAsync(RoomMemberData notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomMemberJoinedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomMemberLeft)]
            public virtual Task OnRoomMemberLeftAsync(RoomMemberRemoved notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomMemberLeftAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomMemberUpdated)]
            public virtual Task OnRoomMemberUpdatedAsync(RoomMemberData notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomMemberUpdatedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomKick)]
            public virtual Task OnRoomKickAsync(RoomMemberKicked notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomKickAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomHostTransfer)]
            public virtual Task OnRoomHostTransferAsync(RoomHostTransfered notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomHostTransferAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomAttributesSet)]
            public virtual Task OnRoomAttributesSetAsync(RoomAttributesSet notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnRoomAttributesSetAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.MemberAttributesSet)]
            public virtual Task OnMemberAttributesSetAsync(MemberAttributesSet notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnMemberAttributesSetAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(RoomsComponentCommand command) => RoomsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RoomsComponentCommand command) => RoomsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RoomsComponentCommand command) => RoomsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RoomsComponentNotification notification) => RoomsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<RoomsComponentCommand, RoomsComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(RoomsComponentBase.Id, RoomsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.selectViewUpdates)]
            public virtual Task<NullStruct> SelectViewUpdatesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.selectViewUpdates, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.selectCategoryUpdates)]
            public virtual Task<NullStruct> SelectCategoryUpdatesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.selectCategoryUpdates, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.joinRoom)]
            public virtual Task<NullStruct> JoinRoomAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.joinRoom, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.leaveRoom)]
            public virtual Task<NullStruct> LeaveRoomAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.leaveRoom, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.kickUser)]
            public virtual Task<NullStruct> KickUserAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.kickUser, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.transferRoomHost)]
            public virtual Task<NullStruct> TransferRoomHostAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.transferRoomHost, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.createRoomCategory)]
            public virtual Task<NullStruct> CreateRoomCategoryAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.createRoomCategory, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.removeRoomCategory)]
            public virtual Task<NullStruct> RemoveRoomCategoryAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.removeRoomCategory, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.createRoom)]
            public virtual Task<NullStruct> CreateRoomAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.createRoom, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.removeRoom)]
            public virtual Task<NullStruct> RemoveRoomAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.removeRoom, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.clearBannedUsers)]
            public virtual Task<NullStruct> ClearBannedUsersAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.clearBannedUsers, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.unbanUser)]
            public virtual Task<NullStruct> UnbanUserAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.unbanUser, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.getViews)]
            public virtual Task<NullStruct> GetViewsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.getViews, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.createScheduledCategory)]
            public virtual Task<NullStruct> CreateScheduledCategoryAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.createScheduledCategory, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.deleteScheduledCategory)]
            public virtual Task<NullStruct> DeleteScheduledCategoryAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.deleteScheduledCategory, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.getScheduledCategories)]
            public virtual Task<NullStruct> GetScheduledCategoriesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.getScheduledCategories, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.lookupRoomData)]
            public virtual Task<NullStruct> LookupRoomDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.lookupRoomData, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.listBannedUsers)]
            public virtual Task<NullStruct> ListBannedUsersAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.listBannedUsers, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.setRoomAttributes)]
            public virtual Task<NullStruct> SetRoomAttributesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.setRoomAttributes, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.checkEntryCriteria)]
            public virtual Task<NullStruct> CheckEntryCriteriaAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.checkEntryCriteria, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.toggleJoinedRoomNotifications)]
            public virtual Task<NullStruct> ToggleJoinedRoomNotificationsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.toggleJoinedRoomNotifications, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.selectPseudoRoomUpdates)]
            public virtual Task<NullStruct> SelectPseudoRoomUpdatesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.selectPseudoRoomUpdates, request);
            }
            
            [BlazeCommand((ushort)RoomsComponentCommand.setMemberAttributes)]
            public virtual Task<NullStruct> SetMemberAttributesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)RoomsComponentCommand.setMemberAttributes, request);
            }
            
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomViewUpdatedNotification)]
            public virtual Task<RoomViewData> OnRoomViewUpdatedNotificationAsync(RoomViewData notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomViewAddedNotification)]
            public virtual Task<RoomViewData> OnRoomViewAddedNotificationAsync(RoomViewData notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomViewRemovedNotification)]
            public virtual Task<RoomViewRemoved> OnRoomViewRemovedNotificationAsync(RoomViewRemoved notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomCategoryUpdatedNotification)]
            public virtual Task<RoomCategoryData> OnRoomCategoryUpdatedNotificationAsync(RoomCategoryData notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomCategoryAddedNotification)]
            public virtual Task<RoomCategoryData> OnRoomCategoryAddedNotificationAsync(RoomCategoryData notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomCategoryRemovedNotification)]
            public virtual Task<RoomCategoryRemoved> OnRoomCategoryRemovedNotificationAsync(RoomCategoryRemoved notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomUpdatedNotification)]
            public virtual Task<RoomData> OnRoomUpdatedNotificationAsync(RoomData notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomAddedNotification)]
            public virtual Task<RoomData> OnRoomAddedNotificationAsync(RoomData notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomRemovedNotification)]
            public virtual Task<RoomRemoved> OnRoomRemovedNotificationAsync(RoomRemoved notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomPopulationUpdated)]
            public virtual Task<RoomsPopulationUpdate> OnRoomPopulationUpdatedAsync(RoomsPopulationUpdate notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomMemberJoined)]
            public virtual Task<RoomMemberData> OnRoomMemberJoinedAsync(RoomMemberData notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomMemberLeft)]
            public virtual Task<RoomMemberRemoved> OnRoomMemberLeftAsync(RoomMemberRemoved notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomMemberUpdated)]
            public virtual Task<RoomMemberData> OnRoomMemberUpdatedAsync(RoomMemberData notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomKick)]
            public virtual Task<RoomMemberKicked> OnRoomKickAsync(RoomMemberKicked notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomHostTransfer)]
            public virtual Task<RoomHostTransfered> OnRoomHostTransferAsync(RoomHostTransfered notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.RoomAttributesSet)]
            public virtual Task<RoomAttributesSet> OnRoomAttributesSetAsync(RoomAttributesSet notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)RoomsComponentNotification.MemberAttributesSet)]
            public virtual Task<MemberAttributesSet> OnMemberAttributesSetAsync(MemberAttributesSet notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(RoomsComponentCommand command) => RoomsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(RoomsComponentCommand command) => RoomsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(RoomsComponentCommand command) => RoomsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(RoomsComponentNotification notification) => RoomsComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(RoomsComponentCommand command) => command switch
        {
            RoomsComponentCommand.selectViewUpdates => typeof(NullStruct),
            RoomsComponentCommand.selectCategoryUpdates => typeof(NullStruct),
            RoomsComponentCommand.joinRoom => typeof(NullStruct),
            RoomsComponentCommand.leaveRoom => typeof(NullStruct),
            RoomsComponentCommand.kickUser => typeof(NullStruct),
            RoomsComponentCommand.transferRoomHost => typeof(NullStruct),
            RoomsComponentCommand.createRoomCategory => typeof(NullStruct),
            RoomsComponentCommand.removeRoomCategory => typeof(NullStruct),
            RoomsComponentCommand.createRoom => typeof(NullStruct),
            RoomsComponentCommand.removeRoom => typeof(NullStruct),
            RoomsComponentCommand.clearBannedUsers => typeof(NullStruct),
            RoomsComponentCommand.unbanUser => typeof(NullStruct),
            RoomsComponentCommand.getViews => typeof(NullStruct),
            RoomsComponentCommand.createScheduledCategory => typeof(NullStruct),
            RoomsComponentCommand.deleteScheduledCategory => typeof(NullStruct),
            RoomsComponentCommand.getScheduledCategories => typeof(NullStruct),
            RoomsComponentCommand.lookupRoomData => typeof(NullStruct),
            RoomsComponentCommand.listBannedUsers => typeof(NullStruct),
            RoomsComponentCommand.setRoomAttributes => typeof(NullStruct),
            RoomsComponentCommand.checkEntryCriteria => typeof(NullStruct),
            RoomsComponentCommand.toggleJoinedRoomNotifications => typeof(NullStruct),
            RoomsComponentCommand.selectPseudoRoomUpdates => typeof(NullStruct),
            RoomsComponentCommand.setMemberAttributes => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(RoomsComponentCommand command) => command switch
        {
            RoomsComponentCommand.selectViewUpdates => typeof(NullStruct),
            RoomsComponentCommand.selectCategoryUpdates => typeof(NullStruct),
            RoomsComponentCommand.joinRoom => typeof(NullStruct),
            RoomsComponentCommand.leaveRoom => typeof(NullStruct),
            RoomsComponentCommand.kickUser => typeof(NullStruct),
            RoomsComponentCommand.transferRoomHost => typeof(NullStruct),
            RoomsComponentCommand.createRoomCategory => typeof(NullStruct),
            RoomsComponentCommand.removeRoomCategory => typeof(NullStruct),
            RoomsComponentCommand.createRoom => typeof(NullStruct),
            RoomsComponentCommand.removeRoom => typeof(NullStruct),
            RoomsComponentCommand.clearBannedUsers => typeof(NullStruct),
            RoomsComponentCommand.unbanUser => typeof(NullStruct),
            RoomsComponentCommand.getViews => typeof(NullStruct),
            RoomsComponentCommand.createScheduledCategory => typeof(NullStruct),
            RoomsComponentCommand.deleteScheduledCategory => typeof(NullStruct),
            RoomsComponentCommand.getScheduledCategories => typeof(NullStruct),
            RoomsComponentCommand.lookupRoomData => typeof(NullStruct),
            RoomsComponentCommand.listBannedUsers => typeof(NullStruct),
            RoomsComponentCommand.setRoomAttributes => typeof(NullStruct),
            RoomsComponentCommand.checkEntryCriteria => typeof(NullStruct),
            RoomsComponentCommand.toggleJoinedRoomNotifications => typeof(NullStruct),
            RoomsComponentCommand.selectPseudoRoomUpdates => typeof(NullStruct),
            RoomsComponentCommand.setMemberAttributes => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(RoomsComponentCommand command) => command switch
        {
            RoomsComponentCommand.selectViewUpdates => typeof(NullStruct),
            RoomsComponentCommand.selectCategoryUpdates => typeof(NullStruct),
            RoomsComponentCommand.joinRoom => typeof(NullStruct),
            RoomsComponentCommand.leaveRoom => typeof(NullStruct),
            RoomsComponentCommand.kickUser => typeof(NullStruct),
            RoomsComponentCommand.transferRoomHost => typeof(NullStruct),
            RoomsComponentCommand.createRoomCategory => typeof(NullStruct),
            RoomsComponentCommand.removeRoomCategory => typeof(NullStruct),
            RoomsComponentCommand.createRoom => typeof(NullStruct),
            RoomsComponentCommand.removeRoom => typeof(NullStruct),
            RoomsComponentCommand.clearBannedUsers => typeof(NullStruct),
            RoomsComponentCommand.unbanUser => typeof(NullStruct),
            RoomsComponentCommand.getViews => typeof(NullStruct),
            RoomsComponentCommand.createScheduledCategory => typeof(NullStruct),
            RoomsComponentCommand.deleteScheduledCategory => typeof(NullStruct),
            RoomsComponentCommand.getScheduledCategories => typeof(NullStruct),
            RoomsComponentCommand.lookupRoomData => typeof(NullStruct),
            RoomsComponentCommand.listBannedUsers => typeof(NullStruct),
            RoomsComponentCommand.setRoomAttributes => typeof(NullStruct),
            RoomsComponentCommand.checkEntryCriteria => typeof(NullStruct),
            RoomsComponentCommand.toggleJoinedRoomNotifications => typeof(NullStruct),
            RoomsComponentCommand.selectPseudoRoomUpdates => typeof(NullStruct),
            RoomsComponentCommand.setMemberAttributes => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(RoomsComponentNotification notification) => notification switch
        {
            RoomsComponentNotification.RoomViewUpdatedNotification => typeof(RoomViewData),
            RoomsComponentNotification.RoomViewAddedNotification => typeof(RoomViewData),
            RoomsComponentNotification.RoomViewRemovedNotification => typeof(RoomViewRemoved),
            RoomsComponentNotification.RoomCategoryUpdatedNotification => typeof(RoomCategoryData),
            RoomsComponentNotification.RoomCategoryAddedNotification => typeof(RoomCategoryData),
            RoomsComponentNotification.RoomCategoryRemovedNotification => typeof(RoomCategoryRemoved),
            RoomsComponentNotification.RoomUpdatedNotification => typeof(RoomData),
            RoomsComponentNotification.RoomAddedNotification => typeof(RoomData),
            RoomsComponentNotification.RoomRemovedNotification => typeof(RoomRemoved),
            RoomsComponentNotification.RoomPopulationUpdated => typeof(RoomsPopulationUpdate),
            RoomsComponentNotification.RoomMemberJoined => typeof(RoomMemberData),
            RoomsComponentNotification.RoomMemberLeft => typeof(RoomMemberRemoved),
            RoomsComponentNotification.RoomMemberUpdated => typeof(RoomMemberData),
            RoomsComponentNotification.RoomKick => typeof(RoomMemberKicked),
            RoomsComponentNotification.RoomHostTransfer => typeof(RoomHostTransfered),
            RoomsComponentNotification.RoomAttributesSet => typeof(RoomAttributesSet),
            RoomsComponentNotification.MemberAttributesSet => typeof(MemberAttributesSet),
            _ => typeof(NullStruct)
        };
        
        public enum RoomsComponentCommand : ushort
        {
            selectViewUpdates = 10,
            selectCategoryUpdates = 11,
            joinRoom = 20,
            leaveRoom = 21,
            kickUser = 31,
            transferRoomHost = 40,
            createRoomCategory = 100,
            removeRoomCategory = 101,
            createRoom = 102,
            removeRoom = 103,
            clearBannedUsers = 104,
            unbanUser = 105,
            getViews = 109,
            createScheduledCategory = 110,
            deleteScheduledCategory = 111,
            getScheduledCategories = 112,
            lookupRoomData = 120,
            listBannedUsers = 122,
            setRoomAttributes = 130,
            checkEntryCriteria = 140,
            toggleJoinedRoomNotifications = 150,
            selectPseudoRoomUpdates = 160,
            setMemberAttributes = 170,
        }
        
        public enum RoomsComponentNotification : ushort
        {
            RoomViewUpdatedNotification = 10,
            RoomViewAddedNotification = 11,
            RoomViewRemovedNotification = 12,
            RoomCategoryUpdatedNotification = 20,
            RoomCategoryAddedNotification = 21,
            RoomCategoryRemovedNotification = 22,
            RoomUpdatedNotification = 30,
            RoomAddedNotification = 31,
            RoomRemovedNotification = 32,
            RoomPopulationUpdated = 40,
            RoomMemberJoined = 50,
            RoomMemberLeft = 51,
            RoomMemberUpdated = 52,
            RoomKick = 60,
            RoomHostTransfer = 70,
            RoomAttributesSet = 80,
            MemberAttributesSet = 90,
        }
        
    }
}
