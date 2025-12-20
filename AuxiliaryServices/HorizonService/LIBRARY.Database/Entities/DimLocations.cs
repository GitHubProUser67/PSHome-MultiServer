using Horizon.RT.Common;
using System;

namespace Horizon.LIBRARY.Database.Entities
{
    public partial class DimLocations
    {
        public int Id { get; set; }

        public int LocationId { get; set; }
        public string LocationName { get; set; }
        public string ChannelName { get; set; }
        public int[] AppIds { get; set; }
        public MediusWorldGenericFieldLevelType GenericFieldLevel { get; set; }

        public DateTime CreateDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
        public DateTime? ModifiedDt { get; set; }
        public DateTime FromDt { get; set; } = DateTime.UtcNow; // Set default value in constructor
    }
}