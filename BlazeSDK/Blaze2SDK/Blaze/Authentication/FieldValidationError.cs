using Tdf;

namespace Blaze2SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct FieldValidationError
    {
        
        [TdfMember("ERR")]
        public NucleusCause mError;
        
        [TdfMember("FLD")]
        public NucleusField mField;
        
    }
}
