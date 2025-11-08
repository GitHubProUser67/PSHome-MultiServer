using CustomLogger;
using System.Text;
using MultiServerLibrary.HTTP;
using HttpMultipartParser;
using Newtonsoft.Json.Linq;
using System.IO;
using System;

namespace WebAPIService.GameServices.PSHOME.CAPONE
{
    public class GriefReporter
    {
        public static string caponeContentStoreUpload(byte[] PostData, string ContentType, string workPath, string absolutePath)
        {
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            using (MemoryStream ms = new MemoryStream(PostData))
            {
                try
                {
                    // Read the multipart content.
                    var provider = MultipartFormDataParser.Parse(ms, boundary);
                    string finalPath = workPath + absolutePath;

                    Directory.CreateDirectory(finalPath);

                    // Process each part.
                    foreach (var part in provider.Files)
                    {
                        // Only process file parts.
                        if (part.FileName != null)
                        {
                            // Get the filename.
                            string fileName = part.FileName;

                            // Save the file with name.
                            string filePath = Path.Combine(finalPath, fileName);

                            // Write the file data directly to the file.
                            using (var fileStream = File.Create(filePath))
                            {
                                part.Data.CopyTo(fileStream);
                            }

                            LoggerAccessor.LogInfo($"[CAPONE] - GriefReporter - Written Evidence file {fileName} to {filePath} contentStore!");
                        }
                    }

                    ms.Flush();
                    return "<xml></xml>";
                }
                catch (Exception e)
                {
                    return $"InternalServerError with exception {e}";
                }


            }

        }

        public static string caponeReportCollectorSubmit(byte[] PostData, string ContentType, string workPath)
        {
            string fileName = string.Empty;
            
            try
            {
                //JObject jObject = JObject.Parse(Encoding.UTF8.GetString(PostData));
                //Uri? dataURL = (Uri?)Utils.JtokenUtils.GetValueFromJToken(jObject, "dataLocation");

                string finalPath = workPath + "/capone/reportCollector/submit";

                Directory.CreateDirectory(finalPath);

                JObject jObject = JObject.Parse(Encoding.UTF8.GetString(PostData));

                string sourceItemId = jObject["sourceItemId"]?.ToString();
                fileName = sourceItemId + ".json";
                // Save the file with name.
                string filePath = Path.Combine(finalPath, fileName);

                File.WriteAllText(filePath, Encoding.UTF8.GetString(PostData));

                LoggerAccessor.LogInfo($"[CAPONE] GriefReporter - GriefReport JSON received and written to {filePath} Collection!");
            }
            catch (Exception e)
            {
                return $"InternalServerError with exception {e}";
            }
            

            return "<xml></xml>";

        }

    }
}