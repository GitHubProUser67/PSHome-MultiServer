using System.Security.Cryptography;
using System.Text;

namespace TechnitiumLibrary.Net.Firewall
{
    public class FirewallHelper
    {
        private static bool _hasGlobalRule = false;

        public static void CheckFirewallEntries(
            string appPath,
            Dictionary<int, Protocol>? ports = null
        )
        {
            if (string.IsNullOrWhiteSpace(appPath))
                return;

            if (appPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                appPath = appPath[..^4] + ".exe";

            RemoveFirewallRulesGlobal("MultiServer", appPath); // Removes the old unsecure rule.

            string serverName = Path.GetFileNameWithoutExtension(appPath);

            if (ports == null)
            {
                if (!WindowsFirewallGlobalEntryExists(serverName, appPath))
                    AddWindowsFirewallGlobalEntry(serverName, appPath);
            }
            else
            {
                foreach (var port in ports)
                {
                    if (!WindowsFirewallPortEntryExists(serverName, appPath, port.Key, port.Value))
                        AddWindowsFirewallPortEntry(serverName, appPath, port.Key, port.Value);
                }
            }
        }

        public static void CheckFirewallEntry(string appPath, int port, Protocol prot)
        {
            if (string.IsNullOrWhiteSpace(appPath) || _hasGlobalRule)
                return;

            if (appPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                appPath = appPath[..^4] + ".exe";

            string serverName = Path.GetFileNameWithoutExtension(appPath);

            if (!WindowsFirewallPortEntryExists(serverName, appPath, port, prot))
                AddWindowsFirewallPortEntry(serverName, appPath, port, prot);
        }

        public static bool RemoveFirewallEntry(string appPath, int port, Protocol prot)
        {
            if (string.IsNullOrWhiteSpace(appPath) || _hasGlobalRule)
                return false;

            if (appPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                appPath = appPath[..^4] + ".exe";

            return RemoveFirewallRulePort(
                Path.GetFileNameWithoutExtension(appPath),
                appPath,
                port,
                prot
            );
        }

        private static bool RemoveFirewallRulesGlobal(string serverName, string appPath)
        {
            try
            {
                while (
                    WindowsFirewall.RuleExistsVista(serverName, appPath) != RuleStatus.DoesNotExists
                )
                    WindowsFirewall.RemoveRuleVista(serverName, appPath);
                return true;
            }
            catch
            {
                // Not important
            }

            return false;
        }

        private static bool RemoveFirewallRulePort(
            string serverName,
            string appPath,
            int port,
            Protocol protocol
        )
        {
            try
            {
                string ruleName = GetRuleName(serverName, port, protocol);
                while (
                    WindowsFirewall.RuleExistsVista(ruleName, appPath) != RuleStatus.DoesNotExists
                )
                    WindowsFirewall.RemoveRuleVista(ruleName, appPath);
                return true;
            }
            catch
            {
                // Not important
            }

            return false;
        }

        private static bool WindowsFirewallGlobalEntryExists(string serverName, string appPath)
        {
            try
            {
                bool ruleIsSet =
                    WindowsFirewall.RuleExistsVista(serverName, appPath) == RuleStatus.Allowed;
                if (ruleIsSet)
                    _hasGlobalRule = true;
                return ruleIsSet;
            }
            catch
            {
                // Not Important.
            }

            return false;
        }

        private static bool WindowsFirewallPortEntryExists(
            string serverName,
            string appPath,
            int port,
            Protocol protocol
        )
        {
            try
            {
                return WindowsFirewall.RuleExistsVista(
                        GetRuleName(serverName, port, protocol),
                        appPath
                    ) == RuleStatus.Allowed;
            }
            catch
            {
                // Not Important.
            }

            return false;
        }

        private static bool AddWindowsFirewallGlobalEntry(string serverName, string appPath)
        {
            try
            {
                WindowsFirewall.AddRuleVista(
                    serverName,
                    "Allows incoming connection request to the server.",
                    FirewallAction.Allow,
                    appPath,
                    Protocol.ANY,
                    null,
                    null,
                    null,
                    null,
                    InterfaceTypeFlags.All,
                    true,
                    Direction.Inbound,
                    true
                );

                _hasGlobalRule = true;

                return true;
            }
            catch
            {
                // Not Important.
            }

            return false;
        }

        private static bool AddWindowsFirewallPortEntry(
            string serverName,
            string appPath,
            int port,
            Protocol protocol
        )
        {
            try
            {
                WindowsFirewall.AddRuleVista(
                    GetRuleName(serverName, port, protocol),
                    $"Allows incoming {protocol} connections on port {port}.",
                    FirewallAction.Allow,
                    appPath,
                    protocol,
                    port.ToString(),
                    null,
                    null,
                    null,
                    InterfaceTypeFlags.All,
                    true,
                    Direction.Inbound,
                    true
                );

                return true;
            }
            catch
            {
                // Not Important.
            }

            return false;
        }

        private static string GetRuleName(string serverName, int port, Protocol protocol)
        {
            return $"{serverName}-{CreateRuleHash(serverName, port, protocol)}";
        }

        private static string CreateRuleHash(string serverName, int port, Protocol protocol)
        {
            return Convert.ToHexString(
                MD5.HashData(
                    Encoding.UTF8.GetBytes(
                        $"{serverName}:{port}:{protocol}:TRYTOGUESSTHIS!!!!!!!*!"
                    )
                )
            );
        }
    }
}
