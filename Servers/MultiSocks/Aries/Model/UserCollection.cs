using MultiSocks.Aries.Messages;

namespace MultiSocks.Aries.Model
{
    public class UserCollection
    {
        protected ConcurrentList<AriesUser> Users = new();

        public List<AriesUser> GetAll()
        {
            return Users.ToList();
        }

        public virtual bool AddUser(AriesUser? user, string VERS = "")
        {
            if (user == null || Users.Contains(user))
                return false;

            Users.Add(user);
            return true;
        }

        public virtual bool AddUserWithRoomMesg(AriesUser? user, string VERS = "")
        {
            if (user == null || Users.Contains(user))
                return false;

            Users.Add(user);
            return true;
        }

        public virtual bool RemoveUser(AriesUser? user)
        {
            if (user == null)
                return false;

            return Users.Remove(user);
        }

        public AriesUser? GetUserByName(string? name)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return Users.FirstOrDefault(x => x.Username == name);
        }

        public AriesUser? GetUserByPersonaName(string name)
        {
            return Users.FirstOrDefault(x => x.PersonaName == name);
        }

        public int Count()
        {
            return Users.Count;
        }

        public void Broadcast(AbstractMessage msg)
        {
            foreach (AriesUser user in Users)
            {
                if (user.Connection == null)
                {
                    new Thread(() =>
                    {
                        int retries = 0;
                        while (retries < 5)
                        {
                            if (user.Connection != null)
                            {
                                user.Connection.SendMessage(msg);
                                break;
                            }
                            retries++;
                            Thread.Sleep(500);
                        }
                    }).Start();
                }
                else
                    user.Connection.SendMessage(msg);
            }
        }
    }
}
