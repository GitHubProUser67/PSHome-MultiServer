using QuazalServer.QNetZ.DDL;
using QuazalServer.RDVServices.DDL.Models.MatchMakingService;

namespace QuazalServer.RDVServices.DDL.Models.SparkService
{
    public class SparkSession
    {
        public GatheringUrls URLs { get; set; }
        public AnyData<SparkGame> Game { get; set; }
        public HashSet<uint> Participants { get; set; }
    }
}
