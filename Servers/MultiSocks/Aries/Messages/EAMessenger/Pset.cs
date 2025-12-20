namespace MultiSocks.Aries.Messages
{
    public class Pset : AbstractMessage
    {
        public override string _Name { get => "PSET"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            string? RSRC = GetInputCacheValue("RSRC");
            string? SHOW = GetInputCacheValue("SHOW");
            string? PROD = GetInputCacheValue("PROD");
            string? STAT = GetInputCacheValue("STAT");

            client.SendMessage(new PgetOut()
            {
                USER = "TEMP"
            });
            
            client.SendMessage(new PresOut()
            {
                CHNG = "1",
                SHOW = SHOW,
                PROD = PROD,
                STAT = STAT,
                P = "en",
                en = "en"
            });
            
        }
    }
}
