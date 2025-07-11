using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace RemoteControl
{ 
    // A simple process manager class to keep track of processes made using ChatGPT
    public static class ProcessManager
    {
        private static readonly object _lock = new object();
        public static readonly Dictionary<uint, Process> Processes = new Dictionary<uint, Process>();

        public static void StartupProgram(ControlWriter writer, TextBox textBox, GroupBox groupBox, string appPrefix, string exePath, uint appid)
        {
            // Start the process in the background
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = exePath,
                WorkingDirectory = Path.GetDirectoryName(exePath),
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            if (MultiServerLibrary.Extension.Microsoft.Win32API.IsWindows && MultiServerLibrary.Extension.Microsoft.Win32API.IsAdministrator())
                startInfo.Verb = "runas";

            Process process = new Process()
            {
                StartInfo = startInfo,
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (sender, e) =>
            {
                writer.WriteLine(e.Data);
            };

            process.Exited += (sender, e) =>
            {
                writer.WriteLine($"[{appid}] Process exited with code {process.ExitCode}");
                if (ShutdownProcess(appid))
                {
                    textBox.Invoke(new Action(() =>
                    {
                        textBox.Text = "Gracefully Shutdown";
                    }));
                    groupBox.Invoke(new Action(() =>
                    {
                        groupBox.BackColor = Color.Yellow;
                    }));
                    CustomLogger.LoggerAccessor.LogWarn($"[{appPrefix}] - Server shutdown at:{DateTime.Now}!");
                }
            };

            // Start the process
            process.Start();
            process.BeginOutputReadLine();

            // Store appid and process information for later shutdown
            RegisterProcess(appid, process);
        }

        public static void RegisterProcess(uint appid, Process process)
        {
            lock (_lock)
                Processes[appid] = process;
        }

        public static bool ShutdownProcess(uint appid)
        {
            lock (_lock)
            {
                if (Processes.TryGetValue(appid, out Process process) && process != null)
                {
                    process.CloseMainWindow(); // Try to close the main window gracefully

                    if (!process.HasExited)
                        process.Kill();

                    // Remove the process from the manager
                    return Processes.Remove(appid);
                }
            }

            return false;
        }
    }
}
