using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alcatraz.Context.Entities
{
	public class SentInvitation
	{
		public uint Id { get; set; }
		public uint SenderId { get; set; }
		public uint GatheringId { get; set; }
		public uint Message { get; set; }
		public DateTime ValidUntil { get; set; }
	}
}
