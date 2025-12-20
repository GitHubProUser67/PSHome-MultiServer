namespace MultiSocks.Aries.Messages
{
    public class PresOut : AbstractMessage
    {
        public override string _Name { get => "PRES"; }
        public string? SHOW { get; set; }
        public string? CHNG { get; set; } = "1";
        public string? TITL { get; set; }
        public string? PROD { get; set; }
        public string? EXTR { get; set; }
        public string? STAT { get; set; }
        public string? ATTR { get; set; }
        public string? P { get; set;}
        public string? en { get; set; }
    }
}
