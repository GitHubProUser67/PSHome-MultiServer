namespace MultiSocks.Aries.Messages
{
    public class Ucre : AbstractMessage
    {
        public override string _Name { get => "ucre"; }

        public string? IDENT { get; set; }
        public string? NAME { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            string? NAME = GetInputCacheValue("NAME");

            if (!string.IsNullOrEmpty(NAME))
            {
                IDENT = "1";
                this.NAME = NAME;
                client.SendMessage(this);
            }
            else
            {
                IDENT = "1";
                this.NAME = "UserSet1";
                client.SendMessage(this);
            }
        }
    }
}
