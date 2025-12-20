using CustomLogger;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.LIBRARY.Database.Models;
using System.Data;
using Horizon.SERVER;
using MultiServerLibrary.Extension;

namespace Horizon.MUM.Models
{
    public class Party
    {
        private object _Lock = new();
        public class PartyClient
        {
            public ClientObject? Client;

            public int DmeId;
            public bool InGame;
        }

        public int MediusVersion = 0;
        public int MediusWorldId = 0;
        public int ApplicationId = 0;
        public ChannelType ChannelType = ChannelType.Game;
        public List<PartyClient> LocalClients = new();
        public string? PartyName;
        public string? PartyPassword;
        public string? SpectatorPassword;
        public byte[] PartyStats = new byte[Constants.GAMESTATS_MAXLEN];
        public MGCL_GAME_HOST_TYPE PartyHostType;
        public NetAddressList? netAddressList;
        public RSA_KEY pubKey = new();
        public int AccountID;
        public int MinPlayers;
        public int MaxPlayers;
        public int PartyLevel;
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

        public Party(ClientObject client, IMediusRequest createParty, ClientObject? dmeServer)
        {
            MediusVersion = client.MediusVersion;

            if (createParty is MediusPartyCreateRequest r)
                FromPartyCreateRequest(r);

            utcTimeCreated = DateTimeUtils.GetHighPrecisionUtcTime();
            utcTimeTick = DateTimeUtils.GetHighPrecisionUtcTime();
            utcTimeEmpty = null;
            DMEServer = dmeServer;
            GameChannel!.RegisterParty(this);
            Host = client;
            SetWorldStatus(MediusWorldStatus.WorldStaging).Wait();

            LoggerAccessor.LogInfo($"Party {MediusWorldId}: {PartyName}: Created by {client} | Host: {Host}");
        }

        public PartyDTO ToPartyDTO()
        {
            return new PartyDTO()
            {
                AppId = ApplicationId,
                PartyCreateDt = utcTimeCreated,
                PartyEndDt = utcTimeEnded,
                PartyStartDt = utcTimeStarted,
                GameHostType = PartyHostType.ToString(),
                PartyId = MediusWorldId,
                PartyName = PartyName,
                PartyPassword = PartyPassword,
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
                Metadata = Metadata,
                Destroyed = Destroyed
            };
        }

        private void FromPartyCreateRequest(MediusPartyCreateRequest partyCreate)
        {
            Channel gameChannel = new(partyCreate.ApplicationID, MediusVersion)
            {
                ApplicationId = partyCreate.ApplicationID,
                Name = partyCreate.PartyName,
                MinPlayers = partyCreate.MinPlayers,
                MaxPlayers = partyCreate.MaxPlayers,
                GenericField1 = (ulong)partyCreate.GenericField1,
                GenericField2 = (ulong)partyCreate.GenericField2,
                GenericField3 = (ulong)partyCreate.GenericField3,
                GenericField4 = (ulong)partyCreate.GenericField4,
                Password = partyCreate.PartyPassword,
                SecurityLevel = string.IsNullOrEmpty(partyCreate.PartyPassword) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD,
                GameHostType = partyCreate.PartyHostType,
                Type = ChannelType.Game
            };

            ApplicationId = partyCreate.ApplicationID;
            PartyName = partyCreate.PartyName;
            PartyPassword = partyCreate.PartyPassword;
            MinPlayers = partyCreate.MinPlayers;
            MaxPlayers = partyCreate.MaxPlayers;
            GenericField1 = partyCreate.GenericField1;
            GenericField2 = partyCreate.GenericField2;
            GenericField3 = partyCreate.GenericField3;
            GenericField4 = partyCreate.GenericField4;
            GenericField5 = partyCreate.GenericField5;
            GenericField6 = partyCreate.GenericField6;
            GenericField7 = partyCreate.GenericField7;
            GenericField8 = partyCreate.GenericField8;
            PartyHostType = partyCreate.PartyHostType;
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

                    if (client == null || client.Client == null || !client.Client.IsConnected || client.Client.CurrentParty?.MediusWorldId != MediusWorldId)
                    {
                        LoggerAccessor.LogWarn($"REMOVING CLIENT: {client}\n IS: {client?.Client}\nHasHostJoined: {hasHostJoined}\nIS Connected?: {client?.Client?.IsConnected}\nClient CurrentParty ID: {client?.Client?.CurrentParty?.MediusWorldId}\nGameId: {MediusWorldId}\nMatch?: {client?.Client?.CurrentParty?.MediusWorldId != MediusWorldId}");
                        LocalClients.RemoveAt(i);
                        --i;
                    }
                }
            }

