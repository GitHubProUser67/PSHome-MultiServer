using System.Threading.Tasks;

namespace ApacheNet.PluginManager
{
    public interface HTTPPlugin
    {
        Task HTTPStartPlugin(string param, ushort port);
        object ProcessPluginMessage(object request);
    }
}