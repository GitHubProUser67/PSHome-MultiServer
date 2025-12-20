using CustomLogger;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.LIBRARY.Database.Models;
using Horizon.SERVER.PluginArgs;
using System.Data;
using Horizon.PluginManager;
using Horizon.SERVER;
using MultiServerLibrary.Extension;

namespace Horizon.MUM.Models
{
    public class Game
    {
        private object _Lock = new();
        public class GameClient
        {
            public ClientObject? Client;

            public int DmeId;
            public bool InGame;
        }

        public int MediusVersion = 0;
        public int MediusWorldId = 0;
        public int ApplicationId = 0;
        public ChannelType ChannelType = ChannelType.Game;
        public List<GameClient> LocalClients = new();
        public string? GameName;
        public string? GamePassword;
        public string? SpectatorPassword;
        public byte[] GameStats = new byte[Constants.GAMESTATS_MAXLEN];
        public MGCL_GAME_HOST_TYPE GameHostType;
        public NetAddressList? netAddressList;
        public RSA_KEY pubKey = new();
        public int AccountID;
        public int MinPlayers;
        public int MaxPlayers;
        public int GameLevel;
        public int PlayerSkillLevel;
        public int RulesSet;
        public string? Metadata;
        public int GenericField1;
        public int GenericField2;
        public int GenericField3;
        public int GenericField4;
        public int GenericField5;
        public int GenericField6;
        public int GenericField7;
        public int GenericField8;
        public int PlayerCount;
        public byte[]? RequestData;
        public uint GroupMemberListSize;
        public byte[]? GroupMemberList;
        public uint AppDataSize;
        public string? AppData;

        public MediusWorldStatus WorldStatus => _worldStatus;
        public MediusWorldAttributesType Attributes;
        public MediusMatchOptions MatchOptions;
        public ClientObject? DMEServer;
        public Channel? GameChannel;
        public ClientObject? Host;

        public string? AccountIdsAtStart => accountIdsAtStart;

        public MediusWorldStatus _worldStatus = MediusWorldStatus.WorldPendingCreation;
        public bool hasHostJoined = false;
        public DateTime utcTimeCreated;
        public DateTime utcTimeTick;
        public DateTime? utcLastJoined;
        public DateTime? utcTimeStarted;
        public DateTime? utcTimeEnded;
        public DateTime? utcTimeEmpty;

        protected string? accountIdsAtStart;

        public uint Time => (uint)(DateTimeUtils.GetHighPrecisionUtcTime() - utcTimeCreated).TotalMilliseconds;

        public bool Destroyed = false;

        public virtual bool ReadyToDestroy => !Destroyed && WorldStatus == MediusWorldStatus.WorldClosed && utcTimeEmpty.HasValue && (DateTimeUtils.GetHighPrecisionUtcTime() - utcTimeEmpty)?.TotalSeconds > 1f;

        public Game(ClientObject client, IMediusRequest createGame, ClientObject? dmeServer)
        {
            MediusVersion = client.MediusVersion;

            if (createGame is MediusCreateGameRequest r)
                FromCreateGameRequest(r);
            else if (createGame is MediusCreateGameRequest0 r0)
                FromCreateGameRequest0(r0);
            else if (createGame is MediusCreateGameRequest1 r1)
                FromCreateGameRequest1(r1);
            else if (createGame is MediusMatchCreateGameRequest r2)
                FromMatchCreateGameRequest(r2);
            else if (createGame is MediusServerCreateGameOnMeRequest r3)
                FromCreateGameOnMeRequest(r3);
            else if (createGame is MediusServerCreateGameOnSelfRequest r5)
                FromCreateGameOnSelfRequest(r5);
            else if (createGame is MediusServerCreateGameOnSelfRequest0 r6)
                FromCreateGameOnSelfRequest0(r6);

            utcTimeCreated = DateTimeUtils.GetHighPrecisionUtcTime();
            utcTimeTick = DateTimeUtils.GetHighPrecisionUtcTime();
            utcTimeEmpty = null;
            DMEServer = dmeServer;
            GameChannel!.RegisterGame(this);
            Host = client;
            SetWorldStatus(MediusWorldStatus.WorldStaging).Wait();

            LoggerAccessor.LogInfo($"Game {MediusWorldId}: {GameName}: Created by {client} | Host: {Host}");
        }

