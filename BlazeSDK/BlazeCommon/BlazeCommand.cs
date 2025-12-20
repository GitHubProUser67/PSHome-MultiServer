namespace BlazeCommon
{
    public class BlazeCommand : Attribute
    {
        public ushort Id { get; }
        public BlazeCommand(ushort commandId)
        {
            Id = commandId;
        }
    }
}
