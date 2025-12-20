using Horizon.MUM.Models;

namespace Horizon.MUIS.PluginArgs
{
    public class OnPlayerArgs
    {
        /// <summary>
        /// Player.
        /// </summary>
        public ClientObject? Player { get; set; }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Player: {Player}";
        }
    }
}