        public GameDTO ToGameDTO()
        {
            return new GameDTO()
            {
                AppId = ApplicationId,
                GameCreateDt = utcTimeCreated,
                GameEndDt = utcTimeEnded,
                GameStartDt = utcTimeStarted,
                GameHostType = GameHostType.ToString(),
                GameId = MediusWorldId,
                GameLevel = GameLevel,
                GameName = GameName,
                GameStats = GameStats,
                GenericField1 = GenericField1,
                GenericField2 = GenericField2,
                GenericField3 = GenericField3,
                GenericField4 = GenericField4,
                GenericField5 = GenericField5,
                GenericField6 = GenericField6,
                GenericField7 = GenericField7,
                GenericField8 = GenericField8,
                MaxPlayers = MaxPlayers,
                MinPlayers = MinPlayers,
                PlayerCount = PlayerCount,
                PlayerSkillLevel = PlayerSkillLevel,
                RuleSet = RulesSet,
                Metadata = Metadata,
                WorldStatus = WorldStatus.ToString(),
                PlayerListCurrent = GetActivePlayerList(),
                PlayerListStart = accountIdsAtStart,
                Destroyed = Destroyed
            };
        }

        private void FromMatchCreateGameRequest(MediusMatchCreateGameRequest createGame)
        {
            Channel gameChannel = new(createGame.ApplicationID, MediusVersion)
            {
                Name = createGame.GameName,
                MinPlayers = createGame.MinPlayers,
                MaxPlayers = createGame.MaxPlayers,
                GameLevel = createGame.GameLevel,
                PlayerSkillLevel = createGame.PlayerSkillLevel,
                GenericField1 = (ulong)createGame.GenericField1,
                GenericField2 = (ulong)createGame.GenericField2,
                GenericField3 = (ulong)createGame.GenericField3,
                GenericField4 = (ulong)createGame.GenericField4,
                Password = createGame.GamePassword,
                SecurityLevel = string.IsNullOrEmpty(createGame.GamePassword) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD,
                GameHostType = createGame.GameHostType,
                Type = ChannelType.Game
            };

            ApplicationId = createGame.ApplicationID;
            GameName = createGame.GameName;
            MinPlayers = createGame.MinPlayers;
            MaxPlayers = createGame.MaxPlayers;
            GameLevel = createGame.GameLevel;
            PlayerSkillLevel = createGame.PlayerSkillLevel;
            RulesSet = createGame.RulesSet;
            GenericField1 = createGame.GenericField1;
            GenericField2 = createGame.GenericField2;
            GenericField3 = createGame.GenericField3;
            GenericField4 = createGame.GenericField4;
            GenericField5 = createGame.GenericField5;
            GenericField6 = createGame.GenericField6;
            GenericField7 = createGame.GenericField7;
            GenericField8 = createGame.GenericField8;
            GamePassword = createGame.GamePassword;
            SpectatorPassword = createGame.SpectatorPassword;
            GameHostType = createGame.GameHostType;
            Attributes = createGame.WorldAttributesType;
            MatchOptions = createGame.MatchOptions;
            MediusWorldId = gameChannel.Id;

            GameChannel = gameChannel;

            MediusClass.Manager.AddChannel(gameChannel).Wait();
        }

        private void FromCreateGameRequest(MediusCreateGameRequest createGame)
        {
            Channel gameChannel = new(createGame.ApplicationID, MediusVersion)
            {
                Name = createGame.GameName,
                MinPlayers = createGame.MinPlayers,
                MaxPlayers = createGame.MaxPlayers,
                GameLevel = createGame.GameLevel,
                PlayerSkillLevel = createGame.PlayerSkillLevel,
                GenericField1 = (ulong)createGame.GenericField1,
                GenericField2 = (ulong)createGame.GenericField2,
                GenericField3 = (ulong)createGame.GenericField3,
                GenericField4 = (ulong)createGame.GenericField4,
                Password = createGame.GamePassword,
                SecurityLevel = string.IsNullOrEmpty(createGame.GamePassword) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD,
                GameHostType = createGame.GameHostType,
                Type = ChannelType.Game
            };

            ApplicationId = createGame.ApplicationID;
            GameName = createGame.GameName;
            MinPlayers = createGame.MinPlayers;
            MaxPlayers = createGame.MaxPlayers;
            GameLevel = createGame.GameLevel;
            PlayerSkillLevel = createGame.PlayerSkillLevel;
            RulesSet = createGame.RulesSet;
            GenericField1 = createGame.GenericField1;
            GenericField2 = createGame.GenericField2;
            GenericField3 = createGame.GenericField3;
            GenericField4 = createGame.GenericField4;
            GenericField5 = createGame.GenericField5;
            GenericField6 = createGame.GenericField6;
            GenericField7 = createGame.GenericField7;
            GenericField8 = createGame.GenericField8;
            GamePassword = createGame.GamePassword;
            SpectatorPassword = createGame.SpectatorPassword;
            GameHostType = createGame.GameHostType;
            Attributes = createGame.Attributes;
            MediusWorldId = gameChannel.Id;

            GameChannel = gameChannel;

            MediusClass.Manager.AddChannel(gameChannel).Wait();
        }

