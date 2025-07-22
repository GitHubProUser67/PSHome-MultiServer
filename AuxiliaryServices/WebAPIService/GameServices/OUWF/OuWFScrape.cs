using System.IO;
using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;
using System.Xml;
using System.Collections.Generic;
using System;

namespace WebAPIService.GameServices.OUWF
{
    public class OuWFScrape
    {
        public static string Scrape(byte[] PostData, string ContentType)
        {
            string boundary = HTTPProcessor.ExtractBoundary(ContentType);

            using (MemoryStream ms = new MemoryStream(PostData))
            {
                var multipartData = MultipartFormDataParser.Parse(ms, boundary);

                int instanceId = Convert.ToInt32(multipartData.GetParameterValue("instanceId"));
                string vers = multipartData.GetParameterValue("version");
                string path = multipartData.GetParameterValue("path");
                string data = multipartData.GetParameterValue("data");

                LoggerAccessor.LogInfo($"[OuWF] - Requested Execute with instanceId {instanceId} | version {vers} | path {path} | data {data}");

                List<string> matches = Scrape(path);
                string xmlString = GenerateXml(matches);

                ms.Flush();

                return data;
            }
        }

        static List<string> Scrape(string mdlFilePath)
        {
            List<string> matches = new List<string>();
            ScrapeRecursive(Path.GetDirectoryName(mdlFilePath), matches);
            return matches;
        }

        static void ScrapeRecursive(string directory, List<string> matches)
        {
            foreach (string filePath in Directory.GetFiles(directory, "*.dds"))
            {
                matches.Add(filePath);
            }

            foreach (string subdirectory in Directory.GetDirectories(directory))
            {
                ScrapeRecursive(subdirectory, matches);
            }
        }

        static string GenerateXml(List<string> matches)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlElement rootElement = xmlDoc.CreateElement("scrape");

            foreach (string match in matches)
            {
                XmlElement matchElement = xmlDoc.CreateElement("match");
                matchElement.InnerText = match;
                rootElement.AppendChild(matchElement);
            }

            xmlDoc.AppendChild(rootElement);

            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
            xmlDoc.WriteTo(xmlTextWriter);

            return stringWriter.ToString();
        }

    }
}
