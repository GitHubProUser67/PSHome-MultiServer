using System;
using System.IO;
using System.Collections.Generic;

using CustomLogger;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using MultiServerLibrary.Extension;

namespace WebAPIService.WebServices.MultiMedia
{
    /// <summary>
	/// Web video dowloader & converter
    /// (Windows only)
	/// </summary>
    public class WebVideoConverter
    {
        /// <summary>
		/// Download and convert an online video (from hostings like YouTube, VK, etc)
		/// </summary>
		public static WebVideo ConvertVideo(Dictionary<string, string> Arguments, string ConverterDir)
        {
            WebVideo video = new WebVideo();
            try
            {
                bool UseFFmpeg = true;
                bool GetYoutubeJson = false;
                string YoutubeDlArgs = string.Empty;
                string FFmpegArgs = string.Empty;

                // Check options
                if (!Arguments.TryGetValue("url", out string url))
                    throw new InvalidOperationException("Internet video address is missing.");

                // Configure output file type
                string PreferredMIME = "application/octet-stream", PreferredName = "video.avi";
                if (Arguments.TryGetValue("f", out string argument)) // (ffmpeg output format)
                {
                    switch (argument)
                    {
                        case "avi":
                            PreferredMIME = "video/msvideo";
                            PreferredName = "onlinevideo.avi";
                            break;
                        case "mpeg1video":
                        case "mpeg2video":
                            PreferredMIME = "video/mpeg";
                            PreferredName = "onlinevideo.mpg";
                            break;
                        case "mpeg4":
                            PreferredMIME = "video/mp4";
                            PreferredName = "onlinevideo.mp4";
                            break;
                        case "mpegts":
                            PreferredMIME = "video/mp2t";
                            PreferredName = "onlinevideo.mts";
                            break;
                        case "asf":
                        case "asf_stream":
                        case "wmv":
                            PreferredMIME = "video/x-ms-asf";
                            PreferredName = "onlinevideo.asf";
                            break;
                        case "mov":
                            PreferredMIME = "video/qucktime";
                            PreferredName = "onlinevideo.mov";
                            break;
                        case "ogg":
                            PreferredMIME = "video/ogg";
                            PreferredName = "onlinevideo.ogg";
                            break;
                        case "webm":
                            PreferredMIME = "video/webm";
                            PreferredName = "onlinevideo.webm";
                            break;
                        case "swf":
                            PreferredMIME = "application/x-shockwave-flash";
                            PreferredName = "onlinevideo.swf";
                            break;
                        case "rm":
                            PreferredMIME = "application/vnd.rn-realmedia";
                            PreferredName = "onlinevideo.rm";
                            break;
                        case "3gp":
                            PreferredMIME = "video/3gpp";
                            PreferredName = "onlinevideo.3gp";
                            break;
                        default:
                            PreferredMIME = "application/octet-stream";
                            PreferredName = "onlinevideo." + argument;
                            break;
                    }
                }
                if (Arguments.ContainsKey("j") ||
                   Arguments.ContainsKey("J") ||
                   Arguments.ContainsKey("dump-json") ||
                   Arguments.ContainsKey("dump-single-json") ||
                   Arguments.ContainsKey("print-json"))
                {
                    PreferredMIME = "application/json";
                    PreferredName = "metadata.json";
                }

                // Set output file type over auto-detected (if need)
#if NET5_0_OR_GREATER
                Arguments.TryAdd("content-type", PreferredMIME);
                Arguments.TryAdd("filename", PreferredName);
#else
                if (!Arguments.ContainsKey("content-type"))
                    Arguments.Add("content-type", PreferredMIME);
                if (!Arguments.ContainsKey("filename"))
                    Arguments.Add("filename", PreferredName);
#endif
                // Load all parameters
                foreach (KeyValuePair<string, string> Arg in Arguments)
                {
                    if (Arg.Key.StartsWith("vf") && Arguments["vcodec"] != "copy" ||
                       Arg.Key.StartsWith("af") && Arguments["acodec"] != "copy")
                    {
                        // Don't apply filters if codec is original
                        FFmpegArgs += string.Format(" -{0} {1}", Arg.Key, Arg.Value);
                        continue;
                    }
                    if (Arg.Key.StartsWith("filter"))
                    {
                        /* Currently may cause FFMPEG errors if combined with `-vcodec copy`:
						 * Filtergraph 'scale=480:-1' was defined for video output stream 0:0 but codec copy was selected.
						 * Filtering and streamcopy cannot be used together.
						 */
                        FFmpegArgs += string.Format(" -{0} {1}", Arg.Key, Arg.Value);
                        continue;
                    }
                    switch (Arg.Key.ToLowerInvariant())
                    {
                        case "enable":
                            if (!ToBoolean(Arg.Value)) throw new Exception("This feature is disabled by administrator.");
                            continue;
                        case "url":
                        case "content-type":
                        case "filename":
                        case "prefer":
                        case "gui":
                        case "youtubedlapp":
                        case "ffmpegapp":
                            continue;
                        case "noffmpeg":
                            UseFFmpeg = !ToBoolean(Arg.Value);
                            continue;
                        case "abort-on-error":
                        case "ignore-config":
                        case "mark-watched":
                        case "no-mark-watched":
                        case "proxy":
                        case "socket-timeout":
                        case "source-address":
                        case "4":
                        case "6":
                        case "geo-verification-proxy":
                        case "geo-bypass":
                        case "no-geo-bypass":
                        case "geo-bypass-country":
                        case "geo-bypass-ip-block":
                        case "include-ads":
                        case "limit-rate":
                        case "retries":
                        case "fragment-retries":
                        case "skip-unavailable-fragments":
                        case "abort-on-unavailable-fragment":
                        case "keep-fragments":
                        case "buffer-size":
                        case "no-resize-buffer":
                        case "http-chunk-size":
                        case "xattr-set-filesize ":
                        case "hls-prefer-native":
                        case "hls-prefer-ffmpeg":
                        case "hls-use-mpegts":
                        case "external-downloader":
                        case "external-downloader-args":
                        case "cookies":
                        case "no-cache-dir":
                        case "newline":
                        case "no-progress":
                        case "no-check-certificate":
                        case "prefer-insecure":
                        case "user-agent":
                        case "referer":
                        case "add-header":
                        case "bidi-workaround":
                        case "sleep-interval":
                        case "max-sleep-interval":
                        case "format":
                        case "youtube-skip-dash-manifest":
                        case "merge-output-format":
                        case "username":
                        case "password":
                        case "twofactor":
                        case "video-password":
                        case "ap-mso":
                        case "ap-username":
                        case "ap-password":
                        case "extract-audio":
                        case "audio-format":
                        case "audio-quality":
                        case "recode-video":
                        case "postprocessor-args":
                        case "embed-subs":
                        case "embed-thumbnail":
                        case "add-metadata":
                        case "metadata-from-title":
                        case "xattrs":
                        case "fixup":
                        case "prefer-avconv":
                        case "prefer-ffmpeg":
                        case "convert-subs":
                            YoutubeDlArgs += string.Format(" --{0} {1}", Arg.Key, Arg.Value);
                            continue;
                        case "j":
                        case "J":
                        case "dump-json":
                        case "dump-single-json":
                        case "print-json":
                            YoutubeDlArgs += string.Format(" --{0} {1}", Arg.Key, Arg.Value);
                            UseFFmpeg = false;
                            GetYoutubeJson = true;
                            continue;
                        case "loglevel":
                        case "max_alloc":
                        case "filter_threads":
                        case "filter_complex_threads":
                        case "stats":
                        case "max_error_rate":
                        case "bits_per_raw_sample":
                        case "vol":
                        case "codec":
                        case "pre":
                        case "t":
                        case "to":
                        case "fs":
                        case "ss":
                        case "sseof":
                        case "seek_timestamp":
                        case "timestamp":
                        case "metadata":
                        case "program":
                        case "target":
                        case "apad":
                        case "frames":
                        case "filter_script":
                        case "reinit_filter":
                        case "discard":
                        case "disposition":
                        case "vframes":
                        case "r":
                        case "s":
                        case "aspect":
                        case "vn":
                        case "vcodec":
                        case "timecode":
                        case "pass":
                        case "ab":
                        case "b":
                        case "dn":
                        case "aframes":
                        case "aq":
                        case "ar":
                        case "ac":
                        case "an":
                        case "acodec":
                        case "sn":
                        case "scodec":
                        case "stag":
                        case "fix_sub_duration":
                        case "canvas_size":
                        case "spre":
                        case "f":
                            FFmpegArgs += string.Format(" -{0} {1}", Arg.Key, Arg.Value);
                            continue;
                        case "vf":
                        case "af":
                        case "filter":
                            //ffmpeg filters parsed above
                            continue;
                        default:
                            LoggerAccessor.LogWarn("[WebVideoConverter] - Unsupported argument: {0}", Arg.Key);
                            continue;
                    }
                }

                // Configure YT-DLP and FFmpeg processes and prepare data stream
                ProcessStartInfo YoutubeDlStart = new ProcessStartInfo()
                {
                    FileName = $"{ConverterDir}/yt-dlp",
                    Arguments = string.Format("\"{0}\"{1} -o -", url, YoutubeDlArgs),
                    WorkingDirectory = ConverterDir,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                ProcessStartInfo FFmpegStart = new ProcessStartInfo()
                {
                    FileName = $"{ConverterDir}/ffmpeg",
                    Arguments = string.Format("-i pipe: {0} pipe:", FFmpegArgs),
                    WorkingDirectory = ConverterDir,
                    RedirectStandardInput = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = false
                };

                video.Available = true;
                video.ErrorMessage = string.Empty;
                video.ContentType = Arguments["content-type"];
                video.FileName = Arguments["filename"];

                // Start both processes
                Process YoutubeDl = null;
                Process FFmpeg = null;
                if (UseFFmpeg)
                {
                    LoggerAccessor.LogWarn("[WebVideoConverter] - Video convert: {0} {1} | {2} {3}", YoutubeDlStart.FileName, YoutubeDlStart.Arguments, FFmpegStart.FileName, FFmpegStart.Arguments);
                    YoutubeDl = Process.Start(YoutubeDlStart);
                    FFmpeg = Process.Start(FFmpegStart);
                }
                else
                {
                    LoggerAccessor.LogWarn("[WebVideoConverter] - Video convert: {0} {1}", YoutubeDlStart.FileName, YoutubeDlStart.Arguments);
                    YoutubeDl = Process.Start(YoutubeDlStart);
                }

                // Calculate approximately end time
                DateTime EndTime = DateTime.Now.AddSeconds(30);

                // Enable YT-DLP error handling
                if (!GetYoutubeJson && YoutubeDl != null)
                {
                    YoutubeDl.ErrorDataReceived += (o, e) =>
                    {
                        if (e.Data != null)
                        {
                            LoggerAccessor.LogInfo("{0}", e.Data);

                            if (e.Data.StartsWith("ERROR:"))
                            {
                                video.Available = false;
                                video.ErrorMessage = "Online video failed to download: " + e.Data.Substring(7);
                                LoggerAccessor.LogError("[WebVideoConverter] - yt-dlp: {0}", e.Data);
                            }
                            if (e.Data.StartsWith("WARNING:"))
                                LoggerAccessor.LogWarn("[WebVideoConverter] - yt-dlp: {0}", e.Data);
                            if (Regex.IsMatch(e.Data, @"\[download\].*ETA (\d\d:\d\d:\d\d|\d\d:\d\d)"))
                            {
                                Match match = Regex.Match(e.Data, @"\[download\].*ETA (\d\d:\d\d:\d\d|\d\d:\d\d)");
                                //assuming, it's succcessfull & have 2 groups
                                EndTime = DateTime.Now.Add(TimeSpan.Parse(Regex.IsMatch(match.Groups[1].Value, @"\d\d:\d\d:\d\d") ? match.Groups[1].Value : "00:" + match.Groups[1].Value));
                            }
                        }
                    };
                    YoutubeDl.BeginErrorReadLine();
                }

                // (Here FFmpeg error handling might be useful, but it's output is hard to parse)

                // Redirect STDIN/STDOUT streams
                if (!GetYoutubeJson)
                {
                    if (UseFFmpeg && YoutubeDl != null && FFmpeg != null)
                    {
                        // - Redirect yt-dlp STDOUT to FFmpeg STDIN stream, and FFmpeg STDOUT to return stream
                        new Task(() =>
                        {
                            YoutubeDl.StandardOutput.BaseStream.CopyTo(FFmpeg.StandardInput.BaseStream);
                        }).Start();
                        video.VideoStream = FFmpeg.StandardOutput.BaseStream;
                    }
                    else if (YoutubeDl != null)
                        // - Redirect yt-dlp STDOUT to return stream
                        video.VideoStream = YoutubeDl.StandardOutput.BaseStream;
                }
                if (GetYoutubeJson && YoutubeDl != null)
                    // - Redirect yt-dlp STDERR to return stream (video metadata JSON)
                    video.VideoStream = YoutubeDl.StandardError.BaseStream;

                // Initialize idleness hunters
                new Task(() =>
                {
                    while (DateTime.Now < EndTime) { Thread.Sleep(1000); }
                    float YoutubeDlCpuLoad = 0;
                    while (YoutubeDl != null && !YoutubeDl.HasExited)
                    {
                        Thread.Sleep(1000);
                        ProcessUtils.PreventProcessIdle(ref YoutubeDl, ref YoutubeDlCpuLoad);
                    }
                }).Start();
                if (UseFFmpeg) new Task(() =>
                {
                    while (DateTime.Now < EndTime) { Thread.Sleep(1000); }
                    float FFmpegCpuLoad = 0;
                    while (FFmpeg != null && !FFmpeg.HasExited)
                    {
                        Thread.Sleep(1000);
                        ProcessUtils.PreventProcessIdle(ref FFmpeg, ref FFmpegCpuLoad);
                    }
                }).Start();

                // Wait for YT-DLP & FFmpeg to start working or end with error
                Thread.Sleep(5000);
            }
            catch (Exception VidCvtError)
            {
                video.Available = false;
                video.ErrorMessage = VidCvtError.Message;
                LoggerAccessor.LogError("[WebVideoConverter] - Cannot convert video: {0} - {1}", VidCvtError.GetType(), VidCvtError.Message);
            }

            return video;
        }

        /// <summary>
        /// Convert string "true/false" or similar to bool true/false.
        /// </summary>
        /// <param name="s">One of these strings: 1/0, y/n, yes/no, on/off, enable/disable, true/false.</param>
        /// <returns>Boolean true/false</returns>
        /// <exception cref="InvalidCastException">Throws if the <paramref name="s"/> is not 1/0/y/n/yes/no/on/off/enable/disable/true/false.</exception>
        private static bool ToBoolean(string s)
        {
            //from https://stackoverflow.com/posts/21864625/revisions
            string[] trueStrings = { "1", "y", "yes", "on", "enable", "true" };
            string[] falseStrings = { "0", "n", "no", "off", "disable", "false", string.Empty };

            if (trueStrings.Contains(s, StringComparer.OrdinalIgnoreCase))
                return true;
            if (falseStrings.Contains(s, StringComparer.OrdinalIgnoreCase))
                return false;

            throw new InvalidCastException("Only the following are supported for converting strings to boolean: "
                + string.Join(",", trueStrings)
                + " and "
                + string.Join(",", falseStrings)
                .Replace(",,", string.Empty)); //hide empty & null
        }
    }
}
