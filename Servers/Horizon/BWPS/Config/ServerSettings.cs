

using MultiServerLibrary.Extension;

namespace Horizon.BWPS.Config
{
    public class ServerSettings
    {
        /// <summary>
        /// How many milliseconds before refreshing the config.
        /// </summary>
        public int RefreshConfigInterval = 5000;

        /// <summary>
        /// Port of the BWPS.
        /// </summary>
        public int BWPSPort { get; set; } = 50100;

        #region BWPS SCE-RT Service Location
        /// <summary>
        /// IP of the BWPS.
        /// </summary>
        public string BWPSIp { get; set; } = InternetProtocolUtils.GetLocalIPAddresses().First().ToString();
        #endregion

        /// <summary>
        /// Whether or not to encrypt messages.
        /// </summary>
        public bool EncryptMessages { get; set; } = true;

        /// <summary>
        /// Milliseconds between plugin ticks.
        /// </summary>
        public int PluginTickIntervalMs { get; set; } = 50;
    }
}