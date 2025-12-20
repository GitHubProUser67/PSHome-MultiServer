namespace MultiSocks.Aries.Messages
{
    public class Rrlc : AbstractMessage
    {
        public override string _Name { get => "rrlc"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(new RrlcTime());
        }
    }
}
