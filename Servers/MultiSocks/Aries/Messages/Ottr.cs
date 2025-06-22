using MultiSocks.Aries.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiSocks.Aries.Messages
{
    public class Ottr : AbstractMessage
    {
        public override string _Name { get => "ottr"; }

        public override void Process(AbstractAriesServer context, AriesClient client)
        {
            client.SendMessage(this);
        }
    }
}
