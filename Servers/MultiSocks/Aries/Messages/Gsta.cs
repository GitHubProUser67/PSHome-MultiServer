using MultiSocks.Aries.Messages.ErrorCodes;
using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Gsta : AbstractMessage
    {
        public override string _Name { get => "gsta"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            AriesUser? user = client.User;
            if (user == null) return;

            if (user.CurrentGame != null)
            {
                user.CurrentGame.SetGameStatus(true);

                user.Connection?.SendMessage(this);

                user.SendPlusWho(user, context.Project);

                user.CurrentGame.BroadcastPopulation(mc);

                user.CurrentGame.BroadcastPlusSes();
            }
            else
                user.Connection?.SendMessage(new GstaImst());
        }
    }
}
