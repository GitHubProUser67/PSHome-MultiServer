using Blaze3SDK.Blaze.CensusData;
using BlazeCommon;

namespace Blaze3SDK.Components
{
    public static class CensusDataComponentBase
    {
        public const ushort Id = 10;
        public const string Name = "CensusDataComponent";
        
        public class Server : BlazeServerComponent<CensusDataComponentCommand, CensusDataComponentNotification, Blaze3RpcError>
        {
            public Server() : base(CensusDataComponentBase.Id, CensusDataComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)CensusDataComponentCommand.subscribeToCensusData)]
            public virtual Task<NullStruct> SubscribeToCensusDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)CensusDataComponentCommand.unsubscribeFromCensusData)]
            public virtual Task<NullStruct> UnsubscribeFromCensusDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)CensusDataComponentCommand.getRegionCounts)]
            public virtual Task<NullStruct> GetRegionCountsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)CensusDataComponentCommand.getLatestCensusData)]
            public virtual Task<NullStruct> GetLatestCensusDataAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze3RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public static Task NotifyServerCensusDataAsync(BlazeServerConnection connection, NotifyServerCensusData notification, bool waitUntilFree = false)
            {
                return connection.NotifyAsync(CensusDataComponentBase.Id, (ushort)CensusDataComponentNotification.NotifyServerCensusData, notification, waitUntilFree);
            }
            
            public override Type GetCommandRequestType(CensusDataComponentCommand command) => CensusDataComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(CensusDataComponentCommand command) => CensusDataComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(CensusDataComponentCommand command) => CensusDataComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(CensusDataComponentNotification notification) => CensusDataComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<CensusDataComponentCommand, CensusDataComponentNotification, Blaze3RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(CensusDataComponentBase.Id, CensusDataComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct SubscribeToCensusData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.subscribeToCensusData, new NullStruct());
            }
            public Task<NullStruct> SubscribeToCensusDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.subscribeToCensusData, new NullStruct());
            }
            
            public NullStruct UnsubscribeFromCensusData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.unsubscribeFromCensusData, new NullStruct());
            }
            public Task<NullStruct> UnsubscribeFromCensusDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.unsubscribeFromCensusData, new NullStruct());
            }
            
            public NullStruct GetRegionCounts()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.getRegionCounts, new NullStruct());
            }
            public Task<NullStruct> GetRegionCountsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.getRegionCounts, new NullStruct());
            }
            
            public NullStruct GetLatestCensusData()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.getLatestCensusData, new NullStruct());
            }
            public Task<NullStruct> GetLatestCensusDataAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.getLatestCensusData, new NullStruct());
            }
            
            
            [BlazeNotification((ushort)CensusDataComponentNotification.NotifyServerCensusData)]
            public virtual Task OnNotifyServerCensusDataAsync(NotifyServerCensusData notification)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Blaze3SDK] - {GetType().FullName}: OnNotifyServerCensusDataAsync NOT IMPLEMENTED!");
                return Task.CompletedTask;
            }
            
            public override Type GetCommandRequestType(CensusDataComponentCommand command) => CensusDataComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(CensusDataComponentCommand command) => CensusDataComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(CensusDataComponentCommand command) => CensusDataComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(CensusDataComponentNotification notification) => CensusDataComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<CensusDataComponentCommand, CensusDataComponentNotification, Blaze3RpcError>
        {
            public Proxy() : base(CensusDataComponentBase.Id, CensusDataComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)CensusDataComponentCommand.subscribeToCensusData)]
            public virtual Task<NullStruct> SubscribeToCensusDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.subscribeToCensusData, request);
            }
            
            [BlazeCommand((ushort)CensusDataComponentCommand.unsubscribeFromCensusData)]
            public virtual Task<NullStruct> UnsubscribeFromCensusDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.unsubscribeFromCensusData, request);
            }
            
            [BlazeCommand((ushort)CensusDataComponentCommand.getRegionCounts)]
            public virtual Task<NullStruct> GetRegionCountsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.getRegionCounts, request);
            }
            
            [BlazeCommand((ushort)CensusDataComponentCommand.getLatestCensusData)]
            public virtual Task<NullStruct> GetLatestCensusDataAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)CensusDataComponentCommand.getLatestCensusData, request);
            }
            
            
            [BlazeNotification((ushort)CensusDataComponentNotification.NotifyServerCensusData)]
            public virtual Task<NotifyServerCensusData> OnNotifyServerCensusDataAsync(NotifyServerCensusData notification)
            {
                return Task.FromResult(notification);
            }
            
            public override Type GetCommandRequestType(CensusDataComponentCommand command) => CensusDataComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(CensusDataComponentCommand command) => CensusDataComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(CensusDataComponentCommand command) => CensusDataComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(CensusDataComponentNotification notification) => CensusDataComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(CensusDataComponentCommand command) => command switch
        {
            CensusDataComponentCommand.subscribeToCensusData => typeof(NullStruct),
            CensusDataComponentCommand.unsubscribeFromCensusData => typeof(NullStruct),
            CensusDataComponentCommand.getRegionCounts => typeof(NullStruct),
            CensusDataComponentCommand.getLatestCensusData => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(CensusDataComponentCommand command) => command switch
        {
            CensusDataComponentCommand.subscribeToCensusData => typeof(NullStruct),
            CensusDataComponentCommand.unsubscribeFromCensusData => typeof(NullStruct),
            CensusDataComponentCommand.getRegionCounts => typeof(NullStruct),
            CensusDataComponentCommand.getLatestCensusData => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(CensusDataComponentCommand command) => command switch
        {
            CensusDataComponentCommand.subscribeToCensusData => typeof(NullStruct),
            CensusDataComponentCommand.unsubscribeFromCensusData => typeof(NullStruct),
            CensusDataComponentCommand.getRegionCounts => typeof(NullStruct),
            CensusDataComponentCommand.getLatestCensusData => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(CensusDataComponentNotification notification) => notification switch
        {
            CensusDataComponentNotification.NotifyServerCensusData => typeof(NotifyServerCensusData),
            _ => typeof(NullStruct)
        };
        
        public enum CensusDataComponentCommand : ushort
        {
            subscribeToCensusData = 1,
            unsubscribeFromCensusData = 2,
            getRegionCounts = 3,
            getLatestCensusData = 4,
        }
        
        public enum CensusDataComponentNotification : ushort
        {
            NotifyServerCensusData = 1,
        }
        
    }
}
