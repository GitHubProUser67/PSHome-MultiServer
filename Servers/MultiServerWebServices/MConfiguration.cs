namespace MultiServerWebServices
{
    public class MConfiguration
    {
        public string QuazalDbConnectionString { get; set; }
        public string HorizonDbConnectionString { get; set; }
        public string WebAPILeaderboardDbConnectionString { get; set; }
        public int DbType { get; set; }

    }
}
