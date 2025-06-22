using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.LIBRARY.Common;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;
using NetworkLibrary.Extension;

namespace Horizon.LIBRARY.libAntiCheat.Models
{
    public enum ChannelType
    {
        Lobby,
        Game
    }

    public class Channel
    {
        public static uint IdCounter = 0;

        public List<ClientObject> Clients = new List<ClientObject>();
        public List<Channel> Channels = new List<Channel>();

        public uint Id = 0;
        public int ApplicationId = 0;
        public ChannelType Type = ChannelType.Lobby;
        public string Name = "Default";
        public string Password = null;
        public int MinPlayers = 1;
        public int MaxPlayers = 10;
        public int GameLevel = 0;
        public int PlayerSkillLevel = 0;
        public int RuleSet = 0;
        public MediusApplicationType AppType = MediusApplicationType.MediusAppTypeGame;
        public MediusWorldSecurityLevelType SecurityLevel = MediusWorldSecurityLevelType.WORLD_SECURITY_NONE;
        public uint GenericField1 = 0;
        public uint GenericField2 = 0;
        public uint GenericField3 = 0;
        public uint GenericField4 = 0;
        public MediusWorldGenericFieldLevelType GenericFieldLevel = MediusWorldGenericFieldLevelType.MediusWorldGenericFieldLevel0;
        public MGCL_GAME_HOST_TYPE GameHostType;
        public MediusWorldStatus WorldStatus;

        public virtual int PlayerCount => Clients.Count;
        public int GameCount => _games.Count;

        protected List<Game> _games = new List<Game>();
        protected bool _removeChannel = false;
        protected DateTime _timeCreated = DateTimeUtils.GetHighPrecisionUtcTime();

        public Channel()
        {
            Id = IdCounter++;
        }

        public Channel(MediusCreateChannelRequest request)
        {
            Id = IdCounter++;

            ApplicationId = request.ApplicationID;
            Name = request.LobbyName;
            Password = request.LobbyPassword;
            SecurityLevel = string.IsNullOrEmpty(Password) ? MediusWorldSecurityLevelType.WORLD_SECURITY_NONE : MediusWorldSecurityLevelType.WORLD_SECURITY_PLAYER_PASSWORD;
            MaxPlayers = request.MaxPlayers;
            GenericField1 = request.GenericField1;
            GenericField2 = request.GenericField2;
            GenericField3 = request.GenericField3;
            GenericField4 = request.GenericField4;
            GenericFieldLevel = request.GenericFieldLevel;
        }

        public Channel(MediusCreateChannelRequest0 request)
        {
            Id = IdCounter++;

            ApplicationId = request.ApplicationID;
            MinPlayers = request.MinPlayers;
            MaxPlayers = request.MaxPlayers;
            GameLevel = request.GameLevel;
            Password = request.GamePassword;
            PlayerSkillLevel = request.PlayerSkillLevel;
            RuleSet = request.RulesSet;
            GenericField1 = request.GenericField1;
            GenericField2 = request.GenericField2;
            GenericField3 = request.GenericField3;
            GameHostType = request.GameHostType;
        }

        public Channel(MediusCreateChannelRequest1 request)
        {
            Id = IdCounter++;

            ApplicationId = request.ApplicationID;
            MaxPlayers = request.MaxPlayers;
            Name = request.LobbyName;
            Password = request.LobbyPassword;
        }

        public virtual Task OnChannelCreate(Channel channel)
        {
            Channels.Add(channel);

            return Task.CompletedTask;
        }

        public virtual Task OnPlayerJoined(ClientObject client)
        {
            Clients.Add(client);

            return Task.CompletedTask;
        }

        public virtual void OnPlayerLeft(ClientObject client)
        {
            Clients.RemoveAll(x => x == client);
        }

        #region Games
        public virtual void RegisterGame(Game game)
        {
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

        #region Messages
        public Task BroadcastBinaryMessage(ClientObject source, MediusBinaryMessage msg)
        {
            foreach (ClientObject client in Clients.Where(x => x != source))
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
            foreach (ClientObject client in Clients.Where(x => x != source))
            {
                client?.Queue(new MediusBinaryFwdMessage1()
                {
                    MessageType = msg.MessageType,
                    OriginatorAccountID = source.AccountId,
                    MessageSize = msg.MessageSize,
                    Message = msg.Message
                });
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

        public Task SendSystemMessage(ClientObject client, string message)
        {
            if (client.MediusVersion >= 112)
            {
                client.Queue(new MediusGenericChatFwdMessage1()
                {
                    OriginatorAccountID = 0,
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
                    OriginatorAccountID = 0,
                    OriginatorAccountName = "SYSTEM",
                    Message = message,
                    MessageType = MediusChatMessageType.Broadcast,
                    TimeStamp = DateTimeUtils.GetUnixTimeU32()
                });
            }

            return Task.CompletedTask;
        }

        public Task BroadcastSystemMessage(IEnumerable<ClientObject> targets, string message)
        {
            foreach (var target in targets)
            {
                if (target.MediusVersion >= 112)
                {
                    target?.Queue(new MediusGenericChatFwdMessage1()
                    {
                        OriginatorAccountID = 0,
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
                        OriginatorAccountID = 0,
                        OriginatorAccountName = "SYSTEM",
                        Message = message,
                        MessageType = MediusChatMessageType.Broadcast,
                        TimeStamp = DateTimeUtils.GetUnixTimeU32()
                    });
                }
            }

            return Task.CompletedTask;
        }
        #endregion
    }
}