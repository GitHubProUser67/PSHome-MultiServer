using Tdf;

namespace Blaze3SDK.Blaze.GameReporting
{
	[TdfStruct]
	public struct UpdateMetricRequest
	{

		[TdfMember("METR")]
		public string mMetricName;

		[TdfMember("VALU")]
		public long mValue;

	}
}
