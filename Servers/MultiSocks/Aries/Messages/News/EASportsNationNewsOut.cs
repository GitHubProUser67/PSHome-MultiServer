using MultiServerLibrary.Extension;

namespace MultiSocks.Aries.Messages
{
    public class EASportsNationNewsOut : AbstractMessage
    {
        public override string _Name { get => "news"; }
        public string? PEERTIMEOUT { get; set; } = "10000";
        public string? BUDDY_URL { get; set; } = "msgconn.beta.ea.com";
        public string? BUDDY_SERVER { get; set; } = InternetProtocolUtils.TryGetServerIP(out _).Result ? InternetProtocolUtils.GetPublicIPAddress() : InternetProtocolUtils.GetLocalIPAddresses().First().ToString();
        public string? BUDDY_PORT { get; set; } = "10899";
        public string? NEWS_URL { get; set; } = "http://msgconn.beta.ea.com/easo/cso05/pub/NEWS.txt";
        public string? NEWS_DATE { get; set; } = "2008.6.11 21:00:00";
        public string? FAQ_URL { get; set; } = "\"http://gos.ea.com/easo/editorial/common/2008/eaconnect/connect.jsp?site=easo&lkey=$LKEY$&lang=%s&country=%s\"";
        public string? FAQ_DATE { get; set; } = "2008.6.11 21:00:00";
        public string? WEB_OFFER_URL { get; set; } = "\"http://gos.ea.com/easo/editorial/common/2008/eaconnect/connect.jsp?site=easo&lkey=$LKEY$&lang=%s&country=%s\"";
        public string? WEB_OFFER_BTN { get; set; } = "button0";
        public string? WEB_OFFER_DATE { get; set; } = "2008.6.11 21:00:00";
        public string? TOSA_URL { get; set; } = "http://msgconn.beta.ea.com/easo/cso05/pub/TOSA.txt";
        public string? TOSA_DATE { get; set; } = "2008.6.11 21:00:00";
        public string? TOSAC_URL { get; set; } = "http://msgconn.beta.ea.com/easo/cso05/pub/TOSAC.txt";
        public string? TOSAC_DATE { get; set; } = "2008.6.11 21:00:00";
        public string? MENU_WEBGM0_URL { get; set; } = "\"http://gos.ea.com/easo/editorial/common/2008/eaconnect/connect.jsp?site=easo&lkey=$LKEY$&lang=%s&country=%s\"";
        public string? MENU_WEBGM1_URL { get; set; } = "\"http://gos.ea.com/easo/editorial/common/2008/eaconnect/connect.jsp?site=easo&lkey=$LKEY$&lang=%s&country=%s\"";
        public string? MENU_WEBGM2_URL { get; set; } = "\"http://gos.ea.com/easo/editorial/common/2008/eaconnect/connect.jsp?site=easo&lkey=$LKEY$&lang=%s&country=%s\"";
        public string? BILLBOARD_URL { get; set; } = "\"http://gos.ea.com/easo/editorial/common/2008/eaconnect/connect.jsp?site=easo&lkey=$LKEY$&lang=%s&country=%s\"";
    }
}
