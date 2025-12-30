using NetCoreServer;
using System.Text.RegularExpressions;

namespace SSFWServer.Services
{
    public class KeepAliveService
    {
        public bool UpdateKeepAliveForClient(string absolutePath, HttpResponse res)
        {
            const byte GuidLength = 36;
            int index = absolutePath.IndexOf("/morelife");

            if (index != -1 && index > GuidLength) // Makes sure we have at least 36 chars available beforehand.
            {
                // Extract the substring between the last '/' and the morelife separator.
                string resultSessionId = absolutePath.Substring(index - GuidLength, GuidLength);

                if (Regex.IsMatch(resultSessionId, @"^[{(]?([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})[)}]?$"))
                {
                    SSFWUserSessionManager.UpdateKeepAliveTime(resultSessionId);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }
    }
}