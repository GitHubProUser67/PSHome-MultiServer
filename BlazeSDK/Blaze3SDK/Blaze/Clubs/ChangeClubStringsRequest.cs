using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct ChangeClubStringsRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("CDSC")]
		public string mDescription;

		[TdfMember("CNAM")]
		public string mName;

		[TdfMember("NUQN")]
		public string mNonUniqueName;

	}
}
