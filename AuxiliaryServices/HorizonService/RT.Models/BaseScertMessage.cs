using System.Diagnostics.CodeAnalysis;
using CustomLogger;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;
using Horizon.RT.Cryptography;

namespace Horizon.RT.Models
{
    public abstract class BaseScertMessage
    {
        public const int HEADER_SIZE = 3;
        public const int HASH_SIZE = 4;

        /// <summary>
        /// Message id.
        /// </summary>
        public abstract RT_MSG_TYPE Id { get; }

        /// <summary>
        /// When true, skips encryption when sending this particular message instance.
        /// </summary>
        public virtual bool SkipEncryption { get; set; } = false;

        public BaseScertMessage() { }

        #region Serialization

        /// <summary>
        /// Deserializes the message from plaintext.
        /// </summary>
        /// <param name="reader"></param>
        public abstract void Deserialize(MessageReader reader);

        /// <summary>
        /// Serializes the message.
        /// </summary>
        public List<byte[]> Serialize(int? mediusVersion, int appId, CipherService cipherService)
        {
            var results = new List<byte[]>();
            byte[] result;
            var buffer = new byte[1024 * 10];
            var length = 0;
            var totalHeaderSize = HEADER_SIZE;

            // Serialize message
            using (var stream = new MemoryStream(buffer, true))
            using (
                var writer = new MessageWriter(stream)
                {
                    MediusVersion = mediusVersion != null ? (int)mediusVersion : 108,
                    AppId = appId,
                }
            )
            {
                Serialize(writer);
                length = (int)writer.BaseStream.Position;
            }

            var ctx =
                (
                    Id == RT_MSG_TYPE.RT_MSG_SERVER_CRYPTKEY_PEER
                    || Id == RT_MSG_TYPE.RT_MSG_CLIENT_CRYPTKEY_PUBLIC
                )
                    ? CipherContext.RSA_AUTH
                    : CipherContext.RC_CLIENT_SESSION;

            // Check for fragmentation
            if (
                Id == RT_MSG_TYPE.RT_MSG_SERVER_APP
                && length > Constants.MEDIUS_MESSAGE_MAXLEN
                && mediusVersion != 108
                && appId != 21834
            )
            {
                var fragments = TypePacketFragment.FromPayload(
                    (NetMessageClass)buffer[0],
                    buffer[1],
                    buffer,
                    2,
                    length - 2
                );

                foreach (var frag in fragments)
                {
                    totalHeaderSize = HEADER_SIZE;

                    // Serialize message
                    using (var stream = new MemoryStream(buffer, true))
                    using (var writer = new MessageWriter(stream))
                    {
                        // Serialize message
                        new RT_MSG_SERVER_APP() { Message = frag }.Serialize(writer);
                        length = (int)stream.Position;

                        var data = new byte[length];
                        Array.Copy(buffer, data, length);
                        if (
                            !SkipEncryption
                            && cipherService != null
                            && cipherService.Encrypt(ctx, data, out var signed, out var hash)
                        )
                        {
                            if (hash == null || signed == null)
                                throw new NullReferenceException(
                                    "[BaseScertMessage-Serialize] - hash or signed was null during the encryption!"
                                );
                            else
                            {
                                totalHeaderSize += HASH_SIZE;

                                Array.Copy(hash, 0, buffer, 3, hash.Length);
                                Array.Copy(signed, 0, buffer, 3 + hash.Length, signed.Length);

                                writer.Seek(0, SeekOrigin.Begin);
                                writer.Write((byte)((byte)Id | 0x80));
                            }
                        }
                        else
                        {
                            Array.Copy(buffer, 0, buffer, 3, length);
                            writer.Seek(0, SeekOrigin.Begin);
                            writer.Write((byte)Id);
                        }

                        // Write length
                        writer.Seek(1, SeekOrigin.Begin);
                        writer.Write((ushort)length);
                    }

                    result = new byte[length + totalHeaderSize];
                    Array.Copy(buffer, 0, result, 0, result.Length);
                    results.Add(result);
                }
            }
            else
            {
                byte[] dataToCopy = null;
                byte[] dataToEncrypt;

                // Massive thanks to score3229!!
                if (Id == RT_MSG_TYPE.RT_MSG_CLIENT_APP_SINGLE)
                {
                    dataToCopy = [buffer[0], buffer[1]];
                    dataToEncrypt = new byte[length - 2];
                    Array.Copy(buffer, 2, dataToEncrypt, 0, dataToEncrypt.Length);
                }
                else if (Id == RT_MSG_TYPE.RT_MSG_CLIENT_APP_LIST)
                {
                    dataToCopy = new byte[buffer[0]];
                    Array.Copy(buffer, 1, dataToCopy, 0, dataToCopy.Length);

                    dataToEncrypt = new byte[length - dataToCopy.Length - 1];
                    Array.Copy(buffer, 1 + dataToCopy.Length, dataToEncrypt, 0, dataToEncrypt.Length);
                }
                else
                {
                    dataToEncrypt = new byte[length];
                    Array.Copy(buffer, dataToEncrypt, length);
                }

                if (
                    !SkipEncryption
                    && cipherService != null
                    && cipherService.Encrypt(ctx, dataToEncrypt, out var signed, out var hash)
                )
                {
                    if (hash == null || signed == null)
                        throw new NullReferenceException(
                            "[BaseScertMessage-Serialize] - hash or signed was null during the encryption!"
                        );
                    else
                    {
                        totalHeaderSize += HASH_SIZE;

                        result = new byte[length + totalHeaderSize];
                        result[0] = (byte)((byte)Id | 0x80);
                        result[1] = (byte)(length & byte.MaxValue);
                        result[2] = (byte)((length >> 8) & byte.MaxValue);
                        Array.Copy(hash, 0, result, HEADER_SIZE, HASH_SIZE);
                        if (dataToCopy != null)
                        {
                            var copyLength = dataToCopy.Length;
                            Array.Copy(dataToCopy, 0, result, totalHeaderSize, copyLength);
                            Array.Copy(signed, 0, result, totalHeaderSize + copyLength, dataToEncrypt.Length);
                        }
                        else
                            Array.Copy(signed, 0, result, totalHeaderSize, length);
                    }
                }
                else
                {
                    // Add id and length to header
                    result = new byte[length + totalHeaderSize];
                    result[0] = (byte)Id;
                    result[1] = (byte)(length & byte.MaxValue);
                    result[2] = (byte)((length >> 8) & byte.MaxValue);
                    Array.Copy(buffer, 0, result, totalHeaderSize, length);
                }

                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// Serialize contents of the message.
        /// </summary>
        public abstract void Serialize(MessageWriter writer);

        #endregion

        #region Logging

        /// <summary>
        /// Whether or not this message passes the log filter.
        /// </summary>
        public virtual bool CanLog()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        #endregion

        #region Dynamic Instantiation

        private static Dictionary<RT_MSG_TYPE, Type> _messageClassById = null;
        private static readonly int _messageClassByIdLockValue = 0;
        private static readonly object _messageClassByIdLockObject = _messageClassByIdLockValue;

        [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetTypes()")]
        private static void Initialize()
        {
            lock (_messageClassByIdLockObject)
            {
                if (_messageClassById != null)
                    return;

                _messageClassById = [];

                // Populate
                var types = System
                    .Reflection.Assembly.GetAssembly(typeof(BaseScertMessage))
                    ?.GetTypes();

                if (types != null)
                {
                    foreach (var classType in types)
                    {
                        // Objects by Id
                        var attrs = (ScertMessageAttribute[])
                            classType.GetCustomAttributes(typeof(ScertMessageAttribute), true);
                        if (attrs != null && attrs.Length > 0)
                            _messageClassById.Add(attrs[0].MessageId, classType);
                    }
                }
            }
        }

        [RequiresUnreferencedCode("This method uses reflection and may break when trimmed.")]
        public static void RegisterMessage(RT_MSG_TYPE id, Type type)
        {
            // Init first
            Initialize();

            if (_messageClassById != null)
            {
                // Set or overwrite.
                if (!_messageClassById.TryAdd(id, type))
                    _messageClassById[id] = type;
            }
        }

        public static BaseScertMessage Instantiate(MessageReader reader)
        {
            var id = reader.ReadByte();
            var rtId = (RT_MSG_TYPE)(id & 0x7F);
            var len = reader.ReadInt16();
            var messageBytes = reader.ReadBytes(len);
            return id >= 0x80
                ? throw new Exception(
                    $"[BaseScertMessage-Instantiate] - Unable instantiate encrypted message {id} without a cipher!"
                )
                : Instantiate(rtId, null, messageBytes, reader.MediusVersion, reader.AppId, null);
        }

        [RequiresUnreferencedCode("This method uses reflection and may break when trimmed.")]
        public static BaseScertMessage Instantiate(
            RT_MSG_TYPE id,
            byte[] hash,
            byte[] messageBuffer,
            int mediusVersion,
            int appId,
            CipherService cipherService
        )
        {
            // Init first
            Initialize();

            Type classType = null;

            // Get class
            if (_messageClassById != null && !_messageClassById.TryGetValue(id, out classType))
                classType = null;

            // Decrypt
            if (hash != null)
            {
                if (cipherService != null)
                {
                    byte[] dataToCopy = null;
                    byte[] dataToDecrypt;

                    // Massive thanks to score3229!!
                    if (id == RT_MSG_TYPE.RT_MSG_CLIENT_APP_SINGLE)
                    {
                        dataToCopy = new byte[2];
                        Array.Copy(messageBuffer, 0, dataToCopy, 0, 2);

                        dataToDecrypt = new byte[messageBuffer.Length - 2];
                        Array.Copy(messageBuffer, 2, dataToDecrypt, 0, dataToDecrypt.Length);
                    }
                    else if (id == RT_MSG_TYPE.RT_MSG_CLIENT_APP_LIST)
                    {
                        byte maskLength = messageBuffer[0];
                        dataToCopy = new byte[maskLength + 1];
                        Array.Copy(messageBuffer, 1, dataToCopy, 1, maskLength);
                        dataToCopy[0] = maskLength;

                        dataToDecrypt = new byte[messageBuffer.Length - dataToCopy.Length];
                        Array.Copy(messageBuffer, dataToCopy.Length, dataToDecrypt, 0, dataToDecrypt.Length);
                    }
                    else
                        dataToDecrypt = messageBuffer;

                    if (cipherService.Decrypt(dataToDecrypt, hash, out var plain))
                    {
                        byte[] finalBuffer;

                        if (dataToCopy != null)
                        {
                            var copyLength = dataToCopy.Length;
                            var plainLength = plain.Length;

                            finalBuffer = new byte[copyLength + plainLength];

                            Array.Copy(dataToCopy, 0, finalBuffer, 0, copyLength);
                            Array.Copy(plain, 0, finalBuffer, copyLength, plainLength);
                        }
                        else
                            finalBuffer = plain;

                        return Instantiate(classType, id, finalBuffer, mediusVersion, appId);
                    }
                }

                LoggerAccessor.LogError(
                        $"[BaseScertMessage-Instantiate] - Unable to decrypt {id}, HASH:{BitConverter.ToString(hash)} DATA:{Convert.ToHexString(messageBuffer)}"
                    );
            }
            else
                return Instantiate(classType, id, messageBuffer, mediusVersion, appId);

            return null;
        }

        private static BaseScertMessage Instantiate(
            [DynamicallyAccessedMembers(
                DynamicallyAccessedMemberTypes.PublicParameterlessConstructor
            )]
                Type classType,
            RT_MSG_TYPE id,
            byte[] plain,
            int mediusVersion,
            int appId
        )
        {
            if (plain == null)
            {
                LoggerAccessor.LogError(
                    "[BaseScertMessage-Instantiate] - null plain given to function!"
                );
                return null;
            }

            BaseScertMessage msg = null;

            using (var stream = new MemoryStream(plain))
            {
                using (
                    var reader = new MessageReader(stream)
                    {
                        MediusVersion = mediusVersion,
                        AppId = appId,
                    }
                )
                {
                    msg =
                        classType == null
                            ? new RawScertMessage(id)
                            : (BaseScertMessage)Activator.CreateInstance(classType);

                    try
                    {
                        msg.Deserialize(reader);
                    }
                    catch (Exception e)
                    {
                        LoggerAccessor.LogError(
                            $"[BaseScertMessage-Instantiate] - Error deserializing {id}, DATA:{BitConverter.ToString(plain)} (Exception: {e})"
                        );
                    }
                }
            }

            return msg;
        }

        #endregion

        public override string ToString()
        {
            return $"Id: {Id}";
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ScertMessageAttribute(RT_MSG_TYPE id) : Attribute
    {
        public RT_MSG_TYPE MessageId = id;
    }
}
