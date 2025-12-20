namespace Horizon.MUM.Models
{
    public class Matchmaking
    {

    }

    public class MediusMatchRosterInfo
    {
        public int NumParties { get; set; }
        public int Parties { get; set; }
    }

    public class MediusMatchPartyInfo
    {
        public int NumPlayers { get; set; }
        public int[]? Players { get; set; }
    }
}