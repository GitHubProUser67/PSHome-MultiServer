namespace MultiSocks.Aries.Messages
{
    internal class Rcat : AbstractMessage
    {
        public override string _Name { get => "rcat"; }

        public string? CAT { get; set; } = "temp,,tp,res0,res1,res2,res3,1";

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(this);
        }
    }
}
