using QuazalServer.RDVServices.MissionClass;

namespace QuazalServer.RDVServices.DDL.Models
{
    public class GetPersonaMissions
    {
        public List<PersonaMission> missions = new();
        public uint missionSeed;
        public uint missionStartTime1; //can be 64-bit value, idk
        public uint missionStartTime2;

        public GetPersonaMissions()
        {
            missions.Add(new PersonaMission { });
        }
    }
}
