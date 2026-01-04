using CustomLogger;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using MultiServerLibrary.Extension;
using System.Collections.Concurrent;

public static class SSFWServerConfiguration
{
    public static bool SSFWCrossSave { get; set; } = true;
    public static bool EnableHTTPCompression { get; set; } = false;
    public static int SSFWTTL { get; set; } = 180;
    public static string SSFWMinibase { get; set; } = "[]";
    public static string SSFWLegacyKey { get; set; } = "**NoNoNoYouCantHaxThis****69";
    public static string SSFWSessionIdKey { get; set; } = StringUtils.GenerateRandomBase64KeyAsync().Result;
    public static string SSFWLayoutsFolder { get; set; } = $"{Directory.GetCurrentDirectory()}/static/layouts";
    public static string SSFWStaticFolder { get; set; } = $"{Directory.GetCurrentDirectory()}/static/wwwssfwroot";
    public static string HTTPSCertificateFile { get; set; } = $"{Directory.GetCurrentDirectory()}/static/SSL/SSFW.pfx";
    public static string HTTPSCertificatePassword { get; set; } = "qwerty";
    public static HashAlgorithmName HTTPSCertificateHashingAlgorithm { get; set; } = HashAlgorithmName.SHA384;
    public static string ScenelistFile { get; set; } = $"{Directory.GetCurrentDirectory()}/static/wwwssfwroot/SceneList.xml";
    public static string[]? HTTPSDNSList { get; set; } = {
            "cprod.homerewards.online.scee.com",
            "cprod.homeidentity.online.scee.com",
            "cprod.homeserverservices.online.scee.com",
            "cdev.homerewards.online.scee.com",
            "cdev.homeidentity.online.scee.com",
            "cdev.homeserverservices.online.scee.com",
            "cdevb.homerewards.online.scee.com",
            "cdevb.homeidentity.online.scee.com",
            "cdevb.homeserverservices.online.scee.com",
            "nonprod1.homerewards.online.scee.com",
            "nonprod1.homeidentity.online.scee.com",
            "nonprod1.homeserverservices.online.scee.com",
            "nonprod2.homerewards.online.scee.com",
            "nonprod2.homeidentity.online.scee.com",
            "nonprod2.homeserverservices.online.scee.com",
            "nonprod3.homerewards.online.scee.com",
            "nonprod3.homeidentity.online.scee.com",
            "nonprod3.homeserverservices.online.scee.com",
            "nonprod4.homerewards.online.scee.com",
            "nonprod4.homeidentity.online.scee.com",
            "nonprod4.homeserverservices.online.scee.com",
        };

    // Sandbox Environments
    public static readonly ConcurrentBag<string> homeEnvs = new()
    {
        "cprod", "cprodts", "cpreprod", "cpreprodb",
        "rc-qa", "rcdev", "rc-dev", "cqa-e",
        "cqa-a", "cqa-j", "cqa-h", "cqab-e",
        "cqab-a", "cqab-j", "cqab-h", "qcqa-e",
        "qcqa-a", "qcqa-j", "qcqa-h", "qcpreprod",
        "qcqab-e", "qcqab-a", "qcqab-j", "qcqab-h",
        "qcpreprodb", "coredev", "core-dev", "core-qa",
        "cdev", "cdev2", "cdev3", "cdeva", "cdevb", "cdevc",
        "nonprod1", "nonprod2", "nonprod3", "prodsp"
    };

    /// <summary>
    /// Tries to load the specified configuration file.
    /// Throws an exception if it fails to find the file.
    /// </summary>
    /// <param name="configPath"></param>
    /// <exception cref="FileNotFoundException"></exception>
    public static void RefreshVariables(string configPath)
    {
        // Make sure the file exists
        if (!File.Exists(configPath))
        {
            LoggerAccessor.LogWarn($"Could not find the configuration file:{configPath}, writing and using server's default.");

            Directory.CreateDirectory(Path.GetDirectoryName(configPath) ?? Directory.GetCurrentDirectory() + "/static");

            // Write the JObject to a file
            File.WriteAllText(configPath, new JObject(
                new JProperty("config_version", (ushort)3),
                new JProperty("minibase", SSFWMinibase),
                new JProperty("legacyKey", SSFWLegacyKey),
                new JProperty("sessionidKey", SSFWSessionIdKey),
                new JProperty("time_to_live", SSFWTTL),
                new JProperty("cross_save", SSFWCrossSave),
                new JProperty("enable_http_compression", EnableHTTPCompression),
                new JProperty("static_folder", SSFWStaticFolder),
                new JProperty("https_dns_list", HTTPSDNSList ?? Array.Empty<string>()),
                new JProperty("certificate_file", HTTPSCertificateFile),
                new JProperty("certificate_password", HTTPSCertificatePassword),
                new JProperty("certificate_hashing_algorithm", HTTPSCertificateHashingAlgorithm.Name),
                new JProperty("scenelist_file", ScenelistFile),
                new JProperty("layouts_folder", SSFWLayoutsFolder)
            ).ToString());

            return;
        }

        try
        {
            // Parse the JSON configuration
            dynamic config = JObject.Parse(File.ReadAllText(configPath));

            ushort config_version = GetValueOrDefault(config, "config_version", (ushort)0);
            if (config_version >= 2)
                EnableHTTPCompression = GetValueOrDefault(config, "enable_http_compression", EnableHTTPCompression);
            SSFWMinibase = GetValueOrDefault(config, "minibase", SSFWMinibase);
            SSFWTTL = GetValueOrDefault(config, "time_to_live", SSFWTTL);
            SSFWLegacyKey = GetValueOrDefault(config, "legacyKey", SSFWLegacyKey);
            SSFWSessionIdKey = GetValueOrDefault(config, "sessionidKey", SSFWSessionIdKey);
            SSFWCrossSave = GetValueOrDefault(config, "cross_save", SSFWCrossSave);
            SSFWStaticFolder = GetValueOrDefault(config, "static_folder", SSFWStaticFolder);
            HTTPSCertificateFile = GetValueOrDefault(config, "certificate_file", HTTPSCertificateFile);
            HTTPSCertificatePassword = GetValueOrDefault(config, "certificate_password", HTTPSCertificatePassword);
            HTTPSCertificateHashingAlgorithm = new HashAlgorithmName(GetValueOrDefault(config, "certificate_hashing_algorithm", HTTPSCertificateHashingAlgorithm.Name));
            HTTPSDNSList = GetValueOrDefault(config, "https_dns_list", HTTPSDNSList);
            ScenelistFile = GetValueOrDefault(config, "scenelist_file", ScenelistFile);
            SSFWLayoutsFolder = GetValueOrDefault(config, "layouts_folder", SSFWLayoutsFolder);
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogWarn($"{configPath} file is malformed (exception: {ex}), using server's default.");
        }
    }

    // Helper method to get a value or default value if not present
    private static T GetValueOrDefault<T>(dynamic obj, string propertyName, T defaultValue)
    {
        try
        {
            if (obj != null)
            {
                if (obj is JObject jObject)
                {
                    if (jObject.TryGetValue(propertyName, out JToken? value))
                    {
                        T? returnvalue = value.ToObject<T>();
                        if (returnvalue != null)
                            return returnvalue;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            LoggerAccessor.LogError($"[Program] - GetValueOrDefault thrown an exception: {ex}");
        }

        return defaultValue;
    }
}
