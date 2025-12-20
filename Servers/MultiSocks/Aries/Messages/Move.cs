using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Move : AbstractMessage
    {
        public override string _Name { get => "move"; }

        public string? IDENT { get; set; }
        public string? NAME { get; set; }
        public string? COUNT { get; set; }
        public string FLAGS { get; set; } = "C";
        public string? LIDENT { get; set; }
        public string? LCOUNT { get; set; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            AriesUser? user = client.User;
            if (user == null) return;

            string? NAME = GetInputCacheValue("NAME");

            AriesRoom? existingRoom = user.CurrentRoom;

            if (existingRoom != null && existingRoom.Users.RemoveUserAndCheckRoomValidity(user))
                mc.Rooms.RemoveRoom(existingRoom);

            AriesRoom? room = null;

            if (!string.IsNullOrEmpty(NAME))
                room = mc.Rooms.GetRoomByName(NAME);

            if (room != null)
            {
                if (room.Users == null)
                {
                    client.SendMessage(new MoveImst());
                    return;
                }
                else if (!room.Users.AddUser(user, context.Project ?? string.Empty))
                {
                    client.SendMessage(new MoveFull());
                    return;
                }
            }
            else
            {
                this.NAME = string.Empty;
                client.SendMessage(this);
            }
        }
    }
}
