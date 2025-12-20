using System;

namespace WebAPIService.GameServices.UBISOFT.Models.Hermes.v2
{
    public class V2SessionsResponse
    {
        public object accountIssues { get; set; }
        public string clientIp { get; set; }
        public string clientIpCountry { get; set; }
        public string environment { get; set; } = "Prod";
        public string expiration { get; set; } = DateTime.UtcNow.AddHours(3).ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");
        public bool hasAcceptedLegalOptins { get; set; } = true;
        public bool initializeUser { get; set; } = true;
        public string nameOnPlatform { get; set; }
        public string platformType { get; set; }
        public string profileId { get; set; }
        public object rememberMeTicket { get; set; }
        public string serverTime { get; set; } = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffffffZ");
        public string sessionId { get; set; }
        public string spaceId { get; set; }
        public string ticket { get; set; }
        public string token { get; set; }
        public object twoFactorAuthenticationTicket { get; set; }
        public string userId { get; set; }
        public string username { get; set; }
    }
}
