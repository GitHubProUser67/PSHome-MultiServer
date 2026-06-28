using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using CustomLogger;
using MultiServerLibrary.GeoLocalization;

namespace MultiServerLibrary
{
    public static class MultiServerLibraryConfiguration
    {
        public static bool EnableServerIpAutoNegotiation { get; set; } =
            Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true";
        public static bool UsePublicIp { get; set; } = true;
        public static bool EnableSNMPReports { get; set; } = false;
        public static string ProxyUserName { get; set; } = string.Empty;
        public static string ProxyPassword { get; set; } = string.Empty;
        public static string ProxyHost { get; set; } = string.Empty;
        public static ushort ProxyPort { get; set; } = 0;
        public static string ServerIpOverride { get; set; } = string.Empty;
        public static string FallbackServerPublicIp { get; set; } = IPAddress.Any.ToString();
        public static string SNMPTrapHost { get; set; } = IPAddress.Loopback.ToString();
        public static string SNMPEnterpriseOid { get; set; } = "12345";
        public static HashAlgorithmName SNMPHashAlgorithm { get; set; } = HashAlgorithmName.MD5;
        public static string SNMPUserName { get; set; } = "usr-md5-aes";
        public static string SNMPAuthPassword { get; set; } = "authkey1";
        public static string SNMPPrivatePassword { get; set; } = "privkey1";

        public static int RatePermitLimit { get; set; } = -1;
        public static int RateWindowSeconds { get; set; } = 60;
        public static int RateSegmentsPerWindow { get; set; } = 1;
        public static int RateQueueLimit { get; set; } = int.MaxValue;

        public static string GeoLandJsonPath { get; set; } = Directory.GetCurrentDirectory() + "/static/ne_110m_land.geojson";

        // To use only with known problematic providers since this is a static list! Each servers should have it's own live/dynamic ban list on top.
        public static List<string> BannedIPs { get; set; }

        public static VpnChecker VpnCheck = null;

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
                // Firstly, perform migration for older config files.
                var networkLibraryConfigPath =
                    Path.GetDirectoryName(configPath) + $"/NetworkLibrary.json";

                if (File.Exists(networkLibraryConfigPath))
                {
                    LoggerAccessor.LogWarn(
                        $"NetworkLibrary config file found, migrating to file:{configPath}..."
                    );
                    File.Move(networkLibraryConfigPath, configPath);
                }
                else
                {
                    LoggerAccessor.LogWarn(
                        $"Could not find the configuration file:{configPath}, writing and using server's default."
                    );

                    Directory.CreateDirectory(
                        Path.GetDirectoryName(configPath)
                            ?? Directory.GetCurrentDirectory() + "/static"
                    );

                    // Write the JsonObject to a file
                    var configObject = new
                    {
                        config_version = (ushort)2,
                        enable_server_ip_auto_negotiation = EnableServerIpAutoNegotiation,
                        use_public_ip = UsePublicIp,
                        fallback_server_public_ip = FallbackServerPublicIp,
                        server_ip_override = ServerIpOverride,
                        http_rate = new
                        {
                            permit_limit = RatePermitLimit,
                            window_seconds = RateWindowSeconds,
                            segments_per_window = RateSegmentsPerWindow,
                            queue_limit = RateQueueLimit,
                        },
                        snmp = new
                        {
                            enable = EnableSNMPReports,
                            trap_host = SNMPTrapHost,
                            enterprise_oid = SNMPEnterpriseOid,
                            hash_algorithm = SNMPHashAlgorithm.Name,
                            username = SNMPUserName,
                            auth_password = SNMPAuthPassword,
                            private_password = SNMPPrivatePassword,
                        },
                        proxy = new
                        {
                            user_name = ProxyUserName,
                            password = ProxyPassword,
                            host = ProxyHost,
                            port = ProxyPort,
                        },
                        vpnban = new
                        {
                            enable = false,
                            ipqs_api_key = "PUT_IPQS_APIKEY_HERE (Register at: https://www.ipqualityscore.com/)",
                        },
                        geo = new
                        {
                            land_json_path = GeoLandJsonPath,
                        },
                        BannedIPs,
                    };

                    File.WriteAllText(
                        configPath,
                        JsonSerializer.Serialize(
                            configObject,
                            new JsonSerializerOptions { WriteIndented = true }
                        )
                    );

                    return;
                }
            }

