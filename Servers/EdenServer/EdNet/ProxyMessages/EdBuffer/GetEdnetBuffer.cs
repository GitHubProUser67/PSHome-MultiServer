using EdNetService.CRC;
using EdNetService.Models;
using System.Net;

namespace EdenServer.EdNet.ProxyMessages.EdBuffer
{
    public class GetEdnetBuffer : AbstractProxyMessage
    {
        private const ushort chunkSize = 1200; // Each buffer can only be 6 * 1200 so 7200 max size.

        private static readonly byte[] defaultDriverProfile = GenerateDefaultDriverProfile();

        public override byte[]? Process(IPEndPoint endpoint, IPEndPoint target, ClientTask task, ushort PacketMagic)
        {
            EdStore request = task.Request;

            uint profileSize;
            uint bufferid = request.ExtractUInt32();
            uint offset = request.ExtractUInt32();

            EdStore response = new EdStore(null, 1400);

            response.InsertStart(edStoreBank.CRC_A_GET_EDNETBUFFER);
            response.InsertUInt32(bufferid);

            switch (bufferid)
            {
                case 0: // Garage
                    profileSize = (uint)defaultDriverProfile.Length;
                    if (offset > profileSize)
                    {
                        response.InsertUInt8(2); // Failure
                        response.InsertUInt32(offset);
                    }
                    else
                    {
                        byte[] payload = new byte[Math.Min(chunkSize, profileSize - offset)];
                        Array.Copy(defaultDriverProfile, (int)offset, payload, 0, payload.Length);

                        response.InsertUInt8(1); // Success
                        response.InsertUInt32(offset);
                        response.InsertUInt32(profileSize);
                        response.InsertUInt16(Utils.GetCRCFromBuffer(payload));
                        response.InsertByteArray(payload, (ushort)payload.Length);
                    }
                    break;
                default:
                    response.InsertUInt8(0); // Buffer not found.
                    response.InsertUInt32(offset);
                    break;
            }

            response.InsertEnd();

            task.Response = response;
            task.Target = endpoint;
            task.ClientMode = ClientMode.ProxyServerRaw;

            return null;
        }

        private static byte[] GenerateDefaultDriverProfile()
        {
            // This seems to be empty and expected to be empty...
            return new byte[0];
        }
    }
}
