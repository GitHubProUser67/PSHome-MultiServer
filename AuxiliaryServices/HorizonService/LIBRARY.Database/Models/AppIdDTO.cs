using System.Collections.Generic;

namespace Horizon.LIBRARY.Database.Models
{
    public class AppIdDTO
    {
        public string Name { get; set; } = string.Empty;
        public List<int> AppIds { get; set; }
    }
}