            // Auto close when everyone leaves or if host fails to connect after timeout time if not a p2p game.
            if ((PartyHostType != MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer) ? (!utcTimeEmpty.HasValue && !LocalClients.Any(x => x.InGame)
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
            PartyClient? player;

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

        public virtual Task OnPlayerJoined(PartyClient player)
        {
            utcLastJoined = DateTime.UtcNow;

            bool ishost = false;

            player.InGame = true;

            if (player.Client == Host)
            {
                LoggerAccessor.LogInfo($"[Party] -> OnHostJoined -> {player.Client?.ApplicationId} - {player.Client?.CurrentParty?.PartyName} (id : {player.Client?.CurrentParty?.MediusWorldId}) -> {player.Client?.AccountName} -> {player.Client?.LanguageType}");
                ishost = true;
                hasHostJoined = true;
            }
            else
                LoggerAccessor.LogInfo($"[Party] -> OnPlayerJoined -> {player.Client?.ApplicationId} - {player.Client?.CurrentParty?.PartyName} (id : {player.Client?.CurrentParty?.MediusWorldId}) -> {player.Client?.AccountName} -> {player.Client?.LanguageType}");

            return Task.CompletedTask;
        }

        public virtual void AddPlayer(ClientObject client)
        {
            lock (LocalClients)
            {
                // Don't add again
                if (LocalClients.Any(x => x.Client == client))
                    return;

                LoggerAccessor.LogInfo($"Party {MediusWorldId}: {PartyName}: {client} added with sessionkey {client.SessionKey}.");

                LocalClients.Add(new PartyClient()
                {
                    Client = client,
                    DmeId = client.DmeId
                });
            }

            // Inform the client of any custom game mode
            //client.CurrentChannel?.SendSystemMessage(client, $"Gamemode is {CustomGamemode?.FullName ?? "default"}.");
        }

        protected virtual async Task OnPlayerLeft(PartyClient player, MediusServerConnectNotification connectNotification)
        {
            LoggerAccessor.LogInfo($"[Party] -> OnPlayerLeft -> {player.Client?.ApplicationId} - {player.Client?.CurrentParty?.PartyName} (id : {player.Client?.CurrentParty?.MediusWorldId}) -> {player.Client?.AccountName} -> {player.Client?.LanguageType}");

            player.InGame = false;

            if (player.Client != null)
            {
                // Update player object
                await player.Client.LeaveParty(this);

                // Remove from collection
                if (player.Client.CurrentParty != null)
                    await RemovePlayer(player.Client, player.Client.ApplicationId, player.Client.CurrentParty?.MediusWorldId.ToString());
            }
        }

        public virtual Task RemovePlayer(ClientObject client, int appid, string? WorldId)
        {
            bool MigrateHost = false;
            LoggerAccessor.LogInfo($"Party {MediusWorldId}: {PartyName}: {client} removed.");

            // Remove host
            if (Host == client)
            {
                MigrateHost = true;

                Host = null;
            }

            // Remove from clients list
            lock (LocalClients)
            {
                LocalClients.RemoveAll(x => x.Client == client);

                if (LocalClients.Count == 0)
                    EndParty(appid).Wait();
                else if (MigrateHost && MediusVersion >= 109)
                    Host = LocalClients.FirstOrDefault()?.Client;
            }

            return Task.CompletedTask;
        }

        public virtual async Task OnEndPartyReport(MediusEndGameReport report, int appid)
        {
            try
            {
                // Send database EndGameReport info
                await EndParty(appid);

                LoggerAccessor.LogInfo($"Successful local delete of game world [{report.MediusWorldID}]");
            }
            catch (Exception e)
            {
                LoggerAccessor.LogWarn($"Couldn't perform local delete of game world [{report.MediusWorldID}] with exception: {e}");
            }
        }

        public virtual void OnPartyPlayerReport(MediusPartyPlayerReport report)
        {
            
        }

        public virtual async Task OnWorldReport(MediusWorldReport report, int appId)
        {
            // Ensure report is for correct game world
            if (report.MediusWorldID != MediusWorldId)
                return;

            utcTimeTick = DateTimeUtils.GetHighPrecisionUtcTime();

            string? previousGameName = PartyName;

            PartyName = report.GameName;
            PartyStats = report.GameStats;
            MinPlayers = report.MinPlayers;
            MaxPlayers = report.MaxPlayers;
            PartyLevel = report.GameLevel;
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
            if (PartyHostType != MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer)
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
                    _ = HorizonServerConfiguration.Database.UpdateParty(ToPartyDTO());
            }
        }

        public virtual async Task OnWorldReportOnMe(MediusServerWorldReportOnMe report)
        {
            // Ensure report is for correct game world
            if (report.MediusWorldID != MediusWorldId)
                return;

            utcTimeTick = DateTimeUtils.GetHighPrecisionUtcTime();

            string? previousGameName = PartyName;

            ApplicationId = report.ApplicationID;
            PartyName = report.GameName;
            PartyStats = report.GameStats;
            MinPlayers = report.MinPlayers;
            MaxPlayers = report.MaxPlayers;
            PartyLevel = report.GameLevel;
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
            if (PartyHostType != MGCL_GAME_HOST_TYPE.MGCLGameHostPeerToPeer)
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
                    _ = HorizonServerConfiguration.Database.UpdateParty(ToPartyDTO());
            }
        }

