namespace MultiSocks.Aries.Messages
{
    public class Flag : AbstractMessage
    {
        public override string _Name { get => "flag"; }

        public string? IDENT { get; set; }
        public string? FLAGS { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            string? CLR = GetInputCacheValue("CLR");
            string? SET = GetInputCacheValue("SET");

            IDENT = "1";
            FLAGS = SET;

            client.SendMessage(this);
        }
    }
}
