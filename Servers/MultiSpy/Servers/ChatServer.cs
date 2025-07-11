using CustomLogger;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiSpy.Servers
{
    internal class ChatServer
    {
        private static readonly string? pythonPath = FindPythonPath();

        private Process? pythonProcess;
        private StreamReader? standardOutput;
        private StreamReader? errorOutput;

        private string? scriptPath;

        public Thread? Thread;
        public Thread? ErrThread;

        public ChatServer()
        {
            if (string.IsNullOrEmpty(pythonPath))
            {
                LoggerAccessor.LogError("[ChatServer] - Python installation invalid, please make sure to install python with the PATH option selected, quitting the engine...");
                return;
            }

            scriptPath = MultiSpyServerConfiguration.ChatServerPath;

            if (!string.IsNullOrEmpty(scriptPath))
            {
                const string edgeZlibExtension = ".EdgeZlib";

                string zlibFilePath = scriptPath + edgeZlibExtension;

                if (File.Exists(zlibFilePath) && !File.Exists(scriptPath))
                {
                    File.WriteAllBytes(scriptPath.Replace(edgeZlibExtension, string.Empty), CompressionLibrary.Edge.Zlib.EdgeZlibDecompress(File.ReadAllBytes(zlibFilePath)));
                    File.Move(zlibFilePath, zlibFilePath + ".old");
                }

                if (File.Exists(scriptPath))
                {
                    StringBuilder sb = new StringBuilder();

                    foreach (var kvp in MultiSpyServerConfiguration.GamesKey)
                    {
                        // Convert string to bytes (UTF-8) and encode as Python byte escape sequences
                        var bytes = Encoding.UTF8.GetBytes(kvp.Value);
                        var safeValue = string.Concat(bytes.Select(b => $"\\x{b:X2}"));

                        sb.AppendLine($"        \"{kvp.Key}\": b\"{safeValue}\",");
                    }

                    // Replace the dictionary contents inside the Python class string
                    File.WriteAllText(scriptPath, Regex.Replace(File.ReadAllText(scriptPath), @"(__gamekeys\s*=\s*\{)[\s\S]*?(\})", $"$1\n{sb}    $2"));
                }
                else
                {
                    LoggerAccessor.LogError("[ChatServer] - Python script not found, quitting the engine...");
                    return;
                }

                StartServer();
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

        private void StartServer()
        {
            LoggerAccessor.LogInfo("[ChatServer] - Starting Chat Server");

            // Create a new process for Python
            pythonProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    WorkingDirectory = Path.GetDirectoryName(scriptPath),
                    FileName = Path.Combine(pythonPath, "python.exe"),
#if DEBUG
                    Arguments = $"\"{scriptPath}\" --debug",
#else
                    Arguments = $"\"{scriptPath}\"",
#endif
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            // Start the process
            pythonProcess.Start();
            pythonProcess.PriorityClass = ProcessPriorityClass.High;

            // Get the standard output and error streams
            standardOutput = pythonProcess.StandardOutput;
            errorOutput = pythonProcess.StandardError;

            // Read both standard output and error in parallel threads
            Thread = new Thread(() =>
            {
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
            })
            { Name = "Chat Server" };
            Thread.Start();

            ErrThread = new Thread(() =>
            {
                string? error;
                try
                {
                    while ((error = errorOutput?.ReadLine()) != null)
                    {
                        LoggerAccessor.LogError("[ChatServer] - Python Error: " + error);
                    }
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError("[ChatServer] - Error while reading python error output: " + ex);
                }
            })
            { Name = "Chat Server Error" };
            ErrThread.Start();
        }

        private static string? FindPythonPath()
        {
            // Check the PATH environment variable for Python installation
            string? envPath = Environment.GetEnvironmentVariable("PATH");

            if (!string.IsNullOrEmpty(envPath))
            {
                foreach (string? path in envPath.Split(Path.PathSeparator))
                {
                    if (!string.IsNullOrEmpty(path) && path.Contains("python", StringComparison.InvariantCultureIgnoreCase) && IsPython3(path))
                        return Path.GetDirectoryName(path);
                }
            }

            return null;
        }

        private static bool IsPython3(string pythonPath)
        {
            if (File.Exists(pythonPath))
            {

            }
            else if (Directory.Exists(pythonPath))
            {
                string pythonExePath = pythonPath + ((pythonPath.EndsWith("\\") || pythonPath.EndsWith("/")) ? "python.exe" : "/python.exe");
                if (File.Exists(pythonExePath))
                    pythonPath = pythonExePath;
                else
                {
#if DEBUG
                    LoggerAccessor.LogWarn($"[ChatServer] - The path:{pythonPath} sepcified is not a valid python root path, skipping...");
#endif
                    return false;
                }
            }
            else
            {
                LoggerAccessor.LogError($"[ChatServer] - The path:{pythonPath} sepcified matched no valid folders/executables on the system.");
                return false;
            }

            try
            {
                using (Process? process = Process.Start(new ProcessStartInfo
                {
                    FileName = pythonPath,
                    Arguments = "--version",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }))
                {
                    if (process != null)
                    {
                        string output = process.StandardOutput.ReadToEnd().Trim();

                        process.WaitForExit();

                        // Check if the output indicates Python 3.x
                        return output.StartsWith("Python 3.");
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError("[ChatServer] - Error while checking if python is at least of version 3: " + ex);
            }

            return false;
        }
    }
}
