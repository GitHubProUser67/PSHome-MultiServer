using CustomLogger;
using System.Diagnostics;

namespace MultiSpy.Servers
{
    internal class ChatServer
    {
        private static readonly string? pythonPath = FindPythonPath();

        private Process? pythonProcess;
        private StreamReader? standardOutput;

        private string? scriptPath;

        public Thread? Thread;

        public ChatServer()
        {
            if (string.IsNullOrEmpty(pythonPath))
            {
                LoggerAccessor.LogError("[ChatServer] - Python installation invalid, quitting the engine...");
                return;
            }

            scriptPath = MultiSpyServerConfiguration.ChatServerPath;

            if (!string.IsNullOrEmpty(scriptPath))
            {
                const string edgeZlibExtension = ".EdgeZlib";

                string zlibFilePath = scriptPath + edgeZlibExtension;

                if (File.Exists(zlibFilePath) && !File.Exists(scriptPath))
                {
                    File.WriteAllBytes(scriptPath.Replace(edgeZlibExtension, string.Empty), CompressionLibrary.Edge.Zlib.EdgeZlibDecompress(File.ReadAllBytes(zlibFilePath)).Result);
                    File.Move(zlibFilePath, zlibFilePath + ".old");
                }
                else if (File.Exists(scriptPath))
                {
                    
                }
                else
                {
                    LoggerAccessor.LogError("[ChatServer] - Python script not found, quitting the engine...");
                    return;
                }

                Thread = new Thread(StartServer)
                {
                    Name = "Chat Thread"
                };
                Thread.Start();
            }
            else
                LoggerAccessor.LogError("[ChatServer] - Python script path was invalid, quitting the engine...");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (pythonProcess != null && !pythonProcess.HasExited)
                    {
                        try
                        {
                            // Kill the Python process
                            pythonProcess.Kill();
                            pythonProcess.WaitForExit();
                        }
                        catch (Exception ex)
                        {
                            LoggerAccessor.LogError("[ChatServer] - Error terminating the python process: " + ex);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        ~ChatServer()
        {
            Dispose(false);
        }

        private void StartServer(object? parameter)
        {
            LoggerAccessor.LogInfo("[ChatServer] - Starting Chat Server");

            // Create a new process for Python
            pythonProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Path.GetDirectoryName(scriptPath),
                    FileName = Path.Combine(pythonPath, "python.exe"),
                    Arguments = $"\"{scriptPath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            // Start the process
            pythonProcess.Start();
            pythonProcess.PriorityClass = ProcessPriorityClass.High;

            // Get the standard output
            standardOutput = pythonProcess.StandardOutput;

            // Read the output of the Python script line by line
            string? output;

            try
            {
                while ((output = standardOutput?.ReadLine()) != null)
                {
                    LoggerAccessor.LogInfo("[ChatServer] - " + output);
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError("[ChatServer] - Error while reading python output: " + ex);
            }
        }

        private static string? FindPythonPath()
        {
            // Check the PATH environment variable for Python installation
            string? envPath = Environment.GetEnvironmentVariable("PATH");

            if (!string.IsNullOrEmpty(envPath))
            {
                foreach (string? path in envPath.Split(Path.PathSeparator))
                {
                    if (!string.IsNullOrEmpty(path) && path.Contains("python", StringComparison.InvariantCultureIgnoreCase)
                        && File.Exists(path) && IsPython27(path))
                        return Path.GetDirectoryName(path);
                }
            }

            return null;
        }

        private static bool IsPython27(string pythonExePath)
        {
            try
            {
                using (Process? process = Process.Start(new ProcessStartInfo
                {
                    FileName = pythonExePath,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }))
                {
                    if (process != null)
                    {
                        string output = process.StandardError.ReadToEnd().Trim(); // Version info is usually in stderr

                        process.WaitForExit();

                        // Check if the output indicates Python 2.7
                        return output.StartsWith("Python 2.7");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError("[ChatServer] - Error while checking if python is of version 2.7: " + ex);
            }

            return false;
        }
    }
}
