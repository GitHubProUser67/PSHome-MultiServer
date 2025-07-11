using CustomLogger;
using MultiServerLibrary.Extension;
using SpaceWizards.HttpListener;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace SVO.Games.PS3
{
    public class Starhawk
    {
        public static async Task Starhawk_SVO(HttpListenerRequest request, HttpListenerResponse response)
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
                        #region STARHAWK
                        case "/BOURBON_XML/":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Set-Cookie", "JSESSIONID=65EE71A76AFEB1D83E32208CC0163539; Path=/BOURBON_XML");
                                        response.Headers.Set("Set-Cookie", "LangID=1; Path=/");
                                        response.Headers.Set("Location", "http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/uri/URIStore.do");
                                        response.Headers.Set("Date", DateTime.Now.ToString("r"));
                                        response.ContentType = "text/html;charset=UTF-8";
                                        response.ContentLength64 = 0;
                                        response.StatusCode = 302;
                                        response.StatusDescription = "Moved Temporarily";
                                    }

                                    break;
                            }

                            break;

                        case "/BOURBON_XML/uri/URIStore.do":

                            switch (method)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        //65EE71A76AFEB1D83E32208CC0163539
                                        response.AddHeader("Set-Cookie", $"JSESSIONID={Guid.NewGuid().ToString()}; Path=/BOURBON_XML");
                                        response.AppendHeader("Set-Cookie", $"LangID=1; Path=/");
                                        // MOVED LOCATION 302 RESPONSE response.AddHeader("Location", $"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/uri/URIStore.do");

                                        byte[]? uriStore = null;

                                        if (SVOServerConfiguration.SVOHTTPSBypass)
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<XML>\r\n\r\n" +
                                                "    <URL_List>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"loginEncryptedURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/Account_Encrypted_Login_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Account_GetUserID\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/Account_GetUserID.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"LoginURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/Account_Login.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Account_Login_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/Account_Login_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"LogoutURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/Account_Logout.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Account_Logout_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/Account_Logout_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"TicketLoginURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/SP_Login_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_CreateAdminEvent\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_CreateAdminEvent.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_CreateClanEvent\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_CreateClanEvent.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_DeleteEventByAnnouncementId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_DeleteEventByAnnouncementId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_DeleteEventByClanId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_DeleteEventByClanId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_DeleteEventBySeriesId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_DeleteEventBySeriesId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_DeleteEventByTournamentId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_DeleteEventByTournamentId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEventInfo\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEventInfo.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEvents\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEvents.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEventsByAnnouncementId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEventsByAnnouncementId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEventsByClanId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEventsByClanId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEventsBySeriesId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEventsBySeriesId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEventsByTournamentId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEventsByTournamentId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetUpcomingBillboardEvents\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetUpcomingBillboardEvents.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_SetEventIsActive\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_SetEventIsActive.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_TouchTournamentEvent\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_TouchTournamentEvent.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_CreateNews_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_CreateNews_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_CreateWithTag_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_CreateWithTag_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_Disband_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_Disband_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanInfo\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanInfo.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanInfoByAccountID\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanInfoByAccountID.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanInfoByID\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanInfoByID.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanInfoByName\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanInfoByName.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanInfosByTag\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanInfosByTag.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanPlayers\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanPlayers.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClansByBuddies\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClansByBuddies.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_Home\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_Home.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_Leave_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_Leave_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_ModifyNews_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ModifyNews_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_ReadNews\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ReadNews.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_RemoveAllMetaData\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RemoveAllMetaData.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_RemoveMember_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RemoveMember_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_RemoveMetaData\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RemoveMetaData.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_RespondInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RespondInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_RevokeInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RevokeInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_SearchClansByName\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_SearchClansByName.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_SendInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_SendInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_TransferLeadership_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_TransferLeadership_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_UpdateClanTag_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_UpdateClanTag_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_UpdateMetaData\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_UpdateMetaData.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_UpdatePlayerLevel_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_UpdatePlayerLevel_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_ViewInvites\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ViewInvites.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_ViewSentInvites\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ViewSentInvites.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesDownloadServletURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/DownloadFileServlet\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"filePermission\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesAddToFavoritesSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_AddToFavoritesSubmit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesAssignClanURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_AssignClan.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesAttributesByIDURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_AttributesByID.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesDeleteURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Delete.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesDeleteFromFavoritesSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_DeleteFromFavoritesSubmit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesDeleteSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_DeleteSubmit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesGriefReportURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_GriefReport.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesGriefReportSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_GriefReportSubmit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesHomeURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Home.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesListURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_List.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileSizeGreaterThan\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSizeLessThan\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileOwnerID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNewerThanTimeStamp\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"filterPermissionFilter\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileTypeID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileModerationStatusID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSystemOwnedFile\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesListExtURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_ListExt.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileSizeGreaterThan\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSizeLessThan\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileOwnerID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNewerThanTimeStamp\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileMetaDataKey\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileMetaDataValue\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileOperator\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSort\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSortOrder\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"filterPermissionFilter\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileTypeID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileModerationStatusID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"languageID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSystemOwnedFile\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesListFavoritesURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_ListFavorites.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesGetMetaDataURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_MetaData.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileMetaDataKey\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesMetaDataBulkURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_MetaDataBulk.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesStatusURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Status.do\">\r\n        <ServerParams>\r\n            <Param name=\"statusCode\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesUpdateMetaDataURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_UpdateMetaData.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileMetaDataKey\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileMetaDataValue\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesUpdateMetaDataBulkURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_UpdateMetaDataBulk.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesUpdateMetaDataBulkSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_UpdateMetaDataBulkSubmit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesUploadURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Upload.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesViewQuotaURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_ViewQuota.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileTypeID\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesUploadServletURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/UploadFileServlet\">\r\n        <ServerParams>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileDescription\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"filePermission\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"systemOwnedProgramId\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"systemOwnedExpirationDate\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"systemOwnedReferenceText\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"vulgarityFilterURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/filter/VulgarityFilter.do\">\r\n        <ServerParams>\r\n            <Param name=\"text\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"gameCreateURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/game/Game_Create.do\">\r\n        <ServerParams>\r\n            <Param name=\"gameMode\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"createGamePlayerURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/game/Game_Create_Player_Submit.do\">\r\n        <ServerParams>\r\n            <Param name=\"SVOGameID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"playerSide\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"createGameSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/game/Game_Create_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"gameFinishURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/game/Game_Finish_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"gamePostBinaryStatsURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/game/Game_PostBinaryStats_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"homeURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/home.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_AddSubTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_AddSubTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_AddSubTopic_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_AddSubTopic_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_AddTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_AddTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_AddTopic_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_AddTopic_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_BanPlayer\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_BanPlayer.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_BanPlayer_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_BanPlayer_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_ClanTopics\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_ClanTopics.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditSubTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditSubTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditSubTopic_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditSubTopic_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditThread\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditThread.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditThread_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditThread_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditTopic_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditTopic_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanAddSubTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanAddSubTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanAddTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanAddTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanBanPlayer\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanBanPlayer.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"MsgBoard_Moderate_ClanBannedPlayerHistory\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanBannedPlayerHistory.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanBannedPlayers\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanBannedPlayers.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanEditSubTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanEditSubTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanEditThread\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanEditThread.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanEditTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanEditTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanHome\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanHome.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanTopics\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanTopics.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanUnBanPlayer\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanUnBanPlayer.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Post\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Post.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Post_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Post_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_PostsList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_PostsList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Reply\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Reply.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Reply_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Reply_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_SubTopics\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_SubTopics.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_SubTopicsByName\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_SubTopicsByName.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Topics\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Topics.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_ViewThread\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_ViewThread.do\"/>\r\n    <DATA dataType=\"URI\" name=\"statsBlobDownloadURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_BinaryStatsDownload_Submit.do\">\r\n        <ServerParams>\r\n            <Param name=\"gameMode\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"AccountID\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"blobType\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"start\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"length\" optional=\"\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_BuddyLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_BuddyLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_BuddyLeaderboardHistory\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_BuddyLeaderboardHistory.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_CareerLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_CareerLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"rankInfoURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_CareerRankInfo.do\">\r\n        <ServerParams>\r\n            <Param name=\"playerList\" optional=\"\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_CareerSearch\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_CareerSearch.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_CareerSearch_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_CareerSearch_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"playerStatsURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_CareerStatsByID.do\">\r\n        <ServerParams>\r\n            <Param name=\"PlayerID\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"gameMode\" optional=\"\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_ClanLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_ClanLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_ClanSearch\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_ClanSearch.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_ClanSearch_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_ClanSearch_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_ClanStatsByID\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_ClanStatsByID.do\"/>\r\n    <DATA dataType=\"URI\" name=\"statsServiceURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_Leaderboard.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"Stats_MyArchivedTeamTourneySocialLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyArchivedTeamTourneySocialLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MyCareerStats\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyCareerStats.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MyClanLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyClanLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MyClanLeaderboardHistory\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyClanLeaderboardHistory.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MySocialLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MySocialLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MyTourneyStats\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyTourneyStats.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MyTourneyStatsHistory\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyTourneyStatsHistory.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_PaddedCareerLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_PaddedCareerLeaderboard.do\">\r\n        <ServerParams>\r\n            <Param name=\"gameMode\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"accountID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"numAfter\" optional=\"true\" type=\"integer\"/>\r\n            <Param name=\"numBefore\" optional=\"true\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_PaddedClanLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_PaddedClanLeaderboard.do\">\r\n        <ServerParams>\r\n            <Param name=\"gameMode\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"clanID\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"numAfter\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"numBefore\" optional=\"\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_PaddedTourneyClanLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_PaddedTourneyClanLeaderboard.do\">\r\n        <ServerParams>\r\n            <Param name=\"teamTourID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"clanID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"numAfter\" optional=\"true\" type=\"integer\"/>\r\n            <Param name=\"numBefore\" optional=\"true\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_PaddedTourneyLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_PaddedTourneyLeaderboard.do\">\r\n        <ServerParams>\r\n            <Param name=\"teamTourID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"AccountID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"numAfter\" optional=\"true\" type=\"integer\"/>\r\n            <Param name=\"numBefore\" optional=\"true\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_TeamTourneyLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_TeamTourneyLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_TeamTourneyLeaderboardHistory\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_TeamTourneyLeaderboardHistory.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_AdvanceRound\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_AdvanceRound.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ArchiveList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ArchiveList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ArchivedSignedUpList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ArchivedSignedUpList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_AutoLaunch\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_AutoLaunch.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_AutoSignup\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_AutoSignup.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_AutoSignup_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_AutoSignup_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_BanPlayerList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_BanPlayerList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_BanPlyr_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_BanPlyr_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Bracket\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Bracket.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Bracket_NoAuth\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Bracket_NoAuth.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptUpdateInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptUpdateInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptainInviteList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptainInviteList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptainOfList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptainOfList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptainRemoveTeam_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptainRemoveTeam_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptainRequestList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptainRequestList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ChangeTeamCaptain_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ChangeTeamCaptain_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CheckIn\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CheckIn.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CombinedArchiveSignedUpList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CombinedArchiveSignedUpList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CombinedSignedUpList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CombinedSignedUpList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Create\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Create.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CreateTeam\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CreateTeam.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CreateTeamForAccount_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CreateTeamForAccount_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CreateTeam_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CreateTeam_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CreateTeam_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CreateTeam_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Create_SetOptions\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Create_SetOptions.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Create_SetOptions_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Create_SetOptions_Submit.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"TeamTourney_Create_SetOptions_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Create_SetOptions_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Create_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Create_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_DeleteUnbracketedTourneyTeam\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_DeleteUnbracketedTourneyTeam.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Delete_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Delete_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Delete_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Delete_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Edit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Edit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Edit_Options\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Edit_Options.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Edit_Options_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Edit_Options_Submit.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"TeamTourney_Edit_Options_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Edit_Options_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Edit_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Edit_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ForfeitConfirm\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ForfeitConfirm.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ForfeitTeam_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ForfeitTeam_Submit.do\">\r\n        <ServerParams>\r\n            <Param name=\"teamTourTeamID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"teamTourBracketID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"teamTourID\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Forfeit_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Forfeit_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_GetTeamTourneyForEdit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_GetTeamTourneyForEdit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_GetTeamTourneysForAccount\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_GetTeamTourneysForAccount.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_GetTeamTourneysInvitedTo\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_GetTeamTourneysInvitedTo.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_GetTeamTourneysSignedUp\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_GetTeamTourneysSignedUp.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_GetTeamsByTeamTourneyID\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_GetTeamsByTeamTourneyID.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Home\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Home.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_HostRemoveTeam_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_HostRemoveTeam_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Info\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Info.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Invite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Invite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_InvitedToList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_InvitedToList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_List\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_List.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Main\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Main.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_MatchData\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_MatchData.do\">\r\n        <ServerParams>\r\n            <Param name=\"teamTourID\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Menu\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Menu.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_MyTeamTourneysMenu\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_MyTeamTourneysMenu.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_OrganizerOfList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_OrganizerOfList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_PlyrInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_PlyrInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_PlyrUpdateInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_PlyrUpdateInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_RegPlayerList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_RegPlayerList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_RemovePlyr_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_RemovePlyr_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_RemovePlyr_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_RemovePlyr_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_RequestedToJoinList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_RequestedToJoinList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ResetRound\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ResetRound.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ResetTeamTourney\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ResetTeamTourney.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Rules\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Rules.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Search\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Search_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search_Submit.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"TeamTourney_Search_Submit_NoAuth_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search_Submit_NoAuth_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"TeamTourney_Search_WithGenericFields_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search_WithGenericFields_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Search_WithOptions\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search_WithOptions.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Search_WithOptions_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search_WithOptions_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_SignedUpList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_SignedUpList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Signup\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Signup.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Signup_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Signup_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_StartTeamTourney\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_StartTeamTourney.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_TeamList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_TeamList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_UnBanPlyr_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_UnBanPlyr_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_UpdateGenericField\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_UpdateGenericField.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_UpdateInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_UpdateInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"TeamTourney_UpdateInvite_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_UpdateInvite_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_WonList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_WonList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Test1_XMLURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/test/Test1_XML.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Test2_XMLURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/test/Test2_XML.do\"/>\r\n    <DATA dataType=\"URI\" name=\"uriStoreURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/uri/URIStore.do\"/>\r\n</URL_List>\r\n</XML>");
                                        else
                                            uriStore = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<XML>\r\n\r\n" +
                                                "    <URL_List>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"loginEncryptedURL\" value=\"https://starhawk-prod2.svo.online.scea.com:10061/BOURBON_XML/account/Account_Encrypted_Login_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Account_GetUserID\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/Account_GetUserID.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"LoginURL\" value=\"https://starhawk-prod2.svo.online.scea.com:10061/BOURBON_XML/account/Account_Login.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Account_Login_Submit\" value=\"https://starhawk-prod2.svo.online.scea.com:10061/BOURBON_XML/account/Account_Login_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"LogoutURL\" value=\"https://starhawk-prod2.svo.online.scea.com:10061/BOURBON_XML/account/Account_Logout.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Account_Logout_Submit\" value=\"https://starhawk-prod2.svo.online.scea.com:10061/BOURBON_XML/account/Account_Logout_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"TicketLoginURI\" value=\"https://starhawk-prod2.svo.online.scea.com:10061/BOURBON_XML/account/SP_Login_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_CreateAdminEvent\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_CreateAdminEvent.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_CreateClanEvent\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_CreateClanEvent.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_DeleteEventByAnnouncementId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_DeleteEventByAnnouncementId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_DeleteEventByClanId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_DeleteEventByClanId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_DeleteEventBySeriesId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_DeleteEventBySeriesId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_DeleteEventByTournamentId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_DeleteEventByTournamentId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEventInfo\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEventInfo.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEvents\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEvents.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEventsByAnnouncementId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEventsByAnnouncementId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEventsByClanId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEventsByClanId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEventsBySeriesId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEventsBySeriesId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetEventsByTournamentId\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetEventsByTournamentId.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_GetUpcomingBillboardEvents\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_GetUpcomingBillboardEvents.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_SetEventIsActive\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_SetEventIsActive.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Calendar_TouchTournamentEvent\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/calendar/Calendar_TouchTournamentEvent.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_CreateNews_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_CreateNews_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_CreateWithTag_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_CreateWithTag_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_Disband_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_Disband_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanInfo\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanInfo.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanInfoByAccountID\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanInfoByAccountID.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanInfoByID\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanInfoByID.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanInfoByName\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanInfoByName.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanInfosByTag\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanInfosByTag.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClanPlayers\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClanPlayers.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_GetClansByBuddies\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_GetClansByBuddies.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_Home\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_Home.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_Leave_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_Leave_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_ModifyNews_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ModifyNews_Submit.do\"/>\r\n" +
                                                "    <DATA dataType=\"URI\" name=\"Clan_ReadNews\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ReadNews.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_RemoveAllMetaData\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RemoveAllMetaData.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_RemoveMember_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RemoveMember_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_RemoveMetaData\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RemoveMetaData.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_RespondInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RespondInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_RevokeInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RevokeInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_SearchClansByName\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_SearchClansByName.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_SendInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_SendInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_TransferLeadership_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_TransferLeadership_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_UpdateClanTag_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_UpdateClanTag_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_UpdateMetaData\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_UpdateMetaData.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_UpdatePlayerLevel_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_UpdatePlayerLevel_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_ViewInvites\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ViewInvites.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Clan_ViewSentInvites\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ViewSentInvites.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesDownloadServletURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/DownloadFileServlet\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"filePermission\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesAddToFavoritesSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_AddToFavoritesSubmit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesAssignClanURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_AssignClan.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesAttributesByIDURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_AttributesByID.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesDeleteURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Delete.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesDeleteFromFavoritesSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_DeleteFromFavoritesSubmit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesDeleteSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_DeleteSubmit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesGriefReportURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_GriefReport.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesGriefReportSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_GriefReportSubmit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesHomeURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Home.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesListURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_List.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileSizeGreaterThan\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSizeLessThan\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileOwnerID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNewerThanTimeStamp\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"filterPermissionFilter\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileTypeID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileModerationStatusID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSystemOwnedFile\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesListExtURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_ListExt.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileSizeGreaterThan\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSizeLessThan\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileOwnerID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNewerThanTimeStamp\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileMetaDataKey\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileMetaDataValue\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileOperator\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSort\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSortOrder\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"filterPermissionFilter\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileTypeID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileModerationStatusID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"languageID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileSystemOwnedFile\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesListFavoritesURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_ListFavorites.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesGetMetaDataURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_MetaData.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileMetaDataKey\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesMetaDataBulkURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_MetaDataBulk.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesStatusURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Status.do\">\r\n        <ServerParams>\r\n            <Param name=\"statusCode\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesUpdateMetaDataURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_UpdateMetaData.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileMetaDataKey\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileMetaDataValue\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesUpdateMetaDataBulkURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_UpdateMetaDataBulk.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesUpdateMetaDataBulkSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_UpdateMetaDataBulkSubmit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesUploadURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Upload.do\"/>\r\n    <DATA dataType=\"URI\" name=\"fileServicesViewQuotaURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_ViewQuota.do\">\r\n        <ServerParams>\r\n            <Param name=\"fileTypeID\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"fileServicesUploadServletURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/UploadFileServlet\">\r\n        <ServerParams>\r\n            <Param name=\"fileNameBeginsWith\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"fileDescription\" optional=\"false\" type=\"string\"/>\r\n            <Param name=\"filePermission\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"systemOwnedProgramId\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"systemOwnedExpirationDate\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"systemOwnedReferenceText\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"vulgarityFilterURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/filter/VulgarityFilter.do\">\r\n        <ServerParams>\r\n            <Param name=\"text\" optional=\"false\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"gameCreateURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/game/Game_Create.do\">\r\n        <ServerParams>\r\n            <Param name=\"gameMode\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"createGamePlayerURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/game/Game_Create_Player_Submit.do\">\r\n        <ServerParams>\r\n            <Param name=\"SVOGameID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"playerSide\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"createGameSubmitURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/game/Game_Create_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"gameFinishURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/game/Game_Finish_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"gamePostBinaryStatsURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/game/Game_PostBinaryStats_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"homeURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/home.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_AddSubTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_AddSubTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_AddSubTopic_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_AddSubTopic_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_AddTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_AddTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_AddTopic_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_AddTopic_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_BanPlayer\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_BanPlayer.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_BanPlayer_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_BanPlayer_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_ClanTopics\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_ClanTopics.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditSubTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditSubTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditSubTopic_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditSubTopic_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditThread\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditThread.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditThread_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditThread_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_EditTopic_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_EditTopic_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanAddSubTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanAddSubTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanAddTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanAddTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanBanPlayer\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanBanPlayer.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"MsgBoard_Moderate_ClanBannedPlayerHistory\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanBannedPlayerHistory.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanBannedPlayers\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanBannedPlayers.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanEditSubTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanEditSubTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanEditThread\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanEditThread.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanEditTopic\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanEditTopic.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanHome\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanHome.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanTopics\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanTopics.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Moderate_ClanUnBanPlayer\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanUnBanPlayer.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Post\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Post.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Post_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Post_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_PostsList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_PostsList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Reply\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Reply.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Reply_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Reply_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_SubTopics\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_SubTopics.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_SubTopicsByName\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_SubTopicsByName.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_Topics\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Topics.do\"/>\r\n    <DATA dataType=\"URI\" name=\"MsgBoard_ViewThread\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_ViewThread.do\"/>\r\n    <DATA dataType=\"URI\" name=\"statsBlobDownloadURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_BinaryStatsDownload_Submit.do\">\r\n        <ServerParams>\r\n            <Param name=\"gameMode\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"AccountID\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"blobType\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"start\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"length\" optional=\"\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_BuddyLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_BuddyLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_BuddyLeaderboardHistory\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_BuddyLeaderboardHistory.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_CareerLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_CareerLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"rankInfoURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_CareerRankInfo.do\">\r\n        <ServerParams>\r\n            <Param name=\"playerList\" optional=\"\" type=\"string\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_CareerSearch\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_CareerSearch.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_CareerSearch_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_CareerSearch_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"playerStatsURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_CareerStatsByID.do\">\r\n        <ServerParams>\r\n            <Param name=\"PlayerID\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"gameMode\" optional=\"\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_ClanLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_ClanLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_ClanSearch\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_ClanSearch.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_ClanSearch_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_ClanSearch_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_ClanStatsByID\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_ClanStatsByID.do\"/>\r\n    <DATA dataType=\"URI\" name=\"statsServiceURL\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_Leaderboard.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"Stats_MyArchivedTeamTourneySocialLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyArchivedTeamTourneySocialLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MyCareerStats\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyCareerStats.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MyClanLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyClanLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MyClanLeaderboardHistory\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyClanLeaderboardHistory.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MySocialLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MySocialLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MyTourneyStats\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyTourneyStats.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_MyTourneyStatsHistory\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_MyTourneyStatsHistory.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_PaddedCareerLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_PaddedCareerLeaderboard.do\">\r\n        <ServerParams>\r\n            <Param name=\"gameMode\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"accountID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"numAfter\" optional=\"true\" type=\"integer\"/>\r\n            <Param name=\"numBefore\" optional=\"true\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_PaddedClanLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_PaddedClanLeaderboard.do\">\r\n        <ServerParams>\r\n            <Param name=\"gameMode\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"clanID\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"numAfter\" optional=\"\" type=\"integer\"/>\r\n            <Param name=\"numBefore\" optional=\"\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_PaddedTourneyClanLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_PaddedTourneyClanLeaderboard.do\">\r\n        <ServerParams>\r\n            <Param name=\"teamTourID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"clanID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"numAfter\" optional=\"true\" type=\"integer\"/>\r\n            <Param name=\"numBefore\" optional=\"true\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_PaddedTourneyLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_PaddedTourneyLeaderboard.do\">\r\n        <ServerParams>\r\n            <Param name=\"teamTourID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"AccountID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"numAfter\" optional=\"true\" type=\"integer\"/>\r\n            <Param name=\"numBefore\" optional=\"true\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"Stats_TeamTourneyLeaderboard\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_TeamTourneyLeaderboard.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Stats_TeamTourneyLeaderboardHistory\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/stats/Stats_TeamTourneyLeaderboardHistory.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_AdvanceRound\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_AdvanceRound.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ArchiveList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ArchiveList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ArchivedSignedUpList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ArchivedSignedUpList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_AutoLaunch\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_AutoLaunch.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_AutoSignup\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_AutoSignup.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_AutoSignup_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_AutoSignup_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_BanPlayerList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_BanPlayerList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_BanPlyr_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_BanPlyr_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Bracket\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Bracket.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Bracket_NoAuth\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Bracket_NoAuth.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptUpdateInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptUpdateInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptainInviteList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptainInviteList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptainOfList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptainOfList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptainRemoveTeam_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptainRemoveTeam_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CaptainRequestList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CaptainRequestList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ChangeTeamCaptain_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ChangeTeamCaptain_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CheckIn\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CheckIn.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CombinedArchiveSignedUpList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CombinedArchiveSignedUpList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CombinedSignedUpList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CombinedSignedUpList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Create\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Create.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CreateTeam\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CreateTeam.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CreateTeamForAccount_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CreateTeamForAccount_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CreateTeam_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CreateTeam_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_CreateTeam_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_CreateTeam_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Create_SetOptions\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Create_SetOptions.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Create_SetOptions_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Create_SetOptions_Submit.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"TeamTourney_Create_SetOptions_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Create_SetOptions_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Create_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Create_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_DeleteUnbracketedTourneyTeam\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_DeleteUnbracketedTourneyTeam.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Delete_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Delete_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Delete_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Delete_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Edit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Edit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Edit_Options\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Edit_Options.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Edit_Options_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Edit_Options_Submit.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"TeamTourney_Edit_Options_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Edit_Options_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Edit_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Edit_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ForfeitConfirm\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ForfeitConfirm.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ForfeitTeam_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ForfeitTeam_Submit.do\">\r\n        <ServerParams>\r\n            <Param name=\"teamTourTeamID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"teamTourBracketID\" optional=\"false\" type=\"integer\"/>\r\n            <Param name=\"teamTourID\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Forfeit_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Forfeit_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_GetTeamTourneyForEdit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_GetTeamTourneyForEdit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_GetTeamTourneysForAccount\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_GetTeamTourneysForAccount.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_GetTeamTourneysInvitedTo\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_GetTeamTourneysInvitedTo.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_GetTeamTourneysSignedUp\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_GetTeamTourneysSignedUp.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_GetTeamsByTeamTourneyID\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_GetTeamsByTeamTourneyID.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Home\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Home.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_HostRemoveTeam_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_HostRemoveTeam_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Info\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Info.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Invite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Invite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_InvitedToList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_InvitedToList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_List\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_List.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Main\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Main.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_MatchData\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_MatchData.do\">\r\n        <ServerParams>\r\n            <Param name=\"teamTourID\" optional=\"false\" type=\"integer\"/>\r\n        </ServerParams>\r\n    </DATA>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Menu\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Menu.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_MyTeamTourneysMenu\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_MyTeamTourneysMenu.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_OrganizerOfList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_OrganizerOfList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_PlyrInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_PlyrInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_PlyrUpdateInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_PlyrUpdateInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_RegPlayerList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_RegPlayerList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_RemovePlyr_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_RemovePlyr_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_RemovePlyr_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_RemovePlyr_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_RequestedToJoinList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_RequestedToJoinList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ResetRound\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ResetRound.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_ResetTeamTourney\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_ResetTeamTourney.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Rules\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Rules.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Search\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Search_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search_Submit.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"TeamTourney_Search_Submit_NoAuth_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search_Submit_NoAuth_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"TeamTourney_Search_WithGenericFields_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search_WithGenericFields_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Search_WithOptions\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search_WithOptions.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Search_WithOptions_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Search_WithOptions_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_SignedUpList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_SignedUpList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Signup\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Signup.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_Signup_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_Signup_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_StartTeamTourney\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_StartTeamTourney.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_TeamList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_TeamList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_UnBanPlyr_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_UnBanPlyr_Submit.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_UpdateGenericField\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_UpdateGenericField.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_UpdateInvite_Submit\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_UpdateInvite_Submit.do\"/>\r\n    <DATA dataType=\"URI\"\r\n        name=\"TeamTourney_UpdateInvite_Submit_NoRedirect\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_UpdateInvite_Submit_NoRedirect.do\"/>\r\n    <DATA dataType=\"URI\" name=\"TeamTourney_WonList\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/teamtourney/TeamTourney_WonList.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Test1_XMLURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/test/Test1_XML.do\"/>\r\n    <DATA dataType=\"URI\" name=\"Test2_XMLURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/test/Test2_XML.do\"/>\r\n    <DATA dataType=\"URI\" name=\"uriStoreURI\" value=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/uri/URIStore.do\"/>\r\n</URL_List>\r\n</XML>");

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

                        case "/BOURBON_XML/account/SP_Login_Submit.do":
                            switch (request.HttpMethod)
                            {
                                case "POST":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

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

                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");

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
                                                "<SVML>\r\n" +
                                                "    <SP_Login>\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>20600</id>\r\n" +
                                                "            <message>ACCT_LOGIN_SUCCESS</message>\r\n" +
                                                "        </status>\r\n" +
                                               $"        <accountID>{accountId}</accountID>\r\n" +
                                                "        <userContext>0</userContext>\r\n" +
                                                "    </SP_Login>\r\n\t\r\n\t" +
                                                "</SVML>");

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

                        case "/BOURBON_XML/fileservices/FileServices_List.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? fileNameBeginsWith = HttpUtility.ParseQueryString(request.Url.Query).Get("fileNameBeginsWith");

                                        if (File.Exists($"{SVOServerConfiguration.SVOStaticFolder}/fileservices/{fileNameBeginsWith}"))
                                        {

                                            string? fileName = fileNameBeginsWith;
                                            string fileId = "1";


                                            // MOVED LOCATION 302 RESPONSE response.AddHeader("Location", $"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/uri/URIStore.do");
                                            /*
                                            string fileServices_List = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<XML>\r\n" +
                                                "    <files end=\"0\" start=\"0\" total=\"1\">\r\n" +
                                                "        <file\r\n" +
                                                "            AddToFavorites=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_AddToFavorites.do?fileID=5025&amp;fileNameBeginsWith=sh104.xml\"\r\n" +
                                                "            AttributesByID=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_AttributesByID.do?fileID=5025&amp;fileNameBeginsWith=sh104.xml\"\r\n" +
                                                "            Download=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Download.do?fileID=5025&amp;fileNameBeginsWith=sh104.xml\"\r\n            DownloadServlet=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/DownloadFileServlet?fileID=5025&amp;fileNameBeginsWith=sh104.xml\"\r\n" +
                                                "            GriefReport=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_GriefReport.do?fileID=5025&amp;fileNameBeginsWith=sh104.xml\"\r\n            UserRating=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_UserRating.do?fileID=5025&amp;fileNameBeginsWith=sh104.xml\"\r\n" +
                                                "            fileAccountName=\"starhawkAdmin\"\r\n" +
                                                "            fileCreateDate=\"2012-24-09 13:24:45 +0000\"\r\n" +
                                                "            fileDescription=\"\" fileDownloadCount=\"3709595\" fileID=\"5025\"\r\n" +
                                                "            fileLastModified=\"2018-04-06 05:33:08 +0000\"\r\n" +
                                                "            fileModerationStatus=\"1\" fileName=\"sh104.xml\"\r\n" +
                                                "            fileOwnerID=\"-1\" filePermission=\"2\" fileSize=\"5988\"\r\n" +
                                                "            fileSystemName=\"/u01/data/svfs/starhawk-prod2/0004/0003/000000000020_001073048502\" fileTypeID=\"1\"/>\r\n" +
                                                "        <prevURL>http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_List.do?fileNameBeginsWith=sh104.xml&amp;fileSizeGreaterThan=0&amp;fileSizeLessThan=999999999&amp;fileOwnerID=-1&amp;fileNewerThanTimeStamp=0&amp;filterPermissionFilter=2&amp;fileTypeID=1&amp;fileModerationStatusID=0&amp;fileSystemOwnedFile=N&amp;start=0&amp;end=0</prevURL>\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>Success</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </files>\r\n" +
                                                "</XML>\r\n ";
                                            */

                                            string fileTotal = "0";



                                            /*
                                            string fileServices_List = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<XML>\r\n" +
                                                $"    <files end=\"0\" start=\"0\" total=\"{fileTotal}\">\r\n" +
                                                "        <file\r\n" +
                                                $"            AddToFavorites=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_AddToFavorites.do?fileID={fileId}&=fileNameBeginsWith={fileNameBeginsWith}\"\r\n" +
                                                $"            Download=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Download.do?fileID={fileId}&=fileNameBeginsWith={fileName}\"\r\n" +
                                                //$"            DownloadServlet=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/DownloadFileServlet?fileID={fileId}&=fileNameBeginsWith={fileName}\"\r\n" +
                                                "            fileAccountName=\"starhawkAdmin\"\r\n" +
                                                "            fileCreateDate=\"2012-24-09 13:24:45 +0000\"\r\n" +
                                                $"            fileDescription=\"\" fileDownloadCount=\"0\"  filePopularity=\"0\" fileID=\"{fileId}\"\r\n" +
                                                "            fileLastModified=\"2018-04-06 05:33:08 +0000\"\r\n" +
                                                $"            fileRatingNumberVotes=\"0\" fileName=\"{fileName}\"\r\n" +
                                                "            fileOwnerID=\"-1\" filePermission=\"2\" fileSize=\"0\"\r\n" +
                                                "            fileSystemName=\"/u01/data/svfs/starhawk-prod2/0004/0003/000000000020_001073048502\" fileTypeID=\"1\"/>\r\n" +
                                               //"        <prevURL>http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_List.do?fileNameBeginsWith=sh104.xml&=fileSizeGreaterThan=0&=fileSizeLessThan=999999999&=fileOwnerID=-1&amp;fileNewerThanTimeStamp=0&amp;filterPermissionFilter=2&amp;fileTypeID=1&amp;fileModerationStatusID=0&amp;fileSystemOwnedFile=N&amp;start=0&amp;end=0</prevURL>\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>Success</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </files>\r\n" +
                                                "</XML>\r\n ";

                                            */

                                            byte[] File_Service = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<XML>\r\n" +
                                                $"    <files end=\"0\" start=\"0\" total=\"{fileTotal}\">\r\n" +
                                                "        <file\r\n" +
                                                $"            AddToFavorites=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_AddToFavorites.do?fileID={fileId}&=fileNameBeginsWith={fileNameBeginsWith}\"\r\n" +
                                                $"            Download=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_Download.do?fileID={fileId}&=fileNameBeginsWith={fileName}\"\r\n" +
                                                //$"            DownloadServlet=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/DownloadFileServlet?fileID={fileId}&=fileNameBeginsWith={fileName}\"\r\n" +
                                                "            fileAccountName=\"starhawkAdmin\"\r\n" +
                                                "            fileCreateDate=\"2023-04-03 13:24:45 +0000\"\r\n" +
                                                $"            fileDescription=\"\" fileDownloadCount=\"0\"  filePopularity=\"0\" fileID=\"{fileId}\"\r\n" +
                                                "            fileLastModified=\"2023-04-03 05:33:08 +0000\"\r\n" +
                                                $"            fileRatingNumberVotes=\"0\" fileName=\"{fileName}\"\r\n" +
                                                "            fileOwnerID=\"-1\" filePermission=\"2\" fileSize=\"0\"\r\n" +
                                                "            fileSystemName=\"/u01/data/svfs/starhawk-prod2/0004/0003/000000000020_001073048502\" fileTypeID=\"1\"/>\r\n" +
                                                //"        <prevURL>http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/fileservices/FileServices_List.do?fileNameBeginsWith=sh104.xml&=fileSizeGreaterThan=0&=fileSizeLessThan=999999999&=fileOwnerID=-1&amp;fileNewerThanTimeStamp=0&amp;filterPermissionFilter=2&amp;fileTypeID=1&amp;fileModerationStatusID=0&amp;fileSystemOwnedFile=N&amp;start=0&amp;end=0</prevURL>\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>Success</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </files>\r\n" +
                                                "</XML>\r\n ");

                                            response.StatusCode = (int)System.Net.HttpStatusCode.OK;

                                            if (response.OutputStream.CanWrite)
                                            {
                                                try
                                                {
                                                    response.ContentLength64 = File_Service.Length;
                                                    response.OutputStream.Write(File_Service, 0, File_Service.Length);
                                                }
                                                catch (Exception)
                                                {
                                                    // Not Important;
                                                }
                                            }
                                        }
                                        else
                                            response.StatusCode = 404;
                                    }

                                    break;
                            }

                            break;

                        case "/BOURBON_XML/fileservices/DownloadFileServlet":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "application/octet-stream");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? toDownload = HttpUtility.ParseQueryString(request.Url.Query).Get("fileNameBeginsWith");

                                        Directory.CreateDirectory($"{SVOServerConfiguration.SVOStaticFolder}/fileservices");

                                        using (FileStream fs = new($"{SVOServerConfiguration.SVOStaticFolder}/fileservices/{toDownload}", FileMode.Open))
                                        {
                                            int fileLen = Convert.ToInt32(fs.Length);

                                            // Create a byte array.
                                            byte[] strArr = new byte[fileLen];
                                            fs.Read(strArr, 0, fileLen);

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
                                    }

                                    break;
                            }

                            break;

                        case "/BOURBON_XML/fileservices/UploadFileServlet":
                            switch (request.HttpMethod)
                            {
                                case "POST":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

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

                                        // Find number of bytes in stream.
                                        int strLen = Convert.ToInt32(request.ContentLength64);

                                        // Create a byte array.
                                        byte[] strArr = new byte[strLen];

                                        request.InputStream.Read(strArr, 0, strLen);

                                        Directory.CreateDirectory($"{SVOServerConfiguration.SVOStaticFolder}/fileservices");

                                        using (FileStream fs = new($"{SVOServerConfiguration.SVOStaticFolder}/fileservices/{toUpload}", FileMode.OpenOrCreate))
                                        {
                                            fs.Write(strArr, 0, strLen);
                                            fs.Flush();
                                        }

                                        response.StatusCode = (int)System.Net.HttpStatusCode.OK;
                                    }

                                    break;
                            }

                            break;

                        case "/BOURBON_XML/stats/Stats_BinaryStatsDownload_Submit.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        byte[]? stats_BinaryStatsDownload_Submit = null;

                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? acctId = HttpUtility.ParseQueryString(request.Url.Query).Get("accountID");
                                        string? blobType = HttpUtility.ParseQueryString(request.Url.Query).Get("blobType");

                                        if (string.IsNullOrEmpty(acctId) || string.IsNullOrEmpty(blobType))
                                        {
                                            response.Headers.Set("x-statuscode", "-608");
                                            response.StatusCode = 500;

                                            stats_BinaryStatsDownload_Submit = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                    "<XML>\r\n" +
                                                    "    <Server_Error>\r\n" +
                                                    "        <id>-608</id>\r\n" +
                                                    "        <message>DATA_NOT_AVAILABLE</message>\r\n" +
                                                    "    </Server_Error>\r\n" +
                                                    "    <logoutURL>http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/Account_Logout.do</logoutURL>\r\n" +
                                                    "</XML>\r\n" +
                                                    " \r\n");
                                        }
                                        else
                                        {
                                            // get directory to store the binary blob files
                                            string directoryPath = $"{SVOServerConfiguration.SVOStaticFolder}/BOURBON_XML/stats/StatBlobs/";
                                            Directory.CreateDirectory(directoryPath);
                                            string blobFile = directoryPath + $"svoStatsBlob-a{acctId}-type{blobType}.bin";
                                            if (File.Exists(blobFile))
                                            {
                                                stats_BinaryStatsDownload_Submit = File.ReadAllBytes(blobFile);

                                                response.StatusCode = 200;
                                                response.ContentLength64 = stats_BinaryStatsDownload_Submit.Length;
                                                response.Headers.Set("Content-Disposition", $"filename=svoStatsBlob-a{acctId}-type{blobType}.bin");
                                                response.Headers.Set("x-statuscode", "0");
                                                response.ContentType = "application/octet-stream; charset=UTF-8";
                                            }
                                            else
                                            {

                                                response.Headers.Set("x-statuscode", "-608");
                                                response.StatusCode = 500;

                                                stats_BinaryStatsDownload_Submit = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                        "<XML>\r\n" +
                                                        "    <Server_Error>\r\n" +
                                                        "        <id>-608</id>\r\n" +
                                                        "        <message>DATA_NOT_AVAILABLE</message>\r\n" +
                                                        "    </Server_Error>\r\n" +
                                                        "    <logoutURL>http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/Account_Logout.do</logoutURL>\r\n" +
                                                        "</XML>\r\n" +
                                                        " \r\n");
                                            }
                                        }

                                        if (response.OutputStream.CanWrite && stats_BinaryStatsDownload_Submit != null)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = stats_BinaryStatsDownload_Submit.Length;
                                                response.OutputStream.Write(stats_BinaryStatsDownload_Submit, 0, stats_BinaryStatsDownload_Submit.Length);
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

                        case "/BOURBON_XML/stats/Stats_CareerStatsByID.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? playerID = HttpUtility.ParseQueryString(request.Url.Query).Get("playerID");

                                        response.Headers.Set("x-statuscode", "0");

                                        byte[] stats_BinaryStatsDownload_Submit = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<XML>\r\n" +
                                                "    <Career_Leaderboard>\r\n" +
                                                "        <userID>0</userID>\r\n" +
                                                "        <username></username>\r\n" +
                                                "        <rank></rank>\r\n" +
                                                "        <total></total>\r\n" +
                                                "        <stat></stat>\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>DATA_NOT_AVAILABLE</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </Career_Leaderboard>\r\n" +
                                                "</XML>\r\n" +
                                                " \r\n");

                                        response.StatusCode = 200;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = stats_BinaryStatsDownload_Submit.Length;
                                                response.OutputStream.Write(stats_BinaryStatsDownload_Submit, 0, stats_BinaryStatsDownload_Submit.Length);
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

                        case "/BOURBON_XML/game/Game_Create_Submit.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("x-statuscode", "0");

                                        byte[] game_Create_Submit = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<XML>\r\n" +
                                                "    <Create_Game>\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>41007</id>\r\n" +
                                                "        </status>\r\n" +
                                                $"       <gameID>{HttpUtility.ParseQueryString(request.Url.Query).Get("scertGameID")}</gameID>\r\n" +
                                                "    </Create_Game>\r\n" +
                                                "</XML>");

                                        response.StatusCode = 200;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = game_Create_Submit.Length;
                                                response.OutputStream.Write(game_Create_Submit, 0, game_Create_Submit.Length);
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

                        case "/BOURBON_XML/game/Game_PostBinaryStats_Submit.do":
                            switch (request.HttpMethod)
                            {
                                case "POST":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        var formData = HttpUtility.ParseQueryString(request.QueryString.ToString());

                                        byte[]? urlEncoded = null;

                                        // Find number of bytes in stream.
                                        int strLen = Convert.ToInt32(request.ContentLength64);
                                        // Create a byte array.
                                        urlEncoded = new byte[strLen];

                                        request.InputStream.Read(urlEncoded, 0, strLen);

                                        // Split the URL-encoded string based on "endStatus"
                                        string[] parts = Encoding.UTF8.GetString(urlEncoded, 0, urlEncoded.Length).Split(new[] { "&endStatus=1", "&endStatus=2", "&endStatus=3", "&endStatus=4", "&endStatus=5", "&endStatus=6", "&endStatus=7", }, StringSplitOptions.None);

                                        // Create a directory to store the binary blob files
                                        string directoryPath = $"{SVOServerConfiguration.SVOStaticFolder}/BOURBON_XML/stats/StatBlobs/";
                                        Directory.CreateDirectory(directoryPath);

                                        // Initialize a list to store statistics for each part
                                        List<Dictionary<string, string>> statisticsList = new List<Dictionary<string, string>>();

                                        // Iterate through each part
                                        foreach (var part in parts)
                                        {
                                            // Parse the part as a URL-encoded string
                                            var parsedData = HttpUtility.ParseQueryString(part);

                                            // Check if the part contains the "AccountID" key
                                            if (parsedData.AllKeys.Contains("AccountID"))
                                            {
                                                // Extract the AccountID and statistics for the part
                                                string? accountId = parsedData["AccountID"];
                                                Dictionary<string, string> statistics = new Dictionary<string, string>();

                                                // Iterate through all keys except "AccountID" to collect statistics
                                                foreach (var key in parsedData.AllKeys)
                                                {
                                                    if (key != null && key != "AccountID")
                                                        statistics[key] = parsedData[key];

                                                    switch (key)
                                                    {
                                                        case "type1":
                                                            {
                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data1"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type1.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type2":
                                                            {
                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data2"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type2.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type3":
                                                            {
                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data3"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type3.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type4":
                                                            {
                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data4"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type4.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type5":
                                                            {
                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data5"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type5.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type6":
                                                            {

                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data6"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type6.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type7":
                                                            {

                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data7"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type7.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type8":
                                                            {

                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data8"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type8.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type9":
                                                            {

                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data9"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type9.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type100":
                                                            {

                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data100"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type100.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type300":
                                                            {

                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data300"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type300.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                        case "type500":
                                                            {
                                                                // Convert Base64 to bytes
                                                                byte[] data = parsedData["data500"].IsBase64().Item2;
                                                                // Convert bytes to a human-readable string (assuming UTF-8 encoding)
                                                                string humanReadableString = Encoding.UTF8.GetString(data);
                                                                // Save the binary data as a .bin file
                                                                string filePath = Path.Combine(directoryPath, $"svoStatsBlob-a{accountId}-type500.bin");
                                                                File.WriteAllText(filePath, humanReadableString);
                                                            }
                                                            break;
                                                    }
                                                }

                                                // Store the statistics in the list
                                                statisticsList.Add(statistics);
                                            }
                                        }

                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("x-statuscode", "0");

                                        byte[] game_Finish_Submit = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<XML>\r\n" +
                                                "    <Finish_Game>\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>41010</id>\r\n" +
                                                "            <message>GAME_FINISH_SUCCESS</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </Finish_Game>\r\n" +
                                                "</XML>");

                                        response.StatusCode = 200;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = game_Finish_Submit.Length;
                                                response.OutputStream.Write(game_Finish_Submit, 0, game_Finish_Submit.Length);
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

                        case "/BOURBON_XML/clan/Clan_Home.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? fileNameBeginsWith = HttpUtility.ParseQueryString(request.Url.Query).Get("fileNameBeginsWith");

                                        response.Headers.Set("x-statuscode", "0");

                                        byte[] clan_homw = Encoding.UTF8.GetBytes("<XML>\r\n" +
                                                "    <MsgBoard_Home>\r\n" +
                                                "        <MsgBoard_Moderate_Names MsgBoard_AddSubTopic=\"Add Sub-Topic\"\r\n" +
                                                "            MsgBoard_AddTopic=\"Add Topic\"\r\n" +
                                                "            MsgBoard_BanPlayer=\"Add Ban User\"\r\n" +
                                                "            MsgBoard_BanPlayers=\"View Banned Users\" MsgBoard_Topics=\"View Topics\"/>\r\n" +
                                                "        <MsgBoard_Moderate_Clan_URLs\r\n" +
                                                "            addSubTopicURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanAddSubTopic.do\"\r\n" +
                                                "            addTopicURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanAddTopic.do\"\r\n" +
                                                "            banUserURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanBanPlayer.do\"\r\n" +
                                                "            moderateHomeURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanHome.do\"\r\n" +
                                                "            topicsURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanTopics.do\" viewBannedUsersURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_Moderate_ClanBannedPlayers.do\"/>\r\n" +
                                                "        <MsgBoard_Clan_URLs topicsURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/msgboard/MsgBoard_ClanTopics.do\"/>\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>Success</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </MsgBoard_Home>\r\n" +
                                                "    <My_Clan clanID=\"0\" clanName=\"\" clanTag=\"\" createDate=\"\"\r\n" +
                                                "        hasClanAdminPrivileges=\"0\" isClanLeader=\"0\" leaderID=\"0\"\r\n" +
                                                "        leaderName=\"\" status=\"-1\"/>\r\n" +
                                                "    <Clan_Players clanID=\"0\"/>\r\n" +
                                                "    <clanURLS\r\n" +
                                                "        clanCreateNewsURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_CreateNews.do\"\r\n" +
                                                "        clanCreateURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_Create.do\"\r\n" +
                                                "        clanDisbandURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_Disband.do\"\r\n" +
                                                "        clanLeaveURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_Leave.do\"\r\n" +
                                                "        clanMailURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/mail/Mail_Clan_Send.do\"\r\n" +
                                                "        clanModifyNewsURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ModifyNews.do\"\r\n" +
                                                "        clanReadNewsURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ReadNews.do\"\r\n" +
                                                "        clanRemoveAllMetadataURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RemoveAllMetaData.do\"\r\n" +
                                                "        clanRemoveMemberURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RemoveMember.do\"\r\n" +
                                                "        clanRemoveMetadataURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RemoveMetaData.do\"\r\n" +
                                                "        clanSendInviteURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_SendInvite.do\"\r\n" +
                                                "        clanTransferLeaderURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_TransferLeadership.do\"\r\n" +
                                                "        clanUpdateMetadataFormURL=\"\"\r\n" +
                                                "        clanUpdateTagURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_UpdateClanTag.do\"\r\n" +
                                                "        clanViewInvitesURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ViewInvites.do\"" +
                                                "        clanViewSentInvitesURL=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_ViewSentInvites.do\"/>\r\n" +
                                                "    <status>\r\n" +
                                                "        <id>0</id>\r\n" +
                                                "        <message>Success</message>\r\n" +
                                                "    </status>\r\n</XML>");

                                        response.StatusCode = 200;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = clan_homw.Length;
                                                response.OutputStream.Write(clan_homw, 0, clan_homw.Length);
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

                        case "/BOURBON_XML/clan/Clan_CreateWithTag_Submit.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? clanName = HttpUtility.ParseQueryString(request.Url.Query).Get("clanName");
                                        string? clanTAG = HttpUtility.ParseQueryString(request.Url.Query).Get("clanTAG");

                                        response.Headers.Set("x-statuscode", "0");

                                        byte[] clan_CreateWithTag_Submit = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n" +
                                                "<XML>\r\n\t" +
                                                "   <Clan>\r\n\t\t" +
                                                "       <status>\r\n\t\t\t" +
                                                "           <id>1</id>\r\n\t\t\t" +
                                                "           <message>Success</message>\r\n\t\t" +
                                                "       </status>\r\n\t" +
                                                "   </Clan>\r\n" +
                                                "</XML>");

                                        response.StatusCode = 200;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = clan_CreateWithTag_Submit.Length;
                                                response.OutputStream.Write(clan_CreateWithTag_Submit, 0, clan_CreateWithTag_Submit.Length);
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

                        case "/BOURBON_XML/clan/Clan_ViewInvites.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("x-statuscode", "0");

                                        byte[] clan_ViewInvites = Encoding.UTF8.GetBytes("<XML>\r\n" +
                                                "    <respondInvite\r\n" +
                                                "        action=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RespondInvite_Submit.do\" clanViewInviteURL=\"\">\r\n" +
                                                "        <newsValid maxLength=\"600\" name=\"news\" type=\"textarea\"/>\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>Success</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </respondInvite>\r\n" +
                                                "    <clanInvites isInAClan=\"0\">\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>Success</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </clanInvites>\r\n" +
                                                "</XML>\r\n \r\n");

                                        response.StatusCode = 200;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = clan_ViewInvites.Length;
                                                response.OutputStream.Write(clan_ViewInvites, 0, clan_ViewInvites.Length);
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

                        case "/BOURBON_XML/clan/Clan_GetClanInfoByID.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? clanID = HttpUtility.ParseQueryString(request.Url.Query).Get("clanID");

                                        response.Headers.Set("x-statuscode", "-999900");

                                        byte[] clan_ViewInvites = Encoding.UTF8.GetBytes("<XML>\r\n" +
                                                "    <Server_Error>\r\n" +
                                                "        <id>-999900</id>\r\n" +
                                                "        <message>ERROR_MISSING_STATUSCODE</message>\r\n" +
                                                "    </Server_Error>\r\n" +
                                                "    <logoutURL>http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/account/Account_Logout.do</logoutURL>\r\n" +
                                                "</XML>");

                                        response.StatusCode = 500;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = clan_ViewInvites.Length;
                                                response.OutputStream.Write(clan_ViewInvites, 0, clan_ViewInvites.Length);
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

                        case "/BOURBON_XML/clan/Clan_GetClanPlayers.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? clanID = HttpUtility.ParseQueryString(request.Url.Query).Get("clanID");

                                        response.Headers.Set("x-statuscode", "0");

                                        byte[] clan_GetClanPlayers = Encoding.UTF8.GetBytes("<XML>\r\n" +
                                                "    <Clan_Players clanID=\"0\" end=\"-1\" start=\"-1\" total=\"0\">\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>Success</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </Clan_Players>\r\n" +
                                                "</XML>");

                                        response.StatusCode = 200;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = clan_GetClanPlayers.Length;
                                                response.OutputStream.Write(clan_GetClanPlayers, 0, clan_GetClanPlayers.Length);
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

                        case "/BOURBON_XML/clan/Clan_ViewSentInvites.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);
                                        response.Headers.Set("x-statuscode", "0");

                                        byte[] clan_ViewSentInvites = Encoding.UTF8.GetBytes("<XML>\r\n" +
                                                "    <revokeInvitation action=\"http://starhawk-prod2.svo.online.scea.com:10060/BOURBON_XML/clan/Clan_RevokeInvite_Submit.do\">\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>Success</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </revokeInvitation>\r\n" +
                                                "    <clanInvites>\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>Success</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </clanInvites>\r\n" +
                                                "</XML>");

                                        response.StatusCode = 200;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = clan_ViewSentInvites.Length;
                                                response.OutputStream.Write(clan_ViewSentInvites, 0, clan_ViewSentInvites.Length);
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

                        case "/BOURBON_XML/clan/Clan_ReadNews.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? clanID = HttpUtility.ParseQueryString(request.Url.Query).Get("clanID");

                                        response.Headers.Set("x-statuscode", "0");

                                        byte[] clan_ReadNews = Encoding.UTF8.GetBytes("<XML>\r\n" +
                                                "    <clanNews end=\"9\" start=\"0\" total=\"0\">\r\n" +
                                                "        <status>\r\n" +
                                                "            <id>0</id>\r\n" +
                                                "            <message>Success</message>\r\n" +
                                                "        </status>\r\n" +
                                                "    </clanNews>\r\n" +
                                                "</XML>");

                                        response.StatusCode = 200;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = clan_ReadNews.Length;
                                                response.OutputStream.Write(clan_ReadNews, 0, clan_ReadNews.Length);
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

                        case "/BOURBON_XML/calendar/Calendar_GetEvents.do":
                            switch (request.HttpMethod)
                            {
                                case "GET":

                                    string? clientMac = request.Headers.Get("X-SVOMac");

                                    string? serverMac = SVOProcessor.CalcuateSVOMac(clientMac);

                                    if (string.IsNullOrEmpty(serverMac))
                                    {
                                        response.StatusCode = (int)System.Net.HttpStatusCode.Forbidden;
                                        return;
                                    }
                                    else
                                    {
                                        response.Headers.Set("Content-Type", "text/xml;charset=UTF-8");
                                        response.Headers.Set("X-SVOMac", serverMac);

                                        string? accountID = HttpUtility.ParseQueryString(request.Url.Query).Get("accountID");
                                        string? clanID = HttpUtility.ParseQueryString(request.Url.Query).Get("clanID");
                                        string? startDate = HttpUtility.ParseQueryString(request.Url.Query).Get("startDate");
                                        string? endDate = HttpUtility.ParseQueryString(request.Url.Query).Get("endtDate");
                                        string? bTween = HttpUtility.ParseQueryString(request.Url.Query).Get("bTween");
                                        string? filterUserCreatedTourneys = HttpUtility.ParseQueryString(request.Url.Query).Get("filterUserCreatedTourneys");

                                        response.Headers.Set("x-statuscode", "0");

                                        byte[] Calendar_GetEvents = Encoding.UTF8.GetBytes("<XML>" +
                                                "\r\n\t<CalendarEvents>" +
                                                "\r\n\t\t<status>" +
                                                "\r\n\t\t\t<id>0</id>" +
                                                "\r\n\t\t\t<message>Success</message>" +
                                                "\r\n\t\t</status>" +
                                                "\r\n\t\t<Event>" +
                                                "\r\n\t\t\t<event_id value=\"1\" />" +
                                                "\r\n\t\t\t<category_id value=\"0\" />" +
                                                "\r\n\t\t\t<Title value=\"Test Event\" />" +
                                                "\r\n\t\t\t<Description value=\"Test Description\" />" +
                                                "\r\n\t\t\t<StartDate value=\"22-03-31\" />" +
                                                "\r\n\t\t\t<StartTime value=\"1679481752\" />" +
                                                "\r\n\t\t\t<EndDate value=\"22-03-31\" />" +
                                                "\r\n\t\t\t<EndTime value=\"1679535752\" />\r\n\t\t\t<IsActive value=\"1\" />\r\n\t\t\t" +
                                                "<IsApproved value=\"1\" />\r\n\t\t\t<IsBillboard value=\"1\" />\r\n\t\t\t<SeriesID value=\"1\" />\r\n\t\t\t" +
                                                "<series_event_type_id value=\"0\" />\r\n\t\t\t<icon value=\"\" />\r\n\t\t\t<color value=\"\" />\r\n\t\t\t" +
                                                "<generic_A value=\"0\" />\r\n\t\t\t<generic_B value=\"0\" />\r\n\t\t\t<generic_C value=\"0\" />\r\n\t\t" +
                                                "</Event>\r\n\t</CalendarEvents>\r\n</XML>");

                                        response.StatusCode = 200;

                                        if (response.OutputStream.CanWrite)
                                        {
                                            try
                                            {
                                                response.ContentLength64 = Calendar_GetEvents.Length;
                                                response.OutputStream.Write(Calendar_GetEvents, 0, Calendar_GetEvents.Length);
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
                LoggerAccessor.LogError($"[SVO] - Starhawk_SVO thrown an assertion - {ex}");
                response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
            }
        }
    }
}
