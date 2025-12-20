namespace MultiSocks.Aries.Messages
{
    public class Slst : AbstractMessage
    {
        public override string _Name { get => "slst"; }

        public string COUNT { get; set; } = "27";
        public string VIEW0 { get; set; } = "lobby,\"Online Lobby Stats View\"";
        public string VIEW1 { get; set; } = "DLC,\"DLC Lobby Stats View\"";
        public string VIEW2 { get; set; } = "RoadRules,\"Road Rules\"";
        public string VIEW3 { get; set; } = "DayBikeRRs,\"Day Bike Road Rules\"";
        public string VIEW4 { get; set; } = "NightBikeRR,\"Night Bike Road Rules\"";
        public string VIEW5 { get; set; } = "PlayerStatS,\"Player Stats Summary\"";
        public string VIEW6 { get; set; } = "LastEvent1,\"Recent Event 1 Details\"";
        public string VIEW7 { get; set; } = "LastEvent2,\"Recent Event 2 Details\"";
        public string VIEW8 { get; set; } = "LastEvent3,\"Recent Event 3 Details\"";
        public string VIEW9 { get; set; } = "LastEvent4,\"Recent Event 4 Details\"";
        public string VIEW10 { get; set; } = "LastEvent5,\"Recent Event 5 Details\"";
        public string VIEW11 { get; set; } = "OfflineProg,\"Offline Progression\"";
        public string VIEW12 { get; set; } = "Rival1,\"Rival 1 information\"";
        public string VIEW13 { get; set; } = "Rival2,\"Rival 2 information\"";
        public string VIEW14 { get; set; } = "Rival3,\"Rival 3 information\"";
        public string VIEW15 { get; set; } = "Rival4,\"Rival 4 information\"";
        public string VIEW16 { get; set; } = "Rival5,\"Rival 5 information\"";
        public string VIEW17 { get; set; } = "Rival6,\"Rival 6 information\"";
        public string VIEW18 { get; set; } = "Rival7,\"Rival 7 information\"";
        public string VIEW19 { get; set; } = "Rival8,\"Rival 8 information\"";
        public string VIEW20 { get; set; } = "Rival9,\"Rival 9 information\"";
        public string VIEW21 { get; set; } = "Rival10,\"Rival 10 information\"";
        public string VIEW22 { get; set; } = "DriverDetai,\"Driver details\"";
        public string VIEW23 { get; set; } = "RiderDetail,\"Rider details\"";
        public string VIEW24 { get; set; } = "IsldDetails,\"Island details\"";
        public string VIEW25 { get; set; } = "Friends,\"Friends List\"";
        public string VIEW26 { get; set; } = "PNetworkSta,\"Paradise Network Stats\"";


        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            if (context is not MatchmakerServer) return;

            client.SendMessage(this);
        }
    }
}
