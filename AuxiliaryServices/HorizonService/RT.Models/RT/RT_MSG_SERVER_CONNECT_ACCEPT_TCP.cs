using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Net;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_CONNECT_ACCEPT_TCP)]
    public class RT_MSG_SERVER_CONNECT_ACCEPT_TCP : BaseScertMessage
    {

        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_CONNECT_ACCEPT_TCP;

        public ushort PlayerId = 0x0000;
        public uint ScertId = 0x10EC;
        public ushort PlayerCount = 0x0001; // Be carefull, only increment when in a DME game.

        public byte[] UNK_07 = { 0x01, 0x08, 0x10 };
        public IPAddress IP;

        public override void Deserialize(MessageReader reader)
        {
            if (reader.MediusVersion <= 108)
            {
                UNK_07 = reader.ReadBytes(3);
                PlayerId = reader.ReadUInt16();
                PlayerCount = reader.ReadUInt16();
                IP = reader.ReadIPAddress();
            }
            else
            {
                PlayerId = reader.ReadUInt16();
                ScertId = reader.ReadUInt32();
                PlayerCount = reader.ReadUInt16();
                IP = reader.ReadIPAddress();
            }
        }

        public override void Serialize(MessageWriter writer)
        {
            if (writer.MediusVersion <= 108)
            {
                writer.Write(UNK_07);
                writer.Write(PlayerId);
                writer.Write(PlayerCount);

                if (IP == null)
                    writer.Write(IPAddress.Any);
                else
                    writer.Write(IP);
            }
            else
            {
                writer.Write(PlayerId);
                writer.Write(ScertId);
                writer.Write(PlayerCount);

                if (IP == null)
                    writer.Write(IPAddress.Any);
                else
                    writer.Write(IP);
            }
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"PlayerId: {PlayerId} " +
                $"ScertId: {ScertId} " +
                $"PlayerCount: {PlayerCount} " +
                $"IP: {IP}";
        }
    }
}
