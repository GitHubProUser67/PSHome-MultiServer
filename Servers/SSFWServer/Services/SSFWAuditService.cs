using CastleLibrary.Sony.SSFW;
using CustomLogger;
using Newtonsoft.Json;
using System.Text;

namespace SSFWServer.Services
{
    public class SSFWAuditService
    {
        private string? sessionid;
        private string? env;
        private string? key;

        public SSFWAuditService(string sessionid, string env, string? key)
        {
            this.sessionid = sessionid;
            this.env = env;
            this.key = key;
        }

        public void HandleAuditService(string absolutepath, byte[] buffer)
        {
            string fileNameGUID = GuidGenerator.SSFWGenerateGuid(sessionid, env);
            string auditLogPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}/{env}";
            try
            {
                Directory.CreateDirectory(auditLogPath);

                var obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString(buffer));
                File.WriteAllText($"{auditLogPath}/{fileNameGUID}.json", JsonConvert.SerializeObject(obj, Formatting.Indented));
#if DEBUG
                LoggerAccessor.LogInfo($"[SSFW] : Audit event posted: {fileNameGUID}");
#endif
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SSFW] - SSFWAuditService HandleAuditService ERROR: \n{ex}");
            }
        }
    }
}