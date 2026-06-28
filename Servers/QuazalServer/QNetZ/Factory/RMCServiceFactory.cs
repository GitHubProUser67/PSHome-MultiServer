using System.Linq.Expressions;
using System.Reflection;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;

namespace QuazalServer.QNetZ.Factory
{
    public class RMCServiceFactory
    {
        private readonly Dictionary<ushort, Func<RMCServiceBase>> s_FactoryFuncs = new();

        public void RegisterService<T>(string namespaceString)
            where T : RMCServiceBase
        {
            RegisterService(typeof(T), namespaceString);
        }

        public void RegisterService(Type serviceType, string namespaceString)
        {
            var serviceAttribute = serviceType.GetCustomAttribute<RMCServiceAttribute>();

            if (serviceAttribute == null)
                return; //throw new Exception($"Service type '{ serviceType.Name }' missing 'RMCService' attribute!");

            if (s_FactoryFuncs.ContainsKey(serviceAttribute.ProtocolId))
            {
                CustomLogger.LoggerAccessor.LogError(
                    $"[RMCServiceFactory] - {namespaceString} '{serviceType.Name}' is already registered at protocol number {serviceAttribute.ProtocolId}!"
                );
                return;
            }

            s_FactoryFuncs.Add(
                serviceAttribute.ProtocolId,
                Expression
                    .Lambda<Func<RMCServiceBase>>(
                        Expression.New(serviceType.GetConstructor(Type.EmptyTypes))
                    )
                    .Compile()
            );
        }

        public Func<RMCServiceBase>? GetServiceFactory(ushort protocolId, string namespaceString)
        {
            if (s_FactoryFuncs.TryGetValue(protocolId, out var existingFactory))
                return existingFactory;

            // search for new controller
            foreach (
                var protoClass in Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t =>
                        string.Equals(
                            t.Namespace,
                            $"QuazalServer.RDVServices.{namespaceString}",
                            StringComparison.Ordinal
                        )
                    )
                    .ToArray()
            )
            {
                var protocolAttribute = protoClass.GetCustomAttribute<RMCServiceAttribute>();

                if (protocolAttribute == null)
                    continue;

                if (protocolAttribute.ProtocolId == protocolId)
                {
                    var createFunc = Expression
                        .Lambda<Func<RMCServiceBase>>(
                            Expression.New(protoClass.GetConstructor(Type.EmptyTypes))
                        )
                        .Compile();

                    s_FactoryFuncs.Add(protocolId, createFunc);

                    return createFunc;
                }
            }

            return null;
        }

        public Type? GetServiceType(ushort protocolId, string namespaceString)
        {
            var factoryFunc = GetServiceFactory(protocolId, namespaceString);
            return factoryFunc == null ? null : factoryFunc.Method.ReturnType;
        }

        public static MethodInfo? GetServiceMethodById(Type rmcServiceType, uint methodId)
        {
            MethodInfo? bestMethod = null;

            // find suitable method which DO have attribute with ID
            var allMethods = rmcServiceType.GetMethods();
            foreach (var method in allMethods)
            {
                var rmcMethodAttr = (RMCMethodAttribute?)
                    method.GetCustomAttributes(typeof(RMCMethodAttribute), true).SingleOrDefault();
                if (rmcMethodAttr != null)
                {
                    if (rmcMethodAttr.MethodId == methodId)
                    {
                        bestMethod = method;
                        break;
                    }
                }
            }

            return bestMethod;
        }
    }
}
