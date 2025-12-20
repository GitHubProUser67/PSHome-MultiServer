namespace MultiSocks.Aries.Messages
{
    public class Peek : AbstractMessage
    {
        public override string _Name { get => "peek"; }

        public string? IDENT { get; set; }
        public string? NAME { get; set; }
        public string? COUNT { get; set; }
        public string FLAGS { get; set; } = "C";

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            Model.AriesUser? user = client.User;
            if (user == null) return;

            string? NAME = GetInputCacheValue("NAME");

            if (string.IsNullOrEmpty(NAME))
            {
                this.NAME = string.Empty;
                client.SendMessage(this);
                return;
            }

            Model.AriesRoom? room = mc.Rooms.GetRoomByName(NAME);
            if (room != null)
                room.Users?.AuditRoom(user);
            else
            {
                this.NAME = string.Empty;
                client.SendMessage(this);
            }
        }
    }

    public class UnPeek : AbstractMessage
    {
        public override string _Name { get => "unpeek"; }
    }
}
