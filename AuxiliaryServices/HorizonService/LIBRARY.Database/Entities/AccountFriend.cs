using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class AccountFriend
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int FriendAccountId { get; set; }
        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor

        public virtual Account Account { get; set; }
    }
}
