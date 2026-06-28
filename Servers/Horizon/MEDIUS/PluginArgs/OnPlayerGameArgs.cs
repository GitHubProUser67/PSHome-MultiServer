using Horizon.MUM.Models;

namespace Horizon.MEDIUS.PluginArgs
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

        /// <summary>
        /// Party.
        /// </summary>
        public Party? Party { get; set; }

        public override string ToString()
        {
            return base.ToString()
                + " "
                + $"Player: {(Player != null ? Player : string.Empty)} "
                + $"Game: {(Game != null ? Game : string.Empty)} "
                + $"Party: {(Party != null ? Party : string.Empty)}";
        }
    }
}
