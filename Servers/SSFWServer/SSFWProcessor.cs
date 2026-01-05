using CustomLogger;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using MultiServerLibrary.SSL;
using NetCoreServer;
using NetCoreServer.CustomServers;
using SSFWServer.Helpers.FileHelper;
using SSFWServer.Services;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace SSFWServer
{
    public class SSFWProcessor
    {
        private const string LoginGUID = "bb88aea9-6bf8-4201-a6ff-5d1f8da0dd37";

        // Defines a list of web-related file extensions
        private static readonly HashSet<string> allowedWebExtensions = new(StringComparer.InvariantCultureIgnoreCase)
        {
            ".html", ".htm", ".cgi", ".css", ".js", ".svg", ".gif", ".ico", ".woff", ".woff2", ".ttf", ".eot"
        };

        private static readonly ushort[] _ports = new ushort[] { NetworkPorts.Http.TcpAux, 10443 };

        private NCHTTPServer? _Server;
        private static readonly ConcurrentDictionary<string, string> LayoutGetOverrides = new();

        private readonly string _certpath;
        private readonly string _certpass;

        private static string? _legacykey;

        public SSFWProcessor(string certpath, string certpass, string? locallegacykey)
        {
            _certpath = certpath;
            _certpass = certpass;
            _legacykey = locallegacykey;
        }

        private static (string HeaderIndex, string HeaderItem)[] CollectHeaders(HttpRequest request)
        {
            int headerindex = (int)request.Headers; // There is a slight mistake in netcoreserver, where the index is long, and the parser is int
                                                    // So we accomodate that with a cast.

            (string HeaderIndex, string HeaderItem)[] CollectHeader = new (string, string)[headerindex];

            for (int i = 0; i < headerindex; i++)
                CollectHeader[i] = request.Header(i);

            return CollectHeader;
        }

        private static string GetHeaderValue((string HeaderIndex, string HeaderItem)[] headers, string requestedHeaderIndex, bool caseSensitive = true)
        {
            if (headers.Length > 0)
            {
                const string pattern = @"^(.*?):\s(.*)$"; // Make a GITHUB ticket for netcoreserver, the header tuple can get out of sync with null values, we try to mitigate the problem.

                foreach ((string HeaderIndex, string HeaderItem) in headers)
                {
                    if (caseSensitive ? HeaderIndex.Equals(requestedHeaderIndex) : HeaderIndex.Equals(requestedHeaderIndex, StringComparison.InvariantCultureIgnoreCase))
                        return HeaderItem;
                    else
                    {
                        try
                        {
                            Match match = Regex.Match(HeaderItem, pattern);

                            if (caseSensitive ? HeaderItem.Contains(requestedHeaderIndex) : HeaderItem.Contains(requestedHeaderIndex, StringComparison.InvariantCultureIgnoreCase)
                                && match.Success) // Make a GITHUB ticket for netcoreserver, the header tuple can get out of sync with null values, we try to mitigate the problem.
                                return match.Groups[2].Value;
                        }
                        catch
                        {

                        }
                    }
                }
            }

            return string.Empty; // Return empty string if the header index is not found, why not null, because in this case it prevents us
                                 // from doing extensive checks everytime we want to display the User-Agent in particular.
        }

        private static string? ExtractBeforeFirstDot(string input)
        {
            if (string.IsNullOrEmpty(input))
                return null;

            int dotIndex = input.IndexOf('.');
            if (dotIndex == -1)
                return null;

            return input[..dotIndex];
        }

        private static bool IsSSFWRegistered(string? sessionid)
        {
            if (string.IsNullOrEmpty(sessionid))
                return false;

            return !string.IsNullOrEmpty(SSFWUserSessionManager.GetIdBySessionId(sessionid));
        }

        public void StartSSFW()
        {
            if (_Server == null)
                _Server = new NCHTTPServer();

            // Ports are hardcoded, ideally move them to a proper loop. 
            _Server.Start(
                new HttpSSFWServer(IPAddress.Any, _ports[0]),
                new SSFWServer(new SslContext(SslProtocols.Tls12, new X509Certificate2(_certpath, _certpass), MyRemoteCertificateValidationCallback) { ClientCertificateRequired = true }, IPAddress.Any, _ports[1])
                );

            LoggerAccessor.LogInfo($"[SSFWProcessor] - Server started on ports {string.Join(", ", _ports)}...");
        }

        public void StopSSFW()
        {
            _Server?.Stop();
        }

        private static HttpResponse SSFWRequestProcess(HttpRequest request, HttpResponse Response)
        {
            try
            {
                string absolutepath = HTTPProcessor.DecodeUrl(request.Url);

                if (!string.IsNullOrEmpty(absolutepath))
                {
                    bool isApiRequest = false;
                    string sessionid;

                    (string HeaderIndex, string HeaderItem)[] Headers = CollectHeaders(request);

                    string host = GetHeaderValue(Headers, "host", false);

                    string? encoding = null;
                    string UserAgent = GetHeaderValue(Headers, "User-Agent", false);
                    string cacheControl = GetHeaderValue(Headers, "Cache-Control");

                    if (SSFWServerConfiguration.EnableHTTPCompression && (string.IsNullOrEmpty(cacheControl) || cacheControl != "no-transform"))
                        encoding = GetHeaderValue(Headers, "Accept-Encoding");

                    // Split the URL into segments
                    string[] segments = absolutepath.Trim('/').Split('/');

                    // Combine the folder segments into a directory path
                    string directoryPath = Path.Combine(SSFWServerConfiguration.SSFWStaticFolder, string.Join("/", segments.Take(segments.Length - 1).ToArray()));

                    // Process the request based on the HTTP method
                    string filePath = Path.Combine(SSFWServerConfiguration.SSFWStaticFolder, absolutepath[1..]);
#if DEBUG
                    LoggerAccessor.LogInfo($"[SSFWProcessor] - Home Client Requested the SSFW Server with URL : {request.Method} {absolutepath} (Details: \n{{ \"NetCoreServer\":" + JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true })
                        + (Headers.Length > 0 ? $", \"Headers\":{JsonSerializer.Serialize(Headers.ToDictionary(header => header.HeaderIndex, header => header.HeaderItem), new JsonSerializerOptions { WriteIndented = true })} }} )" : "} )"));
#else
                    LoggerAccessor.LogInfo($"[SSFWProcessor] - Home Client Requested the SSFW Server with URL : {request.Method} {absolutepath}");
#endif
                    if (!string.IsNullOrEmpty(UserAgent))
                    {
                        // PSHome
                        if (UserAgent.Contains("PSHome"))
                        {
                            isApiRequest = true;

                            string? env = ExtractBeforeFirstDot(host);
                            sessionid = GetHeaderValue(Headers, "X-Home-Session-Id");

                            if (string.IsNullOrEmpty(env) || !SSFWServerConfiguration.homeEnvs.Contains(env))
                                env = "cprod";

                            // Instantiate services
                            AchievementService achievementService = new(sessionid, env, _legacykey);
                            AuditService auditService = new(sessionid, env, _legacykey);
                            AvatarService avatarService = new();
                            FriendsService friendsService = new(sessionid, env, _legacykey);
                            KeepAliveService keepAliveService = new();
                            RewardsService rewardSvc = new(_legacykey);
                            LayoutService layout = new(_legacykey);
                            AvatarLayoutService avatarLayout = new(sessionid, _legacykey);
                            ClanService clanService = new(sessionid);
                            PlayerLookupService playerLookupService = new();
                            SaveDataService saveDataService = new();
                            TradingService tradingService = new(sessionid, env, _legacykey);

                            switch (request.Method)
                            {
                                case "GET":

                                    if (IsSSFWRegistered(sessionid))
                                    {
                                        #region LayoutService
                                        if (absolutepath.Contains($"/LayoutService/{env}/person/"))
                                        {
                                            string? res = null;

                                            if (LayoutGetOverrides.ContainsKey(sessionid))
                                                LayoutGetOverrides.Remove(sessionid, out res);
                                            else
                                                res = layout.HandleLayoutServiceGET(directoryPath, filePath);

                                            if (res == null)
                                            {
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody();
                                            }
                                            else if (res == string.Empty)
                                            {
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.NotFound);
                                                Response.SetBody();
                                            }
                                            else
                                                Response.MakeGetResponse(res, "application/json");
                                            break;
                                        }
                                        #endregion

                                        #region AdminObjectService
                                        else if (absolutepath.Contains("/AdminObjectService/start"))
                                        {
                                            Response.Clear();
                                            if (new AdminObjectService(sessionid, _legacykey).HandleAdminObjectService(UserAgent))
                                                Response.SetBegin((int)HttpStatusCode.OK);
                                            else
                                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                            Response.SetBody();
                                            break;
                                        }
                                        #endregion

                                        #region SaveDataService
                                        else if (absolutepath.Contains($"/SaveDataService/{env}/{segments.LastOrDefault()}"))
                                        {
                                            string? res = saveDataService.DebugGetFileList(directoryPath, segments.LastOrDefault()); ;
                                            if (res != null)
                                                Response.MakeGetResponse(res, "application/json");
                                            else
                                                Response.MakeErrorResponse();
                                            break;
                                        }
                                        #endregion

                                        #region PlayerLookup Service
                                        //Doesn't pass in SessionId!!
                                        else if (absolutepath.Contains($"/{LoginGUID}/person/byDisplayName"))
                                        {
                                            var res = playerLookupService.HandlePlayerLookupService(absolutepath);
                                            if (!string.IsNullOrEmpty(res))
                                                Response.MakeGetResponse(res, "application/json");
                                            else
                                                Response.MakeErrorResponse(404, "Not Found");
                                            break;
                                        }
                                        #endregion

                                        #region DEBUG AchievementService 
                                        else if (absolutepath.Contains($"/AchievementService/{SSFWUserSessionManager.GetIdBySessionId(sessionid)}"))
                                        {
                                            var res = achievementService.HandleAchievementService(absolutepath);
                                            if (!string.IsNullOrEmpty(res))
                                                Response.MakeGetResponse(res, "application/json");
                                            else
                                                Response.MakeErrorResponse(404, "Not Found");
                                            break;
                                        }
                                        #endregion

                                        #region DEBUG AuditService
                                        if (absolutepath.Contains($"/AuditService/log/{env}/{SSFWUserSessionManager.GetIdBySessionId(sessionid)}/counts")
                                            || absolutepath.Contains($"/AuditService/log/{env}/{SSFWUserSessionManager.GetIdBySessionId(sessionid)}/object"))
                                        {
                                            var res = auditService.HandleAuditService(absolutepath, Array.Empty<byte>(), request);

                                            if (!string.IsNullOrEmpty(res))
                                                Response.MakeGetResponse(res, "application/json");
                                            else
                                                Response.MakeErrorResponse(404, "Not Found");
                                            break;
                                        }
                                        #endregion

                                        #region RewardService Inventory System
                                        //First check if this is a Inventory request
                                        if (absolutepath.Contains($"/RewardsService/") && absolutepath.Contains("counts"))
                                        {
                                            //Detect if existing inv exists
                                            if (File.Exists(filePath + ".json"))
                                            {
                                                string? res = FileHelper.ReadAllText(filePath + ".json", _legacykey);

                                                if (!string.IsNullOrEmpty(res))
                                                {
                                                    if (GetHeaderValue(Headers, "Accept") == "application/json")
                                                        Response.MakeGetResponse(res, "application/json");
                                                    else
                                                        Response.MakeGetResponse(res);
                                                }
                                                else
                                                    Response.MakeErrorResponse();
                                            }
                                            else //fallback default 
                                                Response.MakeGetResponse(@"{ ""00000000-00000000-00000000-00000001"": 1 } ", "application/json");
                                            break;
                                        }
                                        //Check for specifically the Tracking GUID
                                        else if (absolutepath.Contains($"/RewardsService/") && absolutepath.Contains("object/00000000-00000000-00000000-00000001"))
                                        {
                                            //Detect if existing inv exists
                                            if (File.Exists(filePath + ".json"))
                                            {
                                                string? res = FileHelper.ReadAllText(filePath + ".json", _legacykey);

                                                if (!string.IsNullOrEmpty(res))
                                                {
                                                    if (GetHeaderValue(Headers, "Accept") == "application/json")
                                                        Response.MakeGetResponse(res, "application/json");
                                                    else
                                                        Response.MakeGetResponse(res);
                                                }
                                                else
                                                    Response.MakeErrorResponse();
                                            }
                                            else //fallback default 
                                            {
#if DEBUG
                                                LoggerAccessor.LogWarn($"[SSFWProcessor] : {UserAgent} Non-existent inventories detected, using defaults!");
#endif
                                                if (absolutepath.Contains("p4t-cprod"))
                                                {
                                                    #region Quest for Greatness
                                                    Response.MakeGetResponse(@"{
                                                      ""result"": 0,
                                                      ""rewards"": {
                                                        ""00000000-00000000-00000000-00000001"": {
                                                          ""migrated"": 1,
                                                          ""_id"": ""1""
                                                        }
                                                      }
                                                    }", "application/json");
                                                    #endregion
                                                }
                                                else
                                                {
                                                    #region Pottermore
                                                    Response.MakeGetResponse(@"{
                                                      ""result"": 0,
                                                      ""rewards"": [
                                                        {
                                                          ""00000000-00000000-00000000-00000001"": {
                                                          ""boost"": ""AQ=="",
                                                          ""_id"": ""tracking""
                                                          }
                                                        }
                                                      ]
                                                    }", "application/json");
                                                    #endregion
                                                }
                                                break;
                                            }
                                        }
                                        #endregion

                                        #region ClanService
                                        else if (absolutepath.Contains($"/ClanService/{env}/clan/"))
                                            clanService.HandleClanDetailsService(request, Response, absolutepath);
                                        #endregion

                                        #region File return JSON, bin, jpeg
                                        else if (File.Exists(filePath + ".json"))
                                        {
                                            string? res = FileHelper.ReadAllText(filePath + ".json", _legacykey);

                                            if (!string.IsNullOrEmpty(res))
                                            {
                                                if (GetHeaderValue(Headers, "Accept") == "application/json")
                                                    Response.MakeGetResponse(res, "application/json");
                                                else
                                                    Response.MakeGetResponse(res);
                                            }
                                            else
                                                Response.MakeErrorResponse();
                                        }
                                        else if (File.Exists(filePath + ".bin"))
                                        {
                                            byte[]? res = FileHelper.ReadAllBytes(filePath + ".bin", _legacykey);

                                            if (res != null)
                                                Response.MakeGetResponse(res, "application/octet-stream");
                                            else
                                                Response.MakeErrorResponse();
                                        }
                                        else if (File.Exists(filePath + ".jpeg"))
                                        {
                                            byte[]? res = FileHelper.ReadAllBytes(filePath + ".jpeg", _legacykey);

                                            if (res != null)
                                                Response.MakeGetResponse(res, "image/jpeg");
                                            else
                                                Response.MakeErrorResponse();
                                        }
                                        else
                                        {
                                            LoggerAccessor.LogWarn($"[SSFWProcessor] : {UserAgent} Requested a non-existent file - {filePath}");
                                            Response.Clear();
                                            Response.SetBegin((int)HttpStatusCode.NotFound);
                                            Response.SetBody();
                                        }
                                        #endregion
                                    }

                                    #region SaveData AvatarService
                                    else if (absolutepath.Contains($"/SaveDataService/avatar/{env}/") 
                                        && absolutepath.EndsWith(".jpg"))
                                    {
                                        byte[]? res = avatarService.HandleAvatarService(filePath, _legacykey);
                                        if (res != null)
                                            Response.MakeGetResponse(res, "image/jpg");
                                        else
                                            Response.MakeErrorResponse(404, "Not Found");
                                    }
                                    else
                                    {
                                        Response.Clear();
                                        Response.SetBegin((int)HttpStatusCode.Forbidden);
                                        Response.SetBody();
                                    }
                                    break;
                                #endregion

                                case "POST":

                                    if (request.BodyLength <= Array.MaxLength)
                                    {
                                        #region IdentityService Login
                                        byte[] postbuffer = request.BodyBytes;
                                        if (absolutepath == $"/{LoginGUID}/login/token/psn")
                                        {
                                            string? XHomeClientVersion = GetHeaderValue(Headers, "X-HomeClientVersion");
                                            string? generalsecret = GetHeaderValue(Headers, "general-secret");

                                            if (!string.IsNullOrEmpty(XHomeClientVersion) && !string.IsNullOrEmpty(generalsecret))
                                            {
                                                IdentityService login = new(XHomeClientVersion, generalsecret, XHomeClientVersion.Replace(".", string.Empty).PadRight(6, '0'), GetHeaderValue(Headers, "x-signature"), _legacykey);
                                                string? result = login.HandleLogin(postbuffer, env);
                                                if (!string.IsNullOrEmpty(result))
                                                {
                                                    Response.Clear();
                                                    Response.SetBegin((int)HttpStatusCode.Created);
                                                    Response.SetContentType("application/json");
                                                    Response.SetBody(result, encoding);
                                                }
                                                else
                                                    Response.MakeErrorResponse();
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody();
                                            }
                                        }
                                        #endregion

                                        #region PING KeepAlive Service
                                        else if (absolutepath.Contains("/morelife") && !string.IsNullOrEmpty(GetHeaderValue(Headers, "x-signature")))
                                        {
                                            if (KeepAliveService.UpdateKeepAliveForClient(absolutepath))
                                                Response.MakeOkResponse(); // This doesn't even need a empty array, simply 200 Status is enough.
                                            //Response.MakeGetResponse("{}", "application/json"); 
                                            else
                                                Response.MakeErrorResponse(403);
                                        }
                                        #endregion

                                        else if (IsSSFWRegistered(sessionid))
                                        {
                                            #region AvatarLayoutService
                                            if (absolutepath.Contains($"/AvatarLayoutService/{env}/"))
                                            {
                                                Response.Clear();
                                                if (avatarLayout.HandleAvatarLayout(postbuffer, directoryPath, filePath, absolutepath, false))
                                                    Response.SetBegin((int)HttpStatusCode.OK);
                                                else
                                                    Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody();
                                            }
                                            #endregion

                                            #region LayoutService
                                            else if (absolutepath.Contains($"/LayoutService/{env}/person/"))
                                            {
                                                Response.Clear();
                                                if (layout.HandleLayoutServicePOST(postbuffer, directoryPath, absolutepath))
                                                    Response.SetBegin((int)HttpStatusCode.OK);
                                                else
                                                    Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody();
                                            }
                                            #endregion

                                            #region RewardsService
                                            else if (absolutepath.Contains($"/RewardsService/{env}/rewards/"))
                                                Response.MakeGetResponse(rewardSvc.HandleRewardServicePOST(postbuffer, directoryPath, filePath, absolutepath), "application/json");
                                            else if (absolutepath.Contains($"/RewardsService/trunks-{env}/trunks/") && absolutepath.Contains("/setpartial"))
                                            {
                                                rewardSvc.HandleRewardServiceTrunksPOST(postbuffer, directoryPath, filePath, absolutepath, env, SSFWUserSessionManager.GetIdBySessionId(sessionid));
                                                Response.MakeOkResponse();
                                            }
                                            else if (absolutepath.Contains($"/RewardsService/trunks-{env}/trunks/") && absolutepath.Contains("/set"))
                                            {
                                                rewardSvc.HandleRewardServiceTrunksEmergencyPOST(postbuffer, directoryPath, absolutepath);
                                                Response.MakeOkResponse();
                                            }
                                            else if (
                                                absolutepath.Contains($"/RewardsService/pm_{env}_cards/")
                                                || absolutepath.Contains($"/RewardsService/pmcards/")
                                                || absolutepath.Contains($"/RewardsService/p4t-{env}/"))
                                                Response.MakeGetResponse(rewardSvc.HandleRewardServiceInvPOST(postbuffer, directoryPath, filePath, absolutepath), "application/json");
                                            #endregion

                                            #region ClanService
                                            else if (absolutepath.Contains($"/ClanService/{env}/clan/"))
                                                clanService.HandleClanDetailsService(request, Response, absolutepath);
                                            #endregion

                                            #region TradingService
                                            else if (absolutepath.Contains($"/TradingService/pmcards/trade"))
                                            {
                                                string? res = tradingService.HandleTradingService(request, sessionid, absolutepath);

                                                if (res != null)
                                                    Response.MakeGetResponse(res, "application/json");
                                                else
                                                    Response.MakeErrorResponse();
                                                break;
                                            }
                                            #endregion

                                            #region FriendsService
                                            else if (absolutepath.Contains($"/identity/person/{sessionid}/data/psn/friends-list"))
                                            {
                                                var res = friendsService.HandleFriendsService(absolutepath, postbuffer);
                                                Response.MakeOkResponse();
                                            }
                                            #endregion

                                            else
                                            {
                                                LoggerAccessor.LogWarn($"[SSFWProcessor] : Host requested a POST method I don't know about! - Report it to GITHUB with the request : {absolutepath}");
                                                if (postbuffer != null)
                                                {
                                                    Directory.CreateDirectory(directoryPath);
                                                    switch (GetHeaderValue(Headers, "Content-type", false))
                                                    {
                                                        case "image/jpeg":
                                                            File.WriteAllBytes($"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.jpeg", postbuffer);
                                                            break;
                                                        case "application/json":
                                                            File.WriteAllBytes($"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.json", postbuffer);
                                                            break;
                                                        default:
                                                            File.WriteAllBytes($"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.bin", postbuffer);
                                                            break;
                                                    }
                                                }
                                                Response.MakeOkResponse();
                                            }
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.SetBegin((int)HttpStatusCode.Forbidden);
                                            Response.SetBody();
                                        }
                                    }
                                    else
                                    {
                                        Response.Clear();
                                        Response.SetBegin(400);
                                        Response.SetBody();
                                    }

                                    break;
                                case "PUT":
                                    if (IsSSFWRegistered(sessionid))
                                    {
                                        if (request.BodyLength <= Array.MaxLength)
                                        {
                                            byte[] putbuffer = request.BodyBytes;
                                            if (putbuffer != null)
                                            {
                                                Directory.CreateDirectory(directoryPath);
                                                switch (GetHeaderValue(Headers, "Content-type", false))
                                                {
                                                    case "image/jpeg":
                                                        string savaDataAvatarDirectoryPath = Path.Combine(SSFWServerConfiguration.SSFWStaticFolder, $"SaveDataService/avatar/{env}/");

                                                        Directory.CreateDirectory(savaDataAvatarDirectoryPath);

                                                        string? userName = SSFWUserSessionManager.GetFormatedUsernameBySessionId(sessionid);

                                                        if (!string.IsNullOrEmpty(userName))
                                                        {
                                                            Task.WhenAll(File.WriteAllBytesAsync($"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.jpeg", putbuffer),
                                                                File.WriteAllBytesAsync($"{savaDataAvatarDirectoryPath}{userName}.jpg", putbuffer)).Wait();
                                                            Response.MakeOkResponse();
                                                        }
                                                        else
                                                            Response.MakeErrorResponse();
                                                        break;
                                                    case "application/json":

                                                        #region Event Log AuditService
                                                        if (absolutepath.Equals("/AuditService/log"))
                                                        {
                                                            auditService.HandleAuditService(absolutepath, putbuffer, request);
                                                            //Audit doesn't care so we send ok!
                                                            Response.MakeOkResponse();
                                                        }
                                                        #endregion

                                                        else
                                                        {
                                                            File.WriteAllBytes($"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.json", putbuffer);
                                                            Response.MakeOkResponse();
                                                        }
                                                        break;
                                                    default:
                                                        File.WriteAllBytes($"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.bin", putbuffer);
                                                        Response.MakeOkResponse();
                                                        break;
                                                }
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody();
                                            }
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.SetBegin(400);
                                            Response.SetBody();
                                        }
                                    }
                                    else
                                    {
                                        Response.Clear();
                                        Response.SetBegin((int)HttpStatusCode.Forbidden);
                                        Response.SetBody();
                                    }
                                    break;
                                case "DELETE":

                                    if (IsSSFWRegistered(sessionid))
                                    {
                                        #region AvatarLayoutService
                                        if (absolutepath.Contains($"/AvatarLayoutService/{env}/"))
                                        {
                                            if (request.BodyLength <= Array.MaxLength)
                                            {
                                                Response.Clear();
                                                if (avatarLayout.HandleAvatarLayout(request.BodyBytes, directoryPath, filePath, absolutepath, true))
                                                    Response.SetBegin((int)HttpStatusCode.OK);
                                                else
                                                    Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody();
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.SetBegin(400);
                                                Response.SetBody();
                                            }
                                        }
                                        #endregion
                                        
                                        #region ClanService
                                        else if (absolutepath.Contains($"/ClanService/{env}/clan/"))
                                        {
                                            clanService.HandleClanDetailsService(request, Response, absolutepath);
                                        }
                                        #endregion

                                        #region RewardsService Inventory DEBUG
                                        // RewardsService Inventory DEBUG - WipeInventory
                                        else if (absolutepath.Contains($"/RewardsService/pmcards/rewards/{SSFWUserSessionManager.GetIdBySessionId(sessionid)}"))
                                        {
                                            var res = rewardSvc.HandleRewardServiceWipeInvDELETE(directoryPath, filePath, absolutepath, UserAgent, sessionid);
                                            if (res != null)
                                                Response.MakeOkResponse();
                                            else
                                                Response.MakeErrorResponse(500, "Failed to Delete Rewards Inventory!");
                                        }
                                        // RewardsService Inventory DEBUG - DebugClearCardTrackingData
                                        else if (absolutepath.Contains($"/RewardsService/pmcards/rewards/{SSFWUserSessionManager.GetIdBySessionId(sessionid)}/00000000-00000000-00000000-00000001"))
                                        {
                                            var res = rewardSvc.HandleRewardServiceInvCardTrackingDataDELETE(directoryPath, filePath, absolutepath, UserAgent, sessionid);
                                            if (res != null)
                                                Response.MakeOkResponse();
                                            else
                                                Response.MakeErrorResponse(500, "Failed to Delete Rewards Card Tracking Data!");
                                        }
                                        #endregion

                                        else
                                        {
                                            if (File.Exists(filePath + ".json"))
                                            {
                                                File.Delete(filePath + ".json");
                                                Response.MakeOkResponse();
                                            }
                                            else if (File.Exists(filePath + ".bin"))
                                            {
                                                File.Delete(filePath + ".bin");
                                                Response.MakeOkResponse();
                                            }
                                            else if (File.Exists(filePath + ".jpeg"))
                                            {
                                                File.Delete(filePath + ".jpeg");
                                                Response.MakeOkResponse();
                                            }
                                            else
                                            {
                                                LoggerAccessor.LogError($"[SSFWProcessor] : {UserAgent} Requested a file to delete that doesn't exist - {filePath}");
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.NotFound);
                                                Response.SetBody();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Response.Clear();
                                        Response.SetBegin((int)HttpStatusCode.Forbidden);
                                        Response.SetBody();
                                    }
                                    break;
                                default:
                                    Response.Clear();
                                    Response.SetBegin((int)HttpStatusCode.Forbidden);
                                    Response.SetBody();
                                    break;
                            }
                        }

                        #region SoundShapes
                        else if (UserAgent.Contains("PS3Application"))
                        {
                            isApiRequest = true;

                            sessionid = GetHeaderValue(Headers, "X-OTG-Identity-SessionId");

                            switch (request.Method)
                            {
                                case "GET":
                                    break;
                                case "POST":
                                    {
                                        if (request.BodyLength <= Array.MaxLength)
                                        {
                                            #region IdentityService Login
                                            byte[] postbuffer = request.BodyBytes;

                                            //SoundShapes Login
                                            if (absolutepath == "/identity/login/token/psn")
                                            {
                                                IdentityService login = new("1.00", "SoundShapes", "1.14", "$ound$h@pesi$C00l", _legacykey);
                                                string? result = login.HandleLoginSS(postbuffer, "cprod");
                                                if (!string.IsNullOrEmpty(result))
                                                {
                                                    Response.Clear();
                                                    Response.SetBegin(201); // Replace with URL or proper Server IP
                                                    Response.SetHeader("location", $"http://{IPAddress.Any}/_dentity/api/service/{LoginGUID}/proxy/login/token/psn/api/client/sessions/f59306bd-3e25-4a34-a41c-ae6c0744c57e");
                                                    Response.SetHeader("X-OTG-Identity-SessionId", sessionid);
                                                    Response.SetContentType("application/json");
                                                    Response.SetBody(result, encoding);
                                                }
                                                else
                                                    Response.MakeErrorResponse();
                                            }
                                            #endregion

                                            #region FriendService
                                            else if (absolutepath.Contains($"/identity/person/{sessionid}/data/psn/friends-list"))
                                            {
                                                FriendsService friendsService = new(sessionid, "prod", _legacykey);
                                                var res = friendsService.HandleFriendsService(absolutepath, postbuffer);
                                                Response.MakeOkResponse();
                                            }
                                            #endregion
                                        }
                                        break;
                                    }
                            }
                        }
                        #endregion

                    }

                    if (!isApiRequest)
                    {
                        switch (request.Method)
                        {
                            case "GET":
                                try
                                {
                                    string? extension = Path.GetExtension(filePath);

                                    if (!string.IsNullOrEmpty(extension) && allowedWebExtensions.Contains(extension))
                                    {
                                        if (File.Exists(filePath))
                                        {
                                            Response.Clear();
                                            Response.SetBegin((int)HttpStatusCode.OK);
                                            Response.SetContentType(HTTPProcessor.GetMimeType(extension, HTTPProcessor.MimeTypes));
                                            Response.SetBody(File.ReadAllBytes(filePath), encoding, GetHeaderValue(Headers, "Origin"));
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.SetBegin((int)HttpStatusCode.NotFound);
                                            Response.SetBody(string.Empty, null, GetHeaderValue(Headers, "Origin"));
                                        }
                                    }
                                    else
                                    {
                                        Response.Clear();
                                        Response.SetBegin((int)HttpStatusCode.Forbidden);
                                        Response.SetBody(string.Empty, null, GetHeaderValue(Headers, "Origin"));
                                    }
                                }
                                catch
                                {
                                    Response.Clear();
                                    Response.SetBegin((int)HttpStatusCode.InternalServerError);
                                    Response.SetBody(string.Empty, null, GetHeaderValue(Headers, "Origin"));
                                }
                                break;
                            case "OPTIONS":
                                Response.Clear();
                                Response.SetBegin((int)HttpStatusCode.OK);
                                Response.SetHeader("Allow", HttpResponse.allowedMethods);
                                Response.SetBody(string.Empty, null, GetHeaderValue(Headers, "Origin"));
                                break;
                            case "POST":
                                byte InventoryEntryType = 0;
                                string? userId = null;
                                string uuid = string.Empty;
                                string sessionId = string.Empty;
                                string env = string.Empty;
                                string[]? uuids = null;

                                switch (absolutepath)
                                {
                                    case "/WebService/GetSceneLike/":
                                        string sceneNameLike = GetHeaderValue(Headers, "like", false);

                                        Response.Clear();

                                        if (!string.IsNullOrEmpty(sceneNameLike))
                                        {
                                            KeyValuePair<string, string>? sceneData = ScenelistParser.GetSceneNameLike(sceneNameLike);

                                            if (sceneData != null && int.TryParse(sceneData.Value.Value, out int extractedId))
                                            {
                                                Response.SetBegin((int)HttpStatusCode.OK);
                                                Response.SetBody(sceneData.Value.Key + ',' + extractedId.ToUuid(), encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                            else
                                            {
                                                Response.SetBegin((int)HttpStatusCode.InternalServerError);
                                                Response.SetBody("SceneNameLike returned a null or empty sceneName!", encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                        }
                                        else
                                        {
                                            Response.SetBegin((int)HttpStatusCode.Forbidden);
                                            Response.SetBody("Invalid like attribute was used!", encoding, GetHeaderValue(Headers, "Origin"));
                                        }
                                        break;
                                    case "/WebService/ApplyLayoutOverride/":
                                        sessionId = GetHeaderValue(Headers, "sessionid", false);
                                        if (sessionId == string.Empty) sessionId = GetHeaderValue(Headers, "X-Home-Session-Id", false);
                                        string targetUserName = GetHeaderValue(Headers, "targetUserName", false);
                                        string sceneId = GetHeaderValue(Headers, "sceneId", false);
                                        env = GetHeaderValue(Headers, "env", false);

                                        if (string.IsNullOrEmpty(env) || !SSFWServerConfiguration.homeEnvs.Contains(env))
                                            env = "cprod";

                                        Response.Clear();

                                        if (!string.IsNullOrEmpty(sessionId) && !string.IsNullOrEmpty(targetUserName) && !string.IsNullOrEmpty(sceneId) && IsSSFWRegistered(sessionId))
                                        {
                                            string? res = null;
                                            bool isRpcnUser = targetUserName.Contains("@RPCN");
                                            string LayoutDirectoryPath = Path.Combine(SSFWServerConfiguration.SSFWStaticFolder, $"LayoutService/{env}/person/");

                                            if (Directory.Exists(LayoutDirectoryPath))
                                            {
                                                string? matchingDirectory = null;
                                                string? username = SSFWUserSessionManager.GetUsernameBySessionId(SSFWUserSessionManager.GetSessionIdByUsername(targetUserName, isRpcnUser));
                                                string? clientVersion = username?.Substring(username.Length - 6, 6);

                                                if (!string.IsNullOrEmpty(clientVersion))
                                                {
                                                    if (isRpcnUser)
                                                    {
                                                        string[] nameParts = targetUserName.Split('@');

                                                        if (nameParts.Length == 2 && !SSFWServerConfiguration.SSFWCrossSave)
                                                        {
                                                            matchingDirectory = Directory.GetDirectories(LayoutDirectoryPath)
                                                               .Where(dir =>
                                                                   Path.GetFileName(dir).StartsWith(nameParts[0]) &&
                                                                   Path.GetFileName(dir).Contains(nameParts[1]) &&
                                                                   Path.GetFileName(dir).Contains(clientVersion)
                                                               ).FirstOrDefault();
                                                        }
                                                        else
                                                            matchingDirectory = Directory.GetDirectories(LayoutDirectoryPath)
                                                              .Where(dir =>
                                                                  Path.GetFileName(dir).StartsWith(targetUserName.Replace("@RPCN", string.Empty)) &&
                                                                  Path.GetFileName(dir).Contains(clientVersion)
                                                              ).FirstOrDefault();
                                                    }
                                                    else
                                                        matchingDirectory = Directory.GetDirectories(LayoutDirectoryPath)
                                                          .Where(dir =>
                                                              Path.GetFileName(dir).StartsWith(targetUserName) &&
                                                              !Path.GetFileName(dir).Contains("RPCN") &&
                                                              Path.GetFileName(dir).Contains(clientVersion)
                                                          ).FirstOrDefault();
                                                }

                                                if (!string.IsNullOrEmpty(matchingDirectory))
                                                    res = new LayoutService(_legacykey).HandleLayoutServiceGET(matchingDirectory, sceneId);

                                            } // if the dir not exists, we return 403.

                                            string errmesg;

                                            if (res == null)
                                            {
                                                errmesg = $"Override set for {sessionId}, but no layout was found for this scene.";
#if DEBUG
                                                LoggerAccessor.LogWarn($"[SSFWProcessor] - " + errmesg);
#endif
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody(errmesg, encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                            else if (res == string.Empty)
                                            {
                                                errmesg = $"Override set for {sessionId}, but layout data was empty.";
#if DEBUG
                                                LoggerAccessor.LogWarn($"[SSFWProcessor] - " + errmesg);
#endif
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.NotFound);
                                                Response.SetBody(errmesg, encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                            else
                                            {
                                                if (!LayoutGetOverrides.TryAdd(sessionId, res))
                                                    LayoutGetOverrides[sessionId] = res;

                                                Response.SetBegin((int)HttpStatusCode.OK);
                                                Response.SetContentType("application/json; charset=utf-8");
                                                Response.SetBody(res, encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                        }
                                        else
                                        {
                                            Response.SetBegin((int)HttpStatusCode.Forbidden);
                                            Response.SetBody("Invalid sessionid or targetUserName attribute was used!", encoding, GetHeaderValue(Headers, "Origin"));
                                        }
                                        break;
                                    case "/WebService/R3moveLayoutOverride/":
                                        sessionId = GetHeaderValue(Headers, "sessionid", false);
                                        if (sessionId == string.Empty) sessionId = GetHeaderValue(Headers, "X-Home-Session-Id", false);

                                        Response.Clear();

                                        if (!string.IsNullOrEmpty(sessionId) && IsSSFWRegistered(sessionId))
                                        {
                                            if (LayoutGetOverrides.Remove(sessionId, out _))
                                            {
                                                Response.SetBegin((int)HttpStatusCode.OK);
                                                Response.SetBody($"Override removed for {sessionId}.", encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                            else
                                            {
                                                Response.SetBegin((int)HttpStatusCode.NotFound);
                                                Response.SetBody($"Override not found for {sessionId}.", encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                        }
                                        else
                                        {
                                            Response.SetBegin((int)HttpStatusCode.Forbidden);
                                            Response.SetBody("Invalid sessionid attribute was used!", encoding, GetHeaderValue(Headers, "Origin"));
                                        }
                                        break;
                                    case "/WebService/GetMini/":
                                        sessionId = GetHeaderValue(Headers, "sessionid", false);
                                        if (sessionId == string.Empty) sessionId = GetHeaderValue(Headers, "X-Home-Session-Id", false);
                                        env = GetHeaderValue(Headers, "env", false);

                                        if (string.IsNullOrEmpty(env) || !SSFWServerConfiguration.homeEnvs.Contains(env))
                                            env = "cprod";

                                        userId = SSFWUserSessionManager.GetIdBySessionId(sessionId);

                                        if (!string.IsNullOrEmpty(userId))
                                        {
                                            string miniPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{userId}/mini.json";

                                            if (File.Exists(miniPath))
                                            {
                                                Response.Clear();

                                                try
                                                {
                                                    Response.SetBegin((int)HttpStatusCode.OK);
                                                    Response.SetContentType("application/json; charset=utf-8");
                                                    Response.SetBody(FileHelper.ReadAllText(miniPath, _legacykey) ?? string.Empty, encoding, GetHeaderValue(Headers, "Origin"));
                                                }
                                                catch
                                                {
                                                    Response.SetBegin((int)HttpStatusCode.InternalServerError);
                                                    Response.SetBody($"Error while reading the mini file for User: {sessionId} on env:{env}!", encoding, GetHeaderValue(Headers, "Origin"));
                                                }
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody($"User: {sessionId} on env:{env} doesn't have a ssfw mini file!", encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.SetBegin((int)HttpStatusCode.Forbidden);
                                            Response.SetBody($"User: {sessionId} is not connected!", encoding, GetHeaderValue(Headers, "Origin"));
                                        }
                                        break;
                                    case "/WebService/AddMiniItem/":
                                        uuid = GetHeaderValue(Headers, "uuid", false);
                                        sessionId = GetHeaderValue(Headers, "sessionid", false);
                                        if (sessionId == string.Empty) sessionId = GetHeaderValue(Headers, "X-Home-Session-Id", false);
                                        env = GetHeaderValue(Headers, "env", false);

                                        if (string.IsNullOrEmpty(env) || !SSFWServerConfiguration.homeEnvs.Contains(env))
                                            env = "cprod";

                                        userId = SSFWUserSessionManager.GetIdBySessionId(sessionId);

                                        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(uuid) && byte.TryParse(GetHeaderValue(Headers, "invtype", false), out InventoryEntryType))
                                        {
                                            string miniPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{userId}/mini.json";

                                            if (File.Exists(miniPath))
                                            {
                                                try
                                                {
                                                    new RewardsService(_legacykey).AddMiniEntry(uuid, InventoryEntryType, $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{userId}.json", env, userId);
                                                    Response.Clear();
                                                    Response.SetBegin((int)HttpStatusCode.OK);
                                                    Response.SetBody($"UUID: {uuid} successfully added to the Mini rewards list.", encoding, GetHeaderValue(Headers, "Origin"));
                                                }
                                                catch (Exception ex)
                                                {
                                                    string errMsg = $"Mini rewards list file update errored out for file: {miniPath} (Exception: {ex})";
                                                    Response.Clear();
                                                    Response.SetBegin((int)HttpStatusCode.InternalServerError);
                                                    Response.SetBody(errMsg, encoding, GetHeaderValue(Headers, "Origin"));
                                                    LoggerAccessor.LogError($"[SSFWProcessor] - {errMsg}");
                                                }
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody($"User: {sessionId} on env:{env} doesn't have a ssfw mini file!", encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.SetBegin((int)HttpStatusCode.Forbidden);
                                            Response.SetBody($"User: {sessionId} is not connected or sent invalid InventoryEntryType!", encoding, GetHeaderValue(Headers, "Origin"));
                                        }
                                        break;
                                    case "/WebService/AddMiniItems/":
                                        uuids = GetHeaderValue(Headers, "uuids", false).Split(',');
                                        sessionId = GetHeaderValue(Headers, "sessionid", false);
                                        if (sessionId == string.Empty) sessionId = GetHeaderValue(Headers, "X-Home-Session-Id", false);
                                        env = GetHeaderValue(Headers, "env", false);

                                        if (string.IsNullOrEmpty(env) || !SSFWServerConfiguration.homeEnvs.Contains(env))
                                            env = "cprod";

                                        userId = SSFWUserSessionManager.GetIdBySessionId(sessionId);

                                        if (!string.IsNullOrEmpty(userId) && uuids != null && byte.TryParse(GetHeaderValue(Headers, "invtype", false), out InventoryEntryType))
                                        {
                                            string miniPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{userId}/mini.json";

                                            if (File.Exists(miniPath))
                                            {
                                                Dictionary<string, byte> entriesToAdd = new();

                                                foreach (string iteruuid in uuids)
                                                {
                                                    entriesToAdd.TryAdd(iteruuid, InventoryEntryType);
                                                }

                                                try
                                                {
                                                    new RewardsService(_legacykey).AddMiniEntries(entriesToAdd, $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{userId}.json", env, userId);
                                                    Response.Clear();
                                                    Response.SetBegin((int)HttpStatusCode.OK);
                                                    Response.SetBody($"UUIDs: {string.Join(",", uuids)} successfully added to the Mini rewards list.", encoding, GetHeaderValue(Headers, "Origin"));
                                                }
                                                catch (Exception ex)
                                                {
                                                    string errMsg = $"Mini rewards list file update errored out for file: {miniPath} (Exception: {ex})";
                                                    Response.Clear();
                                                    Response.SetBegin((int)HttpStatusCode.InternalServerError);
                                                    Response.SetBody(errMsg, encoding, GetHeaderValue(Headers, "Origin"));
                                                    LoggerAccessor.LogError($"[SSFWProcessor] - {errMsg}");
                                                }
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody($"User: {sessionId} on env:{env} doesn't have a ssfw mini file!", encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.SetBegin((int)HttpStatusCode.Forbidden);
                                            Response.SetBody($"User: {sessionId} is not connected or sent invalid InventoryEntryType!", encoding, GetHeaderValue(Headers, "Origin"));
                                        }
                                        break;
                                    case "/WebService/RemoveMiniItem/":
                                        uuid = GetHeaderValue(Headers, "uuid", false);
                                        sessionId = GetHeaderValue(Headers, "sessionid", false);
                                        if (sessionId == string.Empty) sessionId = GetHeaderValue(Headers, "X-Home-Session-Id", false);
                                        env = GetHeaderValue(Headers, "env", false);

                                        if (string.IsNullOrEmpty(env) || !SSFWServerConfiguration.homeEnvs.Contains(env))
                                            env = "cprod";

                                        userId = SSFWUserSessionManager.GetIdBySessionId(sessionId);

                                        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(uuid) && byte.TryParse(GetHeaderValue(Headers, "invtype", false), out InventoryEntryType))
                                        {
                                            string miniPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{userId}/mini.json";

                                            if (File.Exists(miniPath))
                                            {
                                                try
                                                {
                                                    new RewardsService(_legacykey).RemoveMiniEntry(uuid, InventoryEntryType, $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{userId}.json", env, userId);
                                                    Response.Clear();
                                                    Response.SetBegin((int)HttpStatusCode.OK);
                                                    Response.SetBody($"UUID: {uuid} successfully removed in the Mini rewards list.", encoding, GetHeaderValue(Headers, "Origin"));
                                                }
                                                catch (Exception ex)
                                                {
                                                    string errMsg = $"Mini rewards list file update errored out for file: {miniPath} (Exception: {ex})";
                                                    Response.Clear();
                                                    Response.SetBegin((int)HttpStatusCode.InternalServerError);
                                                    Response.SetBody(errMsg, encoding, GetHeaderValue(Headers, "Origin"));
                                                    LoggerAccessor.LogError($"[SSFWProcessor] - {errMsg}");
                                                }
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody($"User: {sessionId} on env:{env} doesn't have a ssfw mini file!", encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.SetBegin((int)HttpStatusCode.Forbidden);
                                            Response.SetBody($"User: {sessionId} is not connected or sent invalid InventoryEntryType!", encoding, GetHeaderValue(Headers, "Origin"));
                                        }
                                        break;
                                    case "/WebService/RemoveMiniItems/":
                                        uuids = GetHeaderValue(Headers, "uuids", false).Split(',');
                                        sessionId = GetHeaderValue(Headers, "sessionid", false);
                                        if (sessionId == string.Empty) sessionId = GetHeaderValue(Headers, "X-Home-Session-Id", false);
                                        env = GetHeaderValue(Headers, "env", false);

                                        if (string.IsNullOrEmpty(env) || !SSFWServerConfiguration.homeEnvs.Contains(env))
                                            env = "cprod";

                                        userId = SSFWUserSessionManager.GetIdBySessionId(sessionId);

                                        if (!string.IsNullOrEmpty(userId) && uuids != null && byte.TryParse(GetHeaderValue(Headers, "invtype", false), out InventoryEntryType))
                                        {
                                            string miniPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{userId}/mini.json";

                                            if (File.Exists(miniPath))
                                            {
                                                Dictionary<string, byte> entriesToRemove = new();

                                                foreach (string iteruuid in uuids)
                                                {
                                                    entriesToRemove.TryAdd(iteruuid, InventoryEntryType);
                                                }

                                                try
                                                {
                                                    new RewardsService(_legacykey).RemoveMiniEntries(entriesToRemove, $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{userId}.json", env, userId);
                                                    Response.Clear();
                                                    Response.SetBegin((int)HttpStatusCode.OK);
                                                    Response.SetBody($"UUIDs: {string.Join(",", uuids)} removed in the Mini rewards list.", encoding, GetHeaderValue(Headers, "Origin"));
                                                }
                                                catch (Exception ex)
                                                {
                                                    string errMsg = $"Mini rewards list file update errored out for file: {miniPath} (Exception: {ex})";
                                                    Response.Clear();
                                                    Response.SetBegin((int)HttpStatusCode.InternalServerError);
                                                    Response.SetBody(errMsg, encoding, GetHeaderValue(Headers, "Origin"));
                                                    LoggerAccessor.LogError($"[SSFWProcessor] - {errMsg}");
                                                }
                                            }
                                            else
                                            {
                                                Response.Clear();
                                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                                Response.SetBody($"User: {sessionId} on env:{env} doesn't have a ssfw mini file!", encoding, GetHeaderValue(Headers, "Origin"));
                                            }
                                        }
                                        else
                                        {
                                            Response.Clear();
                                            Response.SetBegin((int)HttpStatusCode.Forbidden);
                                            Response.SetBody($"User: {sessionId} is not connected or sent invalid InventoryEntryType!", encoding, GetHeaderValue(Headers, "Origin"));
                                        }
                                        break;
                                    default:
                                        Response.Clear();
                                        Response.SetBegin((int)HttpStatusCode.Forbidden);
                                        Response.SetBody(string.Empty, null, GetHeaderValue(Headers, "Origin"));
                                        break;
                                }
                                break;
                            default:
                                Response.Clear();
                                Response.SetBegin((int)HttpStatusCode.Forbidden);
                                Response.SetBody(string.Empty, null, GetHeaderValue(Headers, "Origin"));
                                break;
                        }
                    }
                }
                else
                {
                    LoggerAccessor.LogError($"[SSFWProcessor] - Home Client Requested the SSFW Server with an invalid url!");
                    Response.Clear();
                    Response.SetBegin(400);
                    Response.SetBody();
                }
            }
            catch (Exception e)
            {
                Response.MakeErrorResponse();
                LoggerAccessor.LogError($"[SSFWProcessor] - SSFW Request thrown an error : {e}");
            }

            return Response;
        }

        private bool MyRemoteCertificateValidationCallback(object? sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            // If certificate is null, reject
            if (certificate == null)
            {
                LoggerAccessor.LogError("[SSFWClass] - MyRemoteCertificateValidationCallback: Certificate is null.");
                return false;
            }

            // Cast to X509Certificate2 for date and signature checks
            if (certificate is not X509Certificate2 cert2)
            {
                LoggerAccessor.LogError("[SSFWClass] - MyRemoteCertificateValidationCallback: Certificate is not an X509Certificate2.");
                return false;
            }

            DateTime now = DateTime.UtcNow;

            // Check certificate validity dates (skip NotAfter date as Home certs are old)
            if (now < cert2.NotBefore)
            {
                LoggerAccessor.LogError($"[SSFWClass] - MyRemoteCertificateValidationCallback: Certificate is not valid at current date/time: {now}");
                return false;
            }

            // If no SSL chain reported
            if (chain == null)
            {
                LoggerAccessor.LogError("[SSFWClass] - MyRemoteCertificateValidationCallback: Certificate chain is null.");
                return false;
            }

            const string homeClientCertsPath = "home_certificates";

            // Check the certificate against known ones
            if (Directory.Exists(homeClientCertsPath))
            {
                const string pemExtension = ".pem";

                foreach (var pemFilePath in Directory.GetFiles(homeClientCertsPath, $"*{pemExtension}"))
                {
                    string pemContent = File.ReadAllText(pemFilePath);

                    // Skip private keys
                    if (pemContent.Contains(CertificateHelper.keyBegin))
                        continue;

                    string certFileName = Path.GetFileNameWithoutExtension(pemFilePath);
                    string privPemKeyFilePath = Path.Combine(homeClientCertsPath, certFileName + $"_privkey{pemExtension}");

                    if (!File.Exists(privPemKeyFilePath))
                    {
                        LoggerAccessor.LogWarn($"[SSFWClass] - MyRemoteCertificateValidationCallback: Private key file not found for cert: {certFileName}");
                        continue;
                    }

                    if (CertificateHelper.CertificatesMatch(CertificateHelper.LoadCombinedCertificateAndKeyFromString(pemContent, File.ReadAllText(privPemKeyFilePath)), cert2))
                    {
                        LoggerAccessor.LogInfo($"[SSFWClass] - MyRemoteCertificateValidationCallback: Certificate matched known cert: {pemFilePath}");

                        // All checks passed: cert is valid and verified, chain is good, dates valid, signatures valid
                        return true;
                    }
                }

                LoggerAccessor.LogError("[SSFWClass] - MyRemoteCertificateValidationCallback: No matching certificate found in home_certificates.");
                return false;
            }

            // All checks passed: cert is valid, chain is good, dates valid, signatures valid
            return true;
        }

        private class SSFWSession : HttpsSession
        {
            public SSFWSession(HttpsServer server) : base(server) { }

            protected override void OnReceivedRequest(HttpRequest request)
            {
                SendResponseAsync(SSFWRequestProcess(request, Response));
            }

            protected override void OnReceivedRequestError(HttpRequest request, string error)
            {
                LoggerAccessor.LogError($"[SSFWProcessor] - Request error: {error}");
            }

            protected override void OnError(SocketError error)
            {
                LoggerAccessor.LogError($"[SSFWProcessor] - Session caught an error: {error}");
            }
        }

        private class HttpSSFWSession : HttpSession
        {
            public HttpSSFWSession(HttpServer server) : base(server) { }

            protected override void OnReceivedRequest(HttpRequest request)
            {
                SendResponseAsync(SSFWRequestProcess(request, Response));
            }

            protected override void OnReceivedRequestError(HttpRequest request, string error)
            {
                LoggerAccessor.LogError($"[SSFWProcessor] - Request error: {error}");
            }

            protected override void OnError(SocketError error)
            {
                LoggerAccessor.LogError($"[SSFWProcessor] - HTTP session caught an error: {error}");
            }
        }

        public class SSFWServer : HttpsServer
        {
            public SSFWServer(SslContext context, IPAddress address, int port) : base(context, address, port) { }

            protected override SslSession CreateSession() { return new SSFWSession(this); }

            protected override void OnError(SocketError error)
            {
                LoggerAccessor.LogError($"[SSFWProcessor] - Server caught an error: {error}");
            }
        }

        public class HttpSSFWServer : HttpServer
        {
            public HttpSSFWServer(IPAddress address, int port) : base(address, port) { }

            protected override TcpSession CreateSession() { return new HttpSSFWSession(this); }

            protected override void OnError(SocketError error)
            {
                LoggerAccessor.LogError($"[SSFWProcessor] - HTTPS session caught an error: {error}");
            }
        }
    }
}
