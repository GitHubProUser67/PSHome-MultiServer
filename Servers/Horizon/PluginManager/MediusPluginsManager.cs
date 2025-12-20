using CustomLogger;
using Horizon.RT.Common;
using System.Collections.Concurrent;
using System.Reflection;

namespace Horizon.PluginManager
{
    public class MediusPluginsManager : IPluginHost
    {
        private ConcurrentDictionary<PluginEvent, List<OnRegisterActionHandler>> _pluginCallbackInstances = new();
        private ConcurrentDictionary<RT_MSG_TYPE, List<OnRegisterMessageActionHandler>> _pluginScertMessageCallbackInstances = new();
        private ConcurrentDictionary<(NetMessageClass, byte), List<OnRegisterMediusMessageActionHandler>> _pluginMediusMessageCallbackInstances = new();
        private bool _reload = false;
        private DirectoryInfo? _pluginDir = null;
        private FileSystemWatcher? _watcher = null;

        public MediusPluginsManager(string pluginsDirectory)
        {
            // Ensure valid plugins directory
            _pluginDir = new DirectoryInfo(pluginsDirectory);
            if (!_pluginDir.Exists)
                return;

            // Add a watcher so we can auto reload the plugins on change
            _watcher = new FileSystemWatcher(_pluginDir.FullName, "*.dll");
            _watcher.IncludeSubdirectories = true;
            _watcher.Changed += (s, e) => { _reload = true; };
            _watcher.Renamed += (s, e) => { _reload = true; };
            _watcher.Created += (s, e) => { _reload = true; };
            _watcher.Deleted += (s, e) => { _reload = true; };
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
            if (!_pluginCallbackInstances.ContainsKey(eventType))
                return;

            foreach (var callback in _pluginCallbackInstances[eventType])
            {
                try
                {
                    await callback.Invoke(eventType, data);
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"PLUGIN OnEvent Exception. {callback}({eventType}, {data})");
                    LoggerAccessor.LogError(e);
                }
            }
        }

        public async Task OnMessageEvent(RT_MSG_TYPE msgId, object data)
        {
            if (!_pluginScertMessageCallbackInstances.ContainsKey(msgId))
                return;

            foreach (var callback in _pluginScertMessageCallbackInstances[msgId])
            {
                try
                {
                    await callback.Invoke(msgId, data);
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"PLUGIN OnMessageEvent Exception. {callback}({msgId}, {data})");
                    LoggerAccessor.LogError(e);
                }
            }
        }

        public async Task OnMediusMessageEvent(NetMessageClass msgClass, byte msgType, object data)
        {
            var key = (msgClass, msgType);
            if (!_pluginMediusMessageCallbackInstances.ContainsKey(key))
                return;

            foreach (var callback in _pluginMediusMessageCallbackInstances[key])
            {
                try
                {
                    await callback.Invoke(msgClass, msgType, data);
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"PLUGIN OnMediusMessageEvent Exception. {callback}({key}, {data})");
                    LoggerAccessor.LogError(e);
                }
            }
        }

        #endregion

        #region Register Event

        public void RegisterAction(PluginEvent eventType, OnRegisterActionHandler callback)
        {
            List<OnRegisterActionHandler> callbacks;
            if (!_pluginCallbackInstances.ContainsKey(eventType))
                _pluginCallbackInstances.TryAdd(eventType, callbacks = new List<OnRegisterActionHandler>());
            else
                callbacks = _pluginCallbackInstances[eventType];


            callbacks.Add(callback);
        }

        public void RegisterMessageAction(RT_MSG_TYPE msgId, OnRegisterMessageActionHandler callback)
        {
            List<OnRegisterMessageActionHandler> callbacks;
            if (!_pluginScertMessageCallbackInstances.ContainsKey(msgId))
                _pluginScertMessageCallbackInstances.TryAdd(msgId, callbacks = new List<OnRegisterMessageActionHandler>());
            else
                callbacks = _pluginScertMessageCallbackInstances[msgId];


            callbacks.Add(callback);
        }

        public void RegisterMediusMessageAction(NetMessageClass msgClass, byte msgType, OnRegisterMediusMessageActionHandler callback)
        {
            List<OnRegisterMediusMessageActionHandler> callbacks;
            var key = (msgClass, msgType);
            if (!_pluginMediusMessageCallbackInstances.ContainsKey(key))
                _pluginMediusMessageCallbackInstances.TryAdd(key, callbacks = new List<OnRegisterMediusMessageActionHandler>());
            else
                callbacks = _pluginMediusMessageCallbackInstances[key];


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
                    Assembly pluginAssembly = Assembly.LoadFile(file.FullName);
                    Type pluginInterface = typeof(IPlugin);
                    var plugins = pluginAssembly.GetTypes()
                        .Where(type => pluginInterface.IsAssignableFrom(type));

                    foreach (var plugin in plugins)
                    {
                        IPlugin? instance = Activator.CreateInstance(plugin) as IPlugin;

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