        private void FromCreateGameRequest0(MediusCreateGameRequest0 createGame)
        {
            Channel gameChannel = new(createGame.ApplicationID, MediusVersion)
            {
                Name = createGame.GameName,
                MinPlayers = createGame.MinPlayers,
                MaxPlayers = createGame.MaxPlayers,
                GameLevel = createGame.GameLevel,
                PlayerSkillLevel = createGame.PlayerSkillLevel,
                GenericField1 = (ulong)createGame.GenericField1,
                GenericField2 = (ulong)createGame.GenericField2,
                GenericField3 = (ulong)createGame.GenericField3,
                Password = createGame.GamePassword,
                SecurityLevel = string.IsNullOrEmpty(createGame.GamePassword) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD,
                GameHostType = createGame.GameHostType,
                Type = ChannelType.Game
            };

            ApplicationId = createGame.ApplicationID;
            GameName = createGame.GameName;
            MinPlayers = createGame.MinPlayers;
            MaxPlayers = createGame.MaxPlayers;
            GameLevel = createGame.GameLevel;
            PlayerSkillLevel = createGame.PlayerSkillLevel;
            RulesSet = createGame.RulesSet;
            GenericField1 = createGame.GenericField1;
            GenericField2 = createGame.GenericField2;
            GenericField3 = createGame.GenericField3;
            GamePassword = createGame.GamePassword;
            GameHostType = createGame.GameHostType;
            MediusWorldId = gameChannel.Id;

            GameChannel = gameChannel;

            MediusClass.Manager.AddChannel(gameChannel).Wait();
        }

        private void FromCreateGameRequest1(MediusCreateGameRequest1 createGame)
        {
            Channel gameChannel = new(createGame.ApplicationID, MediusVersion)
            {
                Name = createGame.GameName,
                MinPlayers = createGame.MinPlayers,
                MaxPlayers = createGame.MaxPlayers,
                GameLevel = createGame.GameLevel,
                PlayerSkillLevel = createGame.PlayerSkillLevel,
                GenericField1 = (ulong)createGame.GenericField1,
                GenericField2 = (ulong)createGame.GenericField2,
                GenericField3 = (ulong)createGame.GenericField3,
                Password = createGame.GamePassword,
                SecurityLevel = string.IsNullOrEmpty(createGame.GamePassword) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD,
                GameHostType = createGame.GameHostType,
                Type = ChannelType.Game
            };

            ApplicationId = createGame.ApplicationID;
            GameName = createGame.GameName;
            MinPlayers = createGame.MinPlayers;
            MaxPlayers = createGame.MaxPlayers;
            GameLevel = createGame.GameLevel;
            PlayerSkillLevel = createGame.PlayerSkillLevel;
            RulesSet = createGame.RulesSet;
            GenericField1 = createGame.GenericField1;
            GenericField2 = createGame.GenericField2;
            GenericField3 = createGame.GenericField3;
            GamePassword = createGame.GamePassword;
            SpectatorPassword = createGame.SpectatorPassword;
            GameHostType = createGame.GameHostType;
            MediusWorldId = gameChannel.Id;

            GameChannel = gameChannel;

            MediusClass.Manager.AddChannel(gameChannel).Wait();
        }

        private void FromCreateGameOnMeRequest(MediusServerCreateGameOnMeRequest createGame)
        {
            Channel gameChannel = new(createGame.ApplicationID, MediusVersion)
            {
                Name = createGame.GameName,
                MinPlayers = createGame.MinPlayers,
                MaxPlayers = createGame.MaxPlayers,
                GameLevel = createGame.GameLevel,
                PlayerSkillLevel = createGame.PlayerSkillLevel,
                GenericField1 = (ulong)createGame.GenericField1,
                GenericField2 = (ulong)createGame.GenericField2,
                GenericField3 = (ulong)createGame.GenericField3,
                GenericField4 = (ulong)createGame.GenericField4,
                Password = createGame.GamePassword,
                SecurityLevel = string.IsNullOrEmpty(createGame.GamePassword) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD,
                GameHostType = createGame.GameHostType,
                Type = ChannelType.Game
            };

            GameName = createGame.GameName;
            GameStats = createGame.GameStats;
            GamePassword = createGame.GamePassword;
            ApplicationId = createGame.ApplicationID;
            MaxPlayers = createGame.MaxPlayers;
            MinPlayers = createGame.MinPlayers;
            GameLevel = createGame.GameLevel;
            PlayerSkillLevel = createGame.PlayerSkillLevel;
            RulesSet = createGame.RulesSet;
            GenericField1 = createGame.GenericField1;
            GenericField2 = createGame.GenericField2;
            GenericField3 = createGame.GenericField3;
            GenericField4 = createGame.GenericField4;
            GenericField5 = createGame.GenericField5;
            GenericField6 = createGame.GenericField6;
            GenericField7 = createGame.GenericField7;
            GenericField8 = createGame.GenericField8;
            GameHostType = createGame.GameHostType;
            netAddressList = createGame.AddressList;
            MediusWorldId = gameChannel.Id;

            GameChannel = gameChannel;

            MediusClass.Manager.AddChannel(gameChannel).Wait();
        }

