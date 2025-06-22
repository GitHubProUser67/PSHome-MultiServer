using CustomLogger;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static NetworkLibrary.Extension.Microsoft.Bcrypt;

namespace NetworkLibrary.SSL
{
    // From: https://github.com/PrivateServerEmulator/ME3PSE/blob/master/ME3Server_WV/SSL3SupportCheck.cs
    public class SSL3SupportCheck
    {
        public static void PerformSSL3Checks()
        {
            if (!CheckCipherSuites())
            {
                if (EnableCipherSuites())
                    LoggerAccessor.LogInfo("[SSL3SupportCheck] - PerformSSL3Checks: Cipher suites: successfully enabled by MultiServer.");
                else
                    LoggerAccessor.LogWarn("[SSL3SupportCheck] - PerformSSL3Checks: Cipher suites: attempt to enable by MultiServer has failed.");
            }
            else
            {
                LoggerAccessor.LogInfo("[SSL3SupportCheck] - PerformSSL3Checks: Cipher suites: verification OK.");
            }

            if (Environment.OSVersion.Version.Build < 19041)
                return;

            if (GetSSL3ServerStatus() == 1)
            {
                LoggerAccessor.LogInfo("[SSL3SupportCheck] - PerformSSL3Checks: SSL3 Server: enabled - registry verification OK.");
                return;
            }
            if (EnableSSL3Server())
                LoggerAccessor.LogInfo("[SSL3SupportCheck] - PerformSSL3Checks: SSL3 Server: successfully enabled by MultiServer.");
            else
                LoggerAccessor.LogError("[SSL3SupportCheck] - PerformSSL3Checks: SSL3 Server: attempt to enable by MultiServer has failed.");
        }

        private static List<string> GetCipherSuiteList()
        {
            uint size = 0;
            List<string> res = new List<string>();
            BCryptEnumContextFunctions(ConfigurationTable.CRYPT_LOCAL, "SSL", CryptographicInterface.NCRYPT_SCHANNEL_INTERFACE, ref size, out IntPtr ptrBuffer);
            CRYPT_CONTEXT_FUNCTIONS ccf = (CRYPT_CONTEXT_FUNCTIONS)Marshal.PtrToStructure(ptrBuffer, typeof(CRYPT_CONTEXT_FUNCTIONS));
            for (int i = 0; i < ccf.cFunctions; i++)
            {
                res.Add(Marshal.PtrToStringUni(Marshal.ReadIntPtr(ccf.rgpszFunctions + (IntPtr.Size * i))));
            }
            BCryptFreeBuffer(ptrBuffer);
            return res;
        }

        private static bool AddCipherSuite(string strCipherSuite, bool top = false)
        {
            var x = BCryptAddContextFunction(ConfigurationTable.CRYPT_LOCAL, "SSL", CryptographicInterface.NCRYPT_SCHANNEL_INTERFACE,
                strCipherSuite, top ? FunctionPosition.CRYPT_PRIORITY_TOP : FunctionPosition.CRYPT_PRIORITY_BOTTOM);
            return x == 0;
        }

        private static int RemoveCipherSuite(string strCipherSuite)
        {
            return BCryptRemoveContextFunction(ConfigurationTable.CRYPT_LOCAL, "SSL", CryptographicInterface.NCRYPT_SCHANNEL_INTERFACE, strCipherSuite);
        }

        public static bool CheckCipherSuites()
        {
            var list = GetCipherSuiteList();
            return list.Contains("TLS_RSA_WITH_RC4_128_SHA") || list.Contains("TLS_RSA_WITH_RC4_128_MD5");
        }

        public static bool EnableCipherSuites()
        {
            return AddCipherSuite("TLS_RSA_WITH_RC4_128_SHA") || AddCipherSuite("TLS_RSA_WITH_RC4_128_MD5");
        }

        public static int GetSSL3ServerStatus()
        {
            object result = Registry.GetValue(ssl3serverpath, "Enabled", -1);
            if (result == null)
                return -2;
            return (int)result;
        }

        public static bool EnableSSL3Server()
        {
            try
            {
                Registry.SetValue(ssl3serverpath, "Enabled", 1, RegistryValueKind.DWord);
                return true;
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError("[SSL3SupportCheck] - EnableSSL3Server: " + ex.Message);
            }

            return false;
        }
    }
}
