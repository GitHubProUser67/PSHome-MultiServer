using Tdf;

namespace Blaze3SDK.Blaze.Playgroups
{
	[TdfStruct]
	public struct CreatePlaygroupRequest
	{

		[TdfMember("JOIN")]
		public bool mJoinIfExists;

		[TdfMember("PGRP")]
		public PlaygroupInfo mPlaygroupInfo;

	}
}
