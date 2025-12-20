namespace MultiSocks.Aries.Messages
{
    public class ChalOut : AbstractMessage
    {
        public override string _Name { get => "chal"; }

        public string? MODE { get; set; }

    }
}
