using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_APP_TOSERVER)]
    public class RT_MSG_CLIENT_APP_TOSERVER : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_APP_TOSERVER;

        public BaseMediusMessage Message { get; set; } = null;
        //public BaseMediusGHSMessage GhsMessage { get; set; } = null;

        public override void Deserialize(MessageReader reader)
        {
            Message = BaseMediusMessage.Instantiate(reader);
            /*
            if (Message != null)
            {
            } else
            {
                GhsMessage = BaseMediusGHSMessage.Instantiate(reader);
            }
            */
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(Message.PacketClass);
            writer.Write(Message.PacketType);
            Message.Serialize(writer);
            /*
            if (Message != null)
            {
            } else
            {
                writer.Write(GhsMessage.msgSize);
                writer.Write(ReverseBytes16((ushort)GhsMessage.GhsOpcode));
                GhsMessage.Serialize(writer);
            }
            */
        }

        public override bool CanLog()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"Message: {Message}";
            /*
            if(Message != null)
            {
            } else {
                return base.ToString() + " " +
                    $"GhsMessage: {GhsMessage}";
            }
            */
        }

        /// <summary>
        /// Reverses UInt16 
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        public static ushort ReverseBytes16(ushort nValue)
        {
            return (ushort)((ushort)((nValue >> 8)) | (nValue << 8));
        }
    }
}
