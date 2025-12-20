using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    public class FirstPartyId : TdfUnion
    {

        [TdfUnion(0)]
        private byte[]? mPS3Ticket;
        public byte[]? PS3Ticket { get { return mPS3Ticket; } set { SetValue(value); } }

        [TdfUnion(1)]
        private XboxId? mXboxId;
        public XboxId? XboxId { get { return mXboxId; } set { SetValue(value); } }

    }
}
