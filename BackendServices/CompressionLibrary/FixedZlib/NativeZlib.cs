using System;
using System.Runtime.InteropServices;

namespace FixedZlib
{
    public static class NativeZlib
    {
        private const int Z_OK = 0;
        private const int Z_STREAM_END = 1;
        private const int Z_FINISH = 4;
        private const string ZLIB1_VERSION = "1.3.1.0";
        private const string DLL_NAME = "zlib1.dll";

        public static readonly bool CanRun =
#if !DISABLE_NATIVE_ZLIB
        (Environment.OSVersion.Platform == PlatformID.Win32NT
          || Environment.OSVersion.Platform == PlatformID.Win32S 
            || Environment.OSVersion.Platform == PlatformID.Win32Windows)
              && (RuntimeInformation.OSArchitecture == Architecture.X86
                || RuntimeInformation.OSArchitecture == Architecture.X64
                  || RuntimeInformation.OSArchitecture == Architecture.Arm64);
#else
        false;
#endif
        [StructLayout(LayoutKind.Sequential)]
        private struct ZStream
        {
            public IntPtr next_in;
            public uint avail_in;
            public uint total_in;

            public IntPtr next_out;
            public uint avail_out;
            public uint total_out;

            public IntPtr msg;
            public IntPtr state;

            public IntPtr zalloc;
            public IntPtr zfree;
            public IntPtr opaque;

            public int data_type;
            public uint adler;
            public uint reserved;
        }

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int inflateInit2_(ref ZStream strm, int windowBits, string version, int stream_size);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int inflate(ref ZStream strm, int flush);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int inflateEnd(ref ZStream strm);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int compress2(byte[] dest, ref uint destLen, byte[] source, uint sourceLen, int level);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int uncompress(byte[] destBuffer, ref uint destLen, byte[] sourceBuffer, uint sourceLen);

        public static byte[] Deflate(byte[] data, byte level = 9)
        {
            uint destLen = (uint)((data.Length * 1.1) + 12);
            byte[] dest = new byte[destLen];
            compress2(dest, ref destLen, data, (uint)data.Length, level);
            byte[] result = new byte[destLen];
            Array.Copy(dest, 0, result, 0, result.Length);
            return result;
        }

        public static byte[] DeflateRaw(byte[] data, byte level = 9)
        {
            uint destLen = (uint)((data.Length * 1.1) + 12);
            byte[] dest = new byte[destLen];
            compress2(dest, ref destLen, data, (uint)data.Length, level);
            byte[] result = new byte[destLen - 6];
            Array.Copy(dest, 2, result, 0, result.Length);
            return result;
        }

        public static byte[] Inflate(byte[] data, uint destLen)
        {
            byte[] dest = new byte[destLen];
            if (uncompress(dest, ref destLen, data, (uint)data.Length) != 0)
                return null;
            byte[] result = new byte[destLen];
            Array.Copy(dest, 0, result, 0, result.Length);
            return result;
        }

        public static byte[] InflateRaw(byte[] deflatedData, int outputBufferSize = 1024 * 1024)
        {
            byte[] output = new byte[outputBufferSize];
            ZStream strm = new ZStream();

            GCHandle inHandle = GCHandle.Alloc(deflatedData, GCHandleType.Pinned);
            GCHandle outHandle = GCHandle.Alloc(output, GCHandleType.Pinned);

            try
            {
                strm.next_in = inHandle.AddrOfPinnedObject();
                strm.avail_in = (uint)deflatedData.Length;

                strm.next_out = outHandle.AddrOfPinnedObject();
                strm.avail_out = (uint)output.Length;

                int result = inflateInit2_(ref strm, -15, ZLIB1_VERSION, Marshal.SizeOf(typeof(ZStream)));
                if (result != Z_OK)
                    throw new Exception("[NativeZlib] - inflateInit2_ failed with code: " + result);

                result = inflate(ref strm, Z_FINISH);
                if (result != Z_STREAM_END && result != Z_OK)
                    throw new Exception("[NativeZlib] - inflate failed with code: " + result);

                inflateEnd(ref strm);

                byte[] decompressed = new byte[strm.total_out];
                Array.Copy(output, decompressed, strm.total_out);
                return decompressed;
            }
            finally
            {
                if (inHandle.IsAllocated)
                    inHandle.Free();
                if (outHandle.IsAllocated)
                    outHandle.Free();
            }
        }
    }
}
