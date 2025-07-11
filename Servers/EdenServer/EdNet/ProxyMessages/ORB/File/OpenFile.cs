using CustomLogger;
using EdNetService.CRC;
using EdNetService.Models;
using NetHasher.CRC;
using System.Net;
using System.Reflection;
using System.Text;

namespace EdenServer.EdNet.ProxyMessages.ORB.File
{
    public class OpenFile : AbstractProxyMessage
    {
        private const uint maxFsRetry = 10;
        private const uint timeout = 2000;

        public override byte[]? Process(IPEndPoint endpoint, IPEndPoint target, ClientTask task, ushort PacketMagic)
        {
            EdStore request = task.Request;

            byte facility = request.ExtractUInt8();
            uint userId = request.ExtractUInt32();
            task.Client.StorageUserId = userId;
            string filename = request.ExtractString();
            bool bupdload_wanted = request.ExtractUInt8() == 0x01;
            uint uploadtotalsize = request.ExtractUInt32();

            EdStore response = new EdStore(null, 1400);

            response.InsertStart(edStoreBank.CRC_A_ORB_OPENFILE);

            string staticUserHostedDir = Directory.GetCurrentDirectory() + $"/static/Eden/StaticUserHostedFiles/{task.Client.StorageUserId}";

            if (!bupdload_wanted && !Directory.Exists(staticUserHostedDir))
            {
                LoggerAccessor.LogWarn($"[OpenFile] - Static User file repository expected at path:{staticUserHostedDir}, sending error response...");
                SetFailure(response);
            }
            else
            {
                string suffix;

                switch (facility)
                {
                    case 2: // Core files.
                        suffix = "/Core";
                        break;
                    default:
                        LoggerAccessor.LogWarn($"[OpenFile] - Unknown facility:{facility} requested, please report to GITHUB.");
                        SetFailure(response);

                        response.InsertEnd();

                        task.Response = response;
                        task.Target = endpoint;
                        task.ClientMode = ClientMode.ProxyServer;

                        return null;
                }

                Directory.CreateDirectory(staticUserHostedDir + suffix);

                uint fileId = CRC32.CreateCastagnoli(Encoding.UTF8.GetBytes(filename));
                Dictionary<uint, string> filePaths = new Dictionary<uint, string>();

                foreach (string filePath in Directory.GetFiles(staticUserHostedDir + suffix, "*.*"))
                {
                    filePaths[CRC32.CreateCastagnoli(Encoding.UTF8.GetBytes(Path.GetFileName(filePath)))] = filePath;
                }

                bool fileExists = filePaths.ContainsKey(fileId);

                if (bupdload_wanted)
                {
                    DriveInfo drive = new DriveInfo(Path.GetPathRoot(Assembly.GetExecutingAssembly().Location));
                    if (drive.IsReady && uploadtotalsize <= drive.AvailableFreeSpace)
                    {
                        if (!fileExists) // Create the file buffer in the user folder, waiting for the PUT data to be pushed.
                            System.IO.File.WriteAllBytes($"{staticUserHostedDir + suffix}/{filename}", Array.Empty<byte>());

                        response.InsertUInt8(0); // Success.
                        response.InsertUInt32(fileId);
                        response.InsertUInt32(uploadtotalsize);
                        response.InsertUInt32(maxFsRetry);
                        response.InsertUInt32(timeout);
                    }
                    else
                        SetFailure(response);
                }
                else if (fileExists)
                {
                    response.InsertUInt8(0); // Success.
                    response.InsertUInt32(fileId);
                    response.InsertUInt32((uint)new FileInfo(filePaths[fileId]).Length);
                    response.InsertUInt32(maxFsRetry);
                    response.InsertUInt32(timeout);
                }
                else
                {
                    LoggerAccessor.LogWarn($"[OpenFile] - File:{filename} was not found for userId:{userId}, sending error response...");
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
            store.InsertUInt32(0);
            store.InsertUInt32(0);
        }
    }
}
