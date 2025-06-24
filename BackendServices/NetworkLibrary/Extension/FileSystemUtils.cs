using CustomLogger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkLibrary.Extension
{
    public static class FileSystemUtils
    {
        public enum FileAccessMode
        {
            Read,
            Write
        }

        private const FileAttributes hiddenAttribute = FileAttributes.Hidden;

        public static IEnumerable<FileSystemInfo> AllFilesAndFolders(this DirectoryInfo directory)
        {
            if (!directory.IsHidden())
            {
                foreach (FileInfo f in directory.GetFiles().Where(file => !file.IsHidden()))
                    yield return f;
                foreach (DirectoryInfo d in directory.GetDirectories().Where(dir => !dir.IsHidden()))
                {
                    yield return d;
                    foreach (FileSystemInfo o in d.AllFilesAndFolders())
                        yield return o;
                }
            }
        }

        public static IEnumerable<FileSystemInfo> AllFilesAndFoldersLinq(this DirectoryInfo dir)
        {
            return dir.EnumerateFileSystemInfos("*", SearchOption.AllDirectories).AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount)
                .AsUnordered().Where(info => !info.IsHidden());
        }

        public static IEnumerable<string> GetMediaFilesList(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                return null;

            // Define a set of valid extensions for media quick lookup
            HashSet<string> validExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mp3", ".aac", ".ts" };

            return Directory.EnumerateFiles(directoryPath, "*.*")
                            .Where(s => validExtensions.Contains(Path.GetExtension(s)) && !File.GetAttributes(s)
                            .HasFlag(hiddenAttribute));
        }

        public static bool IsHidden(this FileSystemInfo fileSystemInfo)
        {
            return (fileSystemInfo.Attributes & hiddenAttribute) == hiddenAttribute;
        }

        public static bool IsHidden(this DirectoryInfo directorySystemInfo)
        {
            return (directorySystemInfo.Attributes & hiddenAttribute) == hiddenAttribute;
        }

        public static bool IsHidden(this FileInfo fileInfo)
        {
            return (fileInfo.Attributes & hiddenAttribute) == hiddenAttribute;
        }

        // https://stackoverflow.com/questions/24279882/file-open-hangs-and-freezes-thread-when-accessing-a-local-file
        public static async Task<bool> IsLocked(this FileInfo file, FileAccessMode mode)
        {
            Task<bool> checkTask = Task.Run(() =>
            {
                try
                {
                    if (mode == FileAccessMode.Write)
                        using (file.Open(FileMode.Open, FileAccess.Read, FileShare.None)) { }
                    else
                        using (file.OpenRead()) { }
                    return false;
                }
                catch
                {
                }
                return true;
            });
            Task delayTask = Task.Delay(1000);
            try
            {
                if ((await Task.WhenAny(checkTask, delayTask).ConfigureAwait(false)) == delayTask)
                    return true;
                else
                    return await checkTask.ConfigureAwait(false);
            }
            catch
            {
            }
            return true;
        }

        public static async Task<FileStream> TryOpen(string filePath, FileAccessMode mode, int AwaiterTimeoutInMS = -1)
        {
            if (AwaiterTimeoutInMS != -1)
            {
                FileInfo info;
                try
                {
                    info = new FileInfo(filePath);
                }
                catch
                {
                    return null;
                }
                const int lockCheckInterval = 100;
                int elapsedTime = 0;
                while (await IsLocked(info, mode).ConfigureAwait(false))
                {
                    if (elapsedTime >= AwaiterTimeoutInMS)
                        return null;
                    await Task.Delay(lockCheckInterval).ConfigureAwait(false);
                    elapsedTime += lockCheckInterval;
                }
            }
            try
            {
                return File.OpenRead(filePath);
            }
            catch
            {
            }

            return null;
        }

        public static long GetLength(this DirectoryInfo dir)
        {
            return Directory.GetFiles(dir.FullName, "*", SearchOption.AllDirectories).AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount)
                .AsUnordered().Sum(t => new FileInfo(t).Length);
        }

        private static void SetFileReadWrite(string filePath)
        {
            if ((File.GetAttributes(filePath) & FileAttributes.ReadOnly) != FileAttributes.ReadOnly)
                return;
            File.SetAttributes(filePath, File.GetAttributes(filePath) ^ FileAttributes.ReadOnly);
        }

        public static string RemoveInvalidPathChars(string input)
        {
            const string allowedChars = "-_.abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/\\";
            StringBuilder empty = new StringBuilder();
            foreach (char ch in input)
            {
                if (allowedChars.Contains(ch.ToString()))
                    empty.Append(ch);
                else if (ch == ' ')
                    empty.Append('_');
            }
            return empty.ToString();
        }

        /// <summary>
        /// Compute the MD5 checksum of a file.
        /// <para>Calcul la somme des contr�les en MD5 d'un fichier.</para>
        /// </summary>
        /// <param name="filePath">The input file path.</param>
        /// <returns>A nullable string.</returns>
        public static string ComputeMD5FromFile(string filePath)
        {
            try
            {
                return NetHasher.DotNetHasher.ComputeMD5String(File.OpenRead(filePath));
            }
            catch
            {
            }
            return null;
        }

        /// <summary>
        /// Reads a fragment of a file with a given indicator.
        /// <para>Lire un fragment de fichier avec un indicateur explicite.</para>
        /// </summary>
        /// <param name="filePath">The path of the desired file.</param>
        /// <param name="bytesToRead">The amount of desired fragment data.</param>
        /// <returns>A byte array.</returns>
        public static byte[] ReadFileChunck(string filePath, int bytesToRead)
        {
            if (bytesToRead <= 0)
                throw new ArgumentOutOfRangeException(nameof(bytesToRead), "[FileSystemUtils] - ReadFileChunck() - Number of bytes to read must be greater than zero.");

            int bytesRead;
#if NET5_0_OR_GREATER
            Span<byte> result = new byte[bytesToRead];
#else
            byte[] result = new byte[bytesToRead];
#endif
            try
            {
                using (FileStream fileStream = File.OpenRead(filePath))
                {
#if NET5_0_OR_GREATER
                    bytesRead = fileStream.Read(result);
#else
                    bytesRead = fileStream.Read(result, 0, bytesToRead);
#endif
                }

                // If the file is less than 'bytesToRead', pad with null bytes
                if (bytesRead < bytesToRead)
                {
#if NET5_0_OR_GREATER
                    result[bytesRead..].Fill(0);
#else
                    Array.Clear(result, bytesRead, bytesToRead - bytesRead);
#endif
                }
            }
            catch
            {
            }

            return result.ToArray();
        }

        public static string GetM3UStreamFromDirectory(string directoryPath, string httpdirectoryrequest)
        {
            try
            {
                IEnumerable<string> MediaPaths = GetMediaFilesList(directoryPath);

                if (MediaPaths != null && MediaPaths.Any())
                {
                    StringBuilder builder = new StringBuilder();

                    // Add M3U header
                    builder.AppendLine("#EXTM3U");

                    foreach (string mediaPath in MediaPaths)
                    {
                        string fileName = Path.GetFileName(mediaPath);

                        // Add the song path to the playlist
                        builder.AppendLine($"#EXTINF:,-1,{fileName}");
                        builder.AppendLine(httpdirectoryrequest + $"/{fileName}");
                    }

                    return builder.ToString();
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[GetM3UStreamFromDirectory] - Errored out with exception - {ex}");
            }

            return null;
        }
    }
}
