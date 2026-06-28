using System.Collections.Concurrent;
using System.Reflection;
using CustomLogger;
using Horizon.RT.Common;

namespace Horizon.PluginManager
{
    public class MediusPluginsManager : IPluginHost
    {
        private readonly ConcurrentDictionary<
            PluginEvent,
            List<OnRegisterActionHandler>
        > _pluginCallbackInstances = new();
        private readonly ConcurrentDictionary<
            RT_MSG_TYPE,
            List<OnRegisterMessageActionHandler>
        > _pluginScertMessageCallbackInstances = new();
        private readonly ConcurrentDictionary<
            (NetMessageClass, byte),
            List<OnRegisterMediusMessageActionHandler>
        > _pluginMediusMessageCallbackInstances = new();
        private bool _reload = false;
        private readonly DirectoryInfo? _pluginDir = null;
        private readonly FileSystemWatcher? _watcher = null;

        public MediusPluginsManager(string pluginsDirectory)
        {
            // Ensure valid plugins directory
            _pluginDir = new DirectoryInfo(pluginsDirectory);
            if (!_pluginDir.Exists)
                return;

            // Add a watcher so we can auto reload the plugins on change
            _watcher = new FileSystemWatcher(_pluginDir.FullName, "*.dll");
            _watcher.IncludeSubdirectories = true;
            _watcher.Changed += (s, e) =>
            {
                _reload = true;
            };
            _watcher.Renamed += (s, e) =>
            {
                _reload = true;
            };
            _watcher.Created += (s, e) =>
            {
                _reload = true;
            };
            _watcher.Deleted += (s, e) =>
            {
                _reload = true;
            };
            _watcher.EnableRaisingEvents = true;

            reloadPlugins();
        }

        public async Task Tick()
        {
            if (_reload)
            {
                _reload = false;
                reloadPlugins();
            }

            try
            {
                await OnEvent(PluginEvent.TICK, null);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex.Message, ex);
            }
        }

        #region On Event

        public async Task OnEvent(PluginEvent eventType, object? data)
        {
            if (!_pluginCallbackInstances.TryGetValue(eventType, out var value))
                return;

            foreach (var callback in value)
            {
                try
                {
                    await callback.Invoke(eventType, data);
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError(
                        $"PLUGIN OnEvent Exception. {callback}({eventType}, {data})"
                    );
                    LoggerAccessor.LogError(e);
                }
            }
        }

        public async Task OnMessageEvent(RT_MSG_TYPE msgId, object data)
        {
            if (!_pluginScertMessageCallbackInstances.TryGetValue(msgId, out var value))
                return;

            foreach (var callback in value)
            {
                try
                {
                    await callback.Invoke(msgId, data);
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError(
                        $"PLUGIN OnMessageEvent Exception. {callback}({msgId}, {data})"
                    );
                    LoggerAccessor.LogError(e);
                }
            }
        }

        public async Task OnMediusMessageEvent(NetMessageClass msgClass, byte msgType, object data)
        {
            var key = (msgClass, msgType);
            if (!_pluginMediusMessageCallbackInstances.TryGetValue(key, out var value))
                return;

            foreach (var callback in value)
            {
                try
                {
                    await callback.Invoke(msgClass, msgType, data);
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError(
                        $"PLUGIN OnMediusMessageEvent Exception. {callback}({key}, {data})"
                    );
                    LoggerAccessor.LogError(e);
                }
            }
        }

        #endregion

        #region Register Event

        public void RegisterAction(PluginEvent eventType, OnRegisterActionHandler callback)
        {
            List<OnRegisterActionHandler> callbacks;
            if (!_pluginCallbackInstances.TryGetValue(eventType, out var value))
                _pluginCallbackInstances.TryAdd(
                    eventType,
                    callbacks = new List<OnRegisterActionHandler>()
                );
            else
                callbacks = value;

            callbacks.Add(callback);
        }

        public void RegisterMessageAction(
            RT_MSG_TYPE msgId,
            OnRegisterMessageActionHandler callback
        )
        {
            List<OnRegisterMessageActionHandler> callbacks;
            if (!_pluginScertMessageCallbackInstances.TryGetValue(msgId, out var value))
                _pluginScertMessageCallbackInstances.TryAdd(
                    msgId,
                    callbacks = new List<OnRegisterMessageActionHandler>()
                );
            else
                callbacks = value;

            callbacks.Add(callback);
        }

        public void RegisterMediusMessageAction(
            NetMessageClass msgClass,
            byte msgType,
            OnRegisterMediusMessageActionHandler callback
        )
        {
            List<OnRegisterMediusMessageActionHandler> callbacks;
            var key = (msgClass, msgType);
            if (!_pluginMediusMessageCallbackInstances.TryGetValue(key, out var value))
                _pluginMediusMessageCallbackInstances.TryAdd(
                    key,
                    callbacks = new List<OnRegisterMediusMessageActionHandler>()
                );
            else
                callbacks = value;

            callbacks.Add(callback);
        }

        #endregion

        private void reloadPlugins()
        {
            // Clear cache
            _pluginCallbackInstances.Clear();
            _pluginScertMessageCallbackInstances.Clear();
            _pluginMediusMessageCallbackInstances.Clear();

            LoggerAccessor.LogWarn($"Reloading plugins");

            // Ensure valid plugins directory
            if (_pluginDir == null || !_pluginDir.Exists)
                return;

            // Add assemblies
            foreach (var file in _pluginDir.GetFiles("*.dll", SearchOption.AllDirectories))
            {
                try
                {
                    var pluginAssembly = Assembly.LoadFile(file.FullName);
                    var pluginInterface = typeof(IPlugin);
                    var plugins = pluginAssembly
                        .GetTypes()
                        .Where(type => pluginInterface.IsAssignableFrom(type));

                    foreach (var plugin in plugins)
                    {
                        var instance = Activator.CreateInstance(plugin) as IPlugin;

                        if (instance is not null && file.Directory is not null)
                            _ = instance.Start(file.Directory.FullName, this);

                        //Output the Plugin name
                        LoggerAccessor.LogWarn("Plugin added: " + file.Name);
                    }
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError(ex);
                }
            }
        }
    }
}
