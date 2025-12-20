namespace MultiSocks.Aries.Messages
{
    public class Rrup : AbstractMessage
    {
        public override string _Name { get => "rrup"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(new RrupTime());
        }
    }
}
