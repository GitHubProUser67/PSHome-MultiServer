using System.Text;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.OUWF
{
    public class OuWFSet
    {
        public static string Set(byte[] PostData, string ContentType)
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
                    $"[OuWF] - Requested Set with instanceId {instanceId} | version {vers} | path {path} | data \n{data}"
                );

                /*
                // Check if the directory exists, if not, create it
                if (!Directory.Exists(Path.GetPathRoot(path)))
                {
                    Directory.CreateDirectory(Path.GetPathRoot(path));
                }
                */
                // Create the file (this will also overwrite if the file already exists)
                using (var fs = File.Create(path))
                {
                    LoggerAccessor.LogInfo("File created successfully!");
                    fs.Write(Encoding.UTF8.GetBytes(data));
                    fs.Close();

                    // Perform additional operations with the FileStream if needed
                }

                ms.Flush();

                return data;
            }
        }
    }
}
