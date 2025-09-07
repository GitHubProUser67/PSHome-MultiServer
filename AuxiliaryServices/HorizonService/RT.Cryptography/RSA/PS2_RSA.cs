using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Math;
using System;
using System.Linq;

namespace Horizon.RT.Cryptography.RSA
{
    [JsonConverter(typeof(RSAConverter))]
    public class PS2_RSA : ICipher
    {
        public CipherContext Context { get; protected set; } = CipherContext.RSA_AUTH;

        public string Comment = string.Empty;
        public BigInteger N { get; protected set; }
        public BigInteger E { get; protected set; }
        public BigInteger D { get; protected set; }

        public PS2_RSA(BigInteger n, BigInteger e, BigInteger d)
        {
            N = n;
            E = e;
            D = d;
        }

        private BigInteger Encrypt(BigInteger m)
        {
            return m.ModPow(E, N);
        }

        private BigInteger Decrypt(BigInteger c)
        {
            return c.ModPow(D, N);
        }

        public virtual bool Decrypt(byte[] input, byte[] hash, out byte[] plain)
        {
            plain = new byte[input.Length];
            if (input.Length > N.BitLength / 8)
                throw new NotImplementedException($"Unable to decrypt RSA cipher with length greater than key ({input.Length}).");

            // Check if empty hash
            // If hash is 0, the data is already in plaintext
            if (!IsHashValid(hash))
            {
                Array.Copy(input, 0, plain, 0, input.Length);
                return true;
            }

            // decrypt
            var plainBigInt = Decrypt(input.ToBigInteger());
            var plainBytes = plainBigInt.ToBA();
            Array.Copy(plainBytes, plain, plainBytes.Length);
            Hash(plain, out var ourHash);

            // if hashes don't match then try adding N
            // this accounts for when a value larger than N but less than 513 bits is encrypted
            if (!ourHash.SequenceEqual(hash))
            {
                // decrypt
                plainBytes = plainBigInt.Add(N).ToBA();
                Array.Copy(plainBytes, plain, plainBytes.Length);
                Hash(plain, out ourHash);
            }

            return ourHash.SequenceEqual(hash);
        }

        public virtual bool Encrypt(byte[] input, out byte[] cipher, out byte[] hash)
        {
            Hash(input, out hash);
            cipher = Encrypt(input.ToBigInteger()).ToBA(input.Length);
            return true;
        }

        public virtual void Hash(byte[] input, out byte[] hash)
        {
            hash = RT.Cryptography.Hash.SHA1.Hash(input, Context);
        }

        public virtual bool IsHashValid(byte[] hash)
        {
            if (hash == null || hash.Length != 4)
                return false;

            return !(hash[0] == 0 && hash[1] == 0 && hash[2] == 0 && (hash[3] & 0x1F) == 0);
        }

        #region Comparison

        public override bool Equals(object obj)
        {
            if (obj is PS2_RSA rsa)
                return rsa.Equals(this);

            return base.Equals(obj);
        }

        public bool Equals(PS2_RSA b)
        {
            return b.Context == Context &&
                b.N.CompareTo(N) == 0 &&
                b.E.CompareTo(E) == 0 &&
                b.D.CompareTo(D) == 0;
        }

        #endregion

        public byte[] GetPublicKey()
        {
            return N.ToByteArrayUnsigned();
        }

        public override string ToString()
        {
            return $"PS2_RSA({Context}, {N}, {E}, {D})";
        }
    }

    public class RSAConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            PS2_RSA rsa = (PS2_RSA)value;

            writer.WriteStartObject();
            writer.WritePropertyName("comment");
            serializer.Serialize(writer, rsa.Comment.ToString());
            writer.WritePropertyName("n");
            serializer.Serialize(writer, rsa.N.ToString());
            writer.WritePropertyName("e");
            serializer.Serialize(writer, rsa.E.ToString());
            writer.WritePropertyName("d");
            serializer.Serialize(writer, rsa.D.ToString());
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);

            var sN = (string)jsonObject["n"];
            var sE = (string)jsonObject["e"];
            var sD = (string)jsonObject["d"];
            return new PS2_RSA(new BigInteger(sN, 10), new BigInteger(sE, 10), new BigInteger(sD, 10)) { Comment = (string)jsonObject["comment"] };
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PS2_RSA);
        }
    }

    static class RSAUtils
    {
        public static byte[] ToBA(this BigInteger b)
        {
            return b.ToByteArrayUnsigned().Reverse().ToArray();
        }

        public static byte[] ToBA(this BigInteger b, int minLen)
        {
            var bytes = b.ToByteArrayUnsigned().Reverse().ToArray();
            if (bytes.Length < minLen)
                Array.Resize(ref bytes, minLen);
            return bytes;
        }

        public static BigInteger ToBigInteger(this byte[] ba)
        {
            return new BigInteger(1, ba.Reverse().ToArray());
        }

        public static BigInteger ToBigInteger(this byte[] ba, int startIndex, int length)
        {
            return new BigInteger(1, ba.Skip(startIndex).Take(length).Reverse().ToArray());
        }
    }
}