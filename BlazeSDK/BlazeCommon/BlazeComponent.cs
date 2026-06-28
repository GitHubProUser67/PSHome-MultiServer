namespace BlazeCommon
{
    public abstract class BlazeComponent<CommandEnum, NotificationEnum, ErrorEnum>(
        ushort componentId,
        string componentName
    ) : IBlazeComponent
        where CommandEnum : Enum
        where NotificationEnum : Enum
        where ErrorEnum : Enum
    {
        public ushort Id { get; } = componentId;
        public string Name { get; } = componentName;

        public string GetCommandName(CommandEnum command) => command.ToString();

        public string GetCommandName(ushort commandId) =>
            GetCommandName((CommandEnum)Enum.ToObject(typeof(CommandEnum), commandId));

        public string GetNotificationName(NotificationEnum notification) => notification.ToString();

        public string GetNotificationName(ushort notificationId) =>
            GetNotificationName(
                (NotificationEnum)Enum.ToObject(typeof(NotificationEnum), notificationId)
            );

        public string GetErrorName(ErrorEnum error) => error.ToString();

        public string GetErrorName(int fullErrorCode) =>
            GetErrorName((ErrorEnum)Enum.ToObject(typeof(ErrorEnum), fullErrorCode));

        public string GetErrorName(ushort shortErrorCode) => throw new NotImplementedException();

        public abstract Type GetCommandRequestType(CommandEnum command);
        public abstract Type GetCommandResponseType(CommandEnum command);
        public abstract Type GetCommandErrorResponseType(CommandEnum command);
        public abstract Type GetNotificationType(NotificationEnum notification);

        public Type GetCommandRequestType(ushort commandId) =>
            GetCommandRequestType((CommandEnum)Enum.ToObject(typeof(CommandEnum), commandId));

        public Type GetCommandResponseType(ushort commandId) =>
            GetCommandResponseType((CommandEnum)Enum.ToObject(typeof(CommandEnum), commandId));

        public Type GetCommandErrorResponseType(ushort commandId) =>
            GetCommandErrorResponseType((CommandEnum)Enum.ToObject(typeof(CommandEnum), commandId));

        public Type GetNotificationType(ushort notificationId) =>
            GetNotificationType(
                (NotificationEnum)Enum.ToObject(typeof(NotificationEnum), notificationId)
            );

        public string GetFullName(FireFrame frame)
        {
            if (frame.Component != Id)
                throw new ArgumentException(
                    $"Frame component {frame.Component} does not match this component {Id}"
                );
            var commandOrNotificationName = frame.MsgType switch
            {
                FireFrame.MessageType.MESSAGE
                or FireFrame.MessageType.REPLY
                or FireFrame.MessageType.ERROR_REPLY => GetCommandName(frame.Command),
                FireFrame.MessageType.NOTIFICATION => GetNotificationName(frame.Command),
                _ => frame.Command.ToString(),
            };
            return $"{Name}::{commandOrNotificationName}";
        }
    }
}
