using Tdf;

namespace Blaze3SDK.Blaze.League
{
	[TdfStruct]
	public struct FindLeaguesAsyncResponse
	{

		[TdfMember("CONT")]
		public uint mCount;

		[TdfMember("SQID")]
		public uint mSequenceId;

	}
}
