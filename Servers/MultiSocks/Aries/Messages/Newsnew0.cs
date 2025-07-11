using MultiServerLibrary.Extension;

namespace MultiSocks.Aries.Messages
{
    public class Newsnew0 : AbstractMessage
    {
        public override string _Name { get => "newsnew0"; }
        public string BUDDYSERVERNAME { get; set; } = InternetProtocolUtils.TryGetServerIP(out _).Result ? InternetProtocolUtils.GetPublicIPAddress() : InternetProtocolUtils.GetLocalIPAddresses().First().ToString();
        public string? BUDDYRESOURCE { get; set; }
        public string BUDDYPORT { get; set; } = "10899";
    }
}
