using System;
using System.Collections.Generic;

namespace WebAPIService.GameServices.UBISOFT.Models.Hermes.v1
{
    public class Sessions
    {
        public object children { get; set; }

        public List<string> tags { get; set; }

        public string spaceId { get; set; }

        public string spaceType { get; set; }

        public string spaceName { get; set; }

        public string parentSpaceId { get; set; }

        public string parentSpaceName { get; set; }

        public string releaseType { get; set; }

        public string platformType { get; set; }

        public DateTime dateCreated { get; set; } = DateTime.Parse("2012-08-01T03:30:12.0000000Z");

        public DateTime dateLastModified { get; set; } = DateTime.Parse("2020-10-30T05:59:36.0000000Z");
    }
}
