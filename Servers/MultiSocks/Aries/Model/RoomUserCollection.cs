using MultiSocks.Aries.Messages;

namespace MultiSocks.Aries.Model
{
    public class RoomUserCollection : UserCollection
    {
        public AriesRoom Room;

        public RoomUserCollection(AriesRoom parent)
        {
            Room = parent;
        }

        public override bool AddUser(AriesUser? user, string VERS = "")
        {
            if (user == null)
                return false;

            if (Users.Count >= Room.Max) return false;
            if (!base.AddUser(user)) return false;

            //send move to this user
            Move move = new Move()
            {
                IDENT = Room.ID.ToString(),
                NAME = Room.Name,
                COUNT = Users.Count.ToString()
            };
            user.Connection?.SendMessage(move);

            user.CurrentRoom = Room;

            //send who to this user to tell them who they are
            user.SendPlusWho(user, VERS);

            RefreshUser(user);
            ListToUser(user);
            Room.BroadcastPopulation();
            return true;
        }

        public override bool AddUserWithRoomMesg(AriesUser? user, string VERS = "")
        {
            if (user == null)
                return false;

            if (Users.Count >= Room.Max) return false;
            if (!base.AddUser(user)) return false;

            //send move to this user
            Room move = new Room()
            {
                IDENT = Room.ID.ToString(),
                NAME = Room.Name,
                COUNT = Users.Count.ToString()
            };
            user.Connection?.SendMessage(move);

            user.CurrentRoom = Room;

            //send who to this user to tell them who they are
            user.SendPlusWho(user, VERS);

            RefreshUser(user);
            ListToUser(user);
            Room.BroadcastPopulation();
            return true;
        }

        public void AuditRoom(AriesUser user, string VERS = "")
        {
            //send move to this user
            Peek peek = new Peek()
            {
                IDENT = Room.ID.ToString(),
                NAME = Room.Name,
                COUNT = Users.Count.ToString()
            };
            user.Connection?.SendMessage(peek);

            //send who to this user to tell them who they are

            PlusUser info = user.GetInfo();

            user.Connection?.SendMessage(new PlusWho()
            {
                I = info.I ?? string.Empty,
                N = info.N,
                M = info.M,
                A = info.A ?? string.Empty,
                X = info.X,
                R = Room.Name,
                RI = Room.ID.ToString(),
                S = string.Empty,
                F = string.Empty
            });
            RefreshUser(user);
            ListToUser(user);
            Room.BroadcastPopulation();
        }

        public void RefreshUser(AriesUser target)
        {
            Broadcast(target.GetInfo());
        }

        public void ListToUser(AriesUser target)
        {
            List<PlusUser> infos = new();
            foreach (var user in Users)
            {
                infos.Add(user.GetInfo());
            }
            foreach (var info in infos) target.Connection?.SendMessage(info);
        }

        public override bool RemoveUser(AriesUser? user)
        {
            base.RemoveUser(user);
            Broadcast(new PlusUser()
            {
                I = user.ID.ToString(),
                T = Room.Users.Count().ToString(),
                F = null,
                P = null,
                S = null
            });

            Broadcast(new PlusMesg()
            {
                F = "C",
                T = "\"has left the room\"",
                N = user.PersonaName
            });

            Room.BroadcastPopulation();
            Room.RemoveChallenges(user);
            user.CurrentRoom = null;
            return true;
        }

        public bool RemoveUserAndCheckRoomValidity(AriesUser? user)
        {
            if (Room.IsGlobal)
            {
                RemoveUser(user);
                return false;
            }

            base.RemoveUser(user);
            Broadcast(new PlusUser()
            {
                I = user.ID.ToString(),
                T = Room.Users.Count().ToString(),
                F = null,
                P = null,
                S = null
            });

            Broadcast(new PlusMesg()
            {
                F = "C",
                T = "\"has left the room\"",
                N = user.PersonaName
            });

            Room.BroadcastPopulation();
            Room.RemoveChallenges(user);
            user.CurrentRoom = null;

            if (Room.Users.Count() == 0)
                return true;

            return false;
        }
    }
}
