using System.IO;
using System.Linq;
using System.Xml;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.VEEMEE.gofish
{
    public class FishCaughtProcess
    {
        public static string SetPOST(byte[] PostData, string ContentType, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;
            string fish_mask_upper = string.Empty;
            string fish_mask_lower = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                key = data["key"].First();
                if (key != "tHeHuYUmuDa54qur")
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - gofish - Client tried to push invalid key! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();
                fish_mask_upper = data["fish_mask_upper"].First();
                fish_mask_lower = data["fish_mask_lower"].First();

                Directory.CreateDirectory($"{apiPath}/VEEMEE/gofish/Fish_User_Data");

                if (File.Exists($"{apiPath}/VEEMEE/gofish/Fish_User_Data/{psnid}.xml"))
                {
                    // Load the XML string into an XmlDocument
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml($"<xml>{File.ReadAllText($"{apiPath}/VEEMEE/gofish/Fish_User_Data/{psnid}.xml")}</xml>");

                    // Find the <fish_mask_upper> element
                    XmlElement fish_mask_upperElement = xmlDoc.SelectSingleNode("/xml/fish_mask_upper") as XmlElement;

                    if (fish_mask_upperElement != null)
                    {
                        // Replace the value of <fish_mask_upper> with a new value
                        fish_mask_upperElement.InnerText = fish_mask_upper;

                        // Find the <fish_mask_lower> element
                        XmlElement fish_mask_lowerElement = xmlDoc.SelectSingleNode("/xml/fish_mask_lower") as XmlElement;

                        if (fish_mask_lowerElement != null)
                        {
                            // Replace the value of <fish_mask_lower> with a new value
                            fish_mask_lowerElement.InnerText = fish_mask_lower;

                            string XmlResult = xmlDoc.OuterXml.Replace("<xml>", string.Empty).Replace("</xml>", string.Empty);
                            File.WriteAllText($"{apiPath}/VEEMEE/gofish/Fish_User_Data/{psnid}.xml", XmlResult);
                            return XmlResult;
                        }
                    }
                }
                else
                {
                    string XmlData = $"<fish_mask_upper>{fish_mask_upper}</fish_mask_upper><fish_mask_lower>{fish_mask_lower}</fish_mask_lower>";
                    File.WriteAllText($"{apiPath}/VEEMEE/gofish/Fish_User_Data/{psnid}.xml", XmlData);
                    return XmlData;
                }
            }

            return null;
        }

        public static string GetPOST(byte[] PostData, string ContentType, string apiPath)
        {
            string key = string.Empty;
            string psnid = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                key = data["key"].First();
                if (key != "tHeHuYUmuDa54qur")
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - gofish - Client tried to push invalid key! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();

                if (File.Exists($"{apiPath}/VEEMEE/gofish/Fish_User_Data/{psnid}.xml"))
                    return File.ReadAllText($"{apiPath}/VEEMEE/gofish/Fish_User_Data/{psnid}.xml");
            }

            return null;
        }
    }
}
