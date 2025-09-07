using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Responses;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MultiServerLibrary.GeoLocalization
{
    public class GeoIP : IDisposable
    {
        public readonly DatabaseReader Reader;

        private static GeoIP _instance;

        // Static map of country ISO codes to language codes
        private static readonly Dictionary<string, string> CountryLanguageMap = new Dictionary<string, string>
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

        public GeoIP(DatabaseReader reader)
        {
            Reader = reader;
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
            const string GeoLite2UpdaterUrl = "https://geolite2.edtunnel.best/download";
            string dbUrl = null;

            CustomLogger.LoggerAccessor.LogWarn($"[GeoIP] - Initialize() - Started Initialization at: {DateTime.Now}.");

            string UpdaterPageContent = HTTPProcessor.RequestURLGET(GeoLite2UpdaterUrl);

            if (!string.IsNullOrEmpty(UpdaterPageContent))
            {
                dbUrl = Regex.Match(UpdaterPageContent, @"href\s*=\s*""([^""]*GeoLite2-Country\.mmdb)""", RegexOptions.IgnoreCase)
                                    .Groups[1].Value;
                if (string.IsNullOrWhiteSpace(dbUrl))
                    CustomLogger.LoggerAccessor.LogWarn("[GeoIP] - Initialize() - Database URL not found.");
            }

            InitializeInstance(dbUrl);

            return Task.CompletedTask;
        }

        private static void InitializeInstance(string dbUrl)
        {
            DatabaseReader reader;
            string directoryPath = $"{Directory.GetCurrentDirectory()}/static";
            string DbPath = $"{directoryPath}/GeoIP2-Country.mmdb";
            string liteDbPath = $"{directoryPath}/GeoLite2-Country.mmdb";

            using (Mutex mutex = new Mutex(false, $"Global\\{nameof(GeoIP)}Lock"))
            {
                try
                {
                    mutex.WaitOne();

                    Directory.CreateDirectory(directoryPath);

                    // We favor premium/paid databases (not has the same update procedure as the lite variant so no auto-update for this one).
                    if (File.Exists(DbPath))
                    {
                        reader = new DatabaseReader(DbPath);
#if DEBUG
                        CustomLogger.LoggerAccessor.LogInfo("[GeoIP] - InitializeInstance() - Loaded GeoIP2-Country.mmdb Database...");
#endif
                    }
                    else if (File.Exists(liteDbPath))
                    {
                        if (!string.IsNullOrEmpty(dbUrl))
                        {
                            byte[] dbData = HTTPProcessor.RequestFullURLGET(dbUrl).data;

                            if (dbData != null && NetHasher.DotNetHasher.ComputeSHA256String(dbData) != NetHasher.DotNetHasher.ComputeSHA256String(File.ReadAllBytes(liteDbPath)))
                            {
                                File.WriteAllBytes(liteDbPath, dbData);
#if DEBUG
                                CustomLogger.LoggerAccessor.LogInfo($"[GeoIP] - InitializeInstance() - Updated GeoLite2-Country.mmdb Database as of: {DateTime.Now}.");
#endif
                            }
                        }
                        reader = new DatabaseReader(liteDbPath);
#if DEBUG
                        CustomLogger.LoggerAccessor.LogInfo("[GeoIP] - InitializeInstance() - Loaded GeoLite2-Country.mmdb Database...");
#endif
                    }
                    else if (!string.IsNullOrEmpty(dbUrl))
                    {
                        byte[] dbData = HTTPProcessor.RequestFullURLGET(dbUrl).data;

                        if (dbData != null)
                        {
                            File.WriteAllBytes(liteDbPath, dbData);
                            reader = new DatabaseReader(liteDbPath);
#if DEBUG
                            CustomLogger.LoggerAccessor.LogInfo("[GeoIP] - InitializeInstance() - Loaded GeoLite2-Country.mmdb Database...");
#endif
                        }
                        else
                            reader = null;
                    }
                    else
                        reader = null;

                    _instance = new GeoIP(reader);
                }
                catch (Exception e)
                {
                    CustomLogger.LoggerAccessor.LogError($"[GeoIP] - InitializeInstance() - Failed to initialize GeoIP engine (exception: {e})");
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }
        }

        public static string GetGeoCodeFromIP(IPAddress IPAddr)
        {
            // Format as follows -> Country-IsInEuropeanUnion.
            if (Instance != null && Instance.Reader != null)
            {
                try
                {
                    if (Instance.Reader.TryCountry(IPAddr, out CountryResponse countryresponse) && countryresponse != null && !string.IsNullOrEmpty(countryresponse.Country.Name))
                    {
                        if (Instance.Reader.TryCity(IPAddr, out CityResponse cityresponse) && cityresponse != null && !string.IsNullOrEmpty(cityresponse.City.Name))
                            return countryresponse.Country.Name + $"-{countryresponse.Country.IsInEuropeanUnion}-{cityresponse.City.Name}";
                        else
                            return countryresponse.Country.Name + $"-{countryresponse.Country.IsInEuropeanUnion}";
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
                    if (Instance.Reader.TryCountry(IPAddr, out CountryResponse countryresponse) && countryresponse != null && !string.IsNullOrEmpty(countryresponse.Country.Name))
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
                    if (Instance.Reader.TryCountry(IPAddr, out CountryResponse countryresponse)
                        && countryresponse != null
                        && !string.IsNullOrEmpty(countryresponse.Country.IsoCode))
                    {
                        string isoCode = countryresponse.Country.IsoCode;
                        if (CountryLanguageMap.TryGetValue(isoCode, out string langCode))
                            return $"{langCode}{isoCode}";
                        return $"{CountryLanguageMap["US"]}{isoCode}";
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
            get
            {
                return _instance;
            }
        }
    }
}
