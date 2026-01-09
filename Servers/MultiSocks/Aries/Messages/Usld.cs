namespace MultiSocks.Aries.Messages
{
    public class Usld : AbstractMessage
    {
        public override string _Name { get => "usld"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer || client.User == null) return;

            foreach (var entry in client.User.Settings)
                OutputCache.Add(entry.Key, entry.Value ?? string.Empty);

            client.SendMessage(this);
        }
    }
}
