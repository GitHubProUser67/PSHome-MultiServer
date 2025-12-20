using Blaze3SDK.Blaze.Association;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class AssociationListsComponentBase
    {
        public const ushort Id = 25;
        public const string Name = "AssociationListsComponent";
        
        public class Server : BlazeServerComponent<AssociationListsComponentCommand, AssociationListsComponentNotification, Blaze3RpcError>
        {
            public Server() : base(AssociationListsComponentBase.Id, AssociationListsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.addUsersToList)]
            public virtual Task<UpdateListMembersResponse> AddUsersToListAsync(UpdateListMembersRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.removeUsersFromList)]
            public virtual Task<UpdateListMembersResponse> RemoveUsersFromListAsync(UpdateListMembersRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.clearLists)]
            public virtual Task<NullStruct> ClearListsAsync(UpdateListsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.setUsersToList)]
            public virtual Task<UpdateListMembersResponse> SetUsersToListAsync(UpdateListMembersRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.getListForUser)]
            public virtual Task<ListMembers> GetListForUserAsync(GetListForUserRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.getLists)]
            public virtual Task<Lists> GetListsAsync(GetListsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.subscribeToLists)]
            public virtual Task<NullStruct> SubscribeToListsAsync(UpdateListsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.unsubscribeFromLists)]
            public virtual Task<NullStruct> UnsubscribeFromListsAsync(UpdateListsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.getConfigListsInfo)]
            public virtual Task<ConfigLists> GetConfigListsInfoAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyUpdateListMembershipAsync(BlazeServerConnection connection, UpdateListWithMembersRequest notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(AssociationListsComponentBase.Id, (ushort)AssociationListsComponentNotification.NotifyUpdateListMembership, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(AssociationListsComponentNotification notification) => AssociationListsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<AssociationListsComponentCommand, AssociationListsComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(AssociationListsComponentBase.Id, AssociationListsComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public UpdateListMembersResponse AddUsersToList(UpdateListMembersRequest request)
            {
                return Connection.SendRequest<UpdateListMembersRequest, UpdateListMembersResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.addUsersToList, request);
            }
            public Task<UpdateListMembersResponse> AddUsersToListAsync(UpdateListMembersRequest request)
            {
                return Connection.SendRequestAsync<UpdateListMembersRequest, UpdateListMembersResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.addUsersToList, request);
            }
            
            public UpdateListMembersResponse RemoveUsersFromList(UpdateListMembersRequest request)
            {
                return Connection.SendRequest<UpdateListMembersRequest, UpdateListMembersResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.removeUsersFromList, request);
            }
            public Task<UpdateListMembersResponse> RemoveUsersFromListAsync(UpdateListMembersRequest request)
            {
                return Connection.SendRequestAsync<UpdateListMembersRequest, UpdateListMembersResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.removeUsersFromList, request);
            }
            
            public NullStruct ClearLists(UpdateListsRequest request)
            {
                return Connection.SendRequest<UpdateListsRequest, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.clearLists, request);
            }
            public Task<NullStruct> ClearListsAsync(UpdateListsRequest request)
            {
                return Connection.SendRequestAsync<UpdateListsRequest, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.clearLists, request);
            }
            
            public UpdateListMembersResponse SetUsersToList(UpdateListMembersRequest request)
            {
                return Connection.SendRequest<UpdateListMembersRequest, UpdateListMembersResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.setUsersToList, request);
            }
            public Task<UpdateListMembersResponse> SetUsersToListAsync(UpdateListMembersRequest request)
            {
                return Connection.SendRequestAsync<UpdateListMembersRequest, UpdateListMembersResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.setUsersToList, request);
            }
            
            public ListMembers GetListForUser(GetListForUserRequest request)
            {
                return Connection.SendRequest<GetListForUserRequest, ListMembers, NullStruct>(this, (ushort)AssociationListsComponentCommand.getListForUser, request);
            }
            public Task<ListMembers> GetListForUserAsync(GetListForUserRequest request)
            {
                return Connection.SendRequestAsync<GetListForUserRequest, ListMembers, NullStruct>(this, (ushort)AssociationListsComponentCommand.getListForUser, request);
            }
            
            public Lists GetLists(GetListsRequest request)
            {
                return Connection.SendRequest<GetListsRequest, Lists, NullStruct>(this, (ushort)AssociationListsComponentCommand.getLists, request);
            }
            public Task<Lists> GetListsAsync(GetListsRequest request)
            {
                return Connection.SendRequestAsync<GetListsRequest, Lists, NullStruct>(this, (ushort)AssociationListsComponentCommand.getLists, request);
            }
            
            public NullStruct SubscribeToLists(UpdateListsRequest request)
            {
                return Connection.SendRequest<UpdateListsRequest, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.subscribeToLists, request);
            }
            public Task<NullStruct> SubscribeToListsAsync(UpdateListsRequest request)
            {
                return Connection.SendRequestAsync<UpdateListsRequest, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.subscribeToLists, request);
            }
            
            public NullStruct UnsubscribeFromLists(UpdateListsRequest request)
            {
                return Connection.SendRequest<UpdateListsRequest, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.unsubscribeFromLists, request);
            }
            public Task<NullStruct> UnsubscribeFromListsAsync(UpdateListsRequest request)
            {
                return Connection.SendRequestAsync<UpdateListsRequest, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.unsubscribeFromLists, request);
            }
            
            public ConfigLists GetConfigListsInfo()
            {
                return Connection.SendRequest<NullStruct, ConfigLists, NullStruct>(this, (ushort)AssociationListsComponentCommand.getConfigListsInfo, new NullStruct());
            }
            public Task<ConfigLists> GetConfigListsInfoAsync()
            {
                return Connection.SendRequestAsync<NullStruct, ConfigLists, NullStruct>(this, (ushort)AssociationListsComponentCommand.getConfigListsInfo, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)AssociationListsComponentNotification.NotifyUpdateListMembership)]
            public virtual Task OnNotifyUpdateListMembershipAsync(UpdateListWithMembersRequest notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyUpdateListMembershipAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(AssociationListsComponentCommand command) => AssociationListsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(AssociationListsComponentNotification notification) => AssociationListsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<AssociationListsComponentCommand, AssociationListsComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(AssociationListsComponentBase.Id, AssociationListsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.addUsersToList)]
            public virtual Task<UpdateListMembersResponse> AddUsersToListAsync(UpdateListMembersRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateListMembersRequest, UpdateListMembersResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.addUsersToList, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.removeUsersFromList)]
            public virtual Task<UpdateListMembersResponse> RemoveUsersFromListAsync(UpdateListMembersRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateListMembersRequest, UpdateListMembersResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.removeUsersFromList, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.clearLists)]
            public virtual Task<NullStruct> ClearListsAsync(UpdateListsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateListsRequest, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.clearLists, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.setUsersToList)]
            public virtual Task<UpdateListMembersResponse> SetUsersToListAsync(UpdateListMembersRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateListMembersRequest, UpdateListMembersResponse, NullStruct>(this, (ushort)AssociationListsComponentCommand.setUsersToList, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.getListForUser)]
            public virtual Task<ListMembers> GetListForUserAsync(GetListForUserRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetListForUserRequest, ListMembers, NullStruct>(this, (ushort)AssociationListsComponentCommand.getListForUser, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.getLists)]
            public virtual Task<Lists> GetListsAsync(GetListsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GetListsRequest, Lists, NullStruct>(this, (ushort)AssociationListsComponentCommand.getLists, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.subscribeToLists)]
            public virtual Task<NullStruct> SubscribeToListsAsync(UpdateListsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateListsRequest, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.subscribeToLists, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.unsubscribeFromLists)]
            public virtual Task<NullStruct> UnsubscribeFromListsAsync(UpdateListsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateListsRequest, NullStruct, NullStruct>(this, (ushort)AssociationListsComponentCommand.unsubscribeFromLists, request);
            }
            
            [BlazeCommand((ushort)AssociationListsComponentCommand.getConfigListsInfo)]
            public virtual Task<ConfigLists> GetConfigListsInfoAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, ConfigLists, NullStruct>(this, (ushort)AssociationListsComponentCommand.getConfigListsInfo, request);
            }
            
            
            [BlazeNotification((ushort)AssociationListsComponentNotification.NotifyUpdateListMembership)]
            public virtual Task<UpdateListWithMembersRequest> OnNotifyUpdateListMembershipAsync(UpdateListWithMembersRequest notification)
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
            AssociationListsComponentCommand.addUsersToList => typeof(UpdateListMembersRequest),
            AssociationListsComponentCommand.removeUsersFromList => typeof(UpdateListMembersRequest),
            AssociationListsComponentCommand.clearLists => typeof(UpdateListsRequest),
            AssociationListsComponentCommand.setUsersToList => typeof(UpdateListMembersRequest),
            AssociationListsComponentCommand.getListForUser => typeof(GetListForUserRequest),
            AssociationListsComponentCommand.getLists => typeof(GetListsRequest),
            AssociationListsComponentCommand.subscribeToLists => typeof(UpdateListsRequest),
            AssociationListsComponentCommand.unsubscribeFromLists => typeof(UpdateListsRequest),
            AssociationListsComponentCommand.getConfigListsInfo => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(AssociationListsComponentCommand command) => command switch
        {
            AssociationListsComponentCommand.addUsersToList => typeof(UpdateListMembersResponse),
            AssociationListsComponentCommand.removeUsersFromList => typeof(UpdateListMembersResponse),
            AssociationListsComponentCommand.clearLists => typeof(NullStruct),
            AssociationListsComponentCommand.setUsersToList => typeof(UpdateListMembersResponse),
            AssociationListsComponentCommand.getListForUser => typeof(ListMembers),
            AssociationListsComponentCommand.getLists => typeof(Lists),
            AssociationListsComponentCommand.subscribeToLists => typeof(NullStruct),
            AssociationListsComponentCommand.unsubscribeFromLists => typeof(NullStruct),
            AssociationListsComponentCommand.getConfigListsInfo => typeof(ConfigLists),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(AssociationListsComponentCommand command) => command switch
        {
            AssociationListsComponentCommand.addUsersToList => typeof(NullStruct),
            AssociationListsComponentCommand.removeUsersFromList => typeof(NullStruct),
            AssociationListsComponentCommand.clearLists => typeof(NullStruct),
            AssociationListsComponentCommand.setUsersToList => typeof(NullStruct),
            AssociationListsComponentCommand.getListForUser => typeof(NullStruct),
            AssociationListsComponentCommand.getLists => typeof(NullStruct),
            AssociationListsComponentCommand.subscribeToLists => typeof(NullStruct),
            AssociationListsComponentCommand.unsubscribeFromLists => typeof(NullStruct),
            AssociationListsComponentCommand.getConfigListsInfo => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(AssociationListsComponentNotification notification) => notification switch
        {
            AssociationListsComponentNotification.NotifyUpdateListMembership => typeof(UpdateListWithMembersRequest),
            _ => typeof(NullStruct)
        };
        
        public enum AssociationListsComponentCommand : ushort
        {
            addUsersToList = 1,
            removeUsersFromList = 2,
            clearLists = 3,
            setUsersToList = 4,
            getListForUser = 5,
            getLists = 6,
            subscribeToLists = 7,
            unsubscribeFromLists = 8,
            getConfigListsInfo = 9,
        }
        
        public enum AssociationListsComponentNotification : ushort
        {
            NotifyUpdateListMembership = 1,
        }
        
    }
}
