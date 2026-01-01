using CustomLogger;
using Newtonsoft.Json;

namespace SSFWServer.Services
{
    public class SaveDataService
    {
        public string? DebugGetFileList(string directoryPath, string? segment)
        {
            try
            {
                if (segment != null)
                {
                    List<FileItem>? files = GetFilesInfo(directoryPath + "/" + segment);

                    if (files != null)
                        return JsonConvert.SerializeObject(new FilesContainer() { files = files }, Formatting.Indented);
                }
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError($"[SSFW] -  DebugGetFileList ERROR: \n{e}");
            }

            return null;
        }

        private static List<FileItem>? GetFilesInfo(string directoryPath)
        {
            List<FileItem> files = new();
            try
            {

                foreach (string filePath in Directory.GetFiles(directoryPath))
                {
                    FileInfo fileInfo = new(filePath);
                    files.Add(new FileItem()
                    {
                        objectId = Path.GetFileNameWithoutExtension(fileInfo.Name),
                        size = (int)fileInfo.Length,
                        lastUpdate = (long)fileInfo.LastWriteTime.Subtract(DateTime.UnixEpoch).TotalSeconds
                    });
                }

                return files;
            }
            catch (Exception e)
            {
                LoggerAccessor.LogError($"[SSFW] - SaveDataDebug GetFileList ERROR: \n{e}");
            }

            return null;
        }

        private class FileItem
        {
            public string? objectId { get; set; }
            public int size { get; set; }
            public long lastUpdate { get; set; }
        }

        private class FilesContainer
        {
            public List<FileItem>? files { get; set; }
        }
    }
}
