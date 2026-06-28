namespace MultiSocks.Aries.Components
{
    public class Pset : AbstractMessage
    {
        public override string _Name
        {
            get => "PSET";
        }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            var RSRC = GetInputCacheValue("RSRC");
            var SHOW = GetInputCacheValue("SHOW");
            var PROD = GetInputCacheValue("PROD");
            var STAT = GetInputCacheValue("STAT");

            client.SendMessage(new PgetOut() { USER = "TEMP" });

            client.SendMessage(
                new PresOut()
                {
                    CHNG = "1",
                    SHOW = SHOW,
                    PROD = PROD,
                    STAT = STAT,
                    P = "en",
                    en = "en",
                }
            );
        }
    }
}
