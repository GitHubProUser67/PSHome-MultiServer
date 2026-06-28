namespace SVO.DATABASE
{
    public class AppSettings
    {
        /// <summary>
        /// This settings respective app id.
        /// </summary>
        public int AppId { get; }

        public AppSettings(int appId)
        {
            AppId = appId;
        }

        public void SetSettings(Dictionary<string, string> settings) { }

        public Dictionary<string, string> GetSettings()
        {
            return new Dictionary<string, string>() { };
        }
    }
}
