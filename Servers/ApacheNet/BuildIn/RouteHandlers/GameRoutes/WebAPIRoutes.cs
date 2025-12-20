using ApacheNet.Models;
using CastleLibrary.Sony.XI5;
using CustomLogger;
using MultiServerLibrary.Extension;
using MultiServerLibrary.GeoLocalization;
using MultiServerLibrary.HTTP;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml;
using WebAPIService.GameServices.DEMANGLER;
using WebAPIService.GameServices.FROMSOFTWARE;
using WebAPIService.GameServices.I_Love_Sony;
using WebAPIService.GameServices.PSHOME.CAPONE;
using WebAPIService.GameServices.PSHOME.CDM;
using WebAPIService.GameServices.PSHOME.CODEGLUE;
using WebAPIService.GameServices.PSHOME.COGS;
using WebAPIService.GameServices.PSHOME.DIGITAL_LEISURE;
using WebAPIService.GameServices.PSHOME.HEAVYWATER;
using WebAPIService.GameServices.PSHOME.HELLFIRE;
using WebAPIService.GameServices.PSHOME.HOMELEADERBOARDS;
using WebAPIService.GameServices.PSHOME.HTS;
using WebAPIService.GameServices.PSHOME.JUGGERNAUT;
using WebAPIService.GameServices.PSHOME.LOOT;
using WebAPIService.GameServices.PSHOME.NDREAMS;
using WebAPIService.GameServices.PSHOME.OHS;
using WebAPIService.GameServices.PSHOME.OUWF;
using WebAPIService.GameServices.PSHOME.PREMIUMAGENCY;
using WebAPIService.GameServices.PSHOME.RCHOME;
using WebAPIService.GameServices.PSHOME.THQ;
using WebAPIService.GameServices.PSHOME.TSS;
using WebAPIService.GameServices.PSHOME.VEEMEE;
using WebAPIService.GameServices.UBISOFT.BuildAPI;
using WebAPIService.GameServices.UBISOFT.gsconnect;
using WebAPIService.GameServices.UBISOFT.HERMES_API;
using WebAPIService.GameServices.UBISOFT.MatchMakingConfig;
using WebAPIService.GameServices.UBISOFT.OnlineConfigService;

namespace ApacheNet.BuildIn.RouteHandlers.GameRoutes
{
    internal class WebAPIRoutes
    {
        #region Domains
        private readonly static List<string> HPDDomains = new() {
                                    "prd.destinations.scea.com",
                                    "pre.destinations.scea.com",
                                    "qa.destinations.scea.com",
                                    "dev.destinations.scea.com"
                                };

        private readonly static List<string> CAPONEDomains = new() {
                                    "collector.gr.online.scea.com",
                                    "collector-nonprod.gr.online.scea.com",
                                    "collector-dev.gr.online.scea.com",
                                    "content.gr.online.scea.com",
                                    "content-nonprod.gr.online.scea.com",
                                    "content-dev.gr.online.scea.com",
                                };


        private readonly static List<string> nDreamsDomains = new()
                                {
                                    "pshome.ndreams.net",
                                    "www.ndreamshs.com",
                                    "www.ndreamsportal.com",
                                    "nDreams-multiserver-cdn",
                                    "www.ndreamsgateway.com"
                                };

        private readonly static List<string> HellFireGamesDomains = new()
                                {
                                    "game.hellfiregames.com",
                                    "game2.hellfiregames.com",
                                    "holdemqa.destinations.scea.com",
                                    "holdemeu.destinations.scea.com",
                                    "holdemna.destinations.scea.com",
                                    "c93f2f1d-3946-4f37-b004-1196acf599c5.scalr.ws"
                                };

        private readonly static List<string> HTSDomains = new() {
                                    "samples.hdk.scee.net",
                                };

        private readonly static List<string> ILoveSonyDomains = new() {
                                    "www.myresistance.net",
                                };

        #endregion

