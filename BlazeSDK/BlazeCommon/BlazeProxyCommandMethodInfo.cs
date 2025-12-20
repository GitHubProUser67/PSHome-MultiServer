using System.Reflection;

namespace BlazeCommon
{
    public class BlazeProxyCommandMethodInfo
    {
        public BlazeProxyCommandMethodInfo(IBlazeProxyComponent component, ushort commandId, Type requestType, Type responseType, MethodInfo commandMethod)
        {
            Component = component;
            Id = commandId;
            Name = Component.GetCommandName(commandId);
            RequestType = requestType;
            ResponseType = responseType;
            Method = commandMethod;
        }

        public IBlazeProxyComponent Component { get; }
        public ushort Id { get; }
        public string Name { get; }
        public Type RequestType { get; }
        public Type ResponseType { get; }
        public MethodInfo Method { get; }

        public async Task<object> InvokeAsync(object request, BlazeProxyContext context)
        {
            var task = (Task)Method.Invoke(Component, new object?[] { request, context })!;
            await task;
            return ((dynamic)task).Result;
        }
    }
}
