using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct JoinClubRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("PSWD")]
		public string mPassword;

	}
}
