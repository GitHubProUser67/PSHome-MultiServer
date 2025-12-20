namespace MultiSocks.Aries.Messages
{
    public class Rvup : AbstractMessage
    {
        public override string _Name { get => "rvup"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(this);
        }
    }
}