using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using WebAPIService.GameServices.UBISOFT.Models;

namespace WebAPIService.GameServices.UBISOFT.OnlineConfigService
{
    public static class JsonData
    {
        private readonly static Dictionary<string, string> DriverSanFransiscoPCResponse = new Dictionary<string, string>()
        {
            { "SandboxUrl",                     @"prudp:/address=pdc-lb-rdv-prod01.ubisoft.com;port=60105"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv01.ubisoft.com:60105"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

        private readonly static Dictionary<string, string> Hawx2PS3Response = new Dictionary<string, string>()
        {
            { "SandboxUrlPS3",                  @"prudp:/address=pdc-lb-rdv-prod02.ubisoft.com;port=60115;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv02.ubisoft.com:60115"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

        private readonly static Dictionary<string, string> DriverSanFransiscoPS3Response = new Dictionary<string, string>()
        {
            { "GatorAuthServiceActivity",       @"DRIVER5PS3"},
            { "GatorAuthServiceURI",            @"http://gatorservice.ubi.com/AuthenticationServiceSSL.svc"},
            { "GatorConfigServiceURI",          @"http://gatorservice.ubi.com/ConfigService.svc"},
            { "GatorContentServiceURI",         @"http://gatorservice.ubi.com/ContentService.svc"},
            { "GatorServiceHost",               @"gatorservice.ubi.com"},
            { "GatorServicePort",               @"80"},
            { "GatorTransferServiceURI",        @"/TransferService.svc"},
            { "GatorUserServiceURI",            @"http://gatorservice.ubi.com/GatorUserService.svc"},
            { "NetworkPlatformServiceId",       @"EP0001-BLES00891_00"},
            { "SandboxUrlPS3",                  @"prudp:/address=pdc-lb-rdv-prod03.ubisoft.com;port=61110;serviceid=UP0001-BLUS30536_00"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv03.ubisoft.com:61110"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.0.3-RC-Share/test7/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };
		
        private readonly static Dictionary<string, string> PureFootballPS3Response = new Dictionary<string, string>()
        {
            { "AuthenticationServer",           @"lb-lsg-auth.ubisoft.com:3078"},
            { "GatorAuthServiceActivity",       @"UGC_PUREFOOTBALL_PS3"},
            { "GatorAuthServiceHost",           @"gatorservice.ubi.com"},
            { "GatorAuthServicePort",           @"443"},
            { "GatorConfigServiceURI",          @"http://secure.ubi.com/gatorservice/ConfigService.svc"},
            { "GatorContentServiceURI",         @"http://secure.ubi.com/gatorservice/ContentService.svc"},
            { "GatorServiceHost",               @"gatorservice.ubi.com"},
            { "GatorServicePort",               @"80"},
            { "GatorTransferServiceURI",        @"/TransferService.svc"},
            { "GatorUserServiceURI",            @"http://secure.ubi.com/gatorservice/GatorUserService.svc"},
            { "LobbyServer",                    @"lb-lsg-prod.ubisoft.com:3106"},
            { "MmpTitleId",                     @"0x2777" },
            { "NetworkPlatformCommunicationId", @"NPWR00657_00" },
            { "NetworkPlatformServiceId",       @"UP0001-BLUS30411_00" },
            { "SandboxUrlPS3",                  @"prudp:/address=lb-rdv-prod10.ubisoft.com;port=25000;serviceid=UP0001-BLUS30411_00" },
            { "SandboxUrlWS",                   @"lb-rdv-prod10.ubisoft.com:25000" },
            { "uplay_DownloadServiceUrl",       @"http://secure.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url=" },
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/" },
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.ubi.com/" },
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.0.1/" },
            { "uplay_WebServiceBaseUrl",        @"http://secure.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/" },
        };

        private readonly static Dictionary<string, string> GhostReconFutureSoldierPS3Response = new Dictionary<string, string>()
        {
            { "SandboxUrlPS3",                  @"prudp:/address=pdc-lb-rdv-prod04.ubisoft.com;port=61120;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv04.ubisoft.com:61120"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

private readonly static Dictionary<string, string> GhostReconFutureSoldierMPBetaPS3Response = new Dictionary<string, string>()
        {
            { "SandboxUrlPS3",                  @"prudp:/address=pdc-lb-rdv-prod04.ubisoft.com;port=61119;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv04.ubisoft.com:61119"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

        private readonly static Dictionary<string, string> GhostReconOnlinePCResponse = new Dictionary<string, string>()
        {
            { "SandboxUrl",                     @"prudp:/address=lb-rdv-as-prod01.ubisoft.com;port=21221"},
            { "SandboxUrlWS",                   @"ne1-z3-as-rdv06.ubisoft.com:21215"},
            { "uplay_DownloadServiceUrl",       @"http://secure.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.0.1/"},
            { "uplay_WebServiceBaseUrl",        @"http://secure.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

        private readonly static Dictionary<string, string> ACRevelationsPS3Response = new Dictionary<string, string>()
        {
            { "SandboxUrlPS3",                  @"prudp:/address=lb-ne1-z02-rdv-ac.ubisoft.com;port=21067;serviceid=UP0001-BLUS30808_00"},
            { "SandboxUrlWS",                   @"ne1-z2-ac-rdv04.ubisoft.com:21065"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

        private readonly static Dictionary<string, string> ACBrotherhoodPS3Response = new Dictionary<string, string>()
        {
            { "SandboxUrlPS3",                  @"prudp:/address=lb-ne1-z02-rdv-ac.ubisoft.com;port=23941;serviceid=UP0001-NPUB90483_00"},
            { "SandboxUrlWS",                   @"ne1-z2-ac-rdv04.ubisoft.com:23940"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.0.3-RC-Shop/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

        private readonly static Dictionary<string, string> ACBrotherhoodMultiPS3Response = new Dictionary<string, string>()
        {
            { "NetworkPlatformServiceId",       @"UP0001-NPUB90483_00"}, 
            { "SandboxUrlPS3",                  @"prudp:/address=lb-ne1-z02-rdv-ac.ubisoft.com;port=23901;serviceid=UP0001-NPUB90483_00"},
            { "SandboxUrlWS",                   @"ne1-z2-ac-rdv04.ubisoft.com:23900"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.0.3-RC-Shop/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

        private readonly static Dictionary<string, string> AC3PS3Response = new Dictionary<string, string>()
        {
            { "GenomeId",                       @"9a36ff77-d110-45c9-9dfb-2568d326f0f4"},
            { "killSwitch_contentFiltering",    @"0"},
            { "killSwitch_friendsProposal",     @"0"},
            { "killSwitch_friendsRequest",      @"0"},
            { "killSwitch_friendsSuggestions",  @"0"},
            { "killSwitch_socialFeed",          @"0"},
            { "killSwitch_socialFeedWrites",    @"0"},
            { "killSwitch_uplay",               @"0"},
            { "SandboxUrlPrivPS3",              @"prudp:/address=pdc-lb-rdv-prod05.ubisoft.com;port=62125;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlPS3",                  @"prudp:/address=pdc-lb-rdv-prod05.ubisoft.com;port=61125;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv05.ubisoft.com:61125"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
            { "UplayGameCode",                  @"AC3"},
            { "US_AcceptSuggestion_PS3",        @"http://friendsservice.ubi.com/friendsservice.svc/REST/AcceptSuggestion "},
            { "US_CommentPost_PS3",             @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/InsertWallPostComment/"},
            { "US_DeclineSuggestion_PS3",       @"http://friendsservice.ubi.com/friendsservice.svc/REST/DeclineSuggestion "},
            { "US_GetWallPostsByIds_PS3",       @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/GetWallPostsByIds/"},
            { "US_GetWalls_PS3",                @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/GetWalls/"},
            { "US_GetWallsPost_PS3",            @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/GetWallPosts/"},
            { "US_InitializeUser_PS3",          @"http://wspuplay-ext.ubi.com/UplayServices/WinServices/GameClientServices.svc/REST/JSON/InitializeUser/"},
            { "US_InsertPost_PS3",              @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/InsertWallPost/"},
            { "US_LikePost_PS3",                @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/InsertWallPostLike/"},
            { "US_Login_PS3",                   @"http://signupservice.ubi.com/PublicOnlineUserService.svc/REST/LogIn "},
            { "US_Logout_PS3",                  @"http://signupservice.ubi.com/PublicSignupService.svc/REST/LogOut "},
            { "US_LookupExternalId_PS3",        @"http://signupservice.ubi.com/PublicSignupService.svc/REST/LookupExternalAccountIDsByUbiAccountIDs "},
            { "US_LookupUplayId_PS3",           @"http://signupservice.ubi.com/PublicSignupService.svc/REST/LookupUbiAccountIDsByExternalAccountNames "},
            { "US_LookupUplayUsername_PS3",     @"http://signupservice.ubi.com/PublicSignupService.svc/REST/LookupUsernamesByUbiAccountIDs "},
            { "US_RequestFriends_PS3",          @"http://friendsservice.ubi.com/friendsservice.svc/REST/GetRelationshipsWithPresenceByToken "},
            { "US_RequestSuggestions_PS3",      @"http://friendsservice.ubi.com/friendsservice.svc/REST/GetSuggestionsByPage "},
            { "US_SharePost_PS3",               @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/ShareWallPost/"},
            { "US_SyncFriends_PS3",             @"http://friendsservice.ubi.com/friendsservice.svc/REST/ProposeContacts "},
        };

        private readonly static Dictionary<string, string> AC3MULTPS3Response = new Dictionary<string, string>()
        {
            { "GenomeId",                       @"9a36ff77-d110-45c9-9dfb-2568d326f0f4"},
            { "killSwitch_contentFiltering",    @"0"},
            { "killSwitch_friendsProposal",     @"0"},
            { "killSwitch_friendsRequest",      @"0"},
            { "killSwitch_friendsSuggestions",  @"0"},
            { "killSwitch_socialFeed",          @"0"},
            { "killSwitch_socialFeedWrites",    @"0"},
            { "killSwitch_uplay",               @"0"},
            { "SandboxUrlPrivPS3",              @"prudp:/address=pdc-lb-rdv-prod06.ubisoft.com;port=62127;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlPS3",                  @"prudp:/address=pdc-lb-rdv-prod06.ubisoft.com;port=61127;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv06.ubisoft.com:61127"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
            { "UplayGameCode",                  @"AC3"},
            { "US_AcceptSuggestion_PS3",        @"http://friendsservice.ubi.com/friendsservice.svc/REST/AcceptSuggestion "},
            { "US_CommentPost_PS3",             @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/InsertWallPostComment/"},
            { "US_DeclineSuggestion_PS3",       @"http://friendsservice.ubi.com/friendsservice.svc/REST/DeclineSuggestion "},
            { "US_GetWallPostsByIds_PS3",       @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/GetWallPostsByIds/"},
            { "US_GetWalls_PS3",                @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/GetWalls/"},
            { "US_GetWallsPost_PS3",            @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/GetWallPosts/"},
            { "US_InitializeUser_PS3",          @"http://wspuplay-ext.ubi.com/UplayServices/WinServices/GameClientServices.svc/REST/JSON/InitializeUser/"},
            { "US_InsertPost_PS3",              @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/InsertWallPost/"},
            { "US_LikePost_PS3",                @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/InsertWallPostLike/"},
            { "US_Login_PS3",                   @"http://signupservice.ubi.com/PublicOnlineUserService.svc/REST/LogIn "},
            { "US_Logout_PS3",                  @"http://signupservice.ubi.com/PublicSignupService.svc/REST/LogOut "},
            { "US_LookupExternalId_PS3",        @"http://signupservice.ubi.com/PublicSignupService.svc/REST/LookupExternalAccountIDsByUbiAccountIDs "},
            { "US_LookupUplayId_PS3",           @"http://signupservice.ubi.com/PublicSignupService.svc/REST/LookupUbiAccountIDsByExternalAccountNames "},
            { "US_LookupUplayUsername_PS3",     @"http://signupservice.ubi.com/PublicSignupService.svc/REST/LookupUsernamesByUbiAccountIDs "},
            { "US_RequestFriends_PS3",          @"http://friendsservice.ubi.com/friendsservice.svc/REST/GetRelationshipsWithPresenceByToken "},
            { "US_RequestSuggestions_PS3",      @"http://friendsservice.ubi.com/friendsservice.svc/REST/GetSuggestionsByPage "},
            { "US_SharePost_PS3",               @"http://wspuplay-ext.ubi.com/UplayServices/ShareServices/GameClientServices.svc/REST/JSON/ShareWallPost/"},
            { "US_SyncFriends_PS3",             @"http://friendsservice.ubi.com/friendsservice.svc/REST/ProposeContacts "},
        };

        private readonly static Dictionary<string, string> AC2PS3Response = new Dictionary<string, string>()
        {
            { "SandboxUrlPS3",                  @"prudp:/address=pdc-lb-rdv-prod07.ubisoft.com;port=61129;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv07.ubisoft.com:61129"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };


        private readonly static Dictionary<string, string> BeyondGoodAndEvilHDPS3Response = new Dictionary<string, string>()
        {
            { "SandboxUrlPS3",                  @"prudp:/address=pdc-lb-rdv-prod08.ubisoft.com;port=61137;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv08.ubisoft.com:61137"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

        private readonly static Dictionary<string, string> RaymanLegendsPS3Response = new Dictionary<string, string>()
        {
            { "SandboxUrlPS3",                  @"prudp:/address=pdc-lb-rdv-prod09.ubisoft.com;port=61139;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv09.ubisoft.com:61139"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

        private readonly static Dictionary<string, Tuple<string, string>> SCBLACKLISTPS3Response = new Dictionary<string, Tuple<string, string>>()
        {
            { "punch_DetectUrls",               new Tuple<string, string>(@"lb-prod-mm-detect01.ubisoft.com:11020", "lb-prod-mm-detect02.ubisoft.com:11020")},
            { "SandboxUrlPS3",                  new Tuple<string, string>(@"prudp:/address=lb-rdv-as-prod01.ubisoft.com;port=61131;serviceid=UPxxxx-MYGAME", null)},
            { "SandboxUrlWS",                   new Tuple<string, string>(@"ne1-z3-as-rdv03.ubisoft.com:61131", null)},
            { "uplay_DownloadServiceUrl",       new Tuple<string, string>(@"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url=", null)},
            { "uplay_DynContentBaseUrl",        new Tuple<string, string>(@"http://static8.cdn.ubi.com/u/Uplay/", null)},
            { "uplay_DynContentSecureBaseUrl",  new Tuple<string, string>(@"http://static8.cdn.ubi.com/", null)},
            { "uplay_LinkappBaseUrl",           new Tuple<string, string>(@"http://static8.cdn.ubi.com/private/Uplay/Packages/linkapp/3.0.0-rc/", null)},
            { "uplay_MovieBaseUrl",             new Tuple<string, string>(@"http://static8.cdn.ubi.com/u/Uplay/", null)},
            { "uplay_PackageBaseUrl",           new Tuple<string, string>(@"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/", null)},
            { "uplay_WebServiceBaseUrl",        new Tuple<string, string>(@"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/", null)},
        };
		
		private readonly static Dictionary<string, string> IAAPS3Response = new Dictionary<string, string>()
        {
            { "SandboxUrl",                     @"prudp:/address=lb-rdv-prod01.ubisoft.com;port=60106;serviceid=UPxxxx-MYGAME"},
            { "TrackingSandboxURLPS3",          @"prudp:/address=lb-rdv-prod01.ubisoft.com;port=60106;serviceid=UPxxxx-MYGAME"},
            { "SandboxUrlWS",                   @"pdc-vm-rdv01.ubisoft.com:60106"},
        };

        private readonly static Dictionary<string, string> JCAvatarTheGamePS3Response = new Dictionary<string, string>()
        {
            { "SandboxUrlPS3",                  @"prudp:/address=lb-rdv-prod02.ubisoft.com;port=24250;serviceid=UP0001-BLUS30374_00"},
            { "SandboxUrlWS",                   @"lb-rdv-prod02.ubisoft.com:24250"},
            { "TrackingSandboxURLPs3",          @"prudp:/address=lb-rdv-prod02.ubisoft.com;port=24250;serviceid=UP0001-BLUS30374_00"},
            { "uplay_DownloadServiceUrl",       @"http://wsuplay.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url="},
            { "uplay_DynContentBaseUrl",        @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_DynContentSecureBaseUrl",  @"http://static8.cdn.ubi.com/"},
            { "uplay_LinkappBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/linkapp/1.1/"},
            { "uplay_MovieBaseUrl",             @"http://static8.cdn.ubi.com/u/Uplay/"},
            { "uplay_PackageBaseUrl",           @"http://static8.cdn.ubi.com/u/Uplay/Packages/1.5-Share-rc/"},
            { "uplay_WebServiceBaseUrl",        @"http://wsuplay.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/"},
        };

        public static string GetOnlineConfigPSN(string onlineConfigID)
        {
            if (string.IsNullOrEmpty(onlineConfigID))
                return string.Empty;

            List<OnlineConfigEntry> list = new List<OnlineConfigEntry>();

            switch (onlineConfigID)
            {
                case "f70cace9c14a4656bccb03a661dbe660":
                    foreach (var v in AC3MULTPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "90a8c600b48142629f1e102133e18398":
                    foreach (var v in AC3PS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "1d0a2b0e95c649a6a473f5754e2126d2":
                    foreach (var v in ACRevelationsPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "d2f9d73de80e4e10971ff979e9d25649":
                    foreach (var v in ACBrotherhoodPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "1ebebc7e4d514f60a3f15f90029adf1e":
                    foreach (var v in ACBrotherhoodMultiPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "749b17f318b7463cbaa4eb5bc19527e5":
                    foreach (var v in AC2PS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "1d6d2cd10bdc8fb1ed14b04749481873":
                    foreach (var v in JCAvatarTheGamePS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "36e80e4e6db64f04877c928211c8bb71":
                    foreach (var v in Hawx2PS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "e330746d922f44e3b7c2c6e5637f2e53":
                    foreach (var v in DriverSanFransiscoPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "06f0e94af5bb4f3b83339b4c0c91d714":
                    foreach (var v in GhostReconFutureSoldierMPBetaPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "18a7f5db34d34b118c1a0b8eeb517fd7":
                    foreach (var v in GhostReconFutureSoldierPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "23ad683803a0457cabce83f905811dbc":
                    foreach (var v in GhostReconOnlinePCResponse)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "acd2bb86618441a7b2af9b4b952c9612":
                    foreach (var v in SCBLACKLISTPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = v.Value.Item2 != null ? new[] { v.Value.Item1, v.Value.Item2 } : new[] { v.Value.Item1 }
                        });
                    }
                    break;
                case "7a5a8edc9e2b42abb12b47d04e531b26":
                    foreach (var v in IAAPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "0879cd6bbf17e9cbf6cf44fb35c0142f":
                    foreach (var v in PureFootballPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "295583213fd84aa6a79291455f558402":
                    foreach (var v in BeyondGoodAndEvilHDPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                case "0ae01881bc294a06a95a0e086a6edf0c":
                    foreach (var v in RaymanLegendsPS3Response)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
                default:
                    foreach (var v in DriverSanFransiscoPCResponse)
                    {
                        list.Add(new OnlineConfigEntry
                        {
                            Name = v.Key,
                            Values = new[] { v.Value }
                        });
                    }
                    break;
            }

            return JsonConvert.SerializeObject(list);
        }
    }
}
