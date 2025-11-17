using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MultiServerLibrary.Extension;
using WatsonWebserver.Core;

namespace ApacheNet.BuildIn.Extensions
{
    public class PHP
    {
        private bool ThreadsActive = true;
        private bool OutputStreamHooked = false;

        private Process TheProcess = new();

        private Thread? RunProcessThread;
        private Thread? ReadOutputThread;
        private Thread? ReadErrorOutputThread;

        private (int, byte[], Dictionary<string, string>) StandardOutput;

        private byte[]? ErrorOutput;
        private byte[]? PostData = null;

        public (int, byte[]?, Dictionary<string, string>) ProcessPHPPage(string FilePath, string PHPPath, string PHPVer, HttpContextBase ctx, bool secure)
        {
            int index = ctx.Request.Url.RawWithQuery.IndexOf("?");
            string? queryString = index == -1 ? string.Empty : ctx.Request.Url.RawWithQuery[(index + 1)..];

            // Get paths for PHP
            string? documentRootPath = Path.GetDirectoryName(FilePath);
            string? scriptFilePath = Path.GetFullPath(FilePath);
            string? scriptFileName = Path.GetFileName(FilePath);
            string phpFullPath = $"{PHPPath}/{PHPVer}/";

            PostData = ctx.Request.DataAsBytes;

            TheProcess.StartInfo.FileName = $"{phpFullPath}php-cgi";

            TheProcess.StartInfo.Arguments = $"-q -c \"{$"{phpFullPath}php.ini"}\" -d \"error_reporting=E_ALL\" -d \"display_errors={ApacheNetServerConfiguration.PHPDebugErrors}\" -d \"expose_php=Off\" -d \"include_path='{documentRootPath}'\" \"{FilePath}\"";

            TheProcess.StartInfo.CreateNoWindow = true;
            TheProcess.StartInfo.UseShellExecute = false;
            TheProcess.StartInfo.RedirectStandardOutput = true;
            TheProcess.StartInfo.RedirectStandardError = true;
            TheProcess.StartInfo.RedirectStandardInput = true;

            TheProcess.StartInfo.EnvironmentVariables.Clear();

            // Set content length if needed
            if (PostData != null)
                TheProcess.StartInfo.EnvironmentVariables.Add("CONTENT_LENGTH", PostData.Length.ToString());

            // Set environment variables for PHP
            TheProcess.StartInfo.EnvironmentVariables["SYSTEMROOT"] = Environment.GetEnvironmentVariable("SYSTEMROOT");
            TheProcess.StartInfo.EnvironmentVariables["WINDIR"] = Environment.GetEnvironmentVariable("WINDIR");
            TheProcess.StartInfo.EnvironmentVariables["COMSPEC"] = Environment.GetEnvironmentVariable("COMSPEC");
            TheProcess.StartInfo.EnvironmentVariables["TMPDIR"] = Environment.GetEnvironmentVariable("TMPDIR") ?? Path.GetTempPath();
            TheProcess.StartInfo.EnvironmentVariables["TEMP"] = Environment.GetEnvironmentVariable("TEMP") ?? Path.GetTempPath();
            TheProcess.StartInfo.EnvironmentVariables["PATH"] = Environment.GetEnvironmentVariable("PATH");
            TheProcess.StartInfo.EnvironmentVariables.Add("GATEWAY_INTERFACE", "CGI/1.1");
            TheProcess.StartInfo.EnvironmentVariables.Add("SERVER_PROTOCOL", $"HTTP/{ApacheNetServerConfiguration.HttpVersion}");
            TheProcess.StartInfo.EnvironmentVariables.Add("REDIRECT_STATUS", "200");
            TheProcess.StartInfo.EnvironmentVariables.Add("DOCUMENT_ROOT", documentRootPath);
            TheProcess.StartInfo.EnvironmentVariables.Add("SCRIPT_NAME", scriptFileName);
            TheProcess.StartInfo.EnvironmentVariables.Add("SCRIPT_FILENAME", scriptFilePath);
            TheProcess.StartInfo.EnvironmentVariables.Add("QUERY_STRING", queryString);
            TheProcess.StartInfo.EnvironmentVariables.Add("CONTENT_TYPE", ctx.Request.ContentType);
            TheProcess.StartInfo.EnvironmentVariables.Add("REQUEST_METHOD", ctx.Request.Method.ToString());
            TheProcess.StartInfo.EnvironmentVariables.Add("USER_AGENT", ctx.Request.Useragent);
            TheProcess.StartInfo.EnvironmentVariables.Add("SERVER_ADDR", ctx.Request.Destination.IpAddress);
            TheProcess.StartInfo.EnvironmentVariables.Add("REMOTE_ADDR", ctx.Request.Source.IpAddress);
            TheProcess.StartInfo.EnvironmentVariables.Add("REMOTE_PORT", ctx.Request.Source.Port.ToString());
            TheProcess.StartInfo.EnvironmentVariables.Add("REQUEST_URI", $"{(secure ? "https" : "http")}://{ctx.Request.Destination.IpAddress}:{ctx.Request.Destination.Port}{ctx.Request.Url.RawWithQuery}");
            foreach (var headerKeyPair in ctx.Request.Headers.ConvertHeadersToPhpFriendly())
            {
                string? key = headerKeyPair.Key;
                string? value = headerKeyPair.Value;

                if (!string.IsNullOrEmpty(key) && value != null && IsValidEnvVarKey(key))
                    TheProcess.StartInfo.EnvironmentVariables.Add(key, value);
            }

            ReadOutputThread = new Thread(new ThreadStart(ReadOutput));
            ReadErrorOutputThread = new Thread(new ThreadStart(ReadErrorOutput));
            RunProcessThread = new Thread(new ThreadStart(RunProcess));

            ReadOutputThread.Start();
            ReadErrorOutputThread.Start();
            RunProcessThread.Start();

            RunProcessThread.Join();

            ThreadsActive = false;

            ReadOutputThread.Join();
            ReadErrorOutputThread.Join();

            if (ApacheNetServerConfiguration.PHPDebugErrors && ErrorOutput != null && ErrorOutput.Length > 0)
                return (StandardOutput.Item1, ErrorOutput, StandardOutput.Item3);
            return StandardOutput;
        }

