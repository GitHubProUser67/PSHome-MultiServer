using System.Xml.Linq;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.meta
{
    internal static class Trophy
    {
        public static string SetUserTrophyDataPOST(
            byte[] PostData,
            string ContentType,
            string apiPath
        )
        {
            var key = string.Empty;
            var psnid = string.Empty;
            var gameid = string.Empty;
            var trophyid = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                key = data["key"].First();
                if (key != "01e2Qexh5G6iBriq")
                {
                    CustomLogger.LoggerAccessor.LogError(
                        "[VEEMEE] - meta_trophies - Client tried to push invalid key! Invalidating request."
                    );
                    return null;
                }
                psnid = data["psnid"].First();
                gameid = data["gameid"].First();
                trophyid = data["trophyid"].First();

                var directoryPath = $"{apiPath}/VEEMEE/meta_trophies/{gameid}/User_Data";
                var filePath = $"{directoryPath}/{psnid}.xml";

                Directory.CreateDirectory(directoryPath);

                string XmlData;

                if (File.Exists(filePath))
                {
                    // Load XML into XDocument
                    var xmlDoc = XDocument.Parse(File.ReadAllText(filePath));

                    // Find the <trophy> element
                    var trophyElement = xmlDoc.Descendants("trophy").FirstOrDefault();

                    if (trophyElement != null)
                    {
                        // Check if the ID already exists
                        if (!trophyElement.Elements("trophyID").Any(e => e.Value == trophyid))
                            // Add the new <trophyID> inside the existing <trophy> element
                            trophyElement.Add(new XElement("trophyID", trophyid));
                    }

                    XmlData = xmlDoc.ToString();
                    File.WriteAllText(filePath, XmlData);
                    return XmlData;
                }
                else
                {
                    XmlData = $"<player><trophy><trophyID>{trophyid}</trophyID></trophy></player>";
                    File.WriteAllText(filePath, XmlData);
                    return XmlData;
                }
            }

            return null;
        }

        public static string GetUserTrophyDataPOST(
            byte[] PostData,
            string ContentType,
            string apiPath
        )
        {
            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                var key = data["key"].First();
                if (key != "01e2Qexh5G6iBriq")
                {
                    CustomLogger.LoggerAccessor.LogError(
                        "[VEEMEE] - meta_trophies - Client tried to push invalid key! Invalidating request."
                    );
                    return null;
                }

                var psnid = data["psnid"].First();
                var gameid = data["gameid"].First();
                var directoryPath = $"{apiPath}/VEEMEE/meta_trophies/{gameid}/User_Data";
                var filePath = $"{directoryPath}/{psnid}.xml";

                if (File.Exists(filePath))
                    return File.ReadAllText(filePath);
            }

            return $"<player><trophy></trophy></player>";
        }
    }
}
