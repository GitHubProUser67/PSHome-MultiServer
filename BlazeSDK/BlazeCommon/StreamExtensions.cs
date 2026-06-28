namespace BlazeCommon
{
    public static class StreamExtensions
    {
        public static async Task<bool> ReadAllAsync(
            this Stream stream,
            byte[] buffer,
            int startIndex,
            int count
        )
        {
            if (stream == null)
                return false;

            var offset = 0;
            while (offset < count)
            {
                var readCount = await stream
                    .ReadAsync(buffer.AsMemory(startIndex + offset, count - offset))
                    .ConfigureAwait(false);
                if (readCount == 0)
                    return false;
                offset += readCount;
            }
            return true;
        }

        public static bool ReadAll(this Stream stream, byte[] buffer, int startIndex, int count)
        {
            if (stream == null)
                return false;

            var offset = 0;
            while (offset < count)
            {
                var readCount = stream.Read(buffer, startIndex + offset, count - offset);
                if (readCount == 0)
                    return false;
                offset += readCount;
            }
            return true;
        }
    }
}
