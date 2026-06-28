namespace MultiServerLibrary.Extension
{
    public static class MemoryStreamUtils
    {
        extension(MemoryStream source)
        {
            public void Clear()
            {
                var buffer = source.GetBuffer();
                Array.Clear(buffer, 0, buffer.Length);
                source.Position = 0;
                source.SetLength(0);
            }
        }
    }
}
