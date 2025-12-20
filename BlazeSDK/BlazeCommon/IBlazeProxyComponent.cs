namespace BlazeCommon
{
    public interface IBlazeProxyComponent : IBlazeComponent
    {
        BlazeProxyCommandMethodInfo? GetBlazeCommandInfo(ushort commandId);
        BlazeProxyNotificationMethodInfo? GetBlazeNotificationInfo(ushort notificationId);
    }
}
