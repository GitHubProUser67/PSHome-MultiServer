using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Math;
using System;

namespace Horizon.RT.Cryptography.RSA
{
    [JsonConverter(typeof(RsaKeyPairConverter))]
    public class RsaKeyPair
    {
        public BigInteger N { get; protected set; }
        public BigInteger E { get; protected set; }
        public BigInteger D { get; protected set; }

        public RsaKeyPair()
        {

        }

        public RsaKeyPair(BigInteger n, BigInteger e, BigInteger d)
        {
            N = n;
            E = e;
            D = d;
        }

        public PS2_RSA ToPS2() => new PS2_RSA(N, E, D);
        public PS3_RSA ToPS3() => new PS3_RSA(N, E, D);
    }

    public class RsaKeyPairConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            RsaKeyPair rsa = (RsaKeyPair)value;

            writer.WriteStartObject();
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
            return new RsaKeyPair(new BigInteger((string)jsonObject["n"], 10), new BigInteger((string)jsonObject["e"], 10), new BigInteger((string)jsonObject["d"], 10));
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(RsaKeyPair);
        }
    }
}