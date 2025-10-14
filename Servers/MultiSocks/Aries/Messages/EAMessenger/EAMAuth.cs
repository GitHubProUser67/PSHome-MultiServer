using MultiSocks.Aries.DataStore;

namespace MultiSocks.Aries.Messages
{
    public class EAMAuth : AbstractMessage
    {
        public override string _Name { get => "AUTH"; }

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
            if (context is not EAMessengerServer mc) return;

            string VERS = GetInputCacheValue("VERS") ?? string.Empty;
            string SKU = GetInputCacheValue("SKU") ?? string.Empty;
            string? PRES = GetInputCacheValue("PRES");
            string? USER = GetInputCacheValue("USER");
            string? PROD = GetInputCacheValue("PROD");
            string? LOC = GetInputCacheValue("LOC");
            string? MAC = GetInputCacheValue("MAC");
            string? TOKEN = GetInputCacheValue("TOKEN");
            string? PASS = GetInputCacheValue("PASS");

            client.VERS = VERS;
            client.SKU = SKU;

            DbAccount? user = Program.DirtySocksDatabase?.GetByName(USER?.Split("/").First());
            if (user == null)
            {
                client.SendMessage(this);
                return;
            }

            mc.TryEAMLogin(user, client, PASS, LOC ?? "enUS", MAC, TOKEN);
        }
    }
}
