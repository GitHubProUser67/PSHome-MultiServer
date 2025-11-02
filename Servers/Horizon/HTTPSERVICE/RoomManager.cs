using Horizon.SERVER;
using MultiServerLibrary.Extension;
using Newtonsoft.Json;
using Horizon.MUM.Models;
using static Horizon.MUM.Models.Game;
using static Horizon.MUM.Models.Party;
using WebAPIService.WebServices.WebCrypto;

namespace Horizon.HTTPSERVICE
{
    public static class RoomManager
    {
        private static readonly byte[] RandSecSaltKey = ByteUtils.GenerateRandomBytes((ushort)NetObfuscator.SecSalt.Length);

        private static readonly List<Room> rooms = new List<Room>();

        public static void UpdateOrCreateRoom(string appId, string? gameName, int? gameId, string? worldId, string? accountName, int accountDmeId, string? languageType, bool host)
        {
            lock (rooms)
            {
                Room? roomToUpdate = rooms.FirstOrDefault(r => r.AppId == appId);

                if (roomToUpdate == null)
                {
                    roomToUpdate = new Room { AppId = appId, Worlds = new List<World>() };
                    rooms.Add(roomToUpdate);
                }

                if (worldId != null)
                {
                    World? worldToUpdate = roomToUpdate.Worlds?.FirstOrDefault(w => w.WorldId == worldId);

                    if (worldToUpdate == null && !string.IsNullOrEmpty(worldId))
                    {
                        worldToUpdate = new World { WorldId = worldId, GameSessions = new List<GameList>() };
                        roomToUpdate.Worlds?.Add(worldToUpdate);
                    }

                    GameList? gameToUpdate = worldToUpdate?.GameSessions?.FirstOrDefault(w => w.Name == gameName);

                    if (gameToUpdate == null && !string.IsNullOrEmpty(gameName) && gameId.HasValue)
                    {
                        gameToUpdate = new GameList { DmeWorldId = gameId.Value, Name = gameName, CreationDate = DateTime.Now.ToUniversalTime(), Clients = new List<Player>() };
                        worldToUpdate?.GameSessions?.Add(gameToUpdate);
                    }

                    Player? playerToUpdate = gameToUpdate?.Clients?.FirstOrDefault(p => p.Name == accountName);

                    if (playerToUpdate == null && !string.IsNullOrEmpty(gameToUpdate?.Name) && !string.IsNullOrEmpty(accountName) && !string.IsNullOrEmpty(languageType))
                    {
                        if (gameToUpdate.Name.Contains("AP|"))
                        {
                            Player? playerToUpdatehashed = gameToUpdate.Clients?.FirstOrDefault(p => p.Name == CipherString(accountName, HorizonServerConfiguration.MediusAPIKey));
                            if (playerToUpdatehashed == null)
                            {
                                playerToUpdate = new Player { DmeId = accountDmeId, Name = CipherString(accountName, HorizonServerConfiguration.MediusAPIKey), Languages = languageType, Host = host };
                                gameToUpdate.Clients?.Add(playerToUpdate);
                            }
                        }
                        else
                        {
                            playerToUpdate = new Player { DmeId = accountDmeId, Name = accountName, Languages = languageType, Host = host };
                            gameToUpdate.Clients?.Add(playerToUpdate);
                        }
                    }
                    else if (playerToUpdate != null)
                    {
                        playerToUpdate.Host = host;
                        playerToUpdate.Languages = languageType;
                    }
                }
            }
        }

