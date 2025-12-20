namespace BlazeCommon
{
    public interface IBlazeClientComponent : IBlazeComponent
    {
        BlazeClientNotificationMethodInfo? GetBlazeNotificationInfo(ushort notificationId);
    }
}
