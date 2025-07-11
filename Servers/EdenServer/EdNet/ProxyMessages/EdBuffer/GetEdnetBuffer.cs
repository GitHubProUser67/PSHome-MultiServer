using EdNetService.CRC;
using EdNetService.Models;
using System.Net;

namespace EdenServer.EdNet.ProxyMessages.Database.EdBuffer
{
    public class GetEdnetBuffer : AbstractProxyMessage
    {
        private const ushort chunkSize = 512;

        private static readonly byte[] defaultDriverProfile = GenerateDefaultDriverProfile();

        public override byte[]? Process(IPEndPoint endpoint, IPEndPoint target, ClientTask task, ushort PacketMagic)
        {
            EdStore request = task.Request;

            uint bufferid = request.ExtractUInt32();
            uint offset = request.ExtractUInt32();

            EdStore response = new EdStore(null, 1400);

            response.InsertStart(edStoreBank.CRC_A_GET_EDNETBUFFER);
            response.InsertUInt32(bufferid);

            uint profileSize = (uint)defaultDriverProfile.Length;

            if (offset > profileSize || !GetChunkChecksum(offset, bufferid))
                response.InsertUInt8(0); // Failure
            else
            {
                byte[] payload = new byte[Math.Min(chunkSize, profileSize - offset)];
                Array.Copy(defaultDriverProfile, (int)offset, payload, 0, payload.Length);

                response.InsertUInt8(1); // Success
                response.InsertUInt32(offset);
                response.InsertUInt32(profileSize);
                /* The CRC must be equal to the bufferid:
                   sVar4 = ProcessBufferCRC(iVar2 + offset,iVar9);
                   sStack12 = (short)buffer_id;
                   if (sVar4 == sStack12) {*/
                response.InsertUInt16((ushort)bufferid);
                response.InsertByteArray(payload, (ushort)payload.Length);
            }

            response.InsertEnd();

            task.Response = response;
            task.Target = endpoint;
            task.ClientMode = ClientMode.ProxyServerRaw;

            return null;
        }

        private static bool GetChunkChecksum(uint offset, uint bufferid)
        {
            byte[] securityCheckBytes = new byte[offset];
            Array.Copy(defaultDriverProfile, 0, securityCheckBytes, 0, securityCheckBytes.Length);
            return Utils.GetCRCFromBuffer(securityCheckBytes) == bufferid;
        }

        private static byte[] GenerateDefaultDriverProfile()
        {
            EdStore driverStore = new EdStore(null, 1400);
            driverStore.InsertStart(edStoreBank.NetBufferGarage);
            driverStore.InsertUInt32(0);
            driverStore.InsertString("testgarage");
            driverStore.InsertUInt32(0);
            driverStore.InsertUInt64(0);

            byte[] output = new byte[driverStore.CurrentSize];
            Array.Copy(driverStore.Data, 0, output, 0, output.Length);

            return output;
        }
    }
}
