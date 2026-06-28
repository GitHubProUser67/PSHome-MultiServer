using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace BlazeCommon
{
    public abstract class BlazeServerComponent<CommandEnum, NotificationEnum, ErrorEnum>
        : BlazeComponent<CommandEnum, NotificationEnum, ErrorEnum>,
            IBlazeServerComponent
        where CommandEnum : Enum
        where NotificationEnum : Enum
        where ErrorEnum : Enum
    {
        Dictionary<ushort, BlazeServerCommandMethodInfo> _serverCommands;

        public BlazeServerComponent(ushort componentId, string componentName)
            : base(componentId, componentName)
        {
            InitializeComponent();
        }

        [MemberNotNull(nameof(_serverCommands))]
        void InitializeComponent()
        {
            _serverCommands = [];

            var componentType = GetType();

            var methods = componentType.GetMethods();

            foreach (var method in methods)
            {
                var commandAttr = method.GetCustomAttribute<BlazeCommand>();
                if (commandAttr != null)
                {
                    AddCommand(method, commandAttr);
                    continue;
                }
            }
        }

        bool AddCommand(MethodInfo method, BlazeCommand commandAttribute)
        {
            var commandId = commandAttribute.Id;
            if (_serverCommands.ContainsKey(commandId))
                throw new InvalidOperationException(
                    $"Blaze command {commandId} seen more than once for component {Id}"
                );

            var fullReturnType = method.ReturnType;
            //we need to check if it is Task<Response>
            if (!fullReturnType.IsGenericType)
                return false;
            if (fullReturnType.GetGenericTypeDefinition() != typeof(Task<>))
                return false;

            var responseType = fullReturnType.GetGenericArguments()[0];

            Type[] parameterTypes = [.. method.GetParameters().Select(x => x.ParameterType)];
            if (parameterTypes.Length != 2)
                return false;

            if (parameterTypes[1] != typeof(BlazeRpcContext))
                return false;

            var requestType = parameterTypes[0];

            var commandInfo = new BlazeServerCommandMethodInfo(
                this,
                commandId,
                requestType,
                responseType,
                method
            );
            _serverCommands.Add(commandId, commandInfo);
            return true;
        }

        public BlazeServerCommandMethodInfo? GetBlazeCommandInfo(ushort commandId)
        {
            _serverCommands.TryGetValue(commandId, out var commandInfo);
            return commandInfo;
        }
    }
}
