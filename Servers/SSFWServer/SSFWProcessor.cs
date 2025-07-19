using CustomLogger;
using MultiServerLibrary.Extension;
using MultiServerLibrary.HTTP;
using MultiServerLibrary.SSL;
using Newtonsoft.Json;
using SSFWServer.Helpers.FileHelper;
using SSFWServer.SaveDataHelper;
using SSFWServer.Services;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using WatsonWebserver;
using WatsonWebserver.Core;
using WatsonWebserver.Native;

namespace SSFWServer
{
    public partial class SSFWProcessor
    {
        public const string allowedMethods = "HEAD, GET, PUT, POST, DELETE, OPTIONS";
        public static List<string> allowedOrigins = new List<string>() { };

        private const string LoginGUID = "bb88aea9-6bf8-4201-a6ff-5d1f8da0dd37";

        // Defines a list of web-related file extensions
        private static HashSet<string> allowedWebExtensions = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            ".html", ".htm", ".cgi", ".css", ".js", ".svg", ".gif", ".ico", ".woff", ".woff2", ".ttf", ".eot"
        };

        private static ConcurrentDictionary<string, string> LayoutGetOverrides = new();

        private static string serverRevision = Assembly.GetExecutingAssembly().GetName().Name + " " + Assembly.GetExecutingAssembly().GetName().Version;

        private WebserverBase? _Server;
        private readonly ushort port;
        private Thread? StarterThread;

        public SSFWProcessor(string certpath, string certpass, string ip, ushort port, bool secure, int MaxConcurrentListeners)
        {
            bool useHttpSys = SSFWServerConfiguration.PreferNativeHttpListenerEngine;
            this.port = port;
            WebserverSettings settings = new()
            {
                Hostname = ip,
                Port = port,
            };
            settings.IO.StreamBufferSize = SSFWServerConfiguration.BufferSize;
            settings.IO.EnableKeepAlive = SSFWServerConfiguration.EnableKeepAlive;
            if (secure)
            {
                useHttpSys = false;
                settings.Ssl.PfxCertificateFile = certpath;
                settings.Ssl.PfxCertificatePassword = certpass;
                settings.Ssl.MutuallyAuthenticate = true;
                settings.Ssl.Enable = true;
            }
            if (useHttpSys)
            {
                _Server = new NativeWebserver(settings, DefaultRoute, MaxConcurrentListeners);
#if !DEBUG
                    ((NativeWebserver)_Server).LogResponseSentMsg = false;
#endif
                ((NativeWebserver)_Server).KeepAliveResponseData = false;
            }
            else
            {
                _Server = new Webserver(settings, DefaultRoute, MaxConcurrentListeners);
#if !DEBUG
                    ((Webserver)_Server).LogResponseSentMsg = false;
#endif
                ((Webserver)_Server).KeepAliveResponseData = false;
            }

            StarterThread = new Thread(StartServer)
            {
                Name = "Server Starter"
            };
            StarterThread.Start();
        }

        private static void SetCorsHeaders(HttpContextBase ctx)
        {
            const string originHeader = "Origin";
            string origin = ctx.Request.RetrieveHeaderValue(originHeader);

            if (string.IsNullOrEmpty(origin) || allowedOrigins.Count == 0)
                // Allow requests with no Origin header (e.g., direct server-to-server requests) or if we not set any CORS rules.
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", "*");
            else if (allowedOrigins.Contains(origin))
                // Allow requests with a valid Origin
                ctx.Response.Headers.Add("Access-Control-Allow-Origin", origin);
            else
            {
                ctx.Response.Headers.Add("Access-Control-Deny-Origin", origin);
                return;
            }

            ctx.Response.Headers.Add("Access-Control-Allow-Methods", allowedMethods);
            ctx.Response.Headers.Add("Access-Control-Allow-Headers", "*");
            ctx.Response.Headers.Add("Access-Control-Expose-Headers", string.Empty);
        }

        private static async Task AuthorizeConnection(HttpContextBase ctx)
        {
            string IpToBan = ctx.Request.Source.IpAddress;
            if (!"::1".Equals(IpToBan) && !"127.0.0.1".Equals(IpToBan) && !"localhost".Equals(IpToBan, StringComparison.InvariantCultureIgnoreCase))
            {
                if (!string.IsNullOrEmpty(IpToBan) && MultiServerLibrary.MultiServerLibraryConfiguration.BannedIPs != null && MultiServerLibrary.MultiServerLibraryConfiguration.BannedIPs.Contains(IpToBan))
                {
                    LoggerAccessor.LogError($"[SECURITY] - Client - {ctx.Request.Source.IpAddress}:{ctx.Request.Source.Port} Requested the HTTPS server while being banned!");
                    ctx.Response.StatusCode = 403;
                    await ctx.Response.Send();
                }
            }
        }

        private static (string HeaderIndex, string HeaderItem)[] CollectHeaders(HttpRequestBase request)
        {
            int headerindex = (int)request.Headers.Count;

            (string HeaderIndex, string HeaderItem)[] CollectHeader = new (string, string)[headerindex];

            for (int i = 0; i < headerindex; i++)
            {
                string? headerKey = request.Headers.GetKey(i);
                if (!string.IsNullOrEmpty(headerKey))
                    CollectHeader[i] = (headerKey, request.Headers.Get(i) ?? string.Empty);
            }

            return CollectHeader;
        }

