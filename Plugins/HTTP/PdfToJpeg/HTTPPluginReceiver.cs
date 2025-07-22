using MultiServerLibrary.HTTP;
using System;
using System.IO;
using System.Threading.Tasks;
using WatsonWebserver.Core;
using System.Net;
using HttpMultipartParser;
using System.Collections.Generic;
using System.IO.Compression;
using PDFtoImage;
using SkiaSharp;
using ApacheNet.PluginManager;
using WebAPIService;

namespace PdfToJpeg
{
    public class HTTPPluginReceiver : HTTPPlugin
    {
        public Task HTTPStartPlugin(string param, ushort port)
        {
            return Task.CompletedTask;
        }

        public object? ProcessPluginMessage(object obj)
        {
            if (obj is HttpContextBase ctx)
            {
                HttpRequestBase request = ctx.Request;
                HttpResponseBase response = ctx.Response;

                bool sent = false;

                if (!string.IsNullOrEmpty(request.Url.RawWithQuery))
                {
                    switch (request.Method.ToString())
                    {
                        case "POST":

                            switch (HTTPProcessor.ExtractDirtyProxyPath(request.RetrieveHeaderValue("Referer")) + HTTPProcessor.ProcessQueryString(HTTPProcessor.DecodeUrl(request.Url.RawWithQuery)))
                            {
                                #region PdfConvert
                                case "/!PdfConvert/Process/":
                                    (byte[]?, string)? makeres = ProcessPDFConvert(new MemoryStream(request.DataAsBytes), request.ContentType);
                                    if (makeres != null)
                                    {
                                        response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        response.Headers.Add("Content-disposition", $"attachment; filename={makeres.Value.Item2}");
                                        response.StatusCode = (int)HttpStatusCode.OK;
                                        response.ContentType = "text/plain";
                                        sent = response.Send(makeres.Value.Item1).Result;
                                    }
                                    else
                                    {
                                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                        response.ContentType = "text/plain";
                                        sent = response.Send().Result;
                                    }
                                    break;
                                    #endregion
                            }

                            break;
                    }
                }

                return sent;
            }

            return null;
        }

        private static (byte[], string)? ProcessPDFConvert(Stream? PostData, string ContentType)
        {
            (byte[], string)? output = null;
            List<(byte[], string)?> TasksResult = new();

            if (PostData != null && !string.IsNullOrEmpty(ContentType))
            {
                string maindir = Path.GetTempPath();
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                if (!string.IsNullOrEmpty(boundary))
                {
                    using MemoryStream ms = new();
                    PostData.CopyTo(ms);
                    ms.Position = 0;
                    string? filename = null;
                    foreach (FilePart multipartfile in MultipartFormDataParser.Parse(ms, boundary).Files)
                    {
                        filename = multipartfile.FileName;

                        if (filename.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase))
                        {
                            using Stream filedata = multipartfile.Data;
                            using MemoryStream pdfstream = new();
                            int i = 0;
                            string pdfDirectory = $"{maindir}/{NetHasher.DotNetHasher.ComputeMD5String(WebAPIsUtils.GetCurrentDateTime() + filename)}/";
                            string pdfPrefix = pdfDirectory + Path.GetFileNameWithoutExtension(filename);
                            Directory.CreateDirectory(pdfDirectory);
                            filedata.CopyTo(pdfstream);
                            foreach (SKBitmap image in Conversion.ToImages(Convert.ToBase64String(pdfstream.ToArray())))
                            {
                                // Encode the image to SKData in JPEG format
                                using (SKData encodedData = image.Encode(SKEncodedImageFormat.Jpeg, 100)) // 100 is the quality level (0-100)
                                {
                                    // Save the encoded data to a file
                                    try
                                    {
                                        using FileStream fs = File.OpenWrite(pdfPrefix + $"_{i + 1}.jpg");
                                        encodedData.SaveTo(fs);
                                    }
                                    catch
                                    {

                                    }
                                }
                                i++;
                            }

                            if (Directory.Exists(pdfDirectory))
                            {
                                foreach (string filePath in Directory.GetFiles(pdfDirectory, "*.jpg"))
                                {
                                    TasksResult.Add((File.ReadAllBytes(filePath), Path.GetFileName(filePath)));
                                }

                                Directory.Delete(pdfDirectory, true);
                            }

                            filedata.Flush();
                            pdfstream.Flush();
                        }
                    }
                    ms.Flush();
                }

                // Temp dir auto-removes itself when the folder becomes empty.
            }

            if (TasksResult.Count > 0)
            {
                // Create a memory stream to hold the zip file content
                using MemoryStream memoryStream = new();

                // Create a ZipArchive in memory
                using (ZipArchive archive = new(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var item in TasksResult)
                    {
                        if (item.HasValue)
                        {
                            // Add files or content to the zip archive
                            if (item.Value.Item1 != null)
                                WebAPIsUtils.AddFileToZip(archive, item.Value.Item2, new MemoryStream(item.Value.Item1));
                        }
                    }
                }

                memoryStream.Position = 0;

                output = (memoryStream.ToArray(), $"PdfToJpeg_Results.zip");
            }

            return output;
        }
    }
}
