namespace Horizon.MUIS.Models
{
    public class UniverseInfo
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Status { get; set; }
        public int UserCount { get; set; }
        public int MaxUsers { get; set; }
        public string? Endpoint { get; set; }
        public string? SvoURL { get; set; }
        public string? ExtendedInfo { get; set; }
        public string? UniverseBilling { get; set; }
        public string? BillingSystemName { get; set; }
        public int Port { get; set; }
        public uint UniverseId { get; set; }
    }
}
