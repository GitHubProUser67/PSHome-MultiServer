using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct JoinGameByGroupMasterRequest
	{

		[TdfMember("JLEA")]
		public bool mJoinLeader;

		[TdfMember("JREQ")]
		public JoinGameRequest mJoinRequest;

		[TdfMember("SIDL")]
		public List<uint> mSessionIdList;

	}
}
