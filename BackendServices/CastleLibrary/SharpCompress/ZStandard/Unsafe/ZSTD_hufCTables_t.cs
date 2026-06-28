namespace CastleLibrary.SharpCompress.ZStandard.Unsafe
{
    using System.Runtime.CompilerServices;

    public unsafe struct ZSTD_hufCTables_t
    {
        public _CTable_e__FixedBuffer CTable;
        public HUF_repeat repeatMode;

        [InlineArray(257)]
        public unsafe struct _CTable_e__FixedBuffer
        {
            public nuint e0;
        }
    }
}
