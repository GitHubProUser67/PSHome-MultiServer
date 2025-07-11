using WatsonWebserver.Core;
using MultiServerLibrary.Extension;
using System.Diagnostics;
using System.IO;
using System;
using System.Text;
using System.Linq;

namespace ApacheNet.Extensions
{
    public class PHP
    {
        public static (byte[]?, string[][]) ProcessPHPPage(string FilePath, string phppath, string phpver, HttpContextBase ctx, bool secure)
        {
            int index = ctx.Request.Url.RawWithQuery.IndexOf("?");
            string? queryString = index == -1 ? string.Empty : ctx.Request.Url.RawWithQuery[(index + 1)..];

            // Get paths for PHP
            string? documentRootPath = Path.GetDirectoryName(FilePath);
            string? scriptFilePath = Path.GetFullPath(FilePath);
            string? scriptFileName = Path.GetFileName(FilePath);

            string[][] HeadersLocal = Array.Empty<string[]>();
            byte[]? returndata = null;
            byte[]? postData = ctx.Request.DataAsBytes;

            using Process proc = new();

            proc.StartInfo.FileName = $"{phppath}/{phpver}/php-cgi";

            proc.StartInfo.Arguments = $"-q -c \"{$"{phppath}/{phpver}/php.ini"}\" -d \"error_reporting=E_ALL\" -d \"display_errors={ApacheNetServerConfiguration.PHPDebugErrors}\" -d \"expose_php=Off\" -d \"include_path='{documentRootPath}'\" " +
                         $"-d \"extension_dir='{$"{phppath}/{phpver}/ext/"}'\" \"{FilePath}\"";

            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;

            proc.StartInfo.StandardOutputEncoding = Encoding.UTF8;
            proc.StartInfo.StandardErrorEncoding = Encoding.UTF8;

            proc.StartInfo.EnvironmentVariables.Clear();

            // Set content length if needed
            if (postData != null)
                proc.StartInfo.EnvironmentVariables.Add("CONTENT_LENGTH", postData.Length.ToString());

            // Set environment variables for PHP
            if (MultiServerLibrary.Extension.Microsoft.Win32API.IsWindows)
            {
                proc.StartInfo.EnvironmentVariables["SYSTEMROOT"] = Environment.GetEnvironmentVariable("SYSTEMROOT");
                proc.StartInfo.EnvironmentVariables["WINDIR"] = Environment.GetEnvironmentVariable("WINDIR");
                proc.StartInfo.EnvironmentVariables["COMSPEC"] = Environment.GetEnvironmentVariable("COMSPEC");
                proc.StartInfo.EnvironmentVariables["TMPDIR"] = Environment.GetEnvironmentVariable("TMPDIR");
                proc.StartInfo.EnvironmentVariables["TEMP"] = Environment.GetEnvironmentVariable("TEMP");
            }
            else
            {
                const string usrDir = "/usr";
                const string tmpDir = "/tmp";
                proc.StartInfo.EnvironmentVariables["SYSTEMROOT"] = usrDir;
                proc.StartInfo.EnvironmentVariables["WINDIR"] = usrDir;
                proc.StartInfo.EnvironmentVariables["COMSPEC"] = "/bin/sh";
                proc.StartInfo.EnvironmentVariables["TMPDIR"] = tmpDir;
                proc.StartInfo.EnvironmentVariables["TEMP"] = tmpDir;
            }
            proc.StartInfo.EnvironmentVariables["PATH"] = Environment.GetEnvironmentVariable("PATH");
            proc.StartInfo.EnvironmentVariables.Add("GATEWAY_INTERFACE", "CGI/1.1");
            proc.StartInfo.EnvironmentVariables.Add("SERVER_PROTOCOL", $"HTTP/{ApacheNetServerConfiguration.HttpVersion}");
            proc.StartInfo.EnvironmentVariables.Add("REDIRECT_STATUS", "200");
            proc.StartInfo.EnvironmentVariables.Add("DOCUMENT_ROOT", documentRootPath);
            proc.StartInfo.EnvironmentVariables.Add("SCRIPT_NAME", scriptFileName);
            proc.StartInfo.EnvironmentVariables.Add("SCRIPT_FILENAME", scriptFilePath);
            proc.StartInfo.EnvironmentVariables.Add("QUERY_STRING", queryString);
            proc.StartInfo.EnvironmentVariables.Add("CONTENT_TYPE", ctx.Request.ContentType);
            proc.StartInfo.EnvironmentVariables.Add("REQUEST_METHOD", ctx.Request.Method.ToString());
            proc.StartInfo.EnvironmentVariables.Add("USER_AGENT", ctx.Request.Useragent);
            proc.StartInfo.EnvironmentVariables.Add("SERVER_ADDR", ctx.Request.Destination.IpAddress);
            proc.StartInfo.EnvironmentVariables.Add("REMOTE_ADDR", ctx.Request.Source.IpAddress);
            proc.StartInfo.EnvironmentVariables.Add("REMOTE_PORT", ctx.Request.Source.Port.ToString());
            proc.StartInfo.EnvironmentVariables.Add("REQUEST_URI", $"{(secure ? "https" : "http")}://{ctx.Request.Destination.IpAddress}:{ctx.Request.Destination.Port}{ctx.Request.Url.RawWithQuery}");
            foreach (var headerKeyPair in ctx.Request.Headers.ConvertHeadersToPhpFriendly())
            {
                string? key = headerKeyPair.Key;
                string? value = headerKeyPair.Value;

                if (!string.IsNullOrEmpty(key) && value != null && IsValidEnvVarKey(key))
                    proc.StartInfo.EnvironmentVariables.Add(key, value);
            }

            proc.Start();

            if (postData != null)
            {
                // Write request body to standard input
                using StreamWriter? sw = proc.StandardInput;
                sw.BaseStream.Write(postData, 0, postData.Length);
            }

            // Write headers and content to response stream
            bool headersEnd = false;
            string? errorOutput = null;
            using (MemoryStream ms = new())
            using (StreamReader sr = proc.StandardOutput)
            using (StreamReader err = proc.StandardError)
            using (StreamWriter output = new(ms))
            {
                int i = 0;
                string? line = null;
                // 🔵 FIRST: Drain stdout fully
                while ((line = sr.ReadLine()) != null)
                {
                    if (!headersEnd)
                    {
                        if (line == string.Empty)
                        {
                            headersEnd = true;
                            continue;
                        }

                        // The first few lines are the headers, with a
                        // key and a value. Catch those, to write them
                        // into our response headers.
                        index = line.IndexOf(':');

                        if (index != -1)
                            HeadersLocal = HeadersLocal.AddArray(new string[] { line[..index], line[(index + 2)..] });
                        else
                            // Write non-header lines into the output as is.
                            output.WriteLine(line);
                    }
                    else
                        // Write non-header lines into the output as is.
                        output.WriteLine(line);

                    i++;
                }

                output.Flush();

                // 🔵 THEN: After stdout drained and no results, read stderr
                if (ms.Length == 0 && ApacheNetServerConfiguration.PHPDebugErrors)
                    errorOutput = err.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(errorOutput))
                {
                    output.WriteLine();
                    output.WriteLine("<!-- PHP ERROR OUTPUT BELOW -->");
                    output.WriteLine("<pre style=\"color:red;\">");
                    output.WriteLine(System.Net.WebUtility.HtmlEncode(errorOutput));
                    output.WriteLine("</pre>");
                    output.Flush();
                }

                returndata = ms.ToArray();
            }

            proc.WaitForExit(); // Wait for the PHP process to complete

            return (returndata, HeadersLocal);
        }

        private static bool IsValidEnvVarKey(string key)
        {
            // Environment variable keys usually can't have '=', '\0', or other special characters
            return key.All(c => c > 31 && c != '=' && c != '\0');
        }
    }
}
