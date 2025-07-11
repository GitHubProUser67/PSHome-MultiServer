using System;
using System.IO;

namespace MultiServerLibrary.Extension
{
    public static class MemoryStreamUtils
    {
        public static void Clear(this MemoryStream source)
        {
            byte[] buffer = source.GetBuffer();
            Array.Clear(buffer, 0, buffer.Length);
            source.Position = 0;
            source.SetLength(0);
        }
    }
}
