using Tdf;

namespace Blaze3SDK.Blaze.Rooms
{
	[TdfStruct]
	public struct ScheduledCategory
	{

		[TdfMember("SOID")]
		public uint mScheduledId;

		[TdfMember("SCHS")]
		public ScheduledCategorySpec mScheduledSpec;

	}
}
