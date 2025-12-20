namespace BlazeCommon
{
    public interface IBlazeComponent
    {
        ushort Id { get; }
        string Name { get; }

        string GetCommandName(ushort commandId);
        Type GetCommandRequestType(ushort commandId);
        Type GetCommandResponseType(ushort commandId);
        Type GetCommandErrorResponseType(ushort commandId);

        string GetNotificationName(ushort notificationId);
        Type GetNotificationType(ushort notificationId);

        string GetErrorName(int fullErorCode);
        string GetErrorName(ushort shortErrorCode);
        string GetFullName(FireFrame frame);
    }
}