        private void FromCreateGameOnSelfRequest(MediusServerCreateGameOnSelfRequest createGame)
        {
            Channel gameChannel = new(createGame.ApplicationID, MediusVersion)
            {
                Name = createGame.GameName,
                MinPlayers = createGame.MinPlayers,
                MaxPlayers = createGame.MaxPlayers,
                GameLevel = createGame.GameLevel,
                PlayerSkillLevel = createGame.PlayerSkillLevel,
                GenericField1 = (ulong)createGame.GenericField1,
                GenericField2 = (ulong)createGame.GenericField2,
                GenericField3 = (ulong)createGame.GenericField3,
                Password = createGame.GamePassword,
                SecurityLevel = string.IsNullOrEmpty(createGame.GamePassword) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD,
                GameHostType = createGame.GameHostType,
                Type = ChannelType.Game
            };

            GameName = createGame.GameName;
            GameStats = createGame.GameStats;
            GamePassword = createGame.GamePassword;
            ApplicationId = createGame.ApplicationID;
            MaxPlayers = createGame.MaxPlayers;
            MinPlayers = createGame.MinPlayers;
            GameLevel = createGame.GameLevel;
            PlayerSkillLevel = createGame.PlayerSkillLevel;
            RulesSet = createGame.RulesSet;
            GenericField1 = createGame.GenericField1;
            GenericField2 = createGame.GenericField2;
            GenericField3 = createGame.GenericField3;
            GameHostType = createGame.GameHostType;
            netAddressList = createGame.AddressList;
            AccountID = createGame.AccountID;
            MediusWorldId = gameChannel.Id;

            GameChannel = gameChannel;

            MediusClass.Manager.AddChannel(gameChannel).Wait();
        }

        private void FromCreateGameOnSelfRequest0(MediusServerCreateGameOnSelfRequest0 createGame)
        {
            Channel gameChannel = new(createGame.ApplicationID, MediusVersion)
            {
                Name = createGame.GameName,
                MinPlayers = createGame.MinPlayers,
                MaxPlayers = createGame.MaxPlayers,
                GameLevel = createGame.GameLevel,
                PlayerSkillLevel = createGame.PlayerSkillLevel,
                GenericField1 = (ulong)createGame.GenericField1,
                GenericField2 = (ulong)createGame.GenericField2,
                GenericField3 = (ulong)createGame.GenericField3,
                Password = createGame.GamePassword,
                SecurityLevel = string.IsNullOrEmpty(createGame.GamePassword) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD,
                GameHostType = createGame.GameHostType,
                Type = ChannelType.Game
            };

            GameName = createGame.GameName;
            GameStats = createGame.GameStats;
            GamePassword = createGame.GamePassword;
            ApplicationId = createGame.ApplicationID;
            MaxPlayers = createGame.MaxPlayers;
            MinPlayers = createGame.MinPlayers;
            GameLevel = createGame.GameLevel;
            PlayerSkillLevel = createGame.PlayerSkillLevel;
            RulesSet = createGame.RulesSet;
            GenericField1 = createGame.GenericField1;
            GenericField2 = createGame.GenericField2;
            GenericField3 = createGame.GenericField3;
            GameHostType = createGame.GameHostType;
            netAddressList = createGame.AddressList;
            MediusWorldId = gameChannel.Id;

            GameChannel = gameChannel;

            MediusClass.Manager.AddChannel(gameChannel).Wait();
        }

        public virtual int ReassignGameMediusWorldID(MediusReassignGameMediusWorldID reassignGameMediusWorldID)
        {
            int newId = reassignGameMediusWorldID.NewMediusWorldID;
            int oldId = reassignGameMediusWorldID.OldMediusWorldID;

            // Ensure reassignedGame Old MediusWorldID matches current Game.
            if (oldId != MediusWorldId)
                return 0;
            // Ensure the Id is free to use.
            else if (!Channel.TryRegisterNewId(ApplicationId, newId, MediusVersion <= 108))
                return 0;

            MediusWorldId = GameChannel!.Id = newId;

            Channel.UnregisterId(ApplicationId, oldId);

            DMEServer?.Queue(new MediusServerMoveGameWorldOnMeRequest()
            {
                MessageID = new MessageId(),
                CurrentMediusWorldID = oldId,
                NewGameMediusWorldID = newId,
            });

            return MediusWorldId;
        }

