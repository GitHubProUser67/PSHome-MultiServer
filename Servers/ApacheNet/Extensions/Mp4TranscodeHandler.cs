using CustomLogger;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using SpaceWizards.HttpListener;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WatsonWebserver.Core;

namespace ApacheNet.Extensions
{
    public class MP4TranscodeHandler
    {
        private static int _httpPort = 8081;
        private readonly string filePath;
        private readonly string convertersPath;
        private Thread? _thread = null;
        private readonly ManualResetEvent _waitFFMpeg = new(false);
        private readonly ManualResetEvent _waitCompletation = new(false);
        private readonly ManualResetEvent _waitPort = new(false);
        private readonly ManualResetEvent _stopServer = new(false);

        public (HttpContextBase, Process)? HandlersCache = null;

        public MP4TranscodeHandler(string filePath, string convertersPath)
        {
            this.filePath = filePath;
            this.convertersPath = convertersPath;
        }

        public async Task<bool> ProcessVideoTranscode(HttpContextBase context)
        {
            StartServer();
            if (!TCPUtils.IsTCPPortAvailable(_httpPort))
                StartFFMpeg(context);

            _waitFFMpeg.WaitOne(6000); // We wait, but not more than 6000 if other process failed.

            if (HandlersCache != null)
            {
                _waitCompletation.WaitOne();

                RemoveCacheEntry();

                return true;
            }

            context.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            context.Response.ContentType = "text/plain";
            return await context.Response.Send("Transcoding system failed to start the stream, please contact server administrator!");
        }

        private void RemoveCacheEntry()
        {
            if (HandlersCache != null)
            {
                HandlersCache.Value.Item2.Kill();
                HandlersCache.Value.Item2.Dispose();
                HandlersCache = null;
            }
        }

        private void StartFFMpeg(HttpContextBase context)
        {
            ThreadPool.QueueUserWorkItem(delegate (object? ctx)
            {
                _waitPort.WaitOne();

                try
                {
                    HttpContextBase? httpContext = (HttpContextBase?)ctx;
                    if (httpContext != null)
                    {
                        RemoveCacheEntry();

                        bool isNvenc = CheckForNvencGpu();
                        bool ismp4Source = HTTPProcessor.GetMimeType(Path.GetExtension(filePath)) == "video/mp4";
                        string bitrate = httpContext.Request.RetrieveQueryValue("vbitrate");
                        string offset = httpContext.Request.RetrieveQueryValue("offset");

                        if (string.IsNullOrEmpty(offset))
                            offset = "00:00:00";
                        else
                            offset = GetFormatedOffset(Convert.ToDouble(offset, System.Globalization.CultureInfo.InvariantCulture));

                        _ = bool.TryParse(httpContext.Request.RetrieveQueryValue("vtranscode"), out bool needToTranscode);

                        Process proc = new();

                        HandlersCache = (context, proc);

                        proc.StartInfo = new ProcessStartInfo($"{convertersPath}/ffmpeg",
                            !string.IsNullOrEmpty(bitrate) && bitrate != "NaN" ? string.Format(@"{6}-i ""{0}"" -ss {1} -b:v {4} -r {5} {2} http://localhost:{3}/", filePath,
                            offset, GetBrowserSupportedFFMpegFormat(ismp4Source, needToTranscode, isNvenc), _httpPort, bitrate, httpContext.Request.RetrieveQueryValue("vframerate"), isNvenc ? "-hwaccel cuda -hwaccel_output_format cuda " : string.Empty) :
                            string.Format(@"{5}-i ""{0}"" -ss {1} -r {4} {2} http://localhost:{3}/", filePath, offset, GetBrowserSupportedFFMpegFormat(ismp4Source, needToTranscode, isNvenc), _httpPort,
                            httpContext.Request.RetrieveQueryValue("vframerate"), isNvenc ? "-hwaccel cuda -hwaccel_output_format cuda " : string.Empty))
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };

                        proc.Start();
                        proc.PriorityClass = ProcessPriorityClass.High;

                        LoggerAccessor.LogWarn($"[Mp4TranscodeHandler] - Started FFMpeg stream for client: {context.Request.Source.IpAddress}:{context.Request.Source.Port} at offset:{offset}");
                    }
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[Mp4TranscodeHandler] - FFMpeg stream startup requested by client: {context.Request.Source.IpAddress}:{context.Request.Source.Port} thrown an exception: {e}");
                }

                _waitFFMpeg.Set();
            }, context);
        }

