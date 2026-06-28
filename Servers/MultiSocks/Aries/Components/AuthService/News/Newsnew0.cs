namespace MultiSocks.Aries.Components.AuthService.News
{
    public class Newsnew0 : AbstractMessage
    {
        public override string _Name
        {
            get => "newsnew0";
        }
        public string BUDDYSERVERNAME { get; set; } =
            MultiSocksServerConfiguration.ServerBindAddress;
        public string? BUDDYRESOURCE { get; set; }
        public string BUDDYPORT { get; set; } = "10899";
    }
}
