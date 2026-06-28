using System.Text;
using MultiServerLibrary.Extension;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities.Encoders;

namespace WebAPIService.WebServices.WebCrypto
{
    public static class WebCryptoClass
    {
        public static readonly byte[] AuthIV =
        [
            0x30,
            0x57,
            0xB5,
            0x1F,
            0x32,
            0xD4,
            0xAD,
            0xBF,
            0xAA,
            0xAA,
            0x21,
            0x41,
            0x6C,
            0xDC,
            0x5D,
            0xF5,
        ];
        public static readonly byte[] IdentIV =
        [
            0x47,
            0x1A,
            0xD2,
            0xC3,
            0xA4,
            0x8B,
            0xF1,
            0xD9,
            0x22,
            0xBC,
            0xC7,
            0x61,
            0xFD,
            0x09,
            0x8E,
            0x3A,
        ];

        public static string EncryptCBC(
            object ObjectToEncrypt,
            string AccessKey,
            byte[] IV,
            bool xmlsecuretags = false,
            bool xmlbody = false
        )
        {
            var result = xmlbody
                ? InitiateCBCEncryptBufferTobase64String(
                    JsonConvert
                        .DeserializeXmlNode(
                            new JObject(
                                new JProperty(
                                    "ServerResult",
                                    JToken.Parse(
                                        JsonConvert.SerializeObject(
                                            ObjectToEncrypt,
                                            new JsonSerializerSettings
                                            {
                                                PreserveReferencesHandling =
                                                    PreserveReferencesHandling.Objects
                                                    | PreserveReferencesHandling.Arrays,
                                                Converters = { new JsonIPConverter() },
                                            }
                                        )
                                    )
                                )
                            ).ToString(),
                            "Root"
                        )
                        ?.OuterXml
                        ?? "<Root></Root>",
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                )
                : InitiateCBCEncryptBufferTobase64String(
                    JsonConvert.SerializeObject(
                        ObjectToEncrypt,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            PreserveReferencesHandling =
                                PreserveReferencesHandling.Objects
                                | PreserveReferencesHandling.Arrays,
                            Converters = { new JsonIPConverter() },
                        }
                    ),
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                );
            if (!string.IsNullOrEmpty(result) && xmlsecuretags)
                result = "<Secure>" + result + "</Secure>";

            return result;
        }

        public static byte[] EncryptToByteArrayCBC(
            object ObjectToEncrypt,
            string AccessKey,
            byte[] IV,
            bool xmlsecuretags = false,
            bool xmlbody = false
        )
        {
            var result = xmlbody
                ? InitiateCBCEncryptBufferTobase64String(
                    JsonConvert
                        .DeserializeXmlNode(
                            new JObject(
                                new JProperty(
                                    "ServerResult",
                                    JToken.Parse(
                                        JsonConvert.SerializeObject(
                                            ObjectToEncrypt,
                                            new JsonSerializerSettings
                                            {
                                                PreserveReferencesHandling =
                                                    PreserveReferencesHandling.Objects
                                                    | PreserveReferencesHandling.Arrays,
                                                Converters = { new JsonIPConverter() },
                                            }
                                        )
                                    )
                                )
                            ).ToString(),
                            "Root"
                        )
                        ?.OuterXml
                        ?? "<Root></Root>",
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                )
                : InitiateCBCEncryptBufferTobase64String(
                    JsonConvert.SerializeObject(
                        ObjectToEncrypt,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            PreserveReferencesHandling =
                                PreserveReferencesHandling.Objects
                                | PreserveReferencesHandling.Arrays,
                            Converters = { new JsonIPConverter() },
                        }
                    ),
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                );
            if (!string.IsNullOrEmpty(result) && xmlsecuretags)
                result = "<Secure>" + result + "</Secure>";

