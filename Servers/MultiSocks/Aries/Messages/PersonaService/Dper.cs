namespace MultiSocks.Aries.Messages.PersonaService
{
    public class Dper : AbstractMessage
    {
        public override string _Name { get => "dper"; }

        public string? NAME { get; set; }
        public string? PERS { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer || !client.HasAuth()) return;

            string? PERS = GetInputCacheValue("PERS");

            int index = AriesServer.Database.DeletePersona(client.User.ID, PERS);
            if (index == -1)
                return;
            Model.AriesUser user = client.User;
            for (int i = index; i < 4; i++)
            {
                user.Personas[index] = i == 4 ? null : user.Personas[index + 1];
            }

            NAME = user.Username;
            this.PERS = PERS;

            client.SendMessage(this);
        }
    }
}
