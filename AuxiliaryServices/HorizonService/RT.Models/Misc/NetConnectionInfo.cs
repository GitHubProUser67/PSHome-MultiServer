using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    public class NetConnectionInfo : IStreamSerializer
    {
        /// <summary>
        /// NetConnectionType connected to this server as
        /// </summary>
        public NetConnectionType Type;
        /// <summary>
        /// NetAddressList
        /// </summary>
        public NetAddressList AddressList = new NetAddressList();
        /// <summary>
        /// WorldID of the Connected client
        /// </summary>
        public int TargetWorldID;
        /// <summary>
        /// ServerKey
        /// </summary>
        public RSA_KEY ServerKey = new RSA_KEY();
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey;
        /// <summary>
        /// Access Token
        /// </summary>
        public string AccessKey;

        public void Deserialize(BinaryReader reader)
        {
            Type = reader.Read<NetConnectionType>();
            AddressList = reader.Read<NetAddressList>();
            TargetWorldID = reader.ReadInt32();
            ServerKey = reader.Read<RSA_KEY>();
            SessionKey = reader.ReadString(Constants.NET_SESSION_KEY_LEN);
            AccessKey = reader.ReadString(Constants.NET_ACCESS_KEY_LEN);
            reader.ReadBytes(2);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(Type);
            writer.Write(AddressList);
            writer.Write(TargetWorldID);
            writer.Write(ServerKey);
            writer.Write(SessionKey, Constants.NET_SESSION_KEY_LEN);
            writer.Write(AccessKey, Constants.NET_ACCESS_KEY_LEN);
            writer.Write(new byte[2]);
        }

        public override string ToString()
        {
            return $"Type: {Type} " +
                $"AddressList: {AddressList} " +
                $"WorldID: {TargetWorldID} " +
                $"ServerKey: {ServerKey} " +
                $"SessionKey: {SessionKey} " +
                $"AccessKey: {AccessKey}";
        }
    }
}
