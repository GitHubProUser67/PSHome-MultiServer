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
    public class UserData
    {
        private const string DefaultUserDataJson =
            "{" +
                "\"HeaderData\":{\"version_\":8}," +
                "\"PersonalData\":{" +
                    "\"deliverFarmPoint_\":0," +
                    "\"farmPoint_\":0," +
                    "\"wtPotValue_\":0," +
                    "\"wtPotLevel_\":1," +
                    "\"helpPoint_\":0," +
                    "\"lastLoginYMD_\":0," +
                    "\"lastLoginHMS_\":0," +
                    "\"lastBonusYMD_\":0," +
                    "\"lastBonusHMS_\":0," +
                    "\"lastCoopFarmYMD_\":0," +
                    "\"lastCoopFarmHMS_\":0," +
                    "\"commonFlag_\":\"00000000\"," +
                    "\"tutorialFlag_\":\"00000000\"" +
                "}," +
                "\"OperateCropData\":{" +
                    "\"managedID_\":0," +
                    "\"exp_\":0," +
                    "\"level_\":1," +
                    "\"rankDivide_\":0," +
                    "\"mutatiln_\":0" +
                "}," +
                "\"CropStockData\":{\"crops_\":{}}," +
                "\"CropPlaceData\":{\"sceneSets_\":{}}," +
                "\"CropBagData\":{\"expantion_\":0,\"sortKind_\":1}," +
                "\"ItemData\":{\"items_\":{}}," +
                "\"CollectionData\":{\"resultBits_\":[\"0000000000000000\",\"0000000000000000\"]}" +
            "}";

        public static string loadUserData(byte[] PostData, string ContentType, string workPath, string fulluripath, string method)
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
                LoggerAccessor.LogError($"[OSHABERI] - loadUserData: failed to parse userId: {ex}");
                return ErrorJson(-1);
            }

            if (string.IsNullOrEmpty(userId))
            {
                LoggerAccessor.LogError("[OSHABERI] - loadUserData: userId is empty");
                return ErrorJson(-2);
            }

            string safeId = SanitiseUserId(userId);
            string userDir = Path.Combine(workPath, "oshaberi", "userdata");
            Directory.CreateDirectory(userDir);
            string filePath = Path.Combine(userDir, safeId + ".json");

            string json;
            if (File.Exists(filePath))
            {
                json = File.ReadAllText(filePath, Encoding.UTF8);
                LoggerAccessor.LogInfo($"[OSHABERI] - loadUserData: loaded for '{userId}' ({json.Length} bytes)");
            }
            else
            {
                json = DefaultUserDataJson;
                File.WriteAllText(filePath, json, Encoding.UTF8);
                LoggerAccessor.LogInfo($"[OSHABERI] - loadUserData: created default data for new user '{userId}'");
            }

            return json;
        }
        public static string saveUserData(byte[] PostData, string ContentType, string workPath)
        {
            if (PostData == null || PostData.Length == 0)
            {
                LoggerAccessor.LogError("[OSHABERI] - saveUserData: empty POST body");
                return ErrorJson(-1);
            }

            string userId = string.Empty;
            string dataJson = string.Empty;
            string histJson = string.Empty;
            string evJson = string.Empty;

            try
            {
                string boundary = HTTPProcessor.ExtractBoundary(ContentType);
                using (MemoryStream ms = new MemoryStream(PostData))
                {
                    var form = MultipartFormDataParser.Parse(ms, boundary);
                    userId = form.GetParameterValue("u") ?? string.Empty;
                    dataJson = form.GetParameterValue("data") ?? string.Empty;
                    histJson = form.GetParameterValue("history") ?? string.Empty;
                    evJson = form.GetParameterValue("event") ?? string.Empty;
                    ms.Flush();
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[OSHABERI] - saveUserData: failed to parse multipart: {ex}");
                return ErrorJson(-1);
            }

            if (string.IsNullOrEmpty(userId))
            {
                LoggerAccessor.LogError("[OSHABERI] - saveUserData: userId field missing");
                return ErrorJson(-2);
            }

            if (string.IsNullOrEmpty(dataJson) || dataJson == "{}" || dataJson == "[]")
            {
                LoggerAccessor.LogInfo($"[OSHABERI] - saveUserData: empty delta for '{userId}', no-op");
                return SuccessJson();
            }

            string safeId = SanitiseUserId(userId);

            string userDir = Path.Combine(workPath, "oshaberi", "userdata");
            Directory.CreateDirectory(userDir);
            string filePath = Path.Combine(userDir, safeId + ".json");

            try
            {
                string existing = File.Exists(filePath)
                    ? File.ReadAllText(filePath, Encoding.UTF8)
                    : DefaultUserDataJson;

                string merged = MergeJsonObjects(existing, dataJson);
                File.WriteAllText(filePath, merged, Encoding.UTF8);
                LoggerAccessor.LogInfo($"[OSHABERI] - saveUserData: saved for '{userId}' ({merged.Length} bytes)");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[OSHABERI] - saveUserData: write error: {ex}");
                return ErrorJson(-3);
            }

            if (!string.IsNullOrEmpty(histJson) && histJson != "{}" && histJson != "[]")
            {
                try
                {
                    string histDir = Path.Combine(workPath, "oshaberi", "history");
                    Directory.CreateDirectory(histDir);
                    string histFile = Path.Combine(histDir, safeId + ".jsonl");
                    File.AppendAllText(histFile, $"{{\"ts\":\"{DateTime.UtcNow:o}\",\"data\":{histJson}}}\n", Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogWarn($"[OSHABERI] - saveUserData: history write failed for '{userId}': {ex}");
                }
            }

            if (!string.IsNullOrEmpty(evJson) && evJson != "{}" && evJson != "[]")
            {
                try
                {
                    string evDir = Path.Combine(workPath, "oshaberi", "eventdiff");
                    Directory.CreateDirectory(evDir);
                    string evFile = Path.Combine(evDir, safeId + ".json");
                    string existingEv = File.Exists(evFile)
                        ? File.ReadAllText(evFile, Encoding.UTF8)
                        : "{}";
                    File.WriteAllText(evFile, MergeJsonObjects(existingEv, evJson), Encoding.UTF8);
                }
                catch (Exception ex)
                {
                    LoggerAccessor.LogWarn($"[OSHABERI] - saveUserData: event diff write failed for '{userId}': {ex}");
                }
            }

            return SuccessJson();
        }
        private static string MergeJsonObjects(string baseJson, string patchJson)
        {
            try
            {
                using var baseDoc = JsonDocument.Parse(baseJson);
                using var patchDoc = JsonDocument.Parse(patchJson);

                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { SkipValidation = false }))
                {
                    writer.WriteStartObject();

                    foreach (var prop in baseDoc.RootElement.EnumerateObject())
                    {
                        if (patchDoc.RootElement.TryGetProperty(prop.Name, out _))
                            continue;
                        prop.WriteTo(writer);
                    }

                    foreach (var prop in patchDoc.RootElement.EnumerateObject())
                        prop.WriteTo(writer);

                    writer.WriteEndObject();
                }

                return Encoding.UTF8.GetString(stream.ToArray());
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogWarn($"[OSHABERI] - MergeJsonObjects parse failed, using patch directly: {ex.Message}");
                return patchJson;
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