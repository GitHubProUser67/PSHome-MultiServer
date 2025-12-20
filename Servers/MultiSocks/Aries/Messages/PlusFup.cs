namespace MultiSocks.Aries.Messages
{
    public class PlusFup : AbstractMessage
    {
        public override string _Name { get => "+fup"; }
        public string FLUP { get; set; } = "0";
        public string PRES { get; set; } = string.Empty;
    }
}
