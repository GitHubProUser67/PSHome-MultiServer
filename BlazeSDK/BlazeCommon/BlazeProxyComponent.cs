using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BlazeCommon
{
    public abstract class BlazeProxyComponent<CommandEnum, NotificationEnum, ErrorEnum> : BlazeComponent<CommandEnum, NotificationEnum, ErrorEnum>, IBlazeProxyComponent where CommandEnum : Enum where NotificationEnum : Enum where ErrorEnum : Enum
    {
        Dictionary<ushort, BlazeProxyCommandMethodInfo> _proxyCommands;
        Dictionary<ushort, BlazeProxyNotificationMethodInfo> _proxyNotifications;
        public BlazeProxyComponent(ushort componentId, string componentName) : base(componentId, componentName)
        {
            InitializeComponent();
        }

        [MemberNotNull(nameof(_proxyCommands), nameof(_proxyNotifications))]
        void InitializeComponent()
        {
            _proxyCommands = new Dictionary<ushort, BlazeProxyCommandMethodInfo>();
            _proxyNotifications = new Dictionary<ushort, BlazeProxyNotificationMethodInfo>();

            Type componentType = GetType();

            MethodInfo[] methods = componentType.GetMethods();

            foreach (MethodInfo method in methods)
            {
                BlazeCommand? commandAttr = method.GetCustomAttribute<BlazeCommand>();
                if (commandAttr != null)
                {
                    AddCommand(method, commandAttr);
                    continue;
                }

                BlazeNotification? notificationAttr = method.GetCustomAttribute<BlazeNotification>();
                if (notificationAttr != null)
                {
                    AddNotification(method, notificationAttr);
                    continue;
                }
            }
        }

        bool AddCommand(MethodInfo method, BlazeCommand commandAttribute)
        {
            ushort commandId = commandAttribute.Id;
            if (_proxyCommands.ContainsKey(commandId))
                throw new InvalidOperationException($"Blaze command {commandId} seen more than once for component {Id}");

            Type fullReturnType = method.ReturnType;
            //we need to check if it is Task<Response>
            if (!fullReturnType.IsGenericType)
                return false;
            if (fullReturnType.GetGenericTypeDefinition() != typeof(Task<>))
                return false;

            Type responseType = fullReturnType.GetGenericArguments()[0];

            Type[] parameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
            if (parameterTypes.Length != 2)
                return false;

            if (parameterTypes[1] != typeof(BlazeProxyContext))
                return false;

            Type requestType = parameterTypes[0];

            BlazeProxyCommandMethodInfo commandInfo = new BlazeProxyCommandMethodInfo(this, commandId, requestType, responseType, method);
            _proxyCommands.Add(commandId, commandInfo);
            return true;
        }

        bool AddNotification(MethodInfo method, BlazeNotification notificationAttribute)
        {
            ushort notificationId = notificationAttribute.Id;
            if (_proxyNotifications.ContainsKey(notificationId))
                throw new InvalidOperationException($"Blaze notification {notificationId} seen more than once for component {Id}");

            Type[] parameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
            if (parameterTypes.Length != 1)
                return false;

            Type notificationType = parameterTypes[0];

            Type fullReturnType = method.ReturnType;
            Type expectedType = typeof(Task<>).MakeGenericType(notificationType);
            if (fullReturnType != expectedType)
                return false;

            BlazeProxyNotificationMethodInfo notificationInfo = new BlazeProxyNotificationMethodInfo(this, notificationId, notificationType, method);
            _proxyNotifications.Add(notificationId, notificationInfo);
            return true;
        }

        public BlazeProxyCommandMethodInfo? GetBlazeCommandInfo(ushort commandId)
        {
            _proxyCommands.TryGetValue(commandId, out BlazeProxyCommandMethodInfo? commandInfo);
            return commandInfo;
        }

        public BlazeProxyNotificationMethodInfo? GetBlazeNotificationInfo(ushort notificationId)
        {
            _proxyNotifications.TryGetValue(notificationId, out BlazeProxyNotificationMethodInfo? notificationInfo);
            return notificationInfo;
        }
    }
}
