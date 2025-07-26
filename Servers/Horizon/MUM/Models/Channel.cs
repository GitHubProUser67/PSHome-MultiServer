using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.SERVER;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using CustomLogger;
using MultiServerLibrary.Extension;

namespace Horizon.MUM.Models
{
    public enum ChannelType
    {
        Lobby,
        Game
    }

    public class Channel
    {
        [JsonIgnore]
        private static object _Lock = new();

        [JsonIgnore]
        private static ConcurrentDictionary<int, ConcurrentDictionary<int, bool>> _IdCounter = new();

        [JsonIgnore]
        public ConcurrentList<ClientObject> LocalClients = new();
        [JsonIgnore]
        public ConcurrentList<Game> _games = new();
        [JsonIgnore]
        public ConcurrentList<Party> _parties = new();

        public ConcurrentList<Channel> LocalChannels = new();

        public string LobbyIp = MediusClass.SERVER_IP.ToString();
        public string RegionCode = MultiServerLibrary.GeoLocalization.GeoIP.GetGeoCodeFromIP(MediusClass.SERVER_IP) ?? string.Empty;
        public int LobbyPort = MediusClass.LobbyServer.TCPPort;
        public int Id = 0;
        public int ApplicationId = 0;
        public int MediusVersion = 0;
        public ChannelType Type = ChannelType.Lobby;
        public string Name = "MediusLobby";
        public string? Password = null;
        public int MinPlayers = 1;
        public int MaxPlayers = 10;
        public int GameLevel = 0;
        public int PlayerSkillLevel = 0;
        public int RulesSet = 0;
        public MediusApplicationType AppType = MediusApplicationType.LobbyChatChannel;
        public MediusWorldSecurityLevelType SecurityLevel = MediusWorldSecurityLevelType.WORLD_SECURITY_NONE;
        public MediusLobbyFilterMaskLevelType LobbyFilterMaskLevelType = MediusLobbyFilterMaskLevelType.MediusLobbyFilterMaskLevel0;
        public ulong GenericField1 = 0;
        public ulong GenericField2 = 0;
        public ulong GenericField3 = 0;
        public ulong GenericField4 = 0;
        public MediusWorldGenericFieldLevelType GenericFieldLevel = MediusWorldGenericFieldLevelType.MediusWorldGenericFieldLevel0;
        public MGCL_GAME_HOST_TYPE GameHostType;
        public MediusWorldStatus WorldStatus;

        protected bool _removeChannel = false;
        public DateTime _timeCreated = DateTimeUtils.GetHighPrecisionUtcTime();

        public virtual bool ReadyToDestroy => Type == ChannelType.Game && (_removeChannel || ((DateTimeUtils.GetHighPrecisionUtcTime() - _timeCreated).TotalSeconds > MediusClass.GetAppSettingsOrDefault(ApplicationId).GameTimeoutSeconds) && GameCount == 0 && PartyCount == 0);
        public virtual int PlayerCount => LocalClients.Count;
        public virtual int GameCount => _games.Count;
        public virtual int PartyCount => _parties.Count;

        private static bool InitializeAppId(int ApplicationId, bool Pre108)
        {
            if (_IdCounter.TryAdd(ApplicationId, new ConcurrentDictionary<int, bool> { }))
            {
                if (Pre108)
                {
                    // populate collection for Pre108 list as an optimization.
                    for (byte i = 1; i < 255; ++i)
                        _IdCounter[ApplicationId].TryAdd(i, false);
                }

                return true;
            }

            return false;
        }

        private static bool TryGetNextAvailableId(int ApplicationId, bool Pre108, out int index)
        {
            lock (_Lock)
            {
                // If the ApplicationId does not exist, initialize it
                InitializeAppId(ApplicationId, Pre108);

                if (_IdCounter.TryGetValue(ApplicationId, out ConcurrentDictionary<int, bool>? intList))
                {
                    // Start at index 2, 1 is reserved.
                    for (index = 2; index < (Pre108 ? 255 : int.MaxValue); ++index)
                    {
                        if (intList.TryGetValue(index, out bool isUsed) && !isUsed)
                        {
                            intList[index] = true;
                            return true;
                        }
                        else if (intList.TryAdd(index, true))
                            return true;
                    }
                }
            }

            index = 0;
            return false;
        }

