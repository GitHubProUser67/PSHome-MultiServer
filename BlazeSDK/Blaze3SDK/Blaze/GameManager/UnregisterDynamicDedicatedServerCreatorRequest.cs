using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct UnregisterDynamicDedicatedServerCreatorRequest
	{

		[TdfMember("MLST")]
		public List<string> mMachineIdList;

	}
}
