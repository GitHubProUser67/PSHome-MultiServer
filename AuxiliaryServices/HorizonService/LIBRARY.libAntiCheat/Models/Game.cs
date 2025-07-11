using CustomLogger;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.LIBRARY.Common;
using System.Data;
using System.Collections.Generic;
using System;
using System.Linq;
using MultiServerLibrary.Extension;

namespace Horizon.LIBRARY.libAntiCheat.Models
{
    public class Game
    {
        public static uint IdCounter = 1;

        public class GameClient
        {
            public ClientObject Client;

            public int DmeId;
            public bool InGame;
        }

        public uint Id = 0;
        public int DMEWorldId = -1;
        public int ApplicationId = 0;
        public ChannelType ChannelType = ChannelType.Game;
        public List<GameClient> Clients = new List<GameClient>();
        public string GameName;
        public string GamePassword;
        public string SpectatorPassword;
        public byte[] GameStats = new byte[Constants.GAMESTATS_MAXLEN];
        public MGCL_GAME_HOST_TYPE GameHostType;
        public NetAddressList netAddressList;
        public int WorldID;
        public int AccountID;
        public int MinPlayers;
        public int MaxPlayers;
        public int GameLevel;
        public int PlayerSkillLevel;
        public int RulesSet;
        public string Metadata;
        public int GenericField1;
        public int GenericField2;
        public int GenericField3;
        public int GenericField4;
        public int GenericField5;
        public int GenericField6;
        public int GenericField7;
        public int GenericField8;
        public MediusWorldStatus WorldStatus => _worldStatus;
        public MediusWorldAttributesType Attributes;
        public DMEObject DMEServer;
        public Channel ChatChannel;
        public ClientObject Host;

        public string AccountIdsAtStart => accountIdsAtStart;
        public DateTime UtcTimeCreated => utcTimeCreated;
        public DateTime? UtcTimeStarted => utcTimeStarted;
        public DateTime? UtcTimeEnded => utcTimeEnded;

        protected MediusWorldStatus _worldStatus = MediusWorldStatus.WorldPendingCreation;
        public bool hasHostJoined = false;
        protected string accountIdsAtStart;
        protected DateTime utcTimeCreated;
        protected DateTime? utcTimeStarted;
        protected DateTime? utcTimeEnded;
        protected DateTime? utcTimeEmpty;
        protected bool destroyed = false;

        public uint Time => (uint)(DateTimeUtils.GetHighPrecisionUtcTime() - utcTimeCreated).TotalMilliseconds;

        public virtual bool ReadyToDestroy => WorldStatus == MediusWorldStatus.WorldClosed && utcTimeEmpty.HasValue && (DateTimeUtils.GetHighPrecisionUtcTime() - utcTimeEmpty)?.TotalSeconds > 1f;

        public Game(ClientObject client, IMediusRequest createGame, Channel chatChannel, DMEObject dmeServer)
        {
            if (createGame is MediusCreateGameRequest r)
                FromCreateGameRequest(r);
            else if (createGame is MediusCreateGameRequest0 r0)
                FromCreateGameRequest0(r0);
            else if (createGame is MediusCreateGameRequest1 r1)
                FromCreateGameRequest1(r1);
            else if (createGame is MediusServerCreateGameOnMeRequest r2)
                FromCreateGameOnMeRequest(r2);
            else if (createGame is MediusServerCreateGameOnSelfRequest r3)
                FromCreateGameOnSelfRequest(r3);
            else if (createGame is MediusServerCreateGameOnSelfRequest0 r4)
                FromCreateGameOnSelfRequest0(r4);

            Id = IdCounter++;

            utcTimeCreated = DateTimeUtils.GetHighPrecisionUtcTime();
            utcTimeEmpty = null;
            DMEServer = dmeServer;
            ChatChannel = chatChannel;
            ChatChannel?.RegisterGame(this);
            Host = client;

            LoggerAccessor.LogInfo($"Game {Id}: {GameName}: Created by {client} | Host: {Host}");
        }

        private void FromCreateGameRequest(MediusCreateGameRequest createGame)
        {
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
        }

        private void FromCreateGameRequest0(MediusCreateGameRequest0 createGame)
        {
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
        }

        private void FromCreateGameRequest1(MediusCreateGameRequest1 createGame)
        {
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
            Attributes = createGame.WorldAttributesType;
        }

