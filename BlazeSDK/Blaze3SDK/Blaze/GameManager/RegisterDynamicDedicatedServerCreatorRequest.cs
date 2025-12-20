using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct RegisterDynamicDedicatedServerCreatorRequest
	{

		[TdfMember("LMAP")]
		public SortedDictionary<string, MachineLoadCapacity> mMachineLoadCapacityMap;

	}
}
