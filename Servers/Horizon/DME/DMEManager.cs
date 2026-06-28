using System.Collections.Concurrent;
using CustomLogger;
using Horizon.DME.Models;
using Horizon.PluginManager;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.NET;

namespace Horizon.DME
{
    public class DMEManager
    {
        private readonly DMEProcessor _processor = new();

        public Dictionary<int, MPSClient> MPSManagersQueue { get; } = new();

        private readonly ConcurrentList<int> _MASReconnectQueue = new();
        private readonly ConcurrentDictionary<int, MPSClient> _MPSManagers = new();
        private readonly ConcurrentDictionary<int, MASClient> _MASManagers = new();

        public MediusPluginsManager Plugins = new(HorizonServerConfiguration.DmePluginsFolder);

        private DateTime _timeLastPluginTick = DateTimeUtils.GetHighPrecisionUtcTime();

        public DMEManager(DMEProcessor processor)
        {
            _processor = processor;
        }

        private async Task TickAsync()
        {
            try
            {
                lock (MPSManagersQueue)
                {
                    // Copy the contents of MPSManagersQueue to MPSManagers
                    foreach (var kvp in MPSManagersQueue)
                    {
                        var keyIdent = kvp.Key;

                        if (!_MPSManagers.ContainsKey(keyIdent))
                        {
                            _MPSManagers[keyIdent] = kvp.Value;
                            _ = _MPSManagers[keyIdent].Start();
                            _MASReconnectQueue.Remove(keyIdent);
                        }
                    }

                    // Clear MPSManagersQueue after copying
                    MPSManagersQueue.Clear();
                }

                // connect/reconnect to MAS
                foreach (var manager in _MASManagers.Values)
                    if (manager != null && !manager.IsConnected && !manager.IsAuthenticated)
                        await manager.Start().ConfigureAwait(false);

                await HandleInMessages().ConfigureAwait(false);

                // Tick plugins
                if (
                    (
                        DateTimeUtils.GetHighPrecisionUtcTime() - _timeLastPluginTick
                    ).TotalMilliseconds > HorizonServerConfiguration.DMEPluginTickIntervalMs
                )
                {
                    _timeLastPluginTick = DateTimeUtils.GetHighPrecisionUtcTime();
                    await Plugins.Tick().ConfigureAwait(false);
                }

                await HandleOutMessages().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[DMEManager] - An assertion was thrown while ticking the server. (Exception:{ex})"
                );
            }
        }

        private async Task HandleInMessages()
        {
            // handle incoming
            List<Task> InRequestsTasks = new() { _processor.HandleIncomingMessages() };
            foreach (var manager in _MASManagers.Values)
            {
                if (manager.IsConnected && manager.CheckMASConnectivity())
                    InRequestsTasks.Add(manager.HandleIncomingMessages());
            }
            foreach (var manager in _MPSManagers.Values)
            {
                if (manager.IsConnected && manager.CheckMPSConnectivity())
                    InRequestsTasks.Add(manager.HandleIncomingMessages());
            }

            await Task.WhenAll(InRequestsTasks).ConfigureAwait(false);
        }

        private async Task HandleOutMessages()
        {
            // handle outgoing
            List<Task> OutRequestsTasks = new() { _processor.HandleOutgoingMessages() };
            foreach (var manager in _MASManagers.Values)
            {
                if (manager.IsConnected)
                {
                    if (manager.CheckMASConnectivity())
                        OutRequestsTasks.Add(manager.HandleOutgoingMessages());
                }
                else if (
                    !manager.IsAuthenticated
                    && (
                        DateTimeUtils.GetHighPrecisionUtcTime() - manager.TimeLostConnection
                    )?.TotalSeconds > HorizonServerConfiguration.DMEClientReconnectInterval
                )
                    OutRequestsTasks.Add(manager.Start());
            }
            foreach (var manager in _MPSManagers)
            {
                if (manager.Value.IsConnected)
                {
                    if (manager.Value.CheckMPSConnectivity())
                        OutRequestsTasks.Add(manager.Value.HandleOutgoingMessages());
                }
                else if (
                    (
                        DateTimeUtils.GetHighPrecisionUtcTime() - manager.Value.TimeLostConnection
                    )?.TotalSeconds > HorizonServerConfiguration.DMEClientReconnectInterval
                )
                {
                    var applicationId = manager.Key;

                    if (_MASManagers.TryGetValue(applicationId, out var masClient))
                    {
                        if (_MASReconnectQueue.Contains(applicationId))
                            continue;

                        _MASReconnectQueue.Add(applicationId);
                        _MPSManagers.Remove(applicationId, out _);
                        if (masClient.IsConnected)
                            await masClient.Stop().ConfigureAwait(false);

                        OutRequestsTasks.Add(masClient.Start());
                    }
                    else
                        LoggerAccessor.LogError(
                            $"[DMEManager] - MPS Client timed-out, but no MAS servers exists for it! (ApplicationId: {applicationId})"
                        );
                }
            }

            await Task.WhenAll(OutRequestsTasks).ConfigureAwait(false);
        }

        public async Task StartTickPooling(CancellationToken token)
        {
            _MASManagers.Clear();
            _MPSManagers.Clear();

            // build and start medius managers per app id
            foreach (var applicationId in HorizonServerConfiguration.DMECompatibleApplicationIds)
            {
                _MASManagers.TryAdd(applicationId, new MASClient(applicationId));
            }

            while (!token.IsCancellationRequested)
            {
                await TickAsync().ConfigureAwait(false);
                await Task.Delay(100, token).ConfigureAwait(false);
            }

            foreach (var client in _MASManagers.Values)
            {
                _ = client.Stop();
            }

            foreach (var client in _MPSManagers.Values)
            {
                _ = client.Stop();
            }
        }

        public DMEObject? GetMPSClientByAccessToken(string? accessToken)
        {
            return string.IsNullOrEmpty(accessToken)
                ? null
                : _MPSManagers
                    .Select(x => x.Value.GetClientByAccessToken(accessToken))
                    .FirstOrDefault(x => x != null);
        }

        public DMEObject? GetMPSClientBySessionKey(string? sessionKey)
        {
            return string.IsNullOrEmpty(sessionKey)
                ? null
                : _MPSManagers
                    .Select(x => x.Value.GetClientBySessionKey(sessionKey))
                    .FirstOrDefault(x => x != null);
        }
    }
}
