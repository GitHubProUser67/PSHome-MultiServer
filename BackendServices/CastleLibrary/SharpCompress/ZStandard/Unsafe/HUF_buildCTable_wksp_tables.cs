namespace CastleLibrary.SharpCompress.ZStandard.Unsafe
{
    using System.Runtime.CompilerServices;

    public struct HUF_buildCTable_wksp_tables
    {
        public _huffNodeTbl_e__FixedBuffer huffNodeTbl;
        public _rankPosition_e__FixedBuffer rankPosition;

        [InlineArray(512)]
        public unsafe struct _huffNodeTbl_e__FixedBuffer
        {
            public nodeElt_s e0;
        }

        [InlineArray(192)]
        public unsafe struct _rankPosition_e__FixedBuffer
        {
            public rankPos e0;
        }
    }
}
