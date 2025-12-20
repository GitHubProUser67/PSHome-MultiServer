namespace MultiSocks.Aries.Messages
{
    public class PlusRom : AbstractMessage
    {
        public override string _Name { get => "+rom"; }

        public string? I { get; set; }
        public string? N { get; set; }
        public string? H { get; set; } = "FreeSO";
        public string? F { get; set; } = "CK";
        public string? T { get; set; }
        public string? L { get; set; } = "4";
        public string? P { get; set; } = "0";
    }
}
