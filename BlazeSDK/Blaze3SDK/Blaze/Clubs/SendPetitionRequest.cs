using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct SendPetitionRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("PSWD")]
		public string mPassword;

	}
}