        public virtual int ReassignGameMediusWorldID(MediusServerMoveGameWorldOnMeRequest reassignGameMediusWorldID)
        {
            int newId = reassignGameMediusWorldID.NewGameMediusWorldID;
            int oldId = reassignGameMediusWorldID.CurrentMediusWorldID;

            // Ensure reassignedGame Old MediusWorldID matches current Game.
            if (oldId != MediusWorldId)
                return 0;
            // Ensure the Id is free to use.
            else if (!Channel.TryRegisterNewId(ApplicationId, newId, MediusVersion <= 108))
                return 0;

            MediusWorldId = GameChannel!.Id = newId;

            Channel.UnregisterId(ApplicationId, oldId);

            return MediusWorldId;
        }

        public string GetActivePlayerList()
        {
            IEnumerable<string?>? playlist;

            lock (LocalClients)
                playlist = LocalClients?.Select(x => x.Client?.AccountId.ToString()).Where(x => x != null);

            if (playlist != null)
                return string.Join(",", playlist);

            return string.Empty;
        }

        public virtual async Task Tick()
        {
            // Remove timedout clients
            lock (LocalClients)
            {
                for (int i = 0; i < LocalClients.Count; ++i)
                {
                    var client = LocalClients[i];

                    if (client == null || client.Client == null || !client.Client.IsConnected || client.Client.CurrentGame?.MediusWorldId != MediusWorldId)
                    {
                        LoggerAccessor.LogWarn($"REMOVING CLIENT: {client}\n IS: {client?.Client}\nHasHostJoined: {hasHostJoined}\nIS Connected?: {client?.Client?.IsConnected}\nClient CurrentGame ID: {client?.Client?.CurrentGame?.MediusWorldId}\nGameId: {MediusWorldId}\nMatch?: {client?.Client?.CurrentGame?.MediusWorldId != MediusWorldId}");
                        LocalClients.RemoveAt(i);
                        --i;
                    }
                }
            }

            // Auto close when everyone leaves or if host fails to connect after timeout time if not a p2p game.
            if ((GameHostType != MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer) ? (!utcTimeEmpty.HasValue && !LocalClients.Any(x => x.InGame)
                && (hasHostJoined || (DateTimeUtils.GetHighPrecisionUtcTime() - utcTimeTick).TotalSeconds > MediusClass.GetAppSettingsOrDefault(ApplicationId).GameTimeoutSeconds))
                : (!utcTimeEmpty.HasValue && (DateTimeUtils.GetHighPrecisionUtcTime() - utcTimeTick).TotalSeconds > MediusClass.GetAppSettingsOrDefault(ApplicationId).GameTimeoutSeconds))
            {
                LoggerAccessor.LogWarn("AUTO CLOSING WORLD");
                utcTimeEmpty = DateTimeUtils.GetHighPrecisionUtcTime();
                await SetWorldStatus(MediusWorldStatus.WorldClosed);
            }
        }

        public virtual async Task OnMediusServerConnectNotification(MediusServerConnectNotification connectNotification)
        {
            GameClient? player;

            lock (LocalClients)
                player = LocalClients.FirstOrDefault(x => x.Client?.SessionKey == connectNotification.PlayerSessionKey);

            if (player == null)
                return;

            switch (connectNotification.ConnectEventType)
            {
                case MGCL_EVENT_TYPE.MGCL_EVENT_CLIENT_CONNECT:
                    {
                        await OnPlayerJoined(player);
                        break;
                    }
                case MGCL_EVENT_TYPE.MGCL_EVENT_CLIENT_DISCONNECT:
                    {
                        await OnPlayerLeft(player, connectNotification);
                        break;
                    }
            }
        }

        public virtual async Task OnMediusJoinGameResponse(string? Sessionkey)
        {
            if (string.IsNullOrEmpty(Sessionkey))
                return;

            GameClient? player;

            lock (LocalClients)
                player = LocalClients.FirstOrDefault(x => x.Client?.SessionKey == Sessionkey);

            if (player == null)
                return;

            await OnPlayerJoined(player);
        }

