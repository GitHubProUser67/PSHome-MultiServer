using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct IndirectJoinGameSetupContext
	{

		[TdfMember("RPVC")]
		public bool mRequiresClientVersionCheck;

		[TdfMember("GRID")]
		public BlazeObjectId mUserGroupId;

	}
}
