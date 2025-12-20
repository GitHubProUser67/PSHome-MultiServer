using System.Reflection;

namespace BlazeCommon
{
    public class BlazeProxyNotificationMethodInfo
    {
        public BlazeProxyNotificationMethodInfo(IBlazeProxyComponent component, ushort notificationId, Type notificationType, MethodInfo notificationMethod)
        {
            Component = component;
            Id = notificationId;
            Name = Component.GetNotificationName(notificationId);
            NotificationType = notificationType;
            Method = notificationMethod;
        }

        public IBlazeProxyComponent Component { get; }
        public ushort Id { get; }
        public string Name { get; }
        public Type NotificationType { get; }
        public MethodInfo Method { get; }

        public async Task<object> InvokeAsync(object notification)
        {
            var task = (Task)Method.Invoke(Component, new object?[] { notification })!;
            await task;
            return ((dynamic)task).Result;
        }
    }
}
