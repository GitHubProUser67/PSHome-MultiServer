using System;
using Tpm2Lib;

namespace NetHasher
{
    public class DotNetHasher
    {
        private const string MD5Const = "MD5";
        private const string Sha1Const = "SHA1";
        private const string Sha224Const = "SHA224";
        private const string Sha256Const = "SHA256";
        private const string Sha384Const = "SHA384";
        private const string Sha512Const = "SHA512";

        public static byte[] ComputeMD5(object input)
        {
            byte[] result = HashCompute.ComputeObject(input, MD5Const);

            if (result.Length != 16)
                throw new InvalidOperationException("[DotNetHasher] - ComputeMD5 - The computed MD5 hash is not 16 bytes long.");

            return result;
        }

        public static string ComputeMD5String(object input)
        {
            return BitConverter.ToString(ComputeMD5(input)).Replace("-", string.Empty);
        }

        public static byte[] ComputeSHA1(object input)
        {
            byte[] result = null;
            Tpm2 _tpm = null;

            if (input is byte[])
            {
                try
                {
                    TbsDevice _crypto_device = new TbsDevice();
                    _crypto_device.Connect();
                    _tpm = new Tpm2(_crypto_device);

                    result = _tpm.Hash((byte[])input,
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
                result = HashCompute.ComputeObject(input, Sha1Const);

            if (result.Length != 20)
                throw new InvalidOperationException("[DotNetHasher] - ComputeSHA1 - The computed SHA1 hash is not 20 bytes long.");

            return result;
        }

        public static string ComputeSHA1String(object input)
        {
            return BitConverter.ToString(ComputeSHA1(input)).Replace("-", string.Empty);
        }

        public static byte[] ComputeSHA224(object input)
        {
            byte[] result = HashCompute.ComputeObject(input, Sha224Const);

            if (result.Length != 28)
                throw new InvalidOperationException("[DotNetHasher] - ComputeSHA224 - The computed SHA224 hash is not 28 bytes long.");

            return result;
        }

        public static string ComputeSHA224String(object input)
        {
            return BitConverter.ToString(ComputeSHA224(input)).Replace("-", string.Empty);
        }

        public static byte[] ComputeSHA256(object input)
        {
            byte[] result = null;
            Tpm2 _tpm = null;

            if (input is byte[])
            {
                try
                {
                    TbsDevice _crypto_device = new TbsDevice();
                    _crypto_device.Connect();
                    _tpm = new Tpm2(_crypto_device);

                    result = _tpm.Hash((byte[])input,
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
                result = HashCompute.ComputeObject(input, Sha256Const);

            if (result.Length != 32)
                throw new InvalidOperationException("[DotNetHasher] - ComputeSHA256 - The computed SHA256 hash is not 32 bytes long.");

            return result;
        }

        public static string ComputeSHA256String(object input)
        {
            return BitConverter.ToString(ComputeSHA256(input)).Replace("-", string.Empty);
        }

        public static byte[] ComputeSHA384(object input)
        {
            byte[] result = null;
            Tpm2 _tpm = null;

            if (input is byte[])
            {
                try
                {
                    TbsDevice _crypto_device = new TbsDevice();
                    _crypto_device.Connect();
                    _tpm = new Tpm2(_crypto_device);

                    result = _tpm.Hash((byte[])input,
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
                result = HashCompute.ComputeObject(input, Sha384Const);

            if (result.Length != 48)
                throw new InvalidOperationException("[DotNetHasher] - ComputeSHA384 - The computed SHA384 hash is not 48 bytes long.");

            return result;
        }

        public static string ComputeSHA384String(object input)
        {
            return BitConverter.ToString(ComputeSHA384(input)).Replace("-", string.Empty);
        }

        public static byte[] ComputeSHA512(object input)
        {
            byte[] result = null;
            Tpm2 _tpm = null;

            if (input is byte[])
            {
                try
                {
                    TbsDevice _crypto_device = new TbsDevice();
                    _crypto_device.Connect();
                    _tpm = new Tpm2(_crypto_device);

                    result = _tpm.Hash((byte[])input,
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
                result = HashCompute.ComputeObject(input, Sha512Const);

            if (result.Length != 64)
                throw new InvalidOperationException("[DotNetHasher] - ComputeSHA512 - The computed SHA512 hash is not 64 bytes long.");

            return result;
        }

        public static string ComputeSHA512String(object input)
        {
            return BitConverter.ToString(ComputeSHA512(input)).Replace("-", string.Empty);
        }
    }
}