        private void RunProcess()
        {
            try
            {
                TheProcess.Start();

                // Calculate approximately end time
                DateTime EndTime = DateTime.Now.AddSeconds(5);

                new Task(() =>
                {
                    while (DateTime.Now < EndTime) { Thread.Sleep(1000); }
                    float PhpCpuLoad = 0;
                    try
                    {
                        while (TheProcess != null && !TheProcess.HasExited)
                        {
                            Thread.Sleep(1000);
                            ProcessUtils.PreventProcessIdle(ref TheProcess, ref PhpCpuLoad);
                        }
                    }
                    catch
                    {
                        // No process remaining.
                    }
                }).Start();

                if (PostData != null)
                {
                    // Write request body to standard input
                    using StreamWriter? sw = TheProcess.StandardInput;
                    sw.BaseStream.Write(PostData, 0, PostData.Length);
                }

                TheProcess.WaitForExit();
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogWarn($"[PHP] - Killing bad process. (Exception:{ex})");

                try
                {
                    if (TheProcess != null && !TheProcess.HasExited)
                    {
                        TheProcess.Kill();
                        TheProcess.WaitForExit();
                    }
                }
                catch (Exception innerEx)
                {
                    CustomLogger.LoggerAccessor.LogError($"[PHP] - Failed to kill process. (Exception:{innerEx})");
                }
            }
        }

        private void ReadOutput()
        {
            bool HeadersEnd = false;
            int statusCode = 200;
            List<byte> lineBuffer = new List<byte>();
            Dictionary<string, string> headers = new Dictionary<string, string>();

            using (MemoryStream ms = new())
            using (MemoryStream headerBuffer = new(1))
            {
                Stream? stream = null;

                while (ThreadsActive)
                {
                    if (RunProcessThread!.IsAlive)
                    {
                        try
                        {
                            if (!OutputStreamHooked)
                            {
                                stream = TheProcess.StandardOutput.BaseStream;
                                OutputStreamHooked = true;
                            }

                            if (stream != null)
                            {
                                if (!HeadersEnd)
                                {
                                    headerBuffer.Position = 0;

                                    StreamUtils.CopyStream(stream, headerBuffer, 1, 1);

                                    byte b = headerBuffer.ToArray()[0];
                                    lineBuffer.Add(b);

                                    if (b == '\n') // end of line
                                    {
                                        string line = Encoding.ASCII.GetString(lineBuffer.ToArray()).TrimEnd('\r', '\n');
                                        lineBuffer.Clear();

                                        if (line != string.Empty)
                                        {
                                            int index = line.IndexOf(':');
                                            if (index != -1)
                                            {
                                                string headerKey = line.Substring(0, index);
                                                string headerValue = line.Substring(index + 1).Trim();
                                                if (headerKey == "Status") // Extract the PHP result status code.
                                                {
                                                    var parts = headerValue.Split(' ');
                                                    if (int.TryParse(parts[0], out statusCode))
                                                    {
#if DEBUG
                                                        CustomLogger.LoggerAccessor.LogInfo($"[PHP] - Custom status-code: {statusCode} assigned to result.");
#endif
                                                    }
                                                    else
                                                        CustomLogger.LoggerAccessor.LogWarn($"[PHP] - Unknown status-code format, falling back to OK response.");
                                                }
                                                else
                                                    headers[headerKey] = headerValue;
                                            }
                                        }
                                        else
                                            HeadersEnd = true;
                                    }

                                    continue;
                                }

                                StreamUtils.CopyStream(stream, ms);
                            }
                        }
                        catch
                        {
                            continue;
                        }
                        break;
                    }
                }

                stream?.Dispose();

                StandardOutput = (statusCode, ms.ToArray(), headers);
            }
        }

        private void ReadErrorOutput()
        {
            using (MemoryStream ms = new())
            using (StreamWriter output = new(ms))
            {
                while (ThreadsActive)
                {
                    if (RunProcessThread!.IsAlive)
                    {
                        try
                        {
                            string? line = TheProcess.StandardError.ReadLine();
                            if (line != null)
                            {
                                output.WriteLine(line);
                                continue;
                            }
                        }
                        catch
                        {
                            continue;
                        }
                        break;
                    }
                }

                output.Flush();

                ErrorOutput = ms.ToArray();
            }
        }

        private static bool IsValidEnvVarKey(string key)
        {
            // Environment variable keys usually can't have '=', '\0', or other special characters
            return key.All(c => c > 31 && c != '=' && c != '\0');
        }
    }
}
