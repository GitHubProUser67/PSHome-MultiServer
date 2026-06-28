using System.Collections.Concurrent;
using CustomLogger;

namespace BlazeCommon
{
    public class MitmBlazeServer(BlazeServerConfiguration settings, uint addressEncryptionKey)
        : MitmProtoFireServer(settings, addressEncryptionKey)
    {
        public BlazeServerConfiguration Configuration { get; } = settings;

        private readonly ConcurrentDictionary<
            ProtoFireConnection,
            BlazeServerConnection
        > _connections = new();

        public bool AddComponent<TComponent>()
            where TComponent : IBlazeServerComponent, new()
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
            return _connections.GetOrAdd(
                connection,
                (c) =>
                {
                    return new BlazeServerConnection(c, Configuration);
                }
            );
        }

        public override Task OnProtoFireConnectAsync(ProtoFireConnection connection)
        {
            Configuration.OnNewConnection?.Invoke(GetBlazeConnection(connection));
            return Task.CompletedTask;
        }

        public override Task OnProtoFireDisconnectAsync(ProtoFireConnection connection)
        {
            if (_connections.TryRemove(connection, out var connectionInfo))
                Configuration.OnDisconnected?.Invoke(connectionInfo);
            return Task.CompletedTask;
        }

        public override Task OnProtoFireErrorAsync(
            ProtoFireConnection connection,
            Exception exception
        )
        {
            OnProtoFireError(connection, exception);
            return Task.CompletedTask;
        }

        private void OnProtoFireError(ProtoFireConnection connection, Exception exception)
        {
            LoggerAccessor.LogError(
                $"[BlazeServer] - ProtoFireError occured (Exception: {exception})"
            );
            Configuration.OnError?.Invoke(GetBlazeConnection(connection), exception);
        }
    }
}
