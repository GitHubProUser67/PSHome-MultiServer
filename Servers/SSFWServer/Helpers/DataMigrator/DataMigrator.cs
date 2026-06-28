using MultiServerLibrary.Extension;

namespace SSFWServer.Helpers.DataMigrator
{
    public class DataMigrator
    {
        public static void MigrateSSFWData(string ssfwrootDirectory, string oldStr, string? newStr)
        {
            if (string.IsNullOrEmpty(newStr))
                return;

            foreach (
                var directory in new string[]
                {
                    "/AvatarLayoutService",
                    "/LayoutService",
                    "/RewardsService",
                    "/SaveDataService",
                }
            )
            {
                foreach (
                    var item in FileSystemUtils
                        .AllFilesAndFoldersLinq(new DirectoryInfo(ssfwrootDirectory + directory))
                        .Where(item => item.FullName.Contains(oldStr))
                )
                {
                    // Construct the full path for the new file/folder in the target directory
                    var newFilePath = item.FullName.Replace(oldStr, newStr);

                    // Check if it's a file or directory and copy accordingly
                    if ((item is FileInfo fileInfo) && !File.Exists(newFilePath))
                    {
                        var directoryPath = Path.GetDirectoryName(newFilePath);

                        if (!string.IsNullOrEmpty(directoryPath))
                            Directory.CreateDirectory(directoryPath);

                        File.Copy(item.FullName, newFilePath);

                        FileSystemUtils.SetFileReadWrite(newFilePath);
                    }
                    else if (
                        (item is DirectoryInfo directoryInfo) && !Directory.Exists(newFilePath)
                    )
                        CopyDirectory(directoryInfo.FullName, newFilePath);
                }
            }
        }

        // Helper method to recursively copy directories
        private static void CopyDirectory(string source, string target)
        {
            Directory.CreateDirectory(target);

            foreach (var file in Directory.GetFiles(source))
            {
                var newFilePath = Path.Combine(target, Path.GetFileName(file));
                if (!File.Exists(newFilePath))
                {
                    var directoryPath = Path.GetDirectoryName(newFilePath);

                    if (!string.IsNullOrEmpty(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    File.Copy(file, newFilePath);

                    FileSystemUtils.SetFileReadWrite(newFilePath);
                }
            }

            foreach (var directory in Directory.GetDirectories(source))
            {
                var destinationDirectory = Path.Combine(target, Path.GetFileName(directory));
                if (!Directory.Exists(destinationDirectory))
                    CopyDirectory(directory, destinationDirectory);
            }
        }
    }
}
