using BlazeCommon;

namespace Blaze2SDK.Components
{
    public static class TournamentsComponentBase
    {
        public const ushort Id = 23;
        public const string Name = "TournamentsComponent";
        
        public class Server : BlazeServerComponent<TournamentsComponentCommand, TournamentsComponentNotification, Blaze2RpcError>
        {
            public Server() : base(TournamentsComponentBase.Id, TournamentsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.getTournaments)]
            public virtual Task<NullStruct> GetTournamentsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.getMemberCounts)]
            public virtual Task<NullStruct> GetMemberCountsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.getTrophies)]
            public virtual Task<NullStruct> GetTrophiesAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.getMyTournamentId)]
            public virtual Task<NullStruct> GetMyTournamentIdAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.joinTournament)]
            public virtual Task<NullStruct> JoinTournamentAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.leaveTournament)]
            public virtual Task<NullStruct> LeaveTournamentAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.resetMyTournament)]
            public virtual Task<NullStruct> ResetMyTournamentAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.getMyTournamentDetails)]
            public virtual Task<NullStruct> GetMyTournamentDetailsAsync(NullStruct request, BlazeRpcContext context)
            {
                throw new BlazeRpcException(Blaze2RpcError.ERR_COMMAND_NOT_FOUND);
            }
            
            
            public override Type GetCommandRequestType(TournamentsComponentCommand command) => TournamentsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(TournamentsComponentCommand command) => TournamentsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(TournamentsComponentCommand command) => TournamentsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(TournamentsComponentNotification notification) => TournamentsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Client : BlazeClientComponent<TournamentsComponentCommand, TournamentsComponentNotification, Blaze2RpcError>
        {
            BlazeClientConnection Connection { get; }
            
            public Client(BlazeClientConnection connection) : base(TournamentsComponentBase.Id, TournamentsComponentBase.Name)
            {
                Connection = connection;
                if (!Connection.Config.AddComponent(this))
                    throw new InvalidOperationException($"A component with Id({Id}) has already been created for the connection.");
            }
            
            
            public NullStruct GetTournaments()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getTournaments, new NullStruct());
            }
            public Task<NullStruct> GetTournamentsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getTournaments, new NullStruct());
            }
            
            public NullStruct GetMemberCounts()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getMemberCounts, new NullStruct());
            }
            public Task<NullStruct> GetMemberCountsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getMemberCounts, new NullStruct());
            }
            
            public NullStruct GetTrophies()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getTrophies, new NullStruct());
            }
            public Task<NullStruct> GetTrophiesAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getTrophies, new NullStruct());
            }
            
            public NullStruct GetMyTournamentId()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getMyTournamentId, new NullStruct());
            }
            public Task<NullStruct> GetMyTournamentIdAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getMyTournamentId, new NullStruct());
            }
            
            public NullStruct JoinTournament()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.joinTournament, new NullStruct());
            }
            public Task<NullStruct> JoinTournamentAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.joinTournament, new NullStruct());
            }
            
            public NullStruct LeaveTournament()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.leaveTournament, new NullStruct());
            }
            public Task<NullStruct> LeaveTournamentAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.leaveTournament, new NullStruct());
            }
            
            public NullStruct ResetMyTournament()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.resetMyTournament, new NullStruct());
            }
            public Task<NullStruct> ResetMyTournamentAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.resetMyTournament, new NullStruct());
            }
            
            public NullStruct GetMyTournamentDetails()
            {
                return Connection.SendRequest<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getMyTournamentDetails, new NullStruct());
            }
            public Task<NullStruct> GetMyTournamentDetailsAsync()
            {
                return Connection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getMyTournamentDetails, new NullStruct());
            }
            
            
            public override Type GetCommandRequestType(TournamentsComponentCommand command) => TournamentsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(TournamentsComponentCommand command) => TournamentsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(TournamentsComponentCommand command) => TournamentsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(TournamentsComponentNotification notification) => TournamentsComponentBase.GetNotificationType(notification);
            
        }
        
        public class Proxy : BlazeProxyComponent<TournamentsComponentCommand, TournamentsComponentNotification, Blaze2RpcError>
        {
            public Proxy() : base(TournamentsComponentBase.Id, TournamentsComponentBase.Name)
            {
                
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.getTournaments)]
            public virtual Task<NullStruct> GetTournamentsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getTournaments, request);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.getMemberCounts)]
            public virtual Task<NullStruct> GetMemberCountsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getMemberCounts, request);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.getTrophies)]
            public virtual Task<NullStruct> GetTrophiesAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getTrophies, request);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.getMyTournamentId)]
            public virtual Task<NullStruct> GetMyTournamentIdAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getMyTournamentId, request);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.joinTournament)]
            public virtual Task<NullStruct> JoinTournamentAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.joinTournament, request);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.leaveTournament)]
            public virtual Task<NullStruct> LeaveTournamentAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.leaveTournament, request);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.resetMyTournament)]
            public virtual Task<NullStruct> ResetMyTournamentAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.resetMyTournament, request);
            }
            
            [BlazeCommand((ushort)TournamentsComponentCommand.getMyTournamentDetails)]
            public virtual Task<NullStruct> GetMyTournamentDetailsAsync(NullStruct request, BlazeProxyContext context)
            {
                return context.ClientConnection.SendRequestAsync<NullStruct, NullStruct, NullStruct>(this, (ushort)TournamentsComponentCommand.getMyTournamentDetails, request);
            }
            
            
            public override Type GetCommandRequestType(TournamentsComponentCommand command) => TournamentsComponentBase.GetCommandRequestType(command);
            public override Type GetCommandResponseType(TournamentsComponentCommand command) => TournamentsComponentBase.GetCommandResponseType(command);
            public override Type GetCommandErrorResponseType(TournamentsComponentCommand command) => TournamentsComponentBase.GetCommandErrorResponseType(command);
            public override Type GetNotificationType(TournamentsComponentNotification notification) => TournamentsComponentBase.GetNotificationType(notification);
            
        }
        
        public static Type GetCommandRequestType(TournamentsComponentCommand command) => command switch
        {
            TournamentsComponentCommand.getTournaments => typeof(NullStruct),
            TournamentsComponentCommand.getMemberCounts => typeof(NullStruct),
            TournamentsComponentCommand.getTrophies => typeof(NullStruct),
            TournamentsComponentCommand.getMyTournamentId => typeof(NullStruct),
            TournamentsComponentCommand.joinTournament => typeof(NullStruct),
            TournamentsComponentCommand.leaveTournament => typeof(NullStruct),
            TournamentsComponentCommand.resetMyTournament => typeof(NullStruct),
            TournamentsComponentCommand.getMyTournamentDetails => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandResponseType(TournamentsComponentCommand command) => command switch
        {
            TournamentsComponentCommand.getTournaments => typeof(NullStruct),
            TournamentsComponentCommand.getMemberCounts => typeof(NullStruct),
            TournamentsComponentCommand.getTrophies => typeof(NullStruct),
            TournamentsComponentCommand.getMyTournamentId => typeof(NullStruct),
            TournamentsComponentCommand.joinTournament => typeof(NullStruct),
            TournamentsComponentCommand.leaveTournament => typeof(NullStruct),
            TournamentsComponentCommand.resetMyTournament => typeof(NullStruct),
            TournamentsComponentCommand.getMyTournamentDetails => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetCommandErrorResponseType(TournamentsComponentCommand command) => command switch
        {
            TournamentsComponentCommand.getTournaments => typeof(NullStruct),
            TournamentsComponentCommand.getMemberCounts => typeof(NullStruct),
            TournamentsComponentCommand.getTrophies => typeof(NullStruct),
            TournamentsComponentCommand.getMyTournamentId => typeof(NullStruct),
            TournamentsComponentCommand.joinTournament => typeof(NullStruct),
            TournamentsComponentCommand.leaveTournament => typeof(NullStruct),
            TournamentsComponentCommand.resetMyTournament => typeof(NullStruct),
            TournamentsComponentCommand.getMyTournamentDetails => typeof(NullStruct),
            _ => typeof(NullStruct)
        };
        
        public static Type GetNotificationType(TournamentsComponentNotification notification) => notification switch
        {
            _ => typeof(NullStruct)
        };
        
        public enum TournamentsComponentCommand : ushort
        {
            getTournaments = 1,
            getMemberCounts = 2,
            getTrophies = 4,
            getMyTournamentId = 5,
            joinTournament = 6,
            leaveTournament = 7,
            resetMyTournament = 8,
            getMyTournamentDetails = 9,
        }
        
        public enum TournamentsComponentNotification : ushort
        {
        }
        
    }
}
