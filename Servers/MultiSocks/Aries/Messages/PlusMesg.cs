namespace MultiSocks.Aries.Messages
{
    public class PlusMesg : AbstractMessage
    {
        public override string _Name { get => "+msg"; }

        public string? F { get; set; }
        public string? T { get; set; }
        public string? N { get; set; }
    }
}
