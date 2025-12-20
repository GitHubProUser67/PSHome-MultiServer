namespace Horizon.LIBRARY.Database.Models
{
    public class BuddyDTO
    {
        /// <summary>
        /// Unique account id.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Unique account id of buddy to add/remove.
        /// </summary>
        public int BuddyAccountId { get; set; }
    }

    public class IgnoredDTO
    {
        /// <summary>
        /// Unique account id.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Unique account id of ignored to add/remove.
        /// </summary>
        public int IgnoredAccountId { get; set; }
    }
}