using Tdf;

namespace Blaze3SDK.Blaze.Tournaments
{
	[TdfStruct]
	public struct GetTrophiesRequest
	{

		[TdfMember("BZID")]
		public long mBlazeId;

		[TdfMember("NUMT")]
		public uint mNumTrophies;

	}
}
