using Tdf;

namespace Blaze3SDK.Blaze.DynamicInetFilter
{
	[TdfStruct]
	public struct AddMasterResponse
	{

		[TdfMember("MVER")]
		public uint mMapVersion;

		[TdfMember("RID")]
		public uint mRowId;

	}
}
