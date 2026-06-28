using System.Text;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;
using Newtonsoft.Json.Linq;

namespace WebAPIService.GameServices.PSHOME.CAPONE
{
    public class GriefReporter
    {
        public static string caponeContentStoreUpload(
            byte[] PostData,
            string ContentType,
            string workPath,
            string absolutePath
        )
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            using (var ms = new MemoryStream(PostData))
            {
                try
                {
                    // Read the multipart content.
                    var provider = MultipartFormDataParser.Parse(ms, boundary);
                    var finalPath = workPath + absolutePath;

                    Directory.CreateDirectory(finalPath);

                    // Process each part.
                    foreach (var part in provider.Files)
                    {
                        // Only process file parts.
                        if (part.FileName != null)
                        {
                            // Get the filename.
                            var fileName = part.FileName;

                            // Save the file with name.
                            var filePath = Path.Combine(finalPath, fileName);

                            // Write the file data directly to the file.
                            using (var fileStream = File.Create(filePath))
                            {
                                part.Data.CopyTo(fileStream);
                            }

                            LoggerAccessor.LogInfo(
                                $"[CAPONE] - GriefReporter - Written Evidence file {fileName} to {filePath} contentStore!"
                            );
                        }
                    }

                    return "<xml></xml>";
                }
                catch (Exception e)
                {
                    return $"InternalServerError with exception {e}";
                }
            }
        }

        public static string caponeReportCollectorSubmit(
            byte[] PostData,
            string ContentType,
            string workPath
        )
        {
            try
            {
                //JObject jObject = JObject.Parse(Encoding.UTF8.GetString(PostData));
                //Uri? dataURL = (Uri?)Utils.JtokenUtils.GetValueFromJToken(jObject, "dataLocation");

                var finalPath = workPath + "/capone/reportCollector/submit";

                Directory.CreateDirectory(finalPath);

                var jObject = JObject.Parse(Encoding.UTF8.GetString(PostData));

                var sourceItemId = jObject["sourceItemId"]?.ToString();
                var fileName = sourceItemId + ".json";
                // Save the file with name.
                var filePath = Path.Combine(finalPath, fileName);

                File.WriteAllText(filePath, Encoding.UTF8.GetString(PostData));

                LoggerAccessor.LogInfo(
                    $"[CAPONE] GriefReporter - GriefReport JSON received and written to {filePath} Collection!"
                );
            }
            catch (Exception e)
            {
                return $"InternalServerError with exception {e}";
            }

            return "<xml></xml>";
        }
    }
}
