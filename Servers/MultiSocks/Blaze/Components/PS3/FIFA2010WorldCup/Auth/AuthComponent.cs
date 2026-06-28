using Blaze2SDK.Blaze;
using Blaze2SDK.Blaze.Authentication;
using Blaze2SDK.Components;
using BlazeCommon;
using CastleLibrary.S0ny.XI5;
using CustomLogger;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.NET;
using MultiSocks.Blaze.Model;

namespace MultiSocks.Blaze.Components.PS3.FIFA2010WorldCup.Auth
{
    internal class AuthComponent : AuthenticationComponentBase.Server
    {
        public override Task<ConsoleLoginResponse> Ps3LoginAsync(
            PS3LoginRequest request,
            BlazeRpcContext context
        )
        {
#if DEBUG
            LoggerAccessor.LogInfo($"[Blaze] - Auth: Connection Id    : {context.Connection.ID}");
            LoggerAccessor.LogInfo($"[Blaze] - Auth: Email     : {request.mEmail}");
            LoggerAccessor.LogInfo(
                $"[Blaze] - Auth: XI5Ticket Size      : {request.mPS3Ticket.Length}"
            );
#endif
            var unixTimeStamp = (uint)
                DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;

            // get ticket
            var ticket = XI5Ticket.ReadFromBytes(request.mPS3Ticket);

            // setup username
            var username = ticket.Username;

            // invalid ticket
            if (!ticket.Valid)
            {
                // log to console
                LoggerAccessor.LogWarn(
                    $"[Blaze] - Auth: User {username} tried to alter their ticket data"
                );

                return null;
            }

            // RPCN
            if (ticket.IsSignedByRPCN)
            {
                LoggerAccessor.LogInfo(
                    $"[[Blaze] - Auth: User {username} connected at: {DateTime.Now} and is on RPCN"
                );

                username += $"@{XI5Ticket.RPCNSigner}";
            }
            else if (username.EndsWith($"@{XI5Ticket.RPCNSigner}"))
            {
                LoggerAccessor.LogError(
                    $"[Blaze] - Auth: User {username} was caught using a RPCN suffix while not on it!"
                );

                return null;
            }
            else
                LoggerAccessor.LogInfo(
                    $"[Blaze] - Auth: User {username} connected at: {DateTime.Now} and is on PSN"
                );

            var playerProfileDir =
                Directory.GetCurrentDirectory() + $"/static/BlazeProfiles/{username}/";

            try
            {
                Directory.CreateDirectory(playerProfileDir);
            }
            catch { }

            BlazeUser.AllPlayers.TryAdd(
                ticket.TitleId,
                new ConcurrentList<BlazeUser.BlazeUserInfo>()
            );

            if (
                BlazeUser.BlazeUserInfo.GetServerUserByUserID(ticket.TitleId, (long)ticket.UserId)
                == null
            )
            {
                var conn = context.BlazeConnection.ProtoFireConnection;
                var player = new BlazeUser.BlazeUserInfo(
                    context.BlazeConnection,
                    conn,
                    conn.Stream!
                );
                BlazeUser.AllPlayers[ticket.TitleId].Add(player);
                player.PSNAuth = ticket;
                player.Name = username;
                player.PlayerID = (long)ticket.UserId;
                player.UserID = player.PlayerID;
                player.pathtoprofile = playerProfileDir + "/player_ps3.conf";
                player.Settings = new List<BlazeUser.BlazeUserInfo.SettingEntry>();

                // TODO, load profile data.

                player.UpdateSettings("personas", player.Name);
                player.UpdateSettings("email", request.mEmail);

                player.Update = true;

                var userIdentification = new UserIdentification
                {
                    mAccountId = (long)ticket.UserId,
                    mAccountLocale = 1701729619,
                    mExternalBlob = ticket.ToExternalBlob(),
                    mExternalId = ticket.UserId,
                    mBlazeId = (uint)ticket.UserId,
                    mName = player.Name,
                    mPersonaId = player.GetSettingPerKey("personas").Split(',')[0], // Get the default persona only for now.
                };

                ProcessUtils.RunDelayed(
                    100,
                    () =>
                        UserSessionsBase.Server.NotifyUserAddedAsync(
                            context.BlazeConnection,
                            userIdentification
                        )
                );

                ProcessUtils.RunDelayed(
                    200,
                    () =>
                        UserSessionsBase.Server.NotifyUserSessionExtendedDataUpdateAsync(
                            context.BlazeConnection,
                            new UserSessionExtendedDataUpdate
                            {
                                mExtendedData = new UserSessionExtendedData
                                {
                                    mAddress = null!,
                                    mBestPingSiteAlias = "qos",
                                    mClientAttributes = new SortedDictionary<uint, int>(),
                                    mCountry = string.Empty,
                                    mDataMap = new SortedDictionary<uint, int>(),
                                    mHardwareFlags = HardwareFlags.None,
                                    mLatencyList = new List<int> { 10 },
                                    mQosData = default,
                                    mUserInfoAttribute = 0,
                                    mBlazeObjectIdList = new List<ulong>(),
                                },
                                mUserId = userIdentification.mBlazeId,
                            }
                        )
                );

                return Task.FromResult(
                    new ConsoleLoginResponse
                    {
                        mSessionInfo = new SessionInfo
                        {
                            mBlazeUserId = (uint)ticket.UserId,
                            mSessionKey = ticket.UserId.ToString(),
                            mEmail = player.GetSettingPerKey("email"),
                            mPersonaDetails = new PersonaDetails
                            {
                                mDisplayName = player.Name,
                                mLastAuthenticated = unixTimeStamp,
                                mPersonaId = (long)ticket.UserId,
                                mExtId = ticket.UserId,
                                mExtType = ExternalRefType.PS3,
                            },
                            mUserId = (long)ticket.UserId,
                        },
                        mTosHost = string.Empty,
                        mTosUri = string.Empty,
                    }
                );
            }

            return null;
        }

        public override Task<NullStruct> LogoutAsync(NullStruct request, BlazeRpcContext context)
        {
#if DEBUG
            LoggerAccessor.LogWarn(
                $"[Blaze] - Auth: Logout Connection Id    : {context.Connection.ID}"
            );
#endif
            BlazeUser.BlazeUserInfo.RemoveUser(context.BlazeConnection);
            return null;
        }
    }
}