            try
            {
                // Parse the JSON configuration
                using (var doc = JsonDocument.Parse(File.ReadAllText(configPath)))
                {
                    var config = doc.RootElement;

                    var config_version = GetValueOrDefault(config, "config_version", (ushort)0);
                    if (config_version >= 2)
                    {
                        // Deserialize BannedIPs if it exists
                        if (
                            config.TryGetProperty(nameof(BannedIPs), out var bannedIPsElement)
                            && bannedIPsElement.ValueKind == JsonValueKind.Array
                        )
                        {
                            BannedIPs = new List<string>();

                            foreach (var ipElement in bannedIPsElement.EnumerateArray())
                            {
                                if (ipElement.ValueKind == JsonValueKind.String)
                                {
                                    var entry = ipElement.GetString();
                                    if (!string.IsNullOrEmpty(entry))
                                        BannedIPs.Add(entry);
                                }
                            }
                        }
                    }
                    EnableServerIpAutoNegotiation = GetValueOrDefault(
                        config,
                        "enable_server_ip_auto_negotiation",
                        EnableServerIpAutoNegotiation
                    );
                    UsePublicIp = GetValueOrDefault(config, "use_public_ip", UsePublicIp);
                    var tempVerificationIp = GetValueOrDefault(
                        config,
                        "fallback_server_public_ip",
                        FallbackServerPublicIp
                    );
                    if (IPAddress.TryParse(tempVerificationIp, out _))
                        FallbackServerPublicIp = tempVerificationIp;
                    else
                    {
                        tempVerificationIp = GetValueOrDefault(
                            config,
                            "fallback_server_ip",
                            FallbackServerPublicIp
                        );
                        if (IPAddress.TryParse(tempVerificationIp, out _))
                            FallbackServerPublicIp = tempVerificationIp;
                    }
                    tempVerificationIp = GetValueOrDefault(
                        config,
                        "server_ip_override",
                        ServerIpOverride
                    );
                    if (IPAddress.TryParse(tempVerificationIp, out _))
                        ServerIpOverride = tempVerificationIp;
                    if (config.TryGetProperty("http_rate", out var httprateElement))
                    {
                        RatePermitLimit = httprateElement.GetProperty("permit_limit").GetInt32();
                        RateWindowSeconds = httprateElement.GetProperty("window_seconds").GetInt32();
                        RateSegmentsPerWindow = httprateElement.GetProperty("segments_per_window").GetInt32();
                        RateQueueLimit = httprateElement.GetProperty("queue_limit").GetInt32();
                    }
                    if (config.TryGetProperty("snmp", out var snmpElement))
                    {
                        EnableSNMPReports = snmpElement.GetProperty("enable").GetBoolean();
                        SNMPTrapHost = snmpElement.GetProperty("trap_host").GetString();
                        SNMPEnterpriseOid = snmpElement.GetProperty("enterprise_oid").GetString();
                        SNMPHashAlgorithm = new HashAlgorithmName(
                            snmpElement.GetProperty("hash_algorithm").GetString()
                        );
                        SNMPUserName = snmpElement.GetProperty("username").GetString();
                        SNMPAuthPassword = snmpElement.GetProperty("auth_password").GetString();
                        SNMPPrivatePassword = snmpElement
                            .GetProperty("private_password")
                            .GetString();
                    }
                    if (config.TryGetProperty("proxy", out var proxyElement))
                    {
                        ProxyUserName = proxyElement.GetProperty("user_name").GetString();
                        ProxyPassword = proxyElement.GetProperty("password").GetString();
                        ProxyHost = proxyElement.GetProperty("host").GetString();
                        ProxyPort = proxyElement.GetProperty("port").GetUInt16();
                    }
                    if (config.TryGetProperty("vpnban", out var vpnbanElement))
                    {
                        VpnCheck = vpnbanElement.GetProperty("enable").GetBoolean()
                            ? new VpnChecker(vpnbanElement.GetProperty("ipqs_api_key").GetString())
                            : null;
                    }
                    if (config.TryGetProperty("geo", out var geoElement))
                        GeoLandJsonPath = geoElement.GetProperty("land_json_path").GetString();
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogWarn(
                    $"{configPath} file is malformed (exception: {ex}), using server's default."
                );
            }
        }

        // Helper method to get a value or default value if not present
        private static T GetValueOrDefault<T>(
            JsonElement config,
            string propertyName,
            T defaultValue
        )
        {
            try
            {
                if (config.TryGetProperty(propertyName, out var value))
                {
                    var extractedValue = JsonSerializer.Deserialize<T>(value.GetRawText());
                    return extractedValue == null ? defaultValue : extractedValue;
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[Program] - GetValueOrDefault thrown an exception: {ex}");
            }

            return defaultValue;
        }
    }
}
