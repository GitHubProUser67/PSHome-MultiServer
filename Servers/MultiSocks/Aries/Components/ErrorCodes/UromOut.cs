namespace MultiSocks.Aries.Components
{
    public class UromOut : AbstractMessage
    {
        public override string _Name { get; }

        // Constructor to initialize _Name
        public UromOut(string MsgName)
        {
            _Name = MsgName + "urom";
        }
    }
}
