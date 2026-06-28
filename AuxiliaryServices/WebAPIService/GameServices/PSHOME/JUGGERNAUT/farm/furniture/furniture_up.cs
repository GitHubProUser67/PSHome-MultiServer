using System.Globalization;
using System.Text.RegularExpressions;
using MultiServerLibrary.HTTP;

namespace WebAPIService.GameServices.PSHOME.JUGGERNAUT.farm.furniture
{
    public partial class furniture_up
    {
        public static string ProcessUp(byte[] PostData, string ContentType, string apiPath)
        {
            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                var file = data["file"].First();

                if (!string.IsNullOrEmpty(file))
                {
                    // Match the pattern in the input string
                    var match = MyRegex().Match(file);

                    // Check if the pattern is found
                    if (match.Success)
                    {
                        try
                        {
                            // Convert the string to a int
                            var slotIDInt = (int)
                                double.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);

                            // Match the pattern in the input string
                            match = Regex.Match(file, @"<user>(.*?)<\/user>");

                            // Check if the pattern is found
                            if (match.Success)
                            {
                                // Extract the matched value
                                var userContent = match.Groups[1].Value;

                                Directory.CreateDirectory(
                                    $"{apiPath}/juggernaut/farm/User_Data/{userContent}"
                                );

                                File.WriteAllText(
                                    $"{apiPath}/juggernaut/farm/User_Data/{userContent}/{slotIDInt}.xml",
                                    file
                                );

                                return string.Empty;
                            }
                        }
                        catch { }
                    }
                }
            }

            return null;
        }

        [GeneratedRegex(@"<slotID>(\d+\.\d+)<\/slotID>")]
        private static partial Regex MyRegex();
    }
}
