using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.JUGGERNAUT.farm.furniture
{
    public class furniture_up
    {
        public static string ProcessUp(byte[] PostData, string ContentType, string apiPath)
        {
            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                string file = data["file"].First();

                if (!string.IsNullOrEmpty(file))
                {
                    // Match the pattern in the input string
                    Match match = Regex.Match(file, @"<slotID>(\d+\.\d+)<\/slotID>");

                    // Check if the pattern is found
                    if (match.Success)
                    {
                        try
                        {
                            // Convert the string to a int
                            int slotIDInt = (int)double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

                            // Match the pattern in the input string
                            match = Regex.Match(file, @"<user>(.*?)<\/user>");

                            // Check if the pattern is found
                            if (match.Success)
                            {
                                // Extract the matched value
                                string userContent = match.Groups[1].Value;

                                Directory.CreateDirectory($"{apiPath}/juggernaut/farm/User_Data/{userContent}");

                                File.WriteAllText($"{apiPath}/juggernaut/farm/User_Data/{userContent}/{slotIDInt}.xml", file);

                                return string.Empty;
                            }
                        }
                        catch
                        {
                        }
                    }
                }
            }

            return null;
        }
    }
}
