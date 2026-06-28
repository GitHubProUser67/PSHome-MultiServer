using Horizon.MUM.Models;

namespace Horizon.MEDIUS.PluginArgs
{
    public class OnGameArgs
    {
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
                + $"Game: {(Game != null ? Game : string.Empty)} "
                + $"Party: {(Party != null ? Party : string.Empty)} ";
        }
    }
}
