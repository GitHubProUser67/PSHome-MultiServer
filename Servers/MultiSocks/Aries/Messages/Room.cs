using MultiSocks.Aries.Model;

namespace MultiSocks.Aries.Messages
{
    public class Room : AbstractMessage
    {
        public override string _Name { get => "room"; }

        public string? IDENT { get; set; }
        public string? NAME { get; set; }
        public string? HOST { get; set; }
        public string? DESC { get; set; }
        public string? COUNT { get; set; }
        public string? LIMIT { get; set; }
        public string? MAX { get; set; }
        public string FLAGS { get; set; } = "C";

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer mc) return;

            AriesUser? user = client.User;
            if (user == null) return;

            string? NAME = GetInputCacheValue("NAME");

            AriesRoom? existingRoom = user.CurrentRoom;

            if (existingRoom != null && existingRoom.Users.RemoveUserAndCheckRoomValidity(user))
                mc.Rooms.RemoveRoom(existingRoom);

            AriesRoom? room = mc.Rooms.GetRoomByName(NAME);
            if (room != null)
            {
                if (room.Users != null && room.Users.AddUserWithRoomMesg(user, context.Project ?? string.Empty))
                {

                }
                else
                {
                    this.NAME = string.Empty;
                    client.SendMessage(this);
                }
            }
            else
            {
                room = new Model.AriesRoom() { ID = new Random().Next(), Name = NAME, IsGlobal = false };
                mc.Rooms.AddRoom(room);

                if (room.Users != null && room.Users.AddUserWithRoomMesg(user, context.Project ?? string.Empty))
                {

                }
                else
                {
                    this.NAME = string.Empty;
                    client.SendMessage(this);
                }
            }
        }
    }
}
