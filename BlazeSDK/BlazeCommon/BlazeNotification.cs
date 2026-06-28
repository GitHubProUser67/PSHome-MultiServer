namespace BlazeCommon
{
    public class BlazeNotification(ushort commandId) : Attribute
    {
        public ushort Id { get; } = commandId;
    }
}