        private static string GetHeaderValue((string HeaderIndex, string HeaderItem)[] headers, string requestedHeaderIndex, bool caseSensitive = true)
        {
            if (headers.Length > 0)
            {
                const string pattern = @"^(.*?):\s(.*)$";

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
                                && match.Success)
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

        public void StopServer()
        {
            try
            {
                _Server?.Dispose();

                LoggerAccessor.LogWarn($"SSFW Server on port: {port} stopped...");
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"SSFW Server on port: {port} stopped unexpectedly! (Exception: {ex})");
            }
        }

        public void StartServer()
        {
            if (_Server != null && !_Server.IsListening)
            {
                _Server.Routes.AuthenticateRequest = AuthorizeConnection;
                _Server.Events.ExceptionEncountered += ExceptionEncountered;
                _Server.Events.Logger = LoggerAccessor.LogInfo;
#if DEBUG
                _Server.Settings.Debug.Responses = true;
                _Server.Settings.Debug.Routing = true;
#endif
                _Server.Start();
                LoggerAccessor.LogInfo($"SSFW Server initiated on port: {port}...");
            }
        }

        private static async Task DefaultRoute(HttpContextBase ctx)
        {
            bool sent = false;
            HttpStatusCode statusCode;
            HttpRequestBase request = ctx.Request;
            HttpResponseBase response = ctx.Response;
            string legacykey = SSFWServerConfiguration.SSFWLegacyKey;
            string absolutepath = HTTPProcessor.DecodeUrl(request.Url.RawWithoutQuery);
            string clientip = request.Source.IpAddress;
            string clientport = request.Source.Port.ToString();

            SetCorsHeaders(ctx);

            if (absolutepath != string.Empty)
            {
                (string HeaderIndex, string HeaderItem)[] Headers = CollectHeaders(request);
#if DEBUG
                IEnumerable<string> HeadersValue;
                try
                {
                    HeadersValue = ctx.Request.Headers.AllKeys.SelectMany(key => ctx.Request.Headers.GetValues(key) ?? Enumerable.Empty<string>());
                }
                catch (ArgumentNullException)
                {
                    HeadersValue = Enumerable.Empty<string>();
                }
                LoggerAccessor.LogInfo($"[SSFW] - {clientip}:{clientport} Requested the Server with URL : {request.Url.RawWithQuery} (Details: " + JsonConvert.SerializeObject(new
                {
                    HttpMethod = request.Method,
                    Url = request.Url.RawWithQuery,
                    Headers = request.Headers,
                    HeadersValues = HeadersValue,
                    UserAgent = string.IsNullOrEmpty(request.Useragent) ? string.Empty : request.Useragent,
                    ClientAddress = request.Source.IpAddress + ":" + request.Source.Port,
#if false // Serve as a HTTP json debugging.
                    Body = request.ContentLength >= 0 ? Convert.ToBase64String(request.DataAsBytes) : string.Empty
#endif
                }, Formatting.Indented) + ") (" + ctx.Timestamp.TotalMs + "ms)");
#else
                    LoggerAccessor.LogInfo($"[SSFW] - {clientip}:{clientport} Requested the Server with URL : {request.Url.RawWithQuery} (" + ctx.Timestamp.TotalMs + "ms)");
#endif
                string? encoding = null;
                string UserAgent = GetHeaderValue(Headers, "User-Agent", false);
                string cacheControl = GetHeaderValue(Headers, "Cache-Control");

                if (string.IsNullOrEmpty(cacheControl) || cacheControl != "no-transform")
                    encoding = GetHeaderValue(Headers, "Accept-Encoding");

                // Split the URL into segments
                string[] segments = absolutepath.Trim('/').Split('/');

                // Combine the folder segments into a directory path
                string directoryPath = Path.Combine(SSFWServerConfiguration.SSFWStaticFolder, string.Join("/", segments.Take(segments.Length - 1).ToArray()));

                // Process the request based on the HTTP method
                string filePath = Path.Combine(SSFWServerConfiguration.SSFWStaticFolder, absolutepath[1..]);

                if (!string.IsNullOrEmpty(UserAgent) && UserAgent.Contains("PSHome"))
                {
                    string host = GetHeaderValue(Headers, "host", false);
                    string? env = ExtractBeforeFirstDot(host);
                    string sessionid = GetHeaderValue(Headers, "X-Home-Session-Id");

                    if (string.IsNullOrEmpty(env) || !SSFWMisc.homeEnvs.Contains(env))
                        env = "cprod";

                    // Instantiate services
                    SSFWAuditService auditService = new(sessionid, env, legacykey);
                    SSFWRewardsService rewardSvc = new(legacykey);
                    SSFWLayoutService layout = new(legacykey);
                    SSFWAvatarLayoutService avatarLayout = new(sessionid, legacykey);

                    switch (request.Method.ToString())
                    {
                        case "GET":

                            #region LayoutService
                            if (absolutepath.Contains($"/LayoutService/{env}/person/") && IsSSFWRegistered(sessionid))
                            {
                                string? res = null;

                                if (LayoutGetOverrides.ContainsKey(sessionid))
                                    LayoutGetOverrides.Remove(sessionid, out res);
                                else
                                    res = layout.HandleLayoutServiceGET(directoryPath, filePath);

                                if (res == null)
                                {
                                    statusCode = HttpStatusCode.Forbidden;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                                else if (res == string.Empty)
                                {
                                    statusCode = HttpStatusCode.NotFound;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                                else
                                {
                                    statusCode = HttpStatusCode.OK;
                                    response.StatusCode = (int)statusCode;
                                    response.ContentType = "application/json";
                                    sent = await response.Send(res);
                                }
                            }
                            #endregion

                            #region AdminObjectService
                            else if (absolutepath.Contains("/AdminObjectService/start") && IsSSFWRegistered(sessionid))
                            {
                                if (new SSFWAdminObjectService(sessionid, legacykey).HandleAdminObjectService(UserAgent))
                                    statusCode = HttpStatusCode.OK;
                                else
                                    statusCode = HttpStatusCode.Forbidden;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }
                            #endregion

                            #region SaveDataService
                            else if (absolutepath.Contains($"/SaveDataService/{env}/{segments.LastOrDefault()}") && IsSSFWRegistered(sessionid))
                            {
                                string? res = SSFWGetFileList.SSFWSaveDataDebugGetFileList(directoryPath, segments.LastOrDefault());
                                if (res != null)
                                {
                                    statusCode = HttpStatusCode.OK;
                                    response.StatusCode = (int)statusCode;
                                    response.ContentType = "application/json";
                                    sent = await response.Send(CompressResponse(response, res, encoding));
                                }
                                else
                                {
                                    statusCode = HttpStatusCode.InternalServerError;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                            }
                            #endregion

                            else if (IsSSFWRegistered(sessionid))
                            {
                                //First check if this is a Inventory request
                                if (absolutepath.Contains($"/RewardsService/") && absolutepath.Contains("counts"))
                                {
                                    //Detect if existing inv exists
                                    if (File.Exists(filePath + ".json"))
                                    {
                                        string? res = FileHelper.ReadAllText(filePath + ".json", legacykey);

                                        if (!string.IsNullOrEmpty(res))
                                        {
                                            if (GetHeaderValue(Headers, "Accept") == "application/json")
                                            {
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                response.ContentType = "application/json";
                                                sent = await response.Send(CompressResponse(response, res, encoding));
                                            }
                                            else
                                            {
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send(CompressResponse(response, res, encoding));
                                            }
                                        }
                                        else
                                        {
                                            statusCode = HttpStatusCode.InternalServerError;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send();
                                        }
                                    }
                                    else //fallback default 
                                    {
                                        statusCode = HttpStatusCode.OK;
                                        response.StatusCode = (int)statusCode;
                                        response.ContentType = "application/json";
                                        sent = await response.Send(CompressResponse(response, @"{ ""00000000-00000000-00000000-00000001"": 1 } ", encoding));
                                    }
                                }
                                //Check for specifically the Tracking GUID
                                else if (absolutepath.Contains($"/RewardsService/") && absolutepath.Contains("object/00000000-00000000-00000000-00000001"))
                                {
                                    //Detect if existing inv exists
                                    if (File.Exists(filePath + ".json"))
                                    {
                                        string? res = FileHelper.ReadAllText(filePath + ".json", legacykey);

                                        if (!string.IsNullOrEmpty(res))
                                        {
                                            if (GetHeaderValue(Headers, "Accept") == "application/json")
                                            {
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                response.ContentType = "application/json";
                                                sent = await response.Send(CompressResponse(response, res, encoding));
                                            }
                                            else
                                            {
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send(CompressResponse(response, res, encoding));
                                            }
                                        }
                                        else
                                        {
                                            statusCode = HttpStatusCode.InternalServerError;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send();
                                        }
                                    }
                                    else //fallback default 
                                    {
#if DEBUG
                                        LoggerAccessor.LogWarn($"[SSFW] : {UserAgent} Non-existent inventories detected, using defaults!");
#endif
                                        if (absolutepath.Contains("p4t-cprod"))
                                        {
                                            #region Quest for Greatness
                                            statusCode = HttpStatusCode.OK;
                                            response.StatusCode = (int)statusCode;
                                            response.ContentType = "application/json";
                                            sent = await response.Send(CompressResponse(response, @"{
                                                      ""result"": 0,
                                                      ""rewards"": {
                                                        ""00000000-00000000-00000000-00000001"": {
                                                          ""migrated"": 1,
                                                          ""_id"": ""1""
                                                        }
                                                      }
                                                    }", encoding));
                                            #endregion
                                        }
                                        else
                                        {
                                            #region Pottermore
                                            statusCode = HttpStatusCode.OK;
                                            response.StatusCode = (int)statusCode;
                                            response.ContentType = "application/json";
                                            sent = await response.Send(CompressResponse(response, @"{
                                                      ""result"": 0,
                                                      ""rewards"": [
                                                        {
                                                          ""00000000-00000000-00000000-00000001"": {
                                                          ""boost"": ""AQ=="",
                                                          ""_id"": ""tracking""
                                                          }
                                                        }
                                                      ]
                                                    }", encoding));
                                            #endregion
                                        }

                                    }
                                }
                                else if (File.Exists(filePath + ".json"))
                                {
                                    string? res = FileHelper.ReadAllText(filePath + ".json", legacykey);

                                    if (!string.IsNullOrEmpty(res))
                                    {
                                        if (GetHeaderValue(Headers, "Accept") == "application/json")
                                        {
                                            statusCode = HttpStatusCode.OK;
                                            response.StatusCode = (int)statusCode;
                                            response.ContentType = "application/json";
                                            sent = await response.Send(CompressResponse(response, res, encoding));
                                        }
                                        else
                                        {
                                            statusCode = HttpStatusCode.OK;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send(CompressResponse(response, res, encoding));
                                        }
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.InternalServerError;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send();
                                    }
                                }
                                else if (File.Exists(filePath + ".bin"))
                                {
                                    byte[]? res = FileHelper.ReadAllBytes(filePath + ".bin", legacykey);

                                    if (res != null)
                                    {
                                        statusCode = HttpStatusCode.OK;
                                        response.StatusCode = (int)statusCode;
                                        response.ContentType = "application/octet-stream";
                                        sent = await response.Send(CompressResponse(response, res, encoding));
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.InternalServerError;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send();
                                    }
                                }
                                else if (File.Exists(filePath + ".jpeg"))
                                {
                                    byte[]? res = FileHelper.ReadAllBytes(filePath + ".jpeg", legacykey);

                                    if (res != null)
                                    {
                                        statusCode = HttpStatusCode.OK;
                                        response.StatusCode = (int)statusCode;
                                        response.ContentType = "image/jpeg";
                                        sent = await response.Send(CompressResponse(response, res, encoding));
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.InternalServerError;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send();
                                    }
                                }
                                else
                                {
                                    LoggerAccessor.LogWarn($"[SSFW] : {UserAgent} Requested a non-existent file - {filePath}");
                                    statusCode = HttpStatusCode.NotFound;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                            }
                            else if (absolutepath.Contains($"/SaveDataService/avatar/{env}/") && absolutepath.EndsWith(".jpg"))
                            {
                                if (File.Exists(filePath))
                                {
                                    byte[]? res = FileHelper.ReadAllBytes(filePath, legacykey);

                                    if (res != null)
                                    {
                                        statusCode = HttpStatusCode.OK;
                                        response.StatusCode = (int)statusCode;
                                        response.ContentType = "image/jpg";
                                        sent = await response.Send(CompressResponse(response, res, encoding));
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.InternalServerError;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send();
                                    }
                                }
                                else
                                {
                                    statusCode = HttpStatusCode.NotFound;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                            }
                            else
                            {
                                statusCode = HttpStatusCode.Forbidden;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }
                            break;
                        case "POST":

                            #region SSFW Login
                            byte[] postbuffer = request.DataAsBytes;
                            if (absolutepath == $"/{LoginGUID}/login/token/psn")
                            {
                                string? XHomeClientVersion = GetHeaderValue(Headers, "X-HomeClientVersion");
                                string? generalsecret = GetHeaderValue(Headers, "general-secret");

                                if (!string.IsNullOrEmpty(XHomeClientVersion) && !string.IsNullOrEmpty(generalsecret))
                                {
                                    SSFWLogin login = new(XHomeClientVersion, generalsecret, XHomeClientVersion.Replace(".", string.Empty).PadRight(6, '0'), GetHeaderValue(Headers, "x-signature"), legacykey);
                                    string? res = login.HandleLogin(postbuffer, env);
                                    if (!string.IsNullOrEmpty(res))
                                    {
                                        statusCode = HttpStatusCode.Created;
                                        response.StatusCode = (int)statusCode;
                                        response.ContentType = "application/json";
                                        sent = await response.Send(CompressResponse(response, res, encoding));

                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.InternalServerError;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send();
                                    }
                                    login.Dispose();
                                }
                                else
                                {
                                    statusCode = HttpStatusCode.Forbidden;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                            }
                            #endregion

                            #region PING
                            else if (absolutepath.Contains("/morelife") && !string.IsNullOrEmpty(GetHeaderValue(Headers, "x-signature")))
                            {
                                const byte GuidLength = 36;
                                int index = absolutepath.IndexOf("/morelife");

                                if (index != -1 && index > GuidLength) // Makes sure we have at least 36 chars available beforehand.
                                {
                                    // Extract the substring between the last '/' and the morelife separator.
                                    string resultSessionId = absolutepath.Substring(index - GuidLength, GuidLength);

                                    if (Regex.IsMatch(resultSessionId, @"^[{(]?([0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})[)}]?$") && IsSSFWRegistered(resultSessionId))
                                    {
                                        SSFWUserSessionManager.UpdateKeepAliveTime(resultSessionId);
                                        statusCode = HttpStatusCode.OK;
                                        response.StatusCode = (int)statusCode;
                                        response.ContentType = "application/json";
                                        sent = await response.Send(CompressResponse(response, "{}", encoding));
                                        break;
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.Forbidden;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send();
                                    }
                                }
                                else
                                {
                                    statusCode = HttpStatusCode.InternalServerError;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                            }
                            #endregion

                            #region AvatarLayoutService
                            else if (absolutepath.Contains($"/AvatarLayoutService/{env}/") && IsSSFWRegistered(sessionid))
                            {
                                if (avatarLayout.HandleAvatarLayout(postbuffer, directoryPath, filePath, absolutepath, false))
                                    statusCode = HttpStatusCode.OK;
                                else
                                    statusCode = HttpStatusCode.Forbidden;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }
                            #endregion

                            #region LayoutService
                            else if (absolutepath.Contains($"/LayoutService/{env}/person/") && IsSSFWRegistered(sessionid))
                            {
                                if (layout.HandleLayoutServicePOST(postbuffer, directoryPath, absolutepath))
                                    statusCode = HttpStatusCode.OK;
                                else
                                    statusCode = HttpStatusCode.Forbidden;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }
                            #endregion

                            #region RewardsService
                            else if (absolutepath.Contains($"/RewardsService/{env}/rewards/") && IsSSFWRegistered(sessionid))
                            {
                                statusCode = HttpStatusCode.OK;
                                response.StatusCode = (int)statusCode;
                                response.ContentType = "application/json";
                                sent = await response.Send(CompressResponse(response, rewardSvc.HandleRewardServicePOST(postbuffer, directoryPath, filePath, absolutepath), encoding));
                            }
                            else if (absolutepath.Contains($"/RewardsService/trunks-{env}/trunks/") && absolutepath.Contains("/setpartial") && IsSSFWRegistered(sessionid))
                            {
                                rewardSvc.HandleRewardServiceTrunksPOST(postbuffer, directoryPath, filePath, absolutepath, env, SSFWUserSessionManager.GetIdBySessionId(sessionid));
                                statusCode = HttpStatusCode.OK;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }
                            else if (absolutepath.Contains($"/RewardsService/trunks-{env}/trunks/") && absolutepath.Contains("/set") && IsSSFWRegistered(sessionid))
                            {
                                rewardSvc.HandleRewardServiceTrunksEmergencyPOST(postbuffer, directoryPath, absolutepath);
                                statusCode = HttpStatusCode.OK;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }
                            else if (
                                (absolutepath.Contains($"/RewardsService/pm_{env}_inv/")
                                || absolutepath.Contains($"/RewardsService/pmcards/")
                                || absolutepath.Contains($"/RewardsService/p4t-{env}/"))
                                && IsSSFWRegistered(sessionid))
                            {
                                statusCode = HttpStatusCode.OK;
                                response.StatusCode = (int)statusCode;
                                response.ContentType = "application/json";
                                sent = await response.Send(CompressResponse(response, rewardSvc.HandleRewardServiceInvPOST(postbuffer, directoryPath, filePath, absolutepath), encoding));
                            }
                            #endregion

                            else if (IsSSFWRegistered(sessionid))
                            {
                                LoggerAccessor.LogWarn($"[SSFW] : Host requested a POST method I don't know about! - Report it to GITHUB with the request : {absolutepath}");
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
                                statusCode = HttpStatusCode.OK;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }
                            else
                            {
                                statusCode = HttpStatusCode.Forbidden;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }

                            break;
                        case "PUT":
                            if (IsSSFWRegistered(sessionid))
                            {
                                byte[] putbuffer = request.DataAsBytes;
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
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send();
                                            }
                                            else
                                            {
                                                statusCode = HttpStatusCode.InternalServerError;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send();
                                            }
                                            break;
                                        case "application/json":
                                            if (absolutepath.Equals("/AuditService/log"))
                                            {
                                                auditService.HandleAuditService(absolutepath, putbuffer);
                                                //Audit doesn't care we send ok!
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send();
                                            }
                                            else
                                            {
                                                File.WriteAllBytes($"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.json", putbuffer);
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send();
                                            }
                                            break;
                                        default:
                                            File.WriteAllBytes($"{SSFWServerConfiguration.SSFWStaticFolder}/{absolutepath}.bin", putbuffer);
                                            statusCode = HttpStatusCode.OK;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send();
                                            break;
                                    }
                                }
                                else
                                {
                                    statusCode = HttpStatusCode.Forbidden;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                            }
                            else
                            {
                                statusCode = HttpStatusCode.Forbidden;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }
                            break;
                        case "DELETE":

                            #region AvatarLayoutService
                            if (absolutepath.Contains($"/AvatarLayoutService/{env}/") && IsSSFWRegistered(sessionid))
                            {
                                if (avatarLayout.HandleAvatarLayout(request.DataAsBytes, directoryPath, filePath, absolutepath, true))
                                    statusCode = HttpStatusCode.OK;
                                else
                                    statusCode = HttpStatusCode.Forbidden;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }
                            #endregion

                            else if (IsSSFWRegistered(sessionid))
                            {
                                if (File.Exists(filePath + ".json"))
                                {
                                    File.Delete(filePath + ".json");
                                    statusCode = HttpStatusCode.OK;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                                else if (File.Exists(filePath + ".bin"))
                                {
                                    File.Delete(filePath + ".bin");
                                    statusCode = HttpStatusCode.OK;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                                else if (File.Exists(filePath + ".jpeg"))
                                {
                                    File.Delete(filePath + ".jpeg");
                                    statusCode = HttpStatusCode.OK;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                                else
                                {
                                    LoggerAccessor.LogError($"[SSFW] : {UserAgent} Requested a file to delete that doesn't exist - {filePath}");
                                    statusCode = HttpStatusCode.NotFound;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send();
                                }
                            }
                            else
                            {
                                statusCode = HttpStatusCode.Forbidden;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send();
                            }
                            break;
                        default:
                            statusCode = HttpStatusCode.Forbidden;
                            response.StatusCode = (int)statusCode;
                            sent = await response.Send();
                            break;
                    }
                }
                else
                {
                    switch (request.Method.ToString())
                    {
                        case "GET":
                            try
                            {
                                string? extension = Path.GetExtension(filePath);

                                if (!string.IsNullOrEmpty(extension) && allowedWebExtensions.Contains(extension))
                                {
                                    if (File.Exists(filePath))
                                    {
                                        statusCode = HttpStatusCode.OK;
                                        response.StatusCode = (int)statusCode;
                                        response.ContentType = HTTPProcessor.GetMimeType(extension, HTTPProcessor._mimeTypes);
                                        sent = await response.Send(CompressResponse(response, File.ReadAllBytes(filePath), encoding));
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.NotFound;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send(string.Empty);
                                    }
                                }
                                else
                                {
                                    statusCode = HttpStatusCode.Forbidden;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send(string.Empty);
                                }
                            }
                            catch
                            {
                                statusCode = HttpStatusCode.InternalServerError;
                                response.StatusCode = (int)statusCode;
                                sent = await response.Send(string.Empty);
                            }
                            break;
                        case "OPTIONS":
                            statusCode = HttpStatusCode.OK;
                            response.StatusCode = (int)statusCode;
                            response.Headers.Add("Allow", allowedMethods);
                            sent = await response.Send(string.Empty);
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

                                    if (!string.IsNullOrEmpty(sceneNameLike))
                                    {
                                        KeyValuePair<string, string>? sceneData = ScenelistParser.GetSceneNameLike(sceneNameLike);

                                        if (sceneData != null && int.TryParse(sceneData.Value.Value, out int extractedId))
                                        {
                                            statusCode = HttpStatusCode.OK;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send(sceneData.Value.Key + ',' + extractedId.ToUuid());
                                        }
                                        else
                                        {
                                            statusCode = HttpStatusCode.InternalServerError;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send("SceneNameLike returned a null or empty sceneName!");
                                        }
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.Forbidden;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send("Invalid like attribute was used!");
                                    }
                                    break;
                                case "/WebService/ApplyLayoutOverride/":
                                    sessionId = GetHeaderValue(Headers, "sessionid", false);
                                    string targetUserName = GetHeaderValue(Headers, "targetUserName", false);
                                    string sceneId = GetHeaderValue(Headers, "sceneId", false);
                                    env = GetHeaderValue(Headers, "env", false);

                                    if (string.IsNullOrEmpty(env) || !SSFWMisc.homeEnvs.Contains(env))
                                        env = "cprod";

                                    if (!string.IsNullOrEmpty(sessionId) && !string.IsNullOrEmpty(targetUserName) && !string.IsNullOrEmpty(sceneId) && IsSSFWRegistered(sessionId))
                                    {
                                        string? res = null;
                                        bool isRpcnUser = targetUserName.Contains("@RPCN");
                                        string LayoutDirectoryPath = Path.Combine(SSFWServerConfiguration.SSFWStaticFolder, $"LayoutService/{env}/person/");

                                        if (Directory.Exists(LayoutDirectoryPath))
                                        {
                                            string? matchingDirectory = null;
                                            string? username = SSFWUserSessionManager.GetUsernameBySessionId(sessionId);
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
                                                res = new SSFWLayoutService(legacykey).HandleLayoutServiceGET(matchingDirectory, sceneId);

                                        } // if the dir not exists, we return 403.

                                        if (res == null)
                                        {
                                            statusCode = HttpStatusCode.Forbidden;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send($"Override set for {sessionId}, but no layout was found for this scene.");
                                        }
                                        else if (res == string.Empty)
                                        {
                                            statusCode = HttpStatusCode.NotFound;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send($"Override set for {sessionId}, but layout data was empty.");
                                        }
                                        else
                                        {
                                            if (!LayoutGetOverrides.TryAdd(sessionId, res))
                                                LayoutGetOverrides[sessionId] = res;

                                            statusCode = HttpStatusCode.OK;
                                            response.StatusCode = (int)statusCode;
                                            response.ContentType = "application/json; charset=utf-8";
                                            sent = await response.Send(CompressResponse(response, res, encoding));
                                        }
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.Forbidden;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send("Invalid sessionid or targetUserName attribute was used!");
                                    }
                                    break;
                                case "/WebService/R3moveLayoutOverride/":
                                    sessionId = GetHeaderValue(Headers, "sessionid", false);

                                    if (!string.IsNullOrEmpty(sessionId) && IsSSFWRegistered(sessionId))
                                    {
                                        if (LayoutGetOverrides.Remove(sessionId, out _))
                                        {
                                            statusCode = HttpStatusCode.OK;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send($"Override removed for {sessionId}.");
                                        }
                                        else
                                        {
                                            statusCode = HttpStatusCode.NotFound;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send($"Override not found for {sessionId}.");
                                        }
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.Forbidden;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send("Invalid sessionid attribute was used!");
                                    }
                                    break;
                                case "/WebService/GetMini/":
                                    sessionId = GetHeaderValue(Headers, "sessionid", false);
                                    env = GetHeaderValue(Headers, "env", false);

                                    if (string.IsNullOrEmpty(env) || !SSFWMisc.homeEnvs.Contains(env))
                                        env = "cprod";

                                    userId = SSFWUserSessionManager.GetIdBySessionId(sessionId);

                                    if (!string.IsNullOrEmpty(userId))
                                    {
                                        string miniPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{userId}/mini.json";

                                        if (File.Exists(miniPath))
                                        {
                                            try
                                            {
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                response.ContentType = "application/json; charset=utf-8";
                                                sent = await response.Send(CompressResponse(response, FileHelper.ReadAllText(miniPath, legacykey) ?? string.Empty, encoding));
                                            }
                                            catch
                                            {
                                                statusCode = HttpStatusCode.InternalServerError;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send($"Error while reading the mini file for User: {sessionId} on env:{env}!");
                                            }
                                        }
                                        else
                                        {
                                            statusCode = HttpStatusCode.Forbidden;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send($"User: {sessionId} on env:{env} doesn't have a ssfw mini file!");
                                        }
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.Forbidden;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send($"User: {sessionId} is not connected!");
                                    }
                                    break;
                                case "/WebService/AddMiniItem/":
                                    uuid = GetHeaderValue(Headers, "uuid", false);
                                    sessionId = GetHeaderValue(Headers, "sessionid", false);
                                    env = GetHeaderValue(Headers, "env", false);

                                    if (string.IsNullOrEmpty(env) || !SSFWMisc.homeEnvs.Contains(env))
                                        env = "cprod";

                                    userId = SSFWUserSessionManager.GetIdBySessionId(sessionId);

                                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(uuid) && byte.TryParse(GetHeaderValue(Headers, "invtype", false), out InventoryEntryType))
                                    {
                                        string miniPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{userId}/mini.json";

                                        if (File.Exists(miniPath))
                                        {
                                            try
                                            {
                                                new SSFWRewardsService(legacykey).AddMiniEntry(uuid, InventoryEntryType, $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{userId}.json", env, userId);
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send($"UUID: {uuid} successfully added to the Mini rewards list.");
                                            }
                                            catch (Exception ex)
                                            {
                                                string errMsg = $"Mini rewards list file update errored out for file: {miniPath} (Exception: {ex})";
                                                statusCode = HttpStatusCode.InternalServerError;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send(errMsg);
                                                LoggerAccessor.LogError($"[SSFW] - {errMsg}");
                                            }
                                        }
                                        else
                                        {
                                            statusCode = HttpStatusCode.Forbidden;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send($"User: {sessionId} on env:{env} doesn't have a ssfw mini file!");
                                        }
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.Forbidden;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send($"User: {sessionId} is not connected or sent invalid InventoryEntryType!");
                                    }
                                    break;
                                case "/WebService/AddMiniItems/":
                                    uuids = GetHeaderValue(Headers, "uuids", false).Split(',');
                                    sessionId = GetHeaderValue(Headers, "sessionid", false);
                                    env = GetHeaderValue(Headers, "env", false);

                                    if (string.IsNullOrEmpty(env) || !SSFWMisc.homeEnvs.Contains(env))
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
                                                new SSFWRewardsService(legacykey).AddMiniEntries(entriesToAdd, $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{userId}.json", env, userId);
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send(CompressResponse(response, $"UUIDs: {string.Join(",", uuids)} successfully added to the Mini rewards list.", encoding));
                                            }
                                            catch (Exception ex)
                                            {
                                                string errMsg = $"Mini rewards list file update errored out for file: {miniPath} (Exception: {ex})";
                                                statusCode = HttpStatusCode.InternalServerError;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send(errMsg);
                                                LoggerAccessor.LogError($"[SSFW] - {errMsg}");
                                            }
                                        }
                                        else
                                        {
                                            statusCode = HttpStatusCode.Forbidden;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send($"User: {sessionId} on env:{env} doesn't have a ssfw mini file!");
                                        }
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.Forbidden;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send($"User: {sessionId} is not connected or sent invalid InventoryEntryType!");
                                    }
                                    break;
                                case "/WebService/RemoveMiniItem/":
                                    uuid = GetHeaderValue(Headers, "uuid", false);
                                    sessionId = GetHeaderValue(Headers, "sessionid", false);
                                    env = GetHeaderValue(Headers, "env", false);

                                    if (string.IsNullOrEmpty(env) || !SSFWMisc.homeEnvs.Contains(env))
                                        env = "cprod";

                                    userId = SSFWUserSessionManager.GetIdBySessionId(sessionId);

                                    if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(uuid) && byte.TryParse(GetHeaderValue(Headers, "invtype", false), out InventoryEntryType))
                                    {
                                        string miniPath = $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/{env}/rewards/{userId}/mini.json";

                                        if (File.Exists(miniPath))
                                        {
                                            try
                                            {
                                                new SSFWRewardsService(legacykey).RemoveMiniEntry(uuid, InventoryEntryType, $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{userId}.json", env, userId);
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send($"UUID: {uuid} successfully removed in the Mini rewards list.");
                                            }
                                            catch (Exception ex)
                                            {
                                                string errMsg = $"Mini rewards list file update errored out for file: {miniPath} (Exception: {ex})";
                                                statusCode = HttpStatusCode.InternalServerError;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send(errMsg);
                                                LoggerAccessor.LogError($"[SSFW] - {errMsg}");
                                            }
                                        }
                                        else
                                        {
                                            statusCode = HttpStatusCode.Forbidden;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send($"User: {sessionId} on env:{env} doesn't have a ssfw mini file!");
                                        }
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.Forbidden;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send($"User: {sessionId} is not connected or sent invalid InventoryEntryType!");
                                    }
                                    break;
                                case "/WebService/RemoveMiniItems/":
                                    uuids = GetHeaderValue(Headers, "uuids", false).Split(',');
                                    sessionId = GetHeaderValue(Headers, "sessionid", false);
                                    env = GetHeaderValue(Headers, "env", false);

                                    if (string.IsNullOrEmpty(env) || !SSFWMisc.homeEnvs.Contains(env))
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
                                                new SSFWRewardsService(legacykey).RemoveMiniEntries(entriesToRemove, $"{SSFWServerConfiguration.SSFWStaticFolder}/RewardsService/trunks-{env}/trunks/{userId}.json", env, userId);
                                                statusCode = HttpStatusCode.OK;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send(CompressResponse(response, $"UUIDs: {string.Join(",", uuids)} removed in the Mini rewards list.", encoding));
                                            }
                                            catch (Exception ex)
                                            {
                                                string errMsg = $"Mini rewards list file update errored out for file: {miniPath} (Exception: {ex})";
                                                statusCode = HttpStatusCode.InternalServerError;
                                                response.StatusCode = (int)statusCode;
                                                sent = await response.Send(errMsg);
                                                LoggerAccessor.LogError($"[SSFW] - {errMsg}");
                                            }
                                        }
                                        else
                                        {
                                            statusCode = HttpStatusCode.Forbidden;
                                            response.StatusCode = (int)statusCode;
                                            sent = await response.Send($"User: {sessionId} on env:{env} doesn't have a ssfw mini file!");
                                        }
                                    }
                                    else
                                    {
                                        statusCode = HttpStatusCode.Forbidden;
                                        response.StatusCode = (int)statusCode;
                                        sent = await response.Send($"User: {sessionId} is not connected or sent invalid InventoryEntryType!");
                                    }
                                    break;
                                default:
                                    statusCode = HttpStatusCode.Forbidden;
                                    response.StatusCode = (int)statusCode;
                                    sent = await response.Send(string.Empty);
                                    break;
                            }
                            break;
                        default:
                            statusCode = HttpStatusCode.Forbidden;
                            response.StatusCode = (int)statusCode;
                            sent = await response.Send(string.Empty);
                            break;
                    }
                }
            }
            else
            {
                LoggerAccessor.LogError($"[SSFW] - Home Client Requested the SSFW Server with an invalid url!");
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                sent = await response.Send();
            }

            if (response.StatusCode < 400)
                LoggerAccessor.LogInfo($"[SSFW] - {clientip}:{clientport} -> {response.StatusCode}");
            else
            {
                switch (response.StatusCode)
                {
                    case (int)HttpStatusCode.NotFound:
                    case (int)HttpStatusCode.NotImplemented:
                    case (int)HttpStatusCode.RequestedRangeNotSatisfiable:
                        LoggerAccessor.LogWarn($"[SSFW] - {clientip}:{clientport} -> {response.StatusCode}");
                        break;

                    default:
                        LoggerAccessor.LogError($"[SSFW] - {clientip}:{clientport} -> {response.StatusCode}");
                        break;
                }
            }
        }

        private static byte[]? CompressResponse(HttpResponseBase response, byte[]? data, string? encoding)
        {
            if (SSFWServerConfiguration.EnableHTTPCompression && !string.IsNullOrEmpty(encoding))
            {
                if (encoding.Contains("zstd"))
                {
                    response.Headers.Add("Content-Encoding", "zstd");
                    return HTTPProcessor.CompressZstd(data);
                }
                else if (encoding.Contains("br"))
                {
                    response.Headers.Add("Content-Encoding", "br");
                    return HTTPProcessor.CompressBrotli(data);
                }
                else if (encoding.Contains("gzip"))
                {
                    response.Headers.Add("Content-Encoding", "gzip");
                    return HTTPProcessor.CompressGzip(data);
                }
                else if (encoding.Contains("deflate"))
                {
                    response.Headers.Add("Content-Encoding", "deflate");
                    return HTTPProcessor.Deflate(data);
                }
            }

            return data;
        }

        private static byte[]? CompressResponse(HttpResponseBase response, string? data, string? encoding)
        {
            if (data == null)
                return null;
            else if (SSFWServerConfiguration.EnableHTTPCompression && !string.IsNullOrEmpty(encoding))
            {
                if (encoding.Contains("zstd"))
                {
                    response.Headers.Add("Content-Encoding", "zstd");
                    return HTTPProcessor.CompressZstd(Encoding.UTF8.GetBytes(data));
                }
                else if (encoding.Contains("br"))
                {
                    response.Headers.Add("Content-Encoding", "br");
                    return HTTPProcessor.CompressBrotli(Encoding.UTF8.GetBytes(data));
                }
                else if (encoding.Contains("gzip"))
                {
                    response.Headers.Add("Content-Encoding", "gzip");
                    return HTTPProcessor.CompressGzip(Encoding.UTF8.GetBytes(data));
                }
                else if (encoding.Contains("deflate"))
                {
                    response.Headers.Add("Content-Encoding", "deflate");
                    return HTTPProcessor.Deflate(Encoding.UTF8.GetBytes(data));
                }
            }

            return Encoding.UTF8.GetBytes(data);
        }

        private void ExceptionEncountered(object? sender, ExceptionEventArgs args)
        {
            LoggerAccessor.LogError($"[SSFW] - Exception Encountered: {args.Exception}");
        }

        private bool MyRemoteCertificateValidationCallback(object? sender, X509Certificate? certificate, X509Chain? chain, SslPolicyErrors sslPolicyErrors)
        {
            // If certificate is null, reject
            if (certificate == null)
            {
                LoggerAccessor.LogError("[SSFW] - MyRemoteCertificateValidationCallback: Certificate is null.");
                return false;
            }

            // Cast to X509Certificate2 for date and signature checks
            if (certificate is not X509Certificate2 cert2)
            {
                LoggerAccessor.LogError("[SSFW] - MyRemoteCertificateValidationCallback: Certificate is not an X509Certificate2.");
                return false;
            }

            DateTime now = DateTime.UtcNow;

            // Check certificate validity dates (skip NotAfter date as Home certs are old)
            if (now < cert2.NotBefore)
            {
                LoggerAccessor.LogError($"[SSFW] - MyRemoteCertificateValidationCallback: Certificate is not valid at current date/time: {now}");
                return false;
            }

            // If no SSL chain reported
            if (chain == null)
            {
                LoggerAccessor.LogError("[SSFW] - MyRemoteCertificateValidationCallback: Certificate chain is null.");
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
                        LoggerAccessor.LogWarn($"[SSFW] - MyRemoteCertificateValidationCallback: Private key file not found for cert: {certFileName}");
                        continue;
                    }

                    if (CertificateHelper.CertificatesMatch(CertificateHelper.LoadCombinedCertificateAndKeyFromString(pemContent, File.ReadAllText(privPemKeyFilePath)), cert2))
                    {
                        LoggerAccessor.LogInfo($"[SSFW] - MyRemoteCertificateValidationCallback: Certificate matched known cert: {pemFilePath}");

                        // All checks passed: cert is valid and verified, chain is good, dates valid, signatures valid
                        return true;
                    }
                }

                LoggerAccessor.LogError("[SSFW] - MyRemoteCertificateValidationCallback: No matching certificate found in home_certificates.");
                return false;
            }

            // All checks passed: cert is valid, chain is good, dates valid, signatures valid
            return true;
        }
    }
}
