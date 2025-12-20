using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace SSFWServer.Helpers.RegexHelper
{
    public class GUIDValidator
    {
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
