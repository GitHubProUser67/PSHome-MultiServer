namespace QuazalServer.RDVServices.DDL.Models
{
    public class TrackingInformation
    {
        public uint ipn { get; set; }
        public string? user_id { get; set; }
        public string? machine_id { get; set; }
        public string? visitor_id { get; set; }
        public string? uts_version { get; set; }
    }

    public class TrackingTag
    {
        public uint tracking_id { get; set; }
        public string? tag { get; set; }
        public string? attributes { get; set; }
        public uint delta_time { get; set; }
        public string? new_user_id { get; set; }
    }
}
