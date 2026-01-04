using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace SSFWServer.Helpers.RegexHelper
{
    public class GUIDValidator
    {
        public static Regex RegexSessionValidator = new(@"^[{(]?([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})[)}]?$");
        public static Regex VersionFilter = new(@"\d{6}");

#if NET7_0_OR_GREATER
        [GeneratedRegex("[0-9a-fA-F]{8}-[0-9a-fA-F]{8}-[0-9a-fA-F]{8}-[0-9a-fA-F]{8}")]
        private static partial Regex UUIDRegex();
#endif

        public static Match RegexSceneIdValidMatch(string sceneId)
        {
#if NET7_0_OR_GREATER
            Match match = UUIDRegex().Match(sceneid);
#else
            Match match = new Regex(@"[0-9a-fA-F]{8}-[0-9a-fA-F]{8}-[0-9a-fA-F]{8}-[0-9a-fA-F]{8}").Match(sceneId);
#endif
           return match;
        }

        public static string FixJsonValues(string json)
        {
            // Match GUID portion with 8-8-8-8 format (fix unquoted GUIDs)
            json = Regex.Replace(json, @"(?<![""\w])(\b[a-fA-F0-9]{8}-[a-fA-F0-9]{8}-[a-fA-F0-9]{8}-[a-fA-F0-9]{8}\b)(?![""\w])", "\"$1\"");

            // Match unquoted words that are not true/false/null (i.e., should be strings)
            json = Regex.Replace(json, @"(?<=:\s*)([A-Za-z_][A-Za-z0-9_]*)(?=\s*[,\}])", "\"$1\"");

            // Parse and re-serialize to ensure it's valid JSON
            try
            {
                return JsonConvert.SerializeObject(JsonConvert.DeserializeObject<JObject>(json), Formatting.Indented);
            }
            catch (JsonReaderException ex)
            {
                CustomLogger.LoggerAccessor.LogError("[GUIDValidator] : Invalid JSON format: " + ex.Message);
            }
            catch (Exception ex)
            {
                CustomLogger.LoggerAccessor.LogError("[GUIDValidator] : Unknown error occurred: " + ex.Message);
            }

            return json;
        }
    }
}
