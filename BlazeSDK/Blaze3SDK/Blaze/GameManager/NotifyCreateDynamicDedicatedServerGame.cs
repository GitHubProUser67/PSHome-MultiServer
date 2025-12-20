using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct NotifyCreateDynamicDedicatedServerGame
	{

		[TdfMember("GREQ")]
		public CreateGameRequest mCreateGameRequest;

		[TdfMember("MID")]
		public string mMachineId;

	}
}
