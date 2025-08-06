using MultiSocks.Aries.Messages.Burnout3Plugin.MultiSocks.Aries.Messages;
using MultiSocks.Aries.Messages.PersonaService;
using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Sviw : AbstractMessage
    {
        public override string _Name { get => "sviw"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            string? VIEW = GetInputCacheValue("VIEW");

            if (context is not MatchmakerServer) return;

            AriesUser? user = client.User;
            if (user == null) return;

            if (VIEW == "DLC" || VIEW == "lobby")
                client.SendMessage(new BOPDlc());
            else if (client.VERS == "FLM/A1") //Burnout 3 Takedown
            {
                client.SendMessage(new BO3Stats());
                user.SendPlusWho(user, "FLM/A1");
                client.SendMessage(new PlusRom()
                {
                    I = "420",
                    N = user.PersonaName,
                });
            }
            else
            {
                if (user.SelectedPersona != -1) return;
                user.SelectPersona(GetInputCacheValue("PERS"));
                if (user.SelectedPersona == -1) return; //failed?
                client.SendMessage(new Pers()
                {
                    NAME = user.Username,
                    PERS = user.PersonaName
                });
            }

            client.SendMessage(new Ping());
        }
    }
}
