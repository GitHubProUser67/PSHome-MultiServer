using Tdf;

namespace Blaze3SDK.Blaze
{
	[TdfStruct]
	public struct EntitiesLookupByIdsResponse
	{

		[TdfMember("NAME")]
		public List<string> mEntityNames;

	}
}
