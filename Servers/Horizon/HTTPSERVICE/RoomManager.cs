using Horizon.LIBRARY.Database.Entities;
using Horizon.MEDIUS;
using Horizon.MUM.Models;
using MultiServerLibrary.Extension;
using Newtonsoft.Json;
using WebAPIService.WebServices.WebCrypto;

namespace Horizon.HTTPSERVICE
{
    public static class RoomManager
    {
        private static readonly byte[] RandSecSaltKey = ByteUtils.GenerateRandomBytes(
            (ushort)NetObfuscator.SecSalt.Length
        );

        private static readonly List<Room> rooms = new();

        public static void CreateRoom(string appId)
        {
            lock (rooms)
            {
                var roomToUpdate = rooms.FirstOrDefault(r => r.AppId == appId);

                if (roomToUpdate == null)
                {
                    roomToUpdate = new Room { AppId = appId, Worlds = new List<World>() };
                    rooms.Add(roomToUpdate);
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
                    var appIdStr = channel.ApplicationId.ToString();
                    var worldIdStr = channel.Id.ToString();

                    var room = rooms.FirstOrDefault(r => r.AppId == appIdStr);
                    if (room == null)
                    {
                        room = new Room { AppId = appIdStr, Worlds = new List<World>() };
                        rooms.Add(room);
                    }

                    room.Worlds?.RemoveAll(w => !validWorldIds.Contains(w.WorldId!));

                    var world = room.Worlds!.FirstOrDefault(w => w.WorldId == worldIdStr);
                    if (world == null)
                    {
                        world = new World
                        {
                            WorldId = worldIdStr,
                            GameSessions = new List<GameList>(),
                        };
                        room.Worlds!.Add(world);
                    }

                    world.PlayerSkillLevel = channel.PlayerSkillLevel;
                    world.GenericField1 = channel.GenericField1;
                    world.GenericField2 = channel.GenericField2;
                    world.GenericField3 = channel.GenericField3;
                    world.GenericField4 = channel.GenericField4;
                    world.WorldStatus = (int)channel.WorldStatus;
                    world.NumOfGamesChannel = channel.GameCount;

                    bool hasGame = channel.Game != null,
                        hasParty = channel.Party != null;
                    var incomingGames = new List<int>();

                    if (hasGame)
                        incomingGames.Add(channel.Game!.MediusWorldId);
                    if (hasParty)
                        incomingGames.Add(channel.Party!.MediusWorldId);

                    world.GameSessions!.RemoveAll(p => !incomingGames.Contains(p.DmeWorldId));

                    if (hasGame)
                    {
                        var game = channel.Game;

                        var gameClients = game!.LocalClients.ToArray();

                        var incomingGameClients = gameClients.Select(c => c.DmeId).ToList();

                        var gameSession = world.GameSessions.FirstOrDefault(g =>
                            g.DmeWorldId == game.MediusWorldId
                        );
                        if (gameSession == null)
                        {
                            gameSession = new GameList
                            {
                                DmeWorldId = game.MediusWorldId,
                                Name = game.GameName,
                                CreationDate = game.utcTimeCreated,
                                RulesSet = game.RulesSet,
                                GameLevel = game.GameLevel,
                                PlayerSkillLevel = game.PlayerSkillLevel,
                                MinPlayers = game.MinPlayers,
                                MaxPlayers = game.MaxPlayers,
                                GenericField1 = game.GenericField1,
                                GenericField2 = game.GenericField2,
                                GenericField3 = game.GenericField3,
                                GenericField4 = game.GenericField4,
                                GenericField5 = game.GenericField5,
                                GenericField6 = game.GenericField6,
                                GenericField7 = game.GenericField7,
                                GenericField8 = game.GenericField8,
                                WorldStatus = (int)game.WorldStatus,
                                Clients = new List<Player>(),
                            };
                            world.GameSessions.Add(gameSession);
                        }
                        else
                            gameSession.Name = game.GameName;

                        gameSession.Clients!.RemoveAll(p => !incomingGameClients.Contains(p.DmeId));

                        foreach (var client in gameClients)
                        {
                            var player = new Player
                            {
                                DmeId = client.DmeId,
                                Name = client.Client!.AccountName,
                                Languages = client.Client.LanguageType.ToString(),
                                Host = client.Client == game.Host,
                            };

                            if (
                                !string.IsNullOrEmpty(gameSession.Name)
                                && gameSession.Name.Contains("AP|")
                            )
                                player.Name = CipherString(
                                    player.Name!,
                                    HorizonServerConfiguration.MEDIUSAPIKey
                                );

                            var existingPlayer = gameSession.Clients!.FirstOrDefault(p =>
                                p.DmeId == player.DmeId
                            );
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

                    if (hasParty)
                    {
                        var party = channel.Party;

                        var partyClients = party!.LocalClients.ToArray();

                        var incomingPartyClients = partyClients.Select(c => c.DmeId).ToList();

                        var gameSession = world.GameSessions.FirstOrDefault(g =>
                            g.DmeWorldId == party.MediusWorldId
                        );
                        if (gameSession == null)
                        {
                            gameSession = new GameList
                            {
                                DmeWorldId = party.MediusWorldId,
                                Name = party.PartyName,
                                CreationDate = party.utcTimeCreated,
                                RulesSet = party.RulesSet,
                                GameLevel = party.PartyLevel,
                                PlayerSkillLevel = party.PlayerSkillLevel,
                                MinPlayers = party.MinPlayers,
                                MaxPlayers = party.MaxPlayers,
                                GenericField1 = party.GenericField1,
                                GenericField2 = party.GenericField2,
                                GenericField3 = party.GenericField3,
                                GenericField4 = party.GenericField4,
                                GenericField5 = party.GenericField5,
                                GenericField6 = party.GenericField6,
                                GenericField7 = party.GenericField7,
                                GenericField8 = party.GenericField8,
                                WorldStatus = (int)party.WorldStatus,
                                Clients = new List<Player>(),
                            };
                            world.GameSessions.Add(gameSession);
                        }
                        else
                            gameSession.Name = party.PartyName;

                        gameSession.Clients!.RemoveAll(p =>
                            !incomingPartyClients.Contains(p.DmeId)
                        );

                        foreach (var client in partyClients)
                        {
                            var player = new Player
                            {
                                DmeId = client.DmeId,
                                Name = client.Client!.AccountName,
                                Languages = client.Client.LanguageType.ToString(),
                                Host = client.Client == party.Host,
                            };

                            if (
                                !string.IsNullOrEmpty(gameSession.Name)
                                && gameSession.Name.Contains("AP|")
                            )
                                player.Name = CipherString(
                                    player.Name!,
                                    HorizonServerConfiguration.MEDIUSAPIKey
                                );

                            var existingPlayer = gameSession.Clients!.FirstOrDefault(p =>
                                p.DmeId == player.DmeId
                            );
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

            foreach (var user in Program.MUMManager.GetClients(0))
            {
                if (user.IsLoggedIn && !string.IsNullOrEmpty(user.AccountName))
                    usersList.Add(
                        new KeyValuePair<string, int>(user.AccountName, user.ApplicationId)
                    );
            }

            return usersList;
        }

        public static string ToJson()
        {
            return "{\"usernames\":"
                + JsonConvert.SerializeObject(GetAllLoggedInUsers())
                + ",\"rooms\":"
                + JsonConvert.SerializeObject(GetAllRooms())
                + "}";
        }

        private static string CipherString(string input, string key)
        {
            int i;
            var secSalt = new byte[RandSecSaltKey.Length];

            for (i = 0; i < RandSecSaltKey.Length; i++)
            {
                secSalt[i] =
                    i == 0
                        ? (byte)(NetObfuscator.SecSalt[i] ^ RandSecSaltKey[i] ^ (i * 2))
                        : (byte)(NetObfuscator.SecSalt[i] ^ RandSecSaltKey[i] ^ secSalt[i - 1]);
            }

            return $"<Secure RNG=\"{BitConverter.ToString(secSalt).Replace("-", string.Empty)}\">"
                + NetObfuscator.Encrypt(
                    WebCryptoClass.EncryptCBC(input, key, WebCryptoClass.IdentIV),
                    secSalt,
                    (byte)key.Aggregate(0, (current, c) => current ^ c)
                )
                + "</Secure>";
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
        public int PlayerSkillLevel { get; set; }
        public int RulesSet { get; set; }
        public ulong GenericField1 { get; set; }
        public ulong GenericField2 { get; set; }
        public ulong GenericField3 { get; set; }
        public ulong GenericField4 { get; set; }
        public int WorldStatus { get; set; }
        public int NumOfGamesChannel { get; set; }
        public List<GameList>? GameSessions { get; set; }
    }

    public class GameList
    {
        public int DmeWorldId { get; set; }
        public string? Name { get; set; }
        public DateTime CreationDate { get; set; }
        public int GameLevel { get; set; }
        public int PlayerSkillLevel { get; set; }
        public int RulesSet { get; set; }
        public int MinPlayers { get; set; }
        public int MaxPlayers { get; set; }
        public int GenericField1 { get; set; }
        public int GenericField2 { get; set; }
        public int GenericField3 { get; set; }
        public int GenericField4 { get; set; }
        public int GenericField5 { get; set; }
        public int GenericField6 { get; set; }
        public int GenericField7 { get; set; }
        public int GenericField8 { get; set; }
        public int WorldStatus { get; set; }
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
