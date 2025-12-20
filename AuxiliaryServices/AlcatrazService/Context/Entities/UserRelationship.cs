using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alcatraz.Context.Entities
{
	// Friends
	public class UserRelationship
	{
		public uint User1Id { get; set; }
		public User User1 { get; set; }

		public uint User2Id { get; set; }
		public User User2 { get; set; }

		public uint ByRelationShip { get; set; }
		public uint Details { get; set; }
	}
}
