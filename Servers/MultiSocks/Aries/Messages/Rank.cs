using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Rank : AbstractMessage
    {
        public override string _Name { get => "rank"; }

        public string RANK { get; set; } = "Unranked";
        public string TIME { get; set; } = "1707089968";

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            AriesUser? user = client.User;
            if (user == null) return;

            client.SendMessage(this);

            user.CurrentGame?.BroadcastPopulation(mc);
            user.CurrentGame?.SetGameStatus(false);
        }
    }
}
