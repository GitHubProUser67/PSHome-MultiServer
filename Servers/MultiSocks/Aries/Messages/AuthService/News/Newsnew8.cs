namespace MultiSocks.Aries.Messages.AuthService.News
{
    public class Newsnew8 : AbstractMessage
    {
        public override string _Name { get => "newsnew8"; }
        public string? MIN_TIME_SPENT_SYNCYING_TIME { get; set; } = "1";
        public string? MAX_TIME_SPENT_SYNCYING_TIME { get; set; } = "30";
        public string? MAX_TIME_TO_WAIT_FOR_START_TIME { get; set; } = "30";
        public string? MAX_TIME_TO_WAIT_FOR_SILENT_CLIENT_READY { get; set; } = "30";
        public string? MAX_TIME_TO_WAIT_FOR_COMMUNICATING_CLIENT_READY { get; set; } = "45";
        public string? TIME_GAP_TO_LEAVE_BEFORE_START_TIME { get; set; } = "5";
        public string? IDLE_TIMEOUT { get; set; } = "30000";
        public string? SEARCH_QUERY_TIME_INTERVAL { get; set; } = "30000";
        public string? NAT_TEST_PACKET_TIMEOUT { get; set; } = "30000";
        public string? TOS_BUFFER_SIZE { get; set; } = "250000";
        public string? NEWS_BUFFER_SIZE { get; set; } = "85000";
        public string? LOG_OFF_ON_EXIT_ONLINE_MENU { get; set; } = "FALSE";
        public string? TELEMETRY_FILTERS_FIRST_USE { get; set; } = string.Empty;
        public string? TELEMETRY_FILTERS_NORMAL_USE { get; set; } = string.Empty;
        public string? TIME_BETWEEN_STATS_CHECKS { get; set; } = "30";
        public string? TIME_BETWEEN_ROAD_RULES_UPLOADS { get; set; } = "1";
        public string? TIME_BETWEEN_ROAD_RULES_DOWNLOADS { get; set; } = "900";
        public string? TIME_BEFORE_RETRY_AFTER_FAILED_BUDDY_UPLOAD { get; set; } = "600";
        public string? TIME_BETWEEN_OFFLINE_PROGRESSION_UPLOAD { get; set; } = "600";
    }
}
