using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSocks.Aries.Messages.Burnout3Plugin
{
    namespace MultiSocks.Aries.Messages
    {
        public class BO3Stats : AbstractMessage
        {
            public override string _Name { get => "sviw"; }

            public string N { get; set; } = "5";
            public string NAMES { get; set; } = "1,2,3,4,5";
            public string DESCS { get; set; } = "1,2,3,4,5";
            public string PARAMS { get; set; } = "1,1,1,1,1";
            public string WIDTHS { get; set; } = "1,1,1,1,1";
        }
    }

}
