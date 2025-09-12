using CustomLogger;
using MultiServerLibrary.Extension;
using SpaceWizards.HttpListener;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SVO.Games.PS3
{
    public class SingStar
    {
        public static async Task Singstar_SVO(HttpListenerRequest request, HttpListenerResponse response)
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
                        #region SINGSTAR
                        case "/SINGSTARPS3_SVML/start.jsp":

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

                                        string domain = "singstar.svo.online.com";

                                        if (!SVOServerConfiguration.PreferDNSUrls)
                                            await InternetProtocolUtils.TryGetServerIP(out domain).ConfigureAwait(false);

                                        byte[]? uriStore = null;

                                        if (SVOServerConfiguration.SVOHTTPSBypass)
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                            "<SVML>\n" +
                                            $"    <SET name=\"IP\" IPAddress=\"{request.RemoteEndPoint.Address}\" />    \r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"createGameURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_Create.jsp?gameMode=%d\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"gamePostBinaryStatsURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_PostBinaryStats_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"finishGameURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_Finish_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"SetUniversePasswordURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/account/SP_SetPassword_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"loginEncryptedURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/account/Account_Encrypted_Login_Submit.jsp\" />    \r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/account/SP_Login_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"SetBuddyListURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/buddy/Buddy_SetList_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"SetIgnoreListURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/account/SP_UpdateIgnoreList_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"drmSignatureURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/commerce/Commerce_BufferedSignature.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"spUpdateTicketURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/account/SP_UpdateTicket.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"createGamePlayerURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_Create_Player_Submit.jsp?SVOGameID=%d&amp;playerSide=%d\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"createGameSubmitURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_Create_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"gameBinaryStatsPostURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_BinaryStatsPost_Submit.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"gameFinishURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_Finish_Submit.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"finishGameURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Finish_Game_Submit.jsp\"/>\r\n" +
                                            $"    <BROWSER_INIT name=\"init\" />\r\n" +
                                            $"     \r\n    \r\n\t<REDIRECT href=\"unityNpLogin.jsp\" name=\"redirect\"/>\r\n" +
                                            "</SVML>");
                                        else
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                                            "<SVML>\n" +
                                            $"    <SET name=\"IP\" IPAddress=\"{request.RemoteEndPoint.Address}\" />    \r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"createGameURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_Create.jsp?gameMode=%d\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"gamePostBinaryStatsURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_PostBinaryStats_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"finishGameURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_Finish_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"SetUniversePasswordURI\" value=\"https://{domain}:10061/SINGSTARPS3_SVML/account/SP_SetPassword_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"loginEncryptedURI\" value=\"https://{domain}:10061/SINGSTARPS3_SVML/account/Account_Encrypted_Login_Submit.jsp\" />    \r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"TicketLoginURI\" value=\"https://{domain}:10061/SINGSTARPS3_SVML/account/SP_Login_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"SetBuddyListURI\" value=\"https://{domain}:10061/SINGSTARPS3_SVML/buddy/Buddy_SetList_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"SetIgnoreListURI\" value=\"https://{domain}:10061/SINGSTARPS3_SVML/account/SP_UpdateIgnoreList_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"drmSignatureURI\" value=\"https://{domain}:10061/SINGSTARPS3_SVML/commerce/Commerce_BufferedSignature.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"spUpdateTicketURI\" value=\"https://{domain}:10061/SINGSTARPS3_SVML/account/SP_UpdateTicket.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"createGamePlayerURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_Create_Player_Submit.jsp?SVOGameID=%d&amp;playerSide=%d\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"createGameSubmitURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_Create_Submit.jsp\" />\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"gameBinaryStatsPostURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_BinaryStatsPost_Submit.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"gameFinishURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Game_Finish_Submit.jsp\"/>\r\n" +
                                            $"    <DATA dataType=\"DATA\" name=\"finishGameURI\" value=\"http://{domain}:10060/SINGSTARPS3_SVML/game/Finish_Game_Submit.jsp\"/>\r\n" +
                                            $"    <BROWSER_INIT name=\"init\" />\r\n" +
                                            $"     \r\n    \r\n\t<REDIRECT href=\"unityNpLogin.jsp\" name=\"redirect\"/>\r\n" +
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

                        case "/SINGSTARPS3_SVML/unityNpLogin.jsp":
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

                                        byte[] unityNpLogin = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?> \r\n" +
                                            "<SVML>\r\n" +
                                            $"        <UNITY name=\"login\" type=\"command\" success_href=\"/SINGSTARPS3_SVML/singstar/init_online.jsp\" success_linkoption=\"NORMAL\"/>\r\n" +
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

                        case "/SINGSTARPS3_SVML/account/SP_Login_Submit.jsp":
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
                                            "<XML>\r\n" +
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

                        case "/SINGSTARPS3_SVML/singstar/init_online.jsp":
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

                                        byte[] singOnlnInit = Encoding.UTF8.GetBytes(@"<?xml version=""1.0"" encoding=""UTF-8""?> 
                                        <SVML>
	                                        <UNITY name=""unityinit"" type=""CHANGEMODE"" forwardmode=""online"" backmode=""offline"" href=""started.jsp"" linkOption=""CLIENT_REDIRECT"" />

	                                        <UNITY name=""safepoint"" type=""SAFEPOINT"" variety=""game"" href=""init_online.jsp"" />
	                                        <UNITY name=""safepoint"" type=""SAFEPOINT"" variety=""session"" href=""init_online.jsp"" />
                                        </SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = singOnlnInit.Length;
                                                response.OutputStream.Write(singOnlnInit, 0, singOnlnInit.Length);
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

                        case "/SINGSTARPS3_SVML/singstar/started.jsp":
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

                                        const bool enableNpCommerce = false; // RPCS3 will soft-freeze on that check.

                                        string npRetreiveCommand = $@"<UNITY 
		                                        name=""retrieveRootCategory"" 
		                                        type=""PRODUCTCATEGORY_COMMAND"" 
		                                        categoryId=""UX0002-SSTT00001_01""
		                                        bcategoryId=""UX0002-NPXX00102_00""
		                                        xcategoryId=""SCERT-SCEA00003_01"" 
		                                        ycategoryId=""IV0002-NPXS00004_00"" 
		                                        languageId=""{request.Cookies["LangID"]?.Value ?? "0"}"" 
	                                        />";

                                        byte[] startedInit = Encoding.UTF8.GetBytes($@"<?xml version=""1.0"" encoding=""UTF-8""?>
                                        <SVML>
	                                        {(enableNpCommerce ? npRetreiveCommand : string.Empty)}

	                                        <SS:SCREEN name=""screen"" x=""0"" y=""0"" transition=""invisiblePage"">
                                            </SS:SCREEN>

                                            <REDIRECT name=""OnlineComplete"" href=""static://LoginComplete"" linkOption=""NORMAL"" />

	                                        <SET name=""notinhist"" neverBackOnto=""true"" />
                                        </SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = startedInit.Length;
                                                response.OutputStream.Write(startedInit, 0, startedInit.Length);
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

                        case "/SINGSTARPS3_SVML/singstar/OnlineRoot":
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

                                        byte[] onlnRoot = Encoding.UTF8.GetBytes(@"<?xml version=""1.0"" encoding=""UTF-8""?>

                                        <SVML xmlns:SS=""www.singstar.com/svoTag"">

                                        <!--<CREATEPROFILE name=""1002_PROFILE_METADATA.txt""
	                                        uploadUrl=""http://sing2.online.scee.com:10060/SINGSTARPS3_SVML/fileservices/UploadFileServlet?fileNameBeginsWith=1002_PROFILE_METADATA.txt&filePermission=2&fileTypeID=5""
	                                        successHref=""profile_create_success.jsp""
	                                        failHref=""profile_create_fail.jsp""
	                                        />-->

                                          <SET name=""nohistory"" neverBackOnto=""true"" />

                                          <ACTIONLINK name=""link"" button=""SV_ACTION_BACK"" href=""hide.jsp""/>

                                          <ACTIONLINK name=""linkSquare"" linkOption=""NORMAL"" button=""SV_PAD_SQUARE"" href=""/SingStore/categories.jsp"" />

                                          <SS:SCREEN name=""screen"" transition=""default"" mirrorPos=""570.5"" mirrorHeight=""57.5"">

                                            <SS:DIV name=""titleDiv"" x=""188"" y=""141"">

                                              <SS:RICHTEXT name=""title"" y=""-33"" alignY=""baseline"" font=""ID_FONT_TITLE"" colour=""TitleText"" shadow=""false"">Singstar Online</SS:RICHTEXT>

                                              <SS:RICHTEXT name=""subtitle"" y=""1"" width=""500"" alignY=""baseline"" font=""ID_FONT_SUBTITLE"" colour=""HighlightText"">Powered by MultiServer 4</SS:RICHTEXT>

                                            </SS:DIV>

                                            <SS:PANEL name=""MyPannel"" x=""188"" y=""155"" width=""907"" height=""396"" visual=""SingStore-Panel"">

                                              <SS:DIV name=""MyVideo"" x=""0"" y=""0"" width=""907"" height=""396"">

                                                <SS:MOVIES name=""preview"" width=""907"" height=""396"" loop=""true"" spectrumEnabled=""false"" src=""http://sing2.online.scee.com:10010/singstarps3/prevclips/singstore.mp4"" />

                                              </SS:DIV>

                                            </SS:PANEL>

                                            <SS:LEGEND name=""legendCancel"" type=""legendcancel"" text=""BACK"" />

                                            <SS:LEGEND name=""legendSquare"" type=""legendsquare"" text=""Enter the SingStore"" />
                                          </SS:SCREEN>

                                        </SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = onlnRoot.Length;
                                                response.OutputStream.Write(onlnRoot, 0, onlnRoot.Length);
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

                        case "/SINGSTARPS3_SVML/singstar/Video":
                        case "/SINGSTARPS3_SVML/singstar/SnapShots":
                        case "/SINGSTARPS3_SVML/singstar/Audio":
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
                                        // The game wants to check again for online connectivity, it is designed to use internal SVML files for these services.

                                        response.Headers.Set("Content-Type", "text/svml; charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        byte[] VideoInit = Encoding.UTF8.GetBytes($@"<?xml version=""1.0"" encoding=""UTF-8""?>
                                        <SVML>
	                                        <REDIRECT href=""init_online.jsp"" name=""redirect""/>
                                        </SVML>");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = VideoInit.Length;
                                                response.OutputStream.Write(VideoInit, 0, VideoInit.Length);
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

                        case "/SINGSTARPS3_SVML/singstar/hide.jsp":
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

                                        byte[] onlnRoot = Encoding.UTF8.GetBytes(@"

                                        <?xml version=""1.0"" encoding=""UTF-8""?>

                                        <SVML>
                                          <SS:SCREEN name=""screen"" x=""0"" y=""0"" transition=""invisiblePage"">
                                          </SS:SCREEN>

                                         <HUB type=""HideSvo"" name=""hide""/>
                                         <SET name=""nohistory"" neverBackOnto=""true""/>
                                        </SVML>
                                        ");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = onlnRoot.Length;
                                                response.OutputStream.Write(onlnRoot, 0, onlnRoot.Length);
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
                        
                        case "/SINGSTARPS3_SVML/singstar/profile_create_success.jsp":
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

                                        byte[] successRoot = Encoding.UTF8.GetBytes(@"

                                        <?xml version=""1.0"" encoding=""UTF-8""?>

                                        <SVML>
                                        </SVML>
                                        ");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = successRoot.Length;
                                                response.OutputStream.Write(successRoot, 0, successRoot.Length);
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

                        case "/SINGSTARPS3_SVML/singstar/profile_create_fail.jsp":
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

                                        byte[] failRoot = Encoding.UTF8.GetBytes(@"

                                        <?xml version=""1.0"" encoding=""UTF-8""?>

                                        <SVML>
                                            <SS:SCREEN name=""screen"" x=""0"" y=""0"" transition=""invisiblePage"">
                                            </SS:SCREEN>

                                            <REDIRECT name=""OnlineFail"" href=""static://LoginFailed"" linkOption=""NORMAL"" />

	                                        <SET name=""notinhist"" neverBackOnto=""true"" />
                                        </SVML>
                                        ");

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = failRoot.Length;
                                                response.OutputStream.Write(failRoot, 0, failRoot.Length);
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
                LoggerAccessor.LogError($"[SVO] - Singstar_SVO thrown an assertion - {ex}");
                response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }
    }
}