            return string.IsNullOrEmpty(result) ? null : Encoding.UTF8.GetBytes(result);
        }

        public static string EncryptNoPreserveCBC(
            object ObjectToEncrypt,
            string AccessKey,
            byte[] IV,
            bool xmlsecuretags = false,
            bool xmlbody = false
        )
        {
            var result = xmlbody
                ? InitiateCBCEncryptBufferTobase64String(
                    JsonConvert
                        .DeserializeXmlNode(
                            new JObject(
                                new JProperty(
                                    "ServerResult",
                                    JToken.Parse(
                                        JsonConvert.SerializeObject(
                                            ObjectToEncrypt,
                                            new JsonSerializerSettings
                                            {
                                                Converters = { new JsonIPConverter() },
                                            }
                                        )
                                    )
                                )
                            ).ToString(),
                            "Root"
                        )
                        ?.OuterXml
                        ?? "<Root></Root>",
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                )
                : InitiateCBCEncryptBufferTobase64String(
                    JsonConvert.SerializeObject(
                        ObjectToEncrypt,
                        Formatting.Indented,
                        new JsonSerializerSettings { Converters = { new JsonIPConverter() } }
                    ),
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                );
            if (!string.IsNullOrEmpty(result) && xmlsecuretags)
                result = "<Secure>" + result + "</Secure>";

            return result;
        }

        public static byte[] EncryptNoPreserveToByteArrayCBC(
            object ObjectToEncrypt,
            string AccessKey,
            byte[] IV,
            bool xmlsecuretags = false,
            bool xmlbody = false
        )
        {
            var result = xmlbody
                ? InitiateCBCEncryptBufferTobase64String(
                    JsonConvert
                        .DeserializeXmlNode(
                            new JObject(
                                new JProperty(
                                    "ServerResult",
                                    JToken.Parse(
                                        JsonConvert.SerializeObject(
                                            ObjectToEncrypt,
                                            new JsonSerializerSettings
                                            {
                                                Converters = { new JsonIPConverter() },
                                            }
                                        )
                                    )
                                )
                            ).ToString(),
                            "Root"
                        )
                        ?.OuterXml
                        ?? "<Root></Root>",
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                )
                : InitiateCBCEncryptBufferTobase64String(
                    JsonConvert.SerializeObject(
                        ObjectToEncrypt,
                        Formatting.Indented,
                        new JsonSerializerSettings { Converters = { new JsonIPConverter() } }
                    ),
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                );
            if (!string.IsNullOrEmpty(result) && xmlsecuretags)
                result = "<Secure>" + result + "</Secure>";

            return string.IsNullOrEmpty(result) ? null : Encoding.UTF8.GetBytes(result);
        }

        public static string DecryptCBC(string StringToDecrypt, string AccessKey, byte[] IV)
        {
            return Encoding.UTF8.GetString(
                InitiateCBCDecryptBuffer(
                    StringToDecrypt
                        .Replace("<Secure>", string.Empty)
                        .Replace("</Secure>", string.Empty)
                        .IsBase64()
                        .DecodedBytes,
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                ) ?? []
            );
        }

        public static string DecryptCBC(byte[] ByteArrayToDecrypt, string AccessKey, byte[] IV)
        {
            return Encoding.UTF8.GetString(
                InitiateCBCDecryptBuffer(
                    Encoding
                        .UTF8.GetString(ByteArrayToDecrypt)
                        .Replace("<Secure>", string.Empty)
                        .Replace("</Secure>", string.Empty)
                        .IsBase64()
                        .DecodedBytes,
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                ) ?? []
            );
        }

        public static byte[] DecryptToByteArrayCBC(
            string StringToDecrypt,
            string AccessKey,
            byte[] IV
        )
        {
            return InitiateCBCDecryptBuffer(
                StringToDecrypt
                    .Replace("<Secure>", string.Empty)
                    .Replace("</Secure>", string.Empty)
                    .IsBase64()
                    .DecodedBytes,
                AccessKey.IsBase64().DecodedBytes,
                IV
            );
        }

        public static byte[] DecryptToByteArrayCBC(
            byte[] ByteArrayToDecrypt,
            string AccessKey,
            byte[] IV
        )
        {
            return InitiateCBCDecryptBuffer(
                Encoding
                    .UTF8.GetString(ByteArrayToDecrypt)
                    .Replace("<Secure>", string.Empty)
                    .Replace("</Secure>", string.Empty)
                    .IsBase64()
                    .DecodedBytes,
                AccessKey.IsBase64().DecodedBytes,
                IV
            );
        }

        public static string EncryptCTR(
            object ObjectToEncrypt,
            string AccessKey,
            byte[] IV,
            bool xmlsecuretags = false,
            bool xmlbody = false
        )
        {
            var result = xmlbody
                ? InitiateCTRBufferTobase64String(
                    JsonConvert
                        .DeserializeXmlNode(
                            new JObject(
                                new JProperty(
                                    "ServerResult",
                                    JToken.Parse(
                                        JsonConvert.SerializeObject(
                                            ObjectToEncrypt,
                                            new JsonSerializerSettings
                                            {
                                                PreserveReferencesHandling =
                                                    PreserveReferencesHandling.Objects
                                                    | PreserveReferencesHandling.Arrays,
                                                Converters = { new JsonIPConverter() },
                                            }
                                        )
                                    )
                                )
                            ).ToString(),
                            "Root"
                        )
                        ?.OuterXml
                        ?? "<Root></Root>",
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                )
                : InitiateCTRBufferTobase64String(
                    JsonConvert.SerializeObject(
                        ObjectToEncrypt,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            PreserveReferencesHandling =
                                PreserveReferencesHandling.Objects
                                | PreserveReferencesHandling.Arrays,
                            Converters = { new JsonIPConverter() },
                        }
                    ),
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                );
            if (!string.IsNullOrEmpty(result) && xmlsecuretags)
                result = "<Secure>" + result + "</Secure>";

            return result;
        }

        public static byte[] EncryptToByteArrayCTR(
            object ObjectToEncrypt,
            string AccessKey,
            byte[] IV,
            bool xmlsecuretags = false,
            bool xmlbody = false
        )
        {
            var result = xmlbody
                ? InitiateCTRBufferTobase64String(
                    JsonConvert
                        .DeserializeXmlNode(
                            new JObject(
                                new JProperty(
                                    "ServerResult",
                                    JToken.Parse(
                                        JsonConvert.SerializeObject(
                                            ObjectToEncrypt,
                                            new JsonSerializerSettings
                                            {
                                                PreserveReferencesHandling =
                                                    PreserveReferencesHandling.Objects
                                                    | PreserveReferencesHandling.Arrays,
                                                Converters = { new JsonIPConverter() },
                                            }
                                        )
                                    )
                                )
                            ).ToString(),
                            "Root"
                        )
                        ?.OuterXml
                        ?? "<Root></Root>",
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                )
                : InitiateCTRBufferTobase64String(
                    JsonConvert.SerializeObject(
                        ObjectToEncrypt,
                        Formatting.Indented,
                        new JsonSerializerSettings
                        {
                            PreserveReferencesHandling =
                                PreserveReferencesHandling.Objects
                                | PreserveReferencesHandling.Arrays,
                            Converters = { new JsonIPConverter() },
                        }
                    ),
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                );
            if (!string.IsNullOrEmpty(result) && xmlsecuretags)
                result = "<Secure>" + result + "</Secure>";

            return string.IsNullOrEmpty(result) ? null : Encoding.UTF8.GetBytes(result);
        }

        public static string EncryptNoPreserveCTR(
            object ObjectToEncrypt,
            string AccessKey,
            byte[] IV,
            bool xmlsecuretags = false,
            bool xmlbody = false
        )
        {
            var result = xmlbody
                ? InitiateCTRBufferTobase64String(
                    JsonConvert
                        .DeserializeXmlNode(
                            new JObject(
                                new JProperty(
                                    "ServerResult",
                                    JToken.Parse(
                                        JsonConvert.SerializeObject(
                                            ObjectToEncrypt,
                                            new JsonSerializerSettings
                                            {
                                                Converters = { new JsonIPConverter() },
                                            }
                                        )
                                    )
                                )
                            ).ToString(),
                            "Root"
                        )
                        ?.OuterXml
                        ?? "<Root></Root>",
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                )
                : InitiateCTRBufferTobase64String(
                    JsonConvert.SerializeObject(
                        ObjectToEncrypt,
                        Formatting.Indented,
                        new JsonSerializerSettings { Converters = { new JsonIPConverter() } }
                    ),
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                );
            if (!string.IsNullOrEmpty(result) && xmlsecuretags)
                result = "<Secure>" + result + "</Secure>";

            return result;
        }

        public static byte[] EncryptNoPreserveToByteArrayCTR(
            object ObjectToEncrypt,
            string AccessKey,
            byte[] IV,
            bool xmlsecuretags = false,
            bool xmlbody = false
        )
        {
            var result = xmlbody
                ? InitiateCTRBufferTobase64String(
                    JsonConvert
                        .DeserializeXmlNode(
                            new JObject(
                                new JProperty(
                                    "ServerResult",
                                    JToken.Parse(
                                        JsonConvert.SerializeObject(
                                            ObjectToEncrypt,
                                            new JsonSerializerSettings
                                            {
                                                Converters = { new JsonIPConverter() },
                                            }
                                        )
                                    )
                                )
                            ).ToString(),
                            "Root"
                        )
                        ?.OuterXml
                        ?? "<Root></Root>",
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                )
                : InitiateCTRBufferTobase64String(
                    JsonConvert.SerializeObject(
                        ObjectToEncrypt,
                        Formatting.Indented,
                        new JsonSerializerSettings { Converters = { new JsonIPConverter() } }
                    ),
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                );
            if (!string.IsNullOrEmpty(result) && xmlsecuretags)
                result = "<Secure>" + result + "</Secure>";

            return string.IsNullOrEmpty(result) ? null : Encoding.UTF8.GetBytes(result);
        }

        public static string DecryptCTR(string StringToDecrypt, string AccessKey, byte[] IV)
        {
            return Encoding.UTF8.GetString(
                InitiateCTRBuffer(
                    StringToDecrypt
                        .Replace("<Secure>", string.Empty)
                        .Replace("</Secure>", string.Empty)
                        .IsBase64()
                        .DecodedBytes,
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                ) ?? []
            );
        }

        public static string DecryptCTR(byte[] ByteArrayToDecrypt, string AccessKey, byte[] IV)
        {
            return Encoding.UTF8.GetString(
                InitiateCTRBuffer(
                    Encoding
                        .UTF8.GetString(ByteArrayToDecrypt)
                        .Replace("<Secure>", string.Empty)
                        .Replace("</Secure>", string.Empty)
                        .IsBase64()
                        .DecodedBytes,
                    AccessKey.IsBase64().DecodedBytes,
                    IV
                ) ?? []
            );
        }

        public static byte[] DecryptToByteArrayCTR(
            string StringToDecrypt,
            string AccessKey,
            byte[] IV
        )
        {
            return InitiateCTRBuffer(
                StringToDecrypt
                    .Replace("<Secure>", string.Empty)
                    .Replace("</Secure>", string.Empty)
                    .IsBase64()
                    .DecodedBytes,
                AccessKey.IsBase64().DecodedBytes,
                IV
            );
        }

        public static byte[] DecryptToByteArrayCTR(
            byte[] ByteArrayToDecrypt,
            string AccessKey,
            byte[] IV
        )
        {
            return InitiateCTRBuffer(
                Encoding
                    .UTF8.GetString(ByteArrayToDecrypt)
                    .Replace("<Secure>", string.Empty)
                    .Replace("</Secure>", string.Empty)
                    .IsBase64()
                    .DecodedBytes,
                AccessKey.IsBase64().DecodedBytes,
                IV
            );
        }

        private static byte[] InitiateCBCDecryptBuffer(
            byte[] FileBytes,
            byte[] KeyBytes,
            byte[] m_iv
        )
        {
            if (KeyBytes.Length >= 16 && m_iv.Length == 16)
            {
                // Create the cipher
                var cipher = CipherUtilities.GetCipher("AES/CBC/OAEPWITHSHA224ANDMGF1PADDING");

                cipher.Init(false, new ParametersWithIV(new KeyParameter(KeyBytes), m_iv));

                // Encrypt the plaintext
                var ciphertextBytes = new byte[cipher.GetOutputSize(FileBytes.Length)];
                var ciphertextLength = cipher.ProcessBytes(
                    FileBytes,
                    0,
                    FileBytes.Length,
                    ciphertextBytes,
                    0
                );
                cipher.DoFinal(ciphertextBytes, ciphertextLength);
                return ciphertextBytes;
            }

            CustomLogger.LoggerAccessor.LogError(
                "[WebCrypto] - InitiateCBCDecryptBuffer - Invalid KeyBytes or IV!"
            );

            return null;
        }

        private static string InitiateCBCDecryptBufferTobase64String(
            string FileString,
            byte[] KeyBytes,
            byte[] m_iv
        )
        {
            if (KeyBytes.Length >= 16 && m_iv.Length == 16)
                return Base64.ToBase64String(
                    InitiateCBCDecryptBuffer(Encoding.UTF8.GetBytes(FileString), KeyBytes, m_iv)
                );

            CustomLogger.LoggerAccessor.LogError(
                "[WebCrypto] - InitiateCBCDecryptBufferTobase64String - Invalid KeyBytes or IV!"
            );

            return null;
        }

        private static byte[] InitiateCBCEncryptBuffer(
            byte[] FileBytes,
            byte[] KeyBytes,
            byte[] m_iv
        )
        {
            if (KeyBytes.Length >= 16 && m_iv.Length == 16)
            {
                // Create the cipher
                var cipher = CipherUtilities.GetCipher("AES/CBC/OAEPWITHSHA224ANDMGF1PADDING");

                cipher.Init(true, new ParametersWithIV(new KeyParameter(KeyBytes), m_iv));

                // Encrypt the plaintext
                var ciphertextBytes = new byte[cipher.GetOutputSize(FileBytes.Length)];
                var ciphertextLength = cipher.ProcessBytes(
                    FileBytes,
                    0,
                    FileBytes.Length,
                    ciphertextBytes,
                    0
                );
                cipher.DoFinal(ciphertextBytes, ciphertextLength);
                return ciphertextBytes;
            }

            CustomLogger.LoggerAccessor.LogError(
                "[WebCrypto] - InitiateCBCEncryptBuffer - Invalid KeyBytes or IV!"
            );

            return null;
        }

        private static string InitiateCBCEncryptBufferTobase64String(
            string FileString,
            byte[] KeyBytes,
            byte[] m_iv
        )
        {
            if (KeyBytes.Length >= 16 && m_iv.Length == 16)
                return Base64.ToBase64String(
                    InitiateCBCEncryptBuffer(Encoding.UTF8.GetBytes(FileString), KeyBytes, m_iv)
                );
            else
                CustomLogger.LoggerAccessor.LogError(
                    "[WebCrypto] - InitiateCBCEncryptBufferTobase64String - Invalid KeyBytes or IV!"
                );

            return null;
        }

        private static byte[] InitiateCTRBuffer(byte[] FileBytes, byte[] KeyBytes, byte[] m_iv)
        {
            if (KeyBytes.Length >= 16 && m_iv.Length == 16)
            {
                // Create the cipher
                var cipher = CipherUtilities.GetCipher("AES/CTR/OAEPWITHSHA224ANDMGF1PADDING");

                cipher.Init(false, new ParametersWithIV(new KeyParameter(KeyBytes), m_iv));

                // Encrypt the plaintext
                var ciphertextBytes = new byte[cipher.GetOutputSize(FileBytes.Length)];
                var ciphertextLength = cipher.ProcessBytes(
                    FileBytes,
                    0,
                    FileBytes.Length,
                    ciphertextBytes,
                    0
                );
                cipher.DoFinal(ciphertextBytes, ciphertextLength);
                return ciphertextBytes;
            }

            CustomLogger.LoggerAccessor.LogError(
                "[WebCrypto] - InitiateCTRBuffer - Invalid KeyBytes or IV!"
            );

            return null;
        }

        private static string InitiateCTRBufferTobase64String(
            string FileString,
            byte[] KeyBytes,
            byte[] m_iv
        )
        {
            if (KeyBytes.Length >= 16 && m_iv.Length == 16)
                return Base64.ToBase64String(
                    InitiateCTRBuffer(Encoding.UTF8.GetBytes(FileString), KeyBytes, m_iv)
                );

            CustomLogger.LoggerAccessor.LogError(
                "[WebCrypto] - InitiateCTRBufferTobase64String - Invalid KeyBytes or IV!"
            );

            return null;
        }
    }
}