        public virtual async Task OnPlayerJoined(GameClient player)
        {
            utcLastJoined = DateTime.UtcNow;

            bool ishost = false;

            player.InGame = true;

            if (player.Client == Host)
            {
                LoggerAccessor.LogInfo($"[Game] -> OnHostJoined -> {player.Client?.ApplicationId} - {player.Client?.CurrentGame?.GameName} (id : {player.Client?.CurrentGame?.MediusWorldId}) -> {player.Client?.AccountName} -> {player.Client?.LanguageType}");
                ishost = true;
                hasHostJoined = true;
            }
            else
                LoggerAccessor.LogInfo($"[Game] -> OnPlayerJoined -> {player.Client?.ApplicationId} - {player.Client?.CurrentGame?.GameName} (id : {player.Client?.CurrentGame?.MediusWorldId}) -> {player.Client?.AccountName} -> {player.Client?.LanguageType}");

            // Send to plugins
            await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_PLAYER_ON_JOINED_GAME, new OnPlayerGameArgs() { Player = player.Client, Game = this });
        }

        public virtual void AddPlayer(ClientObject client)
        {
            lock (LocalClients)
            {
                // Don't add again
                if (LocalClients.Any(x => x.Client == client))
                    return;

                LoggerAccessor.LogInfo($"Game {MediusWorldId}: {GameName}: {client} added with sessionkey {client.SessionKey}.");

                LocalClients.Add(new GameClient()
                {
                    Client = client,
                    DmeId = client.DmeId
                });
            }

            // Inform the client of any custom game mode
            //client.CurrentChannel?.SendSystemMessage(client, $"Gamemode is {CustomGamemode?.FullName ?? "default"}.");
        }

        protected virtual async Task OnPlayerLeft(GameClient player, MediusServerConnectNotification connectNotification)
        {
            LoggerAccessor.LogInfo($"[Game] -> OnPlayerLeft -> {player.Client?.ApplicationId} - {player.Client?.CurrentGame?.GameName} (id : {player.Client?.CurrentGame?.MediusWorldId}) -> {player.Client?.AccountName} -> {player.Client?.LanguageType}");

            player.InGame = false;

            if (player.Client != null)
            {
                // Update player object
                await player.Client.LeaveGame(this);

                // Remove from collection
                if (player.Client.CurrentGame != null)
                    await RemovePlayer(player.Client, player.Client.ApplicationId, player.Client.CurrentGame?.MediusWorldId.ToString());
            }
        }

        public virtual async Task RemovePlayer(ClientObject client, int appid, string? WorldId)
        {
            bool MigrateHost = false;
            LoggerAccessor.LogInfo($"Game {MediusWorldId}: {GameName}: {client} removed.");

            // Send to plugins
            await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_PLAYER_ON_LEFT_GAME, new OnPlayerGameArgs() { Player = client, Game = this });

            // Remove host
            if (Host == client)
            {
                MigrateHost = true;

                // Send to plugins
                await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_GAME_ON_HOST_LEFT, new OnPlayerGameArgs() { Player = client, Game = this });