        public virtual Task PartyCreated()
        {
            return Task.CompletedTask;
        }

        public virtual Task EndParty(int appid)
        {
            lock (_Lock)
            {
                if (Destroyed)
                    return Task.CompletedTask;

                LoggerAccessor.LogInfo($"Party {MediusWorldId}: {PartyName}: EndParty() called.");

                // Remove players from game world
                lock (LocalClients)
                {
                    while (LocalClients.Count > 0)
                    {
                        ClientObject? client = LocalClients[0].Client;
                        if (client == null)
                            LocalClients.RemoveAt(0);
                        else
                            client.LeaveParty(this).Wait();
                    }
                }

                // Unregister from channel
                GameChannel?.UnregisterParty(this);

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
                    _ = HorizonServerConfiguration.Database.UpdateParty(ToPartyDTO());
            }

            return Task.CompletedTask;
        }

        public virtual Task SetWorldStatus(MediusWorldStatus status)
        {
            if (WorldStatus == status)
                return Task.CompletedTask;

            _worldStatus = status;

            switch (status)
            {
                case MediusWorldStatus.WorldActive:
                    {
                        utcTimeStarted = DateTimeUtils.GetHighPrecisionUtcTime();
                        accountIdsAtStart = GetActivePlayerList();

                        break;
                    }
                case MediusWorldStatus.WorldClosed:
                    {
                        utcTimeEnded = DateTimeUtils.GetHighPrecisionUtcTime();

                        return Task.CompletedTask;
                    }
            }

            // Update db
            if (!utcTimeEnded.HasValue)
                _ = HorizonServerConfiguration.Database.UpdateParty(ToPartyDTO());

            return Task.CompletedTask;
        }
    }
}
