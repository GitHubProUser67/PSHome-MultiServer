namespace QuazalServer.RDVServices.DDL.Models
{
    public class GameSessionSearchResultEx
    {
        public GameSessionSearchResult? m_base { get; set; }
        public ICollection<GameSessionParticipant>? m_participants { get; set; }
    }
}
