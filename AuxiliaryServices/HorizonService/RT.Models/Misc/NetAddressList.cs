using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;

namespace Horizon.RT.Models
{
    public class NetAddressList : IStreamSerializer
    {
        public NetAddress[] AddressList = null;

        public NetAddressList()
        {
            AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT];
            for (var i = 0; i < Constants.NET_ADDRESS_LIST_COUNT; ++i)
                AddressList[i] = new NetAddress();
        }

        public void Deserialize(BinaryReader reader)
        {
            AddressList = new NetAddress[Constants.NET_ADDRESS_LIST_COUNT];
            for (var i = 0; i < Constants.NET_ADDRESS_LIST_COUNT; ++i)
            {
                AddressList[i] = reader.Read<NetAddress>();
            }
        }

        public void Serialize(BinaryWriter writer)
        {
            for (var i = 0; i < Constants.NET_ADDRESS_LIST_COUNT; ++i)
            {
                writer.Write(
                    (AddressList == null || i >= AddressList.Length)
                        ? NetAddress.Empty
                        : AddressList[i]
                );
            }
        }
    }
}
