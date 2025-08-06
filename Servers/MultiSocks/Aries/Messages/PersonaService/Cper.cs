using MultiSocks.Aries.Messages.PersonaService.ErrorCodes;

namespace MultiSocks.Aries.Messages.PersonaService
{
    public class Cper : AbstractMessage
    {
        public override string _Name { get => "cper"; }

        public string? NAME { get; set; }
        public string? PERS { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            string? PERS = GetInputCacheValue("PERS");
            string? ALTS = GetInputCacheValue("ALTS");

            if (context is not MatchmakerServer || PERS == null || client.User == null || !client.HasAuth()) return;

            int index = AriesServer.Database.AddPersona(client.User.ID, PERS);
            if (index < 0)
            {
                if (index == -2) client.SendMessage(new CperDupl());
                else client.SendMessage(new CperImst());
                return;
            }
            Model.AriesUser user = client.User;
            user.Personas[index] = PERS;

            NAME = user.Username;
            this.PERS = PERS;

            client.SendMessage(this);
        }
    }
}
