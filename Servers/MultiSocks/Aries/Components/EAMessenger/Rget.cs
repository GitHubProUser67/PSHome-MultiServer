namespace MultiSocks.Aries.Components
{
    public class Rget : AbstractMessage
    {
        public override string _Name
        {
            get => "RGET";
        }

        public string? ID { get; set; }
        public string? SIZE { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            var LRSC = GetInputCacheValue("LRSC");
            var LIST = GetInputCacheValue("LIST");
            var PRES = GetInputCacheValue("PRES");
            var ID = GetInputCacheValue("ID");

            this.ID = ID;
            SIZE = "0";

            client.SendMessage(this);
        }
    }
}
