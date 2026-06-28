using System.Text.RegularExpressions;
using CustomLogger;
using Horizon.MEDIUS.Extensions.PSHome;
using Horizon.MEDIUS.Models;
using Horizon.MEDIUS.Processors;
using Horizon.PluginManager;
using Horizon.RT.Common;
using Horizon.RT.Models;
using MultiServerLibrary.Extension;

namespace Horizon.MEDIUS
{
    public class MediusManager(List<BaseMediusProcessor> processors)
    {
        private readonly MPS _proxyServer = processors.OfType<MPS>().FirstOrDefault()!;

        private readonly List<BaseMediusProcessor> _processors = processors;

        public MediusPluginsManager Plugins = new(HorizonServerConfiguration.MediusPluginsFolder);

        private DateTime _timeLastPluginTick = DateTimeUtils.GetHighPrecisionUtcTime();

        public MPS ProxyServer => _proxyServer;

        private async Task TickAsync()
        {
            try
            {
                await Task.WhenAll(_processors.Select(server => server.Tick()))
                    .ConfigureAwait(false);

                await Program.MUMManager.Tick().ConfigureAwait(false);

                // Tick plugins
                if (
                    (
                        DateTimeUtils.GetHighPrecisionUtcTime() - _timeLastPluginTick
                    ).TotalMilliseconds > HorizonServerConfiguration.MEDIUSPluginTickIntervalMs
                )
                {
                    _timeLastPluginTick = DateTimeUtils.GetHighPrecisionUtcTime();
                    await Plugins.Tick().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[MediusManager] - An assertion was thrown while ticking the server. (Exception:{ex})"
                );
            }
        }

        public async Task StartTickPooling(CancellationToken token)
        {
            #region Home Closed Beta Plugin
            if (HorizonServerConfiguration.MEDIUSPlaystationHomeClosedBetaAutoCreatePlugin)
            {
                HomeClosedBetaChannelManager.InitiateBetaChannelsId(
                    HorizonServerConfiguration.MEDIUSPlaystationHomeClosedBetaSceneListPath
                );
                await HomeClosedBetaChannelManager
                    .GenerateOrUpdateChatChannels(20371)
                    .ConfigureAwait(false);
                await HomeClosedBetaChannelManager
                    .GenerateOrUpdateChatChannels(20374)
                    .ConfigureAwait(false);
            }
            #endregion

            while (!token.IsCancellationRequested)
            {
                await TickAsync().ConfigureAwait(false);
                await Task.Delay(100, token).ConfigureAwait(false);
            }
        }

        public static List<MediusGetPolicyResponse> GetPolicyFromText(
            MessageId messageId,
            string policy
        )
        {
            List<MediusGetPolicyResponse> policies = new();
            var i = 0;

            while (i < policy.Length)
            {
                // Determine length of string
                var len = policy.Length - i;
                if (len > Constants.POLICY_MAXLEN)
                    len = Constants.POLICY_MAXLEN;

                // Add policy subtext
                policies.Add(
                    new MediusGetPolicyResponse()
                    {
                        MessageID = messageId,
                        StatusCode = MediusCallbackStatus.MediusSuccess,
                        Policy = policy.Substring(i, len),
                    }
                );

                // Increment i
                i += len;

                LoggerAccessor.LogDebug(
                    $"[MediusManager] - Sending Policy Chunk {i} of {len} Len {policy.Length} bytes"
                );
            }

            // Set end of text
            if (policies.Count > 0)
                policies[policies.Count - 1].EndOfText = true;

            return policies;
        }

        #region Text Filter
        private static string GetTextFilterRegexExpression(int appId, TextFilterContext context)
        {
            var appSettings = DATABASE.DatabaseManager.GetAppSettingsOrDefault(appId);
            string? regex = null;

            switch (context)
            {
                case TextFilterContext.ACCOUNT_NAME:
                    regex = appSettings.TextFilterAccountName;
                    break;
                case TextFilterContext.CHAT:
                    regex = appSettings.TextFilterChat;
                    break;
                case TextFilterContext.CLAN_MESSAGE:
                    regex = appSettings.TextFilterClanMessage;
                    break;
                case TextFilterContext.CLAN_NAME:
                    regex = appSettings.TextFilterClanName;
                    break;
                case TextFilterContext.DEFAULT:
                    regex = appSettings.TextFilterDefault;
                    break;
                case TextFilterContext.GAME_NAME:
                    regex = appSettings.TextFilterGameName;
                    break;
            }

            return string.IsNullOrEmpty(regex) ? appSettings.TextFilterDefault : regex;
        }

        public static bool PassTextFilter(int appId, TextFilterContext context, string text)
        {
            var rExp = GetTextFilterRegexExpression(appId, context);
            return string.IsNullOrEmpty(rExp)
                || !new Regex(rExp, RegexOptions.IgnoreCase | RegexOptions.Multiline).IsMatch(text);
        }

        public static string FilterTextFilter(int appId, TextFilterContext context, string text)
        {
            var rExp = GetTextFilterRegexExpression(appId, context);
            return string.IsNullOrEmpty(rExp)
                ? text
                : new Regex(rExp, RegexOptions.IgnoreCase | RegexOptions.Multiline).Replace(
                    text,
                    string.Empty
                );
        }
        #endregion

        #region MFS
        public static string? GetFileSystemPath(int appId, string filename)
        {
            if (!DATABASE.DatabaseManager.GetAppSettingsOrDefault(appId).EnableMediusFileServices)
                return null;
            if (string.IsNullOrEmpty(HorizonServerConfiguration.MEDIUSMFSRootPath))
                return null;
            if (string.IsNullOrEmpty(filename))
                return null;

            var path = Path.GetFullPath(
                Path.Combine(
                    HorizonServerConfiguration.MEDIUSMFSRootPath,
                    appId.ToString(),
                    filename
                )
            );

            // prevent filename from moving up directories
            return !path.StartsWith(Path.GetFullPath(HorizonServerConfiguration.MEDIUSMFSRootPath))
                ? null
                : path;
        }

        /// <summary>
        /// Gets File Path with AppId included in the path
        /// </summary>
        /// <param name="appId">AppId passed in</param>
        /// <returns></returns>
        public static string? GetFileAppIdPath(int appId)
        {
            if (!DATABASE.DatabaseManager.GetAppSettingsOrDefault(appId).EnableMediusFileServices)
                return null;
            if (string.IsNullOrEmpty(HorizonServerConfiguration.MEDIUSMFSRootPath))
                return null;

            var rootPath = Path.GetFullPath(HorizonServerConfiguration.MEDIUSMFSRootPath);
            var path = Path.Combine(rootPath, appId.ToString());

            Directory.CreateDirectory(path);

            // prevent filename from moving up directories
            return !path.StartsWith(rootPath) ? null : path;
        }
        #endregion
    }
}
