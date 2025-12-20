namespace BlazeCommon
{
    public class BlazeNotification : Attribute
    {
        public ushort Id { get; }
        public BlazeNotification(ushort commandId)
        {
            Id = commandId;
        }
    }
}
