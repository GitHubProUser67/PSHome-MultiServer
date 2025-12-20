namespace MitmDNS
{
    public struct DnsSettings
    {
        public string Address; // For redirect to
        public HandleMode? Mode;
    }

    public enum HandleMode
    {
        Deny,
        Allow,
        Redirect
    }
}