        public static bool TryRegisterNewId(int ApplicationId, int idToAdd, bool Pre108)
        {
            if (idToAdd <= 0)
                return false;
            else if (Pre108 && idToAdd > 255)
                return false;

            lock (_Lock)
            {
                // If the ApplicationId does not exist, initialize it
                InitializeAppId(ApplicationId, Pre108);

                if (_IdCounter.TryGetValue(ApplicationId, out ConcurrentDictionary<int, bool>? intList))
                {
                    if (!intList.ContainsKey(idToAdd))
                        return intList.TryAdd(idToAdd, true);
                    else
                    {
                        intList[idToAdd] = true;
                        return true;
                    }
                }
            }

            return false;
        }

        public static void UnregisterId(int ApplicationId, int idToRemove)
        {
            if (_IdCounter.TryGetValue(ApplicationId, out ConcurrentDictionary<int, bool>? intList) && intList.ContainsKey(idToRemove))
                intList[idToRemove] = false;
        }

        public Channel() { }

        public Channel(int ApplicationId, int mediusVersion)
        {
            if (!TryGetNextAvailableId(ApplicationId, mediusVersion <= 108, out int Id))
                LoggerAccessor.LogError($"[Channel] - Failed to get a new Id in the MUM cache for AppId:{ApplicationId}!");

            this.Id = Id;
            MediusVersion = mediusVersion;

            this.ApplicationId = ApplicationId;

            MumManager.channelsCreated.Inc();
        }

        public Channel(int Id, int ApplicationId, int mediusVersion)
        {
            if (!TryRegisterNewId(ApplicationId, Id, mediusVersion <= 108))
                LoggerAccessor.LogError($"[Channel] - Id:{Id} could not be added in the MUM cache for AppId:{ApplicationId}!");

            this.Id = Id;
            MediusVersion = mediusVersion;

            this.ApplicationId = ApplicationId;

            MumManager.channelsCreated.Inc();
        }

        public Channel(int Id, int ApplicationId, int mediusVersion, string Name, string Password, int MaxPlayers, ulong GenericField1, ulong GenericField2, ulong GenericField3, ulong GenericField4, MediusWorldGenericFieldLevelType GenericFieldLevel, ChannelType type)
        {
            if (!TryRegisterNewId(ApplicationId, Id, mediusVersion <= 108))
                LoggerAccessor.LogError($"[Channel] - Id:{Id} could not be added in the MUM cache for AppId:{ApplicationId}!");

            this.Id = Id;
            MediusVersion = mediusVersion;

            this.ApplicationId = ApplicationId;
            this.Name = Name;
            this.Password = Password;
            SecurityLevel = string.IsNullOrEmpty(Password) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD;
            this.MaxPlayers = MaxPlayers;
            this.GenericField1 = GenericField1;
            this.GenericField2 = GenericField2;
            this.GenericField3 = GenericField3;
            this.GenericField4 = GenericField4;
            this.GenericFieldLevel = GenericFieldLevel;
            Type = type;

            MumManager.channelsCreated.Inc();
        }

        public Channel(int mediusVersion, MediusCreateChannelRequest request)
        {
            ApplicationId = request.ApplicationID;

            if (!TryGetNextAvailableId(ApplicationId, mediusVersion <= 108, out int Id))
                LoggerAccessor.LogError($"[Channel] - Failed to get a new Id in the MUM cache for AppId:{ApplicationId}!");

            this.Id = Id;
            MediusVersion = mediusVersion;

            Name = request.LobbyName;
            Password = request.LobbyPassword;
            SecurityLevel = string.IsNullOrEmpty(Password) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD;
            MaxPlayers = request.MaxPlayers;
            GenericField1 = request.GenericField1;
            GenericField2 = request.GenericField2;
            GenericField3 = request.GenericField3;
            GenericField4 = request.GenericField4;
            GenericFieldLevel = request.GenericFieldLevel;

            MumManager.channelsCreated.Inc();
        }

