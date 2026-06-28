namespace CastleLibrary.SharpCompress.ZStandard.Unsafe
{
    public unsafe struct Fingerprint
    {
        public fixed uint events[1024];
        public nuint nbEvents;
    }
}