        private void StartServer()
        {
            _thread ??= new Thread(() =>
                {
                    _stopServer.Reset();

                    using HttpListener listener = new();
                    try
                    {
                        _httpPort = TCPUtils.GetNextVacantTCPPort(_httpPort, 10);

                        if (_httpPort == -1)
                            return;

                        listener.Prefixes.Add($"http://*:{_httpPort}/");

                        listener.IgnoreWriteExceptions = true;
                        listener.Start();
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError("[WebmStreamHandler] - An Exception Occured while starting the temporary http server: " + ex.Message);
                        return;
                    }

                    List<WaitHandle> handles = new() { _stopServer };

                    while (listener != null && listener.IsListening)
                    {
                        try
                        {
                            // Create a new user connection using TcpClient returned by
                            IAsyncResult result = listener.BeginGetContext(DoAcceptTcpClientCallback, listener);

                            _waitPort.Set();

                            handles.Add(result.AsyncWaitHandle);
                            WaitHandle.WaitAny(handles.ToArray());
                            handles.Remove(result.AsyncWaitHandle);
                            result.AsyncWaitHandle.Close();

                            if (_stopServer.WaitOne(0, true))
                            {
                                listener.Stop();
                                return;
                            }
                        }
                        catch
                        {
                            listener?.Stop();
                        }
                    }
                });
            if (!_thread.IsAlive)
                _thread.Start();
        }

        private void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            HttpListener? listener = (HttpListener?)ar.AsyncState;
            if (listener != null)
            {
                try
                {
                    HttpListenerContext client = listener.EndGetContext(ar);

                    if (HandlersCache == null)
                        return;

                    bool endOfInput = false;
                    int bytesRead = 0;
                    byte[] buffer = new byte[8192];

                    HandlersCache.Value.Item1.Response.ChunkedTransfer = true;
                    HandlersCache.Value.Item1.Response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                    HandlersCache.Value.Item1.Response.ContentType = "video/mp4";

                    while (!endOfInput)
                    {
                        bytesRead = client.Request.InputStream.Read(buffer, 0, buffer.Length);
                        if (bytesRead < buffer.Length)
                            endOfInput = true; // We've reached the end of input stream

                        if (bytesRead > 0)
                        {
                            byte[] output = new byte[bytesRead];

                            Buffer.BlockCopy(buffer, 0, output, 0, bytesRead);

                            if (endOfInput)
                            {
                                HandlersCache.Value.Item1.Response.SendChunk(output, true).Wait();
                                break;
                            }
                            else if (!HandlersCache.Value.Item1.Response.SendChunk(output, false).Result)
                                break;
                        }
                    }

                    LoggerAccessor.LogWarn($"[Mp4TranscodeHandler] - Stopped FFMpeg stream for client: {HandlersCache.Value.Item1.Request.Source.IpAddress}:{HandlersCache.Value.Item1.Request.Source.Port}");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[Mp4TranscodeHandler] - DoAcceptTcpClientCallback thrown an exception: {ex}");
                }
            }

            _waitCompletation.Set();
        }

        private static string GetBrowserSupportedFFMpegFormat(bool ismp4Source, bool needToTranscode, bool isNvidia)
        {
            if (!ismp4Source) // We prefer the safer approach, and assume other-than-mp4 data is not compatible with a straight copy.
                needToTranscode = true;

            if (needToTranscode)
            {
                if (isNvidia)
                    return $"-c:v h264_nvenc -preset fast -acodec aac -strict -2 -b:a 192k -threads {Environment.ProcessorCount} -movflags frag_keyframe -f mp4";
                else
                    return $"-vcodec libx264 -preset ultrafast -acodec aac -strict -2 -b:a 192k -threads {Environment.ProcessorCount} -movflags frag_keyframe -f mp4";
            }

            return "-vcodec copy -preset ultrafast -acodec aac -strict -2 -b:a 192k -movflags frag_keyframe -f mp4";
        }

        private static string GetFormatedOffset(double offset)
        {
            int hours = (int)Math.Floor(offset / 3600);
            int minutes = (int)Math.Floor((offset - hours * 3600) / 60);
            return hours + ":" + minutes + ":" + (int)Math.Floor(offset - hours * 3600 - minutes * 60);
        }

        private static bool CheckForNvencGpu()
        {
            try
            {
                // Check if "nvidia-smi" is available and can detect a GPU
                using (Process proc = new Process())
                {
                    proc.StartInfo.FileName = "nvidia-smi";
                    proc.StartInfo.Arguments = "-L"; // List GPUs
                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.CreateNoWindow = true;

                    proc.Start();

                    string output = proc.StandardOutput.ReadToEnd();

                    proc.WaitForExit();

                    if (output.Contains("GPU"))
                    {
                        const string pattern = @"CUDA Version:\s*([\d.]+)";

                        proc.StartInfo.Arguments = string.Empty;

                        proc.Start();

                        output = proc.StandardOutput.ReadToEnd();

                        proc.WaitForExit();

                        Match match = Regex.Match(output, pattern);

                        if (match.Success)
                        {
                            const string minRequiredVersionStr = "9.0";

                            if (CompareVersions(match.Groups[1].Value, minRequiredVersionStr) > 0)
                                // If output contains GPU information and we have a valid CUDA revision, we can use NVENC
                                return true;
                        }
                    }
                }
            }
            catch
            {
            }

            // If "nvidia-smi" isn't found or fails, assume no NVENC
            return false;
        }

        private static int CompareVersions(string version1, string version2)
        {
            Version v1 = Version.Parse(version1);
            Version v2 = Version.Parse(version2);

            return v1.CompareTo(v2);
        }
    }
}
