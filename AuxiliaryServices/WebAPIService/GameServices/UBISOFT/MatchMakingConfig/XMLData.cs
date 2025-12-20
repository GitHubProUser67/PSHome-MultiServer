namespace WebAPIService.GameServices.UBISOFT.MatchMakingConfig
{
    public class XMLData
    {
        public readonly static string DFS_PS3_NTSC_EN_XMLPayload = "<RESPONSE xmlns=\"\">" +
            "<GatorAuthServiceActivity><VALUE>DRIVER5PS3</VALUE></GatorAuthServiceActivity><GatorAuthServiceURI>" +
            "<VALUE>http://gatorservice.ubi.com/AuthenticationServiceSSL.svc</VALUE></GatorAuthServiceURI><GatorConfigServiceURI>" +
            "<VALUE>http://gatorservice.ubi.com/ConfigService.svc</VALUE></GatorConfigServiceURI><GatorContentServiceURI>" +
            "<VALUE>http://gatorservice.ubi.com/ContentService.svc</VALUE></GatorContentServiceURI><GatorServiceHost>" +
            "<VALUE>gatorservice.ubi.com</VALUE></GatorServiceHost><GatorServicePort><VALUE>80</VALUE>" +
            "</GatorServicePort><GatorTransferServiceURI><VALUE>/TransferService.svc</VALUE></GatorTransferServiceURI>" +
            "<GatorUserServiceURI><VALUE>http://gatorservice.ubi.com/GatorUserService.svc</VALUE></GatorUserServiceURI><NetworkPlatformServiceId>" +
            "<VALUE>UP0001-BLUS30536_00</VALUE></NetworkPlatformServiceId><SandboxUrlPS3><VALUE>prudp:/address=mdc-mm-rdv03.ubisoft.com;port=62110;serviceid=UP0001-BLUS30536_00</VALUE>" +
            "</SandboxUrlPS3><SandboxUrlWS><VALUE>mdc-mm-rdv03.ubisoft.com:62110</VALUE></SandboxUrlWS><uplay_DownloadServiceUrl>" +
            "<VALUE>http://secure.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url=</VALUE></uplay_DownloadServiceUrl>" +
            "<uplay_DynContentBaseUrl><VALUE>http://static8.cdn.ubi.com/u/Uplay/</VALUE></uplay_DynContentBaseUrl><uplay_DynContentSecureBaseUrl>" +
            "<VALUE>http://static8.cdn.ubi.com/</VALUE></uplay_DynContentSecureBaseUrl><uplay_PackageBaseUrl><VALUE>http://static8.cdn.ubi.com/u/Uplay/Packages/1.0.3-RC-Share/test6/</VALUE>" +
            "</uplay_PackageBaseUrl><uplay_WebServiceBaseUrl><VALUE>http://secure.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/</VALUE></uplay_WebServiceBaseUrl>" +
            "</RESPONSE>";

        public readonly static string DFS_PC_EN_XMLPayload = @"<RESPONSE xmlns="">
            <GatorAuthServiceActivity><VALUE>DRIVER5PC</VALUE></GatorAuthServiceActivity>
            <GatorAuthServiceHost><VALUE>secure.ubi.com</VALUE></GatorAuthServiceHost>
            <GatorAuthServicePort><VALUE>443</VALUE></GatorAuthServicePort>
            <GatorAuthServiceURI><VALUE>https://secure.ubi.com/gatorservice/AuthenticationServiceSSL.svc</VALUE></GatorAuthServiceURI>
            <GatorConfigServiceURI><VALUE>https://secure.ubi.com/gatorservice/ConfigService.svc</VALUE></GatorConfigServiceURI>
            <GatorContentServiceURI><VALUE>https://secure.ubi.com/gatorservice/ContentService.svc</VALUE></GatorContentServiceURI>
            <GatorServiceHost><VALUE>secure.ubi.com</VALUE></GatorServiceHost><GatorServicePort><VALUE>443</VALUE></GatorServicePort>
            <GatorTransferServiceURI><VALUE>http://gatorservice.ubi.com/TransferService.svc</VALUE></GatorTransferServiceURI>
            <GatorUserServiceURI><VALUE>https://secure.ubi.com/gatorservice/GatorUserService.svc</VALUE></GatorUserServiceURI>
            <SandboxUrl><VALUE>prudp:/address=mdc-mm-rdv01.ubisoft.com;port=61105</VALUE></SandboxUrl>
            <SandboxUrlWS><VALUE>mdc-mm-rdv01.ubisoft.com:61105</VALUE></SandboxUrlWS>
            <uplay_DownloadServiceUrl><VALUE>https://secure.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url=</VALUE></uplay_DownloadServiceUrl>
            <uplay_DynContentBaseUrl><VALUE>http://static8.cdn.ubi.com/u/Uplay/</VALUE></uplay_DynContentBaseUrl>
            <uplay_DynContentSecureBaseUrl><VALUE>http://static8.cdn.ubi.com/</VALUE></uplay_DynContentSecureBaseUrl>
            <uplay_PackageBaseUrl><VALUE>http://static8.cdn.ubi.com/u/Uplay/Packages/1.0.1/</VALUE></uplay_PackageBaseUrl>
            <uplay_WebServiceBaseUrl><VALUE>https://secure.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/</VALUE></uplay_WebServiceBaseUrl></RESPONSE>";

        public readonly static string PB_PS3_EN_XMLPayload = @"<RESPONSE xmlns="">
            <AuthenticationServer><VALUE>lb-lsg-auth.ubisoft.com:3078</VALUE></AuthenticationServer>
            <GatorAuthServiceActivity><VALUE>UGC_PUREFOOTBALL_PS3</VALUE></GatorAuthServiceActivity>
            <GatorAuthServiceHost><VALUE>gatorservice.ubi.com</VALUE></GatorAuthServiceHost>
            <GatorAuthServicePort><VALUE>443</VALUE></GatorAuthServicePort>
            <GatorAuthServiceURI><VALUE>https://secure.ubi.com/gatorservice/AuthenticationServiceSSL.svc</VALUE></GatorAuthServiceURI>
            <GatorConfigServiceURI><VALUE>https://secure.ubi.com/gatorservice/ConfigService.svc</VALUE></GatorConfigServiceURI>
            <GatorContentServiceURI><VALUE>https://secure.ubi.com/gatorservice/ContentService.svc</VALUE></GatorContentServiceURI>
            <GatorServiceHost><VALUE>gatorservice.ubi.com</VALUE></GatorServiceHost>
            <GatorServicePort><VALUE>80</VALUE></GatorServicePort>
            <GatorTransferServiceURI><VALUE>/TransferService.svc</VALUE></GatorTransferServiceURI>
            <GatorUserServiceURI><VALUE>https://secure.ubi.com/gatorservice/GatorUserService.svc</VALUE></GatorUserServiceURI>
            <LobbyServer><VALUE>lb-lsg-prod.ubisoft.com:3106</VALUE></LobbyServer><MmpTitleId><VALUE>0x2777</VALUE></MmpTitleId>
            <NetworkPlatformCommunicationId><VALUE>NPWR00657_00</VALUE></NetworkPlatformCommunicationId>
            <NetworkPlatformServiceId><VALUE>UP0001-BLUS30411_00</VALUE></NetworkPlatformServiceId>
            <SandboxUrlPS3><VALUE>prudp:/address=lb-rdv-prod02.ubisoft.com;port=25000;serviceid=UP0001-BLUS30411_00</VALUE></SandboxUrlPS3>
            <SandboxUrlWS><VALUE>lb-rdv-prod02.ubisoft.com:25000</VALUE></SandboxUrlWS>
            <uplay_DownloadServiceUrl><VALUE>https://secure.ubi.com/UplayServices/UplayFacade/DownloadServicesRESTXML.svc/REST/XML/?url=</VALUE></uplay_DownloadServiceUrl>
            <uplay_DynContentBaseUrl><VALUE>http://static8.cdn.ubi.com/u/Uplay/</VALUE></uplay_DynContentBaseUrl>
            <uplay_DynContentSecureBaseUrl><VALUE>http://static8.ubi.com/</VALUE></uplay_DynContentSecureBaseUrl>
            <uplay_PackageBaseUrl><VALUE>http://static8.cdn.ubi.com/u/Uplay/Packages/1.0.1/</VALUE></uplay_PackageBaseUrl>
            <uplay_WebServiceBaseUrl><VALUE>https://secure.ubi.com/UplayServices/UplayFacade/ProfileServicesFacadeRESTXML.svc/REST/</VALUE></uplay_WebServiceBaseUrl></RESPONSE>";
    }
}
