using Horizon.MUM.Models;

namespace Horizon.SERVER.PluginArgs
{
    public class OnPlayerWideStatsArgs
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
        /// Incoming wide stats to post.
        /// </summary>
        public int[]? WideStats { get; set; }

        /// <summary>
        /// Whether or not the incoming stats are for the player's clan.
        /// </summary>
        public bool IsClan { get; set; } = false;

        /// <summary>
        /// Whether or not to reject the incoming stats.
        /// </summary>
        public bool Reject { get; set; } = false;

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Player: {Player}";
        }
    }
}