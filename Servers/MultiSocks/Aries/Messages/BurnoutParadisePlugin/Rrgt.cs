namespace MultiSocks.Aries.Messages
{
    public class Rrgt : AbstractMessage
    {
        public override string _Name { get => "rrgt"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(new RrgtTime());
        }
    }
}
