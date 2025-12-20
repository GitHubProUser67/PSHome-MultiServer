using Horizon.DME.Models;

namespace Horizon.DME.PluginArgs
{
    public class OnPlayerArgs
    {
        public DMEObject? Player { get; set; }

        public World? Game { get; set; }
    }
}