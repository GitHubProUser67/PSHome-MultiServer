using Tdf;

namespace Blaze3SDK.Blaze.Clubs
{
	[TdfStruct]
	public struct SetMetadataRequest
	{

		[TdfMember("CLID")]
		public uint mClubId;

		[TdfMember("METD")]
		public string mMetaData;

		[TdfMember("MDTY")]
		public MetaDataType mMetaDataType;

	}
}
