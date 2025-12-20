namespace MultiSocks.Aries.Messages
{
    public class Lggr : AbstractMessage
    {
        public override string _Name { get => "lggr"; }

        public string? STR { get; set; }
    }
}
