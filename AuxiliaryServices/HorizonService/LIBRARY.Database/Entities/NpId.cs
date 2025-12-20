using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class NpId
    {
        public int AppId { get; set; }
        public byte[] data { get; set; }
        public byte term { get; set; }
        public byte[] dummy { get; set; }

        public byte[] opt { get; set; }
        public byte[] reserved { get; set; }

        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime ModifiedDt { get; set; }
    }
}
