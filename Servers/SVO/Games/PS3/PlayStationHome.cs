using CustomLogger;
using MultiServerLibrary.Extension;
using SpaceWizards.HttpListener;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SVO.Games.PS3
{
    public class PlayStationHome
    {
        public static async Task Home_SVO(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                if (request.Url == null)
                {
                    response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                    return;
                }

                string? method = request.HttpMethod;

                using (response)
                {
                    switch (request.Url.AbsolutePath)
                    {
                        #region HOME
                        case "/HUBPS3_SVML/unity/closed_beta_start.jsp": // Home 0.41 path.

                            switch (method)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string regionNoptions = string.Empty;

                                        try
                                        {
                                            regionNoptions = request.Url.Query.Substring(8);
                                        }
                                        catch (Exception)
                                        {
                                            regionNoptions = "en-US";
                                        }

                                        regionNoptions = regionNoptions.Replace("&", "&amp;");

                                        string domain = "homeps3.svo.online.scee.com";

                                        if (!SVOServerConfiguration.PreferDNSUrls)
                                            await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                        byte[]? uriStore = null;

                                        if (SVOServerConfiguration.SVOHTTPSBypass)
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<SVML>\r\n" +
                                           $"    <BROWSER_INIT name=\"init\" />\r\n\t" +
                                            "    \r\n    <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                           $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsUpload\" value=\"http://{domain}:10060/HUBPS3_SVML/fileservices/UploadFileServlet\"/>\r\n" +
                                           $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsDownload\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Download.jsp\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"SvfsDeleteSubmit\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Delete.jsp\"/>\r\n    \r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpArcadeMachines\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=arcade\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpBowling\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=bowling\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpCharacterCreation\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=characterCustomisation\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpChess\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=chess\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpConversations\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=conversations\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpDoors\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=relocation\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpDraughts\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=draughts\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpEmotes\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=emotes\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpFirstTimeUser\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=firstTimeUsing\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpGameLaunchingCreate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamelaunchingCreate\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpGameLaunchingJoin\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamelaunchingJoin\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpGamesRoom\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamespace\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpHomeApartment\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=homespace\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpPool\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=pool\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpSafetyInHome\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=stayingsafe\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpSeats\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=seats\"/>\r\n\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityNews\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=news\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityLatestUpdate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=latestUpdate\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityHandyLinks\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=handyLinks\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityMotd\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=messageoftheday\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityUsagePolicy\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=eula\"/>\r\n\r\n    " +
                                           $"    <DATA dataType=\"URI\" name=\"GriefReportStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/griefreporting/GriefReportWelcome.jsp?region={regionNoptions}\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"ViralProvisioningStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/viralprovisioning/HomeInviteWelcome.jsp\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"UserActivityLogUploadServlet\" value=\"http://{domain}:10060/HUBPS3_SVML/tracking/StatTrackingServlet\"/>\r\n    \r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityNewsSummary\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=news\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityNewsDetailed\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=latestUpdate\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityBetaTrialRoadmap\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=handyLinks\"/>\r\n    \r\n" +
                                           $"    <DATA dataType=\"DATA\" name=\"gameFinishURL\" value=\"http://{domain}:10060/HUBPS3_SVML/game/Game_Finish_Submit.jsp\" />\r\n\t\r\n" +
                                           $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"http://{domain}:10060/HUBPS3_SVML/account/SP_Login_Submit.jsp\" />\r\n\t\r\n\t" +
                                           $"     \r\n    \r\n\t<REDIRECT href=\"eulaCheck.jsp?region={regionNoptions}\" name=\"redirect\"/>\r\n" +
                                            "      \r\n" +
                                            "</SVML>");
                                        else
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<SVML>\r\n" +
                                            $"   <BROWSER_INIT name=\"init\" />\r\n\t" +
                                            "    \r\n    <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                            $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsUpload\" value=\"http://{domain}:10060/HUBPS3_SVML/fileservices/UploadFileServlet\"/>\r\n" +
                                            $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsDownload\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Download.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"URI\" name=\"SvfsDeleteSubmit\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Delete.jsp\"/>\r\n    \r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpArcadeMachines\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=arcade\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpBowling\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=bowling\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpCharacterCreation\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=characterCustomisation\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpChess\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=chess\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpConversations\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=conversations\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpDoors\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=relocation\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpDraughts\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=draughts\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpEmotes\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=emotes\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpFirstTimeUser\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=firstTimeUsing\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpGameLaunchingCreate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamelaunchingCreate\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpGameLaunchingJoin\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamelaunchingJoin\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpGamesRoom\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamespace\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpHomeApartment\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=homespace\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpPool\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=pool\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpSafetyInHome\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=stayingsafe\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpSeats\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=seats\"/>\r\n\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityNews\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=news\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityLatestUpdate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=latestUpdate\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityHandyLinks\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=handyLinks\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityMotd\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=messageoftheday\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityUsagePolicy\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=eula\"/>\r\n\r\n    " +
                                            $"   <DATA dataType=\"URI\" name=\"GriefReportStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/griefreporting/GriefReportWelcome.jsp?region={regionNoptions}\"/>\r\n" +
                                            $"    <DATA dataType=\"URI\" name=\"ViralProvisioningStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/viralprovisioning/HomeInviteWelcome.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"URI\" name=\"UserActivityLogUploadServlet\" value=\"http://{domain}:10060/HUBPS3_SVML/tracking/StatTrackingServlet\"/>\r\n    \r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityNewsSummary\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=news\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityNewsDetailed\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=latestUpdate\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityBetaTrialRoadmap\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=handyLinks\"/>\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"gameFinishURL\" value=\"http://{domain}:10060/HUBPS3_SVML/game/Game_Finish_Submit.jsp\" />\r\n\t\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"https://{domain}:10061/HUBPS3_SVML/account/SP_Login_Submit.jsp\" />\r\n       " +
                                            $"     \r\n    \r\n\t<REDIRECT href=\"eulaCheck.jsp?region={regionNoptions}\" name=\"redirect\"/>\r\n" +
                                            "      \r\n" +
                                            "</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = uriStore.Length;
                                                response.OutputStream.Write(uriStore, 0, uriStore.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;

                        case "/HUBPS3_SVML/unity/beta_start.jsp":

                            switch (method)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string region = string.Empty;

                                        try
                                        {
                                            region = request.Url.Query.Substring(8);
                                        }
                                        catch (Exception)
                                        {
                                            region = "en-US";
                                        }

                                        string domain = "homeps3.svo.online.scee.com";

                                        if (!SVOServerConfiguration.PreferDNSUrls)
                                            await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                        byte[]? uriStore = null;

                                        if (SVOServerConfiguration.SVOHTTPSBypass)
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<SVML>\r\n" +
                                           $"    <BROWSER_INIT name=\"init\" />\r\n\t" +
                                            "    \r\n    <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                           $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsUpload\" value=\"http://{domain}:10060/HUBPS3_SVML/fileservices/UploadFileServlet\"/>\r\n" +
                                           $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsDownload\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Download.jsp\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"SvfsDeleteSubmit\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Delete.jsp\"/>\r\n    \r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpArcadeMachines\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=arcade\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpBowling\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=bowling\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpCharacterCreation\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=characterCustomisation\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpChess\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=chess\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpConversations\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=conversations\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpDoors\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=relocation\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpDraughts\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=draughts\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpEmotes\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=emotes\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpFirstTimeUser\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=firstTimeUsing\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpGameLaunchingCreate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=gamelaunchingCreate\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpGameLaunchingJoin\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=gamelaunchingJoin\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpGamesRoom\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=gamespace\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpHomeApartment\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=homespace\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpPool\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=pool\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpSafetyInHome\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=stayingsafe\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpSeats\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=seats\"/>\r\n\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityNews\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=news\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityLatestUpdate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=latestUpdate\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityHandyLinks\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=handyLinks\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityMotd\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=messageoftheday\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityUsagePolicy\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=eula\"/>\r\n\r\n    " +
                                           $"    <DATA dataType=\"URI\" name=\"GriefReportStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/griefreporting/GriefReportWelcome.jsp?region={region}\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"ViralProvisioningStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/viralprovisioning/HomeInviteWelcome.jsp\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"UserActivityLogUploadServlet\" value=\"http://{domain}:10060/HUBPS3_SVML/tracking/StatTrackingServlet\"/>\r\n    \r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityNewsSummary\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=news\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityNewsDetailed\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=latestUpdate\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityBetaTrialRoadmap\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=handyLinks\"/>\r\n    \r\n" +
                                           $"    <DATA dataType=\"DATA\" name=\"gameFinishURL\" value=\"http://{domain}:10060/HUBPS3_SVML/game/Game_Finish_Submit.jsp\" />\r\n\t\r\n" +
                                           $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"http://{domain}:10060/HUBPS3_SVML/account/SP_Login_Submit.jsp\" />\r\n\t\r\n\t" +
                                           $"     \r\n    \r\n\t<REDIRECT href=\"eulaCheck.jsp?region={region}&autologin=0\" name=\"redirect\"/>\r\n" +
                                            "      \r\n" +
                                            "</SVML>");
                                        else
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<SVML>\r\n" +
                                            $"   <BROWSER_INIT name=\"init\" />\r\n\t" +
                                            "    \r\n    <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                            $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsUpload\" value=\"http://{domain}:10060/HUBPS3_SVML/fileservices/UploadFileServlet\"/>\r\n" +
                                            $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsDownload\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Download.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"URI\" name=\"SvfsDeleteSubmit\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Delete.jsp\"/>\r\n    \r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpArcadeMachines\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=arcade\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpBowling\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=bowling\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpCharacterCreation\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=characterCustomisation\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpChess\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=chess\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpConversations\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=conversations\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpDoors\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=relocation\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpDraughts\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=draughts\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpEmotes\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=emotes\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpFirstTimeUser\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=firstTimeUsing\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpGameLaunchingCreate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=gamelaunchingCreate\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpGameLaunchingJoin\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=gamelaunchingJoin\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpGamesRoom\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=gamespace\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpHomeApartment\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=homespace\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpPool\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=pool\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpSafetyInHome\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=stayingsafe\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpSeats\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=seats\"/>\r\n\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityNews\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=news\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityLatestUpdate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=latestUpdate\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityHandyLinks\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=handyLinks\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityMotd\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=messageoftheday\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityUsagePolicy\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=eula\"/>\r\n\r\n    " +
                                            $"   <DATA dataType=\"URI\" name=\"GriefReportStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/griefreporting/GriefReportWelcome.jsp?region={region}\"/>\r\n" +
                                            $"    <DATA dataType=\"URI\" name=\"ViralProvisioningStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/viralprovisioning/HomeInviteWelcome.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"URI\" name=\"UserActivityLogUploadServlet\" value=\"http://{domain}:10060/HUBPS3_SVML/tracking/StatTrackingServlet\"/>\r\n    \r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityNewsSummary\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=news\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityNewsDetailed\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=latestUpdate\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityBetaTrialRoadmap\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={region}&amp;pageName=handyLinks\"/>\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"gameFinishURL\" value=\"http://{domain}:10060/HUBPS3_SVML/game/Game_Finish_Submit.jsp\" />\r\n\t\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"https://{domain}:10061/HUBPS3_SVML/account/SP_Login_Submit.jsp\" />\r\n       " +
                                            $"     \r\n    \r\n\t<REDIRECT href=\"eulaCheck.jsp?region={region}&autologin=0\" name=\"redirect\"/>\r\n" +
                                            "      \r\n" +
                                            "</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = uriStore.Length;
                                                response.OutputStream.Write(uriStore, 0, uriStore.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;

                        case "/HUBPS3_SVML/unity/start.jsp":

                            switch (method)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string regionNoptions = string.Empty;

                                        try
                                        {
                                            regionNoptions = request.Url.Query.Substring(8);
                                        }
                                        catch (Exception)
                                        {
                                            regionNoptions = "en-US";
                                        }

                                        regionNoptions = regionNoptions.Replace("&", "&amp;");

                                        string domain = "homeps3.svo.online.scee.com";

                                        if (!SVOServerConfiguration.PreferDNSUrls)
                                            await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                        byte[]? uriStore = null;

                                        if (SVOServerConfiguration.SVOHTTPSBypass)
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<SVML>\r\n" +
                                           $"    <BROWSER_INIT name=\"init\" />\r\n\t" +
                                            "    \r\n    <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                           $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsUpload\" value=\"http://{domain}:10060/HUBPS3_SVML/fileservices/UploadFileServlet\"/>\r\n" +
                                           $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsDownload\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Download.jsp\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"SvfsDeleteSubmit\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Delete.jsp\"/>\r\n    \r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpArcadeMachines\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=arcade\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpBowling\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=bowling\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpCharacterCreation\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=characterCustomisation\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpChess\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=chess\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpConversations\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=conversations\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpDoors\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=relocation\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpDraughts\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=draughts\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpEmotes\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=emotes\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpFirstTimeUser\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=firstTimeUsing\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpGameLaunchingCreate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamelaunchingCreate\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpGameLaunchingJoin\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamelaunchingJoin\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpGamesRoom\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamespace\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpHomeApartment\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=homespace\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpPool\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=pool\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpSafetyInHome\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=stayingsafe\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"HelpSeats\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=seats\"/>\r\n\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityNews\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=news\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityLatestUpdate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=latestUpdate\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityHandyLinks\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=handyLinks\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityMotd\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=messageoftheday\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityUsagePolicy\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=eula\"/>\r\n\r\n    " +
                                           $"    <DATA dataType=\"URI\" name=\"GriefReportStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/griefreporting/GriefReportWelcome.jsp?region={regionNoptions}\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"ViralProvisioningStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/viralprovisioning/HomeInviteWelcome.jsp\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"UserActivityLogUploadServlet\" value=\"http://{domain}:10060/HUBPS3_SVML/tracking/StatTrackingServlet\"/>\r\n    \r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityNewsSummary\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=news\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityNewsDetailed\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=latestUpdate\"/>\r\n" +
                                           $"    <DATA dataType=\"URI\" name=\"CommunityBetaTrialRoadmap\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=handyLinks\"/>\r\n    \r\n" +
                                           $"    <DATA dataType=\"DATA\" name=\"gameFinishURL\" value=\"http://{domain}:10060/HUBPS3_SVML/game/Game_Finish_Submit.jsp\" />\r\n\t\r\n" +
                                           $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"http://{domain}:10060/HUBPS3_SVML/account/SP_Login_Submit.jsp\" />\r\n\t\r\n\t" +
                                           $"     \r\n    \r\n\t<REDIRECT href=\"eulaCheck.jsp?region={regionNoptions}&autologin=1\" name=\"redirect\"/>\r\n" +
                                            "      \r\n" +
                                            "</SVML>");
                                        else
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<SVML>\r\n" +
                                            $"   <BROWSER_INIT name=\"init\" />\r\n\t" +
                                            "    \r\n    <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                            $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsUpload\" value=\"http://{domain}:10060/HUBPS3_SVML/fileservices/UploadFileServlet\"/>\r\n" +
                                            $"    \r\n    <DATA dataType=\"URI\" name=\"SvfsDownload\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Download.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"URI\" name=\"SvfsDeleteSubmit\" value=\"http://{domain}:10060/HUBPS3_SVML/home/fileservices/Delete.jsp\"/>\r\n    \r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpArcadeMachines\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=arcade\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpBowling\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=bowling\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpCharacterCreation\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=characterCustomisation\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpChess\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=chess\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpConversations\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=conversations\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpDoors\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=relocation\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpDraughts\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=draughts\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpEmotes\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=emotes\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpFirstTimeUser\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=firstTimeUsing\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpGameLaunchingCreate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamelaunchingCreate\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpGameLaunchingJoin\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamelaunchingJoin\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpGamesRoom\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=gamespace\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpHomeApartment\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=homespace\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpPool\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=pool\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpSafetyInHome\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=stayingsafe\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"HelpSeats\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=seats\"/>\r\n\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityNews\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=news\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityLatestUpdate\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=latestUpdate\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityHandyLinks\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=handyLinks\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityMotd\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=messageoftheday\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityUsagePolicy\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=eula\"/>\r\n\r\n    " +
                                            $"   <DATA dataType=\"URI\" name=\"GriefReportStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/griefreporting/GriefReportWelcome.jsp?region={regionNoptions}\"/>\r\n" +
                                            $"    <DATA dataType=\"URI\" name=\"ViralProvisioningStart\" value=\"http://{domain}:10060/HUBPS3_SVML/home/viralprovisioning/HomeInviteWelcome.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"URI\" name=\"UserActivityLogUploadServlet\" value=\"http://{domain}:10060/HUBPS3_SVML/tracking/StatTrackingServlet\"/>\r\n    \r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityNewsSummary\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=news\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityNewsDetailed\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=latestUpdate\"/>\r\n" +
                                            $"   <DATA dataType=\"URI\" name=\"CommunityBetaTrialRoadmap\" value=\"http://{domain}:10060/HUBPS3_SVML/home/help/HelpGeneric.jsp?region={regionNoptions}&amp;pageName=handyLinks\"/>\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"gameFinishURL\" value=\"http://{domain}:10060/HUBPS3_SVML/game/Game_Finish_Submit.jsp\" />\r\n\t\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"https://{domain}:10061/HUBPS3_SVML/account/SP_Login_Submit.jsp\" />\r\n       " +
                                            $"     \r\n    \r\n\t<REDIRECT href=\"eulaCheck.jsp?region={regionNoptions}&autologin=1\" name=\"redirect\"/>\r\n" +
                                            "      \r\n" +
                                            "</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = uriStore.Length;
                                                response.OutputStream.Write(uriStore, 0, uriStore.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;

                        case "/HUBPS3_SVML/unity/eulaCheck.jsp":
                            if (request.Url.PathAndQuery.Contains("autologin=0"))
                            {
                                switch (request.HttpMethod)
                                {
                                    case "GET":

                                        string? clientMac = request.Headers.Get("X-SVOMac");

                                        string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                        if (string.IsNullOrEmpty(serverMac))
                                        {
                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                        {
                                            response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                            response.Headers.Set("X-SVOMac", serverMac);

                                            string? region = string.Empty;

                                            try
                                            {
                                                region = HttpUtility.ParseQueryString(request.Url.Query).Get("region");
                                            }
                                            catch (Exception)
                                            {
                                                region = "en-US";
                                            }

                                            byte[] eulaCheck = Encoding.ASCII.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n" +
                                                "<SVML>\r\n  " +
                                                "<SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n  " +
                                                $"<EULA name=\"eula\" mode=\"check\" href=\"unityNpLogin.jsp?region={region}\" eulahref=\"eulaDisplay.jsp?region={region}\" linkOption=\"NORMAL\" />\r\n" +
                                                "</SVML>");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                            if (response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    response.ContentLength64 = eulaCheck.Length;
                                                    response.OutputStream.Write(eulaCheck, 0, eulaCheck.Length);
                                                }
                                                catch (Exception)
                                                {
                                                    // Not Important;
                                                }
                                            }
                                        }

                                        break;
                                }
                            }
                            else if (request.Url.PathAndQuery.Contains("autologin=1"))
                            {
                                switch (request.HttpMethod)
                                {
                                    case "GET":

                                        string? clientMac = request.Headers.Get("X-SVOMac");

                                        string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                        if (string.IsNullOrEmpty(serverMac))
                                        {
                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                        {
                                            response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                            response.Headers.Set("X-SVOMac", serverMac);

                                            string? region = string.Empty;

                                            try
                                            {
                                                region = HttpUtility.ParseQueryString(request.Url.Query).Get("region");
                                            }
                                            catch (Exception)
                                            {
                                                region = "en-US";
                                            }

                                            byte[] unityNpLogin = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n" +
                                                "<SVML>\r\n" +
                                                "        <EULA name=\"eula\" mode=\"save\" />\r\n" +
                                                $"        <UNITY name=\"login\" type=\"command\" success_href=\"../announcement/Medius_Announcement_Read.jsp?region={region}\" success_linkoption=\"NORMAL\"/>\r\n" +
                                                $" \r\n\t\t<HOMEACTION name=\"FrontEndAction\">" +
                                                $"\r\n\t\t\t<OnEnterPage event=\"FrontEndEvent\" param1=\"SigningIntoSvo\" param2=\"\" />" +
                                                $"\r\n\t\t</HOMEACTION>\r\n" +
                                                $"  \r\n" +
                                                $"        <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                                $"</SVML>");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                            if (response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    response.ContentLength64 = unityNpLogin.Length;
                                                    response.OutputStream.Write(unityNpLogin, 0, unityNpLogin.Length);
                                                }
                                                catch (Exception)
                                                {
                                                    // Not Important;
                                                }
                                            }
                                        }

                                        break;
                                }
                            }
                            else // Home 0.41 path, EULA check was not implemented yet.
                            {
                                switch (request.HttpMethod)
                                {
                                    case "GET":

                                        string? clientMac = request.Headers.Get("X-SVOMac");

                                        string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                        if (string.IsNullOrEmpty(serverMac))
                                        {
                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }
                                        else
                                        {
                                            response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                            response.Headers.Set("X-SVOMac", serverMac);

                                            string? region = string.Empty;

                                            try
                                            {
                                                region = HttpUtility.ParseQueryString(request.Url.Query).Get("region");
                                            }
                                            catch (Exception)
                                            {
                                                region = "en-US";
                                            }

                                            byte[] eulaMesg = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<SVML>\r\n" +
                                                "    <RECTANGLE class=\"CHIP_FACE\" name=\"backPanel\" x=\"292\" y=\"140\" width=\"708\" height=\"440\"/>\r\n" +
                                                "    <RECTANGLE class=\"CHIP_RECESS\" name=\"backPanel\" x=\"300\" y=\"148\" width=\"692\" height=\"384\" fillColor=\"#FFFFFFFF\"/>\r\n\r\n" +
                                                "    <TEXT name=\"text\" x=\"640\" y=\"171\" width=\"636\" height=\"26\" fontSize=\"26\" align=\"center\" textColor=\"#cc000000\">Usage Policy</TEXT>\r\n\r\n" +
                                                "    <TEXTAREA class=\"TEXTAREA1\" name=\"message\" x=\"308\" y=\"204\" width=\"664\" height=\"320\"\r\n\t\tfontSize=\"22\" lineSpacing=\"22\" linesVisible=\"14\"\r\n\t\t" +
                                                "readonly=\"true\" selectable=\"false\" blinkCursor=\"false\"\r\n\t\t" +
                                                "textColor=\"#CC000000\" highlightTextColor=\"#FF000000\"\r\n\t\t" +
                                                "leftPadValue=\"8\" topPadValue=\"8\" \r\n" +
                                                "        defaultTextEntry=\"1\" defaultTextScroll=\"1\">Participation in the Home Beta Trial is subject to the terms of the Home Beta Trial Agreement available at http://homebetatrial.com/eula.html and the PLAYSTATION®Network Terms of Service and User Agreement.</TEXTAREA>\r\n    \r\n" +
                                                "    <TEXT name=\"legend\" x=\"824\" y=\"548\" width=\"652\" height=\"18\" fontSize=\"18\" align=\"left\" textColor=\"#CCFFFFFF\">[CROSS] Accept | [CIRCLE] Decline</TEXT>\r\n" +
                                                "    <QUICKLINK name=\"accept\" button=\"SV_PAD_X\" linkOption=\"NORMAL\" href=\"../unity/unityNpLogin.jsp?beta=true\"/>\r\n" +
                                                "</SVML>");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                            if (response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    response.ContentLength64 = eulaMesg.Length;
                                                    response.OutputStream.Write(eulaMesg, 0, eulaMesg.Length);
                                                }
                                                catch (Exception)
                                                {
                                                    // Not Important;
                                                }
                                            }
                                        }

                                        break;
                                }
                            }

                            break;

                        case "/HUBPS3_SVML/unity/eulaDisplay.jsp":

                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? region = string.Empty;

                                        try
                                        {
                                            region = HttpUtility.ParseQueryString(request.Url.Query).Get("region");
                                        }
                                        catch (Exception)
                                        {
                                            region = "en-US";
                                        }

                                        byte[] eulaDisplay = Array.Empty<byte>();

                                        if (SVOServerConfiguration.PSHomeRPCS3Workaround)
                                            eulaDisplay = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n" +
                                            "<SVML>\r\n  <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n  " +
                                            $"<EULA name=\"eula\" mode=\"save\" href=\"unityNpLogin.jsp?region={region}\" eulahref=\"eulaDisplay.jsp?region={region}\" linkOption=\"NORMAL\" />\r\n" + // Gets around a RPCS3 issue by not calling web browser.
                                            "</SVML>");
                                        else
                                            eulaDisplay = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n" +
                                            "<SVML>\r\n  <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n  " +
                                            $"<EULA name=\"eula\" mode=\"browser\" href=\"unityNpLogin.jsp?region={region}\" eulahref=\"eulaDisplay.jsp?region={region}\" linkOption=\"NORMAL\" />\r\n" +
                                            "</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = eulaDisplay.Length;
                                                response.OutputStream.Write(eulaDisplay, 0, eulaDisplay.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }


                                    break;
                            }
                            break;

                        case "/HUBPS3_SVML/unity/unityNpLogin.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        bool isClosedBeta = request.Url.PathAndQuery.Contains("beta=true");

                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? region = string.Empty;

                                        try
                                        {
                                            region = HttpUtility.ParseQueryString(request.Url.Query).Get("region");
                                        }
                                        catch (Exception)
                                        {
                                            region = "en-US";
                                        }

                                        byte[] unityNpLogin = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n" +
                                            "<SVML>\r\n" +
                                            $"{(isClosedBeta ? string.Empty : "        <EULA name=\"eula\" mode=\"save\" />\r\n")}" +
                                            $"        <UNITY name=\"login\" type=\"command\" success_href=\"../announcement/Medius_Announcement_Read.jsp?region={region}{(isClosedBeta ? "&beta=true" : string.Empty)}\" success_linkoption=\"NORMAL\"/>\r\n" +
                                            $" \r\n\t\t<HOMEACTION name=\"FrontEndAction\">" +
                                            $"\r\n\t\t\t<OnEnterPage event=\"FrontEndEvent\" param1=\"SigningIntoSvo\" param2=\"\" />" +
                                            $"\r\n\t\t</HOMEACTION>\r\n" +
                                            $"  \r\n" +
                                            $"        <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                            $"</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = unityNpLogin.Length;
                                                response.OutputStream.Write(unityNpLogin, 0, unityNpLogin.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }

                                    break;
                            }

                            break;

                        case "/HUBPS3_SVML/account/SP_Login_Submit.jsp":
                            switch (request.HttpMethod)
                            {
                                case "POST":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        int appId = Convert.ToInt32(HttpUtility.ParseQueryString(request.Url.Query).Get("applicationID"));

                                        if (!request.HasEntityBody)
                                        {
                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }

                                        response.Headers.Set("Content-Type", "text/xml");

                                        string s = string.Empty;

                                        // Get the data from the HTTP stream
                                        using (StreamReader reader = new(request.InputStream, request.ContentEncoding))
                                        {
                                            // Convert the data to a string and display it on the console.
                                            s = reader.ReadToEnd();
                                            reader.Close();
                                        }

                                        byte[] bytes = Encoding.ASCII.GetBytes(s);
                                        int AcctNameLen = Convert.ToInt32(bytes.GetValue(81));

                                        string acctName = s.Substring(82, 32);

                                        string acctNameREX = Regex.Replace(acctName, @"[^a-zA-Z0-9]+", string.Empty);

                                        LoggerAccessor.LogInfo($"Logging user {acctNameREX} into SVO...\n");

                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? sig = HttpUtility.ParseQueryString(request.Url.Query).Get("sig");

                                        int accountId = -1;

                                        string langId = "0";

                                        try
                                        {
                                            await SVOServerConfiguration.Database.GetAccountByName(acctNameREX, appId).ContinueWith((r) =>
                                            {
                                                //Found in database so keep.
                                                langId = request.Url.Query.Substring(94, request.Url.Query.Length - 94);
                                                if (r.Result != null)
                                                    accountId = r.Result.AccountId;
                                            });
                                        }
                                        catch (Exception)
                                        {
                                            langId = request.Url.Query.Substring(94, request.Url.Query.Length - 94);
                                            accountId = 0;
                                        }

                                        response.AddHeader("Set-Cookie", $"LangID={langId}; Path=/");
                                        response.AppendHeader("Set-Cookie", $"AcctID={accountId}; Path=/");
                                        response.AppendHeader("Set-Cookie", $"NPCountry=us; Path=/");
                                        response.AppendHeader("Set-Cookie", $"ClanID=-1; Path=/");
                                        response.AppendHeader("Set-Cookie", $"AuthKeyTime=03-31-2023 16:03:41; Path=/");
                                        response.AppendHeader("Set-Cookie", $"NPLang=1; Path=/");
                                        response.AppendHeader("Set-Cookie", $"ModerateMode=false; Path=/");
                                        response.AppendHeader("Set-Cookie", $"TimeZone=PST; Path=/");
                                        response.AppendHeader("Set-Cookie", $"ClanID=-1; Path=/");
                                        response.AppendHeader("Set-Cookie", $"NPContentRating=201326592; Path=/");
                                        response.AppendHeader("Set-Cookie", $"AuthKey=nRqnf97f~UaSANLErurJIzq9GXGWqWCADdA3TfqUIVXXisJyMnHsQ34kA&C^0R#&~JULZ7xUOY*rXW85slhQF&P&Eq$7kSB&VBtf`V8rb^BC`53jGCgIT; Path=/");
                                        response.AppendHeader("Set-Cookie", $"AcctName={acctNameREX}; Path=/");
                                        response.AppendHeader("Set-Cookie", $"OwnerID=-255; Path=/");
                                        response.AppendHeader("Set-Cookie", $"Sig={sig}==; Path=/");

                                        byte[] sp_Login = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<XML action=\"http://scee-home.playstation.net/c.home/prod/live\">\r\n" +
                                            "    <SP_Login>\r\n" +
                                            "        <status>\r\n" +
                                            "            <id>20600</id>\r\n" +
                                            "            <message>ACCT_LOGIN_SUCCESS</message>\r\n" +
                                            "        </status>\r\n" +
                                            $"       <accountID>{accountId}</accountID>\r\n" +
                                            "        <userContext>0</userContext>\r\n" +
                                            "    </SP_Login>\r\n" +
                                            "</XML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = sp_Login.Length;
                                                response.OutputStream.Write(sp_Login, 0, sp_Login.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }

                                    break;
                            }

                            break;

                        case "/HUBPS3_SVML/announcement/Medius_Announcement_Read.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        byte[]? Medius_Announcement_Read = null;

                                        if (string.IsNullOrEmpty(SVOServerConfiguration.MOTD))
                                            Medius_Announcement_Read = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<SVML>\r\n" +
                                                    "    <RECTANGLE class=\"CHIP_FACE\" name=\"backPanel\" x=\"292\" y=\"140\" width=\"708\" height=\"440\"/>\r\n" +
                                                    "    <RECTANGLE class=\"CHIP_RECESS\" name=\"backPanel\" x=\"300\" y=\"148\" width=\"692\" height=\"384\" fillColor=\"#FFFFFFFF\"/>\r\n\r\n" +
                                                    "    <TEXT name=\"text\" x=\"640\" y=\"171\" width=\"636\" height=\"26\" fontSize=\"26\" align=\"center\" textColor=\"#cc000000\">Message Of the Day</TEXT>\r\n\r\n" +
                                                    "    <TEXTAREA class=\"TEXTAREA1\" name=\"message\" x=\"308\" y=\"204\" width=\"664\" height=\"320\"\r\n\t\tfontSize=\"22\" lineSpacing=\"22\" linesVisible=\"14\"\r\n\t\t" +
                                                    "readonly=\"true\" selectable=\"false\" blinkCursor=\"false\"\r\n\t\t" +
                                                    "textColor=\"#CC000000\" highlightTextColor=\"#FF000000\"\r\n\t\t" +
                                                    "leftPadValue=\"8\" topPadValue=\"8\" \r\n" +
                                                    "        defaultTextEntry=\"1\" defaultTextScroll=\"1\">Welcome to PlayStationÂ®Home Open Beta.\r\n\r\n" +
                                                    "Head over to the new Resident Evil 5 Studio Lot space, accessible via the Menu Pad by selecting Locations &gt; World Map and then clicking on the Capcom chip. Here you can enjoy an interactive behind-the-scenes look at the tools and devices used on location for the filming of a portion of Resident Evil 5.\r\n\r\n" +
                                                    "CydoniaX (PlayStationÂ®Home Community Manager) &amp; Locust_Star (PlayStationÂ®Home Community Specialist)</TEXTAREA>\r\n    \r\n    <TEXT name=\"legend\" x=\"984\" y=\"548\" width=\"652\" height=\"18\" fontSize=\"18\" align=\"right\" textColor=\"#CCFFFFFF\">[CROSS] Continue</TEXT>\r\n" +
                                                    "    <QUICKLINK name=\"refresh\" button=\"SV_PAD_X\" linkOption=\"NORMAL\" href=\"../home/homeEnterWorld.jsp\"/>\r\n" +
                                                    "</SVML>");
                                        else
                                            Medius_Announcement_Read = Encoding.UTF8.GetBytes(SVOServerConfiguration.MOTD);

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = Medius_Announcement_Read.Length;
                                                response.OutputStream.Write(Medius_Announcement_Read, 0, Medius_Announcement_Read.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }

                                    break;
                            }

                            break;

                        case "/HUBPS3_SVML/home/homeEnterWorld.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        byte[] homeEnterWorld = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n<SVML>\r\n\r\n" +
                                            "    <HUB type=\"AutoChangeMode\" textColor=\"#FF7381BA\" highlightTextColor=\"#FF7381BA\" x=\"20\" y=\"200\" width=\"200\" height=\"40\"\r\n" +
                                            "      align=\"center\" border=\"true\" href=\"EnterLobby\" extra=\"Central Lobby\" skipOn=\"6\"></HUB>\r\n\r\n\t" +
                                            "   <REDIRECT name=\"toBlankPage\" href=\"homeInWorld.jsp\" linkOption=\"NORMAL\"/>\r\n\r\n" +
                                            "</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = homeEnterWorld.Length;
                                                response.OutputStream.Write(homeEnterWorld, 0, homeEnterWorld.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }

                                    break;
                            }

                            break;

                        case "/HUBPS3_SVML/home/homeInWorld.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        byte[] homeEnterWorld = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n" +
                                            "<SVML>\r\n\r\n</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = homeEnterWorld.Length;
                                                response.OutputStream.Write(homeEnterWorld, 0, homeEnterWorld.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }

                                    break;
                            }

                            break;

                        case "/HUBPS3_SVML/home/fileservices/Download.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? fileNameBeginsWith = HttpUtility.ParseQueryString(request.Url.Query).Get("filename");

                                        bool fileExists = File.Exists($"{SVOServerConfiguration.SVOStaticFolder}/HUBPS3_SVML/fileservices/{fileNameBeginsWith}");

                                        byte[] xmlMessage;

                                        string? encodedFileName = SecurityElement.Escape(fileNameBeginsWith);

                                        if (fileExists)
                                        {
                                            string fileId = "1";
                                            string domain = "homeps3.svo.online.scee.com";

                                            if (!SVOServerConfiguration.PreferDNSUrls)
                                                await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                            xmlMessage = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                $"<XML>\r\n\r\n<XMLSVOFILETRANSFER direction=\"download\" filename=\"{encodedFileName}\" errorCode=\"None\" src=\"http://{domain}:10060/HUBPS3_SVML/fileservices/DownloadFileServlet?fileID={fileId}&amp;fileNameBeginsWith={encodedFileName}\"/>\r\n</XML>");
                                        }
                                        else
                                            xmlMessage = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                    $"<XML>\r\n\r\n<XMLSVOFILETRANSFER direction=\"download\" filename=\"{encodedFileName}\" errorCode=\"FileDoesNotExist\" src=\"\"/>\r\n</XML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = xmlMessage.Length;
                                                response.OutputStream.Write(xmlMessage, 0, xmlMessage.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }

                                    break;
                            }

                            break;


                        case "/HUBPS3_SVML/fileservices/DownloadFileServlet":

                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? fileNameBeginsWith = HttpUtility.ParseQueryString(request.Url.Query).Get("fileNameBeginsWith");

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        if (fileNameBeginsWith != null)
                                        {
                                            response.Headers.Set("X-SVOMac", serverMac);

                                            if (fileNameBeginsWith.Contains(".jpg"))
                                            {
                                                response.Headers.Set("Content-Type", "image/jpeg;charset=UTF-8");
                                                response.AppendHeader("Content-Disposition", $"attachment; filename={fileNameBeginsWith}");
                                                response.AppendHeader("Accept-Ranges", "bytes");
                                                response.AppendHeader("Cache-Control", "private");
                                                response.AppendHeader("ETag", $"{Guid.NewGuid().ToString().Substring(0, 4)}-{Guid.NewGuid().ToString().Substring(0, 12)}");
                                            }
                                            else if (fileNameBeginsWith.Contains(".xml") || fileNameBeginsWith.Contains("Profile"))
                                            {
                                                response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                                response.AppendHeader("Content-Disposition", $"attachment; filename={fileNameBeginsWith}");
                                                response.AppendHeader("Accept-Ranges", "bytes");
                                                response.AppendHeader("Cache-Control", "private");
                                                response.AppendHeader("ETag", $"{Guid.NewGuid().ToString().Substring(0, 4)}-{Guid.NewGuid().ToString().Substring(0, 12)}");
                                            }

                                            Directory.CreateDirectory($"{SVOServerConfiguration.SVOStaticFolder}/HUBPS3_SVML/fileservices");

                                            using FileStream fs = new($"{SVOServerConfiguration.SVOStaticFolder}/HUBPS3_SVML/fileservices/{fileNameBeginsWith}", FileMode.Open);

                                            int fileLen = Convert.ToInt32(fs.Length);

                                            response.AppendHeader("Content-Length", fileLen.ToString());

                                            // Create a byte array.
                                            byte[] strArr = new byte[fileLen];

                                            fs.Read(strArr, 0, fileLen);
                                            fs.Flush();

                                            response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                            if (response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    response.ContentLength64 = strArr.Length;
                                                    response.OutputStream.Write(strArr, 0, strArr.Length);
                                                }
                                                catch (Exception)
                                                {
                                                    // Not Important;
                                                }
                                            }

                                            fs.Flush();
                                        }
                                        else
                                        {
                                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                            return;
                                        }
                                    }

                                    break;
                            }

                            break;

                        //Uploads Player's UserTrackingLog, we can handle this in different way but we just upload it for now
                        case "/HUBPS3_SVML/tracking/StatTrackingServlet":
                            switch (request.HttpMethod)
                            {
                                case "POST":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        byte[] xmlMessage;
                                        string fileNameBeginsWith = "UserTrackingLog.xml";

                                        // Find number of bytes in stream.
                                        int strLen = Convert.ToInt32(request.ContentLength64);

                                        // Create a byte array.
                                        byte[] strArr = new byte[strLen];

                                        request.InputStream.Read(strArr, 0, strLen);

                                        //We can do whatever we want with the POST information for the UseTrackingLog from any player it seems? 
                                        //Lets just write to file!

                                        Directory.CreateDirectory($"{SVOServerConfiguration.SVOStaticFolder}/HUBPS3_SVML/tracking");

                                        using (FileStream fs = new($"{SVOServerConfiguration.SVOStaticFolder}/HUBPS3_SVML/tracking/{fileNameBeginsWith}", FileMode.OpenOrCreate))
                                        {
                                            fs.Write(strArr, 0, strLen);
                                            fs.Flush();
                                        }

                                        xmlMessage = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
                                        $"<XML>\r\n\r\n<XMLSVOFILETRANSFER direction=\"upload\" filename=\"{fileNameBeginsWith}\"/>\r\n</XML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = xmlMessage.Length;
                                                response.OutputStream.Write(xmlMessage, 0, xmlMessage.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;

                        case "/HUBPS3_SVML/fileservices/UploadFileServlet":
                            switch (request.HttpMethod)
                            {
                                case "POST":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? toUpload = HttpUtility.ParseQueryString(request.Url.Query).Get("fileNameBeginsWith");

                                        byte[]? xmlMessage = null;

                                        using (MemoryStream ms = new())
                                        {
                                            request.InputStream.CopyTo(ms);

                                            // Reset the memory stream position to the beginning
                                            ms.Position = 0;

                                            // Find the number of bytes in the stream
                                            int contentLength = (int)ms.Length;

                                            // Create a byte array
                                            byte[] buffer = new byte[contentLength];

                                            // Read the contents of the memory stream into the byte array
                                            ms.Read(buffer, 0, contentLength);

                                            Directory.CreateDirectory($"{SVOServerConfiguration.SVOStaticFolder}/HUBPS3_SVML/fileservices");

                                            using (FileStream fs = new($"{SVOServerConfiguration.SVOStaticFolder}/HUBPS3_SVML/fileservices/{toUpload}", FileMode.OpenOrCreate))
                                            {
                                                fs.Write(buffer, 0, contentLength);
                                                fs.Flush();
                                            }

                                            ms.Flush();
                                        }

                                        xmlMessage = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n" +
                                        $"<XML>\r\n\r\n" +
                                        $"<XMLSVOFILETRANSFER direction=\"upload\" filename=\"{toUpload}\"/>\r\n" +
                                        $"</XML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = xmlMessage.Length;
                                                response.OutputStream.Write(xmlMessage, 0, xmlMessage.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }
                                    break;
                            }
                            break;

                        case "/HUBPS3_SVML/home/help/HelpGeneric.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? region = HttpUtility.ParseQueryString(request.Url.Query).Get("region");

                                        string pageName = HttpUtility.ParseQueryString(request.Url.Query).Get("pageName") ?? string.Empty;

                                        byte[] xmlMessage;

                                        if (pageName == "news")
                                        {
                                            xmlMessage = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                                                "<SVML>\r\n  <RECTANGLE name=\"background\" x=\"0\" y=\"0\" width=\"1280\" height=\"720\" fillColor=\"#99000000\"/>\r\n" +
                                                "  <IMAGE name=\"icon\" x=\"32\" y=\"32\" width=\"96\" height=\"96\" src=\"file:///HOST_SVML/images/xmb_Firsttimeusinghome.dds\"/>\r\n" +
                                                "  <TEXT name=\"title\" x=\"160\" y=\"52\" width=\"1056\" height=\"56\" fontSize=\"56\" align=\"left\" textColor=\"#ffffffff\" selectable=\"false\">News</TEXT>\r\n" +
                                               $"  <TEXTAREA\r\n\t\tclass=\"BG_NONE,SCROLL_ARROWS\" name=\"message\"\r\n\t\tx=\"64\" y=\"160\" width=\"1152\" height=\"452\" scrollBarWidth=\"32\"\r\n\t\tfontSize=\"52\" lineSpacing=\"56\" linesVisible=\"8\"\r\n\t\treadonly=\"\" blinkCursor=\"false\"\r\n\t\ttextColor=\"#BBFFFFFF\" highlightTextColor=\"#FFFFFFFF\"\r\n\t\tleftPadValue=\"0\" topPadValue=\"0\"\r\n\t\tdefaultTextEntry=\"1\" defaultTextScroll=\"1\"\r\n\t\tselectable=\"false\" selected=\"false\">News!</TEXTAREA>\r\n\r\n" +
                                                "  <TEXT name=\"legend\" class=\"localise\" x=\"48\" y=\"640\" fontSize=\"48\" align=\"left\" textColor=\"#ffffffff\" selectable=\"false\">[CIRCLE]Back</TEXT>\r\n" +
                                                "  <TEXT name=\"legend\" class=\"localise\" x=\"1232\" y=\"640\" fontSize=\"48\" align=\"right\" textColor=\"#ffffffff\" selectable=\"false\">SCROLL_UP_DOWN</TEXT>\r\n" +
                                                "  <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                                "  <QUICKLINK name=\"returnToPSP\" button=\"SV_PAD_BACK\" linkOption=\"\" href=\"file:///HOST_SVML/ReturnToPSP.svml\"/>\r\n" +
                                                "</SVML>");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                            if (response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    response.ContentLength64 = xmlMessage.Length;
                                                    response.OutputStream.Write(xmlMessage, 0, xmlMessage.Length);
                                                }
                                                catch (Exception)
                                                {
                                                    // Not Important;
                                                }
                                            }
                                        }
                                        else if (pageName == "latestUpdate")
                                        {
                                            xmlMessage = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                                                "<SVML>\r\n  <RECTANGLE name=\"background\" x=\"0\" y=\"0\" width=\"1280\" height=\"720\" fillColor=\"#99000000\"/>\r\n" +
                                                "  <IMAGE name=\"icon\" x=\"32\" y=\"32\" width=\"96\" height=\"96\" src=\"file:///HOST_SVML/images/xmb_Firsttimeusinghome.dds\"/>\r\n" +
                                                "  <TEXT name=\"title\" x=\"160\" y=\"52\" width=\"1056\" height=\"56\" fontSize=\"56\" align=\"left\" textColor=\"#ffffffff\" selectable=\"false\">Latest Update</TEXT>\r\n" +
                                               $"  <TEXTAREA\r\n\t\tclass=\"BG_NONE,SCROLL_ARROWS\" name=\"message\"\r\n\t\tx=\"64\" y=\"160\" width=\"1152\" height=\"452\" scrollBarWidth=\"32\"\r\n\t\tfontSize=\"52\" lineSpacing=\"56\" linesVisible=\"8\"\r\n\t\treadonly=\"\" blinkCursor=\"false\"\r\n\t\ttextColor=\"#BBFFFFFF\" highlightTextColor=\"#FFFFFFFF\"\r\n\t\tleftPadValue=\"0\" topPadValue=\"0\"\r\n\t\tdefaultTextEntry=\"1\" defaultTextScroll=\"1\"\r\n\t\tselectable=\"false\" selected=\"false\">Last Update!</TEXTAREA>\r\n\r\n" +
                                                "  <TEXT name=\"legend\" class=\"localise\" x=\"48\" y=\"640\" fontSize=\"48\" align=\"left\" textColor=\"#ffffffff\" selectable=\"false\">[CIRCLE]Back</TEXT>\r\n" +
                                                "  <TEXT name=\"legend\" class=\"localise\" x=\"1232\" y=\"640\" fontSize=\"48\" align=\"right\" textColor=\"#ffffffff\" selectable=\"false\">SCROLL_UP_DOWN</TEXT>\r\n" +
                                                "  <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                                "  <QUICKLINK name=\"returnToPSP\" button=\"SV_PAD_BACK\" linkOption=\"\" href=\"file:///HOST_SVML/ReturnToPSP.svml\"/>\r\n" +
                                                "</SVML>");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                            if (response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    response.ContentLength64 = xmlMessage.Length;
                                                    response.OutputStream.Write(xmlMessage, 0, xmlMessage.Length);
                                                }
                                                catch (Exception)
                                                {
                                                    // Not Important;
                                                }
                                            }
                                        }
                                        else if (pageName == "messageoftheday")
                                        {
                                            xmlMessage = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n" +
                                                "<SVML>\r\n  <RECTANGLE name=\"background\" x=\"0\" y=\"0\" width=\"1280\" height=\"720\" fillColor=\"#99000000\"/>\r\n" +
                                                "  <IMAGE name=\"icon\" x=\"32\" y=\"32\" width=\"96\" height=\"96\" src=\"file:///HOST_SVML/images/xmb_Firsttimeusinghome.dds\"/>\r\n" +
                                                "  <TEXT name=\"title\" x=\"160\" y=\"52\" width=\"1056\" height=\"56\" fontSize=\"56\" align=\"left\" textColor=\"#ffffffff\" selectable=\"false\">Message of the Day</TEXT>\r\n" +
                                               $"  <TEXTAREA\r\n\t\tclass=\"BG_NONE,SCROLL_ARROWS\" name=\"message\"\r\n\t\tx=\"64\" y=\"160\" width=\"1152\" height=\"452\" scrollBarWidth=\"32\"\r\n\t\tfontSize=\"52\" lineSpacing=\"56\" linesVisible=\"8\"\r\n\t\treadonly=\"\" blinkCursor=\"false\"\r\n\t\ttextColor=\"#BBFFFFFF\" highlightTextColor=\"#FFFFFFFF\"\r\n\t\tleftPadValue=\"0\" topPadValue=\"0\"\r\n\t\tdefaultTextEntry=\"1\" defaultTextScroll=\"1\"\r\n\t\tselectable=\"false\" selected=\"false\">Message of the Day!</TEXTAREA>\r\n\r\n" +
                                                "  <TEXT name=\"legend\" class=\"localise\" x=\"48\" y=\"640\" fontSize=\"48\" align=\"left\" textColor=\"#ffffffff\" selectable=\"false\">[CIRCLE]Back</TEXT>\r\n" +
                                                "  <TEXT name=\"legend\" class=\"localise\" x=\"1232\" y=\"640\" fontSize=\"48\" align=\"right\" textColor=\"#ffffffff\" selectable=\"false\">SCROLL_UP_DOWN</TEXT>\r\n" +
                                                "  <SET name=\"nohistory\" neverBackOnto=\"true\"/>\r\n" +
                                                "  <QUICKLINK name=\"returnToPSP\" button=\"SV_PAD_BACK\" linkOption=\"\" href=\"file:///HOST_SVML/ReturnToPSP.svml\"/>\r\n" +
                                                "</SVML>");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                            if (response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    response.ContentLength64 = xmlMessage.Length;
                                                    response.OutputStream.Write(xmlMessage, 0, xmlMessage.Length);
                                                }
                                                catch (Exception)
                                                {
                                                    // Not Important;
                                                }
                                            }
                                        }
                                        else
                                            response.StatusCode = (int)System.Net.HttpStatusCode.NotImplemented;
                                    }

                                    break;
                            }

                            break;

                        case "/HUBPS3_SVML/home/griefreporting/GriefReportWelcome.jsp":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = CastleLibrary.Sony.SVO.WebSecurityUtils.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/svml");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        byte[] xmlMessage = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                            "<SVML>\r\n" +
                                            "    <RECTANGLE class=\"CHIP_FACE\" name=\"backPanel\" x=\"292\" y=\"140\" width=\"708\" height=\"440\"/>\r\n" +
                                            "    <RECTANGLE class=\"CHIP_RECESS\" name=\"backPanel\" x=\"300\" y=\"148\" width=\"692\" height=\"384\" fillColor=\"#FFFFFFFF\"/>\r\n\r\n" +
                                            "    <TEXT name=\"text\" x=\"640\" y=\"171\" width=\"636\" height=\"26\" fontSize=\"26\" align=\"center\" textColor=\"#cc000000\">Grief Report</TEXT>\r\n\r\n" +
                                            "    <TEXTAREA class=\"TEXTAREA1\" name=\"message\" x=\"308\" y=\"204\" width=\"664\" height=\"320\"\r\n\t\tfontSize=\"22\" lineSpacing=\"22\" linesVisible=\"14\"\r\n\t\t readonly=\"true\"selectable=\"false\" blinkCursor=\"false\"\r\n\t\ttextColor=\"#CC000000\" highlightTextColor=\"#FF000000\"\r\n\t\tleftPadValue=\"8\" topPadValue=\"8\" \r\n defaultTextEntry=\"1\" defaultTextScroll=\"1\">" +
                                            "    Please provide evidence of the person you would like to submit for the grief report!\r\n\r\n" +
                                            "    <TEXT name=\"legend\" x=\"984\" y=\"548\" width=\"652\" height=\"18\" fontSize=\"18\" align=\"right\" textColor=\"#CCFFFFFF\">[CROSS] Continue</TEXT>\r\n" +
                                            "    <QUICKLINK name=\"refresh\" button=\"SV_PAD_X\" linkOption=\"NORMAL\" href=\"../home/homeInWorld.jsp\"/>\r\n" +
                                            "</SVML>\r\n" +
                                            "<SVML>\r\n" +
                                            "</SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = xmlMessage.Length;
                                                response.OutputStream.Write(xmlMessage, 0, xmlMessage.Length);
                                            }
                                            catch (Exception)
                                            {
                                                // Not Important;
                                            }
                                        }
                                    }

                                    break;
                            }

                            break;

                        default:
                            response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                            break;

                            #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerAccessor.LogError($"[SVO] - Home_SVO thrown an assertion - {ex}");
                response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }
    }
}
