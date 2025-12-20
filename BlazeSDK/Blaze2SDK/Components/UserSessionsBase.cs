using Blaze2SDK.Blaze;
using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class UserSessionsBase
    {
        public const ushort Id = 30722;
        public const string Name = "UserSessions";
        
        public class Server : BlazeServerComponent<UserSessionsCommand, UserSessionsNotification, Blaze2RpcError>
        {
            public Server() : base(UserSessionsBase.Id, UserSessionsBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUserInformation)]
            public virtual Task<NullStruct> LookupUserInformationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUsersInformation)]
            public virtual Task<NullStruct> LookupUsersInformationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.fetchExtendedData)]
            public virtual Task<NullStruct> FetchExtendedDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updatePingSiteLatency)]
            public virtual Task<NullStruct> UpdatePingSiteLatencyAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateExtendedDataAttribute)]
            public virtual Task<NullStruct> UpdateExtendedDataAttributeAsync(UpdateUserSessionAttributeRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.assignUserToGroup)]
            public virtual Task<NullStruct> AssignUserToGroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.removeUserFromGroup)]
            public virtual Task<NullStruct> RemoveUserFromGroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateHardwareFlags)]
            public virtual Task<NullStruct> UpdateHardwareFlagsAsync(UpdateHardwareFlagsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.getPermissions)]
            public virtual Task<NullStruct> GetPermissionsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.getAccessGroup)]
            public virtual Task<NullStruct> GetAccessGroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.checkOnlineStatus)]
            public virtual Task<NullStruct> CheckOnlineStatusAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUser)]
            public virtual Task<UserData> LookupUserAsync(UserIdentification request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUsers)]
            public virtual Task<UserDataResponse> LookupUsersAsync(LookupUsersRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateNetworkInfo)]
            public virtual Task<NullStruct> UpdateNetworkInfoAsync(NetworkInfo request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.listDefaultAccessGroup)]
            public virtual Task<NullStruct> ListDefaultAccessGroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.listAuthorization)]
            public virtual Task<NullStruct> ListAuthorizationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUserGeoIPData)]
            public virtual Task<GeoLocationData> LookupUserGeoIPDataAsync(UserIdentification request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.overrideUserGeoIPData)]
            public virtual Task<NullStruct> OverrideUserGeoIPDataAsync(GeoLocationData request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.setUserInfoAttribute)]
            public virtual Task<NullStruct> SetUserInfoAttributeAsync(SetUserInfoAttributeRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyUserSessionExtendedDataUpdateAsync(BlazeServerConnection connection, UserSessionExtendedDataUpdate notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(UserSessionsBase.Id, (ushort)UserSessionsNotification.UserSessionExtendedDataUpdate, notification, waitUntilFree);
            }
            
            public static Task NotifyUserAddedAsync(BlazeServerConnection connection, UserIdentification notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(UserSessionsBase.Id, (ushort)UserSessionsNotification.UserAdded, notification, waitUntilFree);
            }
            
            public static Task NotifyUserSessionUnsubscribedAsync(BlazeServerConnection connection, UserIdentification notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(UserSessionsBase.Id, (ushort)UserSessionsNotification.UserSessionUnsubscribed, notification, waitUntilFree);
            }
            
            public static Task NotifyUserSessionDisconnectedAsync(BlazeServerConnection connection, UserSessionDisconnectReason notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(UserSessionsBase.Id, (ushort)UserSessionsNotification.UserSessionDisconnected, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(UserSessionsCommand command) => UserSessionsBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(UserSessionsCommand command) => UserSessionsBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(UserSessionsCommand command) => UserSessionsBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(UserSessionsNotification notification) => UserSessionsBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<UserSessionsCommand, UserSessionsNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(UserSessionsBase.Id, UserSessionsBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct LookupUserInformation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserInformation, new NullStruct());
            }
            public Task<NullStruct> LookupUserInformationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserInformation, new NullStruct());
            }
            
            public NullStruct LookupUsersInformation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsersInformation, new NullStruct());
            }
            public Task<NullStruct> LookupUsersInformationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsersInformation, new NullStruct());
            }
            
            public NullStruct FetchExtendedData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchExtendedData, new NullStruct());
            }
            public Task<NullStruct> FetchExtendedDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchExtendedData, new NullStruct());
            }
            
            public NullStruct UpdatePingSiteLatency()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updatePingSiteLatency, new NullStruct());
            }
            public Task<NullStruct> UpdatePingSiteLatencyAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updatePingSiteLatency, new NullStruct());
            }
            
            public NullStruct UpdateExtendedDataAttribute(UpdateUserSessionAttributeRequest request)
            {
                return Connection.SendRequest<UpdateUserSessionAttributeRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateExtendedDataAttribute, request);
            }
            public Task<NullStruct> UpdateExtendedDataAttributeAsync(UpdateUserSessionAttributeRequest request)
            {
                return Connection.SendRequestAsync<UpdateUserSessionAttributeRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateExtendedDataAttribute, request);
            }
            
            public NullStruct AssignUserToGroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.assignUserToGroup, new NullStruct());
            }
            public Task<NullStruct> AssignUserToGroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.assignUserToGroup, new NullStruct());
            }
            
            public NullStruct RemoveUserFromGroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.removeUserFromGroup, new NullStruct());
            }
            public Task<NullStruct> RemoveUserFromGroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.removeUserFromGroup, new NullStruct());
            }
            
            public NullStruct UpdateHardwareFlags(UpdateHardwareFlagsRequest request)
            {
                return Connection.SendRequest<UpdateHardwareFlagsRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateHardwareFlags, request);
            }
            public Task<NullStruct> UpdateHardwareFlagsAsync(UpdateHardwareFlagsRequest request)
            {
                return Connection.SendRequestAsync<UpdateHardwareFlagsRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateHardwareFlags, request);
            }
            
            public NullStruct GetPermissions()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.getPermissions, new NullStruct());
            }
            public Task<NullStruct> GetPermissionsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.getPermissions, new NullStruct());
            }
            
            public NullStruct GetAccessGroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.getAccessGroup, new NullStruct());
            }
            public Task<NullStruct> GetAccessGroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.getAccessGroup, new NullStruct());
            }
            
            public NullStruct CheckOnlineStatus()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.checkOnlineStatus, new NullStruct());
            }
            public Task<NullStruct> CheckOnlineStatusAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.checkOnlineStatus, new NullStruct());
            }
            
            public UserData LookupUser(UserIdentification request)
            {
                return Connection.SendRequest<UserIdentification, UserData, NullStruct>(this, (ushort)UserSessionsCommand.lookupUser, request);
            }
            public Task<UserData> LookupUserAsync(UserIdentification request)
            {
                return Connection.SendRequestAsync<UserIdentification, UserData, NullStruct>(this, (ushort)UserSessionsCommand.lookupUser, request);
            }
            
            public UserDataResponse LookupUsers(LookupUsersRequest request)
            {
                return Connection.SendRequest<LookupUsersRequest, UserDataResponse, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsers, request);
            }
            public Task<UserDataResponse> LookupUsersAsync(LookupUsersRequest request)
            {
                return Connection.SendRequestAsync<LookupUsersRequest, UserDataResponse, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsers, request);
            }
            
            public NullStruct UpdateNetworkInfo(NetworkInfo request)
            {
                return Connection.SendRequest<NetworkInfo, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateNetworkInfo, request);
            }
            public Task<NullStruct> UpdateNetworkInfoAsync(NetworkInfo request)
            {
                return Connection.SendRequestAsync<NetworkInfo, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateNetworkInfo, request);
            }
            
            public NullStruct ListDefaultAccessGroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.listDefaultAccessGroup, new NullStruct());
            }
            public Task<NullStruct> ListDefaultAccessGroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.listDefaultAccessGroup, new NullStruct());
            }
            
            public NullStruct ListAuthorization()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.listAuthorization, new NullStruct());
            }
            public Task<NullStruct> ListAuthorizationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.listAuthorization, new NullStruct());
            }
            
            public GeoLocationData LookupUserGeoIPData(UserIdentification request)
            {
                return Connection.SendRequest<UserIdentification, GeoLocationData, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserGeoIPData, request);
            }
            public Task<GeoLocationData> LookupUserGeoIPDataAsync(UserIdentification request)
            {
                return Connection.SendRequestAsync<UserIdentification, GeoLocationData, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserGeoIPData, request);
            }
            
            public NullStruct OverrideUserGeoIPData(GeoLocationData request)
            {
                return Connection.SendRequest<GeoLocationData, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.overrideUserGeoIPData, request);
            }
            public Task<NullStruct> OverrideUserGeoIPDataAsync(GeoLocationData request)
            {
                return Connection.SendRequestAsync<GeoLocationData, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.overrideUserGeoIPData, request);
            }
            
            public NullStruct SetUserInfoAttribute(SetUserInfoAttributeRequest request)
            {
                return Connection.SendRequest<SetUserInfoAttributeRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.setUserInfoAttribute, request);
            }
            public Task<NullStruct> SetUserInfoAttributeAsync(SetUserInfoAttributeRequest request)
            {
                return Connection.SendRequestAsync<SetUserInfoAttributeRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.setUserInfoAttribute, request);
            }
            
            
            [BlazeNotification((ushort)UserSessionsNotification.UserSessionExtendedDataUpdate)]
            public virtual Task OnUserSessionExtendedDataUpdateAsync(UserSessionExtendedDataUpdate notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnUserSessionExtendedDataUpdateAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserAdded)]
            public virtual Task OnUserAddedAsync(UserIdentification notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnUserAddedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserSessionUnsubscribed)]
            public virtual Task OnUserSessionUnsubscribedAsync(UserIdentification notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnUserSessionUnsubscribedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserSessionDisconnected)]
            public virtual Task OnUserSessionDisconnectedAsync(UserSessionDisconnectReason notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze2SDK] - {GetType().FullName}: OnUserSessionDisconnectedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(UserSessionsCommand command) => UserSessionsBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(UserSessionsCommand command) => UserSessionsBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(UserSessionsCommand command) => UserSessionsBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(UserSessionsNotification notification) => UserSessionsBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<UserSessionsCommand, UserSessionsNotification, Blaze2RpcError>
        {
            public Proxy() : base(UserSessionsBase.Id, UserSessionsBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUserInformation)]
            public virtual Task<NullStruct> LookupUserInformationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserInformation, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUsersInformation)]
            public virtual Task<NullStruct> LookupUsersInformationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsersInformation, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.fetchExtendedData)]
            public virtual Task<NullStruct> FetchExtendedDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchExtendedData, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updatePingSiteLatency)]
            public virtual Task<NullStruct> UpdatePingSiteLatencyAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updatePingSiteLatency, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateExtendedDataAttribute)]
            public virtual Task<NullStruct> UpdateExtendedDataAttributeAsync(UpdateUserSessionAttributeRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateUserSessionAttributeRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateExtendedDataAttribute, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.assignUserToGroup)]
            public virtual Task<NullStruct> AssignUserToGroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.assignUserToGroup, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.removeUserFromGroup)]
            public virtual Task<NullStruct> RemoveUserFromGroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.removeUserFromGroup, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateHardwareFlags)]
            public virtual Task<NullStruct> UpdateHardwareFlagsAsync(UpdateHardwareFlagsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateHardwareFlagsRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateHardwareFlags, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.getPermissions)]
            public virtual Task<NullStruct> GetPermissionsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.getPermissions, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.getAccessGroup)]
            public virtual Task<NullStruct> GetAccessGroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.getAccessGroup, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.checkOnlineStatus)]
            public virtual Task<NullStruct> CheckOnlineStatusAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.checkOnlineStatus, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUser)]
            public virtual Task<UserData> LookupUserAsync(UserIdentification request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UserIdentification, UserData, NullStruct>(this, (ushort)UserSessionsCommand.lookupUser, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUsers)]
            public virtual Task<UserDataResponse> LookupUsersAsync(LookupUsersRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<LookupUsersRequest, UserDataResponse, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsers, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateNetworkInfo)]
            public virtual Task<NullStruct> UpdateNetworkInfoAsync(NetworkInfo request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NetworkInfo, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateNetworkInfo, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.listDefaultAccessGroup)]
            public virtual Task<NullStruct> ListDefaultAccessGroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.listDefaultAccessGroup, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.listAuthorization)]
            public virtual Task<NullStruct> ListAuthorizationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.listAuthorization, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUserGeoIPData)]
            public virtual Task<GeoLocationData> LookupUserGeoIPDataAsync(UserIdentification request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UserIdentification, GeoLocationData, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserGeoIPData, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.overrideUserGeoIPData)]
            public virtual Task<NullStruct> OverrideUserGeoIPDataAsync(GeoLocationData request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<GeoLocationData, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.overrideUserGeoIPData, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.setUserInfoAttribute)]
            public virtual Task<NullStruct> SetUserInfoAttributeAsync(SetUserInfoAttributeRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<SetUserInfoAttributeRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.setUserInfoAttribute, request);
            }
            
            
            [BlazeNotification((ushort)UserSessionsNotification.UserSessionExtendedDataUpdate)]
            public virtual Task<UserSessionExtendedDataUpdate> OnUserSessionExtendedDataUpdateAsync(UserSessionExtendedDataUpdate notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserAdded)]
            public virtual Task<UserIdentification> OnUserAddedAsync(UserIdentification notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserSessionUnsubscribed)]
            public virtual Task<UserIdentification> OnUserSessionUnsubscribedAsync(UserIdentification notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserSessionDisconnected)]
            public virtual Task<UserSessionDisconnectReason> OnUserSessionDisconnectedAsync(UserSessionDisconnectReason notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(UserSessionsCommand command) => UserSessionsBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(UserSessionsCommand command) => UserSessionsBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(UserSessionsCommand command) => UserSessionsBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(UserSessionsNotification notification) => UserSessionsBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(UserSessionsCommand command) => command switch
        {
            UserSessionsCommand.lookupUserInformation => typeof(NullStruct),
            UserSessionsCommand.lookupUsersInformation => typeof(NullStruct),
            UserSessionsCommand.fetchExtendedData => typeof(NullStruct),
            UserSessionsCommand.updatePingSiteLatency => typeof(NullStruct),
            UserSessionsCommand.updateExtendedDataAttribute => typeof(UpdateUserSessionAttributeRequest),
            UserSessionsCommand.assignUserToGroup => typeof(NullStruct),
            UserSessionsCommand.removeUserFromGroup => typeof(NullStruct),
            UserSessionsCommand.updateHardwareFlags => typeof(UpdateHardwareFlagsRequest),
            UserSessionsCommand.getPermissions => typeof(NullStruct),
            UserSessionsCommand.getAccessGroup => typeof(NullStruct),
            UserSessionsCommand.checkOnlineStatus => typeof(NullStruct),
            UserSessionsCommand.lookupUser => typeof(UserIdentification),
            UserSessionsCommand.lookupUsers => typeof(LookupUsersRequest),
            UserSessionsCommand.updateNetworkInfo => typeof(NetworkInfo),
            UserSessionsCommand.listDefaultAccessGroup => typeof(NullStruct),
            UserSessionsCommand.listAuthorization => typeof(NullStruct),
            UserSessionsCommand.lookupUserGeoIPData => typeof(UserIdentification),
            UserSessionsCommand.overrideUserGeoIPData => typeof(GeoLocationData),
            UserSessionsCommand.setUserInfoAttribute => typeof(SetUserInfoAttributeRequest),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(UserSessionsCommand command) => command switch
        {
            UserSessionsCommand.lookupUserInformation => typeof(NullStruct),
            UserSessionsCommand.lookupUsersInformation => typeof(NullStruct),
            UserSessionsCommand.fetchExtendedData => typeof(NullStruct),
            UserSessionsCommand.updatePingSiteLatency => typeof(NullStruct),
            UserSessionsCommand.updateExtendedDataAttribute => typeof(NullStruct),
            UserSessionsCommand.assignUserToGroup => typeof(NullStruct),
            UserSessionsCommand.removeUserFromGroup => typeof(NullStruct),
            UserSessionsCommand.updateHardwareFlags => typeof(NullStruct),
            UserSessionsCommand.getPermissions => typeof(NullStruct),
            UserSessionsCommand.getAccessGroup => typeof(NullStruct),
            UserSessionsCommand.checkOnlineStatus => typeof(NullStruct),
            UserSessionsCommand.lookupUser => typeof(UserData),
            UserSessionsCommand.lookupUsers => typeof(UserDataResponse),
            UserSessionsCommand.updateNetworkInfo => typeof(NullStruct),
            UserSessionsCommand.listDefaultAccessGroup => typeof(NullStruct),
            UserSessionsCommand.listAuthorization => typeof(NullStruct),
            UserSessionsCommand.lookupUserGeoIPData => typeof(GeoLocationData),
            UserSessionsCommand.overrideUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.setUserInfoAttribute => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(UserSessionsCommand command) => command switch
        {
            UserSessionsCommand.lookupUserInformation => typeof(NullStruct),
            UserSessionsCommand.lookupUsersInformation => typeof(NullStruct),
            UserSessionsCommand.fetchExtendedData => typeof(NullStruct),
            UserSessionsCommand.updatePingSiteLatency => typeof(NullStruct),
            UserSessionsCommand.updateExtendedDataAttribute => typeof(NullStruct),
            UserSessionsCommand.assignUserToGroup => typeof(NullStruct),
            UserSessionsCommand.removeUserFromGroup => typeof(NullStruct),
            UserSessionsCommand.updateHardwareFlags => typeof(NullStruct),
            UserSessionsCommand.getPermissions => typeof(NullStruct),
            UserSessionsCommand.getAccessGroup => typeof(NullStruct),
            UserSessionsCommand.checkOnlineStatus => typeof(NullStruct),
            UserSessionsCommand.lookupUser => typeof(NullStruct),
            UserSessionsCommand.lookupUsers => typeof(NullStruct),
            UserSessionsCommand.updateNetworkInfo => typeof(NullStruct),
            UserSessionsCommand.listDefaultAccessGroup => typeof(NullStruct),
            UserSessionsCommand.listAuthorization => typeof(NullStruct),
            UserSessionsCommand.lookupUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.overrideUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.setUserInfoAttribute => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(UserSessionsNotification notification) => notification switch
        {
            UserSessionsNotification.UserSessionExtendedDataUpdate => typeof(UserSessionExtendedDataUpdate),
            UserSessionsNotification.UserAdded => typeof(UserIdentification),
            UserSessionsNotification.UserSessionUnsubscribed => typeof(UserIdentification),
            UserSessionsNotification.UserSessionDisconnected => typeof(UserSessionDisconnectReason),
            _ => typeof(NullStruct)
        };
        
        public enum UserSessionsCommand : ushort
        {
            lookupUserInformation = 1,
            lookupUsersInformation = 2,
            fetchExtendedData = 3,
            updatePingSiteLatency = 4,
            updateExtendedDataAttribute = 5,
            assignUserToGroup = 6,
            removeUserFromGroup = 7,
            updateHardwareFlags = 8,
            getPermissions = 9,
            getAccessGroup = 10,
            checkOnlineStatus = 11,
            lookupUser = 12,
            lookupUsers = 13,
            updateNetworkInfo = 20,
            listDefaultAccessGroup = 21,
            listAuthorization = 22,
            lookupUserGeoIPData = 23,
            overrideUserGeoIPData = 24,
            setUserInfoAttribute = 25,
        }
        
        public enum UserSessionsNotification : ushort
        {
            UserSessionExtendedDataUpdate = 1,
            UserAdded = 2,
            UserSessionUnsubscribed = 3,
            UserSessionDisconnected = 4,
        }
        
    }
}
