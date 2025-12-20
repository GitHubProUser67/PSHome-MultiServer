using QuazalServer.RDVServices.MissionClass;

namespace QuazalServer.RDVServices.DDL.Models
{
    public class GetAllMissionTemplate
    {
        public List<Mission> missions = new ();
        public List<MissionArc> missionArcs = new();
        public List<MissionSequence> missionSeqs = new();
    }
}
