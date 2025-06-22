using MultiSocks.Aries.Messages;
using System.Collections.Concurrent;

namespace MultiSocks.Aries.Model
{
    public class GameCollection
    {
        private static object _Lock = new();

        private static ConcurrentDictionary<int, bool> _IdCounter = new();
        public ConcurrentDictionary<int, AriesGame> GamesSessions = new();

        private static bool TryGetNextAvailableId(out int index)
        {
            lock (_Lock)
            {
                for (index = 1; index < int.MaxValue; ++index)
                {
                    if (_IdCounter.TryGetValue(index, out bool isUsed) && !isUsed)
                    {
                        _IdCounter[index] = true;
                        return true;
                    }
                    else if (_IdCounter.TryAdd(index, true))
                        return true;
                }
            }

            index = -1;
            return false;
        }

        private static bool TryRegisterNewId(int idToAdd)
        {
            if (idToAdd <= 0)
                return false;

            lock (_Lock)
            {
                if (!_IdCounter.ContainsKey(idToAdd))
                    return _IdCounter.TryAdd(idToAdd, true);
                else if (!_IdCounter[idToAdd])
                {
                    _IdCounter[idToAdd] = true;
                    return true;
                }
            }

            return false;
        }

        public static void UnregisterId(int idToRemove)
        {
            if (_IdCounter.ContainsKey(idToRemove))
                _IdCounter[idToRemove] = false;
        }

        public virtual AriesGame? AddGame(int maxSize, int minSize, string? custFlags, string @params,
                string name, bool priv, string? seed, string sysFlags, string? pass, int roomId)
        {
            if (!GamesSessions.Values.Any(game =>
                    game.Name == name))
            {
                if (TryGetNextAvailableId(out int GameID))
                {
                    AriesGame game = new(maxSize, minSize, GameID, custFlags, @params,
                        name, priv, seed, sysFlags, pass, roomId);
                    GamesSessions.TryAdd(game.ID, game);
                    CustomLogger.LoggerAccessor.LogInfo($"[Game] - Adding Game:{game.Name}:{game.ID}.");
                    return game;
                }
                else
                    CustomLogger.LoggerAccessor.LogError($"[Game] - Failed to register game id!");
            }
            else
                CustomLogger.LoggerAccessor.LogWarn("[Game] - Trying to add a game while an other one with the same name exists!");

            return null;
        }

        public virtual bool RemoveGame(AriesGame? game)
        {
            if (game != null && GamesSessions.ContainsKey(game.ID))
            {
                CustomLogger.LoggerAccessor.LogWarn($"[Game] - Removing Game:{game.Name}:{game.ID}.");

                foreach (AriesUser user in GamesSessions[game.ID].Users.GetAll())
                {
                    user.Connection?.SendMessage(new Gdel());

                    user.CurrentGame = null;

                    user.SendPlusWho(user, user.Connection?.Context.Project);
                }

                GamesSessions.TryRemove(game.ID, out _);

                UnregisterId(game.ID);

                game = null;

                return true;
            }

            return false;
        }

        public virtual bool TryChangeGameId(AriesGame game, int newID)
        {
            if (TryRegisterNewId(newID))
            {
                GamesSessions.TryRemove(game.ID, out _);
                game.ID = newID;
                return GamesSessions.TryAdd(newID, game);
            }
            return false;
        }

        public AriesGame? GetGameByName(string? name, string? pass)
        {
            if (string.IsNullOrEmpty(name))
                return null;

            return GamesSessions.FirstOrDefault(x => x.Value.Name == name && (string.IsNullOrEmpty(x.Value.pass) || x.Value.pass == pass)).Value;
        }

        public int Count()
        {
            return GamesSessions.Count;
        }
    }
}
