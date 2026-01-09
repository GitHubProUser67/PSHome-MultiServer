namespace MultiSocks.Aries.Messages
{
    public class Ussv : AbstractMessage
    {
        public override string _Name { get => "ussv"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer || client.User == null) return;

            CopyInputCacheToOutputCache();

            client.User.Settings = new Dictionary<string, string?>(OutputCache);

            client.SendMessage(this);
        }
    }
}