        public Channel(int mediusVersion, MediusCreateChannelRequest0 request)
        {
            ApplicationId = request.ApplicationID;

            if (!TryGetNextAvailableId(ApplicationId, mediusVersion <= 108, out int Id))
                LoggerAccessor.LogError($"[Channel] - Failed to get a new Id in the MUM cache for AppId:{ApplicationId}!");

            this.Id = Id;
            MediusVersion = mediusVersion;

            MinPlayers = request.MinPlayers;
            MaxPlayers = request.MaxPlayers;
            GameLevel = request.GameLevel;
            Password = request.GamePassword;
            PlayerSkillLevel = request.PlayerSkillLevel;
            RulesSet = request.RulesSet;
            GenericField1 = request.GenericField1;
            GenericField2 = request.GenericField2;
            GenericField3 = request.GenericField3;
            GameHostType = request.GameHostType;

            MumManager.channelsCreated.Inc();
        }

        public Channel(int mediusVersion, MediusCreateChannelRequest1 request)
        {
            ApplicationId = request.ApplicationID;

            if (!TryGetNextAvailableId(ApplicationId, mediusVersion <= 108, out int Id))
                LoggerAccessor.LogError($"[Channel] - Failed to get a new Id in the MUM cache for AppId:{ApplicationId}!");

            this.Id = Id;
            MediusVersion = mediusVersion;

            MaxPlayers = request.MaxPlayers;
            Name = request.LobbyName;
            Password = request.LobbyPassword;

            MumManager.channelsCreated.Inc();
        }

        public static Channel GetDefaultChannel(int ApplicationId, int mediusVersion)
        {
            //Eyetoy Chat
            if (ApplicationId == 10550)
                return new Channel(1, ApplicationId, mediusVersion) { Name = "Default", Type = ChannelType.Lobby, LobbyFilterMaskLevelType = MediusLobbyFilterMaskLevelType.MediusLobbyFilterMaskLevel1, GenericField1 = 1000 };
            else
                return new Channel(1, ApplicationId, mediusVersion) { Name = "Default", Type = ChannelType.Lobby };
        }

        public virtual Task Tick()
        {
            // Remove inactive clients
            for (int i = 0; i < LocalClients.Count; ++i)
            {
                if (!LocalClients[i].IsConnected)
                {
                    LocalClients.RemoveAt(i);
                    --i;
                }
            }

            return Task.CompletedTask;
        }

        public virtual Task OnChannelCreate(Channel channel)
        {
            LocalChannels.Add(channel);

            return Task.CompletedTask;
        }

        public virtual Task OnPlayerJoined(ClientObject client)
        {
            LocalClients.Add(client);

            return Task.CompletedTask;
        }

        public virtual void OnPlayerLeft(ClientObject client)
        {
            LocalClients.RemoveAll(x => x == client);
        }

        #region Parties

        public virtual void RegisterParty(Party party)
        {
            _removeChannel = false; // If an other thread removed party but channel not closed yet.
            _parties.Add(party);
        }

        public virtual void UnregisterParty(Party party)
        {
            // Remove Party
            _parties.Remove(party);

            // If empty, just end channel
            if (_parties.Count == 0)
                _removeChannel = true;
        }
        #endregion

        #region Games
        public virtual void RegisterGame(Game game)
        {
            _removeChannel = false; // If an other thread removed game but channel not closed yet.
            _games.Add(game);
        }

        public virtual void UnregisterGame(Game game)
        {
            // Remove game
            _games.Remove(game);

            // If empty, just end channel
            if (_games.Count == 0)
                _removeChannel = true;
        }
        #endregion

        #region Buddies
        /// <summary>
        /// Add to buddy list confirmation for only them to appear on your friends list
        /// </summary>
        /// <param name="source">The client making the request for confirmation</param>
        /// <param name="accountToAdd">The account to add as a buddy on the client</param>
        /// <param name="msg"></param>
        public void AddToBuddyListConfirmationSingleRequest(ClientObject source, ClientObject accountToAdd, MediusAddToBuddyListConfirmationRequest msg)
        {
            accountToAdd?.Queue(new MediusAddToBuddyListFwdConfirmationRequest()
            {
                MessageID = msg.MessageID,
                OriginatorAccountID = source.AccountId,
                OriginatorAccountName = source.AccountName,
                AddType = MediusBuddyAddType.AddSingle,
            });
        }

        /// <summary>
        /// Add to buddy list confirmation for both you and them to appear on both clients friends list
        /// </summary>
        /// <param name="source">The client making the request for confirmation</param>
        /// <param name="accountToAdd">The account to add as a buddy on the client</param>
        /// <param name="msg"></param>
        public void AddToBuddyListConfirmationSymmetricRequest(ClientObject source, ClientObject accountToAdd, MediusAddToBuddyListConfirmationRequest msg)
        {
            accountToAdd?.Queue(new MediusAddToBuddyListFwdConfirmationRequest()
            {
                MessageID = msg.MessageID,
                OriginatorAccountID = source.AccountId,
                OriginatorAccountName = source.AccountName,
                AddType = MediusBuddyAddType.AddSymmetric,
            });
        }

