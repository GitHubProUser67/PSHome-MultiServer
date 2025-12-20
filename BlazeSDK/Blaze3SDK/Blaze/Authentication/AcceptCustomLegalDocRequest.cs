using Tdf;

namespace Blaze3SDK.Blaze.Authentication
{
    [TdfStruct]
    public struct AcceptCustomLegalDocRequest
    {

        [TdfMember("TURI")]
        public string mLegalDocUri;

    }
}
