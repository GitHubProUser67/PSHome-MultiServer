using CustomLogger;
using System.Collections.Concurrent;

namespace BlazeCommon
{
    public class MitmBlazeServer : MitmProtoFireServer
    {
        public BlazeServerConfiguration Configuration { get; }

        private ConcurrentDictionary<ProtoFireConnection, BlazeServerConnection> _connections;

        public MitmBlazeServer(BlazeServerConfiguration settings, uint addressEncryptionKey) : base(settings, addressEncryptionKey)
        {
            Configuration = settings;

            _connections = new ConcurrentDictionary<ProtoFireConnection, BlazeServerConnection>();
        }

        public bool AddComponent<TComponent>() where TComponent : IBlazeServerComponent, new()
        {
            return Configuration.AddComponent<TComponent>();
        }

        public bool RemoveComponent(ushort componentId, out IBlazeServerComponent? component)
        {
            return Configuration.RemoveComponent(componentId, out component);
        }

        public IBlazeServerComponent? GetComponent(ushort componentId)
        {
            return Configuration.GetComponent(componentId);
        }

        BlazeServerConnection GetBlazeConnection(ProtoFireConnection connection)
        {
            return _connections.GetOrAdd(connection, (c) =>
            {
                return new BlazeServerConnection(c, Configuration);
            });
        }

        public override Task OnProtoFireConnectAsync(ProtoFireConnection connection)
        {
            Configuration.OnNewConnection?.Invoke(GetBlazeConnection(connection));
            return Task.CompletedTask;
        }

        public override Task OnProtoFireDisconnectAsync(ProtoFireConnection connection)
        {
            if (_connections.TryRemove(connection, out BlazeServerConnection? connectionInfo))
                Configuration.OnDisconnected?.Invoke(connectionInfo);
            return Task.CompletedTask;
        }

        public override Task OnProtoFireErrorAsync(ProtoFireConnection connection, Exception exception)
        {
            OnProtoFireError(connection, exception);
            return Task.CompletedTask;
        }

        private void OnProtoFireError(ProtoFireConnection connection, Exception exception)
        {
            LoggerAccessor.LogError($"[BlazeServer] - ProtoFireError occured (Exception: {exception})");
            Configuration.OnError?.Invoke(GetBlazeConnection(connection), exception);
        }
    }
}
