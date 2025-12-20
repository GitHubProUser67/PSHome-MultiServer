using System.Reflection;

namespace BlazeCommon
{
    public class BlazeServerCommandMethodInfo
    {
        public BlazeServerCommandMethodInfo(IBlazeServerComponent component, ushort commandId, Type requestType, Type responseType, MethodInfo commandMethod)
        {
            Component = component;
            Id = commandId;
            Name = Component.GetCommandName(commandId);
            RequestType = requestType;
            ResponseType = responseType;
            Method = commandMethod;
        }

        public IBlazeServerComponent Component { get; }
        public ushort Id { get; }
        public string Name { get; }
        public Type RequestType { get; }
        public Type ResponseType { get; }
        public MethodInfo Method { get; }

        public async Task<object> InvokeAsync(object request, BlazeRpcContext context)
        {
            var task = (Task)Method.Invoke(Component, new object?[] { request, context })!;
            await task;
            return ((dynamic)task).Result;
        }
    }
}
