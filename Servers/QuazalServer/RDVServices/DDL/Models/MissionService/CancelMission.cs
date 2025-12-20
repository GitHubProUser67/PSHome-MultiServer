namespace QuazalServer.RDVServices.DDL.Models
{
    public class CancelMission
    {
        public uint unused { get; set; } = 0x11;
        public uint missionId { get; set; } = 0;
    }
}
