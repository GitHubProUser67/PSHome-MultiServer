using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Collections.Generic;
using System;
using EndianTools.ZipperEndian;
using EndianTools;

namespace Horizon.RT.Models
{
    #region BaseMediusPluginMessage
    public abstract class BaseMediusPluginMessage
    {
        /// <summary>
        /// Message class.
        /// </summary>
        public abstract byte IncomingMessage { get; }

        public abstract int Size { get; }

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
        private static bool debug = true;
#else
        private static bool debug = false;
#endif
        public BaseMediusPluginMessage()
        {

        }

        #region Serialization

        /// <summary>
        /// Deserializes the plugin message from plaintext.
        /// </summary>
        /// <param name="reader"></param>
        public virtual void DeserializePlugin(MessageReader reader)
        {

        }

        /// <summary>
        /// Serialize contents of the plugin message.
        /// </summary>
        public virtual void SerializePlugin(MessageWriter writer)
        {

        }

        #endregion

        #region Dynamic Instantiation

        private static Dictionary<NetMessageTypeIds, Type> _netPluginMessageTypeById = null;

        private static int _messageClassByIdLockValue = 0;
        private static object _messageClassByIdLockObject = _messageClassByIdLockValue;

        private static void Initialize()
        {
            lock (_messageClassByIdLockObject)
            {
                _netPluginMessageTypeById = new Dictionary<NetMessageTypeIds, Type>();

                // Populate
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(BaseMediusPluginMessage));
                var types = assembly.GetTypes();

                foreach (Type classType in types)
                {
                    // Objects by Id
                    var attrs = (MediusMessageAttribute[])classType.GetCustomAttributes(typeof(MediusMessageAttribute), true);
                    if (attrs != null && attrs.Length > 0)
                    {
                        switch (attrs[0].MessageClass)
                        {
                            case NetMessageClass.MessageClassApplication:
                                {
                                    _netPluginMessageTypeById.Add((NetMessageTypeIds)attrs[0].MessageType, classType);
                                    break;
                                }
                        }

                    }
                }
            }
        }

        public static BaseMediusPluginMessage InstantiateClientPlugin(MessageReader reader)
        {
            BaseMediusPluginMessage msg;

            Type classType = null;

            byte[] buffer = reader.ReadBytes(3);
            int msgSize = (buffer[0] << 16) | (buffer[1] << 8) | buffer[2];
            NetMessageTypeIds msgType = (NetMessageTypeIds)EndianAwareConverter.ToUInt16(reader.ReadBytes(2), Endianness.BigEndian, 0);

            // Init
            Initialize();

            if (!_netPluginMessageTypeById.TryGetValue(msgType, out classType))
                classType = null;

            // Instantiate
            if (classType == null)
                msg = new RawMediusClientMessage(msgSize, msgType);
            else
                msg = (BaseMediusPluginMessage)Activator.CreateInstance(classType);

            // Deserialize
            msg.DeserializePlugin(reader);
            return msg;
        }

        public static BaseMediusPluginMessage InstantiateServerPlugin(MessageReader reader)
        {
            BaseMediusPluginMessage msg;

            Type classType = null;

            byte incomingMsg = reader.ReadByte();
            ushort msgSize = EndianAwareConverter.ToUInt16(reader.ReadBytes(2), Endianness.BigEndian, 0);
            byte PluginId = reader.ReadByte();
            reader.ReadBytes(2);
            NetMessageTypeIds msgType = (NetMessageTypeIds)EndianAwareConverter.ToUInt16(reader.ReadBytes(2), Endianness.BigEndian, 0);

            // Init
            Initialize();

            if (!_netPluginMessageTypeById.TryGetValue(msgType, out classType))
                classType = null;

            // Instantiate
            if (classType == null)
                msg = new RawMediusServerMessage(incomingMsg, msgSize, PluginId, msgType);
            else
                msg = (BaseMediusPluginMessage)Activator.CreateInstance(classType);

            // Deserialize
            msg.DeserializePlugin(reader);
            return msg;
        }

        #endregion
    }
    #endregion
}