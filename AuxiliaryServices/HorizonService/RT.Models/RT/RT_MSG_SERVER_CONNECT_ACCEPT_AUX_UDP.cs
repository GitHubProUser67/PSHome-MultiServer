using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Net;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_CONNECT_ACCEPT_AUX_UDP)]
    public class RT_MSG_SERVER_CONNECT_ACCEPT_AUX_UDP : BaseScertMessage
    {

        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_CONNECT_ACCEPT_AUX_UDP;

        public ushort PlayerId;
        public uint ScertId = 0xD4;
        public ushort PlayerCount = 0x0001;

        public IPEndPoint EndPoint;

        public override void Deserialize(MessageReader reader)
        {
            if (reader.MediusVersion <= 108)
            {
                // unk
                reader.ReadBytes(3);
                PlayerId = reader.ReadUInt16();
                PlayerCount = reader.ReadUInt16();
                EndPoint = new IPEndPoint(reader.ReadIPAddress(), (int)reader.ReadUInt16());
            }
            else
            {
                PlayerId = reader.ReadUInt16();
                ScertId = reader.ReadUInt32();
                PlayerCount = reader.ReadUInt16();

                EndPoint = new IPEndPoint(reader.ReadIPAddress(), (int)reader.ReadUInt16());
            }
        }

        public override void Serialize(MessageWriter writer)
        {
            if (writer.MediusVersion <= 108)
            {
                // unk
                writer.Write(new byte[] { 0x01, 0x08, 0x10 });
                writer.Write(PlayerId);
                writer.Write(PlayerCount);

                writer.Write(EndPoint.Address);
                writer.Write((ushort)EndPoint.Port);
            }
            else
            {
                writer.Write(PlayerId);
                writer.Write(ScertId);
                writer.Write(PlayerCount);

                writer.Write(EndPoint.Address);
                writer.Write((ushort)EndPoint.Port);
            }
        }
    }
}
