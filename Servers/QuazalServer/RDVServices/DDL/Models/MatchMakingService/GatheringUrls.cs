using QuazalServer.QNetZ.DDL;

namespace QuazalServer.RDVServices.DDL.Models.MatchMakingService
{
	// https://github.com/kinnay/NintendoClients/wiki/Match-Making-Types#gatheringurls-structure
    public class GatheringUrls
    {
        public uint gid { get; set; }
        public List<StationURL> lst_station_urls { get; set; }
    }
}
