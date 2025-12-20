using Blaze3SDK.Blaze;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class UserSessionsBase
    {
        public const ushort Id = 30722;
        public const string Name = "UserSessions";
        
        public class Server : BlazeServerComponent<UserSessionsCommand, UserSessionsNotification, Blaze3RpcError>
        {
            public Server() : base(UserSessionsBase.Id, UserSessionsBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.fetchExtendedData)]
            public virtual Task<NullStruct> FetchExtendedDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateExtendedDataAttribute)]
            public virtual Task<NullStruct> UpdateExtendedDataAttributeAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateHardwareFlags)]
            public virtual Task<NullStruct> UpdateHardwareFlagsAsync(UpdateHardwareFlagsRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUser)]
            public virtual Task<NullStruct> LookupUserAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUsers)]
            public virtual Task<UserDataResponse> LookupUsersAsync(LookupUsersRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUsersByPrefix)]
            public virtual Task<NullStruct> LookupUsersByPrefixAsync(LookupUsersByPrefixRequest request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateNetworkInfo)]
            public virtual Task<NullStruct> UpdateNetworkInfoAsync(NetworkInfo request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUserGeoIPData)]
            public virtual Task<NullStruct> LookupUserGeoIPDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.overrideUserGeoIPData)]
            public virtual Task<NullStruct> OverrideUserGeoIPDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateUserSessionClientData)]
            public virtual Task<NullStruct> UpdateUserSessionClientDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.setUserInfoAttribute)]
            public virtual Task<NullStruct> SetUserInfoAttributeAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.resetUserGeoIPData)]
            public virtual Task<NullStruct> ResetUserGeoIPDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUserSessionId)]
            public virtual Task<NullStruct> LookupUserSessionIdAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.fetchLastLocaleUsedAndAuthError)]
            public virtual Task<NullStruct> FetchLastLocaleUsedAndAuthErrorAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.fetchUserFirstLastAuthTime)]
            public virtual Task<NullStruct> FetchUserFirstLastAuthTimeAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.resumeSession)]
            public virtual Task<NullStruct> ResumeSessionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyUserSessionExtendedDataUpdateAsync(BlazeServerConnection connection, UserSessionExtendedDataUpdate notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(UserSessionsBase.Id, (ushort)UserSessionsNotification.UserSessionExtendedDataUpdate, notification, waitUntilFree);
            }
            
            public static Task NotifyUserAddedAsync(BlazeServerConnection connection, NotifyUserAdded notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(UserSessionsBase.Id, (ushort)UserSessionsNotification.UserAdded, notification, waitUntilFree);
            }
            
            public static Task NotifyUserRemovedAsync(BlazeServerConnection connection, NotifyUserRemoved notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(UserSessionsBase.Id, (ushort)UserSessionsNotification.UserRemoved, notification, waitUntilFree);
            }
            
            public static Task NotifyUserSessionDisconnectedAsync(BlazeServerConnection connection, UserSessionDisconnectReason notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(UserSessionsBase.Id, (ushort)UserSessionsNotification.UserSessionDisconnected, notification, waitUntilFree);
            }
            
            public static Task NotifyUserUpdatedAsync(BlazeServerConnection connection, UserStatus notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(UserSessionsBase.Id, (ushort)UserSessionsNotification.UserUpdated, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(UserSessionsCommand command) => UserSessionsBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(UserSessionsCommand command) => UserSessionsBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(UserSessionsCommand command) => UserSessionsBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(UserSessionsNotification notification) => UserSessionsBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<UserSessionsCommand, UserSessionsNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(UserSessionsBase.Id, UserSessionsBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct FetchExtendedData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchExtendedData, new NullStruct());
            }
            public Task<NullStruct> FetchExtendedDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchExtendedData, new NullStruct());
            }
            
            public NullStruct UpdateExtendedDataAttribute()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateExtendedDataAttribute, new NullStruct());
            }
            public Task<NullStruct> UpdateExtendedDataAttributeAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateExtendedDataAttribute, new NullStruct());
            }
            
            public NullStruct UpdateHardwareFlags(UpdateHardwareFlagsRequest request)
            {
                return Connection.SendRequest<UpdateHardwareFlagsRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateHardwareFlags, request);
            }
            public Task<NullStruct> UpdateHardwareFlagsAsync(UpdateHardwareFlagsRequest request)
            {
                return Connection.SendRequestAsync<UpdateHardwareFlagsRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateHardwareFlags, request);
            }
            
            public NullStruct LookupUser()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUser, new NullStruct());
            }
            public Task<NullStruct> LookupUserAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUser, new NullStruct());
            }
            
            public UserDataResponse LookupUsers(LookupUsersRequest request)
            {
                return Connection.SendRequest<LookupUsersRequest, UserDataResponse, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsers, request);
            }
            public Task<UserDataResponse> LookupUsersAsync(LookupUsersRequest request)
            {
                return Connection.SendRequestAsync<LookupUsersRequest, UserDataResponse, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsers, request);
            }
            
            public NullStruct LookupUsersByPrefix(LookupUsersByPrefixRequest request)
            {
                return Connection.SendRequest<LookupUsersByPrefixRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsersByPrefix, request);
            }
            public Task<NullStruct> LookupUsersByPrefixAsync(LookupUsersByPrefixRequest request)
            {
                return Connection.SendRequestAsync<LookupUsersByPrefixRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsersByPrefix, request);
            }
            
            public NullStruct UpdateNetworkInfo(NetworkInfo request)
            {
                return Connection.SendRequest<NetworkInfo, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateNetworkInfo, request);
            }
            public Task<NullStruct> UpdateNetworkInfoAsync(NetworkInfo request)
            {
                return Connection.SendRequestAsync<NetworkInfo, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateNetworkInfo, request);
            }
            
            public NullStruct LookupUserGeoIPData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserGeoIPData, new NullStruct());
            }
            public Task<NullStruct> LookupUserGeoIPDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserGeoIPData, new NullStruct());
            }
            
            public NullStruct OverrideUserGeoIPData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.overrideUserGeoIPData, new NullStruct());
            }
            public Task<NullStruct> OverrideUserGeoIPDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.overrideUserGeoIPData, new NullStruct());
            }
            
            public NullStruct UpdateUserSessionClientData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateUserSessionClientData, new NullStruct());
            }
            public Task<NullStruct> UpdateUserSessionClientDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateUserSessionClientData, new NullStruct());
            }
            
            public NullStruct SetUserInfoAttribute()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.setUserInfoAttribute, new NullStruct());
            }
            public Task<NullStruct> SetUserInfoAttributeAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.setUserInfoAttribute, new NullStruct());
            }
            
            public NullStruct ResetUserGeoIPData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.resetUserGeoIPData, new NullStruct());
            }
            public Task<NullStruct> ResetUserGeoIPDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.resetUserGeoIPData, new NullStruct());
            }
            
            public NullStruct LookupUserSessionId()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserSessionId, new NullStruct());
            }
            public Task<NullStruct> LookupUserSessionIdAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserSessionId, new NullStruct());
            }
            
            public NullStruct FetchLastLocaleUsedAndAuthError()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchLastLocaleUsedAndAuthError, new NullStruct());
            }
            public Task<NullStruct> FetchLastLocaleUsedAndAuthErrorAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchLastLocaleUsedAndAuthError, new NullStruct());
            }
            
            public NullStruct FetchUserFirstLastAuthTime()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchUserFirstLastAuthTime, new NullStruct());
            }
            public Task<NullStruct> FetchUserFirstLastAuthTimeAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchUserFirstLastAuthTime, new NullStruct());
            }
            
            public NullStruct ResumeSession()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.resumeSession, new NullStruct());
            }
            public Task<NullStruct> ResumeSessionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.resumeSession, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)UserSessionsNotification.UserSessionExtendedDataUpdate)]
            public virtual Task OnUserSessionExtendedDataUpdateAsync(UserSessionExtendedDataUpdate notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnUserSessionExtendedDataUpdateAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserAdded)]
            public virtual Task OnUserAddedAsync(NotifyUserAdded notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnUserAddedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserRemoved)]
            public virtual Task OnUserRemovedAsync(NotifyUserRemoved notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnUserRemovedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserSessionDisconnected)]
            public virtual Task OnUserSessionDisconnectedAsync(UserSessionDisconnectReason notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnUserSessionDisconnectedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserUpdated)]
            public virtual Task OnUserUpdatedAsync(UserStatus notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnUserUpdatedAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(UserSessionsCommand command) => UserSessionsBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(UserSessionsCommand command) => UserSessionsBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(UserSessionsCommand command) => UserSessionsBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(UserSessionsNotification notification) => UserSessionsBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<UserSessionsCommand, UserSessionsNotification, Blaze3RpcError>
        {
            public Proxy() : base(UserSessionsBase.Id, UserSessionsBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.fetchExtendedData)]
            public virtual Task<NullStruct> FetchExtendedDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchExtendedData, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateExtendedDataAttribute)]
            public virtual Task<NullStruct> UpdateExtendedDataAttributeAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateExtendedDataAttribute, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateHardwareFlags)]
            public virtual Task<NullStruct> UpdateHardwareFlagsAsync(UpdateHardwareFlagsRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<UpdateHardwareFlagsRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateHardwareFlags, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUser)]
            public virtual Task<NullStruct> LookupUserAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUser, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUsers)]
            public virtual Task<UserDataResponse> LookupUsersAsync(LookupUsersRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<LookupUsersRequest, UserDataResponse, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsers, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUsersByPrefix)]
            public virtual Task<NullStruct> LookupUsersByPrefixAsync(LookupUsersByPrefixRequest request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<LookupUsersByPrefixRequest, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUsersByPrefix, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateNetworkInfo)]
            public virtual Task<NullStruct> UpdateNetworkInfoAsync(NetworkInfo request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NetworkInfo, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateNetworkInfo, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUserGeoIPData)]
            public virtual Task<NullStruct> LookupUserGeoIPDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserGeoIPData, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.overrideUserGeoIPData)]
            public virtual Task<NullStruct> OverrideUserGeoIPDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.overrideUserGeoIPData, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.updateUserSessionClientData)]
            public virtual Task<NullStruct> UpdateUserSessionClientDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.updateUserSessionClientData, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.setUserInfoAttribute)]
            public virtual Task<NullStruct> SetUserInfoAttributeAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.setUserInfoAttribute, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.resetUserGeoIPData)]
            public virtual Task<NullStruct> ResetUserGeoIPDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.resetUserGeoIPData, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.lookupUserSessionId)]
            public virtual Task<NullStruct> LookupUserSessionIdAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.lookupUserSessionId, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.fetchLastLocaleUsedAndAuthError)]
            public virtual Task<NullStruct> FetchLastLocaleUsedAndAuthErrorAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchLastLocaleUsedAndAuthError, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.fetchUserFirstLastAuthTime)]
            public virtual Task<NullStruct> FetchUserFirstLastAuthTimeAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.fetchUserFirstLastAuthTime, request);
            }
            
            [BlazeCommand((ushort)UserSessionsCommand.resumeSession)]
            public virtual Task<NullStruct> ResumeSessionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)UserSessionsCommand.resumeSession, request);
            }
            
            
            [BlazeNotification((ushort)UserSessionsNotification.UserSessionExtendedDataUpdate)]
            public virtual Task<UserSessionExtendedDataUpdate> OnUserSessionExtendedDataUpdateAsync(UserSessionExtendedDataUpdate notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserAdded)]
            public virtual Task<NotifyUserAdded> OnUserAddedAsync(NotifyUserAdded notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserRemoved)]
            public virtual Task<NotifyUserRemoved> OnUserRemovedAsync(NotifyUserRemoved notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserSessionDisconnected)]
            public virtual Task<UserSessionDisconnectReason> OnUserSessionDisconnectedAsync(UserSessionDisconnectReason notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)UserSessionsNotification.UserUpdated)]
            public virtual Task<UserStatus> OnUserUpdatedAsync(UserStatus notification)
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
            UserSessionsCommand.fetchExtendedData => typeof(NullStruct),
            UserSessionsCommand.updateExtendedDataAttribute => typeof(NullStruct),
            UserSessionsCommand.updateHardwareFlags => typeof(UpdateHardwareFlagsRequest),
            UserSessionsCommand.lookupUser => typeof(NullStruct),
            UserSessionsCommand.lookupUsers => typeof(LookupUsersRequest),
            UserSessionsCommand.lookupUsersByPrefix => typeof(LookupUsersByPrefixRequest),
            UserSessionsCommand.updateNetworkInfo => typeof(NetworkInfo),
            UserSessionsCommand.lookupUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.overrideUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.updateUserSessionClientData => typeof(NullStruct),
            UserSessionsCommand.setUserInfoAttribute => typeof(NullStruct),
            UserSessionsCommand.resetUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.lookupUserSessionId => typeof(NullStruct),
            UserSessionsCommand.fetchLastLocaleUsedAndAuthError => typeof(NullStruct),
            UserSessionsCommand.fetchUserFirstLastAuthTime => typeof(NullStruct),
            UserSessionsCommand.resumeSession => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(UserSessionsCommand command) => command switch
        {
            UserSessionsCommand.fetchExtendedData => typeof(NullStruct),
            UserSessionsCommand.updateExtendedDataAttribute => typeof(NullStruct),
            UserSessionsCommand.updateHardwareFlags => typeof(NullStruct),
            UserSessionsCommand.lookupUser => typeof(NullStruct),
            UserSessionsCommand.lookupUsers => typeof(UserDataResponse),
            UserSessionsCommand.lookupUsersByPrefix => typeof(NullStruct),
            UserSessionsCommand.updateNetworkInfo => typeof(NullStruct),
            UserSessionsCommand.lookupUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.overrideUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.updateUserSessionClientData => typeof(NullStruct),
            UserSessionsCommand.setUserInfoAttribute => typeof(NullStruct),
            UserSessionsCommand.resetUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.lookupUserSessionId => typeof(NullStruct),
            UserSessionsCommand.fetchLastLocaleUsedAndAuthError => typeof(NullStruct),
            UserSessionsCommand.fetchUserFirstLastAuthTime => typeof(NullStruct),
            UserSessionsCommand.resumeSession => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(UserSessionsCommand command) => command switch
        {
            UserSessionsCommand.fetchExtendedData => typeof(NullStruct),
            UserSessionsCommand.updateExtendedDataAttribute => typeof(NullStruct),
            UserSessionsCommand.updateHardwareFlags => typeof(NullStruct),
            UserSessionsCommand.lookupUser => typeof(NullStruct),
            UserSessionsCommand.lookupUsers => typeof(NullStruct),
            UserSessionsCommand.lookupUsersByPrefix => typeof(NullStruct),
            UserSessionsCommand.updateNetworkInfo => typeof(NullStruct),
            UserSessionsCommand.lookupUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.overrideUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.updateUserSessionClientData => typeof(NullStruct),
            UserSessionsCommand.setUserInfoAttribute => typeof(NullStruct),
            UserSessionsCommand.resetUserGeoIPData => typeof(NullStruct),
            UserSessionsCommand.lookupUserSessionId => typeof(NullStruct),
            UserSessionsCommand.fetchLastLocaleUsedAndAuthError => typeof(NullStruct),
            UserSessionsCommand.fetchUserFirstLastAuthTime => typeof(NullStruct),
            UserSessionsCommand.resumeSession => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(UserSessionsNotification notification) => notification switch
        {
            UserSessionsNotification.UserSessionExtendedDataUpdate => typeof(UserSessionExtendedDataUpdate),
            UserSessionsNotification.UserAdded => typeof(NotifyUserAdded),
            UserSessionsNotification.UserRemoved => typeof(NotifyUserRemoved),
            UserSessionsNotification.UserSessionDisconnected => typeof(UserSessionDisconnectReason),
            UserSessionsNotification.UserUpdated => typeof(UserStatus),
            _ => typeof(NullStruct)
        };
        
        public enum UserSessionsCommand : ushort
        {
            fetchExtendedData = 3,
            updateExtendedDataAttribute = 5,
            updateHardwareFlags = 8,
            lookupUser = 12,
            lookupUsers = 13,
            lookupUsersByPrefix = 14,
            updateNetworkInfo = 20,
            lookupUserGeoIPData = 23,
            overrideUserGeoIPData = 24,
            updateUserSessionClientData = 25,
            setUserInfoAttribute = 26,
            resetUserGeoIPData = 27,
            lookupUserSessionId = 32,
            fetchLastLocaleUsedAndAuthError = 33,
            fetchUserFirstLastAuthTime = 34,
            resumeSession = 35,
        }
        
        public enum UserSessionsNotification : ushort
        {
            UserSessionExtendedDataUpdate = 1,
            UserAdded = 2,
            UserRemoved = 3,
            UserSessionDisconnected = 4,
            UserUpdated = 5,
        }
        
    }
}
