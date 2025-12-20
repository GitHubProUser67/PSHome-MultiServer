using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.RDVServices.GameServices.PS3DriverServices
{
    [RMCService((ushort)RMCProtocolId.GameSessionExService)]
    public class GameSessionExService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult SearchSessions(GameSessionQuery game_session_query)
        {
            // TODO, truly implement this function.
            return Result(new { search_results = new List<GameSessionSearchResultEx>() });
        }
    }
}
