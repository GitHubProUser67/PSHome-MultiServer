using System.Collections.Concurrent;
using System.Net;
using System.Reflection;
using CustomLogger;
using EdNetService.CRC;
using EdNetService.Models;
using MultiServerLibrary.Extension.NET;

namespace EdenServer.EdNet.ProxyMessages.ORB.File
{
    public class OpenFile : AbstractProxyMessage
    {
        private const uint maxFsRetry = 10;
        private const uint timeout = 6000;

        public static readonly UniqueIDGenerator FileSystemIdCounter = new();

        public static readonly ConcurrentDictionary<uint, (string, byte[], bool)> FileSystemCache =
            new();

        public override byte[]? Process(
            IPEndPoint endpoint,
            IPEndPoint target,
            ClientTask task,
            ushort PacketMagic
        )
        {
            var request = task.Request;

            var facility = request.ExtractUInt8();
            var userId = request.ExtractUInt32();
            var filename = request.ExtractString();
            var bupdload_wanted = request.ExtractUInt8() == 0x01;
            var uploadtotalsize = request.ExtractUInt32();

            var response = new EdStore(null, 1400);

            response.InsertStart(edStoreBank.CRC_A_ORB_OPENFILE);

            var staticUserHostedDir =
                Directory.GetCurrentDirectory() + $"/static/Eden/StaticUserHostedFiles/{userId}";

            if (!bupdload_wanted && !Directory.Exists(staticUserHostedDir))
            {
                LoggerAccessor.LogWarn(
                    $"[OpenFile] - Static User file repository expected at path:{staticUserHostedDir}, sending error response..."
                );
                SetFailure(response);
            }
            else
            {
                string suffix;

                switch (facility)
                {
                    case 2: // Core files.
                        suffix = "/Core/";
                        break;
                    default:
                        LoggerAccessor.LogWarn(
                            $"[OpenFile] - Unknown facility:{facility} requested, please report to GITHUB."
                        );
                        SetFailure(response);

                        response.InsertEnd();

                        task.Response = response;
                        task.Target = endpoint;
                        task.ClientMode = ClientMode.ProxyServer;

                        return null;
                }

                var fileId = FileSystemIdCounter.CreateUniqueID();
                var directoryPath = staticUserHostedDir + suffix;
                var filePath = directoryPath + filename;

                Directory.CreateDirectory(directoryPath);

                if (bupdload_wanted)
                {
                    var drive = new DriveInfo(
                        Path.GetPathRoot(Assembly.GetExecutingAssembly().Location)
                    );
                    if (
                        drive.IsReady
                        && uploadtotalsize <= drive.AvailableFreeSpace
                        && FileSystemCache.TryAdd(
                            fileId,
                            (filePath, new byte[uploadtotalsize], true)
                        )
                    )
                    {
                        response.InsertUInt8(0); // Success.
                        response.InsertUInt32(fileId);
                        response.InsertUInt32(uploadtotalsize);
                        response.InsertUInt32(maxFsRetry);
                        response.InsertUInt32(timeout);
                    }
                    else
                    {
                        FileSystemIdCounter.ReleaseID(fileId);
                        SetFailure(response);
                    }
                }
                else if (System.IO.File.Exists(filePath))
                {
                    var fileSize = (uint)new FileInfo(filePath).Length;

                    if (
                        FileSystemCache.TryAdd(
                            fileId,
                            (filePath, System.IO.File.ReadAllBytes(filePath), false)
                        )
                    )
                    {
                        response.InsertUInt8(0); // Success.
                        response.InsertUInt32(fileId);
                        response.InsertUInt32(fileSize);
                        response.InsertUInt32(maxFsRetry);
                        response.InsertUInt32(timeout);
                    }
                    else
                    {
                        FileSystemIdCounter.ReleaseID(fileId);
                        SetFailure(response);
                    }
                }
                else
                {
                    FileSystemIdCounter.ReleaseID(fileId);
                    LoggerAccessor.LogWarn(
                        $"[OpenFile] - File:{filename} was not found for userId:{userId}, sending error response..."
                    );
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
