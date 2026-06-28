namespace CastleLibrary.NetHasher
{
    public class DotNetHasher
    {
        public const string MD5Const = "MD5";
        public const string Sha1Const = "SHA1";
        public const string Sha224Const = "SHA224";
        public const string Sha256Const = "SHA256";
        public const string Sha384Const = "SHA384";
        public const string Sha512Const = "SHA512";

        public static byte[] ComputeMD5(object input, byte[] HMACKey = null)
        {
            ArgumentNullException.ThrowIfNull(input);

            var result = HashCompute.ComputeObject(input, MD5Const, HMACKey);

            return result.Length != 16
                ? throw new InvalidOperationException(
                    "[DotNetHasher] - ComputeMD5 - The computed MD5 hash is not 16 bytes long."
                )
                : result;
        }

        public static string ComputeMD5String(object input, byte[] HMACKey = null)
        {
            return Convert.ToHexString(ComputeMD5(input, HMACKey));
        }

        public static byte[] ComputeSHA1(object input, byte[] HMACKey = null)
        {
            ArgumentNullException.ThrowIfNull(input);

            var result = HashCompute.ComputeObject(input, Sha1Const, HMACKey);

            return result.Length != 20
                ? throw new InvalidOperationException(
                    "[DotNetHasher] - ComputeSHA1 - The computed SHA1 hash is not 20 bytes long."
                )
                : result;
        }

        public static string ComputeSHA1String(object input, byte[] HMACKey = null)
        {
            return Convert.ToHexString(ComputeSHA1(input, HMACKey));
        }

        public static byte[] ComputeSHA224(object input, byte[] HMACKey = null)
        {
            ArgumentNullException.ThrowIfNull(input);

            var result = HashCompute.ComputeObject(input, Sha224Const, HMACKey);

            return result.Length != 28
                ? throw new InvalidOperationException(
                    "[DotNetHasher] - ComputeSHA224 - The computed SHA224 hash is not 28 bytes long."
                )
                : result;
        }

        public static string ComputeSHA224String(object input, byte[] HMACKey = null)
        {
            return Convert.ToHexString(ComputeSHA224(input, HMACKey));
        }

        public static byte[] ComputeSHA256(object input, byte[] HMACKey = null)
        {
            ArgumentNullException.ThrowIfNull(input);

            var result = HashCompute.ComputeObject(input, Sha256Const, HMACKey);

            return result.Length != 32
                ? throw new InvalidOperationException(
                    "[DotNetHasher] - ComputeSHA256 - The computed SHA256 hash is not 32 bytes long."
                )
                : result;
        }

        public static string ComputeSHA256String(object input, byte[] HMACKey = null)
        {
            return Convert.ToHexString(ComputeSHA256(input, HMACKey));
        }

        public static byte[] ComputeSHA384(object input, byte[] HMACKey = null)
        {
            ArgumentNullException.ThrowIfNull(input);

            var result = HashCompute.ComputeObject(input, Sha384Const, HMACKey);

            return result.Length != 48
                ? throw new InvalidOperationException(
                    "[DotNetHasher] - ComputeSHA384 - The computed SHA384 hash is not 48 bytes long."
                )
                : result;
        }

        public static string ComputeSHA384String(object input, byte[] HMACKey = null)
        {
            return Convert.ToHexString(ComputeSHA384(input, HMACKey));
        }

        public static byte[] ComputeSHA512(object input, byte[] HMACKey = null)
        {
            ArgumentNullException.ThrowIfNull(input);

            var result = HashCompute.ComputeObject(input, Sha512Const, HMACKey);

            return result.Length != 64
                ? throw new InvalidOperationException(
                    "[DotNetHasher] - ComputeSHA512 - The computed SHA512 hash is not 64 bytes long."
                )
                : result;
        }

        public static string ComputeSHA512String(object input, byte[] HMACKey = null)
        {
            return Convert.ToHexString(ComputeSHA512(input, HMACKey));
        }
    }
}
