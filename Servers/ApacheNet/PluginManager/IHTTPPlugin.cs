using ApacheNet.Models;

namespace ApacheNet.PluginManager
{
    public interface IHTTPPlugin
    {
        Task HTTPStartPlugin(string param);
        object ProcessPluginMessage(object request);
        List<Route> GetRoutes();
    }
}
