using System.Diagnostics.CodeAnalysis;
using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    #region MediusMessageAttribute
    [AttributeUsage(AttributeTargets.Class)]
    public class MediusMessageAttribute : Attribute
    {
        public NetMessageClass MessageClass;
        public byte MessageType;
        public GhsOpcode GhsMsgType;

        public MediusMessageAttribute(NetMessageClass msgClass, MediusDmeMessageIds msgType)
        {
            MessageClass = msgClass;
            MessageType = (byte)msgType;
        }

        public MediusMessageAttribute(NetMessageClass msgClass, MediusMGCLMessageIds msgType)
        {
            MessageClass = msgClass;
            MessageType = (byte)msgType;
        }

        public MediusMessageAttribute(NetMessageClass msgClass, MediusLobbyMessageIds msgType)
        {
            MessageClass = msgClass;
            MessageType = (byte)msgType;
        }

        public MediusMessageAttribute(NetMessageClass msgClass, MediusLobbyExtMessageIds msgType)
        {
            MessageClass = msgClass;
            MessageType = (byte)msgType;
        }

        public MediusMessageAttribute(GhsOpcode msgType)
        {
            GhsMsgType = msgType;
        }

        public MediusMessageAttribute(NetMessageClass msgClass, NetMessageTypeIds msgType)
        {
            MessageClass = msgClass;
            MessageType = (byte)msgType;
        }
    }
    #endregion

    #region BaseMediusMessage
    public abstract class BaseMediusMessage
    {
        /// <summary>
        /// Message class.
        /// </summary>
        public abstract NetMessageClass PacketClass { get; }

        /// <summary>
        /// Message type.
        /// </summary>
        public abstract byte PacketType { get; }

        /// <summary>
        /// When true, skips encryption when sending this particular message instance.
        /// </summary>
        public virtual bool SkipEncryption { get; set; } = false;

        public BaseMediusMessage() { }

        #region Serialization

        /// <summary>
        /// Deserializes the message from plaintext.
        /// </summary>
        /// <param name="reader"></param>
        public virtual void Deserialize(MessageReader reader) { }

        /// <summary>
        /// Serialize contents of the message.
        /// </summary>
        public virtual void Serialize(MessageWriter writer) { }

        #endregion

        #region Dynamic Instantiation

        private static Dictionary<MediusDmeMessageIds, Type> _dmeMessageClassById = null;
        private static Dictionary<MediusMGCLMessageIds, Type> _mgclMessageClassById = null;
        private static Dictionary<MediusLobbyMessageIds, Type> _lobbyMessageClassById = null;
        private static Dictionary<MediusLobbyExtMessageIds, Type> _lobbyExtMessageClassById = null;
        private static readonly int _messageClassByIdLockValue = 0;
        private static readonly object _messageClassByIdLockObject = _messageClassByIdLockValue;

        [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetTypes()")]
        private static void Initialize()
        {
            lock (_messageClassByIdLockObject)
            {
                if (_dmeMessageClassById != null)
                    return;

                _dmeMessageClassById = new Dictionary<MediusDmeMessageIds, Type>();
                _mgclMessageClassById = new Dictionary<MediusMGCLMessageIds, Type>();
                _lobbyMessageClassById = new Dictionary<MediusLobbyMessageIds, Type>();
                _lobbyExtMessageClassById = new Dictionary<MediusLobbyExtMessageIds, Type>();

                // Populate
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(BaseMediusMessage));
                var types = assembly.GetTypes();

                foreach (var classType in types)
                {
                    // Objects by Id
                    var attrs = (MediusMessageAttribute[])
                        classType.GetCustomAttributes(typeof(MediusMessageAttribute), true);
                    if (attrs != null && attrs.Length > 0)
                    {
                        switch (attrs[0].MessageClass)
                        {
                            case NetMessageClass.MessageClassDME:
                            {
                                _dmeMessageClassById.Add(
                                    (MediusDmeMessageIds)attrs[0].MessageType,
                                    classType
                                );
                                break;
                            }
                            case NetMessageClass.MessageClassLobbyReport:
                            {
                                _mgclMessageClassById.Add(
                                    (MediusMGCLMessageIds)attrs[0].MessageType,
                                    classType
                                );
                                break;
                            }
                            case NetMessageClass.MessageClassLobby:
                            {
                                _lobbyMessageClassById.Add(
                                    (MediusLobbyMessageIds)attrs[0].MessageType,
                                    classType
                                );
                                break;
                            }
                            case NetMessageClass.MessageClassLobbyExt:
                            {
                                _lobbyExtMessageClassById.Add(
                                    (MediusLobbyExtMessageIds)attrs[0].MessageType,
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
        public static BaseMediusMessage Instantiate(MessageReader reader)
        {
            BaseMediusMessage msg;
            Type classType = null;

            // Init
            Initialize();

            var msgClass = reader.Read<NetMessageClass>();
            var msgType = reader.ReadByte();

            switch (msgClass)
            {
                case NetMessageClass.MessageClassDME:
                {
                    if (
                        !_dmeMessageClassById.TryGetValue(
                            (MediusDmeMessageIds)msgType,
                            out classType
                        )
                    )
                        classType = null;
                    break;
                }
                case NetMessageClass.MessageClassLobbyReport:
                {
                    if (
                        !_mgclMessageClassById.TryGetValue(
                            (MediusMGCLMessageIds)msgType,
                            out classType
                        )
                    )
                        classType = null;
                    break;
                }
                case NetMessageClass.MessageClassLobby:
                {
                    if (
                        !_lobbyMessageClassById.TryGetValue(
                            (MediusLobbyMessageIds)msgType,
                            out classType
                        )
                    )
                        classType = null;
                    break;
                }
                case NetMessageClass.MessageClassLobbyExt:
                {
                    if (
                        !_lobbyExtMessageClassById.TryGetValue(
                            (MediusLobbyExtMessageIds)msgType,
                            out classType
                        )
                    )
                        classType = null;
                    break;
                }
            }

            // Instantiate
            msg =
                classType == null
                    ? new RawMediusMessage(msgClass, msgType)
                    : (BaseMediusMessage)Activator.CreateInstance(classType);

            // Deserialize
            msg.Deserialize(reader);
            return msg;
        }

        #endregion
    }
    #endregion
}
