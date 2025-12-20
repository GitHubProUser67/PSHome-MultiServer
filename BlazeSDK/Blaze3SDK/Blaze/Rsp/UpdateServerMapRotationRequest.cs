using Tdf;

namespace Blaze3SDK.Blaze.Rsp
{
	[TdfStruct]
	public struct UpdateServerMapRotationRequest
	{

		[TdfMember("MAPR")]
		public MapRotation mMapRotation;

		[TdfMember("SID")]
		public uint mServerId;

	}
}