        /// <summary>
        /// Add to buddy list confirmation for only them to appear on your friends list
        /// </summary>
        /// <param name="source">The client making the request for confirmation</param>
        /// <param name="accountToAdd">The account to add as a buddy on the client</param>
        /// <param name="msg"></param>
        public void AddToBuddyListConfirmationSingleResponse(ClientObject source, ClientObject accountToAdd, MediusAddToBuddyListFwdConfirmationResponse msg)
        {
            accountToAdd?.Queue(new MediusAddToBuddyListConfirmationResponse()
            {
                MessageID = msg.MessageID,
                StatusCode = msg.StatusCode,
                TargetAccountID = source.AccountId,
                TargetAccountName = source.AccountName,
            });
        }

        /// <summary>
        /// Add to buddy list confirmation for both you and them to appear on both clients friends list
        /// </summary>
        /// <param name="source">The client making the request for confirmation</param>
        /// <param name="accountToAdd">The account to add as a buddy on the client</param>
        /// <param name="msg"></param>
        public void AddToBuddyListConfirmationSymmetricResponse(ClientObject source, ClientObject accountToAdd, MediusAddToBuddyListFwdConfirmationResponse msg)
        {
            accountToAdd?.Queue(new MediusAddToBuddyListConfirmationResponse()
            {
                MessageID = msg.MessageID,
                StatusCode = msg.StatusCode,
                TargetAccountID = source.AccountId,
                TargetAccountName = source.AccountName,
            });
        }

        #endregion

        #region Messages
        public Task BroadcastBinaryMessage(ClientObject source, MediusBinaryMessage msg)
        {
            foreach (var client in LocalClients.Where(x => x != source))
            {
                client?.Queue(new MediusBinaryFwdMessage()
                {
                    MessageType = msg.MessageType,
                    OriginatorAccountID = source.AccountId,
                    Message = msg.Message
                });
            }

            return Task.CompletedTask;
        }

        public Task BroadcastBinaryMessage(ClientObject source, MediusBinaryMessage1 msg)
        {
            foreach (var client in LocalClients.Where(x => x != source))
            {
                client?.Queue(new MediusBinaryFwdMessage1()
                {
                    MessageID = msg.MessageID,
                    MessageType = msg.MessageType,
                    OriginatorAccountID = source.AccountId,
                    MessageSize = msg.MessageSize,
                    Message = msg.Message
                });
            }

            return Task.CompletedTask;
        }

        public Task BroadcastDirectBinaryMessage(MediusBinaryFwdMessage msg)
        {
            foreach (var client in LocalClients)
            {
                client?.Queue(msg);
            }

            return Task.CompletedTask;
        }

        public Task BroadcastDirectBinaryMessage(MediusBinaryFwdMessage1 msg)
        {
            foreach (var client in LocalClients)
            {
                client?.Queue(msg);
            }

            return Task.CompletedTask;
        }

        #region GenericChatMessages
        public Task BroadcastChatMessage(IEnumerable<ClientObject> targets, ClientObject source, string message)
        {
            foreach (var target in targets)
            {
                if (target.MediusVersion >= 112)
                {
                    target?.Queue(new MediusGenericChatFwdMessage1()
                    {
                        OriginatorAccountID = source.AccountId,
                        OriginatorAccountName = source.AccountName,
                        Message = message,
                        MessageType = MediusChatMessageType.Broadcast,
                        TimeStamp = DateTimeUtils.GetUnixTimeU32()
                    });
                }
                else
                {
                    target?.Queue(new MediusGenericChatFwdMessage()
                    {
                        OriginatorAccountID = source.AccountId,
                        OriginatorAccountName = source.AccountName,
                        Message = message,
                        MessageType = MediusChatMessageType.Broadcast,
                        TimeStamp = DateTimeUtils.GetUnixTimeU32()
                    });
                }
            }

            return Task.CompletedTask;
        }

