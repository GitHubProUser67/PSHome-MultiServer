using Horizon.MUM.Models;

namespace Horizon.SERVER.PluginArgs
{
    public class OnPlayerGameArgs
    {
        /// <summary>
        /// Player.
        /// </summary>
        public ClientObject? Player { get; set; }

        /// <summary>
        /// Game.
        /// </summary>
        public Game? Game { get; set; }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Player: {Player} " +
                $"Game: {Game}";
        }
    }
}