namespace Horizon.LIBRARY.Database.Entities
{
    public partial class Banned
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public DateTime FromDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime? ToDt { get; set; }
    }
}
