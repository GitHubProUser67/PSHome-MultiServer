using Horizon.MUM.Models;

namespace Horizon.SERVER.PluginArgs
{
    public class OnPartyArgs
    {
        /// <summary>
        /// Party.
        /// </summary>
        public Party? Party { get; set; }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Party: {Party}";
        }
    }
}