using CustomLogger;
using EdNetService.CRC;
using EdNetService.Models;
using MultiServerLibrary.Extension;
using NetHasher.CRC;
using System.Net;
using System.Text;

namespace EdenServer.EdNet.ProxyMessages.ORB.File
{
    public class GetFile : AbstractProxyMessage
    {
        private const int FileLockAwaitMs = 500;
        private const ushort nexttimewait = 5;
        private const ushort chunkSize = 1024;

        public override byte[]? Process(IPEndPoint endpoint, IPEndPoint target, ClientTask task, ushort PacketMagic)
        {
            EdStore request = task.Request;

            uint fileid = request.ExtractUInt32();
            uint offset = request.ExtractUInt32();

            EdStore response = new EdStore(null, 1400);

            response.InsertStart(edStoreBank.CRC_A_ORB_GETFILE);

            string staticUserHostedDir = Directory.GetCurrentDirectory() + $"/static/Eden/StaticUserHostedFiles/{task.Client.PendingFileUserId}";

            if (!Directory.Exists(staticUserHostedDir))
            {
                LoggerAccessor.LogWarn($"[GetFile] - Static User file repository expected at path:{staticUserHostedDir}, sending error response...");
                SetFailure(response);
            }
            else
            {
                Dictionary<uint, string> filePaths = new Dictionary<uint, string>();

                foreach (string filePath in Directory.GetFiles(staticUserHostedDir, "*.*", SearchOption.AllDirectories))
                {
                    filePaths[CRC32.CreateCastagnoli(Encoding.UTF8.GetBytes(Path.GetFileName(filePath)))] = filePath;
                }

                if (filePaths.ContainsKey(fileid))
                {
                    response.InsertUInt8(0); // Success.
                    response.InsertUInt32(fileid);
                    response.InsertUInt32(offset);
                    response.InsertUInt16(nexttimewait);
                    byte[] payload = FileSystemUtils.TryReadFileChunck(filePaths[fileid], (int)Math.Min(chunkSize, new FileInfo(filePaths[fileid]).Length - offset), FileSystemUtils.FileShareMode.ReadWrite, FileLockAwaitMs);
                    response.InsertByteArray(payload, (ushort)payload.Length);
                }
                else
                {
                    LoggerAccessor.LogWarn($"[GetFile] - File with Id:{fileid} was not found for userId:{task.Client.PendingFileUserId}, sending error response...");
                    SetFailure(response);
                }
            }

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
