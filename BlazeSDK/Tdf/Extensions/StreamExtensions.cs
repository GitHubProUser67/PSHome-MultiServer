namespace Tdf.Extensions
{
    public static class StreamExtensions
    {
        public static async Task<bool> ReadAllAsync(this Stream stream, byte[] buffer, int startIndex, int count)
        {
            if (stream == null)
                return false;

            int offset = 0;
            while (offset < count)
            {
                int readCount = await stream.ReadAsync(buffer, startIndex + offset, count - offset).ConfigureAwait(false);
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

            int offset = 0;
            while (offset < count)
            {
                int readCount = stream.Read(buffer, startIndex + offset, count - offset);
                if (readCount == 0)
                    return false;
                offset += readCount;
            }
            return true;
        }
    }
}
