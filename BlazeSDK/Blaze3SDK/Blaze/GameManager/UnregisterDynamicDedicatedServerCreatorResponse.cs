using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct UnregisterDynamicDedicatedServerCreatorResponse
	{

		[TdfMember("MLST")]
		public List<string> mMachineIdList;

	}
}
