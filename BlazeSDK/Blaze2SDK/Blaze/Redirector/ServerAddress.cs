using Tdf;

namespace Blaze2SDK.Blaze.Redirector
{
    public class ServerAddress : TdfUnion
    {

        [TdfUnion(0)]
        private IpAddress? mIpAddress;
        public IpAddress? IpAddress { get { return mIpAddress; } set { SetValue(value); } }

        [TdfUnion(1)]
        private XboxServerAddress? mXboxServerAddress;
        public XboxServerAddress? XboxServerAddress { get { return mXboxServerAddress; } set { SetValue(value); } }

    }
}
