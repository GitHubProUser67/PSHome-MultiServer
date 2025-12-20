using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_READY_REQUIRE)]
    public class RT_MSG_CLIENT_CONNECT_READY_REQUIRE : BaseScertMessage
    {

        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_READY_REQUIRE;

        public byte ServReq;
        public ushort Password_Len;
        public char[] Password;

        public override void Deserialize(MessageReader reader)
        {
            ServReq = reader.ReadByte();
            if (ServReq != 0)
            {
                Password_Len = reader.ReadByte();
                Password = reader.ReadChars(Password_Len);
            }
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(ServReq); 
            if (ServReq != 0)
            {
                writer.Write(Password_Len);
                writer.Write(Password);
            }
        }

        public override string ToString()
        {
            if(ServReq != 0)
            {
                return base.ToString() + " " +
                    $"ServReq: {ServReq} " +
                    $"PW Length: {Password_Len} " +
                    $"Password: {new string(Password)}";
            } else {
                return base.ToString() + " " +
                    $"ServReq: {ServReq} ";
            }
        }
    }
}
