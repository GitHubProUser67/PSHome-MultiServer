using System.Reflection;

namespace BlazeCommon
{
    public class BlazeClientNotificationMethodInfo
    {
        public BlazeClientNotificationMethodInfo(IBlazeClientComponent component, ushort notificationId, Type notificationType, MethodInfo notificationMethod)
        {
            Component = component;
            Id = notificationId;
            Name = Component.GetNotificationName(notificationId);
            NotificationType = notificationType;
            Method = notificationMethod;
        }

        public IBlazeClientComponent Component { get; }
        public ushort Id { get; }
        public string Name { get; }
        public Type NotificationType { get; }
        public MethodInfo Method { get; }

        public Task InvokeAsync(object notification)
        {
            return (Task)Method.Invoke(Component, new object?[] { notification })!;
        }
    }
}
