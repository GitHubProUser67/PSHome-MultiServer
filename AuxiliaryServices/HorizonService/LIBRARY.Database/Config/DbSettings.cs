namespace Horizon.LIBRARY.Database.Config
{
    public class DbSettings
    {
        /// <summary>
        /// When true, the controller will simulate a database.
        /// Data is not persistent in simulated mode.
        /// </summary>
        public bool SimulatedMode { get; set; } = true;

        /// <summary>
        /// Database url.
        /// </summary>
        public string DatabaseUrl { get; set; }

        /// <summary>
        /// Database username.
        /// </summary>
        public string DatabaseUsername { get; set; }

        /// <summary>
        /// Database password.
        /// </summary>
        public string DatabasePassword { get; set; }

        /// <summary>
        /// Database AccessKey.
        /// </summary>
        public string DatabaseAccessKey { get; set; } = string.Empty; // Base64 key or empty string for OG horizon middleware.
    }
}