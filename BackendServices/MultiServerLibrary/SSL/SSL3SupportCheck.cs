using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using CustomLogger;
using Microsoft.Win32;
using static MultiServerLibrary.Extension.Windows.Bcrypt;

namespace MultiServerLibrary.SSL
{
    // Adapted from: https://github.com/PrivateServerEmulator/ME3PSE/blob/master/ME3Server_WV/SSL3SupportCheck.cs
    public class SSL3SupportCheck
    {
        public const string ssl3serverpath =
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\SSL 3.0\Server";
        public const string ssl3clientpath =
            @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\SSL 3.0\Client";

        private static readonly (string Path, string Name, int Value)[] Ciphers = new[]
        {
            (
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Ciphers\RC4 128/128",
                "Enabled",
                unchecked((int)0xFFFFFFFF)
            ),
        };

        private static readonly (string Path, string Name, int Value)[] Hashes = new[]
        {
            (
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Hashes\MD5",
                "Enabled",
                unchecked((int)0xFFFFFFFF)
            ),
            (
                @"HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Hashes\SHA",
                "Enabled",
                unchecked((int)0xFFFFFFFF)
            ),
        };

        [SupportedOSPlatform("windows")]
        public static void PerformSSL3Checks()
        {
            if (!CheckCipherSuites())
            {
                if (EnableCipherSuites())
                    LoggerAccessor.LogInfo(
                        "[SSL3SupportCheck] - PerformSSL3Checks: Cipher suites: successfully enabled by MultiServer."
                    );
                else
                    LoggerAccessor.LogWarn(
                        "[SSL3SupportCheck] - PerformSSL3Checks: Cipher suites: attempt to enable by MultiServer has failed."
                    );
            }
            else
                LoggerAccessor.LogInfo(
                    "[SSL3SupportCheck] - PerformSSL3Checks: Cipher suites: verification OK."
                );

            if (Environment.OSVersion.Version.Build < 19041) // Windows 10 version 2004
                return;

            if (GetSSL3ClientServerStatus())
                LoggerAccessor.LogInfo(
                    "[SSL3SupportCheck] - PerformSSL3Checks: SSL3 Client/Server: enabled - registry verification OK."
                );
            else if (EnableSSL3ClientServer())
                LoggerAccessor.LogInfo(
                    "[SSL3SupportCheck] - PerformSSL3Checks: SSL3 Client/Server: successfully enabled by MultiServer."
                );
            else
                LoggerAccessor.LogError(
                    "[SSL3SupportCheck] - PerformSSL3Checks: SSL3 Client/Server: attempt to enable by MultiServer has failed."
                );

            ApplySchannelSettings();
        }

        private static List<string> GetCipherSuiteList()
        {
            uint size = 0;
            var res = new List<string>();
            BCryptEnumContextFunctions(
                ConfigurationTable.CRYPT_LOCAL,
                "SSL",
                CryptographicInterface.NCRYPT_SCHANNEL_INTERFACE,
                ref size,
                out var ptrBuffer
            );
            var ccf = (CRYPT_CONTEXT_FUNCTIONS)
                Marshal.PtrToStructure(ptrBuffer, typeof(CRYPT_CONTEXT_FUNCTIONS));
            for (var i = 0; i < ccf.cFunctions; i++)
                res.Add(
                    Marshal.PtrToStringUni(
                        Marshal.ReadIntPtr(ccf.rgpszFunctions + (IntPtr.Size * i))
                    )
                );
            BCryptFreeBuffer(ptrBuffer);
            return res;
        }

        private static bool AddCipherSuite(string strCipherSuite, bool top = false)
        {
            return BCryptAddContextFunction(
                    ConfigurationTable.CRYPT_LOCAL,
                    "SSL",
                    CryptographicInterface.NCRYPT_SCHANNEL_INTERFACE,
                    strCipherSuite,
                    top
                        ? FunctionPosition.CRYPT_PRIORITY_TOP
                        : FunctionPosition.CRYPT_PRIORITY_BOTTOM
                ) == 0;
        }

