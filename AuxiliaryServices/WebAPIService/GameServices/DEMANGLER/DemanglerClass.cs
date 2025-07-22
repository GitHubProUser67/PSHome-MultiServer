using CustomLogger;
using System.Collections.Generic;
using System.Text;

namespace WebAPIService.GameServices.DEMANGLER
{
    public class DemanglerClass
    {
        public static (string, string)? ProcessDemanglerRequest(IDictionary<string, string> QueryParameters, string serverIp, string absolutepath, byte[] PostData)
        {
            switch (absolutepath)
            {
                case "/getPeerAddress":
                    if (QueryParameters.Count > 0 && QueryParameters.ContainsKey("myIP") && QueryParameters.ContainsKey("myPort"))
                        return ($"status=probe\n\n\ntargetIP-1={serverIp}\ntargetPort-1=10000\nsourcePort-1={QueryParameters["myPort"]}\ntag-1=probeg:0005581120:0\nsendCount-1=2\n\n", "text/plain");
                    break;
                case "/connectionStatus":
                    string connectionInfos = Encoding.UTF8.GetString(PostData);
                    LoggerAccessor.LogWarn($"[DemanglerClass] - connectionStatus was sent, details: {connectionInfos}");
                    return (connectionInfos, "application/x-www-form-urlencoded");
            }

            return null;
        }
    }
}
