using Tdf;

namespace Blaze3SDK.Blaze.Association
{
	[TdfStruct]
	public struct UpdateListsRequest
	{

		[TdfMember("LIDS")]
		public List<ListIdentification> mListIdentificationVector;

		[TdfMember("MUTA")]
		public bool mMutualAction;

	}
}
