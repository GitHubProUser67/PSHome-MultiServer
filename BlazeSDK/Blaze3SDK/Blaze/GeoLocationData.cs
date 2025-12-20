using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct GeoLocationData
	{

		[TdfMember("ID")]
		public long mBlazeId;

		[TdfMember("CTY")]
		public string mCity;

		[TdfMember("CNTY")]
		public string mCountry;

		[TdfMember("LAT")]
		public int mLatitude;

		[TdfMember("LON")]
		public int mLongitude;

		[TdfMember("OPT")]
		public bool mOptOut;

		[TdfMember("ST")]
		public string mStateRegion;

	}
}
