using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class LockerComponentBase
    {
        public const ushort Id = 20;
        public const string Name = "LockerComponent";
        
        public class Server : BlazeServerComponent<LockerComponentCommand, LockerComponentNotification, Blaze2RpcError>
        {
            public Server() : base(LockerComponentBase.Id, LockerComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.createContent)]
            public virtual Task<NullStruct> CreateContentAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.createSubContent)]
            public virtual Task<NullStruct> CreateSubContentAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.confirmUpload)]
            public virtual Task<NullStruct> ConfirmUploadAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.updateContentInfo)]
            public virtual Task<NullStruct> UpdateContentInfoAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.deleteContent)]
            public virtual Task<NullStruct> DeleteContentAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.copyContentReference)]
            public virtual Task<NullStruct> CopyContentReferenceAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.bookmarkContentReference)]
            public virtual Task<NullStruct> BookmarkContentReferenceAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.getContentInfo)]
            public virtual Task<NullStruct> GetContentInfoAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.ListContent)]
            public virtual Task<NullStruct> ListContentAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.getTopN)]
            public virtual Task<NullStruct> GetTopNAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.getLeaderboardView)]
            public virtual Task<NullStruct> GetLeaderboardViewAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.updateRating)]
            public virtual Task<NullStruct> UpdateRatingAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.incrementUseCount)]
            public virtual Task<NullStruct> IncrementUseCountAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.setOwnerGroup)]
            public virtual Task<NullStruct> SetOwnerGroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.removeOwnerGroup)]
            public virtual Task<NullStruct> RemoveOwnerGroupAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(LockerComponentCommand command) => LockerComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(LockerComponentCommand command) => LockerComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(LockerComponentCommand command) => LockerComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(LockerComponentNotification notification) => LockerComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<LockerComponentCommand, LockerComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(LockerComponentBase.Id, LockerComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct CreateContent()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.createContent, new NullStruct());
            }
            public Task<NullStruct> CreateContentAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.createContent, new NullStruct());
            }
            
            public NullStruct CreateSubContent()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.createSubContent, new NullStruct());
            }
            public Task<NullStruct> CreateSubContentAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.createSubContent, new NullStruct());
            }
            
            public NullStruct ConfirmUpload()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.confirmUpload, new NullStruct());
            }
            public Task<NullStruct> ConfirmUploadAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.confirmUpload, new NullStruct());
            }
            
            public NullStruct UpdateContentInfo()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.updateContentInfo, new NullStruct());
            }
            public Task<NullStruct> UpdateContentInfoAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.updateContentInfo, new NullStruct());
            }
            
            public NullStruct DeleteContent()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.deleteContent, new NullStruct());
            }
            public Task<NullStruct> DeleteContentAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.deleteContent, new NullStruct());
            }
            
            public NullStruct CopyContentReference()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.copyContentReference, new NullStruct());
            }
            public Task<NullStruct> CopyContentReferenceAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.copyContentReference, new NullStruct());
            }
            
            public NullStruct BookmarkContentReference()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.bookmarkContentReference, new NullStruct());
            }
            public Task<NullStruct> BookmarkContentReferenceAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.bookmarkContentReference, new NullStruct());
            }
            
            public NullStruct GetContentInfo()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.getContentInfo, new NullStruct());
            }
            public Task<NullStruct> GetContentInfoAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.getContentInfo, new NullStruct());
            }
            
            public NullStruct ListContent()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.ListContent, new NullStruct());
            }
            public Task<NullStruct> ListContentAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.ListContent, new NullStruct());
            }
            
            public NullStruct GetTopN()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.getTopN, new NullStruct());
            }
            public Task<NullStruct> GetTopNAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.getTopN, new NullStruct());
            }
            
            public NullStruct GetLeaderboardView()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.getLeaderboardView, new NullStruct());
            }
            public Task<NullStruct> GetLeaderboardViewAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.getLeaderboardView, new NullStruct());
            }
            
            public NullStruct UpdateRating()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.updateRating, new NullStruct());
            }
            public Task<NullStruct> UpdateRatingAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.updateRating, new NullStruct());
            }
            
            public NullStruct IncrementUseCount()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.incrementUseCount, new NullStruct());
            }
            public Task<NullStruct> IncrementUseCountAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.incrementUseCount, new NullStruct());
            }
            
            public NullStruct SetOwnerGroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.setOwnerGroup, new NullStruct());
            }
            public Task<NullStruct> SetOwnerGroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.setOwnerGroup, new NullStruct());
            }
            
            public NullStruct RemoveOwnerGroup()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.removeOwnerGroup, new NullStruct());
            }
            public Task<NullStruct> RemoveOwnerGroupAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.removeOwnerGroup, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(LockerComponentCommand command) => LockerComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(LockerComponentCommand command) => LockerComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(LockerComponentCommand command) => LockerComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(LockerComponentNotification notification) => LockerComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<LockerComponentCommand, LockerComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(LockerComponentBase.Id, LockerComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.createContent)]
            public virtual Task<NullStruct> CreateContentAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.createContent, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.createSubContent)]
            public virtual Task<NullStruct> CreateSubContentAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.createSubContent, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.confirmUpload)]
            public virtual Task<NullStruct> ConfirmUploadAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.confirmUpload, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.updateContentInfo)]
            public virtual Task<NullStruct> UpdateContentInfoAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.updateContentInfo, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.deleteContent)]
            public virtual Task<NullStruct> DeleteContentAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.deleteContent, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.copyContentReference)]
            public virtual Task<NullStruct> CopyContentReferenceAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.copyContentReference, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.bookmarkContentReference)]
            public virtual Task<NullStruct> BookmarkContentReferenceAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.bookmarkContentReference, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.getContentInfo)]
            public virtual Task<NullStruct> GetContentInfoAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.getContentInfo, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.ListContent)]
            public virtual Task<NullStruct> ListContentAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.ListContent, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.getTopN)]
            public virtual Task<NullStruct> GetTopNAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.getTopN, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.getLeaderboardView)]
            public virtual Task<NullStruct> GetLeaderboardViewAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.getLeaderboardView, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.updateRating)]
            public virtual Task<NullStruct> UpdateRatingAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.updateRating, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.incrementUseCount)]
            public virtual Task<NullStruct> IncrementUseCountAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.incrementUseCount, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.setOwnerGroup)]
            public virtual Task<NullStruct> SetOwnerGroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.setOwnerGroup, request);
            }
            
            [BlazeCommand((ushort)LockerComponentCommand.removeOwnerGroup)]
            public virtual Task<NullStruct> RemoveOwnerGroupAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)LockerComponentCommand.removeOwnerGroup, request);
            }
            
            
            public override Type GetCommandRequestType(LockerComponentCommand command) => LockerComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(LockerComponentCommand command) => LockerComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(LockerComponentCommand command) => LockerComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(LockerComponentNotification notification) => LockerComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(LockerComponentCommand command) => command switch
        {
            LockerComponentCommand.createContent => typeof(NullStruct),
            LockerComponentCommand.createSubContent => typeof(NullStruct),
            LockerComponentCommand.confirmUpload => typeof(NullStruct),
            LockerComponentCommand.updateContentInfo => typeof(NullStruct),
            LockerComponentCommand.deleteContent => typeof(NullStruct),
            LockerComponentCommand.copyContentReference => typeof(NullStruct),
            LockerComponentCommand.bookmarkContentReference => typeof(NullStruct),
            LockerComponentCommand.getContentInfo => typeof(NullStruct),
            LockerComponentCommand.ListContent => typeof(NullStruct),
            LockerComponentCommand.getTopN => typeof(NullStruct),
            LockerComponentCommand.getLeaderboardView => typeof(NullStruct),
            LockerComponentCommand.updateRating => typeof(NullStruct),
            LockerComponentCommand.incrementUseCount => typeof(NullStruct),
            LockerComponentCommand.setOwnerGroup => typeof(NullStruct),
            LockerComponentCommand.removeOwnerGroup => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(LockerComponentCommand command) => command switch
        {
            LockerComponentCommand.createContent => typeof(NullStruct),
            LockerComponentCommand.createSubContent => typeof(NullStruct),
            LockerComponentCommand.confirmUpload => typeof(NullStruct),
            LockerComponentCommand.updateContentInfo => typeof(NullStruct),
            LockerComponentCommand.deleteContent => typeof(NullStruct),
            LockerComponentCommand.copyContentReference => typeof(NullStruct),
            LockerComponentCommand.bookmarkContentReference => typeof(NullStruct),
            LockerComponentCommand.getContentInfo => typeof(NullStruct),
            LockerComponentCommand.ListContent => typeof(NullStruct),
            LockerComponentCommand.getTopN => typeof(NullStruct),
            LockerComponentCommand.getLeaderboardView => typeof(NullStruct),
            LockerComponentCommand.updateRating => typeof(NullStruct),
            LockerComponentCommand.incrementUseCount => typeof(NullStruct),
            LockerComponentCommand.setOwnerGroup => typeof(NullStruct),
            LockerComponentCommand.removeOwnerGroup => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(LockerComponentCommand command) => command switch
        {
            LockerComponentCommand.createContent => typeof(NullStruct),
            LockerComponentCommand.createSubContent => typeof(NullStruct),
            LockerComponentCommand.confirmUpload => typeof(NullStruct),
            LockerComponentCommand.updateContentInfo => typeof(NullStruct),
            LockerComponentCommand.deleteContent => typeof(NullStruct),
            LockerComponentCommand.copyContentReference => typeof(NullStruct),
            LockerComponentCommand.bookmarkContentReference => typeof(NullStruct),
            LockerComponentCommand.getContentInfo => typeof(NullStruct),
            LockerComponentCommand.ListContent => typeof(NullStruct),
            LockerComponentCommand.getTopN => typeof(NullStruct),
            LockerComponentCommand.getLeaderboardView => typeof(NullStruct),
            LockerComponentCommand.updateRating => typeof(NullStruct),
            LockerComponentCommand.incrementUseCount => typeof(NullStruct),
            LockerComponentCommand.setOwnerGroup => typeof(NullStruct),
            LockerComponentCommand.removeOwnerGroup => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(LockerComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum LockerComponentCommand : ushort
        {
            createContent = 1,
            createSubContent = 2,
            confirmUpload = 3,
            updateContentInfo = 4,
            deleteContent = 5,
            copyContentReference = 6,
            bookmarkContentReference = 7,
            getContentInfo = 8,
            ListContent = 9,
            getTopN = 10,
            getLeaderboardView = 11,
            updateRating = 12,
            incrementUseCount = 13,
            setOwnerGroup = 14,
            removeOwnerGroup = 15,
        }
        
        public enum LockerComponentNotification : ushort
        {
        }
        
    }
}
