using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.OUWF
{
    public class OuWFExecute
    {
        public static string Execute(byte[] PostData, string ContentType)
        {
            var boundary = HTTPProcessor.ExtractBoundary(ContentType);

            using (var ms = new MemoryStream(PostData))
            {
                var multipartData = MultipartFormDataParser.Parse(ms, boundary);

                var instanceId = Convert.ToInt32(multipartData.GetParameterValue("instanceId"));
                var vers = multipartData.GetParameterValue("version");
                var path = multipartData.GetParameterValue("path");
                var data = multipartData.GetParameterValue("data");

                LoggerAccessor.LogInfo(
                    $"[OuWF] - Requested Execute with instanceId {instanceId} | version {vers} | path {path} | data {data}"
                );

                try
                {
                    //string toTrim = "lua|";
                    var fileContent = ReadFile(data[4..]);
                    LoggerAccessor.LogInfo($"[OuWF] - File content: {fileContent}");
                    return fileContent;
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogError($"[OuWF] - Error reading the file: {ex.Message}");
                }

                ms.Flush();

                return null;
            }
        }

        static string ReadFile(string filePath)
        {
            // Check if the file exists
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            // Read the content of the file
            using (var reader = new StreamReader(filePath))
                return reader.ReadToEnd();
        }
    }
}
