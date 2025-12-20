using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct FieldValidateErrorList
    {
        
        [TdfMember("LIST")]
        public List<FieldValidationError> mList;
        
    }
}
