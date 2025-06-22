using NetworkLibrary.HTTP;
using System;
using System.Diagnostics;
using System.IO;
#if !NETFRAMEWORK
using System.Web;
#endif
namespace WebAPIService.MultiMedia
{
    // From: https://www.codeproject.com/Articles/1079119/Video-Transcoding-and-Streaming-on-the-fly
    public class MediaInfo
    {
        private string workpath;
        private string convertersPath;
        private string fullurl;
        private string filepath;

        public MediaInfo(string workpath, string convertersPath, string fullurl, string filepath)
        {
            this.workpath = workpath;
            this.convertersPath = convertersPath;
            this.fullurl = fullurl;
            this.filepath = filepath;
        }

        public bool IsSupported()
        {
            string path = string.Format(@"{0}/{1}", workpath,
#if NETFRAMEWORK
                Uri.UnescapeDataString(MediaBrowse.GetQueryParameter(HTTPProcessor.ExtractQueryString(fullurl), "item")))
#else
                HttpUtility.UrlDecode(MediaBrowse.GetQueryParameter(HTTPProcessor.ExtractQueryString(fullurl), "item")))
#endif
                .Replace(@"\\", @"\").Replace("//", "/");

            if (File.Exists(path))
            {
                if (path.EndsWith(".isoimg", StringComparison.InvariantCultureIgnoreCase)
                    || path.EndsWith(".iso", StringComparison.InvariantCultureIgnoreCase))
                    return false;

                return true;
            }

            return false;
        }

        public (ushort, string, string) StartFFProbe()
        {
            try
            {
                const string seekingTypeJson = "{ \"seeking_type\": \"";
                const string mediaInfoJson = "\", \"media_info\": ";

                string output = seekingTypeJson;
                string path = string.Format(@"{0}/{1}", workpath,
#if NETFRAMEWORK
                    Uri.UnescapeDataString(MediaBrowse.GetQueryParameter(HTTPProcessor.ExtractQueryString(fullurl), "item")))
#else
                    HttpUtility.UrlDecode(MediaBrowse.GetQueryParameter(HTTPProcessor.ExtractQueryString(fullurl), "item")))
#endif
                    .Replace(@"\\", @"\").Replace("//", "/");

                ProcessStartInfo pr = new ProcessStartInfo(convertersPath + "/ffprobe",
                        string.Format(@"-i ""{0}"" -show_streams -show_format -print_format json", path));

                pr.UseShellExecute = false;
                pr.RedirectStandardOutput = true;
                pr.CreateNoWindow = true;

                using (Process proc = new Process())
                {
                    proc.StartInfo = pr;

                    proc.Start();

                    output += GetSeekingType(new FileInfo(path).Extension) + mediaInfoJson + proc.StandardOutput.ReadToEnd() + " }";

                    proc.WaitForExit();

                    return (200, "text/plain", output);
                }
            }
            catch (Exception e)
            {
                CustomLogger.LoggerAccessor.LogError(string.Format("[MediaInfo] - Media.StartFFProbe, Exception : {0}", e.Message));
            }

            return (500, "text/plain", string.Empty);
        }

        public static string GetSeekingType(string fileExt)
        {
            switch (fileExt)
            {
                case ".mp4":
                    return "client";
                default:
                    return "server";
            }
        }
    }
}
