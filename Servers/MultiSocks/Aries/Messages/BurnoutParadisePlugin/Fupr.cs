namespace MultiSocks.Aries.Messages
{
    public class Fupr : AbstractMessage
    {
        public override string _Name { get => "fupr"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(this);
        }
    }
}
