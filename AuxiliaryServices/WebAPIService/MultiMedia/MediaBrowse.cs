using MultiServerLibrary.HTTP;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Text;
#if !NETFRAMEWORK
using System.Web;
#endif

namespace WebAPIService.MultiMedia
{
    // From: https://www.codeproject.com/Articles/1079119/Video-Transcoding-and-Streaming-on-the-fly
    public class MediaBrowse
    {
        private string workpath;
        private string fullurl;
        private string filepath;

        public MediaBrowse(string workpath, string fullurl, string filepath)
        {
            this.workpath = workpath;
            this.fullurl = fullurl;
            this.filepath = filepath;
        }

        public bool IsSupported()
        {
            if (Directory.Exists(string.Format(@"{0}/{1}", workpath,
#if NETFRAMEWORK
                Uri.UnescapeDataString(GetQueryParameter(HTTPProcessor.ExtractQueryString(fullurl), "item")))
#else
                HttpUtility.UrlDecode(GetQueryParameter(HTTPProcessor.ExtractQueryString(fullurl), "item")))
#endif
                .Replace(@"\\", @"\").Replace("//", "/")))
                return true;

            return false;
        }

        public (ushort, string, string) ListDirectoriesHandler()
        {
            try
            {
                NameValueCollection urlArgs = HTTPProcessor.ExtractQueryString(fullurl);
                if (urlArgs != null)
                {
                    long offset = long.Parse(GetQueryParameter(urlArgs, "offset"));
                    long count = long.Parse(GetQueryParameter(urlArgs, "count"));
#if NETFRAMEWORK
                    string instance = Uri.UnescapeDataString(GetQueryParameter(urlArgs, "item"));
#else
                    string instance = HttpUtility.UrlDecode(GetQueryParameter(urlArgs, "item"));
#endif
                    string path = string.Format(@"{0}/{1}", workpath, instance).Replace(@"\\", @"\").Replace("//", "/");
                    if (Directory.Exists(path))
                    {
                        long readCount = 0;

                        StringBuilder dirs = new StringBuilder();
                        dirs.Append("[");

                        foreach (FileSystemInfo fsInfo in new DirectoryInfo(path).GetFileSystemInfos())
                        {
                            if (readCount < offset)
                                continue;

                            if (readCount < offset + count)
                            {
                                dirs.Append("{");
                                dirs.AppendFormat("\"name\": \"{1}\", \"path\": \"{0}/{1}\", \"type\": \"{2}\"", instance.Replace(@"\", "/"), fsInfo.Name, (fsInfo.Attributes & FileAttributes.Directory) == FileAttributes.Directory ? 0 : 1);
                                dirs.Append("}");
                                dirs.Append(",");
                                readCount++;
                            }
                            else
                                break;
                        }

                        dirs.Length = dirs.Length - 1;
                        dirs.Append("]");

                        return (200, HTTPProcessor.GetMimeType(".js"), dirs.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                CustomLogger.LoggerAccessor.LogError(string.Format("[MediaBrowse] - Browse.ListDirectoriesHandler, Exception: {0}", e.Message));
            }

            return (500, "text/plain", string.Empty);
        }

        public static string GetQueryParameter(NameValueCollection query, string name)
        {
            string result = string.Empty;
            try
            {
                return query[name];
            }
            catch { };
            return result;
        }
    }
}
