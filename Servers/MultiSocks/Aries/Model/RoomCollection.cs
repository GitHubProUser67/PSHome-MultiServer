using MultiSocks.Aries.Messages;

namespace MultiSocks.Aries.Model
{
    public class RoomCollection
    {
        public MatchmakerServer? Server;
        private int RoomID = 1;
        private List<AriesRoom> Rooms = new();

        public virtual void AddRoom(AriesRoom room)
        {
            if (Server != null)
            {
                lock (Rooms)
                {
                    room.Server = Server;
                    room.ID = RoomID++;
                    Rooms.Add(room);
                }
            }
        }

        public virtual void RemoveRoom(AriesRoom room)
        {
            lock (Rooms)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Room] - Removing Room:{room.Name}:{room.ID}.");
                Rooms.Remove(room);
            }
        }

        public AriesRoom? GetRoomByName(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            lock (Rooms)
            {
                return Rooms.FirstOrDefault(x => x.Name == name);
            }
        }

        public int Count()
        {
            lock (Rooms)
            {
                return Rooms.Count;
            }
        }

        public void SendRooms(AriesUser user)
        {
            var infos = new List<PlusRom>();
            lock (Rooms)
            {
                foreach (var room in Rooms)
                {
                    infos.Add(room.GetInfo());
                }
            }
            foreach (PlusRom info in infos) user.Connection?.SendMessage(info);
        }
    }
}
