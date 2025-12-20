using System.Collections.Generic;

namespace WebAPIService.GameServices.UBISOFT.Models.Hermes.v2
{
    public class ClubService
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class Configuration
    {
        public Custom custom { get; set; }
        public List<FeaturesSwitch> featuresSwitches { get; set; }
        public List<GatewayResource> gatewayResources { get; set; }
        public Storm storm { get; set; }
        public SdkConfig sdkConfig { get; set; }
        public List<ClubService> clubServices { get; set; }
        public PlatformConfig platformConfig { get; set; }
        public List<LegacyUrl> legacyUrls { get; set; }
        public List<Sandbox> sandboxes { get; set; }
        public Events events { get; set; }
        public Punch punch { get; set; }
        public List<UplayService> uplayServices { get; set; }
    }

    public class Custom
    {
        public List<object> resources { get; set; }
        public List<object> featuresSwitches { get; set; }
    }

    public class Events
    {
        public List<Queue> queues { get; set; }
        public List<Tag> tags { get; set; }
    }

    public class FeaturesSwitch
    {
        public string name { get; set; }
        public bool? value { get; set; }
    }

    public class GatewayResource
    {
        public string url { get; set; }
        public string name { get; set; }
        public int? version { get; set; }
    }

    public class HttpParam
    {
        public TimeoutParam timeoutParam { get; set; }
    }

    public class LegacyUrl
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class PlatformConfig
    {
        public string uplayGameCode { get; set; }
        public string applicationId { get; set; }
        public string spaceId { get; set; }
        public string platform { get; set; }
        public string environment { get; set; }
    }

    public class Punch
    {
        public string detectUrl1 { get; set; }
        public string detectUrl2 { get; set; }
        public string traversalUrl { get; set; }
    }

    public class Queue
    {
        public string name { get; set; }
        public int? sendPeriodMilliseconds { get; set; }
    }

    public class RemoteLogs
    {
        public string ubiservicesLogLevel { get; set; }
        public string prodLogLevel { get; set; }
    }

    public class Root
    {
        public Configuration configuration { get; set; }
    }

    public class Sandbox
    {
        public string name { get; set; }
        public string friendlyName { get; set; }
        public string url { get; set; }
    }

    public class SdkConfig
    {
        public RemoteLogs remoteLogs { get; set; }
        public int? httpSafetySleepTime { get; set; }
        public int? keepAliveTimeoutMin { get; set; }
        public int? timeoutSec { get; set; }
        public HttpParam httpParam { get; set; }
        public WebsocketParam websocketParam { get; set; }
    }

    public class Storm
    {
        public string detectUrl1 { get; set; }
        public string detectUrl2 { get; set; }
        public string traversalUrl { get; set; }
    }

    public class Tag
    {
        public string name { get; set; }
        public bool? value { get; set; }
    }

    public class TimeoutParam
    {
        public int? initialDelayMsec { get; set; }
    }

    public class UplayService
    {
        public string url { get; set; }
        public string name { get; set; }
    }

    public class WebsocketParam
    {
        public TimeoutParam timeoutParam { get; set; }
    }
}
