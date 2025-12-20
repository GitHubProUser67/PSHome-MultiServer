namespace QuazalServer.RDVServices.DDL.Models
{
	public class Privilege
	{
		public Privilege() { }
		public Privilege(uint id, string description)
        {
            m_ID = id;
            m_description = description;
        }

        public uint m_ID { get; set; }
		public string? m_description { get; set; }
	}

	public class PrivilegeEx
	{
		public PrivilegeEx() { }
		public PrivilegeEx(uint id, string description)
		{
			m_ID = id;
			m_description = description;
		}
		public uint m_ID { get; set; }
		public string? m_description { get; set; }
		public ushort m_duration { get; set; }
	}

	public class PrivilegeGroup
    {
		public PrivilegeGroup()
        {
			m_privileges = new List<Privilege>();
		}
		public PrivilegeGroup(string description)
        {
            m_description=description;
            m_privileges = new List<Privilege>();
        }

        public string? m_description { get; set; }
		public ICollection<Privilege> m_privileges { get; set; }
	}

	public static class DeluxePrivileges
    {
		// enum UnlockablesManager::eUnlockables : __int32
		// {
		//   U_Cars = 0x0,
		//   U_UniqueKeyChallenges_1 = 0x1,
		//   U_UniqueKeyChallenges_2 = 0x2,
		//   U_UniqueKeyChallenges_3 = 0x3,
		//   U_UniqueKeyChallenges_4 = 0x4,
		//   U_UniqueVehicle = 0x5,
		//   U_Uplay_TimeTrial = 0x6,
		//   U_Uplay_Challenges = 0x7,
		//   U_Car_1 = 8,
		//   U_Car_2 = 9,
		//   U_Car_3 = 10,
		//   U_Car_4 = 11,
		//   U_Car_5 = 12,
		//   U_Car_6 = 13,
		//   U_Car_7 = 14,
		//   U_Car_8 = 15,
		//   U_Car_9 = 16,
		//   U_Purple_Car = 17,
		//   U_MAX = 18,
		// };

		public static Privilege[] ChallengeIds = new Privilege[] {
			// new Privilege(0, "Cars"), // not defined in the game's Lua!
			new Privilege(1, "Unique Challenge - Team Colors Tutorial"),
			new Privilege(2, "Unique Challenge - Smoke Trail"), 
			new Privilege(3, "Unique Challenge - Relay Race"), 
			new Privilege(4, "Unique Challenge - Mass Chase"), 
			// new Privilege(5, "Unique Vehicle"), // not defined in the game's Lua!
			new Privilege(6, "Uplay Challenge - Time Trial"), 
			new Privilege(7, "Uplay Challenges"),
		};
		public static Privilege[] VehicleIds = new Privilege[] {
			new Privilege(8, "2010 Chevrolet Camaro SS"), 
			new Privilege(9, "1966 Shelby Cobra 427"), 
			new Privilege(10, "1972 Lamborghini Miura"),  
			new Privilege(11, "1963 Aston Martin DB5"),  
			new Privilege(12, "1968 Ford Mustang Fastback GT390"),  
			new Privilege(13, "2009 Alfa Romeo Mito"),  
			new Privilege(14, "1959 Cadillac Eldorado"),  
			new Privilege(15, "1973 Chevrolet El Camino"),  
			new Privilege(16, "1963 Volkswagen Kafer/Beetle"), 
			new Privilege(17, "1959 Cadillac Eldorado in Purple Royal")
		};
	}
}
