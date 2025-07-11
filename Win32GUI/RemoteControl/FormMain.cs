using CustomLogger;
using Microsoft.Extensions.Logging;
using NetHasher.CRC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RemoteControl
{
    public partial class FormMain : Form
    {
        private static string currentDir = Directory.GetCurrentDirectory();
        private static readonly string jsonEditorPath = currentDir + "/ZTn.Json.Editor.exe";

        private Dictionary<uint, ControlWriter> _writersList = new Dictionary<uint, ControlWriter>();

        private const string exeExtension = ".exe";
        private const string jsonExtension = ".json";

        private readonly uint httpCRC = CRC32.CreateCastagnoli(Encoding.ASCII.GetBytes("ApacheNet" + exeExtension));
        private readonly uint dnsCRC = CRC32.CreateCastagnoli(Encoding.ASCII.GetBytes("MitmDNS" + exeExtension));
        private readonly uint horizonCRC = CRC32.CreateCastagnoli(Encoding.ASCII.GetBytes("Horizon" + exeExtension));
        private readonly uint multisocksCRC = CRC32.CreateCastagnoli(Encoding.ASCII.GetBytes("MultiSocks" + exeExtension));
        private readonly uint multispyCRC = CRC32.CreateCastagnoli(Encoding.ASCII.GetBytes("MultiSpy" + exeExtension));
        private readonly uint quazalCRC = CRC32.CreateCastagnoli(Encoding.ASCII.GetBytes("QuazalServer" + exeExtension));
        private readonly uint ssfwCRC = CRC32.CreateCastagnoli(Encoding.ASCII.GetBytes("SSFWServer" + exeExtension));
        private readonly uint svoCRC = CRC32.CreateCastagnoli(Encoding.ASCII.GetBytes("SVO" + exeExtension));
        private readonly uint edenCRC = CRC32.CreateCastagnoli(Encoding.ASCII.GetBytes("EdenServer" + exeExtension));

        public FormMain()
        {
            bool horizonConfsLoaded = false, multisocksConfsLoaded = false;

            InitializeComponent();

            LoggerAccessor.SetupLogger("GUI", currentDir);

            LoggerAccessor.RegisterPostLogAction(LogLevel.Information, (msg, args) =>
            {
                Console.WriteLine($"[INFO]: {msg}");
            });
            LoggerAccessor.RegisterPostLogAction(LogLevel.Warning, (msg, args) =>
            {
                Console.WriteLine($"[WARN]: {msg}");
            });
            LoggerAccessor.RegisterPostLogAction(LogLevel.Error, (msg, args) =>
            {
                Console.WriteLine($"[ERROR]: {msg}");
            });
            LoggerAccessor.RegisterPostLogAction(LogLevel.Critical, (msg, args) =>
            {
                Console.WriteLine($"[CRIT]: {msg}");
            });
#if DEBUG
            LoggerAccessor.RegisterPostLogAction(LogLevel.Debug, (msg, args) =>
            {
                Console.WriteLine($"[DBG]: {msg}");
            });
#endif
            string startupSoundPath = currentDir + "/startup.mp3";

            if (File.Exists(startupSoundPath))
            {
                using (FileStream jingleSt = File.OpenRead(startupSoundPath))
                    new SoundPlayer(jingleSt).Play();
            }

            richTextBoxLicense.SelectAll();
            richTextBoxLicense.SelectionAlignment = HorizontalAlignment.Center;

            // Settings programs path.
            textBoxApacheNetPath.Text = Program.currentDir + "/ApacheNet" + exeExtension;
            textBoxDNSPath.Text = Program.currentDir + "/MitmDNS" + exeExtension;
            textBoxHorizonPath.Text = Program.currentDir + "/Horizon" + exeExtension;
            textBoxMultisocksPath.Text = Program.currentDir + "/MultiSocks" + exeExtension;
            textBoxMultispyPath.Text = Program.currentDir + "/MultiSpy" + exeExtension;
            textBoxQuazalserverPath.Text = Program.currentDir + "/QuazalServer" + exeExtension;
            textBoxSSFWServerPath.Text = Program.currentDir + "/SSFWServer" + exeExtension;
            textBoxSVOPath.Text = Program.currentDir + "/SVO" + exeExtension;
            textBoxEdenserverPath.Text = Program.currentDir + "/EdenServer" + exeExtension;

            textBoxApacheNetJsonPath.Text = Program.currentDir + "/static/ApacheNet" + jsonExtension;
            textBoxDNSJsonPath.Text = Program.currentDir + "/static/MitmDNS" + jsonExtension;
            string horizonConfigPath = textBoxHorizonJsonPath.Text = Program.currentDir + "/static/Horizon" + jsonExtension;
            string multisocksConfigPath = textBoxMultisocksJsonPath.Text = Program.currentDir + "/static/MultiSocks" + jsonExtension;
            textBoxMultispyJsonPath.Text = Program.currentDir + "/static/MultiSpy" + jsonExtension;
            textBoxQuazalserverJsonPath.Text = Program.currentDir + "/static/QuazalServer" + jsonExtension;
            textBoxSSFWServerJsonPath.Text = Program.currentDir + "/static/SSFWServer" + jsonExtension;
            textBoxSVOJsonPath.Text = Program.currentDir + "/static/SVO" + jsonExtension;
            textBoxEdenserverJsonPath.Text = Program.currentDir + "/static/EdenServer" + jsonExtension;

            try
            {
                if (File.Exists(horizonConfigPath))
                {
                    var configRoot = JsonNode.Parse(File.ReadAllText(horizonConfigPath));
                    textBoxMediusJsonPath.Text = configRoot?["medius"]?["config"]?.ToString() ?? string.Empty;
                    textBoxDMEJsonPath.Text = configRoot?["dme"]?["config"]?.ToString() ?? string.Empty;
                    textBoxMUISJsonPath.Text = configRoot?["muis"]?["config"]?.ToString() ?? string.Empty;
                    textBoxNatJsonPath.Text = configRoot?["nat"]?["config"]?.ToString() ?? string.Empty;
                    textBoxBwpsJsonPath.Text = configRoot?["bwps"]?["config"]?.ToString() ?? string.Empty;
                    textBoxEbootDefsJsonPath.Text = configRoot?["eboot_defs_config"]?.ToString() ?? string.Empty;
                    textBoxHorizonDatabaseJsonPath.Text = configRoot?["database"]?.ToString() ?? string.Empty;

                    horizonConfsLoaded = true;
                }
            }
            catch
            {
            }

            try
            {
                if (File.Exists(multisocksConfigPath))
                {
                    var configRoot = JsonNode.Parse(File.ReadAllText(multisocksConfigPath));
                    textBoxAriesDatabaseJsonPath.Text = configRoot?["dirtysocks_database_path"]?.ToString() ?? string.Empty;

                    multisocksConfsLoaded = true;
                }
            }
            catch
            {
            }

            if (!horizonConfsLoaded)
            {
                textBoxNatJsonPath.Text = Program.currentDir + "/static/nat" + jsonExtension;
                textBoxBwpsJsonPath.Text = Program.currentDir + "/static/bwps" + jsonExtension;
                textBoxMediusJsonPath.Text = Program.currentDir + "/static/medius" + jsonExtension;
                textBoxDMEJsonPath.Text = Program.currentDir + "/static/dme" + jsonExtension;
                textBoxMUISJsonPath.Text = Program.currentDir + "/static/muis" + jsonExtension;
                textBoxEbootDefsJsonPath.Text = Program.currentDir + "/static/ebootdefs" + jsonExtension;
                textBoxHorizonDatabaseJsonPath.Text = Program.currentDir + "/static/db.config" + jsonExtension;
            }

            if (!multisocksConfsLoaded)
                textBoxAriesDatabaseJsonPath.Text = Program.currentDir + "/static/dirtysocks.db" + jsonExtension;

            _writersList.Add(httpCRC, new ControlWriter(richTextBoxHTTPLog));
            _writersList.Add(dnsCRC, new ControlWriter(richTextBoxDNSLog));
            _writersList.Add(horizonCRC, new ControlWriter(richTextBoxHorizonLog));
            _writersList.Add(multisocksCRC, new ControlWriter(richTextBoxMultisocksLog));
            _writersList.Add(multispyCRC, new ControlWriter(richTextBoxMultispyLog));
            _writersList.Add(quazalCRC, new ControlWriter(richTextBoxQuazalserverLog));
            _writersList.Add(ssfwCRC, new ControlWriter(richTextBoxSSFWServerLog));
            _writersList.Add(svoCRC, new ControlWriter(richTextBoxSVOLog));
            _writersList.Add(edenCRC, new ControlWriter(richTextBoxEdenserverLog));

            Console.SetOut(new MultiTextWriter(new ControlWriter(richTextBoxInformation), Console.Out));

            LoggerAccessor.LogInfo($"Remote Control started at: {Program.timeStarted}");

            // Attach the event handler to the FormClosing event
            FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you really want to close the application?\nShutdown can take a little while if servers are running.", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // If the user clicks "No", cancel the form closing
            if (result == DialogResult.No)
                e.Cancel = true;
            else
            {
                Console.SetOut(TextWriter.Null); // Avoids a race-condition when stalling apps try to write the info to the disposed richtextbox.

                Parallel.ForEach(ProcessManager.Processes.Keys, guid =>
                {
                    try
                    {
                        ProcessManager.ShutdownProcess(guid);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, $"Application with guid:{guid} thrown an exception while being closed.");
                    }
                });
            }
        }

        private void buttonBrowseHTTPPath_Click(object sender, EventArgs e)
        {
            Utils.UpdateConfigurationFile(textBoxApacheNetJsonPath, Utils.OpenExecutableFile(textBoxApacheNetPath));
        }

        private void buttonBrowseDNSPath_Click(object sender, EventArgs e)
        {
            Utils.UpdateConfigurationFile(textBoxDNSJsonPath, Utils.OpenExecutableFile(textBoxDNSPath));
        }

        private void buttonBrowseHorizonPath_Click(object sender, EventArgs e)
        {
            string exePath = Utils.OpenExecutableFile(textBoxHorizonPath);
            if (!string.IsNullOrEmpty(exePath))
            {
                var configRoot = JsonNode.Parse(File.ReadAllText(Utils.UpdateConfigurationFile(textBoxHorizonJsonPath, exePath)));
                textBoxMediusJsonPath.Text = configRoot?["medius"]?["config"]?.ToString() ?? string.Empty;
                textBoxDMEJsonPath.Text = configRoot?["dme"]?["config"]?.ToString() ?? string.Empty;
                textBoxMUISJsonPath.Text = configRoot?["muis"]?["config"]?.ToString() ?? string.Empty;
                textBoxNatJsonPath.Text = configRoot?["nat"]?["config"]?.ToString() ?? string.Empty;
                textBoxBwpsJsonPath.Text = configRoot?["bwps"]?["config"]?.ToString() ?? string.Empty;
                textBoxEbootDefsJsonPath.Text = configRoot?["eboot_defs_config"]?.ToString() ?? string.Empty;
                textBoxHorizonDatabaseJsonPath.Text = configRoot?["database"]?.ToString() ?? string.Empty;
            }
        }

        private void buttonBrowseMultisocksPath_Click(object sender, EventArgs e)
        {
            string exePath = Utils.OpenExecutableFile(textBoxMultisocksPath);
            if (!string.IsNullOrEmpty(exePath))
            {
                var configRoot = JsonNode.Parse(File.ReadAllText(Utils.UpdateConfigurationFile(textBoxMultisocksJsonPath, exePath)));
                textBoxAriesDatabaseJsonPath.Text = configRoot?["dirtysocks_database_path"]?.ToString() ?? string.Empty;
            }
        }

        private void buttonBrowseMultispyPath_Click(object sender, EventArgs e)
        {
            Utils.UpdateConfigurationFile(textBoxMultispyJsonPath, Utils.OpenExecutableFile(textBoxMultispyPath));
        }

        private void buttonBrowseQuazalserverPath_Click(object sender, EventArgs e)
        {
            Utils.UpdateConfigurationFile(textBoxQuazalserverJsonPath, Utils.OpenExecutableFile(textBoxQuazalserverPath));
        }

        private void buttonBrowseSSFWServerPath_Click(object sender, EventArgs e)
        {
            Utils.UpdateConfigurationFile(textBoxSSFWServerJsonPath, Utils.OpenExecutableFile(textBoxSSFWServerPath));
        }

        private void buttonBrowseSVOPath_Click(object sender, EventArgs e)
        {
            Utils.UpdateConfigurationFile(textBoxSVOJsonPath, Utils.OpenExecutableFile(textBoxSVOPath));
        }

        private void buttonBrowseEdenserverPath_Click(object sender, EventArgs e)
        {
            Utils.UpdateConfigurationFile(textBoxEdenserverJsonPath, Utils.OpenExecutableFile(textBoxEdenserverPath));
        }

        private void buttonStartHTTP_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                const string prefix = "ApacheNet";

                try
                {
                    _writersList[httpCRC].Flush();
                    ProcessManager.StartupProgram(_writersList[httpCRC], textBoxHTTP, groupBoxHTTP, prefix, textBoxApacheNetPath.Text, httpCRC);
                    textBoxHTTP.Invoke(new Action(() =>
                    {
                        textBoxHTTP.Text = "Running";
                    }));
                    groupBoxHTTP.Invoke(new Action(() =>
                    {
                        groupBoxHTTP.BackColor = Color.Green;
                    }));
                    LoggerAccessor.LogInfo($"[{prefix}] - Server started at: {DateTime.Now}!");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogInfo($"[{prefix}] - An assertion was thrown while starting the server! (Exception: {ex})");
                    textBoxHTTP.Invoke(new Action(() =>
                    {
                        textBoxHTTP.Text = "Stopped";
                    }));
                    groupBoxHTTP.Invoke(new Action(() =>
                    {
                        groupBoxHTTP.BackColor = Color.Red;
                    }));
                }
            });
        }

        private void buttonStartDNS_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                const string prefix = "MitmDNS";

                try
                {
                    _writersList[dnsCRC].Flush();
                    ProcessManager.StartupProgram(_writersList[dnsCRC], textBoxDNS, groupBoxDNS, prefix, textBoxDNSPath.Text, dnsCRC);
                    textBoxDNS.Invoke(new Action(() =>
                    {
                        textBoxDNS.Text = "Running";
                    }));
                    groupBoxDNS.Invoke(new Action(() =>
                    {
                        groupBoxDNS.BackColor = Color.Green;
                    }));
                    LoggerAccessor.LogInfo($"[{prefix}] - Server started at: {DateTime.Now}!");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[{prefix}] - An assertion was thrown while starting the server! (Exception: {ex})");
                    textBoxDNS.Invoke(new Action(() =>
                    {
                        textBoxDNS.Text = "Stopped";
                    }));
                    groupBoxDNS.Invoke(new Action(() =>
                    {
                        groupBoxDNS.BackColor = Color.Red;
                    }));
                }
            });
        }

        private void buttonStartHorizon_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                const string prefix = "Horizon";

                try
                {
                    _writersList[horizonCRC].Flush();
                    ProcessManager.StartupProgram(_writersList[horizonCRC], textBoxHorizon, groupBoxHorizon, prefix, textBoxHorizonPath.Text, horizonCRC);
                    textBoxHorizon.Invoke(new Action(() =>
                    {
                        textBoxHorizon.Text = "Running";
                    }));
                    groupBoxHorizon.Invoke(new Action(() =>
                    {
                        groupBoxHorizon.BackColor = Color.Green;
                    }));
                    LoggerAccessor.LogInfo($"[{prefix}] - Server started at: {DateTime.Now}!");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[{prefix}] - An assertion was thrown while starting the server! (Exception: {ex})");
                    textBoxHorizon.Invoke(new Action(() =>
                    {
                        textBoxHorizon.Text = "Stopped";
                    }));
                    groupBoxHorizon.Invoke(new Action(() =>
                    {
                        groupBoxHorizon.BackColor = Color.Red;
                    }));
                }
            });
        }

        private void buttonStartMultisocks_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                const string prefix = "MultiSocks";

                try
                {
                    _writersList[multisocksCRC].Flush();
                    ProcessManager.StartupProgram(_writersList[multisocksCRC], textBoxMultisocks, groupBoxMultisocks, prefix, textBoxMultisocksPath.Text, multisocksCRC);
                    textBoxMultisocks.Invoke(new Action(() =>
                    {
                        textBoxMultisocks.Text = "Running";
                    }));
                    groupBoxMultisocks.Invoke(new Action(() =>
                    {
                        groupBoxMultisocks.BackColor = Color.Green;
                    }));
                    LoggerAccessor.LogInfo($"[{prefix}] - Server started at: {DateTime.Now}!");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[{prefix}] - An assertion was thrown while starting the server! (Exception: {ex})");
                    textBoxMultisocks.Invoke(new Action(() =>
                    {
                        textBoxMultisocks.Text = "Stopped";
                    }));
                    groupBoxMultisocks.Invoke(new Action(() =>
                    {
                        groupBoxMultisocks.BackColor = Color.Red;
                    }));
                }
            });
        }

        private void buttonStartMultispy_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                const string prefix = "MultiSpy";

                try
                {
                    _writersList[multispyCRC].Flush();
                    ProcessManager.StartupProgram(_writersList[multispyCRC], textBoxMultispy, groupBoxMultispy, prefix, textBoxMultispyPath.Text, multispyCRC);
                    textBoxMultispy.Invoke(new Action(() =>
                    {
                        textBoxMultispy.Text = "Running";
                    }));
                    groupBoxMultispy.Invoke(new Action(() =>
                    {
                        groupBoxMultispy.BackColor = Color.Green;
                    }));
                    LoggerAccessor.LogInfo($"[{prefix}] - Server started at: {DateTime.Now}!");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[{prefix}] - An assertion was thrown while starting the server! (Exception: {ex})");
                    textBoxMultispy.Invoke(new Action(() =>
                    {
                        textBoxMultispy.Text = "Stopped";
                    }));
                    groupBoxMultispy.Invoke(new Action(() =>
                    {
                        groupBoxMultispy.BackColor = Color.Red;
                    }));
                }
            });
        }

        private void buttonStartQuazalserver_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                const string prefix = "QuazalServer";

                try
                {
                    _writersList[quazalCRC].Flush();
                    ProcessManager.StartupProgram(_writersList[quazalCRC], textBoxQuazalserver, groupBoxQuazalserver, prefix, textBoxQuazalserverPath.Text, quazalCRC);
                    textBoxQuazalserver.Invoke(new Action(() =>
                    {
                        textBoxQuazalserver.Text = "Running";
                    }));
                    groupBoxQuazalserver.Invoke(new Action(() =>
                    {
                        groupBoxQuazalserver.BackColor = Color.Green;
                    }));
                    LoggerAccessor.LogInfo($"[{prefix}] - Server started at: {DateTime.Now}!");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[{prefix}] - An assertion was thrown while starting the server! (Exception: {ex})");
                    textBoxQuazalserver.Invoke(new Action(() =>
                    {
                        textBoxQuazalserver.Text = "Stopped";
                    }));
                    groupBoxQuazalserver.Invoke(new Action(() =>
                    {
                        groupBoxQuazalserver.BackColor = Color.Red;
                    }));
                }
            });
        }

        private void buttonStartSSFWServer_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                const string prefix = "SSFWServer";

                try
                {
                    _writersList[ssfwCRC].Flush();
                    ProcessManager.StartupProgram(_writersList[ssfwCRC], textBoxSSFWServer, groupBoxSSFWServer, prefix, textBoxSSFWServerPath.Text, ssfwCRC);
                    textBoxSSFWServer.Invoke(new Action(() =>
                    {
                        textBoxSSFWServer.Text = "Running";
                    }));
                    groupBoxSSFWServer.Invoke(new Action(() =>
                    {
                        groupBoxSSFWServer.BackColor = Color.Green;
                    }));
                    LoggerAccessor.LogInfo($"[{prefix}] - Server started at: {DateTime.Now}!");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[{prefix}] - An assertion was thrown while starting the server! (Exception: {ex})");
                    textBoxSSFWServer.Invoke(new Action(() =>
                    {
                        textBoxSSFWServer.Text = "Stopped";
                    }));
                    groupBoxSSFWServer.Invoke(new Action(() =>
                    {
                        groupBoxSSFWServer.BackColor = Color.Red;
                    }));
                }
            });
        }

        private void buttonStartSVO_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                const string prefix = "SVO";

                try
                {
                    _writersList[svoCRC].Flush();
                    ProcessManager.StartupProgram(_writersList[svoCRC], textBoxSVO, groupBoxSVO, prefix, textBoxSVOPath.Text, svoCRC);
                    textBoxSVO.Invoke(new Action(() =>
                    {
                        textBoxSVO.Text = "Running";
                    }));
                    groupBoxSVO.Invoke(new Action(() =>
                    {
                        groupBoxSVO.BackColor = Color.Green;
                    }));
                    LoggerAccessor.LogInfo($"[{prefix}] - Server started at: {DateTime.Now}!");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[{prefix}] - An assertion was thrown while starting the server! (Exception: {ex})");
                    textBoxSVO.Invoke(new Action(() =>
                    {
                        textBoxSVO.Text = "Stopped";
                    }));
                    groupBoxSVO.Invoke(new Action(() =>
                    {
                        groupBoxSVO.BackColor = Color.Red;
                    }));
                }
            });
        }

        private void buttonStartEdenserver_Click(object sender, EventArgs e)
        {
            _ = Task.Run(() =>
            {
                const string prefix = "EdenServer";

                try
                {
                    _writersList[edenCRC].Flush();
                    ProcessManager.StartupProgram(_writersList[edenCRC], textBoxEdenserver, groupBoxEdenserver, prefix, textBoxEdenserverPath.Text, edenCRC);
                    textBoxEdenserver.Invoke(new Action(() =>
                    {
                        textBoxEdenserver.Text = "Running";
                    }));
                    groupBoxEdenserver.Invoke(new Action(() =>
                    {
                        groupBoxEdenserver.BackColor = Color.Green;
                    }));
                    LoggerAccessor.LogInfo($"[{prefix}] - Server started at: {DateTime.Now}!");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[{prefix}] - An assertion was thrown while starting the server! (Exception: {ex})");
                    textBoxEdenserver.Invoke(new Action(() =>
                    {
                        textBoxEdenserver.Text = "Stopped";
                    }));
                    groupBoxEdenserver.Invoke(new Action(() =>
                    {
                        groupBoxEdenserver.BackColor = Color.Red;
                    }));
                }
            });
        }

        private void buttonStopHTTP_Click(object sender, EventArgs e)
        {
            if (ProcessManager.ShutdownProcess(httpCRC))
            {
                textBoxHTTP.Invoke(new Action(() =>
                {
                    textBoxHTTP.Text = "Stopped";
                }));
                groupBoxHTTP.Invoke(new Action(() =>
                {
                    groupBoxHTTP.BackColor = Color.Red;
                }));
                LoggerAccessor.LogWarn($"[ApacheNet] - Server stopped at: {DateTime.Now}!");
            }
            else
                Utils.ShowNoProcessMessageBox();
        }

        private void buttonStopDNS_Click(object sender, EventArgs e)
        {
            if (ProcessManager.ShutdownProcess(dnsCRC))
            {
                textBoxDNS.Invoke(new Action(() =>
                {
                    textBoxDNS.Text = "Stopped";
                }));
                groupBoxDNS.Invoke(new Action(() =>
                {
                    groupBoxDNS.BackColor = Color.Red;
                }));
                LoggerAccessor.LogWarn($"[MitmDNS] - Server stopped at: {DateTime.Now}!");
            }
            else
                Utils.ShowNoProcessMessageBox();
        }

        private void buttonStopHorizon_Click(object sender, EventArgs e)
        {
            if (ProcessManager.ShutdownProcess(horizonCRC))
            {
                textBoxHorizon.Invoke(new Action(() =>
                {
                    textBoxHorizon.Text = "Stopped";
                }));
                groupBoxHorizon.Invoke(new Action(() =>
                {
                    groupBoxHorizon.BackColor = Color.Red;
                }));
                LoggerAccessor.LogWarn($"[Horizon] - Server stopped at: {DateTime.Now}!");
            }
            else
                Utils.ShowNoProcessMessageBox();
        }

        private void buttonStopMultisocks_Click(object sender, EventArgs e)
        {
            if (ProcessManager.ShutdownProcess(multisocksCRC))
            {
                textBoxMultisocks.Invoke(new Action(() =>
                {
                    textBoxMultisocks.Text = "Stopped";
                }));
                groupBoxMultisocks.Invoke(new Action(() =>
                {
                    groupBoxMultisocks.BackColor = Color.Red;
                }));
                LoggerAccessor.LogWarn($"[MultiSocks] - Server stopped at: {DateTime.Now}!");
            }
            else
                Utils.ShowNoProcessMessageBox();
        }

        private void buttonStopMultispy_Click(object sender, EventArgs e)
        {
            if (ProcessManager.ShutdownProcess(multispyCRC))
            {
                textBoxMultispy.Invoke(new Action(() =>
                {
                    textBoxMultispy.Text = "Stopped";
                }));
                groupBoxMultispy.Invoke(new Action(() =>
                {
                    groupBoxMultispy.BackColor = Color.Red;
                }));
                LoggerAccessor.LogWarn($"[MultiSpy] - Server stopped at: {DateTime.Now}!");
            }
            else
                Utils.ShowNoProcessMessageBox();
        }

        private void buttonStopQuazalserver_Click(object sender, EventArgs e)
        {
            if (ProcessManager.ShutdownProcess(quazalCRC))
            {
                textBoxQuazalserver.Invoke(new Action(() =>
                {
                    textBoxQuazalserver.Text = "Stopped";
                }));
                groupBoxQuazalserver.Invoke(new Action(() =>
                {
                    groupBoxQuazalserver.BackColor = Color.Red;
                }));
                LoggerAccessor.LogWarn($"[QuazalServer] - Server stopped at: {DateTime.Now}!");
            }
            else
                Utils.ShowNoProcessMessageBox();
        }

        private void buttonStopSSFWServer_Click(object sender, EventArgs e)
        {
            if (ProcessManager.ShutdownProcess(ssfwCRC))
            {
                textBoxSSFWServer.Invoke(new Action(() =>
                {
                    textBoxSSFWServer.Text = "Stopped";
                }));
                groupBoxSSFWServer.Invoke(new Action(() =>
                {
                    groupBoxSSFWServer.BackColor = Color.Red;
                }));
                LoggerAccessor.LogWarn($"[SSFWServer] - Server stopped at: {DateTime.Now}!");
            }
            else
                Utils.ShowNoProcessMessageBox();
        }

        private void buttonStopSVO_Click(object sender, EventArgs e)
        {
            if (ProcessManager.ShutdownProcess(svoCRC))
            {
                textBoxSVO.Invoke(new Action(() =>
                {
                    textBoxSVO.Text = "Stopped";
                }));
                groupBoxSVO.Invoke(new Action(() =>
                {
                    groupBoxSVO.BackColor = Color.Red;
                }));
                LoggerAccessor.LogWarn($"[SVO] - Server stopped at: {DateTime.Now}!");
            }
            else
                Utils.ShowNoProcessMessageBox();
        }

        private void buttonStopEdenserver_Click(object sender, EventArgs e)
        {
            if (ProcessManager.ShutdownProcess(edenCRC))
            {
                textBoxEdenserver.Invoke(new Action(() =>
                {
                    textBoxEdenserver.Text = "Stopped";
                }));
                groupBoxEdenserver.Invoke(new Action(() =>
                {
                    groupBoxEdenserver.BackColor = Color.Red;
                }));
                LoggerAccessor.LogWarn($"[EdenServer] - Server stopped at: {DateTime.Now}!");
            }
            else
                Utils.ShowNoProcessMessageBox();
        }

        private void linkLabelGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string url;
            if (e.Link.LinkData != null)
                url = e.Link.LinkData.ToString();
            else
                url = linkLabelGithub.Text.Substring(e.Link.Start, e.Link.Length);

            if (!string.IsNullOrEmpty(url))
            {
                if (!url.Contains("://"))
                    url = "https://" + url;

                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                linkLabelGithub.LinkVisited = true;
            }
        }

        private void buttonConfigureApacheNet_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxApacheNetJsonPath.Text + "\"");
        }

        private void buttonConfigureDNS_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxDNSJsonPath.Text + "\"");
        }

        private void buttonConfigureHorizon_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxHorizonJsonPath.Text + "\"");
        }

        private void buttonConfigureMultisocks_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxMultisocksJsonPath.Text + "\"");
        }

        private void buttonConfigureMultispy_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxMultispyJsonPath.Text + "\"");
        }

        private void buttonConfigureQuazalserver_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxQuazalserverJsonPath.Text + "\"");
        }

        private void buttonConfigureSSFWServer_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxSSFWServerJsonPath.Text + "\"");
        }

        private void buttonConfigureSVO_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxSVOJsonPath.Text + "\"");
        }

        private void buttonConfigureEdenserver_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxEdenserverJsonPath.Text + "\"");
        }

        private void buttonConfigureNat_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxNatJsonPath.Text + "\"");
        }

        private void buttonConfigureBwps_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxBwpsJsonPath.Text + "\"");
        }

        private void buttonConfigureMedius_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxMediusJsonPath.Text + "\"");
        }

        private void buttonConfigureDME_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxDMEJsonPath.Text + "\"");
        }

        private void buttonConfigureMUIS_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxMUISJsonPath.Text + "\"");
        }

        private void buttonConfigureEbootDefs_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxEbootDefsJsonPath.Text + "\"");
        }

        private void buttonConfigureHorizonDatabase_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxHorizonDatabaseJsonPath.Text + "\"");
        }

        private void buttonConfigureAriesDatabase_Click(object sender, EventArgs e)
        {
            Process.Start(jsonEditorPath, "\"" + textBoxAriesDatabaseJsonPath.Text + "\"");
        }
    }
}
