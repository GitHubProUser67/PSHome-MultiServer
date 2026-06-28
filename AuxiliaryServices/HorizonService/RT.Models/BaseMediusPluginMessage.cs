using System.Diagnostics.CodeAnalysis;
using EndianTools;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    #region BaseMediusPluginMessage
    public abstract class BaseMediusPluginMessage
    {
        /// <summary>
        /// Message class.
        /// </summary>
        public abstract int Size { get; }
        public abstract ushort ClientBufferSize { get; }
        public abstract byte PluginId { get; }

        /// <summary>
        /// Message type.
        /// </summary>
        public abstract NetMessageTypeIds PacketType { get; }

        /// <summary>
        /// When true, skips encryption when sending this particular message instance.
        /// </summary>
        public virtual bool SkipEncryption { get; set; } = false;
#if DEBUG
        private static readonly bool debug = true;
#else
        private static bool debug = false;
#endif

        public BaseMediusPluginMessage() { }

        #region Serialization

        /// <summary>
        /// Deserializes the plugin message from plaintext.
        /// </summary>
        /// <param name="reader"></param>
        public virtual void DeserializePlugin(MessageReader reader) { }

        /// <summary>
        /// Serialize contents of the plugin message.
        /// </summary>
        public virtual void SerializePlugin(MessageWriter writer) { }

        #endregion

        #region Dynamic Instantiation

        private static Dictionary<NetMessageTypeIds, Type> _netPluginMessageTypeById = null;

        private static readonly int _messageClassByIdLockValue = 0;
        private static readonly object _messageClassByIdLockObject = _messageClassByIdLockValue;

        [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetTypes()")]
        private static void Initialize()
        {
            lock (_messageClassByIdLockObject)
            {
                _netPluginMessageTypeById = [];

                // Populate
                foreach (var classType in System.Reflection.Assembly.GetAssembly(
                    typeof(BaseMediusPluginMessage)
                ).GetTypes())
                {
                    // Objects by Id
                    var attrs = (MediusMessageAttribute[])
                        classType.GetCustomAttributes(typeof(MediusMessageAttribute), true);
                    if (attrs != null && attrs.Length > 0)
                    {
                        switch (attrs[0].MessageClass)
                        {
                            case NetMessageClass.MessageClassApplication:
                            {
                                _netPluginMessageTypeById.Add(
                                    (NetMessageTypeIds)attrs[0].MessageType,
                                    classType
                                );
                                break;
                            }
                        }
                    }
                }
            }
        }

        [RequiresUnreferencedCode("This method uses reflection and may break when trimmed.")]
        public static BaseMediusPluginMessage InstantiateClientPlugin(MessageReader reader)
        {
            BaseMediusPluginMessage msg;

            Type classType = null;

            var msgSize = EndianAwareConverter.ToUInt24(
                reader.ReadBytes(3),
                Endianness.BigEndian,
                0
            );
            var msgType = (NetMessageTypeIds)
                EndianAwareConverter.ToUInt16(reader.ReadBytes(2), Endianness.BigEndian, 0);

            // Init
            Initialize();

            if (!_netPluginMessageTypeById.TryGetValue(msgType, out classType))
                classType = null;

            // Instantiate
            msg =
                classType == null
                    ? new RawMediusClientMessage(msgSize, msgType)
                    : (BaseMediusPluginMessage)Activator.CreateInstance(classType);

            // Deserialize
            msg.DeserializePlugin(reader);
            return msg;
        }

        [RequiresUnreferencedCode("This method uses reflection and may break when trimmed.")]
        public static BaseMediusPluginMessage InstantiateServerPlugin(MessageReader reader)
        {
            BaseMediusPluginMessage msg;

            Type classType = null;
            ushort clientBuffSize = 0;
            byte PluginId = 0;

            var msgSize = EndianAwareConverter.ToUInt24(
                reader.ReadBytes(3),
                Endianness.BigEndian,
                0
            );
            if (reader.RemainingBytes > 2)
            {
                clientBuffSize = EndianAwareConverter.ToUInt16(
                    reader.ReadBytes(2),
                    Endianness.BigEndian,
                    0
                );
                PluginId = reader.ReadByte();
            }
            var msgType = (NetMessageTypeIds)
                EndianAwareConverter.ToUInt16(reader.ReadBytes(2), Endianness.BigEndian, 0);

            // Init
            Initialize();

            if (!_netPluginMessageTypeById.TryGetValue(msgType, out classType))
                classType = null;

            // Instantiate
            msg =
                classType == null
                    ? new RawMediusServerMessage(msgSize, clientBuffSize, PluginId, msgType)
                    : (BaseMediusPluginMessage)Activator.CreateInstance(classType);

            // Deserialize
            msg.DeserializePlugin(reader);
            return msg;
        }

        #endregion
    }
    #endregion
}
