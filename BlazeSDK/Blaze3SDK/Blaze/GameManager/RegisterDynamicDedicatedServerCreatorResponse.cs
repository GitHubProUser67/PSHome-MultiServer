using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct RegisterDynamicDedicatedServerCreatorResponse
	{

		[TdfMember("MLST")]
		public List<string> mRegisteredMachineList;

	}
}
