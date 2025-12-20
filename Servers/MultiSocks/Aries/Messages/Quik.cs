namespace MultiSocks.Aries.Messages
{
    public class Quik : AbstractMessage
    {
        public override string _Name { get => "quik"; }
        public string? KIND { get; set; }
        public string? QMFP { get; set; }
        public string? AUTO { get; set; }
        public string? VOIP { get; set; }
        public string? GAME { get; set; }
        public string? CHAR { get; set; }
        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(this);
            // For now I keep game create commented out until I know proper flow for game creation if no matches were found
            //client.SendMessage(new GcreOut());
        }
    }
}
