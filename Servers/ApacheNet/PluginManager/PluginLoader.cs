using System.Reflection;

namespace ApacheNet.PluginManager
{
    public class PluginLoader
    {
        public static Dictionary<string, IHTTPPlugin> LoadPluginsFromFolder(string folderPath)
        {
            Dictionary<string, IHTTPPlugin> plugins = [];

            if (Directory.Exists(folderPath))
            {
                foreach (
                    var dllFile in Directory.GetFiles(
                        folderPath,
                        "*.dll",
                        SearchOption.AllDirectories
                    )
                )
                {
                    var plugin = LoadPlugin(dllFile);
                    if (plugin is not null)
                    {
                        CustomLogger.LoggerAccessor.LogInfo(
                            $"[PluginLoader] - Plugin: {dllFile} Loaded."
                        );
                        plugins.Add(Path.GetFileNameWithoutExtension(dllFile), plugin);
                    }
                }
            }
            else
                CustomLogger.LoggerAccessor.LogWarn(
                    $"[PluginLoader] - No Plugins Folder found: {folderPath}"
                );

            return plugins;
        }

        public static IHTTPPlugin? LoadPlugin(string pluginPath)
        {
            try
            {
                foreach (var type in Assembly.LoadFrom(pluginPath).GetTypes())
                {
                    try
                    {
                        if (typeof(IHTTPPlugin).IsAssignableFrom(type))
                            return Activator.CreateInstance(type) as IHTTPPlugin;
                    }
                    catch (ReflectionTypeLoadException)
                    {
                        CustomLogger.LoggerAccessor.LogWarn(
                            $"[PluginLoader] - Plugin: {pluginPath} is not compatible with this project, ignoring..."
                        );
                    }
                }
            }
            catch (BadImageFormatException) { }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError(
                    $"[PluginLoader] - Error loading plugin/dependency '{pluginPath}': {ex}"
                );
            }

            return null;
        }
    }
}
