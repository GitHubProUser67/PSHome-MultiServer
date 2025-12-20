namespace QuazalServer.RDVServices.DDL.Models
{
    public class TimedNotification
    {
        public DateTime m_timestamp { get; set; }
        public NotificationEvent? m_notification { get; set; }
    }
}
