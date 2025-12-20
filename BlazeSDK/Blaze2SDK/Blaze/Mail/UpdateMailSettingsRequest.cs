using Tdf;

namespace Blaze2SDK.Blaze.Mail
{
    [TdfStruct]
    public struct UpdateMailSettingsRequest
    {
        
        [TdfMember("MSET")]
        public MailSettings mMailSettings;
        
    }
}
