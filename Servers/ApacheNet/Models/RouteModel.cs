// Copyright (C) 2016 by Barend Erasmus and donated to the public domain
using System;

namespace ApacheNet.Models
{
    public class Route
    {
        #region Properties

        public string? Name { get; set; } // descriptive name for debugging
        public string? UserAgentCriteria { get; set; }
        public string? UrlRegex { get; set; }
        public string? Method { get; set; }
        public string? HostCriteria { get; set; }
        public string[]? Hosts { get; set; }
        public string? ContentTypeCriteria { get; set; }
        public Func<ApacheContext, bool?>? Callable { get; set; }

        #endregion
    }
}
