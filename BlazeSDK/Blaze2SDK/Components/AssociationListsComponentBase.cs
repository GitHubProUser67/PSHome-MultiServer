using Blaze2SDK.Blaze.Association;
using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class AssociationListsComponentBase
    {
        public const ushort Id = 25;
        public const string Name = "AssociationListsComponent";
        
        public class Server : BlazeServerComponent<AssociationListsComponentCommand, AssociationListsComponentNotification, Blaze2RpcError>
        {
            public Server() : base(AssociationListsComponentBase.Id, AssociationListsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.addUsersToList)]
            public virtual Task<UpdateListResponse> AddUsersToListAsync(UpdateList request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.removeUsersFromList)]
            public virtual Task<UpdateListResponse> RemoveUsersFromListAsync(UpdateList request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.clearList)]
            public virtual Task<NullStruct> ClearListAsync(ClearList request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.setUsersToList)]
            public virtual Task<UpdateListResponse> SetUsersToListAsync(UpdateList request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.getListForUser)]
            public virtual Task<AssociationListInfo> GetListForUserAsync(GetListForUser request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.getLists)]
            public virtual Task<AssociationListCollectionInfo> GetListsAsync(GetUserLists request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.subscribeToLists)]
            public virtual Task<NullStruct> SubscribeToListsAsync(SubscriptionInfo request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.unsubscribeFromLists)]
            public virtual Task<NullStruct> UnsubscribeFromListsAsync(SubscriptionInfo request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyListMemberUpdatedAsync(BlazeServerConnection connection, MemberInfo notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(AssociationListsComponentBase.Id, (ushort)AssociationListsComponentNotification.NotifyListMemberUpdated, notification, waitUntilFree);
            }
            
            public static Task NotifyListMemberRemovedAsync(BlazeServerConnection connection, AssociationListInfo notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(AssociationListsComponentBase.Id, (ushort)AssociationListsComponentNotification.NotifyListMemberRemoved, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(AssociationListsComponentNotification notification) => AssociationListsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<AssociationListsComponentCommand, AssociationListsComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(AssociationListsComponentBase.Id, AssociationListsComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public UpdateListResponse AddUsersToList(UpdateList request)
            {
                return Connection.SendRequest<UpdateList, UpdateListResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.addUsersToList, request);
            }
            public Task<UpdateListResponse> AddUsersToListAsync(UpdateList request)
            {
                return Connection.SendRequestAsync<UpdateList, UpdateListResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.addUsersToList, request);
            }
            
            public UpdateListResponse RemoveUsersFromList(UpdateList request)
            {
                return Connection.SendRequest<UpdateList, UpdateListResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.removeUsersFromList, request);
            }
            public Task<UpdateListResponse> RemoveUsersFromListAsync(UpdateList request)
            {
                return Connection.SendRequestAsync<UpdateList, UpdateListResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.removeUsersFromList, request);
            }
            
            public NullStruct ClearList(ClearList request)
            {
                return Connection.SendRequest<ClearList, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.clearList, request);
            }
            public Task<NullStruct> ClearListAsync(ClearList request)
            {
                return Connection.SendRequestAsync<ClearList, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.clearList, request);
            }
            
            public UpdateListResponse SetUsersToList(UpdateList request)
            {
                return Connection.SendRequest<UpdateList, UpdateListResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.setUsersToList, request);
            }
            public Task<UpdateListResponse> SetUsersToListAsync(UpdateList request)
            {
                return Connection.SendRequestAsync<UpdateList, UpdateListResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.setUsersToList, request);
            }
            
            public AssociationListInfo GetListForUser(GetListForUser request)
            {
                return Connection.SendRequest<GetListForUser, AssociationListInfo, NullStruct>(this, (ushort)AssociationListsComponentCommand.getListForUser, request);
            }
            public Task<AssociationListInfo> GetListForUserAsync(GetListForUser request)
            {
                return Connection.SendRequestAsync<GetListForUser, AssociationListInfo, NullStruct>(this, (ushort)AssociationListsComponentCommand.getListForUser, request);
            }
            
            public AssociationListCollectionInfo GetLists(GetUserLists request)
            {
                return Connection.SendRequest<GetUserLists, AssociationListCollectionInfo, NullStruct>(this, (ushort)AssociationListsComponentCommand.getLists, request);
            }
            public Task<AssociationListCollectionInfo> GetListsAsync(GetUserLists request)
            {
                return Connection.SendRequestAsync<GetUserLists, AssociationListCollectionInfo, NullStruct>(this, (ushort)AssociationListsComponentCommand.getLists, request);
            }
            
            public NullStruct SubscribeToLists(SubscriptionInfo request)
            {
                return Connection.SendRequest<SubscriptionInfo, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.subscribeToLists, request);
            }
            public Task<NullStruct> SubscribeToListsAsync(SubscriptionInfo request)
            {
                return Connection.SendRequestAsync<SubscriptionInfo, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.subscribeToLists, request);
            }
            
            public NullStruct UnsubscribeFromLists(SubscriptionInfo request)
            {
                return Connection.SendRequest<SubscriptionInfo, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.unsubscribeFromLists, request);
            }
            public Task<NullStruct> UnsubscribeFromListsAsync(SubscriptionInfo request)
            {
                return Connection.SendRequestAsync<SubscriptionInfo, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.unsubscribeFromLists, request);
            }
            
            
            [BlazeNotification((ushort)AssociationListsComponentNotification.NotifyListMemberUpdated)]
            public virtual Task OnNotifyListMemberUpdatedAsync(MemberInfo notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyListMemberUpdatedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)AssociationListsComponentNotification.NotifyListMemberRemoved)]
            public virtual Task OnNotifyListMemberRemovedAsync(AssociationListInfo notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnNotifyListMemberRemovedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(AssociationListsComponentNotification notification) => AssociationListsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<AssociationListsComponentCommand, AssociationListsComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(AssociationListsComponentBase.Id, AssociationListsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.addUsersToList)]
            public virtual Task<UpdateListResponse> AddUsersToListAsync(UpdateList request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateList, UpdateListResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.addUsersToList, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.removeUsersFromList)]
            public virtual Task<UpdateListResponse> RemoveUsersFromListAsync(UpdateList request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateList, UpdateListResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.removeUsersFromList, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.clearList)]
            public virtual Task<NullStruct> ClearListAsync(ClearList request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<ClearList, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.clearList, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.setUsersToList)]
            public virtual Task<UpdateListResponse> SetUsersToListAsync(UpdateList request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateList, UpdateListResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.setUsersToList, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.getListForUser)]
            public virtual Task<AssociationListInfo> GetListForUserAsync(GetListForUser request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetListForUser, AssociationListInfo, NullStruct>(this, (ushort)AssociationListsComponentCommand.getListForUser, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.getLists)]
            public virtual Task<AssociationListCollectionInfo> GetListsAsync(GetUserLists request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetUserLists, AssociationListCollectionInfo, NullStruct>(this, (ushort)AssociationListsComponentCommand.getLists, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.subscribeToLists)]
            public virtual Task<NullStruct> SubscribeToListsAsync(SubscriptionInfo request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SubscriptionInfo, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.subscribeToLists, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.unsubscribeFromLists)]
            public virtual Task<NullStruct> UnsubscribeFromListsAsync(SubscriptionInfo request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SubscriptionInfo, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.unsubscribeFromLists, request);
            }
            
            
            [BlazeNotification((ushort)AssociationListsComponentNotification.NotifyListMemberUpdated)]
            public virtual Task<MemberInfo> OnNotifyListMemberUpdatedAsync(MemberInfo notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)AssociationListsComponentNotification.NotifyListMemberRemoved)]
            public virtual Task<AssociationListInfo> OnNotifyListMemberRemovedAsync(AssociationListInfo notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(AssociationListsComponentNotification notification) => AssociationListsComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(AssociationListsComponentCommand command) => command switch
        {
            AssociationListsComponentCommand.addUsersToList => typeof(UpdateList),
            AssociationListsComponentCommand.removeUsersFromList => typeof(UpdateList),
            AssociationListsComponentCommand.clearList => typeof(ClearList),
            AssociationListsComponentCommand.setUsersToList => typeof(UpdateList),
            AssociationListsComponentCommand.getListForUser => typeof(GetListForUser),
            AssociationListsComponentCommand.getLists => typeof(GetUserLists),
            AssociationListsComponentCommand.subscribeToLists => typeof(SubscriptionInfo),
            AssociationListsComponentCommand.unsubscribeFromLists => typeof(SubscriptionInfo),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(AssociationListsComponentCommand command) => command switch
        {
            AssociationListsComponentCommand.addUsersToList => typeof(UpdateListResponse),
            AssociationListsComponentCommand.removeUsersFromList => typeof(UpdateListResponse),
            AssociationListsComponentCommand.clearList => typeof(NullStruct),
            AssociationListsComponentCommand.setUsersToList => typeof(UpdateListResponse),
            AssociationListsComponentCommand.getListForUser => typeof(AssociationListInfo),
            AssociationListsComponentCommand.getLists => typeof(AssociationListCollectionInfo),
            AssociationListsComponentCommand.subscribeToLists => typeof(NullStruct),
            AssociationListsComponentCommand.unsubscribeFromLists => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(AssociationListsComponentCommand command) => command switch
        {
            AssociationListsComponentCommand.addUsersToList => typeof(NullStruct),
            AssociationListsComponentCommand.removeUsersFromList => typeof(NullStruct),
            AssociationListsComponentCommand.clearList => typeof(NullStruct),
            AssociationListsComponentCommand.setUsersToList => typeof(NullStruct),
            AssociationListsComponentCommand.getListForUser => typeof(NullStruct),
            AssociationListsComponentCommand.getLists => typeof(NullStruct),
            AssociationListsComponentCommand.subscribeToLists => typeof(NullStruct),
            AssociationListsComponentCommand.unsubscribeFromLists => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(AssociationListsComponentNotification notification) => notification switch
        {
            AssociationListsComponentNotification.NotifyListMemberUpdated => typeof(MemberInfo),
            AssociationListsComponentNotification.NotifyListMemberRemoved => typeof(AssociationListInfo),
            _ => typeof(NullStruct)
        };
        
        public enum AssociationListsComponentCommand : ushort
        {
            addUsersToList = 1,
            removeUsersFromList = 2,
            clearList = 3,
            setUsersToList = 4,
            getListForUser = 5,
            getLists = 6,
            subscribeToLists = 7,
            unsubscribeFromLists = 8,
        }
        
        public enum AssociationListsComponentNotification : ushort
        {
            NotifyListMemberUpdated = 1,
            NotifyListMemberRemoved = 2,
        }
        
    }
}
