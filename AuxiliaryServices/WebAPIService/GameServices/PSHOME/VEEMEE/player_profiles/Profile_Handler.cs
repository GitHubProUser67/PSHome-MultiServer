using MultiServerLibrary.HTTP;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace WebAPIService.GameServices.PSHOME.VEEMEE.player_profiles
{
    public class Profile_Handler
    {
        public static string SetPOST(byte[] PostData, string ContentType, string apiPath)
        {
            string game = string.Empty;
            string psnid = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                // Organize data by identifying variables and lists
                List<Dictionary<string, string>> variables = new List<Dictionary<string, string>>();
                Dictionary<(string, string), List<string>> lists = new Dictionary<(string, string), List<string>>();

                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                game = data["game"].First();
                if (string.IsNullOrEmpty(game))
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - Profile_Handler:SetPOST - Client tried to push invalid game! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();

                foreach (var item in data)
                {
                    // Process variables
                    if (item.Key.StartsWith("name["))
                    {
                        int index = GetIndex(item.Key);

                        variables.Add(new Dictionary<string, string>
                        {
                            { "name", item.Value.First() },
                            { "type", data[$"type[{index}]"].First() },
                            { "value", data[$"value[{index}]"] .First()}
                        });
                    }
                    // Process lists
                    else if (item.Key.StartsWith("list["))
                    {
                        int listIndex = GetIndex(item.Key);
                        (string, string) listIdentifier = (data[$"list[{listIndex}][name]"].First(), data[$"list[{listIndex}][type]"].First());

                        if (lists.ContainsKey(listIdentifier))
                            continue; // Already processed, skip.
                        else
                            lists.Add(listIdentifier, new List<string>());

                        // Add values to the list based on value indices
                        foreach (var listItem in data.Where(kvp => kvp.Key.StartsWith($"list[{listIndex}][values]")))
                        {
                            lists[listIdentifier].Add(listItem.Value.First());
                        }
                    }
                }

                switch (game)
                {
                    case "winterwonderland":
                        XDocument xmlDocument;
                        string profilePath = $"{apiPath}/VEEMEE/winterwonderland/SantaQuest_User_Data/{psnid}.xml";
                        Directory.CreateDirectory($"{apiPath}/VEEMEE/winterwonderland/SantaQuest_User_Data");

                        if (!File.Exists(profilePath))
                        {
                            xmlDocument = new XDocument(
                                new XElement("profiles",
                                    new XElement("player",
                                        new XAttribute("psnid_id", psnid),
                                        new XElement("game",
                                            new XAttribute("game_id", "2010"), // Assign a default game ID
                                            from variable in variables
                                            select new XElement("variable",
                                                new XAttribute("name", variable["name"]),
                                                new XAttribute("type", variable["type"]),
                                                variable["value"]),
                                            from list in lists
                                            select new XElement("list",
                                                new XAttribute("name", list.Key.Item1),
                                                new XAttribute("type", list.Key.Item2),
                                                from value in list.Value
                                                select new XElement("value", value)
                                            )
                                        )
                                    )
                                )
                            );
                        }
                        else
                        {
                            // Load existing XML
                            xmlDocument = XDocument.Load(profilePath);

                            // Find or add player element
                            XElement playerElement = xmlDocument.Element("profiles")?.Element("player");
                            if (playerElement == null)
                            {
                                playerElement = new XElement("player", new XAttribute("psnid_id", psnid));
                                xmlDocument.Root.Add(playerElement);
                            }

                            // Find or add game element within player
                            XElement gameElement = playerElement.Element("game");
                            if (gameElement == null)
                            {
                                gameElement = new XElement("game", new XAttribute("game_id", "2010"));
                                playerElement.Add(gameElement);
                            }

                            // Update or add variables within game
                            foreach (var variable in variables)
                            {
                                XElement existingVariable = gameElement.Elements("variable")
                                    .FirstOrDefault(v => (string)v.Attribute("name") == variable["name"]);

                                if (existingVariable != null)
                                {
                                    // Update existing variable
                                    existingVariable.SetAttributeValue("type", variable["type"]);
                                    existingVariable.Value = variable["value"];
                                }
                                else
                                {
                                    // Add new variable
                                    gameElement.Add(new XElement("variable",
                                        new XAttribute("name", variable["name"]),
                                        new XAttribute("type", variable["type"]),
                                        variable["value"]));
                                }
                            }

                            // Update or add lists within game
                            foreach (var listEntry in lists)
                            {
                                (string listName, string listType) = listEntry.Key;
                                List<string> listValues = listEntry.Value;

                                XElement existingList = gameElement.Elements("list")
                                    .FirstOrDefault(l => (string)l.Attribute("name") == listName && (string)l.Attribute("type") == listType);

                                if (existingList == null)
                                {
                                    // Add new list if it doesn't exist
                                    existingList = new XElement("list",
                                        new XAttribute("name", listName),
                                        new XAttribute("type", listType));
                                    gameElement.Add(existingList);
                                }

                                // Add missing values to the list in order
#if NETCOREAPP2_0_OR_GREATER || NET472 || NET48 || NET481
                                HashSet<string> existingValues = existingList.Elements("value").Select(v => v.Value).ToHashSet();
#else
                                HashSet<string> existingValues = new HashSet<string>(
                                    existingList.Elements("value").Select(v => v.Value)
                                );
#endif
                                foreach (var value in listValues)
                                {
                                    if (!existingValues.Contains(value))
                                        existingList.Add(new XElement("value", value));
                                }
                            }
                        }

                        xmlDocument.Save(profilePath);

                        return xmlDocument.ToString();
                    default:
                        CustomLogger.LoggerAccessor.LogWarn($"[VEEMEE] - Profile_Handler:SetPOST - Client requested an unknown game! (Game: {game})");
                        break; ;
                }

                return "<xml></xml>";
            }

            return null;
        }

        public static string GetPOST(byte[] PostData, string ContentType, string apiPath)
        {
            string game = string.Empty;
            string psnid = string.Empty;

            if (ContentType == "application/x-www-form-urlencoded" && PostData != null)
            {
                var data = HTTPProcessor.ExtractAndSortUrlEncodedPOSTData(PostData);
                game = data["game"].First();
                if (string.IsNullOrEmpty(game))
                {
                    CustomLogger.LoggerAccessor.LogError("[VEEMEE] - Profile_Handler:GetPOST - Client tried to push invalid game! Invalidating request.");
                    return null;
                }
                psnid = data["psnid"].First();

                switch (game)
                {
                    case "winterwonderland":
                        if (File.Exists($"{apiPath}/VEEMEE/winterwonderland/SantaQuest_User_Data/{psnid}.xml"))
                            return File.ReadAllText($"{apiPath}/VEEMEE/winterwonderland/SantaQuest_User_Data/{psnid}.xml");
                        break;
                    default:
                        CustomLogger.LoggerAccessor.LogWarn($"[VEEMEE] - Profile_Handler:GetPOST - Client requested an unknown game! (Game: {game})");
                        break; ;
                }

                return "<xml></xml>";
            }

            return null;
        }

        // Helper function to extract index from key (e.g., name[1] => 1)
        private static int GetIndex(string key)
        {
            int start = key.IndexOf('[') + 1;
            return int.Parse(key.Substring(start, key.IndexOf(']') - start));
        }
    }
}
