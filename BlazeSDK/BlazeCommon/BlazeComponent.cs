namespace BlazeCommon
{
    public abstract class BlazeComponent<CommandEnum, NotificationEnum, ErrorEnum> : IBlazeComponent where CommandEnum : Enum where NotificationEnum : Enum where ErrorEnum : Enum
    {
        public ushort Id { get; }
        public string Name { get; }

        public BlazeComponent(ushort componentId, string componentName)
        {
            Id = componentId;
            Name = componentName;
        }

        public string GetCommandName(CommandEnum command) => command.ToString();
        public string GetCommandName(ushort commandId) => GetCommandName((CommandEnum)Enum.ToObject(typeof(CommandEnum), commandId));
        public string GetNotificationName(NotificationEnum notification) => notification.ToString();
        public string GetNotificationName(ushort notificationId) => GetNotificationName((NotificationEnum)Enum.ToObject(typeof(NotificationEnum), notificationId));
        public string GetErrorName(ErrorEnum error) => error.ToString();
        public string GetErrorName(int fullErrorCode) => GetErrorName((ErrorEnum)Enum.ToObject(typeof(ErrorEnum), fullErrorCode));
        public string GetErrorName(ushort shortErrorCode) => throw new NotImplementedException();

        public abstract Type GetCommandRequestType(CommandEnum command);
        public abstract Type GetCommandResponseType(CommandEnum command);
        public abstract Type GetCommandErrorResponseType(CommandEnum command);
        public abstract Type GetNotificationType(NotificationEnum notification);

        public Type GetCommandRequestType(ushort commandId) => GetCommandRequestType((CommandEnum)Enum.ToObject(typeof(CommandEnum), commandId));
        public Type GetCommandResponseType(ushort commandId) => GetCommandResponseType((CommandEnum)Enum.ToObject(typeof(CommandEnum), commandId));
        public Type GetCommandErrorResponseType(ushort commandId) => GetCommandErrorResponseType((CommandEnum)Enum.ToObject(typeof(CommandEnum), commandId));
        public Type GetNotificationType(ushort notificationId) => GetNotificationType((NotificationEnum)Enum.ToObject(typeof(NotificationEnum), notificationId));

        public string GetFullName(FireFrame frame)
        {
            if (frame.Component != Id)
                throw new ArgumentException($"Frame component {frame.Component} does not match this component {Id}");

            string commandOrNotificationName;
            switch (frame.MsgType)
            {
                case FireFrame.MessageType.MESSAGE:
                case FireFrame.MessageType.REPLY:
                case FireFrame.MessageType.ERROR_REPLY:
                    commandOrNotificationName = GetCommandName(frame.Command);
                    break;
                case FireFrame.MessageType.NOTIFICATION:
                    commandOrNotificationName = GetNotificationName(frame.Command);
                    break;
                default:
                    commandOrNotificationName = frame.Command.ToString();
                    break;
            }

            return $"{Name}::{commandOrNotificationName}";
        }

    }
}
