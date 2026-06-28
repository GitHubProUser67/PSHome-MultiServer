namespace CastleLibrary.SharpCompress.ZStandard.Unsafe
{
    using System.Runtime.InteropServices;

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate nuint ZSTD_BlockCompressor_f(
        ZSTD_MatchState_t* bs,
        SeqStore_t* seqStore,
        uint* rep,
        void* src,
        nuint srcSize
    );
}
