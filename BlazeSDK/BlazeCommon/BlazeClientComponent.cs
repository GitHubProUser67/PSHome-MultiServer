using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BlazeCommon
{
    public abstract class BlazeClientComponent<CommandEnum, NotificationEnum, ErrorEnum> : BlazeComponent<CommandEnum, NotificationEnum, ErrorEnum>, IBlazeClientComponent where CommandEnum : Enum where NotificationEnum : Enum where ErrorEnum : Enum
    {
        Dictionary<ushort, BlazeClientNotificationMethodInfo> _clientNotifications;
        public BlazeClientComponent(ushort componentId, string componentName) : base(componentId, componentName)
        {
            InitializeComponent();
        }

        [MemberNotNull(nameof(_clientNotifications))]
        void InitializeComponent()
        {
            _clientNotifications = new Dictionary<ushort, BlazeClientNotificationMethodInfo>();

            Type componentType = GetType();

            MethodInfo[] methods = componentType.GetMethods();

            foreach (MethodInfo method in methods)
            {

                BlazeNotification? notificationAttr = method.GetCustomAttribute<BlazeNotification>();
                if (notificationAttr != null)
                {
                    AddNotification(method, notificationAttr);
                    continue;
                }
            }
        }

        bool AddNotification(MethodInfo method, BlazeNotification notificationAttribute)
        {
            ushort notificationId = notificationAttribute.Id;
            if (_clientNotifications.ContainsKey(notificationId))
                throw new InvalidOperationException($"Blaze notification {notificationId} seen more than once for component {Id}");

            Type fullReturnType = method.ReturnType;
            //we need to check if it is Task
            if (fullReturnType != typeof(Task))
                return false;

            Type[] parameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
            if (parameterTypes.Length != 1)
                return false;

            Type notificationType = parameterTypes[0];

            BlazeClientNotificationMethodInfo notificationInfo = new BlazeClientNotificationMethodInfo(this, notificationId, notificationType, method);
            _clientNotifications.Add(notificationId, notificationInfo);
            return true;
        }

        public BlazeClientNotificationMethodInfo? GetBlazeNotificationInfo(ushort notificationId)
        {
            _clientNotifications.TryGetValue(notificationId, out BlazeClientNotificationMethodInfo? notificationInfo);
            return notificationInfo;
        }
    }
}
