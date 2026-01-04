using CastleLibrary.Sony.SSFW;
using CustomLogger;
using NetCoreServer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace SSFWServer.Services
{
    public class AuditService
    {
        private string? sessionid;
        private string? env;
        private string? key;

        public AuditService(string sessionid, string env, string? key)
        {
            this.sessionid = sessionid;
            this.env = env;
            this.key = key;
        }

        public string HandleAuditService(string absolutepath, byte[] buffer, HttpRequest request)
        {
            string fileNameGUID = GuidGenerator.SSFWGenerateGuid(sessionid, env);
            string? personIdToCompare = SSFWUserSessionManager.GetIdBySessionId(sessionid);
            string auditLogPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}";

            switch (request.Method)
            {
                case "PUT":
                    try
                    {
                        Directory.CreateDirectory(auditLogPath);

                        File.WriteAllText($"{auditLogPath}/{fileNameGUID}.json", Encoding.UTF8.GetString(buffer));
#if DEBUG
                        LoggerAccessor.LogInfo($"[SSFW] AuditService - HandleAuditService Audit event log posted: {fileNameGUID}");
#endif
                        return $"{{ \"result\": 0 }}";
                    }
                    catch (Exception ex)
                    {
                        LoggerAccessor.LogError($"[SSFW] AuditService - HandleAuditService ERROR caught: \n{ex}");
                        return $"{{ \"result\": -1 }}";
                    }
                case "GET":

                    if(absolutepath.Contains("counts"))
                    {
                        var files = Directory.GetFiles(auditLogPath.Replace("/counts", ""));

                        string newFileMatchingEntry = string.Empty;

                        List<string> listOfEventsByUser = new();
                        int userEventTotal = 1;
                        int idxTotal = 0;
                        foreach (string fileToRead in files)
                        {
                            string fileContents = File.ReadAllText(fileToRead);
                            JObject? jsonContents = JsonConvert.DeserializeObject<JObject>(fileContents); 
                            if(fileContents != null )
                            { 
                                JObject mainFile = JObject.Parse(fileContents);

                                var userNameInEvent = mainFile["owner"];

                                if (personIdToCompare == (string?)userNameInEvent)
                                {
                                    string fileName = Path.GetFileNameWithoutExtension(fileToRead);
                                    if(files.Length == userEventTotal)
                                    {
                                        newFileMatchingEntry = $"\"{fileName}\"";
                                    } else 
                                        newFileMatchingEntry = $"\"{fileName}\",";
                                }
                                listOfEventsByUser.Add(newFileMatchingEntry);
                                idxTotal++;
                            }
                        }
#if DEBUG
                        LoggerAccessor.LogInfo($"[SSFW] AuditService - HandleAuditService returning count list of logs for player {personIdToCompare}");
#endif
                        return $"{{ \"count\": {idxTotal}, \"events\": {{ {string.Join("", listOfEventsByUser)} }} }}";
                    } else if(absolutepath.Contains("object"))
                    {
#if DEBUG
                        LoggerAccessor.LogInfo("[SSFW] AuditService - HandleAuditService Event log get " + auditLogPath.Replace("/object", "") + ".json");
#endif
                        return File.ReadAllText(auditLogPath.Replace("/object", "") + ".json");
                    }
                    break;
                default:
                    LoggerAccessor.LogError($"[SSFW] AuditService - HandleAuditService Method {request.Method} unhandled!");
                    return $"{{ \"result\": -1 }}";
            }

            return string.Empty;
        }
    }
}