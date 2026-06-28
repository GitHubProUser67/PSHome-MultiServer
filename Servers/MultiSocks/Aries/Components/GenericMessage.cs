namespace MultiSocks.Aries.Components
{
    public class GenericMessage : AbstractMessage
    {
        public override string _Name { get; }

        // Constructor to initialize _Name
        public GenericMessage(string MsgName)
        {
            _Name = MsgName;
        }
    }
}
