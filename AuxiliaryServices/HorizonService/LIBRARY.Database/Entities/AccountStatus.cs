namespace Horizon.LIBRARY.Database.Entities
{
    public partial class AccountStatus
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int AppId { get; set; }
        public bool LoggedIn { get; set; }
        public int? GameId { get; set; }
        public int? ChannelId { get; set; }
        public int? WorldId { get; set; }
        public string GameName { get; set; }
        public string DatabaseUser { get; set; }
    }
}
