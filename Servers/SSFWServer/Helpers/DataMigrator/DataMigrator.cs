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
                foreach (FileSystemInfo item in new DirectoryInfo(ssfwrootDirectory + directory).AllFilesAndFoldersLinq().Where(item => item.FullName.Contains(oldStr)))
                {
                    // Construct the full path for the new file/folder in the target directory
                    string newPath = item.FullName.Replace(oldStr, newStr);

                    // Check if it's a file or directory and copy accordingly
                    if (item is FileInfo fileInfo && !File.Exists(newPath))
                    {
                        string? directoryPath = Path.GetDirectoryName(newPath);

                        if (!string.IsNullOrEmpty(directoryPath))
                            Directory.CreateDirectory(directoryPath);

                        File.Copy(item.FullName, newPath);
                    }
                    else if (item is DirectoryInfo directoryInfo && !Directory.Exists(newPath))
                        CopyDirectory(directoryInfo.FullName, newPath);
                }
            }
        }

        // Helper method to recursively copy directories
        private static void CopyDirectory(string source, string target)
        {
            Directory.CreateDirectory(target);

            foreach (string file in Directory.GetFiles(source))
            {
                string destinationFile = Path.Combine(target, Path.GetFileName(file));
                if (!File.Exists(destinationFile))
                {
                    string? directoryPath = Path.GetDirectoryName(destinationFile);

                    if (!string.IsNullOrEmpty(directoryPath))
                        Directory.CreateDirectory(directoryPath);

                    File.Copy(file, destinationFile);
                }
            }

            foreach (string directory in Directory.GetDirectories(source))
            {
                string destinationDirectory = Path.Combine(target, Path.GetFileName(directory));
                if (!Directory.Exists(destinationDirectory))
                {
                    CopyDirectory(directory, destinationDirectory);
                }
            }
        }
    }
}
