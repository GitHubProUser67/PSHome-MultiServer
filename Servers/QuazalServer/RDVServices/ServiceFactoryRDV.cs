using System.Reflection;
using QuazalServer.QNetZ.Factory;

namespace QuazalServer.RDVServices
{
    public static class ServiceFactoryRDV
    {
        private static readonly Dictionary<string, RMCServiceFactory> factoryList = new();

        public static void RegisterRDVServices(
            this RMCServiceFactory factory,
            string namespaceString
        )
        {
            foreach (
                var protoClass in Assembly
                    .GetExecutingAssembly()
                    .GetTypes()
                    .Where(t =>
                        string.Equals(
                            t.Namespace,
                            $"QuazalServer.RDVServices.GameServices.{namespaceString}",
                            StringComparison.Ordinal
                        )
                    )
                    .ToArray()
            )
            {
                factory.RegisterService(protoClass, namespaceString);
            }
        }

        public static RMCServiceFactory? TryGetServiceFactory(string namespaceString)
        {
            if (string.IsNullOrEmpty(namespaceString))
                return null;

            lock (factoryList)
            {
                if (factoryList.TryGetValue(namespaceString, out var value))
                    return value;
            }

            return null;
        }

        public static void TryInsertFactory(string namespaceString)
        {
            if (string.IsNullOrEmpty(namespaceString))
                return;

            RMCServiceFactory factory = new();

            lock (factoryList)
            {
                if (factoryList.TryAdd(namespaceString, factory))
                    RegisterRDVServices(factory, namespaceString);
            }
        }

        public static void TryRemoveFactory(string namespaceString)
        {
            if (string.IsNullOrEmpty(namespaceString))
                return;

            lock (factoryList)
            {
                factoryList.Remove(namespaceString);
            }
        }

        public static void ClearServices()
        {
            lock (factoryList)
                factoryList.Clear();
        }
    }
}