        private static bool CheckCipherSuites()
        {
            var list = GetCipherSuiteList();
            return list.Contains("TLS_RSA_WITH_RC4_128_SHA")
                || list.Contains("TLS_RSA_WITH_RC4_128_MD5");
        }

        private static bool EnableCipherSuites()
        {
            return AddCipherSuite("TLS_RSA_WITH_RC4_128_SHA")
                || AddCipherSuite("TLS_RSA_WITH_RC4_128_MD5");
        }

        [SupportedOSPlatform("windows")]
        private static bool GetSSL3ClientServerStatus()
        {
            var ssl3resultServer = Registry.GetValue(ssl3serverpath, "Enabled", -1);
            var ssl3resultClient = Registry.GetValue(ssl3clientpath, "Enabled", -1);
            return (ssl3resultServer != null && ((int)ssl3resultServer == 1))
                && (ssl3resultClient != null && ((int)ssl3resultClient == 1));
        }

        [SupportedOSPlatform("windows")]
        private static bool EnableSSL3ClientServer()
        {
            try
            {
                Registry.SetValue(ssl3serverpath, "Enabled", 1, RegistryValueKind.DWord);
                Registry.SetValue(ssl3serverpath, "DisabledByDefault", 0, RegistryValueKind.DWord);
                Registry.SetValue(ssl3clientpath, "Enabled", 1, RegistryValueKind.DWord);
                Registry.SetValue(ssl3clientpath, "DisabledByDefault", 0, RegistryValueKind.DWord);
                return true;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    "[SSL3SupportCheck] - EnableSSL3ClientServer: " + ex.Message
                );
            }

            return false;
        }

        [SupportedOSPlatform("windows")]
        public static bool EnsureKeyExists(string path)
        {
            try
            {
                using var key = Registry.LocalMachine.CreateSubKey(
                    path.Replace(@"HKEY_LOCAL_MACHINE\", string.Empty)
                );
                return key != null;
            }
            catch (UnauthorizedAccessException)
            {
                // Expected when not running as admin → ignore or log as info
            }
            catch (System.Security.SecurityException)
            {
                // Also possible for insufficient privileges
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError(
                    $"[SSL3SupportCheck] - EnsureKeyExists: {path} - {ex.Message}"
                );
            }

            return false;
        }

        [SupportedOSPlatform("windows")]
        public static bool ApplySchannelSettings()
        {
            bool changesMade = false;

            foreach (var (path, name, value) in Ciphers)
            {
                if (EnsureKeyExists(path))
                {
                    try
                    {
                        var current = Registry.GetValue(path, name, null);
                        if (current == null || (int)current != value)
                        {
                            Registry.SetValue(path, name, value, RegistryValueKind.DWord);
                            changesMade = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(
                            "[SSL3SupportCheck] - ApplySchannelSettings - Ciphers error: "
                                + ex.Message
                        );
                        return false;
                    }
                }
            }

            foreach (var (path, name, value) in Hashes)
            {
                if (EnsureKeyExists(path))
                {
                    try
                    {
                        var current = Registry.GetValue(path, name, null);
                        if (current == null || (int)current != value)
                        {
                            Registry.SetValue(path, name, value, RegistryValueKind.DWord);
                            changesMade = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(
                            "[SSL3SupportCheck] - ApplySchannelSettings - Hashes error: "
                                + ex.Message
                        );
                        return false;
                    }
                }
            }

            if (changesMade)
                LoggerAccessor.LogInfo(
                    "[SSL3SupportCheck] - ApplySchannelSettings: SCHANNEL ciphers and hashes successfully configured by MultiServer.."
                );
            else
                LoggerAccessor.LogInfo(
                    "[SSL3SupportCheck] - ApplySchannelSettings: SCHANNEL settings: verification OK"
                );

            return true;
        }
    }
}
