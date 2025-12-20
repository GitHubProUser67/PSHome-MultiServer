using Tdf;

namespace Blaze2SDK.Blaze.Locker
{
    [TdfStruct]
    public struct ConfirmationRequest
    {
        
        [TdfMember("INFO")]
        public ContentInfo mContentInfo;
        
        [TdfMember("STST")]
        public UploadStatus mUploadStatus;
        
    }
}
