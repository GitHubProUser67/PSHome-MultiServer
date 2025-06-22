namespace MultiSocks.Aries.Messages
{
    public class Onln : AbstractMessage
    {
        public override string _Name { get => "onln"; }

        public string? M { get; set; }
        public string? N { get; set; }
        public string? MA { get; set; }
        public string? A { get; set; }
        public string? LA { get; set; }
        public string? P { get; set; }
        public string? CL { get; set; }
        public string F { get; set; } = string.Empty;
        public string? G { get; set; }
        public string? HW { get; set; }
        public string? I { get; set; }
        public string? LO { get; set; }
        public string? LV { get; set; }
        public string? MD { get; set; }
        public string? PRES { get; set; }
        public string? RP { get; set; }
        public string? RG { get; set; }
        public string? RM { get; set; }
        public string S { get; set; } = string.Empty;
        public string? AT { get; set; }
        public string? C { get; set; }
        public string? CI { get; set; }
        public string? CT { get; set; }
        public string? US { get; set; }
        public string? VER { get; set; }
        public string? VERS { get; set; }
        public string? X { get; set; }
        public string? R { get; set; } //room
        public string? RGC { get; set; }
        public string? RI { get; set; } //room id
        public string? RF { get; set; }
        public string? RT { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            string? PERS = GetInputCacheValue("PERS");
            string? OFFLINE = GetInputCacheValue("OFFLINE");
            string? ROOM = GetInputCacheValue("ROOM");

            Model.AriesUser? user = !string.IsNullOrEmpty(PERS) ? mc.Users.GetUserByPersonaName(PERS) : client.User;
            if (user == null) return;

            if (!string.IsNullOrEmpty(context.Project) && (context.Project.Contains("BURNOUT5") || context.Project.Contains("NASCAR09")))
                client.SendMessage(user.SendOnlnOut(user, client.VERS));
            else
            {
                Model.AriesRoom? Room = user.CurrentRoom;

                PlusUser info = user.GetInfo();

                client.SendMessage(info);
                client.SendMessage(this);
                //client.SendMessage(new OnlnImst());
            }
        }
    }
}
