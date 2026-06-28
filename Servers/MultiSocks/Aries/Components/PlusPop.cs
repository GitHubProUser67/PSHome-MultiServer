namespace MultiSocks.Aries.Components
{
    public class PlusPop : AbstractMessage
    {
        public override string _Name
        {
            get => "+pop";
        }

        public string? Z { get; set; }
    }
}
