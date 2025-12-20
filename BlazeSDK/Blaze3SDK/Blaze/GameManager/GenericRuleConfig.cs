using Tdf;

namespace Blaze3SDK.Blaze.GameManager
{
	[TdfStruct]
	public struct GenericRuleConfig
	{

		[TdfMember("ANME")]
		public string mAttributeName;

		[TdfMember("ATYP")]
		public GenericRuleAttributeType mAttributeType;

		[TdfMember("POSV")]
		public List<string> mPossibleValues;

		[TdfMember("RNME")]
		public string mRuleName;

		[TdfMember("THLS")]
		public List<string> mThresholdNames;

		[TdfMember("WGHT")]
		public uint mWeight;

		public enum GenericRuleAttributeType : int
		{
			PLAYER_ATTRIBUTE = 0,
			GAME_ATTRIBUTE = 1,
		}

	}
}
