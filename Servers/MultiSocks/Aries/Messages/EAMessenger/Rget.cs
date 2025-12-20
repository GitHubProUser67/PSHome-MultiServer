namespace MultiSocks.Aries.Messages
{
    public class Rget : AbstractMessage
    {
        public override string _Name { get => "RGET"; }

        public string? ID { get; set; }
        public string? SIZE { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            string? LRSC = GetInputCacheValue("LRSC");
            string? LIST = GetInputCacheValue("LIST");
            string? PRES = GetInputCacheValue("PRES");
            string? ID = GetInputCacheValue("ID");

            this.ID = ID;
            SIZE = "0";

            client.SendMessage(this);

        }
    }
}
