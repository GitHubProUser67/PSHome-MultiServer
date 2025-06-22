using MultiSocks.Aries.Messages;

namespace MultiSocks.Aries.Model
{
    public class AriesRoom
    {
        public MatchmakerServer? Server;
        public int ID;
        public string? Name;
        public RoomUserCollection Users;
        public List<AriesGame> Games = new();
        public bool IsGlobal; // if a room is global, it is always open
        public int Max = 24;
        public int GameIDsCounter = 0;

        public bool AllInGame; // if session initiated between two users should extend to everyone

        public Dictionary<string, ChalIn> ChallengeMap = new();

        public AriesRoom()
        {
            Users = new RoomUserCollection(this);
        }

        public AriesRoom(int Max)
        {
            if (Max > 0)
                this.Max = Max;
            Users = new RoomUserCollection(this);
        }

        public PlusRom GetInfo()
        {
            return new PlusRom()
            {
                I = ID.ToString(),
                N = Name,
                T = Users?.Count().ToString(),
                L = Max.ToString()
            };
        }

        public AriesGame? CreateGame(int maxSize, int minSize, string custFlags, string @params,
                string name, bool priv, string seed, string sysFlags, string pass, int roomId)
        {
            lock (Games)
            {
                if (!Games.Any(game =>
                    game.Name == name))
                {
                    AriesGame game = new(maxSize, minSize, GameIDsCounter, custFlags, @params,
                                    name, priv, seed, sysFlags, pass, roomId);
                    GameIDsCounter++;
                    Games.Add(game);
                    return game;
                }
                else
                    CustomLogger.LoggerAccessor.LogWarn("[Room] - Trying to add a game while an other with same properties exists!");
            }

            return null;
        }

        public void BroadcastPopulation()
        {
            Server?.Users.Broadcast(new PlusPop() { Z = ID + "/" + Users?.Count().ToString() });
        }

        public void RemoveChallenges(AriesUser user)
        {
            lock (ChallengeMap)
            {
                foreach (var chal in ChallengeMap.Where(x => x.Value._From == user.PersonaName).ToList())
                {
                    ChallengeMap.Remove(chal.Key);
                }
            }
        }
    }
}
