using CastleLibrary.ICSharpCode.SharpZipLib.Core;

namespace CastleLibrary.ICSharpCode.SharpZipLib.Zip.Compression
{
    /// <summary>
    /// A marker type for pooled version of an inflator that we can return back to <see cref="InflaterPool"/>.
    /// </summary>
    internal sealed class PooledInflater(bool noHeader) : Inflater(noHeader)
    {
    }
}
