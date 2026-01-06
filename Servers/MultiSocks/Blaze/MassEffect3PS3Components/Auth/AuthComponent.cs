using Blaze3SDK.Blaze.Authentication;
using Blaze3SDK.Components;
using BlazeCommon;
using CustomLogger;
using MultiSocks.Blaze.MassEffect3PS3Components.Model;
using CastleLibrary.S0ny.XI5;

namespace MultiSocks.Blaze.MassEffect3PS3Components.Auth
{
    internal class AuthComponent : AuthenticationComponentBase.Server
    {
        private static UniqueIDGenerator playerIDCounter = new UniqueIDGenerator();

        public override Task<ConsoleLoginResponse> Ps3LoginAsync(PS3LoginRequest request, BlazeRpcContext context)
        {
#if DEBUG
            LoggerAccessor.LogInfo($"[Blaze] - Auth: Connection Id    : {context.Connection.ID}");
            LoggerAccessor.LogInfo($"[Blaze] - Auth: Email     : {request.mEmail}");
            LoggerAccessor.LogInfo($"[Blaze] - Auth: XI5Ticket Size      : {request.mPS3Ticket.Length}");
#endif
            uint unixTimeStamp = (uint)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            // get ticket
            XI5Ticket ticket = XI5Ticket.ReadFromBytes(request.mPS3Ticket);

            // setup username
            string username = ticket.Username;

            // invalid ticket
            if (!ticket.Valid)
            {
                // log to console
                LoggerAccessor.LogWarn($"[Blaze] - Auth: User {username} tried to alter their ticket data");

                return null;
            }

            // RPCN
            if (ticket.IsSignedByRPCN)
            {
                LoggerAccessor.LogInfo($"[[Blaze] - Auth: User {username} connected at: {DateTime.Now} and is on RPCN");

                username += $"@{XI5Ticket.RPCNSigner}";
            }
            else if (username.EndsWith($"@{XI5Ticket.RPCNSigner}"))
            {
                LoggerAccessor.LogError($"[Blaze] - Auth: User {username} was caught using a RPCN suffix while not on it!");

                return null;
            }
            else
                LoggerAccessor.LogInfo($"[Blaze] - Auth: User {username} connected at: {DateTime.Now} and is on PSN");

            string playerProfileDir = Directory.GetCurrentDirectory() + $"/static/ME3Profiles/{username}/";

            try
            {
                Directory.CreateDirectory(playerProfileDir);
            }
            catch
            {
            }

            ME3Player.ME3PlayerInfo player = new ME3Player.ME3PlayerInfo(context.BlazeConnection.ProtoFireConnection, context.BlazeConnection.ProtoFireConnection.Stream!);
            ME3Player.AllPlayers.Add(player);
            context.AccountId = player.ID;
            player.Name = username;
            player.PlayerID = playerIDCounter.CreateSequentialID();
            player.UserID = player.PlayerID;
            player.pathtoprofile = playerProfileDir + "/player_ps3.conf";
            player.Settings = new List<ME3Player.ME3PlayerInfo.SettingEntry>();
            player.Update = true;

            return Task.FromResult(new ConsoleLoginResponse()
            {
                mCanAgeUp = false,
                mIsOfLegalContactAge = true,
                mLegalDocHost = string.Empty,
                mNeedsLegalDoc = false,
                mPrivacyPolicyUri = string.Empty,
                mSessionInfo = new SessionInfo()
                {
                    mBlazeUserId = player.UserID,
                    mEmail = request.mEmail,
                    mIsFirstLogin = true,
                    mLastLoginDateTime = unixTimeStamp,
                    mPersonaDetails = new PersonaDetails()
                    {
                        mDisplayName = username,
                        mExtId = (ulong)player.UserID,
                        mExtType = ExternalRefType.BLAZE_EXTERNAL_REF_TYPE_PS3,
                        mLastAuthenticated = unixTimeStamp,
                        mPersonaId = player.UserID,
                        mStatus = PersonaStatus.ACTIVE,
                    },
                    mSessionKey = "11229301_9b171d92cc562b293e602ee8325612e7",
                    mUserId = player.UserID,
                },
                mTosHost = string.Empty,
                mTermsOfServiceUri = string.Empty,
                mTosUri = string.Empty,
            });
        }

        public override Task<NullStruct> LogoutAsync(NullStruct request, BlazeRpcContext context)
        {
#if DEBUG
            LoggerAccessor.LogWarn($"[Blaze] - Auth: Logout Connection Id    : {context.Connection.ID}");
#endif

            return null;
        }
    }
}
