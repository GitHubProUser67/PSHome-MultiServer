using Horizon.MUM.Models;
using Horizon.RT.Models;

namespace Horizon.MEDIUS.PluginArgs
{
    public class OnAccountLoginRequestArgs
    {
        /// <summary>
        /// Player making request.
        /// </summary>
        public ClientObject? Player { get; set; }

        /// <summary>
        /// AccountLogin request.
        /// </summary>
        public IMediusRequest? Request { get; set; }

        public override string ToString()
        {
            return base.ToString() + " " + $"Player: {Player} " + $"Request: {Request}";
        }
    }
}
