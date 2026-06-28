using System.Xml;
using CustomLogger;
using HttpMultipartParser;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.OUWF
{
    public class OuWFScrape
    {
        public static string Scrape(byte[] PostData, string ContentType)
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

                var matches = Scrape(path);
                var xmlString = GenerateXml(matches);

                ms.Flush();

                return data;
            }
        }

        static List<string> Scrape(string mdlFilePath)
        {
            List<string> matches = [];
            ScrapeRecursive(Path.GetDirectoryName(mdlFilePath), matches);
            return matches;
        }

        static void ScrapeRecursive(string directory, List<string> matches)
        {
            foreach (var filePath in Directory.GetFiles(directory, "*.dds"))
            {
                matches.Add(filePath);
            }

            foreach (var subdirectory in Directory.GetDirectories(directory))
            {
                ScrapeRecursive(subdirectory, matches);
            }
        }

        static string GenerateXml(List<string> matches)
        {
            var xmlDoc = new XmlDocument();
            var rootElement = xmlDoc.CreateElement("scrape");

            foreach (var match in matches)
            {
                var matchElement = xmlDoc.CreateElement("match");
                matchElement.InnerText = match;
                rootElement.AppendChild(matchElement);
            }

            xmlDoc.AppendChild(rootElement);

            var stringWriter = new StringWriter();
            var xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlDoc.WriteTo(xmlTextWriter);

            return stringWriter.ToString();
        }
    }
}
