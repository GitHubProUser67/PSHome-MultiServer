using NetworkLibrary.HTTP;
using System.Collections.Generic;
using System.Net;
using WebAPIService.THQ;
using WebAPIService.RCHOME;
using WatsonWebserver.Core;
using WebAPIService.HOMELEADERBOARDS;
using WebAPIService.CODEGLUE;
using System;
using CustomLogger;
using System.Threading.Tasks;
using System.Globalization;
using NetworkLibrary.Extension;
using System.Text.Json;
using WebAPIService.COGS;
using WebAPIService.AdobeFlash.binaries.JwPlayer;

namespace ApacheNet.RouteHandlers
{
    public class Main
    {
        public static List<Route> index = new() {
                new() {
                    Name = "Server shutdown endpoint",
                    UrlRegex = "^/shutdown$",
                    Method = "GET",
                    Host = null,
                    Callable = (HttpContextBase ctx) => {
                        string ipAddr = ctx.Request.Source.IpAddress;
                        if (!string.IsNullOrEmpty(ipAddr) && ((ApacheNetServerConfiguration.AllowedManagementIPs != null && ApacheNetServerConfiguration.AllowedManagementIPs.Contains(ipAddr))
                        || "::1".Equals(ipAddr) || "127.0.0.1".Equals(ipAddr) || "localhost".Equals(ipAddr, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            LoggerAccessor.LogWarn($"[Main] - Allowed IP:{ipAddr} issued a server shutdown command at:{DateTime.Now}.");
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.Send("Shutdown initiated.").Wait();
                            LoggerAccessor.LogInfo("Shutting down. Goodbye!");
                            Environment.Exit(0);
                        }
                        LoggerAccessor.LogError($"[Main] - IP:{ipAddr} tried to issue a server shutdown command at:{DateTime.Now}, but this is not allowed for this address!");
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.Send().Wait();
                        return true;
                     }
                },
                new() {
                    Name = "Server reboot endpoint",
                    UrlRegex = "^/reboot$",
                    Method = "GET",
                    Host = null,
                    Callable = (HttpContextBase ctx) => {
                        string ipAddr = ctx.Request.Source.IpAddress;
                        if (!string.IsNullOrEmpty(ipAddr) && ((ApacheNetServerConfiguration.AllowedManagementIPs != null && ApacheNetServerConfiguration.AllowedManagementIPs.Contains(ipAddr))
                        || "::1".Equals(ipAddr) || "127.0.0.1".Equals(ipAddr) || "localhost".Equals(ipAddr, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            LoggerAccessor.LogWarn($"[Main] - Allowed IP:{ipAddr} issued a server reboot command at:{DateTime.Now}.");
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.Send("Reboot initiated.").Wait();
                            _ = Task.Run(() => {
                                LoggerAccessor.LogInfo("Rebooting!");

                                ApacheNetServerConfiguration.RefreshVariables(Program.configPath);

                                Program.StartOrUpdateServer();
                            });
                            return true;
                        }
                        LoggerAccessor.LogError($"[Main] - IP:{ipAddr} tried to issue a server reboot command at:{DateTime.Now}, but this is not allowed for this address!");
                        ctx.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        ctx.Response.Send().Wait();
                        return true;
                     }
                },
                new() {
                    Name = "Home UniqueInstanceId decypher",
                    UrlRegex = "^/DecryptUniqueInstanceID.php$",
                    Method = "POST",
                    Host = null,
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                        return false;
                     }
                },
                new() {
                    Name = "AdobeFlash PS3 Player",
                    UrlRegex = "jwplayer/jwplayer",
                    Method = "GET",
                    Host = null,
                    Callable = (HttpContextBase ctx) => {
                        ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                        ctx.Response.ContentType = "application/x-shockwave-flash";
                        if (ctx.Request.Url.RawWithoutQuery.EndsWith("53.swf", StringComparison.InvariantCultureIgnoreCase))
                            return ctx.Response.Send(jwPlayer53Swf.Data).Result;
                        else if (ctx.Request.Url.RawWithoutQuery.EndsWith("43.swf", StringComparison.InvariantCultureIgnoreCase))
                            return ctx.Response.Send(jwPlayer43Swf.Data).Result;
                        return ctx.Response.Send(jwPlayer6Swf.Data).Result;
                     }
                },
                new() {
                    Name = "Ubisoft MasterAdServerInitXml",
                    UrlRegex = "^/MasterAdServerWS/MasterAdServerWS.asmx/InitXml",
                    Method = "POST",
                    Host = "master10.doublefusion.com",
                    Callable = (HttpContextBase ctx) => {
                        ctx.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        return ctx.Response.Send().Result;
                     }
                },
                new() {
                    Name = "Ubisoft GetOnlineConfig (including PSN)",
                    UrlRegex = "^/OnlineConfigService.svc/GetOnlineConfig",
                    Method = "GET",
                    Host = "onlineconfigservice.ubi.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "application/json; charset=utf-8";
                            return ctx.Response.Send(WebAPIService.UBISOFT.OnlineConfigService.JsonData.GetOnlineConfigPSN(ctx.Request.RetrieveQueryValue("onlineConfigID"))).Result;
                        }
                        return false;
                     }
                },
                new() {
                    Name = "Ubisoft MatchMakingConfig.aspx",
                    UrlRegex = "^/MatchMakingConfig.aspx",
                    Method = "GET",
                    Host = "gconnect.ubi.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                                                            return ctx.Response.Send(WebAPIService.UBISOFT.MatchMakingConfig.XMLData.DFS_PS3_NTSC_EN_XMLPayload).Result;
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
                                                            return ctx.Response.Send(WebAPIService.UBISOFT.MatchMakingConfig.XMLData.DFS_PC_EN_XMLPayload).Result;
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
                                                            return ctx.Response.Send(WebAPIService.UBISOFT.MatchMakingConfig.XMLData.PB_PS3_EN_XMLPayload).Result;
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
                        return false;
                     }
                },
                new() {
                    Name = "UFC Undisputed PS Home",
                    UrlRegex = "^/index.php",
                    Method = "POST",
                    Host = "sonyhome.thqsandbox.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                        return false;
                     }
                },
                new() {
                    Name = "Home Firing Range leaderboard system",
                    UrlRegex = "^/rchome/leaderboard.py/",
                    Method = "POST",
                    Host = null,
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                        return false;
                     }
                },
                new() {
                    Name = "COGS writeDB scoreboard",
                    UrlRegex = "^/Cogs/Development/Single/writeDB.php$",
                    Method = "POST",
                    Host = "ec2-174-129-44-204.compute-1.amazonaws.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                        return false;
                     }
                },
                 new() {
                    Name = "COGS readDB scoreboard",
                    UrlRegex = "^/Cogs/Development/Single/readDB.php$",
                    Method = "GET",
                    Host = "ec2-174-129-44-204.compute-1.amazonaws.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                        return false;
                     }
                },
                new() {
                    Name = "Codeglue Wipeout 2D scorepost",
                    UrlRegex = "^/wipeout/scorepost.php$",
                    Method = "POST",
                    Host = "sonyhome.codeglue.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                        return false;
                     }
                },
                 new() {
                    Name = "Codeglue Wipeout 2D scorelist",
                    UrlRegex = "^/wipeout/scorelist.php$",
                    Method = "GET",
                    Host = "sonyhome.codeglue.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                        return false;
                     }
                },
                new() {
                    Name = "Home Athletic games leaderboard system",
                    UrlRegex = "^/entryBare.php",
                    Method = "POST",
                    Host = "homeleaderboards.software.eu.playstation.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                        return false;
                     }
                },
                new() {
                    Name = "aurora_stats",
                    UrlRegex = "^/scenes/aurora_stats.xml",
                    Method = "GET",
                    Host = "ndreams.stats.s3.amazonaws.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                        return false;
                     }
                },
                new() {
                    Name = "aurora_mystery",
                    UrlRegex = "^/scenes/mystery.xml",
                    Method = "GET",
                    Host = "ndreams.stats.s3.amazonaws.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/xml";
                            return ctx.Response.Send("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                                "<mystery>\r\n\t" +
                                "<https url=\"http://pshome.ndreams.net/aurora/MysteryItems/mystery3.php\"/>\r\n" +
                                "</mystery>").Result;
                        }
                        return false;
                     }
                },
                new() {
                    Name = "aurora_stats_config",
                    UrlRegex = "^/aurora/aurora_stats_config.xml",
                    Method = "GET",
                    Host = "ndreams.stats.s3.amazonaws.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/xml";
                            return ctx.Response.Send("<stats>\n" +
                                "<pstats id=\"general\" data=\"ndreams.stats.s3.amazonaws.com/aurora\" target=\"pshome.ndreams.net/aurora\" fallover=\"pshome.ndreams.net/aurora\"/>\n" +
                                "</stats>").Result;
                        }
                        return false;
                     }
                },
                new() {
                    Name = "xi2_stats_config",
                    UrlRegex = "^/xi2/xi2_stats_config.xml",
                    Method = "GET",
                    Host = "ndreams.stats.s3.amazonaws.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/xml";
                            return ctx.Response.Send("<stats>\n\t" +
                                    "<pstats id=\"xi2\" data=\"ndreams.stats.s3.amazonaws.com/xi2\" target=\"pshome.ndreams.net/xi2/cont\" fallover=\"pshome.ndreams.net/xi2/cont\"/>\n\t" +
                                    "<pstats id=\"general\" data=\"ndreams.stats.s3.amazonaws.com/aurora\" target=\"pshome.ndreams.net/aurora\" fallover=\"pshome.ndreams.net/aurora\"/>\n" +
                                    "</stats>").Result;
                        }
                        return false;
                     }
                },
                new() {
                    Name = "ansda_stats",
                    UrlRegex = "^/ndreams.stats/scenes/ansda_stats.xml",
                    Method = "GET",
                    Host = "s3.amazonaws.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
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
                        return false;
                     }
                },
                new() {
                    Name = "ndreams objs",
                    UrlRegex = "objs.txt",
                    Method = "GET",
                    Host = "s3.amazonaws.com",
                    Callable = (HttpContextBase ctx) => {
                        if (ApacheNetServerConfiguration.EnableBuiltInPlugins)
                        {
                            ctx.Response.StatusCode = (int)HttpStatusCode.OK;
                            ctx.Response.ContentType = "text/plain";
                            return ctx.Response.Send(HTTPProcessor.RequestURLGET($"https://s3.amazonaws.com{ctx.Request.Url.RawWithQuery}")).Result;
                        }
                        return false;
                     }
                }
            };
    }
}
