using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Net;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_AUX_UDP)]
    public class RT_MSG_CLIENT_CONNECT_AUX_UDP : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_AUX_UDP;

        public int WorldId;
        public int ApplicationId;
        public IPEndPoint EndPoint;
        public ushort PlayerId;
        public ushort ScertId;
        public ushort OrdinalID;

        public override void Deserialize(MessageReader reader)
        {
            if (reader.MediusVersion <= 108)
            {
                reader.ReadBytes(3);
                WorldId = reader.ReadUInt16();
                ApplicationId = reader.ReadInt32();
                EndPoint = new IPEndPoint(reader.ReadIPAddress(), (int)reader.ReadUInt16());
                PlayerId = reader.ReadUInt16();
            }
            else
            {
                WorldId = reader.ReadInt32();
                ApplicationId = reader.ReadInt32();
                EndPoint = new IPEndPoint(reader.ReadIPAddress(), (int)reader.ReadUInt16());
                PlayerId = reader.ReadUInt16();
                ScertId = reader.ReadUInt16();
                OrdinalID = reader.ReadUInt16();
            }
        }

        public override void Serialize(MessageWriter writer)
        {
            if (writer.MediusVersion <= 108)
            {
                writer.Write(new byte[3]);
                writer.Write((ushort)WorldId);
                writer.Write(ApplicationId);
                writer.Write(EndPoint.Address);
                writer.Write((ushort)EndPoint.Port);
                writer.Write(PlayerId);
            }
            else
            {
                writer.Write(WorldId);
                writer.Write(ApplicationId);
                writer.Write(EndPoint.Address);
                writer.Write((ushort)EndPoint.Port);
                writer.Write(PlayerId);
                writer.Write(ScertId);
                writer.Write(OrdinalID);
            }
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"WorldId: {WorldId} " +
                $"ApplicationId: {ApplicationId} " +
                $"EndPoint: {EndPoint} " +
                $"PlayerId: {PlayerId} " +
                $"ScertId: {ScertId} " +
                $"OrdinalID: {OrdinalID}";
        }
    }
}
