namespace CastleLibrary.SharpCompress.ZStandard
{
    internal partial class ZStandardStream
    {
        internal static async ValueTask<bool> IsZStandardAsync(
            Stream stream,
            CancellationToken cancellationToken = default
        )
        {
            cancellationToken.ThrowIfCancellationRequested();
            var buffer = new byte[4];
            var bytesRead = await stream
                .ReadAsync(buffer.AsMemory(0, 4), cancellationToken)
                .ConfigureAwait(false);
            if (bytesRead < 4)
            {
                return false;
            }

            var magic = BitConverter.ToUInt32(buffer, 0);
            if (ZstandardConstants.MAGIC != magic)
            {
                return false;
            }
            return true;
        }
    }
}