                Host = null;
            }

            // Remove from clients list
            lock (LocalClients)
            {
                LocalClients.RemoveAll(x => x.Client == client);
				
                if (LocalClients.Count == 0)
                    EndGame(appid).Wait();
                else if (MigrateHost && MediusVersion >= 109)
                    Host = LocalClients.FirstOrDefault()?.Client;
            }
        }

        public virtual async Task OnEndGameReport(MediusEndGameReport report, int appid)
        {
            try
            {
                // Send database EndGameReport info
                await EndGame(appid);

                LoggerAccessor.LogInfo($"Successful local delete of game world [{report.MediusWorldID}]");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogWarn($"Couldn't perform local delete of game world [{report.MediusWorldID}] with exception: {e}");
            }
        }

        public virtual async Task OnWorldReport(MediusWorldReport report, int appId)
        {
            // Ensure report is for correct game world
            if (report.MediusWorldID != MediusWorldId)
                return;

            utcTimeTick = DateTimeUtils.GetHighPrecisionUtcTime();

            string? previousGameName = GameName;

            if (appId == 24180)
                report.MaxPlayers = 10;

            GameName = report.GameName;
            GameStats = report.GameStats;
            MinPlayers = report.MinPlayers;
            MaxPlayers = report.MaxPlayers;
            GameLevel = report.GameLevel;
            PlayerSkillLevel = report.PlayerSkillLevel;
            RulesSet = report.RulesSet;
            GenericField1 = report.GenericField1;
            GenericField2 = report.GenericField2;
            GenericField3 = report.GenericField3;
            GenericField4 = report.GenericField4;
            GenericField5 = report.GenericField5;
            GenericField6 = report.GenericField6;
            GenericField7 = report.GenericField7;
            GenericField8 = report.GenericField8;
            // Some games codding are so poor that they not find useful to feed the player count info, GREAT!
            if (GameHostType != MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer)
            {
                lock (LocalClients)
                    PlayerCount = LocalClients.Count(x => x != null && x.Client != null && x.Client.IsConnected && x.InGame);
            }
            else
                PlayerCount = report.PlayerCount;

            // Update Game channel too
            if (GameChannel != null)
            {
                GameChannel.Name = report.GameName;
                GameChannel.MinPlayers = report.MinPlayers;
                GameChannel.MaxPlayers = report.MaxPlayers;
                GameChannel.GameLevel = report.GameLevel;
                GameChannel.RulesSet = report.RulesSet;
                GameChannel.PlayerSkillLevel = report.PlayerSkillLevel;
                GameChannel.GenericField1 = (ulong)report.GenericField1;
                GameChannel.GenericField2 = (ulong)report.GenericField2;
                GameChannel.GenericField3 = (ulong)report.GenericField3;
                GameChannel.GenericField4 = (ulong)report.GenericField4;
                GameChannel.WorldStatus = report.WorldStatus;
            }

            // Once the world has been closed then we force it closed.
            // This is because when the host hits 'Play Again' they tell the server the world has closed (EndGameReport)
            // but the existing clients tell the server the world is still active.
            // This gives the host a "Game Name Already Exists" when they try to remake with the same name.
            // This just fixes that. At the cost of the game not showing after a host leaves a game.
            if (WorldStatus != MediusWorldStatus.WorldClosed && WorldStatus != report.WorldStatus)
                await SetWorldStatus(report.WorldStatus);
            else
            {
                // Update db
                if (!utcTimeEnded.HasValue)
                    _ = HorizonServerConfiguration.Database.UpdateGame(ToGameDTO());
            }
        }

        public virtual async Task OnWorldReport0(MediusWorldReport0 report)
        {
            // Ensure report is for correct game world
            if (report.MediusWorldID != MediusWorldId)
                return;

            utcTimeTick = DateTimeUtils.GetHighPrecisionUtcTime();

            string? previousGameName = GameName;

            GameName = report.GameName;
            GameStats = report.GameStats;
            MinPlayers = report.MinPlayers;
            MaxPlayers = report.MaxPlayers;
            GameLevel = report.GameLevel;
            PlayerSkillLevel = report.PlayerSkillLevel;
            RulesSet = report.RulesSet;
            GenericField1 = report.GenericField1;
            GenericField2 = report.GenericField2;
            GenericField3 = report.GenericField3;
            // Some games codding are so poor that they not find useful to feed the player count info, GREAT!
            if (GameHostType != MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer)
            {
                lock (LocalClients)
                    PlayerCount = LocalClients.Count(x => x != null && x.Client != null && x.Client.IsConnected && x.InGame);
            }
            else
                PlayerCount = report.PlayerCount;

            // Update Game channel too
            if (GameChannel != null)
            {
                GameChannel.Name = report.GameName;
                GameChannel.MinPlayers = report.MinPlayers;
                GameChannel.MaxPlayers = report.MaxPlayers;
                GameChannel.GameLevel = report.GameLevel;
                GameChannel.RulesSet = report.RulesSet;
                GameChannel.PlayerSkillLevel = report.PlayerSkillLevel;
                GameChannel.GenericField1 = (ulong)report.GenericField1;
                GameChannel.GenericField2 = (ulong)report.GenericField2;
                GameChannel.GenericField3 = (ulong)report.GenericField3;
                GameChannel.WorldStatus = report.WorldStatus;
            }

            // Once the world has been closed then we force it closed.
            // This is because when the host hits 'Play Again' they tell the server the world has closed (EndGameReport)
            // but the existing clients tell the server the world is still active.
            // This gives the host a "Game Name Already Exists" when they try to remake with the same name.
            // This just fixes that. At the cost of the game not showing after a host leaves a game.
            if (WorldStatus != MediusWorldStatus.WorldClosed && WorldStatus != report.WorldStatus)
                await SetWorldStatus(report.WorldStatus);
            else
            {
                // Update db
                if (!utcTimeEnded.HasValue)
                    _ = HorizonServerConfiguration.Database.UpdateGame(ToGameDTO());
            }
        }

        public virtual async Task OnWorldReportOnMe(MediusServerWorldReportOnMe report)
        {
            // Ensure report is for correct game world
            if (report.MediusWorldID != MediusWorldId)
                return;

            utcTimeTick = DateTimeUtils.GetHighPrecisionUtcTime();

            string? previousGameName = GameName;

            ApplicationId = report.ApplicationID;
            GameName = report.GameName;
            GameStats = report.GameStats;
            MinPlayers = report.MinPlayers;
            MaxPlayers = report.MaxPlayers;
            GameLevel = report.GameLevel;
            PlayerSkillLevel = report.PlayerSkillLevel;
            RulesSet = report.RulesSet;
            GenericField1 = report.GenericField1;
            GenericField2 = report.GenericField2;
            GenericField3 = report.GenericField3;
            GenericField4 = report.GenericField4;
            GenericField5 = report.GenericField5;
            GenericField6 = report.GenericField6;
            GenericField7 = report.GenericField7;
            GenericField8 = report.GenericField8;
            // Some games codding are so poor that they not find useful to feed the player count info, GREAT!
            if (GameHostType != MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer)
            {
                lock (LocalClients)
                    PlayerCount = LocalClients.Count(x => x != null && x.Client != null && x.Client.IsConnected && x.InGame);
            }
            else
                PlayerCount = report.PlayerCount;

            // Update Game channel too
            if (GameChannel != null)
            {
                GameChannel.Name = report.GameName;
                GameChannel.MinPlayers = report.MinPlayers;
                GameChannel.MaxPlayers = report.MaxPlayers;
                GameChannel.GameLevel = report.GameLevel;
                GameChannel.RulesSet = report.RulesSet;
                GameChannel.PlayerSkillLevel = report.PlayerSkillLevel;
                GameChannel.GenericField1 = (ulong)report.GenericField1;
                GameChannel.GenericField2 = (ulong)report.GenericField2;
                GameChannel.GenericField3 = (ulong)report.GenericField3;
                GameChannel.GenericField4 = (ulong)report.GenericField4;
                GameChannel.WorldStatus = report.WorldStatus;
            }

            // Once the world has been closed then we force it closed.
            // This is because when the host hits 'Play Again' they tell the server the world has closed (EndGameReport)
            // but the existing clients tell the server the world is still active.
            // This gives the host a "Game Name Already Exists" when they try to remake with the same name.
            // This just fixes that. At the cost of the game not showing after a host leaves a game.
            if (WorldStatus != MediusWorldStatus.WorldClosed && WorldStatus != report.WorldStatus)
                await SetWorldStatus(report.WorldStatus);
            else
            {
                // Update db
                if (!utcTimeEnded.HasValue)
                    _ = HorizonServerConfiguration.Database.UpdateGame(ToGameDTO());
            }
        }

        public virtual Task GameCreated()
        {
            return Task.CompletedTask;
        }

        public virtual Task EndGame(int appid)
        {
            lock (_Lock)
            {
                if (Destroyed)
                    return Task.CompletedTask;

                LoggerAccessor.LogInfo($"Game {MediusWorldId}: {GameName}: EndGame() called.");

                // Send to plugins
                MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_GAME_ON_DESTROYED, new OnGameArgs() { Game = this }).Wait();

                // Remove players from game world
                lock (LocalClients)
                {
                    while (LocalClients.Count > 0)
                    {
                        ClientObject? client = LocalClients[0].Client;
                        if (client == null)
                            LocalClients.RemoveAt(0);
                        else
                            client.LeaveGame(this).Wait();
                    }
                }

                // Unregister from channel
				GameChannel?.UnregisterGame(this);

                // Send end game
                DMEServer?.Queue(new MediusServerEndGameRequest()
                {
                    MediusWorldID = MediusWorldId,
                    BrutalFlag = false
                });

                // destroy flag
                Destroyed = true;

                // Delete db entry if game hasn't started
                // Otherwise do a final update
                if (!utcTimeStarted.HasValue)
                    _ = HorizonServerConfiguration.Database.DeleteGame(MediusWorldId);
                else
                    _ = HorizonServerConfiguration.Database.UpdateGame(ToGameDTO());
            }

            return Task.CompletedTask;
        }

        public virtual async Task SetWorldStatus(MediusWorldStatus status)
        {
            if (WorldStatus == status)
                return;

            _worldStatus = status;

            switch (status)
            {
                case MediusWorldStatus.WorldActive:
                    {
                        utcTimeStarted = DateTimeUtils.GetHighPrecisionUtcTime();
                        accountIdsAtStart = GetActivePlayerList();

                        // Send to plugins
                        await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_GAME_ON_STARTED, new OnGameArgs() { Game = this });
                        break;
                    }
                case MediusWorldStatus.WorldClosed:
                    {
                        utcTimeEnded = DateTimeUtils.GetHighPrecisionUtcTime();

                        // Send to plugins
                        await MediusClass.Plugins.OnEvent(PluginEvent.MEDIUS_GAME_ON_ENDED, new OnGameArgs() { Game = this });

                        return;
                    }
            }

            // Update db
            if (!utcTimeEnded.HasValue)
                _ = HorizonServerConfiguration.Database.UpdateGame(ToGameDTO());
        }
    }
}
