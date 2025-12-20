using CustomLogger;
using Horizon.RT.Common;
using Horizon.RT.Models;
using Horizon.RT.Models.AntiCheat;
using Horizon.LIBRARY.libAntiCheat.Models;
using System.Threading.Tasks;

namespace Horizon.LIBRARY.libAntiCheat
{
    public class AntiCheat
    {
        private static ulong _sessionKeyCounter = 0;
        private static readonly object _sessionKeyCounterLock = _sessionKeyCounter;
        public AntiCheatEventInfo sEventFcn;
        AntiCheatEventInfo aInfo = null;

        public Task AntiCheatInit(LM_SEVERITY_LEVEL severity_Level, bool on)
        {
            aInfo = new AntiCheatEventInfo();

            if (on)
            {
                LoggerAccessor.LogInfo("Initializing anticheat\n");

                mc_anticheat_init(aInfo);
            }

            return Task.CompletedTask;
        }

        #region AntiCheat 

        public void mc_anticheat_event(AnticheatEventCode anitCheatEventCode, int WorldID, int SourceClientIndex)
        {

        }

        public void mc_anticheat_event_msg_PLAYERREPORT(AnticheatEventCode anitCheatEventCode, int WorldID, int SourceClientIndex, ClientObject cachePlayer, byte[] stats, int msg_sza)
        {
            AntiCheatEventInfo antiCheatEventSet;

            if (sEventFcn != null)
            {

                int ApplicationId = 0;
                int AccountID = 0;
                bool AntiCheatEnabled = false;

                aInfo.code = anitCheatEventCode;
                if (cachePlayer.ApplicationId != 0)
                    ApplicationId = cachePlayer.ApplicationId;
                aInfo.AppId = ApplicationId;
                aInfo.world_Index = WorldID;
                aInfo.client_Index = SourceClientIndex;
                if (cachePlayer.AccountId != 0)
                    AccountID = cachePlayer.AccountId;
                aInfo.iAccountID = AccountID;
                if (cachePlayer.PassedAntiCheat != true)
                    AntiCheatEnabled = cachePlayer.PassedAntiCheat;
                aInfo.mPassedAntiCheat = AntiCheatEnabled;
                if (cachePlayer.SessionKey != null)
                    aInfo.acSessionkey = cachePlayer.SessionKey;
                else
                    aInfo.acSessionkey = Constants.SESSIONKEY_MAXLEN.ToString();

                if (stats != null)
                {
                    int mDataSize = msg_sza + 1;
                    aInfo.mData = mDataSize;

                    antiCheatEventSet = sEventFcn;
                }
                else
                    aInfo.mData = 0;
                antiCheatEventSet = aInfo;
            }


        }

        public void mc_anticheat_event_msg_CREATELOBBYWORLD(AnticheatEventCode anitCheatEventCode, int WorldID, int SourceClientIndex, ClientObject cachePlayer, AnticheatEvent_CreateLobbyWorld stats, int msg_sza)
        {
            AntiCheatEventInfo antiCheatEventSet;

            if (sEventFcn != null)
            {

                int ApplicationId = 0;
                int AccountID = 0;
                bool AntiCheatEnabled = false;

                aInfo.code = anitCheatEventCode;
                if (cachePlayer.ApplicationId != 0)
                    ApplicationId = cachePlayer.ApplicationId;
                aInfo.AppId = ApplicationId;
                aInfo.world_Index = WorldID;
                aInfo.client_Index = SourceClientIndex;
                if (cachePlayer.AccountId != 0)
                    AccountID = cachePlayer.AccountId;
                aInfo.iAccountID = AccountID;
                if (cachePlayer.PassedAntiCheat != true)
                    AntiCheatEnabled = cachePlayer.PassedAntiCheat;
                aInfo.mPassedAntiCheat = AntiCheatEnabled;
                if (cachePlayer.SessionKey != null)
                    aInfo.acSessionkey = cachePlayer.SessionKey;
                else
                    aInfo.acSessionkey = Constants.SESSIONKEY_MAXLEN.ToString();

                if (stats != null)
                {
                    int mDataSize = msg_sza + 1;
                    aInfo.mData = mDataSize;

                    antiCheatEventSet = sEventFcn;
                }
                else
                    aInfo.mData = 0;
                antiCheatEventSet = aInfo;
            }

        }

        public void mc_anticheat_event_msg_UPDATEUSERSTATE(AnticheatEventCode anitCheatEventCode, int WorldID, int SourceClientIndex, ClientObject cachePlayer, MediusUpdateUserState userState, int msg_sza)
        {
            AntiCheatEventInfo antiCheatEventSet;

            if (sEventFcn != null)
            {

                int ApplicationId = 0;
                int AccountID = 0;
                bool AntiCheatEnabled = false;

                aInfo.code = anitCheatEventCode;
                if (cachePlayer.ApplicationId != 0)
                    ApplicationId = cachePlayer.ApplicationId;
                aInfo.AppId = ApplicationId;
                aInfo.world_Index = WorldID;
                aInfo.client_Index = SourceClientIndex;
                if (cachePlayer.AccountId != 0)
                    AccountID = cachePlayer.AccountId;
                aInfo.iAccountID = AccountID;
                if (cachePlayer.PassedAntiCheat != true)
                    AntiCheatEnabled = cachePlayer.PassedAntiCheat;
                aInfo.mPassedAntiCheat = AntiCheatEnabled;
                if (cachePlayer.SessionKey != null)
                    aInfo.acSessionkey = cachePlayer.SessionKey;
                else
                    aInfo.acSessionkey = Constants.SESSIONKEY_MAXLEN.ToString();

                if (userState != null)
                {
                    int mDataSize = msg_sza + 1;
                    aInfo.mData = mDataSize;

                    antiCheatEventSet = sEventFcn;
                }
                else
                    aInfo.mData = 0;
                antiCheatEventSet = aInfo;
            }


        }

        public static void anticheatGetVersion()
        {
            //string anticheatVersion = "AntiCheat V2.9.2";
            /*
            if (Settings.AntiCheatOn != true)
            {
                LoggerAccessor.LogInfo("AntiCheat is not activated. \n Try setting AntiCheatOn=1 in your config.json file.");
            }
            else
            {
                LoggerAccessor.LogInfo($"{anticheatVersion}");
            }
            */
        }

        public virtual void mc_anticheat_init(AntiCheatEventInfo antiCheatEventInfo)
        {
            sEventFcn = antiCheatEventInfo;

        }
        #endregion

        /// <summary>
        /// Generates a incremental session key number
        /// </summary>
        /// <returns></returns>
        public static string GenerateSessionKey()
        {
            lock (_sessionKeyCounterLock)
            {
                return (++_sessionKeyCounter).ToString();
            }
        }
    }

    public class AnticheatEvent_CreateLobbyWorld
    {
        public string LobbyName = null;
        public string LobbyPassword = null;
    }
}
