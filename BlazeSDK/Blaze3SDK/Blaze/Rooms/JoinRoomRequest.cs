using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct JoinRoomRequest
	{

		[TdfMember("CTID")]
		public uint mCategoryId;

		[TdfMember("INID")]
		public long mInviterId;

		[TdfMember("INVT")]
		public bool mIsUserInvited;

		[TdfMember("PASS")]
		public string mPassword;

		[TdfMember("PVAL")]
		public string mPseudoValue;

		[TdfMember("RMID")]
		public uint mRoomId;

	}
}