        public Task WhisperChatMessage(IEnumerable<ClientObject> targets, ClientObject source, string message)
        {
            foreach (var target in targets)
            {
                if (target.MediusVersion >= 112)
                {
                    target?.Queue(new MediusGenericChatFwdMessage1()
                    {
                        OriginatorAccountID = source.AccountId,
                        OriginatorAccountName = source.AccountName,
                        Message = message,
                        MessageType = MediusChatMessageType.Whisper,
                        TimeStamp = DateTimeUtils.GetUnixTimeU32()
                    });
                }
                else
                {
                    target?.Queue(new MediusGenericChatFwdMessage()
                    {
                        OriginatorAccountID = source.AccountId,
                        OriginatorAccountName = source.AccountName,
                        Message = message,
                        MessageType = MediusChatMessageType.Whisper,
                        TimeStamp = DateTimeUtils.GetUnixTimeU32()
                    });
                }
            }

            return Task.CompletedTask;
        }

        public Task ClanChatMessage(IEnumerable<ClientObject> targets, ClientObject source, string message)
        {
            foreach (var target in targets)
            {
                if (target.MediusVersion >= 112)
                {
                    target?.Queue(new MediusGenericChatFwdMessage1()
                    {
                        OriginatorAccountID = source.AccountId,
                        OriginatorAccountName = source.AccountName,
                        Message = message,
                        MessageType = MediusChatMessageType.Clan,
                        TimeStamp = DateTimeUtils.GetUnixTimeU32()
                    });
                }
                else
                {
                    target?.Queue(new MediusGenericChatFwdMessage()
                    {
                        OriginatorAccountID = source.AccountId,
                        OriginatorAccountName = source.AccountName,
                        Message = message,
                        MessageType = MediusChatMessageType.Clan,
                        TimeStamp = DateTimeUtils.GetUnixTimeU32()
                    });
                }
            }

            return Task.CompletedTask;
        }
        #endregion

        /// <summary>
        /// Send Server System Message 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="message"></param>
        public Task SendSystemMessage(ClientObject client, string message)
        {
            if (client.MediusVersion >= 112)
            {
                client.Queue(new MediusGenericChatFwdMessage1()
                {
                    OriginatorAccountID = 95481,
                    OriginatorAccountName = "SYSTEM",
                    Message = message,
                    MessageType = MediusChatMessageType.Broadcast,
                    TimeStamp = DateTimeUtils.GetUnixTimeU32()
                });
            }
            else
            {
                client.Queue(new MediusGenericChatFwdMessage()
                {
                    OriginatorAccountID = 95481,
                    OriginatorAccountName = "SYSTEM",
                    Message = message,
                    MessageType = MediusChatMessageType.Broadcast,
                    TimeStamp = DateTimeUtils.GetUnixTimeU32()
                });
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// Send Server System Message 
        /// </summary>
        /// <param name="targets"></param>
        /// <param name="message"></param>
        public Task BroadcastGenericChatMessage(IEnumerable<ClientObject> targets, string message)
        {
            foreach (var target in targets)
            {
                if (target.MediusVersion >= 112)
                {
                    target?.Queue(new MediusGenericChatFwdMessage1()
                    {
                        OriginatorAccountID = 95481,
                        OriginatorAccountName = "SYSTEM",
                        Message = message,
                        MessageType = MediusChatMessageType.Broadcast,
                        TimeStamp = DateTimeUtils.GetUnixTimeU32()
                    });
                }
                else
                {
                    target?.Queue(new MediusGenericChatFwdMessage()
                    {
                        OriginatorAccountID = 95481,
                        OriginatorAccountName = "SYSTEM",
                        Message = message,
                        MessageType = MediusChatMessageType.Broadcast,
                        TimeStamp = DateTimeUtils.GetUnixTimeU32()
                    });
                }
            }

            return Task.CompletedTask;
        }


        public Task BroadcastSystemMessage(IEnumerable<ClientObject> targets, string msg, byte severity)
        {
            foreach (var target in targets)
            {
                target?.Queue(new RT_MSG_SERVER_SYSTEM_MESSAGE()
                {
                    Severity = severity,
                    EncodingType = DME_SERVER_ENCODING_TYPE.DME_SERVER_ENCODING_UTF8,
                    LanguageType = DME_SERVER_LANGUAGE_TYPE.DME_SERVER_LANGUAGE_US_ENGLISH,
                    EndOfMessage = true,
                    Message = msg
                });
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}