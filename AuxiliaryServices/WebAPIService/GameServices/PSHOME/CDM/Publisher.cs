using CustomLogger;

namespace WebAPIService.GameServices.PSHOME.CDM
{
    public class Publisher
    {
        public static string handlePublisherList(
            byte[] PostData,
            string ContentType,
            string workpath,
            string absolutePath
        )
        {
            var pubListPath = $"{workpath}/CDM/Publishers";

            Directory.CreateDirectory(pubListPath);
            var filePath = $"{pubListPath}/list.xml";
            if (File.Exists(filePath))
            {
                return "<xml>\r\n\t"
                    + "<status>success</status>\r\n"
                    + $"{File.ReadAllText(filePath)}\r\n"
                    + "</xml>";
            }
            else
                LoggerAccessor.LogError(
                    $"[CDM] - Failed to find publisher list with expected path {filePath}!"
                );

            return "<xml><status>fail</status></xml>";
        }
    }
}