        public static void UpdateRoomsFromChannels(List<Channel> channels)
        {
            lock (rooms)
            {
                var validWorldIds = channels.Select(c => c.Id.ToString()).Distinct().ToList();

                foreach (var channel in channels)
                {
                    string appIdStr = channel.ApplicationId.ToString();
                    string worldIdStr = channel.Id.ToString();

                    Room? room = rooms.FirstOrDefault(r => r.AppId == appIdStr);
                    if (room == null)
                    {
                        room = new Room
                        {
                            AppId = appIdStr,
                            Worlds = new List<World>()
                        };
                        rooms.Add(room);
                    }

                    room.Worlds?.RemoveAll(w => !validWorldIds.Contains(w.WorldId!));

                    World? world = room.Worlds!.FirstOrDefault(w => w.WorldId == worldIdStr);
                    if (world == null)
                    {
                        world = new World
                        {
                            WorldId = worldIdStr,
                            GameSessions = new List<GameList>()
                        };
                        room.Worlds!.Add(world);
                    }

                    var incomingGames = channel._games
                        .Select(g => g.MediusWorldId)
                        .ToList();

                    incomingGames.AddRange(channel._parties
                        .Select(g => g.MediusWorldId)
                        .ToList());

                    world.GameSessions!.RemoveAll(p => !incomingGames.Contains(p.DmeWorldId));

                    foreach (var game in channel._games)
                    {
                        GameClient[] gameClients = game.LocalClients.ToArray();

                        var incomingGameClients = gameClients.Select(c => c.DmeId).ToList();

                        GameList? gameSession = world.GameSessions.FirstOrDefault(g => g.DmeWorldId == game.MediusWorldId);
                        if (gameSession == null)
                        {
                            gameSession = new GameList
                            {
                                DmeWorldId = game.MediusWorldId,
                                Name = game.GameName,
                                CreationDate = game.utcTimeCreated,
                                Clients = new List<Player>()
                            };
                            world.GameSessions.Add(gameSession);
                        }
                        else
                            gameSession.Name = game.GameName;

                        gameSession.Clients!.RemoveAll(p => !incomingGameClients.Contains(p.DmeId));

                        foreach (var client in gameClients)
                        {
                            Player player = new Player
                            {
                                DmeId = client.DmeId,
                                Name = client.Client!.AccountName,
                                Languages = client.Client.LanguageType.ToString(),
                                Host = client.Client == game.Host
                            };

                            if (!string.IsNullOrEmpty(gameSession.Name) && gameSession.Name.Contains("AP|"))
                                player.Name = CipherString(player.Name!, HorizonServerConfiguration.MediusAPIKey);

                            var existingPlayer = gameSession.Clients!.FirstOrDefault(p => p.DmeId == player.DmeId);
                            if (existingPlayer == null)
                                gameSession.Clients!.Add(player);
                            else
                            {
                                existingPlayer.Name = player.Name;
                                existingPlayer.Languages = player.Languages;
                                existingPlayer.Host = player.Host;
                            }
                        }
                    }

                    foreach (var party in channel._parties)
                    {
                        PartyClient[] partyClients = party.LocalClients.ToArray();

                        var incomingPartyClients = partyClients.Select(c => c.DmeId).ToList();

                        GameList? gameSession = world.GameSessions.FirstOrDefault(g => g.DmeWorldId == party.MediusWorldId);
                        if (gameSession == null)
                        {
                            gameSession = new GameList
                            {
                                DmeWorldId = party.MediusWorldId,
                                Name = party.PartyName,
                                CreationDate = party.utcTimeCreated,
                                Clients = new List<Player>()
                            };
                            world.GameSessions.Add(gameSession);
                        }
                        else
                            gameSession.Name = party.PartyName;

                        gameSession.Clients!.RemoveAll(p => !incomingPartyClients.Contains(p.DmeId));

                        foreach (var client in partyClients)
                        {
                            Player player = new Player
                            {
                                DmeId = client.DmeId,
                                Name = client.Client!.AccountName,
                                Languages = client.Client.LanguageType.ToString(),
                                Host = client.Client == party.Host
                            };

                            if (!string.IsNullOrEmpty(gameSession.Name) && gameSession.Name.Contains("AP|"))
                                player.Name = CipherString(player.Name!, HorizonServerConfiguration.MediusAPIKey);

                            var existingPlayer = gameSession.Clients!.FirstOrDefault(p => p.DmeId == player.DmeId);
                            if (existingPlayer == null)
                                gameSession.Clients!.Add(player);
                            else
                            {
                                existingPlayer.Name = player.Name;
                                existingPlayer.Languages = player.Languages;
                                existingPlayer.Host = player.Host;
                            }
                        }
                    }
                }
            }
        }

        public static List<Room> GetAllRooms()
        {
            lock (rooms)
                return rooms.ToList();
        }

        public static List<KeyValuePair<string, int>> GetAllLoggedInUsers()
        {
            List<KeyValuePair<string, int>> usersList = new();

            foreach (var user in MediusClass.Manager.GetClients(0))
            {
                if (user.IsLoggedIn && !string.IsNullOrEmpty(user.AccountName))
                    usersList.Add(new KeyValuePair<string, int>(user.AccountName, user.ApplicationId));
            }

            return usersList;
        }

        public static string ToJson()
        {
            return "{\"usernames\":" + JsonConvert.SerializeObject(GetAllLoggedInUsers()) + ",\"rooms\":" + JsonConvert.SerializeObject(GetAllRooms()) + "}";
        }

        private static string CipherString(string input, string key)
        {
            int i;
            byte[] secSalt = new byte[RandSecSaltKey.Length];

            for (i = 0; i < RandSecSaltKey.Length; i++)
            {
                if (i == 0)
                    secSalt[i] = (byte)(NetObfuscator.SecSalt[i] ^ RandSecSaltKey[i] ^ (i * 2));
                else
                    secSalt[i] = (byte)(NetObfuscator.SecSalt[i] ^ RandSecSaltKey[i] ^ secSalt[i - 1]);
            }

            return $"<Secure RNG=\"{BitConverter.ToString(secSalt).Replace("-", string.Empty)}\">" + NetObfuscator.Encrypt(WebCryptoClass.EncryptCBC(input, key, WebCryptoClass.IdentIV), secSalt, (byte)key.Aggregate(0, (current, c) => current ^ c)) + "</Secure>";
        }
    }

    public class Room
    {
        public string? AppId { get; set; }
        public List<World>? Worlds { get; set; }
    }

    public class World
    {
        public string? WorldId { get; set; }
        public List<GameList>? GameSessions { get; set; }
    }

    public class GameList
    {
        public int DmeWorldId { get; set; }
        public string? Name { get; set; }
        public DateTime CreationDate { get; set; }
        public List<Player>? Clients { get; set; }
    }

    public class Player
    {
        public int DmeId { get; set; }
        public bool Host { get; set; }
        public string? Name { get; set; }
        public string? Languages { get; set; }
    }
}
