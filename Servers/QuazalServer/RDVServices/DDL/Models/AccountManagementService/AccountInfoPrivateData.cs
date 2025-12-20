namespace QuazalServer.RDVServices.DDL.Models
{
    public class AccountInfoPrivateData
    {
        public float trialPlayTimeHours { get; set; }
        public DateTime paidPlayTime { get; set; }
        public string? macAddress { get; set; }
        public string? version { get; set; }
    }
}
