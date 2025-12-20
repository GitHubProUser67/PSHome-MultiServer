using Tdf;

namespace Blaze2SDK.Blaze.Mail
{
    [TdfStruct]
    public struct MailSettings
    {

        [TdfMember("EFPF")]
        public EmailFormatPref mEmailFormatPref;

        [TdfMember("OPTF")]
        public EmailOptInFlags mEmailOptInFlags;

    }
}
