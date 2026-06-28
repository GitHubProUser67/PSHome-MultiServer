namespace BlazeCommon
{
    public class BlazeCommand(ushort commandId) : Attribute
    {
        public ushort Id { get; } = commandId;
    }
}
