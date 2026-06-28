namespace MultiSocks.Aries.Components
{
    public class EAMAuth : AbstractMessage
    {
        public override string _Name
        {
            get => "AUTH";
        }

        public string? TOS { get; set; }
        public string? NAME { get; set; }
        public string MAIL { get; set; } = "tsbo@freeso.net";
        public string? BORN { get; set; }
        public string? GEND { get; set; }
        public string? FROM { get; set; }
        public string? SHARE { get; set; }
        public string? GFIDS { get; set; }
        public string? LANG { get; set; }
        public string? LOC { get; set; }
        public string SPAM { get; set; } = "NN";
        public string? PERSONAS { get; set; } // comma separated list
        public string? LAST { get; set; }
        public string? SINCE { get; set; }
        public string? ADDR { get; set; }
        public string? LUID { get; set; }
        public string? TOKEN { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not EAMessengerServer mc)
                return;

            var VERS = GetInputCacheValue("VERS") ?? string.Empty;
            var SKU = GetInputCacheValue("SKU") ?? string.Empty;
            var PRES = GetInputCacheValue("PRES");
            var USER = GetInputCacheValue("USER");
            var PROD = GetInputCacheValue("PROD");
            var LOC = GetInputCacheValue("LOC");
            var MAC = GetInputCacheValue("MAC");
            var TOKEN = GetInputCacheValue("TOKEN");
            var PASS = GetInputCacheValue("PASS");

            client.VERS = VERS;
            client.SKU = SKU;

            var user = Program.DirtySocksDatabase?.GetByName(USER?.Split("/").First());
            if (user == null)
            {
                client.SendMessage(this);
                return;
            }

            mc.TryEAMLogin(user, client, PASS, LOC ?? "enUS", MAC, TOKEN);
        }
    }
}
