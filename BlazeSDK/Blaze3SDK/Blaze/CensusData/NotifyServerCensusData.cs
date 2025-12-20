using Tdf;

namespace Blaze3SDK.Blaze.CensusData
{
	[TdfStruct]
	public struct NotifyServerCensusData
	{

		[TdfMember("TDFL")]
		public List<NotifyServerCensusDataItem> mCensusDataList;

	}
}
