using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BlazeCommon
{
    public abstract class BlazeClientComponent<CommandEnum, NotificationEnum, ErrorEnum>
        : BlazeComponent<CommandEnum, NotificationEnum, ErrorEnum>,
            IBlazeClientComponent
        where CommandEnum : Enum
        where NotificationEnum : Enum
        where ErrorEnum : Enum
    {
        Dictionary<ushort, BlazeClientNotificationMethodInfo> _clientNotifications;

        public BlazeClientComponent(ushort componentId, string componentName)
            : base(componentId, componentName)
        {
            InitializeComponent();
        }

        [MemberNotNull(nameof(_clientNotifications))]
        void InitializeComponent()
        {
            _clientNotifications = [];

            var componentType = GetType();

            var methods = componentType.GetMethods();

            foreach (var method in methods)
            {
                var notificationAttr = method.GetCustomAttribute<BlazeNotification>();
                if (notificationAttr != null)
                {
                    AddNotification(method, notificationAttr);
                    continue;
                }
            }
        }

        bool AddNotification(MethodInfo method, BlazeNotification notificationAttribute)
        {
            var notificationId = notificationAttribute.Id;
            if (_clientNotifications.ContainsKey(notificationId))
                throw new InvalidOperationException(
                    $"Blaze notification {notificationId} seen more than once for component {Id}"
                );

            var fullReturnType = method.ReturnType;
            //we need to check if it is Task
            if (fullReturnType != typeof(Task))
                return false;

            Type[] parameterTypes = [.. method.GetParameters().Select(x => x.ParameterType)];
            if (parameterTypes.Length != 1)
                return false;

            var notificationType = parameterTypes[0];

            var notificationInfo = new BlazeClientNotificationMethodInfo(
                this,
                notificationId,
                notificationType,
                method
            );
            _clientNotifications.Add(notificationId, notificationInfo);
            return true;
        }

        public BlazeClientNotificationMethodInfo? GetBlazeNotificationInfo(ushort notificationId)
        {
            _clientNotifications.TryGetValue(notificationId, out var notificationInfo);
            return notificationInfo;
        }
    }
}
