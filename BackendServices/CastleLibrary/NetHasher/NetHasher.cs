using System;
using Tpm2Lib;

namespace NetHasher
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
            byte[] result = HashCompute.ComputeObject(input, MD5Const, HMACKey);

            if (result.Length != 16)
                throw new InvalidOperationException("[DotNetHasher] - ComputeMD5 - The computed MD5 hash is not 16 bytes long.");

            return result;
        }

        public static string ComputeMD5String(object input, byte[] HMACKey = null)
        {
            return BitConverter.ToString(ComputeMD5(input, HMACKey)).Replace("-", string.Empty);
        }

        public static byte[] ComputeSHA1(object input, byte[] HMACKey = null)
        {
            byte[] result = null;
            Tpm2 _tpm = null;

            if ((HMACKey == null || HMACKey.Length == 0) && input is byte[] v)
            {
                try
                {
                    TbsDevice _crypto_device = new TbsDevice();
                    _crypto_device.Connect();
                    _tpm = new Tpm2(_crypto_device);

                    result = _tpm.Hash(v,
                           TpmAlgId.Sha1,
                           TpmRh.Owner,
                           out _);
                }
                catch
                {
                    // Fallback to classic HashFactory Methods.
                }
                finally
                {
                    if (_tpm != null) _tpm.Dispose();
                }
            }

            if (result == null)
                result = HashCompute.ComputeObject(input, Sha1Const, HMACKey);

            if (result.Length != 20)
                throw new InvalidOperationException("[DotNetHasher] - ComputeSHA1 - The computed SHA1 hash is not 20 bytes long.");

            return result;
        }

        public static string ComputeSHA1String(object input, byte[] HMACKey = null)
        {
            return BitConverter.ToString(ComputeSHA1(input, HMACKey)).Replace("-", string.Empty);
        }

        public static byte[] ComputeSHA224(object input, byte[] HMACKey = null)
        {
            byte[] result = HashCompute.ComputeObject(input, Sha224Const, HMACKey);

            if (result.Length != 28)
                throw new InvalidOperationException("[DotNetHasher] - ComputeSHA224 - The computed SHA224 hash is not 28 bytes long.");

            return result;
        }

        public static string ComputeSHA224String(object input, byte[] HMACKey = null)
        {
            return BitConverter.ToString(ComputeSHA224(input, HMACKey)).Replace("-", string.Empty);
        }

        public static byte[] ComputeSHA256(object input, byte[] HMACKey = null)
        {
            byte[] result = null;
            Tpm2 _tpm = null;

            if ((HMACKey == null || HMACKey.Length == 0) && input is byte[] v)
            {
                try
                {
                    TbsDevice _crypto_device = new TbsDevice();
                    _crypto_device.Connect();
                    _tpm = new Tpm2(_crypto_device);

                    result = _tpm.Hash(v,
                           TpmAlgId.Sha256,
                           TpmRh.Owner,
                           out _);
                }
                catch
                {
                    // Fallback to classic HashFactory Methods.
                }
                finally
                {
                    if (_tpm != null) _tpm.Dispose();
                }
            }

            if (result == null)
                result = HashCompute.ComputeObject(input, Sha256Const, HMACKey);

            if (result.Length != 32)
                throw new InvalidOperationException("[DotNetHasher] - ComputeSHA256 - The computed SHA256 hash is not 32 bytes long.");

            return result;
        }

        public static string ComputeSHA256String(object input, byte[] HMACKey = null)
        {
            return BitConverter.ToString(ComputeSHA256(input, HMACKey)).Replace("-", string.Empty);
        }

        public static byte[] ComputeSHA384(object input, byte[] HMACKey = null)
        {
            byte[] result = null;
            Tpm2 _tpm = null;

            if ((HMACKey == null || HMACKey.Length == 0) && input is byte[] v)
            {
                try
                {
                    TbsDevice _crypto_device = new TbsDevice();
                    _crypto_device.Connect();
                    _tpm = new Tpm2(_crypto_device);

                    result = _tpm.Hash(v,
                           TpmAlgId.Sha384,
                           TpmRh.Owner,
                           out _);
                }
                catch
                {
                    // Fallback to classic HashFactory Methods.
                }
                finally
                {
                    if (_tpm != null) _tpm.Dispose();
                }
            }

            if (result == null)
                result = HashCompute.ComputeObject(input, Sha384Const, HMACKey);

            if (result.Length != 48)
                throw new InvalidOperationException("[DotNetHasher] - ComputeSHA384 - The computed SHA384 hash is not 48 bytes long.");

            return result;
        }

        public static string ComputeSHA384String(object input, byte[] HMACKey = null)
        {
            return BitConverter.ToString(ComputeSHA384(input, HMACKey)).Replace("-", string.Empty);
        }

        public static byte[] ComputeSHA512(object input, byte[] HMACKey = null)
        {
            byte[] result = null;
            Tpm2 _tpm = null;

            if ((HMACKey == null || HMACKey.Length == 0) && input is byte[] v)
            {
                try
                {
                    TbsDevice _crypto_device = new TbsDevice();
                    _crypto_device.Connect();
                    _tpm = new Tpm2(_crypto_device);

                    result = _tpm.Hash(v,
                           TpmAlgId.Sha512,
                           TpmRh.Owner,
                           out _);
                }
                catch
                {
                    // Fallback to classic HashFactory Methods.
                }
                finally
                {
                    if (_tpm != null) _tpm.Dispose();
                }
            }

            if (result == null)
                result = HashCompute.ComputeObject(input, Sha512Const, HMACKey);

            if (result.Length != 64)
                throw new InvalidOperationException("[DotNetHasher] - ComputeSHA512 - The computed SHA512 hash is not 64 bytes long.");

            return result;
        }

        public static string ComputeSHA512String(object input, byte[] HMACKey = null)
        {
            return BitConverter.ToString(ComputeSHA512(input, HMACKey)).Replace("-", string.Empty);
        }
    }
}
