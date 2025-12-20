using System;

namespace TechnitiumLibrary.Net.Firewall
{
    public class FirewallHelper
    {
        public static void CheckFirewallEntries(string appPath)
        {
            if (string.IsNullOrWhiteSpace(appPath))
                return;

            if (appPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                appPath = appPath.Substring(0, appPath.Length - 4) + ".exe";

            if (!WindowsFirewallEntryExists(appPath))
                AddWindowsFirewallEntry(appPath);
        }

        private static bool WindowsFirewallEntryExists(string appPath)
        {
            try
            {
                return WindowsFirewall.RuleExistsVista(string.Empty, appPath) == RuleStatus.Allowed;
            }
            catch
            {

            }

            return false;
        }

        private static bool AddWindowsFirewallEntry(string appPath)
        {
            try
            {
                switch (WindowsFirewall.RuleExistsVista(string.Empty, appPath))
                {
                    case RuleStatus.Blocked:
                    case RuleStatus.Disabled:
                        WindowsFirewall.RemoveRuleVista(string.Empty, appPath);
                        break;

                    case RuleStatus.Allowed:
                        return true;
                }

                WindowsFirewall.AddRuleVista("MultiServer", $"Allows incoming connection request to the server.", FirewallAction.Allow, appPath, Protocol.ANY, null, null, null, null, InterfaceTypeFlags.All, true, Direction.Inbound, true);

                return true;
            }
            catch
            {
                
            }

            return false;
        }
    }
}