        public static List<Route> frontend = new() {
                new() {
                    Name = "Default Home TSS Endpoint",
                    UrlRegex = @"^/tss/(clientconfig0001|coreHztFmpQrx0002).*",
                    Method = "GET",
                    Hosts = null,
                    Callable = (ctx) => {
                        if (!File.Exists(ctx.FilePath))
                        {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/xml";
                                return ctx.Response.Send(Path.GetFileName(ctx.FilePath)?.StartsWith("clientconfig0001") == true ? ClientConfig0001.GenerateXML() : CoreHztFmpQrx0002.GenerateXML()).Result;
                        }
                        return false;
                     }
                },
                new() {
                    Name = "Home UniqueInstanceId decypher",
                    UrlRegex = "^/DecryptUniqueInstanceID.php$",
                    Method = "POST",
                    Hosts = null,
                    Callable = (ctx) => {
                       try
                            {
                                string instanceId = ctx.Request.DataAsString;
                                if (instanceId.Length == 21)
                                {
                                    // Parse World ID (8 hex)
                                    uint worldId = uint.Parse(instanceId.Substring(0, 8), NumberStyles.HexNumber);

                                    // Parse Local ID (5 decimal)
                                    int localId = int.Parse(instanceId.Substring(8, 5));

                                    // Parse Packed Address (8 hex)
                                    IPAddress Address = InternetProtocolUtils.GetIPAddressFromUInt(uint.Parse(instanceId.Substring(13, 8), NumberStyles.HexNumber));

                                    ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                    ctx.Response.ContentType = "application/json; charset=utf-8";
                                    return ctx.Response.Send(JsonSerializer.Serialize(new
                                    {
                                        WorldId = worldId,
                                        LocalId = localId,
                                        Address = Address.ToString()
                                    })).Result;
                                }
                            }
                            catch
                            {
                            }
                            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                            return ctx.Response.Send("Invalid instance ID format.").Result;
                     }
                },
                new() {
                    Name = "Ubisoft MasterAdServerInitXml",
                    UrlRegex = "^/MasterAdServerWS/MasterAdServerWS.asmx/InitXml",
                    Method = "POST",
                    Hosts = new string[] { "master10.doublefusion.com" },
                    Callable = (ctx) => {
                        ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return ctx.Response.Send().Result;
                     }
                },
                new() {
                    Name = "Ubisoft GetOnlineConfig (including PSN)",
                    UrlRegex = "^/OnlineConfigService.svc/GetOnlineConfig",
                    Method = "GET",
                    Hosts = new string[] { "onlineconfigservice.ubi.com" },
                    Callable = (ctx) => {
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "application/json; charset=utf-8";
                        return ctx.Response.Send(JsonData.GetOnlineConfigPSN(ctx.Request.RetrieveQueryValue("onlineConfigID"))).Result;
                     }
                },
                new() {
                    Name = "Ubisoft MatchMakingConfig.aspx",
                    UrlRegex = "^/MatchMakingConfig.aspx",
                    Method = "GET",
                    Hosts = new string[] { "gconnect.ubi.com" },
                    Callable = (ctx) => {
                        string action = ctx.Request.RetrieveQueryValue("action");
                            string gid = ctx.Request.RetrieveQueryValue("gid");
                            string locale = ctx.Request.RetrieveQueryValue("locale");
                            string format = ctx.Request.RetrieveQueryValue("format");

                            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(gid) && !string.IsNullOrEmpty(locale) && !string.IsNullOrEmpty(format))
                            {
                               switch (action)
                                {
                                    case "g_mmc":
                                        switch (gid)
                                        {
                                            case "e330746d922f44e3b7c2c6e5637f2e53": // DFSPS3
                                            case "20a6ed08781847c48e4cbc4dde73fd33": // DFSPS3
                                                switch (locale)
                                                {
                                                    default:
                                                        if (format == "xml")
                                                        {
                                                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                                            ctx.Response.ContentType = "text/html; charset=utf-8"; // Not an error, packet shows this content type...
                                                            return ctx.Response.Send(XMLData.DFS_PS3_NTSC_EN_XMLPayload).Result;
                                                        }
                                                        break;
                                                }
                                                break;
                                            case "885642bfde8842b79bbcf2c1f8102403": // DFSPC
                                                switch (locale)
                                                {
                                                    default:
                                                        if (format == "xml")
                                                        {
                                                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                                            ctx.Response.ContentType = "text/html; charset=utf-8"; // Not an error, packet shows this content type...
                                                            return ctx.Response.Send(XMLData.DFS_PC_EN_XMLPayload).Result;
                                                        }
                                                        break;
                                                }
                                                break;
                                            case "0879cd6bbf17e9cbf6cf44fb35c0142f": //PBPS3
                                                switch (locale)
                                                {
                                                    default:
                                                        if (format == "xml")
                                                        {
                                                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                                            ctx.Response.ContentType = "text/html; charset=utf-8"; // Not an error, packet shows this content type...
                                                            return ctx.Response.Send(XMLData.PB_PS3_EN_XMLPayload).Result;
                                                        }
                                                        break;
                                                }
                                                break;
                                        }
                                        break;
                                }
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                            return ctx.Response.Send().Result;
                     }
                },
                new() {
                    Name = "UFC Undisputed PS Home",
                    UrlRegex = "^/index.php",
                    Method = "POST",
                    Hosts = new string[] { "sonyhome.thqsandbox.com" },
                    Callable = (ctx) => {
                        string? UFCResult = UFC2010PsHomeClass.ProcessUFCUserData(ctx.Request.DataAsBytes, HTTPProcessor.ExtractBoundary(ctx.Request.ContentType), ApacheNetServerConfiguration.APIStaticFolder);
                            if (!string.IsNullOrEmpty(UFCResult))
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/xml";
                                return ctx.Response.Send(UFCResult).Result;
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            return ctx.Response.Send().Result;
                     }
                },
                new() {
                    Name = "Home Firing Range leaderboard system",
                    UrlRegex = "^/rchome/leaderboard.py/",
                    Method = "POST",
                    Hosts = null,
                    Callable = (ctx) => {
                        string? RCHOMEResult = new RCHOMEClass(ctx.Request.Method.ToString(), ctx.Request.Url.RawWithoutQuery, ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType);
                            if (!string.IsNullOrEmpty(RCHOMEResult))
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/xml";
                                return ctx.Response.Send(RCHOMEResult).Result;
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            return ctx.Response.Send().Result;
                     }
                },
                new() {
                    Name = "COGS writeDB scoreboard",
                    UrlRegex = "^/Cogs/Development/Single/writeDB.php$",
                    Method = "POST",
                    Hosts = new string[] { "ec2-174-129-44-204.compute-1.amazonaws.com" },
                    Callable = (ctx) => {
                        string? COGSResult = new COGSClass(ctx.Request.Method.ToString(), ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType);
                            if (!string.IsNullOrEmpty(COGSResult))
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/xml";
                                return ctx.Response.Send(COGSResult).Result;
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            return ctx.Response.Send().Result;
                     }
                },
                 new() {
                    Name = "COGS readDB scoreboard",
                    UrlRegex = "^/Cogs/Development/Single/readDB.php$",
                    Method = "GET",
                    Hosts = new string[] { "ec2-174-129-44-204.compute-1.amazonaws.com" },
                    Callable = (ctx) => {
                        string? COGSResult = new COGSClass(ctx.Request.Method.ToString(), ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType);
                            if (!string.IsNullOrEmpty(COGSResult))
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/xml";
                                return ctx.Response.Send(COGSResult).Result;
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            return ctx.Response.Send().Result;
                     }
                },
                new() {
                    Name = "Codeglue Wipeout 2D scorepost",
                    UrlRegex = "^/wipeout/scorepost.php$",
                    Method = "POST",
                    Hosts = new string[] { "sonyhome.codeglue.com" },
                    Callable = (ctx) => {
                        string? Wipoeut2DResult = new WipeoutShooterClass(ctx.Request.Method.ToString(), ApacheNetServerConfiguration.APIStaticFolder)
                            .ProcessRequest(HTTPProcessor.GetQueryParameters(HTTPProcessor.DecodeUrl(ctx.Request.Url.RawWithQuery)), ctx.Request.DataAsBytes, ctx.Request.ContentType);
                            if (!string.IsNullOrEmpty(Wipoeut2DResult))
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/xml";
                                return ctx.Response.Send(Wipoeut2DResult).Result;
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            return ctx.Response.Send().Result;
                     }
                },
                 new() {
                    Name = "Codeglue Wipeout 2D scorelist",
                    UrlRegex = "^/wipeout/scorelist.php$",
                    Method = "GET",
                    Hosts = new string[] { "sonyhome.codeglue.com" },
                    Callable = (ctx) => {
                        string? Wipoeut2DResult = new WipeoutShooterClass(ctx.Request.Method.ToString(), ApacheNetServerConfiguration.APIStaticFolder)
                            .ProcessRequest(HTTPProcessor.GetQueryParameters(HTTPProcessor.DecodeUrl(ctx.Request.Url.RawWithQuery)), ctx.Request.DataAsBytes, ctx.Request.ContentType);
                            if (!string.IsNullOrEmpty(Wipoeut2DResult))
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/xml";
                                return ctx.Response.Send(Wipoeut2DResult).Result;
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            return ctx.Response.Send().Result;
                     }
                },
                new() {
                    Name = "Home Athletic games leaderboard system",
                    UrlRegex = "^/entryBare.php",
                    Method = "POST",
                    Hosts = new string[] { "homeleaderboards.software.eu.playstation.com" },
                    Callable = (ctx) => {
                        string? EntryBareResult = HOMELEADERBOARDSClass.ProcessEntryBare(ctx.Request.DataAsBytes, HTTPProcessor.ExtractBoundary(ctx.Request.ContentType), ApacheNetServerConfiguration.APIStaticFolder);
                            if (!string.IsNullOrEmpty(EntryBareResult))
                            {
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/xml";
                                return ctx.Response.Send(EntryBareResult).Result;
                            }

                            ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            return ctx.Response.Send().Result;
                     }
                },
                new() {
                    Name = "aurora_stats",
                    UrlRegex = "^/scenes/aurora_stats.xml",
                    Method = "GET",
                    Hosts = new string[] { "ndreams.stats.s3.amazonaws.com" },
                    Callable = (ctx) => {
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/xml";
                            return ctx.Response.Send("<?xml version=\"1.0\" encoding=\"utf-8\"?>\n" +
                                "<STATS>\n\t" +
                                "<TRACKING active=\"false\"/>\n\t" +
                                "<PURCHASE active=\"false\"/>\n\n\t" +
                                "<VISIT active=\"true\">\n\t\t" +
                                "<URL>http://pshome.ndreams.net/aurora/visit.php</URL>\n\t" +
                                "</VISIT>\n" +
                                "</STATS>").Result;
                     }
                },
                new() {
                    Name = "aurora_mystery",
                    UrlRegex = "^/scenes/mystery.xml",
                    Method = "GET",
                    Hosts = new string[] { "ndreams.stats.s3.amazonaws.com" },
                    Callable = (ctx) => {
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/xml";
                            return ctx.Response.Send("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                                "<mystery>\r\n\t" +
                                "<https url=\"http://pshome.ndreams.net/aurora/MysteryItems/mystery3.php\"/>\r\n" +
                                "</mystery>").Result;
                     }
                },
                new() {
                    Name = "aurora_stats_config",
                    UrlRegex = "^/aurora/aurora_stats_config.xml",
                    Method = "GET",
                    Hosts = new string[] { "ndreams.stats.s3.amazonaws.com" },
                    Callable = (ctx) => {
                         ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/xml";
                            return ctx.Response.Send("<stats>\n" +
                                "<pstats id=\"general\" data=\"ndreams.stats.s3.amazonaws.com/aurora\" target=\"pshome.ndreams.net/aurora\" fallover=\"pshome.ndreams.net/aurora\"/>\n" +
                                "</stats>").Result;
                     }
                },
                new() {
                    Name = "xi2_stats_config",
                    UrlRegex = "^/xi2/xi2_stats_config.xml",
                    Method = "GET",
                    Hosts = new string[] { "ndreams.stats.s3.amazonaws.com" },
                    Callable = (ctx) => {
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/xml";
                            return ctx.Response.Send("<stats>\n\t" +
                                    "<pstats id=\"xi2\" data=\"ndreams.stats.s3.amazonaws.com/xi2\" target=\"pshome.ndreams.net/xi2/cont\" fallover=\"pshome.ndreams.net/xi2/cont\"/>\n\t" +
                                    "<pstats id=\"general\" data=\"ndreams.stats.s3.amazonaws.com/aurora\" target=\"pshome.ndreams.net/aurora\" fallover=\"pshome.ndreams.net/aurora\"/>\n" +
                                    "</stats>").Result;
                     }
                },
                new() {
                    Name = "ansda_stats",
                    UrlRegex = "^/ndreams.stats/scenes/ansda_stats.xml",
                    Method = "GET",
                    Hosts = new string[] { "s3.amazonaws.com" },
                    Callable = (ctx) => {
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/xml";
                            return ctx.Response.Send(@"<?xml version=""1.0"" encoding=""utf-8""?>
                                <STATS>
	                                <TRACKING active=""false"">
		                                <TIMING>10</TIMING>
		                                <SIZE>30</SIZE>
		                                <URL>http://pshome.ndreams.net/legacy/ansada/track.php</URL>
	                                </TRACKING>
	                                <PURCHASE active=""false"">
		                                <ITEMS url=""https://s3.amazonaws.com/ndreams.stats/scenes/ansdaobjs.txt"">
		                                </ITEMS>
		                                <ZONES>
			                                <ZONE x=""-15.21987"" y=""4.02206"" z=""-2.13493"" radius = ""3""/>
		                                </ZONES>
		                                <URL>http://pshome.ndreams.net/legacy/ansada/purchase.php</URL>
	                                </PURCHASE>
	                                <VISIT active=""false"">
		                                <URL>http://pshome.ndreams.net/legacy/ansada/visit.php</URL>
	                                </VISIT>
                                </STATS>").Result;
                     }
                },
                new() {
                    Name = "ndreams objs",
                    UrlRegex = "objs.txt",
                    Method = "GET",
                    HostCriteria = "s3.amazonaws.com",
                    Callable = (ctx) => {
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/plain";
                            return ctx.Response.Send(File.Exists(ctx.FilePath) ? File.ReadAllText(ctx.FilePath) : HTTPProcessor.RequestURLGET($"https://{ctx.GetHost()}{ctx.Request.Url.RawWithQuery}")).Result;
                     }
                }
            };

        public static List<Route> backend = new() {
                new() {
                    Name = "EA Demangler",
                    Hosts = new string[] { "demangler.ea.com" },
                    Callable = (ctx) => {
                                HttpStatusCode statusCode;

                                ctx.Response.ChunkedTransfer = false;
                                ctx.Response.ProtocolVersion = "1.0";

                                (string?, string?)? res = DemanglerClass.ProcessDemanglerRequest(ctx.Request.Query.Elements.ToDictionary(), ctx.ServerIP, ctx.AbsolutePath, ctx.Request.DataAsBytes);
                                bool hasResult = res != null;
                                if (!hasResult)
                                    statusCode = HttpStatusCode.InternalServerError;
                                else
                                {
                                    statusCode = HttpStatusCode.OK;
                                    ctx.Response.Headers.Add("x-envoy-upstream-service-time", "0");
                                    ctx.Response.Headers.Add("server", "istio-envoy");
                                    ctx.Response.Headers.Add("content-length", res!.Value.Item1!.Length.ToString());
                                    ctx.Response.ContentType = res.Value.Item2;
                                }

                                ctx.Response.StatusCode = (int)statusCode;
                                return ctx.Response.Send(hasResult ? res!.Value.Item1 : null).Result;
                     }
                },
                new() {
                    Name = "VEEMEE API",
                    UrlRegex = @".*\.(php|xml)$",
                    Hosts = new string[] { "away.veemee.com", "home.veemee.com", "ww-prod-sec.destinations.scea.com", "ww-prod.destinations.scea.com" },
                    Callable = (ctx) => {
                                HttpStatusCode statusCode;
                                ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                                if (((ctx.AbsolutePath.EndsWith(".php") && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder)) || ctx.AbsolutePath.EndsWith(".xml")) && (File.Exists(ctx.FilePath) || File.Exists(ctx.ApiPath)))
                                {
                                    // Let main server handler handle it.
                                }
                                else
                                {
                                    (byte[]?, string?) res = new VEEMEEClass(ctx.Request.Method.ToString(), ctx.AbsolutePath).ProcessRequest(ctx.Request.ContentLength > 0 ? ctx.Request.DataAsBytes : null, ctx.Request.ContentType, ApacheNetServerConfiguration.APIStaticFolder);
                                    if (res.Item1 == null || res.Item1.Length == 0)
                                        statusCode = HttpStatusCode.InternalServerError;
                                    else
                                    {
                                        ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        statusCode = HttpStatusCode.OK;
                                    }
                                    ctx.Response.StatusCode = (int)statusCode;
                                    if (!string.IsNullOrEmpty(res.Item2))
                                        ctx.Response.ContentType = res.Item2;
                                    else
                                        ctx.Response.ContentType = "text/plain";
                                    if (ctx.Response.ChunkedTransfer)
                                        return ctx.Response.SendChunk(res.Item1, true).Result;
                                    else
                                        return ctx.Response.Send(res.Item1).Result;
                                }

                                return false;
                     }
                },
                new() {
                    Name = "NDREAMS API",
                    UrlRegex = @".*(\.php|/NDREAMS/|/gateway/).*",
                    Hosts = nDreamsDomains.ToArray(),
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        if (ctx.AbsolutePath.EndsWith(".php") && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder) && (File.Exists(ctx.FilePath) || File.Exists(ctx.ApiPath)))
                                {
                                    // Let main server handler handle it.
                                }
                                else
                                {
                                    string? res = new NDREAMSClass(ctx.CurrentDate, ctx.Request.Method.ToString(), ctx.ApiPath, $"{(ctx.Secure ? "https" : "http")}://nDreams-multiserver-cdn/", $"{(ctx.Secure ? "https" : "http")}://{ctx.GetHost()}{ctx.FullUrl}", ctx.AbsolutePath,
                                         ApacheNetServerConfiguration.APIStaticFolder, ctx.GetHost()).ProcessRequest(ctx.Request.Query.Elements.ToDictionary(), ctx.Request.DataAsBytes, ctx.Request.ContentType);
                                    if (string.IsNullOrEmpty(res))
                                    {
                                        ctx.Response.ContentType = "text/plain";
                                        statusCode = HttpStatusCode.InternalServerError;
                                    }
                                    else
                                    {
                                        ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        ctx.Response.ContentType = "text/xml";
                                        statusCode = HttpStatusCode.OK;
                                    }
                                    ctx.Response.StatusCode = (int)statusCode;
                                    if (ctx.Response.ChunkedTransfer)
                                        return ctx.Response.SendChunk(!string.IsNullOrEmpty(res) ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                    else
                                        return ctx.Response.Send(res).Result;
                                }

                        return false;
                    }
                },
                new() {
                    Name = "HELLFIRE API",
                    UrlRegex = @".*\.(php|jpg)$",
                    Hosts = HellFireGamesDomains.ToArray(),
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        if (ctx.AbsolutePath.EndsWith(".php") && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder) && (File.Exists(ctx.FilePath) || File.Exists(ctx.ApiPath)))
                                {
                                    // Let main server handler handle it.
                                }
                                else
                                {
                                    byte[] res = new HELLFIREClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType, ctx.Secure);
                                    if (res == null)
                                    {
                                        ctx.Response.ContentType = "text/plain";
                                        statusCode = HttpStatusCode.InternalServerError;
                                    }
                                    else
                                    {
                                        ctx.Response.ContentType = "application/xml;charset=UTF-8";
                                        ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        statusCode = HttpStatusCode.OK;
                                    }
                                    ctx.Response.StatusCode = (int)statusCode;
                                    if (ctx.Response.ChunkedTransfer)
                                        return ctx.Response.SendChunk(res, true).Result;
                                    else
                                        return ctx.Response.Send(res).Result;
                                }

