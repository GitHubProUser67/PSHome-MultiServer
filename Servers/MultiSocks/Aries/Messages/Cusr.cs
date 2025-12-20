namespace MultiSocks.Aries.Messages
{
    public class Cusr : AbstractMessage
    {
        public override string _Name { get => "cusr"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer) return;

            client.SendMessage(this);
        }
    }
}
