using CustomLogger;
using Horizon.LIBRARY.Common;
using MultiServerLibrary.Extension;

namespace SVO
{
    public class SVOManager
    {
        private Dictionary<string, int[]> _appIdGroups = new();

#nullable enable
        private static DateTime? _lastSuccessfulDbAuth;
#nullable disable
        public static SVOManager Manager = new();

        private static DateTime _lastComponentLog = DateTimeUtils.GetHighPrecisionUtcTime();

        private static async Task TickAsync()
        {
            try
            {
                // Attempt to authenticate with the db middleware
                // We do this every 24 hours to get a fresh new token
                if (_lastSuccessfulDbAuth == null || (DateTimeUtils.GetHighPrecisionUtcTime() - _lastSuccessfulDbAuth.Value).TotalHours > 24)
                {
                    if (!await SVOServerConfiguration.Database.Authenticate())
                    {
                        // Log and exit when unable to authenticate
                        LoggerAccessor.LogError($"Unable to authenticate connection to Cache Server.");
                        return;
                    }
                    else
                    {
                        _lastSuccessfulDbAuth = DateTimeUtils.GetHighPrecisionUtcTime();

                        // pass to manager
                        await Manager.OnDatabaseAuthenticated();

                        #region Check Cache Server Simulated
                        if (SVOServerConfiguration.Database._settings.SimulatedMode != true)
                            LoggerAccessor.LogInfo("Connected to Cache Server");
                        else
                            LoggerAccessor.LogInfo("Connected to Cache Server (Simulated)");
                        #endregion
                    }
                }

                if ((DateTimeUtils.GetHighPrecisionUtcTime() - _lastComponentLog).TotalSeconds > 15f)
                    _lastComponentLog = DateTimeUtils.GetHighPrecisionUtcTime();
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(ex);
            }
        }

        public static async void StartTickPooling()
        {
            // iterate
            while (true)
            {
                // tick
                await TickAsync();

                await Task.Delay(100);
            }
        }

        #region App Ids

        public async Task OnDatabaseAuthenticated()
        {
            // get supported app ids
            var appids = await SVOServerConfiguration.Database.GetAppIds();

            // build dictionary of app ids from response
            _appIdGroups = appids.ToDictionary(x => x.Name, x => x.AppIds.ToArray());
        }

        public bool IsAppIdSupported(int appId)
        {
            return _appIdGroups.Any(x => x.Value.Contains(appId));
        }

        public int[] GetAppIdsInGroup(int appId)
        {
            return _appIdGroups.FirstOrDefault(x => x.Value.Contains(appId)).Value ?? new int[0];
        }

        #endregion
    }
}
