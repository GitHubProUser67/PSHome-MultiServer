using EndianTools;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NetHasher
{
    public class HashCompute
    {
        private static readonly bool isLittleEndianSystem = BitConverter.IsLittleEndian;

        public static byte[] ComputeObject(object inData, string hashName, byte[] HMACKey = null)
        {
            if (inData is byte[] bytes)
                return ComputeHash(bytes, hashName, HMACKey);
            else if (inData is string str)
                return ComputeHash(Encoding.Unicode.GetBytes(str), hashName, HMACKey);
            else if (inData is Stream stream)
                return ComputeStreamHash(stream, hashName, HMACKey);
            else if (inData is byte b)
                return ComputeHash(new[] { b }, hashName, HMACKey);
            else if (inData is short s)
                return ComputeHash(BitConverter.GetBytes(!isLittleEndianSystem ? EndianUtils.ReverseShort(s) : s), hashName, HMACKey);
            else if (inData is ushort us)
                return ComputeHash(BitConverter.GetBytes(!isLittleEndianSystem ? EndianUtils.ReverseUshort(us) : us), hashName, HMACKey);
            else if (inData is char c)
                return ComputeHash(BitConverter.GetBytes(!isLittleEndianSystem ? EndianUtils.ReverseChar(c) : c), hashName, HMACKey);
            else if (inData is int i)
                return ComputeHash(BitConverter.GetBytes(!isLittleEndianSystem ? EndianUtils.ReverseInt(i) : i), hashName, HMACKey);
            else if (inData is uint ui)
                return ComputeHash(BitConverter.GetBytes(!isLittleEndianSystem ? EndianUtils.ReverseUint(ui) : ui), hashName, HMACKey);
            else if (inData is long l)
                return ComputeHash(BitConverter.GetBytes(!isLittleEndianSystem ? EndianUtils.ReverseLong(l) : l), hashName, HMACKey);
            else if (inData is ulong ul)
                return ComputeHash(BitConverter.GetBytes(!isLittleEndianSystem ? EndianUtils.ReverseUlong(ul) : ul), hashName, HMACKey);
            else if (inData is float f)
                return ComputeHash(BitConverter.GetBytes(!isLittleEndianSystem ? EndianUtils.ReverseFloat(f) : f), hashName, HMACKey);
            else if (inData is double d)
                return ComputeHash(BitConverter.GetBytes(!isLittleEndianSystem ? EndianUtils.ReverseDouble(d) : d), hashName, HMACKey);

            var t = inData.GetType();

            if (t.IsArray)
            {
                var elemType = t.GetElementType();

                if (elemType == typeof(short))
                    return ComputeHash(ToByteArray((short[])inData), hashName, HMACKey);
                else if (elemType == typeof(ushort))
                    return ComputeHash(ToByteArray((ushort[])inData), hashName, HMACKey);
                else if (elemType == typeof(char))
                    return ComputeHash(ToByteArray((char[])inData), hashName, HMACKey);
                else if (elemType == typeof(int))
                    return ComputeHash(ToByteArray((int[])inData), hashName, HMACKey);
                else if (elemType == typeof(uint))
                    return ComputeHash(ToByteArray((uint[])inData), hashName, HMACKey);
                else if (elemType == typeof(long))
                    return ComputeHash(ToByteArray((long[])inData), hashName, HMACKey);
                else if (elemType == typeof(ulong))
                    return ComputeHash(ToByteArray((ulong[])inData), hashName, HMACKey);
                else if (elemType == typeof(float))
                    return ComputeHash(ToByteArray((float[])inData), hashName, HMACKey);
                else if (elemType == typeof(double))
                    return ComputeHash(ToByteArray((double[])inData), hashName, HMACKey);
            }

            throw new ArgumentException($"[HashCompute] - ComputeObject - Unsupported data type: {inData.GetType()}", nameof(inData));
        }

        private static byte[] ComputeHash(byte[] data, string hashName, byte[] HMACKey)
        {
            if (HMACKey != null && HMACKey.Length > 0)
            {
                if (hashName == "SHA224")
                    return ComputeHmacSha224Hash(data, HMACKey);
                else
                {
                    HMAC hmac = hashName switch
                    {
                        "MD5" => new HMACMD5(HMACKey),
                        "SHA1" => new HMACSHA1(HMACKey),
                        "SHA256" => new HMACSHA256(HMACKey),
                        "SHA384" => new HMACSHA384(HMACKey),
                        "SHA512" => new HMACSHA512(HMACKey),
                        _ => throw new ArgumentException($"[HashCompute] - ComputeHash - Unknown HMAC algorithm: {hashName}")
                    };

                    using (hmac)
                        return hmac.ComputeHash(data);
                }
            }
            else
            {
                return hashName switch
                {
                    "MD5" => ComputeMD5Hash(data),
                    "SHA1" => ComputeSha1Hash(data),
                    "SHA224" => ComputeSha224Hash(data),
                    "SHA256" => ComputeSha256Hash(data),
                    "SHA384" => ComputeSha384Hash(data),
                    "SHA512" => ComputeSha512Hash(data),
                    _ => ComputeWithHashAlgorithm(data, hashName)
                };
            }
        }

        private static byte[] ComputeStreamHash(Stream stream, string hashName, byte[] HMACKey)
        {
            if (HMACKey != null && HMACKey.Length > 0)
                throw new NotSupportedException($"[HashCompute] - ComputeStreamHash - HMAC algorithms not supported while using streams.");

            using (var algorithm = HashAlgorithm.Create(hashName))
            {
                if (algorithm == null)
                    throw new ArgumentException($"[HashCompute] - ComputeStreamHash - Unknown hash algorithm: {hashName}");
                return algorithm.ComputeHash(stream);
            }
        }

        private static byte[] ComputeWithHashAlgorithm(byte[] data, string hashName)
        {
            using (var algorithm = HashAlgorithm.Create(hashName))
            {
                if (algorithm == null)
                    throw new ArgumentException($"[HashCompute] - ComputeWithHashAlgorithm - Unknown hash algorithm: {hashName}");
                return algorithm.ComputeHash(data);
            }
        }

        private static byte[] ComputeMD5Hash(byte[] data)
        {
            MD5Digest digest = new MD5Digest();
            digest.BlockUpdate(data, 0, data.Length);
            byte[] hashBuf = new byte[digest.GetDigestSize()];
            digest.DoFinal(hashBuf, 0);
            return hashBuf;
        }

        private static byte[] ComputeSha1Hash(byte[] data)
        {
            Sha1Digest digest = new Sha1Digest();
            digest.BlockUpdate(data, 0, data.Length);
            byte[] hashBuf = new byte[digest.GetDigestSize()];
            digest.DoFinal(hashBuf, 0);
            return hashBuf;
        }

        private static byte[] ComputeSha224Hash(byte[] data)
        {
            Sha224Digest digest = new Sha224Digest();
            digest.BlockUpdate(data, 0, data.Length);
            byte[] hashBuf = new byte[digest.GetDigestSize()];
            digest.DoFinal(hashBuf, 0);
            return hashBuf;
        }

        private static byte[] ComputeHmacSha224Hash(byte[] data, byte[] key)
        {
            HMac hmac = new HMac(new Sha224Digest());
            hmac.Init(new KeyParameter(key));
            hmac.BlockUpdate(data, 0, data.Length);
            byte[] hashBuf = new byte[hmac.GetMacSize()];
            hmac.DoFinal(hashBuf, 0);
            return hashBuf;
        }

        private static byte[] ComputeSha256Hash(byte[] data)
        {
            Sha256Digest digest = new Sha256Digest();
            digest.BlockUpdate(data, 0, data.Length);
            byte[] hashBuf = new byte[digest.GetDigestSize()];
            digest.DoFinal(hashBuf, 0);
            return hashBuf;
        }

        private static byte[] ComputeSha384Hash(byte[] data)
        {
            Sha384Digest digest = new Sha384Digest();
            digest.BlockUpdate(data, 0, data.Length);
            byte[] hashBuf = new byte[digest.GetDigestSize()];
            digest.DoFinal(hashBuf, 0);
            return hashBuf;
        }

        private static byte[] ComputeSha512Hash(byte[] data)
        {
            Sha512Digest digest = new Sha512Digest();
            digest.BlockUpdate(data, 0, data.Length);
            byte[] hashBuf = new byte[digest.GetDigestSize()];
            digest.DoFinal(hashBuf, 0);
            return hashBuf;
        }

        private static byte[] ToByteArray(short[] values)
        {
            byte[] result = new byte[values.Length * sizeof(short)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }

        private static byte[] ToByteArray(ushort[] values)
        {
            byte[] result = new byte[values.Length * sizeof(ushort)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }

        private static byte[] ToByteArray(char[] values)
        {
            byte[] result = new byte[values.Length * sizeof(char)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }

        private static byte[] ToByteArray(int[] values)
        {
            byte[] result = new byte[values.Length * sizeof(int)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }

        private static byte[] ToByteArray(uint[] values)
        {
            byte[] result = new byte[values.Length * sizeof(uint)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }

        private static byte[] ToByteArray(long[] values)
        {
            byte[] result = new byte[values.Length * sizeof(long)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }

        private static byte[] ToByteArray(ulong[] values)
        {
            byte[] result = new byte[values.Length * sizeof(ulong)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }

        private static byte[] ToByteArray(float[] values)
        {
            byte[] result = new byte[values.Length * sizeof(float)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }

        private static byte[] ToByteArray(double[] values)
        {
            byte[] result = new byte[values.Length * sizeof(double)];
            Buffer.BlockCopy(values, 0, result, 0, result.Length);
            return result;
        }
    }
}
