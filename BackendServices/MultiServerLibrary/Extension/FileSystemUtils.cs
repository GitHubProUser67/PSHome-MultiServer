using System.Text;

namespace MultiServerLibrary.Extension
{
    public static class FileSystemUtils
    {
        public const string ASCIIChars =
            "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private const FileAttributes hiddenAttribute = FileAttributes.Hidden;

        // Define a set of valid extensions for media quick lookup
        public static HashSet<string> ValidM3UExtensions { get; set; } =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { ".mp3", ".aac", ".ts" };

        extension(DirectoryInfo dir)
        {
            public IEnumerable<FileSystemInfo> AllFilesAndFolders()
            {
                if (!dir.IsHidden())
                {
                    foreach (var f in dir.GetFiles().Where(file => !file.IsHidden()))
                        yield return f;
                    foreach (var d in dir.GetDirectories().Where(dir => !dir.IsHidden()))
                    {
                        yield return d;
                        foreach (var o in d.AllFilesAndFolders())
                            yield return o;
                    }
                }
            }

            public IEnumerable<FileSystemInfo> AllFilesAndFoldersLinq(bool multiThread = false)
            {
                return multiThread
                    ? dir.EnumerateFileSystemInfos("*", SearchOption.AllDirectories)
                        .AsParallel()
                        .WithDegreeOfParallelism(Environment.ProcessorCount)
                        .AsUnordered()
                        .Where(info => !info.IsHidden())
                    : dir.EnumerateFileSystemInfos("*", SearchOption.AllDirectories)
                        .Where(info => !info.IsHidden());
            }

            public bool IsHidden()
            {
                return (dir.Attributes & hiddenAttribute) == hiddenAttribute;
            }

            public long GetLength(bool multiThread = false)
            {
                return multiThread
                    ? Directory
                        .GetFiles(dir.FullName, "*", SearchOption.AllDirectories)
                        .AsParallel()
                        .WithDegreeOfParallelism(Environment.ProcessorCount)
                        .AsUnordered()
                        .Sum(t => new FileInfo(t).Length)
                    : Directory
                        .GetFiles(dir.FullName, "*", SearchOption.AllDirectories)
                        .Sum(t => new FileInfo(t).Length);
            }
        }

        extension(FileSystemInfo fsi)
        {
            public bool IsHidden()
            {
                return (fsi.Attributes & hiddenAttribute) == hiddenAttribute;
            }
        }

        extension(FileInfo fi)
        {
            public bool IsHidden()
            {
                return (fi.Attributes & hiddenAttribute) == hiddenAttribute;
            }

            // https://stackoverflow.com/questions/24279882/file-open-hangs-and-freezes-thread-when-accessing-a-local-file
            public async Task<bool> IsLocked(FileShare mode)
            {
                var checkTask = Task.Run(() =>
                {
                    try
                    {
                        using (fi.Open(FileMode.Open, FileAccess.Read, mode)) { }
                        return false;
                    }
                    catch { }
                    return true;
                });
                var delayTask = Task.Delay(1000);
                try
                {
                    return (await Task.WhenAny(checkTask, delayTask).ConfigureAwait(false))
                            == delayTask
                        || await checkTask.ConfigureAwait(false);
                }
                catch { }
                return true;
            }
        }

        public static IEnumerable<string> GetMediaFilesList(string directoryPath)
        {
            return string.IsNullOrEmpty(directoryPath) || !Directory.Exists(directoryPath)
                ? null
                : Directory
                    .EnumerateFiles(directoryPath, "*.*")
                    .Where(s =>
                        ValidM3UExtensions.Contains(Path.GetExtension(s))
                        && !File.GetAttributes(s).HasFlag(hiddenAttribute)
                    );
        }

        public static async Task<FileStream> TryOpen(
            string filePath,
            FileShare mode,
            int AwaiterTimeoutInMS = -1
        )
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
                var elapsedTime = 0;
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
                return new FileStream(filePath, FileMode.Open, FileAccess.Read, mode);
            }
            catch { }
            return null;
        }

        public static void SetFileReadWrite(string filePath)
        {
            if (
                !File.Exists(filePath)
                || (File.GetAttributes(filePath) & FileAttributes.ReadOnly)
                    != FileAttributes.ReadOnly
            )
                return;
            File.SetAttributes(filePath, File.GetAttributes(filePath) ^ FileAttributes.ReadOnly);
        }

        public static string RemoveInvalidPathChars(string input)
        {
            var allowedChars = $"[]-_.+{ASCIIChars}/\\ ";
            var empty = new StringBuilder();
            foreach (var ch in input)
            {
                if (allowedChars.Contains(ch.ToString()))
                    empty.Append(ch);
            }
            return empty.ToString();
        }

        /// <summary>
        /// Reads a fragment of a file with a given indicator.
        /// <para>Lire un fragment de fichier avec un indicateur explicite.</para>
        /// </summary>
        /// <param name="filePath">The path of the desired file.</param>
        /// <param name="bytesToRead">The amount of desired fragment data.</param>
        /// <returns>A byte array.</returns>
        public static byte[] TryReadFileChunck(
            string filePath,
            int bytesToRead,
            FileShare mode,
            int AwaiterTimeoutInMS = -1
        )
        {
            if (bytesToRead <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(bytesToRead),
                    "[FileSystemUtils] - ReadFileChunck() - Number of bytes to read must be greater than zero."
                );

            int bytesRead;
            Span<byte> result = new byte[bytesToRead];
            try
            {
                using (var fileStream = TryOpen(filePath, mode, AwaiterTimeoutInMS).Result)
                {
                    bytesRead = fileStream.Read(result);
                }

                // If the file is less than 'bytesToRead', pad with null bytes
                if (bytesRead < bytesToRead)
                {
                    result[bytesRead..].Clear();
                }
            }
            catch
            {
                // Failed to read file, returning nulled out array (function is not expected to return the data everytime, hence the Try, but it should be very rare).
            }

            return result.ToArray();
        }
    }
}
