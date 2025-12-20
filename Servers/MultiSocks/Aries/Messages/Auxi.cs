namespace MultiSocks.Aries.Messages
{
    public class Auxi : AbstractMessage
    {
        public override string _Name { get => "auxi"; }

        public string TEXT { get; set; } = string.Empty;

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer) return;

            Model.AriesUser? user = client.User;
            if (user == null) return;

            user.Auxiliary = TEXT;
            client.SendMessage(this);
        }
    }
}
