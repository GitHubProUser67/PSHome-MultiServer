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
    public class WebmTranscodeHandler
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

        public WebmTranscodeHandler(string filePath, string convertersPath)
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

                        bool isNvenc = CheckForAdaLovelaceGpu();
                        bool isWebmSource = HTTPProcessor.GetMimeType(Path.GetExtension(filePath)) == "video/webm";
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
                            offset, GetBrowserSupportedFFMpegFormat(httpContext.Request.RetrieveQueryValue("format"), isWebmSource, needToTranscode, isNvenc), _httpPort, bitrate, httpContext.Request.RetrieveQueryValue("vframerate"), isNvenc ? "-hwaccel cuda -hwaccel_output_format cuda " : string.Empty) :
                            string.Format(@"{5}-i ""{0}"" -ss {1} -r {4} {2} http://localhost:{3}/", filePath, offset, GetBrowserSupportedFFMpegFormat(httpContext.Request.RetrieveQueryValue("format"), isWebmSource, needToTranscode, isNvenc), _httpPort,
                            httpContext.Request.RetrieveQueryValue("vframerate"), isNvenc ? "-hwaccel cuda -hwaccel_output_format cuda " : string.Empty))
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true
                        };

                        proc.Start();
                        proc.PriorityClass = ProcessPriorityClass.High;

                        LoggerAccessor.LogWarn($"[WebmTranscodeHandler] - Started FFMpeg stream for client: {context.Request.Source.IpAddress}:{context.Request.Source.Port} at offset:{offset}");
                    }
                }
                catch (Exception e)
                {
                    LoggerAccessor.LogError($"[WebmTranscodeHandler] - FFMpeg stream startup requested by client: {context.Request.Source.IpAddress}:{context.Request.Source.Port} thrown an exception: {e}");
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
                    HandlersCache.Value.Item1.Response.ContentType = "video/webm";

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

                    LoggerAccessor.LogWarn($"[WebmTranscodeHandler] - Stopped FFMpeg stream for client: {HandlersCache.Value.Item1.Request.Source.IpAddress}:{HandlersCache.Value.Item1.Request.Source.Port}");
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[WebmTranscodeHandler] - DoAcceptTcpClientCallback thrown an exception: {ex}");
                }
            }

            _waitCompletation.Set();
        }

        private static string GetBrowserSupportedFFMpegFormat(string format, bool isWebmSource, bool needToTranscode, bool isNvidia)
        {
            format = string.IsNullOrEmpty(format) ? "webm" : format;

            if (format == "webm" && !isWebmSource) // Webm is picky on ffmpeg, it needs transcoding for non-webm source.
                needToTranscode = true;

            if (needToTranscode)
            {
                if (isNvidia)
                    return "-c:v av1_nvenc -preset p7 -tune hq -cq 18 -acodec libvorbis -b:a 192k -f webm";
                else
                    return "-vcodec libvpx -acodec libvorbis -b:a 192k -deadline realtime -speed 8 -f webm";
            }

            return "-vcodec copy -preset ultrafast -acodec libvorbis -b:a 192k -movflags frag_keyframe -f " + format;
        }

        private static string GetFormatedOffset(double offset)
        {
            int hours = (int)Math.Floor(offset / 3600);
            int minutes = (int)Math.Floor((offset - hours * 3600) / 60);
            return hours + ":" + minutes + ":" + (int)Math.Floor(offset - hours * 3600 - minutes * 60);
        }

        private static bool CheckForAdaLovelaceGpu()
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

                    // Check each GPU in the output
                    const string pattern = @"GPU \d+: NVIDIA GeForce RTX (\d+)(\S*)";

                    foreach (Match match in Regex.Matches(output, pattern))
                    {
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int seriesNumber))
                        {
                            // RTX 40 series or higher
                            if (seriesNumber >= 4000)
                                return true;
                        }
                    }
                }
            }
            catch
            {
            }

            // If "nvidia-smi" isn't found or no RTX 40+ match, assume no Ada GPU.
            return false;
        }
    }
}
