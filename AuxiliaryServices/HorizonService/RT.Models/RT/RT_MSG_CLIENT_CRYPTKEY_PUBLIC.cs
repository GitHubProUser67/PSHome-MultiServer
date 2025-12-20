using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_CRYPTKEY_PUBLIC)]
    public class RT_MSG_CLIENT_CRYPTKEY_PUBLIC : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_CRYPTKEY_PUBLIC;

        public byte[] PublicKey = null;

        public override void Deserialize(MessageReader reader)
        {
            PublicKey = reader.ReadBytes(0x40);
        }

        public override void Serialize(MessageWriter writer)
        {
            if (PublicKey == null || PublicKey.Length != 0x40)
                throw new InvalidOperationException("Unable to serialize CLIENT_GET_KEY key because key is either null or not 64 bytes long!");

            writer.Write(PublicKey);
        }
    }
}
