using Blaze3SDK.Blaze;
using Blaze3SDK.Blaze.Authentication;
using Blaze3SDK.Blaze.Util;
using Blaze3SDK.Components;
using BlazeCommon;
using CastleLibrary.S0ny.XI5;
using CustomLogger;
using MultiServerLibrary.Extension;
using MultiServerLibrary.Extension.NET;
using MultiSocks.Blaze.Model;
using MultiSocks.Utils;
using Tdf;

namespace MultiSocks.Blaze.Components.PS3.MassEffect3.Auth
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

                var extendedData = new UserSessionExtendedData
                {
                    mAddress = null!,
                    mBestPingSiteAlias = "qos",
                    mBlazeObjectIdList = new List<BlazeObjectId>(),
                    mClientAttributes = new SortedDictionary<uint, int>(),
                    mClientData = null,
                    mCountry = string.Empty,
                    mDataMap = null,
                    mHardwareFlags = HardwareFlags.None,
                    mLatencyList = new List<int> { 10 },
                    mQosData = new NetworkQosData
                    {
                        mDownstreamBitsPerSecond = 10,
                        mNatType = NatType.NAT_TYPE_MODERATE,
                        mUpstreamBitsPerSecond = 10,
                    },
                    mUserInfoAttribute = ticket.UserId,
                };

                var userIdentification = new UserIdentification
                {
                    mAccountId = (long)ticket.UserId,
                    mAccountLocale = 1701729619,
                    mBlazeId = (long)ticket.UserId,
                    mExternalBlob = ticket.ToExternalBlob(),
                    mExternalId = ticket.UserId,
                    mName = player.Name,
                };

                var sessionInfo = new SessionInfo
                {
                    mBlazeUserId = (long)ticket.UserId,
                    mIsFirstLogin = true,
                    mSessionKey = BlazeServerUtils.GenerateSessionKey(),
                    mLastLoginDateTime = unixTimeStamp,
                    mEmail = request.mEmail,
                    mPersonaDetails = new PersonaDetails
                    {
                        mDisplayName = player.GetSettingPerKey("personas").Split(',')[0], // Get the default persona only for now.
                        mLastAuthenticated = unixTimeStamp,
                        mPersonaId = (long)ticket.UserId,
                        mStatus = PersonaStatus.ACTIVE,
                        mExtId = ticket.UserId,
                        mExtType = ExternalRefType.BLAZE_EXTERNAL_REF_TYPE_PS3,
                    },
                    mUserId = (long)ticket.UserId,
                };

                ProcessUtils.RunDelayed(
                    500,
                    async () =>
                    {
                        await Task.Delay(500).ConfigureAwait(false);

                        await UserSessionsBase
                            .Server.NotifyUserAddedAsync(
                                context.BlazeConnection,
                                new NotifyUserAdded
                                {
                                    mExtendedData = extendedData,
                                    mUserInfo = userIdentification,
                                }
                            )
                            .ConfigureAwait(false);

                        await Task.Delay(600).ConfigureAwait(false);

                        await UserSessionsBase
                            .Server.NotifyUserSessionExtendedDataUpdateAsync(
                                context.BlazeConnection,
                                new UserSessionExtendedDataUpdate
                                {
                                    mExtendedData = extendedData,
                                    mUserId = userIdentification.mAccountId,
                                }
                            )
                            .ConfigureAwait(false);

                        await Task.Delay(800).ConfigureAwait(false);

                        await UserSessionsBase
                            .Server.NotifyUserUpdatedAsync(
                                context.BlazeConnection,
                                new UserStatus
                                {
                                    mBlazeId = userIdentification.mBlazeId,
                                    mStatusFlags = UserDataFlags.Online,
                                }
                            )
                            .ConfigureAwait(false);
                    }
                );

                return Task.FromResult(
                    new ConsoleLoginResponse
                    {
                        mCanAgeUp = false,
                        mLegalDocHost = string.Empty,
                        mNeedsLegalDoc = false,
                        mSessionInfo = sessionInfo,
                        mPrivacyPolicyUri = string.Empty,
                        mIsOfLegalContactAge = true,
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
