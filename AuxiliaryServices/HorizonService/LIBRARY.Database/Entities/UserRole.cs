namespace Horizon.LIBRARY.Database.Entities
{
    public partial class UserRole
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int RoleId { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime FromDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime? ToDt { get; set; }
    }
}