                        return false;
                    }
                },
                new() {
                    Name = "Heavy Water API",
                    Hosts = new[] { "secure.heavyh2o.net", "services.heavyh2o.net", "www.services.heavyh2o.net" },
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        ctx.Response.ContentType = "application/json";
                                statusCode = HttpStatusCode.OK;

                                string res = new HeavyWaterClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.Query.Elements.ToDictionary(), ctx.Request.DataAsBytes, ctx.Request.ContentType);
                                if (string.IsNullOrEmpty(res))
                                    res = "{\"STATUS\":\"FAILURE\"}";

                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(Encoding.UTF8.GetBytes(res), true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                    }
                },
                new() {
                    Name = "OHS API",
                    UrlRegex = @"/(ohs_[^/]*|ohs|statistic|Konami|tracker)/.*\/$",
                    Hosts = new[] { "stats.outso-srv1.com", "www.outso-srv1.com", "ec2-184-72-239-107.compute-1.amazonaws.com" },
                    Callable = (ctx) => {
                         HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        string absolutepath = ctx.AbsolutePath;

                        #region OHS API Version
                                int? version = null;

                                if (!ctx.Secure)
                                {
                                    Dictionary<int, (string, bool)[]> versionMap = new Dictionary<int, (string, bool)[]>
                                    {
                                        [2] = new[]
                                        {
                                            ("/Insomniac/4BarrelsOfFury/", false),
                                            ("/SCEA/SaucerPop/", false),
                                            ("/AirRace/", false),
                                            ("/Flugtag/", false)
                                        },
                                        [1] = new[]
                                        {
                                            ("/SCEA/op4_", false),
                                            ("/uncharted2", true),
                                            ("/SuckerPunch/", false),
                                            ("/warhawk_shooter/", false),
                                            ("/SCEA/WorldDomination/", false)
                                        }
                                    };

                                    foreach (var kvp in versionMap)
                                    {
                                        foreach (var tuple in kvp.Value)
                                        {
                                            if (absolutepath.Contains(tuple.Item1, tuple.Item2 ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture))
                                            {
                                                version = kvp.Key;
                                                break;
                                            }
                                        }

                                        if (version.HasValue)
                                            break;
                                    }
                                }
                                #endregion

                                string? res = new OHSClass(ctx.Request.Method.ToString(), absolutepath, version ?? 0).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType, ctx.ApiPath);
                                if (string.IsNullOrEmpty(res))
                                {
                                    ctx.Response.ContentType = "text/plain";
                                    statusCode = HttpStatusCode.InternalServerError;
                                }
                                else
                                {
                                    using (var sw = new StringWriter())
                                    using (var xw = XmlWriter.Create(sw, new XmlWriterSettings { OmitXmlDeclaration = true }))
                                    {
                                        xw.WriteStartElement("ohs");
                                        xw.WriteCData(res); // Uses CDATA (natively handled by the Home client) to properly insert crypto data.
                                        xw.WriteEndElement();
                                        xw.Flush();
                                        res = sw.ToString();
                                    }
                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = "application/xml;charset=UTF-8";
                                    statusCode = HttpStatusCode.OK;
                                }
                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(!string.IsNullOrEmpty(res) ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                    }
                },
                new() {
                    Name = "OUWF Debug API",
                    Hosts = new[] { "ouwf.outso-srv1.com" },
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        string? res = new OuWFClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.HTTPStaticFolder).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType);
                                if (string.IsNullOrEmpty(res))
                                {
                                    ctx.Response.ContentType = "text/plain";
                                    statusCode = HttpStatusCode.InternalServerError;
                                }
                                else
                                {
                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = "text/xml";
                                    statusCode = HttpStatusCode.OK;
                                }
                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(!string.IsNullOrEmpty(res) ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                    }
                },
                new() {
                    Name = "Playmetrix Stats API",
                    Hosts = new[] { "stats.playmetrix.com" },
                    Callable = (ctx) => {
                        ctx.Response.ChunkedTransfer = false;
                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                ctx.Response.ContentType = "text/plain";
                                return ctx.Response.Send().Result;
                    }
                },
                new() {
                    Name = "LOOT API",
                    Hosts = new[] { "server.lootgear.com", "alpha.lootgear.com" },
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        string? res = new LOOTClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.Query.Elements.ToDictionary(), ctx.Request.DataAsBytes, ctx.Request.ContentType);
                                if (string.IsNullOrEmpty(res))
                                {
                                    ctx.Response.ContentType = "text/plain";
                                    statusCode = HttpStatusCode.InternalServerError;
                                }
                                else
                                {
                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = "application/xml;charset=UTF-8";
                                    statusCode = HttpStatusCode.OK;
                                }
                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(!string.IsNullOrEmpty(res) ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                    }
                },
                new() {
                    Name = "Juggernaut Games API",
                    UrlRegex =  @".*\.php$",
                    Hosts = new[] { "juggernaut-games.com" },
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        if (ctx.AbsolutePath.EndsWith(".php") && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder) && (File.Exists(ctx.FilePath) || File.Exists(ctx.ApiPath)))
                                {
                                    // Let main server handler handle it.
                                }
                                else
                                {
                                    string? res = null;
                                    JUGGERNAUTClass juggernaut = new(ctx.Request.Method.ToString(), ctx.AbsolutePath);
                                    if (ctx.Request.ContentLength > 0)
                                        res = juggernaut.ProcessRequest(HTTPProcessor.GetQueryParameters(ctx.FullUrl), ApacheNetServerConfiguration.APIStaticFolder, ctx.Request.DataAsBytes, ctx.Request.ContentType);
                                    else
                                        res = juggernaut.ProcessRequest(HTTPProcessor.GetQueryParameters(ctx.FullUrl), ApacheNetServerConfiguration.APIStaticFolder);

                                    if (res == null)
                                        statusCode = HttpStatusCode.InternalServerError;
                                    else if (res == string.Empty)
                                    {
                                        ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        ctx.Response.ContentType = "text/plain";
                                        statusCode = HttpStatusCode.OK;
                                    }
                                    else
                                    {
                                        ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        ctx.Response.ContentType = "text/xml";
                                        statusCode = HttpStatusCode.OK;
                                    }

                                    ctx.Response.StatusCode = (int)statusCode;
                                    if (ctx.Response.ChunkedTransfer)
                                        return ctx.Response.SendChunk(res != null ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                    else
                                        return ctx.Response.Send(res).Result;
                                }

                        return false;
                    }
                },
                new() {
                    Name = "Digital Leisure Casino API",
                    UrlRegex = @".*\.php$",
                    Hosts = new[] { "root.pshomecasino.com" },
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        if (ctx.AbsolutePath.EndsWith(".php") && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder) && (File.Exists(ctx.FilePath) || File.Exists(ctx.ApiPath)))
                                {
                                    // Let main server handler handle it.
                                }
                                else
                                {
                                    string? res = new DLCasinoClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.Query.Elements.ToDictionary(), ctx.Request.DataAsBytes, ctx.Request.ContentType);

                                    if (res == null)
                                        statusCode = HttpStatusCode.InternalServerError;
                                    else
                                    {
                                        ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        ctx.Response.ContentType = "text/xml";
                                        statusCode = HttpStatusCode.OK;
                                    }

                                    ctx.Response.StatusCode = (int)statusCode;
                                    if (ctx.Response.ChunkedTransfer)
                                        return ctx.Response.SendChunk(res != null ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                    else
                                        return ctx.Response.Send(res).Result;
                                }

                        return false;
                    }
                },
                new() {
                    Name = "PREMIUMAGENCY API",
                    UrlRegex = @".*/eventController/.*",
                    Hosts = new[] {
                        "test.playstationhome.jp",
                        "playstationhome.jp",
                        "homeec.scej-nbs.jp",
                        "homeecqa.scej-nbs.jp",
                        "homect-scej.jp",
                        "qa-homect-scej.jp",
                        "home-eas.jp.playstation.com"
                    },
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        string? res = new PREMIUMAGENCYClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder, ctx.FullUrl).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType);
                                if (string.IsNullOrEmpty(res))
                                {
                                    ctx.Response.ContentType = "text/plain";
                                    statusCode = HttpStatusCode.InternalServerError;
                                }
                                else
                                {
                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = "text/xml";
                                    statusCode = HttpStatusCode.OK;
                                }
                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(!string.IsNullOrEmpty(res) ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                    }
                },
                new() {
                    Name = "FROMSOFTWARE API",
                    Hosts = new[] { "acvd-ps3ww-cdn.fromsoftware.jp" },
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        (byte[]?, string?, string[][]?) res = res = new FROMSOFTWAREClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType);

                                if (res.Item1 == null || string.IsNullOrEmpty(res.Item2) || res.Item3?.Length == 0)
                                    statusCode = HttpStatusCode.InternalServerError;
                                else
                                {
                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = res.Item2;
                                    statusCode = HttpStatusCode.OK;
                                    foreach (string[] innerArray in res.Item3!)
                                    {
                                        // Ensure the inner array has at least two elements
                                        if (innerArray.Length >= 2)
                                            // Extract two values from the inner array
                                            ctx.Response.Headers.Add(innerArray[0], innerArray[1]);
                                    }
                                }

                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(res.Item1, true).Result;
                                else
                                    return ctx.Response.Send(res.Item1).Result;
                    }
                },
                new() {
                    Name = "UBISOFT API",
                    HostCriteria = "api-ubiservices.ubi.com",
                    UserAgentCriteria = "UbiServices_SDK_HTTP_Client",
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        string Authorization = ctx.Request.RetrieveHeaderValue("Authorization");

                                if (!string.IsNullOrEmpty(Authorization))
                                {
                                    // TODO, verify ticket data for every platforms.

                                    if (Authorization.StartsWith("psn t="))
                                    {
                                        (bool, byte[]) base64Data = Authorization.Replace("psn t=", string.Empty).IsBase64();

                                        if (base64Data.Item1)
                                        {
                                            // get ticket
                                            XI5Ticket ticket = XI5Ticket.ReadFromBytes(base64Data.Item2);

                                            // setup username
                                            string username = ticket.Username;

                                            // invalid ticket
                                            if (!ticket.Valid)
                                            {
                                                // log to console
                                                LoggerAccessor.LogWarn($"[HERMES] - User {username} tried to alter their ticket data");

                                                ctx.Response.ChunkedTransfer = false;
                                                statusCode = HttpStatusCode.Forbidden;
                                                ctx.Response.StatusCode = (int)statusCode;
                                                ctx.Response.ContentType = "text/plain";
                                                return ctx.Response.Send().Result;
                                            }

                                            // RPCN
                                            if (ticket.IsSignedByRPCN)
                                                LoggerAccessor.LogInfo($"[HERMES] - User {username} connected at: {DateTime.Now} and is on RPCN");
                                            else if (username.EndsWith($"@{XI5Ticket.RPCNSigner}"))
                                            {
                                                LoggerAccessor.LogError($"[HERMES] - User {username} was caught using a RPCN suffix while not on it!");

                                                ctx.Response.ChunkedTransfer = false;
                                                statusCode = HttpStatusCode.Forbidden;
                                                ctx.Response.StatusCode = (int)statusCode;
                                                ctx.Response.ContentType = "text/plain";
                                                return ctx.Response.Send().Result;
                                            }
                                            else
                                                LoggerAccessor.LogInfo($"[HERMES] - User {username} connected at: {DateTime.Now} and is on PSN");
                                        }
                                    }
                                    else if (Authorization.StartsWith("Ubi_v1 t="))
                                    {
                                        // Our JWT token is fake for now.
                                    }

                                    (string?, string?) res = new HERMESClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ctx.Request.RetrieveHeaderValue("Ubi-AppId"), ctx.Request.RetrieveHeaderValue("Ubi-RequestedPlatformType"),
                                            ctx.Request.RetrieveHeaderValue("ubi-appbuildid"), ctx.ClientIP, GeoIP.GetISOCodeFromIP(IPAddress.Parse(ctx.ClientIP)), Authorization.Replace("psn t=", string.Empty), ApacheNetServerConfiguration.APIStaticFolder)
                                        .ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType);
                                    if (string.IsNullOrEmpty(res.Item1))
                                        statusCode = HttpStatusCode.InternalServerError;
                                    else
                                    {
                                        ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                        ctx.Response.Headers.Add("Ubi-Forwarded-By", "ue1-p-us-public-nginx-056b582ac580ba328");
                                        ctx.Response.Headers.Add("Ubi-TransactionId", Guid.NewGuid().ToString());
                                        statusCode = HttpStatusCode.OK;
                                    }
                                    ctx.Response.StatusCode = (int)statusCode;
                                    if (!string.IsNullOrEmpty(res.Item2))
                                        ctx.Response.ContentType = res.Item2;
                                    else
                                        ctx.Response.ContentType = "text/plain";
                                    if (ctx.Response.ChunkedTransfer)
                                        return ctx.Response.SendChunk(!string.IsNullOrEmpty(res.Item1) ? Encoding.UTF8.GetBytes(res.Item1) : null, true).Result;
                                    else
                                        return ctx.Response.Send(res.Item1).Result;
                                }
                                else
                                {
                                    ctx.Response.ChunkedTransfer = false;
                                    statusCode = HttpStatusCode.Forbidden;
                                    ctx.Response.StatusCode = (int)statusCode;
                                    ctx.Response.ContentType = "text/plain";
                                    return ctx.Response.Send().Result;
                                }
                    }
                },
                new() {
                    Name = "Ubisoft Build API",
                    Hosts = new[] { "builddatabasepullapi" },
                    ContentTypeCriteria = "application/soap+xml",
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                        string? res = new SoapBuildAPIClass(ctx.Request.Method.ToString(), ctx.AbsolutePath).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType);
                                if (string.IsNullOrEmpty(res))
                                    statusCode = HttpStatusCode.InternalServerError;
                                else
                                {
                                    ctx.Response.ContentType = "text/xml";
                                    statusCode = HttpStatusCode.OK;
                                }

                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(!string.IsNullOrEmpty(res) ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                    }
                },
                new() {
                    Name = "gsconnect API",
                    Hosts = new[] { "gsconnect.ubisoft.com" },
                    Callable = (ctx) => {
                        HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = false;
                                ctx.Response.ProtocolVersion = "1.0";

                                (string?, string?, Dictionary<string, string>?) res;
                                gsconnectClass gsconn = new(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder);
                                if (ctx.Request.ContentLength > 0)
                                    res = gsconn.ProcessRequest(ctx.Request.Query.Elements.ToDictionary(), ctx.Request.DataAsBytes, ctx.Request.ContentType);
                                else
                                    res = gsconn.ProcessRequest(ctx.Request.Query.Elements.ToDictionary());

                                if (string.IsNullOrEmpty(res.Item1) || string.IsNullOrEmpty(res.Item2))
                                    statusCode = HttpStatusCode.InternalServerError;
                                else
                                {
                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = res.Item2;
                                    statusCode = HttpStatusCode.OK;
                                    if (res.Item3 != null)
                                    {
                                        foreach (KeyValuePair<string, string> header in res.Item3)
                                        {
                                            ctx.Response.Headers.Add(header.Key, header.Value);
                                        }
                                    }
                                }

                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(!string.IsNullOrEmpty(res.Item1) ? Encoding.UTF8.GetBytes(res.Item1) : null, true).Result;
                                else
                                    return ctx.Response.Send(res.Item1).Result;
                    }
                },
                new() {
                    Name = "CentralDispatchManager API",
                    Hosts = HPDDomains.ToArray(),
                    Callable = (ctx) => {
                         HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                                string? res = new CDMClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType);
                                if (string.IsNullOrEmpty(res))
                                {
                                    ctx.Response.ContentType = "text/plain";
                                    statusCode = HttpStatusCode.InternalServerError;
                                }
                                else
                                {
                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = "text/xml";
                                    statusCode = HttpStatusCode.OK;
                                }
                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(!string.IsNullOrEmpty(res) ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                    }
                },
                new() {
                    Name = "CAPONE GriefReporter API",
                    Hosts = CAPONEDomains.ToArray(),
                    Callable = (ctx) => {
                         HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                                string? res = new CAPONEClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType, ctx.Secure);
                                if (string.IsNullOrEmpty(res))
                                {
                                    ctx.Response.ContentType = "text/plain";
                                    statusCode = HttpStatusCode.InternalServerError;
                                }
                                else
                                {
                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = "text/xml";
                                    statusCode = HttpStatusCode.OK;
                                }
                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(!string.IsNullOrEmpty(res) ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                    }
                },
                new() {
                    Name = "HTS Samples API",
                    Hosts = HTSDomains.ToArray(),
                    Callable = (ctx) => {
                         HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                                string? res = null;
                                if (ctx.Request.ContentLength > 0)
                                    res = new HTSClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType, ctx.Secure);
                                if (string.IsNullOrEmpty(res))
                                    statusCode = HttpStatusCode.InternalServerError;
                                else
                                {
                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = "text/xml";
                                    statusCode = HttpStatusCode.OK;
                                }

                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(!string.IsNullOrEmpty(res) ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                    }
                },
                new() {
                    Name = "XI Status API",
                    Hosts = new string[] { "www.bigbenuk.com" },
                    UrlRegex = "^/status/status.xml$",
                    Callable = (ctx) => {
                                ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                                string res = WebAPIService.GameServices.PSHOME.NDREAMS.Xi1.StatusBuilder.BuildStatusXml();
                                ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                ctx.Response.ContentType = "text/xml";

                                ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(Encoding.UTF8.GetBytes(res), true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                     }
                },
                new() {
                    Name = "ILoveSony API",
                    Hosts = ILoveSonyDomains.ToArray(),
                    Callable = (ctx) => {
                         HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                                string? res = null;
                                if (ctx.Request.ContentLength > 0)
                                    res = new ILoveSonyClass(ctx.Request.Method.ToString(), ctx.AbsolutePath, ApacheNetServerConfiguration.APIStaticFolder).ProcessRequest(ctx.Request.DataAsBytes, ctx.Request.ContentType, ctx.Secure);
                                if (string.IsNullOrEmpty(res))
                                    statusCode = HttpStatusCode.InternalServerError;
                                else
                                {
                                    ctx.Response.Headers.Add("Date", DateTime.Now.ToString("r"));
                                    ctx.Response.ContentType = "text/plain";
                                    statusCode = HttpStatusCode.OK;
                                }

                                ctx.Response.StatusCode = (int)statusCode;
                                if (ctx.Response.ChunkedTransfer)
                                    return ctx.Response.SendChunk(!string.IsNullOrEmpty(res) ? Encoding.UTF8.GetBytes(res) : null, true).Result;
                                else
                                    return ctx.Response.Send(res).Result;
                    }
                },
                new() {
                    Name = "PSH Central",
                    UrlRegex = @"^/PrivateRTE/checkAuth\.php$",
                    Hosts = new string[] { "apps.pshomecentral.net" },
                    Callable = (ctx) => {
                         HttpStatusCode statusCode;
                        ctx.Response.ChunkedTransfer = ctx.AcceptChunked;

                                if (ctx.AbsolutePath.EndsWith(".php") && Directory.Exists(ApacheNetServerConfiguration.PHPStaticFolder) && (File.Exists(ctx.FilePath) || File.Exists(ctx.ApiPath)))
                                {
                                    // Let main server handler handle it.
                                }
                                else
                                {
                                    statusCode = HttpStatusCode.OK;
                                    ctx.Response.StatusCode = (int)statusCode;
                                    ctx.Response.ContentType = "text/plain";
                                    if (ctx.Response.ChunkedTransfer)
                                        return ctx.Response.SendChunk(Encoding.UTF8.GetBytes("false"), true).Result;
                                    else
                                        return ctx.Response.Send("false").Result;
                                }

                               return false;
                    }
                },
            };
    }
}
