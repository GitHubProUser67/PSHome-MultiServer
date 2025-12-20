using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
	[TdfStruct]
	public struct FieldValidateErrorList
	{

		[TdfMember("LIST")]
		public List<FieldValidationError> mList;

	}
}
