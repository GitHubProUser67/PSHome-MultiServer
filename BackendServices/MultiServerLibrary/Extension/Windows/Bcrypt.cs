using System.Runtime.InteropServices;

namespace MultiServerLibrary.Extension.Windows
{
    // From: https://github.com/PrivateServerEmulator/ME3PSE/blob/master/ME3Server_WV/SSL3SupportCheck.cs
    public partial class Bcrypt
    {
        [LibraryImport("bcrypt.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static partial int BCryptAddContextFunction(
            ConfigurationTable dwTable,
            string pszContext,
            CryptographicInterface dwInterface,
            string pszFunction,
            FunctionPosition dwPosition
        );

        [LibraryImport("bcrypt.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static partial int BCryptRemoveContextFunction(
            ConfigurationTable dwTable,
            string pszContext,
            CryptographicInterface dwInterface,
            string pszFunction
        );

        [LibraryImport("bcrypt.dll", StringMarshalling = StringMarshalling.Utf16)]
        public static partial int BCryptEnumContextFunctions(
            ConfigurationTable dwTable,
            string pszContext,
            CryptographicInterface dwInterface,
            ref uint pcbBuffer,
            out IntPtr ppBuffer
        );

        [LibraryImport("bcrypt.dll")]
        public static partial void BCryptFreeBuffer(IntPtr pvBuffer);

        public enum FunctionPosition : uint
        {
            CRYPT_PRIORITY_TOP = 0x00000000,
            CRYPT_PRIORITY_BOTTOM = 0xFFFFFFFF,
        }

        public enum CryptographicInterface : uint
        {
            BCRYPT_ASYMMETRIC_ENCRYPTION_INTERFACE = 0x00000003,
            BCRYPT_CIPHER_INTERFACE = 0x00000001,
            BCRYPT_HASH_INTERFACE = 0x00000002,
            BCRYPT_RNG_INTERFACE = 0x00000006,
            BCRYPT_SECRET_AGREEMENT_INTERFACE = 0x00000004,
            BCRYPT_SIGNATURE_INTERFACE = 0x00000005,
            NCRYPT_KEY_STORAGE_INTERFACE = 0x00010001,
            NCRYPT_SCHANNEL_INTERFACE = 0x00010002,
            NCRYPT_SCHANNEL_SIGNATURE_INTERFACE = 0x00010003,
        }

        public enum ConfigurationTable : uint
        {
            CRYPT_LOCAL = 0x00000001,
            CRYPT_DOMAIN = 0x00000002,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CRYPT_CONTEXT_FUNCTIONS
        {
            public int cFunctions;
            public IntPtr rgpszFunctions;
        }
    }
}
