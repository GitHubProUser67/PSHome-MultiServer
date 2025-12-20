using Tdf;

namespace Blaze2SDK.Blaze
{
    public class NetworkAddress : TdfUnion
    {

        [TdfUnion(0)]
        private XboxClientAddress? mXboxClientAddress;
        public XboxClientAddress? XboxClientAddress { get { return mXboxClientAddress; } set { SetValue(value); } }

        [TdfUnion(1)]
        private XboxServerAddress? mXboxServerAddress;
        public XboxServerAddress? XboxServerAddress { get { return mXboxServerAddress; } set { SetValue(value); } }

        [TdfUnion(2)]
        private IpPairAddress? mIpPairAddress;
        public IpPairAddress? IpPairAddress { get { return mIpPairAddress; } set { SetValue(value); } }

        [TdfUnion(3)]
        private IpAddress? mIpAddress;
        public IpAddress? IpAddress { get { return mIpAddress; } set { SetValue(value); } }

        [TdfUnion(4)]
        private HostNameAddress? mHostNameAddress;
        public HostNameAddress? HostNameAddress { get { return mHostNameAddress; } set { SetValue(value); } }

    }
}
