using EdNetService.CRC;
using EdNetService.Models;
using System.Net;

namespace EdenServer.EdNet.ProxyMessages.ORB.File
{
    public class GetFile : AbstractProxyMessage
    {
        private const ushort nexttimewait = 5;
        private const ushort chunkSize = 512;

        public override byte[]? Process(IPEndPoint endpoint, IPEndPoint target, ClientTask task, ushort PacketMagic)
        {
            EdStore request = task.Request;

            uint fileid = request.ExtractUInt32();
            uint offset = request.ExtractUInt32();

            EdStore response = new EdStore(null, 1400);

            response.InsertStart(edStoreBank.CRC_A_ORB_GETFILE);

            if (OpenFile.fileSystemCache.TryRemove(fileid, out var fileSystemEntry))
            {
                byte[] responseBytes = new byte[(int)Math.Min(chunkSize, fileSystemEntry.Item2.Length - offset)];

                Array.Copy(fileSystemEntry.Item2, offset, responseBytes, 0, responseBytes.Length);

                response.InsertUInt8(0); // Success.
                response.InsertUInt32(fileid);
                response.InsertUInt32(offset);
                response.InsertUInt16(nexttimewait);
                response.InsertByteArray(responseBytes, (ushort)responseBytes.Length);
            }
            else
                SetFailure(response);

            response.InsertEnd();

            task.Response = response;
            task.Target = endpoint;
            task.ClientMode = ClientMode.ProxyServer;

            return null;
        }

        public static void SetFailure(EdStore store)
        {
            store.InsertUInt8(1); // Failure.
            store.InsertUInt32(0);
            store.InsertUInt32(0);
            store.InsertUInt16(0);
        }
    }
}
