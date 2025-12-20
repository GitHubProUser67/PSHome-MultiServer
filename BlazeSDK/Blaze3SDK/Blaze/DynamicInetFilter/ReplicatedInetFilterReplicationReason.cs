using Tdf;

namespace Blaze3SDK.Blaze.DynamicInetFilter
{
	[TdfStruct(0xDC68BBC0)]
	public struct ReplicatedInetFilterReplicationReason
	{

		[TdfMember("RSN")]
		public Reason mReason;

		public enum Reason : int
		{
			MAP_CREATED = 0,
			MAP_DESTROYED = 1,
			OBJECT_CREATED = 2,
			OBJECT_UPDATED = 3,
			OBJECT_DESTROYED = 4,
		}

	}
}
