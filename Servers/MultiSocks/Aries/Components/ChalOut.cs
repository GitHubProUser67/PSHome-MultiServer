namespace MultiSocks.Aries.Components
{
    public class ChalOut : AbstractMessage
    {
        public override string _Name
        {
            get => "chal";
        }

        public string? MODE { get; set; }
    }
}
