using MultiServerLibrary.Extension;

namespace SSFWServer.Helpers.DataMigrator
{
    public class DataMigrator
    {
        public static void MigrateSSFWData(string ssfwrootDirectory, string oldStr, string? newStr)
        {
            if (string.IsNullOrEmpty(newStr))
                return;

            foreach (string directory in new string[] { "/AvatarLayoutService", "/LayoutService", "/RewardsService", "/SaveDataService" })
            {
                foreach (FileSystemInfo item in FileSystemUtils.AllFilesAndFoldersLinq(new DirectoryInfo(ssfwrootDirectory + directory)).Where(item => item.FullName.Contains(oldStr)))
                {
                    // Construct the full path for the new file/folder in the target directory
                    string newFilePath = item.FullName.Replace(oldStr, newStr);

                    // Check if it's a file or directory and copy accordingly
                    if ((item is FileInfo fileInfo) && !File.Exists(newFilePath))
                    {
                        string? directoryPath = Path.GetDirectoryName(newFilePath);

                        if (!string.IsNullOrEmpty(directoryPath))
                            Directory.CreateDirectory(directoryPath);

                        File.Copy(item.FullName, newFilePath);

                        FileSystemUtils.SetFileReadWrite(newFilePath);
                    }
                    else if ((item is DirectoryInfo directoryInfo) && !Directory.Exists(newFilePath))
                        CopyDirectory(directoryInfo.FullName, newFilePath);
                }
            }
        }

        // Helper method to recursively copy directories
        private static void CopyDirectory(string source, string target)
        {
            Directory.CreateDirectory(target);

            foreach (string file in Directory.GetFiles(source))
            {
                string newFilePath = Path.Combine(target, Path.GetFileName(file));
                if (!File.Exists(newFilePath))
                {
                    string? directoryPath = Path.GetDirectoryName(newFilePath);

                    if (!string.IsNullOrEmpty(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    File.Copy(file, newFilePath);

                    FileSystemUtils.SetFileReadWrite(newFilePath);
                }
            }

            foreach (string directory in Directory.GetDirectories(source))
            {
                string destinationDirectory = Path.Combine(target, Path.GetFileName(directory));
                if (!Directory.Exists(destinationDirectory))
                    CopyDirectory(directory, destinationDirectory);
            }
        }
    }
}
