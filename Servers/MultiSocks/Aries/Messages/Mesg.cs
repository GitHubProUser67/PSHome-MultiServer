namespace MultiSocks.Aries.Messages
{
    public class Mesg : AbstractMessage
    {
        public override string _Name { get => "mesg"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc || !client.HasAuth()) return;

            Model.AriesUser? user = client.User;
            if (user == null) return;

            string? PRIV = GetInputCacheValue("PRIV");
            string? ATTR = GetInputCacheValue("ATTR");
            string TEXT = GetInputCacheValue("TEXT") ?? string.Empty;

            PlusMesg mesg = new()
            {
                N = user?.PersonaName,
                T = TEXT,
            };

            if (!string.IsNullOrEmpty(context.Project) && (context.Project.Contains("NASCAR09") && context.SKU == "PS3"))
                user?.Connection?.SendMessage(this);

            //where is this message going
            Model.AriesRoom? room = user?.CurrentRoom;

            if (!string.IsNullOrEmpty(PRIV))
            {
                if (ATTR != null && ATTR.Length > 1 && ATTR[0] == 'N')
                    mesg.F = "EP" + ATTR[1..];
                mc.SendToPersona(PRIV, mesg);
            }
            else
            {
                mesg.F = ATTR;
                room?.Users?.Broadcast(mesg);
            }
        }
    }
}