        private void FromCreateGameOnMeRequest(MediusServerCreateGameOnMeRequest serverCreateGameOnMe)
        {
            GameName = serverCreateGameOnMe.GameName;
            GameStats = serverCreateGameOnMe.GameStats;
            GamePassword = serverCreateGameOnMe.GamePassword;
            ApplicationId = serverCreateGameOnMe.ApplicationID;
            MaxPlayers = serverCreateGameOnMe.MaxPlayers;
            MinPlayers = serverCreateGameOnMe.MinPlayers;
            GameLevel = serverCreateGameOnMe.GameLevel;
            PlayerSkillLevel = serverCreateGameOnMe.PlayerSkillLevel;
            RulesSet = serverCreateGameOnMe.RulesSet;
            GenericField1 = serverCreateGameOnMe.GenericField1;
            GenericField2 = serverCreateGameOnMe.GenericField2;
            GenericField3 = serverCreateGameOnMe.GenericField3;
            GenericField4 = serverCreateGameOnMe.GenericField4;
            GenericField5 = serverCreateGameOnMe.GenericField5;
            GenericField6 = serverCreateGameOnMe.GenericField6;
            GenericField7 = serverCreateGameOnMe.GenericField7;
            GenericField8 = serverCreateGameOnMe.GenericField8;
            GameHostType = serverCreateGameOnMe.GameHostType;
            netAddressList = serverCreateGameOnMe.AddressList;
            WorldID = serverCreateGameOnMe.WorldID;
            AccountID = serverCreateGameOnMe.AccountID;
        }

        private void FromCreateGameOnSelfRequest(MediusServerCreateGameOnSelfRequest serverCreateGameOnSelf)
        {
            GameName = serverCreateGameOnSelf.GameName;
            GameStats = serverCreateGameOnSelf.GameStats;
            GamePassword = serverCreateGameOnSelf.GamePassword;
            ApplicationId = serverCreateGameOnSelf.ApplicationID;
            MaxPlayers = serverCreateGameOnSelf.MaxPlayers;
            MinPlayers = serverCreateGameOnSelf.MinPlayers;
            GameLevel = serverCreateGameOnSelf.GameLevel;
            PlayerSkillLevel = serverCreateGameOnSelf.PlayerSkillLevel;
            RulesSet = serverCreateGameOnSelf.RulesSet;
            GenericField1 = serverCreateGameOnSelf.GenericField1;
            GenericField2 = serverCreateGameOnSelf.GenericField2;
            GenericField3 = serverCreateGameOnSelf.GenericField3;
            GameHostType = serverCreateGameOnSelf.GameHostType;
            netAddressList = serverCreateGameOnSelf.AddressList;
            WorldID = serverCreateGameOnSelf.WorldID;
            AccountID = serverCreateGameOnSelf.AccountID;
        }

        private void FromCreateGameOnSelfRequest0(MediusServerCreateGameOnSelfRequest0 serverCreateGameOnSelf0)
        {
            GameName = serverCreateGameOnSelf0.GameName;
            GameStats = serverCreateGameOnSelf0.GameStats;
            GamePassword = serverCreateGameOnSelf0.GamePassword;
            ApplicationId = serverCreateGameOnSelf0.ApplicationID;
            MaxPlayers = serverCreateGameOnSelf0.MaxPlayers;
            MinPlayers = serverCreateGameOnSelf0.MinPlayers;
            GameLevel = serverCreateGameOnSelf0.GameLevel;
            PlayerSkillLevel = serverCreateGameOnSelf0.PlayerSkillLevel;
            RulesSet = serverCreateGameOnSelf0.RulesSet;
            GenericField1 = serverCreateGameOnSelf0.GenericField1;
            GenericField2 = serverCreateGameOnSelf0.GenericField2;
            GenericField3 = serverCreateGameOnSelf0.GenericField3;
            GameHostType = serverCreateGameOnSelf0.GameHostType;
            netAddressList = serverCreateGameOnSelf0.AddressList;
            WorldID = serverCreateGameOnSelf0.WorldID;
        }

        public string GetActivePlayerList()
        {
            return string.Join(",", Clients?.Select(x => x.Client.AccountId.ToString()).Where(x => x != null));
        }
    }
}
