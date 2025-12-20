using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.RDVServices.RMC;

namespace QuazalServer.RDVServices.GameServices.PS3UbisoftServices
{
    [RMCService((ushort)RMCProtocolId.MissionService)]
    public class MissionService : RMCServiceBase
    {
        [RMCMethod(3)]
        public RMCResult GetPersonaMissions()
        {
            return Result(new GetPersonaMissions());
        }

        [RMCMethod(4)]
        public RMCResult GetAllMissionTemplate()
        {
            return Result(new GetAllMissionTemplate());
        }

        [RMCMethod(7)]
        public RMCResult CancelMission()
        {
            return Result(new CancelMission());
        }
    }
}
