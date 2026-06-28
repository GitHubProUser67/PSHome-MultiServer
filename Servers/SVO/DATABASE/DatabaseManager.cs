using CustomLogger;
using MultiServerLibrary.Extension;

namespace SVO.DATABASE
{
    public class DatabaseManager
    {
#nullable enable
        private DateTime? _lastSuccessfulDbAuth;

#nullable disable

        private static readonly Dictionary<int, AppSettings> _appSettings = new();
        private static readonly AppSettings _defaultAppSettings = new(0);

        private async Task TickAsync()
        {
            try
            {
                // Attempt to authenticate with the db middleware
                // We do this every 24 hours to get a fresh new token
                if (
                    _lastSuccessfulDbAuth == null
                    || (
                        DateTimeUtils.GetHighPrecisionUtcTime() - _lastSuccessfulDbAuth.Value
                    ).TotalHours > 24
                )
                    if (!await SVOServerConfiguration.Database.Authenticate().ConfigureAwait(false))
                    {
                        // Log and exit when unable to authenticate
                        LoggerAccessor.LogError(
                            $"[DatabaseManager] - Unable to authenticate connection to Cache Server."
                        );
                        return;
                    }
                    else
                    {
                        _lastSuccessfulDbAuth = DateTimeUtils.GetHighPrecisionUtcTime();

                        // refresh app settings
                        await RefreshAppSettings().ConfigureAwait(false);

                        #region Check Cache Server Simulated
                        if (!SVOServerConfiguration.Database.IsSimulated)
                            LoggerAccessor.LogInfo("[DatabaseManager] - Connected to Cache Server");
                        else
                            LoggerAccessor.LogInfo(
                                "[DatabaseManager] - Connected to Cache Server (Simulated)"
                            );
                        #endregion
                    }

                await SVOServerConfiguration.Database.Tick().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[DatabaseManager] - An assertion was thrown while ticking the database. (Exception:{ex})"
                );
            }
        }

        public async Task StartTickPooling(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                await TickAsync().ConfigureAwait(false);
                await Task.Delay(100, token).ConfigureAwait(false);
            }
        }

        private static async Task RefreshAppSettings()
        {
            try
            {
                if (!SVOServerConfiguration.Database.AmIAuthenticated())
                    return;

                // get supported app ids
                var appIdGroups = await SVOServerConfiguration
                    .Database.GetAppIds()
                    .ConfigureAwait(false);
                if (appIdGroups == null)
                    return;

                // get settings
                foreach (var appIdGroup in appIdGroups)
                    if (appIdGroup.AppIds != null)
                        foreach (var appId in appIdGroup.AppIds)
                        {
                            var settings = await SVOServerConfiguration
                                .Database.GetServerSettings(appId)
                                .ConfigureAwait(false);
                            if (settings != null)
                                if (_appSettings.TryGetValue(appId, out var appSettings))
                                    appSettings.SetSettings(settings);
                                else
                                {
                                    appSettings = new AppSettings(appId);
                                    appSettings.SetSettings(settings);
                                    _appSettings.Add(appId, appSettings);

                                    // we also want to send this back to the server since this is new locally
                                    // and there might be new setting fields that aren't yet on the db
                                    await SVOServerConfiguration
                                        .Database.SetServerSettings(
                                            appId,
                                            appSettings.GetSettings()
                                        )
                                        .ConfigureAwait(false);
                                }
                        }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[DatabaseManager] - RefreshAppSettings: An assertion was thrown while loading configuration. (Exception:{ex})"
                );
            }
        }

        public static AppSettings GetAppSettingsOrDefault(int appId)
        {
            return _appSettings.TryGetValue(appId, out var appSettings)
                ? appSettings
                : _defaultAppSettings;
        }
    }
}
