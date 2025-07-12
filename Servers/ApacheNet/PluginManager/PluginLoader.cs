using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ApacheNet.PluginManager
{
    public class PluginLoader
    {
        public static Dictionary<string, HTTPPlugin> LoadPluginsFromFolder(string folderPath)
        {
            Dictionary<string, HTTPPlugin> plugins = new Dictionary<string, HTTPPlugin>();

            if (Directory.Exists(folderPath))
            {
                foreach (string dllFile in Directory.GetFiles(folderPath, "*.dll", SearchOption.AllDirectories))
                {
                    HTTPPlugin? plugin = LoadPlugin(dllFile);
                    if (plugin is not null)
                    {
                        CustomLogger.LoggerAccessor.LogInfo($"[PluginLoader] - Plugin: {dllFile} Loaded.");
                        plugins.Add(Path.GetFileNameWithoutExtension(dllFile), plugin);
                    }
                }
            }
            else
                CustomLogger.LoggerAccessor.LogWarn($"[PluginLoader] - No Plugins Folder found: {folderPath}");

            return plugins;
        }

        public static HTTPPlugin? LoadPlugin(string pluginPath)
        {
            try
            {
                foreach (Type type in Assembly.LoadFrom(pluginPath).GetTypes())
                {
                    try
                    {
                        if (typeof(HTTPPlugin).IsAssignableFrom(type))
                            return Activator.CreateInstance(type) as HTTPPlugin;
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        CustomLogger.LoggerAccessor.LogWarn($"[PluginLoader] - Plugin: {pluginPath} is not compatible with this project, ignoring...");
                    }
                }
            }
            catch (BadImageFormatException)
            {

            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError($"[PluginLoader] - Error loading plugin/dependency '{pluginPath}': {ex}");
            }

            return null;
        }
    }
}
