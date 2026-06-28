using System.Text;
using CastleLibrary.S0ny.SSFW;
using CustomLogger;
using NetCoreServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SSFWServer.Services
{
    public class AuditService(string sessionid, string env, string? key)
    {
        private readonly string? sessionid = sessionid;
        private readonly string? env = env;
        private readonly string? key = key;

        public string HandleAuditService(string absolutepath, byte[] buffer, HttpRequest request)
        {
            var fileNameGUID = GuidGenerator.SSFWGenerateGuid(sessionid, env);
            var personIdToCompare = SSFWUserSessionManager.GetIdBySessionId(sessionid);
            var auditLogPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}";

            switch (request.Method)
            {
                case "PUT":
                    try
                    {
                        Directory.CreateDirectory(auditLogPath);

                        File.WriteAllText(
                            $"{auditLogPath}/{fileNameGUID}.json",
                            Encoding.UTF8.GetString(buffer)
                        );
#if DEBUG
                        LoggerAccessor.LogInfo(
                            $"[SSFW] AuditService - HandleAuditService Audit event log posted: {fileNameGUID}"
                        );
#endif
                        return $"{{ \"result\": 0 }}";
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError(
                            $"[SSFW] AuditService - HandleAuditService ERROR caught: \n{ex}"
                        );
                        return $"{{ \"result\": -1 }}";
                    }
                case "GET":

                    if (absolutepath.Contains("counts"))
                    {
                        var files = Directory.GetFiles(auditLogPath.Replace("/counts", ""));

                        var newFileMatchingEntry = string.Empty;

                        List<string> listOfEventsByUser = [];
                        var userEventTotal = 1;
                        var idxTotal = 0;
                        foreach (var fileToRead in files)
                        {
                            var fileContents = File.ReadAllText(fileToRead);
                            var jsonContents = JsonConvert.DeserializeObject<JObject>(fileContents);
                            if (fileContents != null)
                            {
                                var mainFile = JObject.Parse(fileContents);

                                var userNameInEvent = mainFile["owner"];

                                if (personIdToCompare == (string?)userNameInEvent)
                                {
                                    var fileName = Path.GetFileNameWithoutExtension(fileToRead);
                                    newFileMatchingEntry =
                                        files.Length == userEventTotal
                                            ? $"\"{fileName}\""
                                            : $"\"{fileName}\",";
                                }
                                listOfEventsByUser.Add(newFileMatchingEntry);
                                idxTotal++;
                            }
                        }
#if DEBUG
                        LoggerAccessor.LogInfo(
                            $"[SSFW] AuditService - HandleAuditService returning count list of logs for player {personIdToCompare}"
                        );
#endif
                        return $"{{ \"count\": {idxTotal}, \"events\": {{ {string.Join("", listOfEventsByUser)} }} }}";
                    }
                    else if (absolutepath.Contains("object"))
                    {
#if DEBUG
                        LoggerAccessor.LogInfo(
                            "[SSFW] AuditService - HandleAuditService Event log get "
                                + auditLogPath.Replace("/object", "")
                                + ".json"
                        );
#endif
                        return File.ReadAllText(auditLogPath.Replace("/object", "") + ".json");
                    }
                    break;
                default:
                    LoggerAccessor.LogError(
                        $"[SSFW] AuditService - HandleAuditService Method {request.Method} unhandled!"
                    );
                    return $"{{ \"result\": -1 }}";
            }

            return string.Empty;
        }
    }
}
