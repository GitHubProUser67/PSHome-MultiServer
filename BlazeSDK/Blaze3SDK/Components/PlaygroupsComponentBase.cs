using Blaze3SDK.Blaze.Playgroups;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class PlaygroupsComponentBase
    {
        public const ushort Id = 6;
        public const string Name = "PlaygroupsComponent";
        
        public class Server : BlazeServerComponent<PlaygroupsComponentCommand, PlaygroupsComponentNotification, Blaze3RpcError>
        {
            public Server() : base(PlaygroupsComponentBase.Id, PlaygroupsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.createPlaygroup)]
            public virtual Task<NullStruct> CreatePlaygroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.destroyPlaygroup)]
            public virtual Task<NullStruct> DestroyPlaygroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.joinPlaygroup)]
            public virtual Task<NullStruct> JoinPlaygroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.leavePlaygroup)]
            public virtual Task<NullStruct> LeavePlaygroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.setPlaygroupAttributes)]
            public virtual Task<NullStruct> SetPlaygroupAttributesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.setMemberAttributes)]
            public virtual Task<NullStruct> SetMemberAttributesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.kickPlaygroupMember)]
            public virtual Task<NullStruct> KickPlaygroupMemberAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.setPlaygroupJoinControls)]
            public virtual Task<NullStruct> SetPlaygroupJoinControlsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.finalizePlaygroupCreation)]
            public virtual Task<NullStruct> FinalizePlaygroupCreationAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.lookupPlaygroupInfo)]
            public virtual Task<NullStruct> LookupPlaygroupInfoAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.resetPlaygroupSession)]
            public virtual Task<NullStruct> ResetPlaygroupSessionAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyDestroyPlaygroupAsync(BlazeServerConnection connection, NotifyDestroyPlaygroup notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyDestroyPlaygroup, notification, waitUntilFree);
            }
            
            public static Task NotifyJoinPlaygroupAsync(BlazeServerConnection connection, NotifyJoinPlaygroup notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyJoinPlaygroup, notification, waitUntilFree);
            }
            
            public static Task NotifyMemberJoinedPlaygroupAsync(BlazeServerConnection connection, NotifyMemberJoinedPlaygroup notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyMemberJoinedPlaygroup, notification, waitUntilFree);
            }
            
            public static Task NotifyMemberRemovedFromPlaygroupAsync(BlazeServerConnection connection, NotifyMemberRemoveFromPlaygroup notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyMemberRemovedFromPlaygroup, notification, waitUntilFree);
            }
            
            public static Task NotifyPlaygroupAttributesSetAsync(BlazeServerConnection connection, NotifyPlaygroupAttributesSet notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyPlaygroupAttributesSet, notification, waitUntilFree);
            }
            
            public static Task NotifyMemberAttributesSetAsync(BlazeServerConnection connection, NotifyMemberAttributesSet notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyMemberAttributesSet, notification, waitUntilFree);
            }
            
            public static Task NotifyLeaderChangeAsync(BlazeServerConnection connection, NotifyLeaderChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyLeaderChange, notification, waitUntilFree);
            }
            
            public static Task NotifyMemberPermissionsChangeAsync(BlazeServerConnection connection, NotifyMemberPermissionsChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyMemberPermissionsChange, notification, waitUntilFree);
            }
            
            public static Task NotifyJoinControlsChangeAsync(BlazeServerConnection connection, NotifyJoinControlsChange notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyJoinControlsChange, notification, waitUntilFree);
            }
            
            public static Task NotifyXboxSessionInfoAsync(BlazeServerConnection connection, NotifyXboxSessionInfo notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyXboxSessionInfo, notification, waitUntilFree);
            }
            
            public static Task NotifyXboxSessionChangeAsync(BlazeServerConnection connection, NotifyXboxSessionInfo notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(PlaygroupsComponentBase.Id, (ushort)PlaygroupsComponentNotification.NotifyXboxSessionChange, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(PlaygroupsComponentCommand command) => PlaygroupsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(PlaygroupsComponentCommand command) => PlaygroupsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(PlaygroupsComponentCommand command) => PlaygroupsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(PlaygroupsComponentNotification notification) => PlaygroupsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<PlaygroupsComponentCommand, PlaygroupsComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(PlaygroupsComponentBase.Id, PlaygroupsComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct CreatePlaygroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.createPlaygroup, new NullStruct());
            }
            public Task<NullStruct> CreatePlaygroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.createPlaygroup, new NullStruct());
            }
            
            public NullStruct DestroyPlaygroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.destroyPlaygroup, new NullStruct());
            }
            public Task<NullStruct> DestroyPlaygroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.destroyPlaygroup, new NullStruct());
            }
            
            public NullStruct JoinPlaygroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.joinPlaygroup, new NullStruct());
            }
            public Task<NullStruct> JoinPlaygroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.joinPlaygroup, new NullStruct());
            }
            
            public NullStruct LeavePlaygroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.leavePlaygroup, new NullStruct());
            }
            public Task<NullStruct> LeavePlaygroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.leavePlaygroup, new NullStruct());
            }
            
            public NullStruct SetPlaygroupAttributes()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.setPlaygroupAttributes, new NullStruct());
            }
            public Task<NullStruct> SetPlaygroupAttributesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.setPlaygroupAttributes, new NullStruct());
            }
            
            public NullStruct SetMemberAttributes()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.setMemberAttributes, new NullStruct());
            }
            public Task<NullStruct> SetMemberAttributesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.setMemberAttributes, new NullStruct());
            }
            
            public NullStruct KickPlaygroupMember()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.kickPlaygroupMember, new NullStruct());
            }
            public Task<NullStruct> KickPlaygroupMemberAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.kickPlaygroupMember, new NullStruct());
            }
            
            public NullStruct SetPlaygroupJoinControls()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.setPlaygroupJoinControls, new NullStruct());
            }
            public Task<NullStruct> SetPlaygroupJoinControlsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.setPlaygroupJoinControls, new NullStruct());
            }
            
            public NullStruct FinalizePlaygroupCreation()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.finalizePlaygroupCreation, new NullStruct());
            }
            public Task<NullStruct> FinalizePlaygroupCreationAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.finalizePlaygroupCreation, new NullStruct());
            }
            
            public NullStruct LookupPlaygroupInfo()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.lookupPlaygroupInfo, new NullStruct());
            }
            public Task<NullStruct> LookupPlaygroupInfoAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.lookupPlaygroupInfo, new NullStruct());
            }
            
            public NullStruct ResetPlaygroupSession()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.resetPlaygroupSession, new NullStruct());
            }
            public Task<NullStruct> ResetPlaygroupSessionAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.resetPlaygroupSession, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyDestroyPlaygroup)]
            public virtual Task OnNotifyDestroyPlaygroupAsync(NotifyDestroyPlaygroup notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyDestroyPlaygroupAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyJoinPlaygroup)]
            public virtual Task OnNotifyJoinPlaygroupAsync(NotifyJoinPlaygroup notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyJoinPlaygroupAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyMemberJoinedPlaygroup)]
            public virtual Task OnNotifyMemberJoinedPlaygroupAsync(NotifyMemberJoinedPlaygroup notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyMemberJoinedPlaygroupAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyMemberRemovedFromPlaygroup)]
            public virtual Task OnNotifyMemberRemovedFromPlaygroupAsync(NotifyMemberRemoveFromPlaygroup notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyMemberRemovedFromPlaygroupAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyPlaygroupAttributesSet)]
            public virtual Task OnNotifyPlaygroupAttributesSetAsync(NotifyPlaygroupAttributesSet notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyPlaygroupAttributesSetAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyMemberAttributesSet)]
            public virtual Task OnNotifyMemberAttributesSetAsync(NotifyMemberAttributesSet notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyMemberAttributesSetAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyLeaderChange)]
            public virtual Task OnNotifyLeaderChangeAsync(NotifyLeaderChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyLeaderChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyMemberPermissionsChange)]
            public virtual Task OnNotifyMemberPermissionsChangeAsync(NotifyMemberPermissionsChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyMemberPermissionsChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyJoinControlsChange)]
            public virtual Task OnNotifyJoinControlsChangeAsync(NotifyJoinControlsChange notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyJoinControlsChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyXboxSessionInfo)]
            public virtual Task OnNotifyXboxSessionInfoAsync(NotifyXboxSessionInfo notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyXboxSessionInfoAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyXboxSessionChange)]
            public virtual Task OnNotifyXboxSessionChangeAsync(NotifyXboxSessionInfo notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyXboxSessionChangeAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(PlaygroupsComponentCommand command) => PlaygroupsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(PlaygroupsComponentCommand command) => PlaygroupsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(PlaygroupsComponentCommand command) => PlaygroupsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(PlaygroupsComponentNotification notification) => PlaygroupsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<PlaygroupsComponentCommand, PlaygroupsComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(PlaygroupsComponentBase.Id, PlaygroupsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.createPlaygroup)]
            public virtual Task<NullStruct> CreatePlaygroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.createPlaygroup, request);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.destroyPlaygroup)]
            public virtual Task<NullStruct> DestroyPlaygroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.destroyPlaygroup, request);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.joinPlaygroup)]
            public virtual Task<NullStruct> JoinPlaygroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.joinPlaygroup, request);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.leavePlaygroup)]
            public virtual Task<NullStruct> LeavePlaygroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.leavePlaygroup, request);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.setPlaygroupAttributes)]
            public virtual Task<NullStruct> SetPlaygroupAttributesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.setPlaygroupAttributes, request);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.setMemberAttributes)]
            public virtual Task<NullStruct> SetMemberAttributesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.setMemberAttributes, request);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.kickPlaygroupMember)]
            public virtual Task<NullStruct> KickPlaygroupMemberAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.kickPlaygroupMember, request);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.setPlaygroupJoinControls)]
            public virtual Task<NullStruct> SetPlaygroupJoinControlsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.setPlaygroupJoinControls, request);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.finalizePlaygroupCreation)]
            public virtual Task<NullStruct> FinalizePlaygroupCreationAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.finalizePlaygroupCreation, request);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.lookupPlaygroupInfo)]
            public virtual Task<NullStruct> LookupPlaygroupInfoAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.lookupPlaygroupInfo, request);
            }
            
            [BlazeCommand((ushort)PlaygroupsComponentCommand.resetPlaygroupSession)]
            public virtual Task<NullStruct> ResetPlaygroupSessionAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)PlaygroupsComponentCommand.resetPlaygroupSession, request);
            }
            
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyDestroyPlaygroup)]
            public virtual Task<NotifyDestroyPlaygroup> OnNotifyDestroyPlaygroupAsync(NotifyDestroyPlaygroup notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyJoinPlaygroup)]
            public virtual Task<NotifyJoinPlaygroup> OnNotifyJoinPlaygroupAsync(NotifyJoinPlaygroup notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyMemberJoinedPlaygroup)]
            public virtual Task<NotifyMemberJoinedPlaygroup> OnNotifyMemberJoinedPlaygroupAsync(NotifyMemberJoinedPlaygroup notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyMemberRemovedFromPlaygroup)]
            public virtual Task<NotifyMemberRemoveFromPlaygroup> OnNotifyMemberRemovedFromPlaygroupAsync(NotifyMemberRemoveFromPlaygroup notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyPlaygroupAttributesSet)]
            public virtual Task<NotifyPlaygroupAttributesSet> OnNotifyPlaygroupAttributesSetAsync(NotifyPlaygroupAttributesSet notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyMemberAttributesSet)]
            public virtual Task<NotifyMemberAttributesSet> OnNotifyMemberAttributesSetAsync(NotifyMemberAttributesSet notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyLeaderChange)]
            public virtual Task<NotifyLeaderChange> OnNotifyLeaderChangeAsync(NotifyLeaderChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyMemberPermissionsChange)]
            public virtual Task<NotifyMemberPermissionsChange> OnNotifyMemberPermissionsChangeAsync(NotifyMemberPermissionsChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyJoinControlsChange)]
            public virtual Task<NotifyJoinControlsChange> OnNotifyJoinControlsChangeAsync(NotifyJoinControlsChange notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyXboxSessionInfo)]
            public virtual Task<NotifyXboxSessionInfo> OnNotifyXboxSessionInfoAsync(NotifyXboxSessionInfo notification)
            {
                return Task.FromResult(notification);
            }
            
            [BlazeNotification((ushort)PlaygroupsComponentNotification.NotifyXboxSessionChange)]
            public virtual Task<NotifyXboxSessionInfo> OnNotifyXboxSessionChangeAsync(NotifyXboxSessionInfo notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(PlaygroupsComponentCommand command) => PlaygroupsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(PlaygroupsComponentCommand command) => PlaygroupsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(PlaygroupsComponentCommand command) => PlaygroupsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(PlaygroupsComponentNotification notification) => PlaygroupsComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(PlaygroupsComponentCommand command) => command switch
        {
            PlaygroupsComponentCommand.createPlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.destroyPlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.joinPlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.leavePlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.setPlaygroupAttributes => typeof(NullStruct),
            PlaygroupsComponentCommand.setMemberAttributes => typeof(NullStruct),
            PlaygroupsComponentCommand.kickPlaygroupMember => typeof(NullStruct),
            PlaygroupsComponentCommand.setPlaygroupJoinControls => typeof(NullStruct),
            PlaygroupsComponentCommand.finalizePlaygroupCreation => typeof(NullStruct),
            PlaygroupsComponentCommand.lookupPlaygroupInfo => typeof(NullStruct),
            PlaygroupsComponentCommand.resetPlaygroupSession => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(PlaygroupsComponentCommand command) => command switch
        {
            PlaygroupsComponentCommand.createPlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.destroyPlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.joinPlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.leavePlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.setPlaygroupAttributes => typeof(NullStruct),
            PlaygroupsComponentCommand.setMemberAttributes => typeof(NullStruct),
            PlaygroupsComponentCommand.kickPlaygroupMember => typeof(NullStruct),
            PlaygroupsComponentCommand.setPlaygroupJoinControls => typeof(NullStruct),
            PlaygroupsComponentCommand.finalizePlaygroupCreation => typeof(NullStruct),
            PlaygroupsComponentCommand.lookupPlaygroupInfo => typeof(NullStruct),
            PlaygroupsComponentCommand.resetPlaygroupSession => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(PlaygroupsComponentCommand command) => command switch
        {
            PlaygroupsComponentCommand.createPlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.destroyPlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.joinPlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.leavePlaygroup => typeof(NullStruct),
            PlaygroupsComponentCommand.setPlaygroupAttributes => typeof(NullStruct),
            PlaygroupsComponentCommand.setMemberAttributes => typeof(NullStruct),
            PlaygroupsComponentCommand.kickPlaygroupMember => typeof(NullStruct),
            PlaygroupsComponentCommand.setPlaygroupJoinControls => typeof(NullStruct),
            PlaygroupsComponentCommand.finalizePlaygroupCreation => typeof(NullStruct),
            PlaygroupsComponentCommand.lookupPlaygroupInfo => typeof(NullStruct),
            PlaygroupsComponentCommand.resetPlaygroupSession => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(PlaygroupsComponentNotification notification) => notification switch
        {
            PlaygroupsComponentNotification.NotifyDestroyPlaygroup => typeof(NotifyDestroyPlaygroup),
            PlaygroupsComponentNotification.NotifyJoinPlaygroup => typeof(NotifyJoinPlaygroup),
            PlaygroupsComponentNotification.NotifyMemberJoinedPlaygroup => typeof(NotifyMemberJoinedPlaygroup),
            PlaygroupsComponentNotification.NotifyMemberRemovedFromPlaygroup => typeof(NotifyMemberRemoveFromPlaygroup),
            PlaygroupsComponentNotification.NotifyPlaygroupAttributesSet => typeof(NotifyPlaygroupAttributesSet),
            PlaygroupsComponentNotification.NotifyMemberAttributesSet => typeof(NotifyMemberAttributesSet),
            PlaygroupsComponentNotification.NotifyLeaderChange => typeof(NotifyLeaderChange),
            PlaygroupsComponentNotification.NotifyMemberPermissionsChange => typeof(NotifyMemberPermissionsChange),
            PlaygroupsComponentNotification.NotifyJoinControlsChange => typeof(NotifyJoinControlsChange),
            PlaygroupsComponentNotification.NotifyXboxSessionInfo => typeof(NotifyXboxSessionInfo),
            PlaygroupsComponentNotification.NotifyXboxSessionChange => typeof(NotifyXboxSessionInfo),
            _ => typeof(NullStruct)
        };
        
        public enum PlaygroupsComponentCommand : ushort
        {
            createPlaygroup = 1,
            destroyPlaygroup = 2,
            joinPlaygroup = 3,
            leavePlaygroup = 4,
            setPlaygroupAttributes = 5,
            setMemberAttributes = 6,
            kickPlaygroupMember = 7,
            setPlaygroupJoinControls = 8,
            finalizePlaygroupCreation = 9,
            lookupPlaygroupInfo = 10,
            resetPlaygroupSession = 11,
        }
        
        public enum PlaygroupsComponentNotification : ushort
        {
            NotifyDestroyPlaygroup = 50,
            NotifyJoinPlaygroup = 51,
            NotifyMemberJoinedPlaygroup = 52,
            NotifyMemberRemovedFromPlaygroup = 53,
            NotifyPlaygroupAttributesSet = 54,
            NotifyMemberAttributesSet = 75,
            NotifyLeaderChange = 79,
            NotifyMemberPermissionsChange = 80,
            NotifyJoinControlsChange = 85,
            NotifyXboxSessionInfo = 86,
            NotifyXboxSessionChange = 87,
        }
        
    }
}
