namespace Horizon.PluginManager
{
    public interface IPlugin
    {
        Task Start(string workingDirectory, IPluginHost host);
    }
}