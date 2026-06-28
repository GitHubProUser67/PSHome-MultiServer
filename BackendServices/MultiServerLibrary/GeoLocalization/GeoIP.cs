using System.Net;
using System.Text.RegularExpressions;
using CastleLibrary.Utils;
using MaxMind.GeoIP2;
using MultiServerLibrary.HTTP;

namespace MultiServerLibrary.GeoLocalization
{
    public partial class GeoIP : IDisposable
    {
        public readonly DatabaseReader Reader;
        public readonly DatabaseReader CityReader;

        private static GeoIP _instance;

        // Static map of country ISO codes to language codes
        private static readonly Dictionary<string, string> CountryLanguageMap = new()
        {
            { "US", "en" },
            { "GB", "en" },
            { "FR", "fr" },
            { "DE", "de" },
            { "JP", "ja" },
            { "CN", "zh" },
            { "KR", "ko" },
            { "IT", "it" },
            { "ES", "es" },
            { "RU", "ru" },
            { "BR", "pt" },
            { "IN", "hi" },
        };

        public GeoIP(DatabaseReader reader, DatabaseReader cityReader)
        {
            Reader = reader;
            CityReader = cityReader;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    Reader?.Dispose();
                    CityReader?.Dispose();

                    if (_instance != null)
                    {
                        _instance.Dispose();
                        _instance = null;
                    }
                }
            }
            catch
            {
                // Not Important.
            }
        }

        ~GeoIP()
        {
            Dispose(false);
        }

        public static Task Initialize()
        {
            string dbUrl = null;

            CustomLogger.LoggerAccessor.LogWarn(
                $"[GeoIP] - Initialize() - Started Initialization at: {DateTime.Now}."
            );

            /* OBSOLETE!
             *
             * const string GeoLite2UpdaterUrl = "https://geolite2.edtunnel.best/download";

            var UpdaterPageContent = HTTPProcessor.RequestURLGET(GeoLite2UpdaterUrl, true);

            if (!string.IsNullOrEmpty(UpdaterPageContent))
            {
                dbUrl = MyRegex().Match(UpdaterPageContent)
                                    .Groups[1].Value;
                if (string.IsNullOrWhiteSpace(dbUrl))
                    CustomLogger.LoggerAccessor.LogWarn("[GeoIP] - Initialize() - Database URL not found.");
            }*/

            InitializeInstance(dbUrl);

            return Task.CompletedTask;
        }

        private static void InitializeInstance(string dbUrl)
        {
            DatabaseReader reader,
                cityReader;
            var directoryPath = $"{Directory.GetCurrentDirectory()}/static";
            var DbPath = $"{directoryPath}/GeoIP2-Country.mmdb";
            var liteDbPath = $"{directoryPath}/GeoLite2-Country.mmdb";
            var cityDbPath = $"{directoryPath}/GeoIP2-City.mmdb";
            var cityliteDbPath = $"{directoryPath}/GeoLite2-City.mmdb";

            try
            {
                using (var mutex = new Mutex(false, $"Global\\{nameof(GeoIP)}Lock"))
                {
                    try
                    {
                        MutexExtensions.TryWithMutex(
                            mutex,
                            null,
                            () =>
                            {
                                Directory.CreateDirectory(directoryPath);

                                // We favor premium/paid databases (not has the same update procedure as the lite variant so no auto-update for this one).
                                if (File.Exists(DbPath))
                                {
                                    reader = new DatabaseReader(DbPath);
#if DEBUG
                                    CustomLogger.LoggerAccessor.LogInfo(
                                        "[GeoIP] - InitializeInstance() - Loaded GeoIP2-Country.mmdb Database..."
                                    );
#endif
                                }
                                else if (File.Exists(liteDbPath))
                                {
                                    if (!string.IsNullOrEmpty(dbUrl))
                                    {
                                        var dbData = HTTPProcessor
                                            .RequestFullURLGET(dbUrl, true)
                                            .data;

                                        if (
                                            dbData != null
                                            && CastleLibrary.NetHasher.DotNetHasher.ComputeSHA256String(
                                                dbData
                                            )
                                                != CastleLibrary.NetHasher.DotNetHasher.ComputeSHA256String(
                                                    File.ReadAllBytes(liteDbPath)
                                                )
                                        )
                                        {
                                            File.WriteAllBytes(liteDbPath, dbData);
#if DEBUG
                                            CustomLogger.LoggerAccessor.LogInfo(
                                                $"[GeoIP] - InitializeInstance() - Updated GeoLite2-Country.mmdb Database as of: {DateTime.Now}."
                                            );
#endif
                                        }
                                    }
                                    reader = new DatabaseReader(liteDbPath);
#if DEBUG
                                    CustomLogger.LoggerAccessor.LogInfo(
                                        "[GeoIP] - InitializeInstance() - Loaded GeoLite2-Country.mmdb Database..."
                                    );
#endif
                                }
                                else if (!string.IsNullOrEmpty(dbUrl))
                                {
                                    var dbData = HTTPProcessor.RequestFullURLGET(dbUrl, true).data;

                                    if (dbData != null)
                                    {
                                        File.WriteAllBytes(liteDbPath, dbData);
                                        reader = new DatabaseReader(liteDbPath);
#if DEBUG
                                        CustomLogger.LoggerAccessor.LogInfo(
                                            "[GeoIP] - InitializeInstance() - Loaded GeoLite2-Country.mmdb Database..."
                                        );
#endif
                                    }
                                    else
                                        reader = null;
                                }
                                else
                                    reader = null;

                                if (File.Exists(cityDbPath))
                                {
                                    cityReader = new DatabaseReader(cityDbPath);
#if DEBUG
                                    CustomLogger.LoggerAccessor.LogInfo(
                                        "[GeoIP] - InitializeInstance() - Loaded GeoIP2-City.mmdb Database..."
                                    );
#endif
                                }
                                else if (File.Exists(cityliteDbPath))
                                {
                                    cityReader = new DatabaseReader(cityliteDbPath);
#if DEBUG
                                    CustomLogger.LoggerAccessor.LogInfo(
                                        "[GeoIP] - InitializeInstance() - Loaded GeoLite2-City.mmdb Database..."
                                    );
#endif
                                }
                                else
                                    cityReader = null;

                                _instance = new GeoIP(reader, cityReader);
                            }
                        );
                    }
                    catch (Exception e)
                    {
                        CustomLogger.LoggerAccessor.LogError(
                            $"[GeoIP] - InitializeInstance() - Failed to initialize GeoIP engine (exception: {e})"
                        );
                    }
                }
            }
            catch (Exception e)
            {
                CustomLogger.LoggerAccessor.LogError(
                    $"[GeoIP] - InitializeInstance() - Failed to get mutex (exception: {e})"
                );
            }
        }

        public static string GetGeoCodeFromIP(IPAddress IPAddr)
        {
            // Format as follows -> Country-IsInEuropeanUnion.
            if (Instance != null && Instance.Reader != null)
            {
                try
                {
                    if (
                        Instance.Reader.TryCountry(IPAddr, out var countryresponse)
                        && countryresponse != null
                        && !string.IsNullOrEmpty(countryresponse.Country.Name)
                    )
                    {
                        return
                            Instance.CityReader != null
                            && Instance.CityReader.TryCity(IPAddr, out var cityresponse)
                            && cityresponse != null
                            && !string.IsNullOrEmpty(cityresponse.City.Name)
                            ? countryresponse.Country.Name
                                + $"-{countryresponse.Country.IsInEuropeanUnion}-{cityresponse.City.Name}"
                            : countryresponse.Country.Name
                                + $"-{countryresponse.Country.IsInEuropeanUnion}";
                    }
                }
                catch
                {
                    // Not Important.
                }
            }

            return null;
        }

        public static string GetISOCodeFromIP(IPAddress IPAddr)
        {
            // Format as follows -> Country-IsInEuropeanUnion.
            if (Instance != null && Instance.Reader != null)
            {
                try
                {
                    if (
                        Instance.Reader.TryCountry(IPAddr, out var countryresponse)
                        && countryresponse != null
                        && !string.IsNullOrEmpty(countryresponse.Country.Name)
                    )
                        return countryresponse.Country.IsoCode;
                }
                catch
                {
                    // Not Important.
                }
            }

            return null;
        }

        public static string GetCountryLangCodeFromIP(IPAddress IPAddr)
        {
            // Format as follows -> enUS.
            if (Instance != null && Instance.Reader != null)
            {
                try
                {
                    if (
                        Instance.Reader.TryCountry(IPAddr, out var countryresponse)
                        && countryresponse != null
                        && !string.IsNullOrEmpty(countryresponse.Country.IsoCode)
                    )
                    {
                        var isoCode = countryresponse.Country.IsoCode;
                        return CountryLanguageMap.TryGetValue(isoCode, out var langCode)
                            ? $"{langCode}{isoCode}"
                            : $"{CountryLanguageMap["US"]}{isoCode}";
                    }
                }
                catch
                {
                    // Not Important.
                }
            }

            return null;
        }

        public static GeoIP Instance
        {
            get { return _instance; }
        }

        [GeneratedRegex(
            @"href\s*=\s*""([^""]*GeoLite2-Country\.mmdb)""",
            RegexOptions.IgnoreCase,
            "fr-FR"
        )]
        private static partial Regex MyRegex();
    }
}
