using System;
using System.IO;
using System.Text;
using System.Text.Json;
using MultiServerLibrary.HTTP;
using CustomLogger;
using HttpMultipartParser;




#if !NETFRAMEWORK
using System.Web;
#endif

namespace WebAPIService.GameServices.PSHOME.OSHABERI
{
    public class EventRecords
    {
        private const string DefaultEventJson =
            "{\"ver_\":1,\"records_\":{}}";

        public static string loadUserEvent(byte[] PostData, string ContentType, string workPath, string fulluripath, string method)
        {
            string userId = string.Empty;

            try
            {
                if (method == "GET" || ContentType == null)
                {
#if NETFRAMEWORK
                    userId = HTTPProcessor.GetQueryParameters(fulluripath)["u"];
#else
                    userId = HttpUtility.ParseQueryString(new Uri("http://x" + fulluripath).Query).Get("u");
#endif
                }
                else
                {
                    string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                    using (MemoryStream ms = new MemoryStream(PostData))
                    {
                        userId = MultipartFormDataParser.Parse(ms, boundary).GetParameterValue("u") ?? string.Empty;
                        ms.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[OSHABERI] - loadUserEvent: parse error: {ex}");
                return DefaultEventJson;
            }

            if (string.IsNullOrEmpty(userId))
            {
                LoggerAccessor.LogWarn("[OSHABERI] - loadUserEvent: no userId, returning default");
                return DefaultEventJson;
            }

            string safeId = SanitiseUserId(userId);
            string evDir = Path.Combine(workPath, "oshaberi", "events");
            Directory.CreateDirectory(evDir);
            string filePath = Path.Combine(evDir, safeId + ".json");

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath, Encoding.UTF8);
                LoggerAccessor.LogInfo($"[OSHABERI] - loadUserEvent: loaded for '{userId}' ({json.Length} bytes)");
                return json;
            }

            File.WriteAllText(filePath, DefaultEventJson, Encoding.UTF8);
            LoggerAccessor.LogInfo($"[OSHABERI] - loadUserEvent: created default for new user '{userId}'");
            return DefaultEventJson;
        }

        public static string saveUserEvent(byte[] PostData, string ContentType, string workPath)
        {
            if (PostData == null || PostData.Length == 0)
            {
                LoggerAccessor.LogError("[OSHABERI] - saveUserEvent: empty POST body");
                return ErrorJson(-1);
            }

            string userId = string.Empty;
            string recordsJson = string.Empty;

            try
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var form = MultipartFormDataParser.Parse(ms, boundary);
                    userId = form.GetParameterValue("u") ?? string.Empty;
                    recordsJson = form.GetParameterValue("records") ?? string.Empty;
                    ms.Flush();
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[OSHABERI] - saveUserEvent: parse error: {ex}");
                return ErrorJson(-1);
            }

            if (string.IsNullOrEmpty(userId))
            {
                LoggerAccessor.LogError("[OSHABERI] - saveUserEvent: missing userId");
                return ErrorJson(-2);
            }

            if (string.IsNullOrEmpty(recordsJson) || recordsJson == "{}")
            {
                LoggerAccessor.LogInfo($"[OSHABERI] - saveUserEvent: empty records for '{userId}', no-op");
                return SuccessJson();
            }

            string safeId = SanitiseUserId(userId);
            string evDir = Path.Combine(workPath, "oshaberi", "events");
            Directory.CreateDirectory(evDir);
            string filePath = Path.Combine(evDir, safeId + ".json");

            try
            {
                string existing = File.Exists(filePath)
                    ? File.ReadAllText(filePath, Encoding.UTF8)
                    : DefaultEventJson;
                string merged = MergeEventRecords(existing, recordsJson);
                File.WriteAllText(filePath, merged, Encoding.UTF8);
                LoggerAccessor.LogInfo($"[OSHABERI] - saveUserEvent: saved for '{userId}' ({merged.Length} bytes)");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[OSHABERI] - saveUserEvent: write error: {ex}");
                return ErrorJson(-3);
            }

            return SuccessJson();
        }

        private static string MergeEventRecords(string existing, string incoming)
        {
            try
            {
                using var incomingDoc = JsonDocument.Parse(incoming);
                if (incomingDoc.RootElement.TryGetProperty("ver_", out _))
                    return incoming;

                using var existingDoc = JsonDocument.Parse(existing);

                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream))
                {
                    writer.WriteStartObject();

                    foreach (var prop in existingDoc.RootElement.EnumerateObject())
                    {
                        if (prop.Name == "records_")
                            continue;
                        prop.WriteTo(writer);
                    }

                    writer.WritePropertyName("records_");
                    writer.WriteStartObject();

                    JsonElement existingRecords;
                    bool hasExisting = existingDoc.RootElement.TryGetProperty("records_", out existingRecords);

                    if (hasExisting)
                    {
                        foreach (var rec in existingRecords.EnumerateObject())
                        {
                            if (incomingDoc.RootElement.TryGetProperty(rec.Name, out _))
                                continue;
                            rec.WriteTo(writer);
                        }
                    }

                    foreach (var rec in incomingDoc.RootElement.EnumerateObject())
                        rec.WriteTo(writer);

                    writer.WriteEndObject();
                    writer.WriteEndObject(); 
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogWarn($"[OSHABERI] - MergeEventRecords failed, using incoming: {ex.Message}");
                return incoming;
            }
        }

        private static string SanitiseUserId(string userId)
        {
            var sb = new StringBuilder(userId.Length);
            foreach (char c in userId)
                sb.Append(char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '.' ? c : '_');
            return sb.Length > 0 ? sb.ToString() : "unknown";
        }

        private static string SuccessJson() =>
            "{\"result\":1,\"error_no\":0}";

        private static string ErrorJson(int code) =>
            $"{{\"result\":0,\"error_no\":{code}}}";
    }
}