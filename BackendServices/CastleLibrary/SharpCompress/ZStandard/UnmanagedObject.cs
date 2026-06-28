using System.Runtime.InteropServices;

namespace CastleLibrary.SharpCompress.ZStandard
{
    /*
 * Wrap object to void* to make it unmanaged
 */
    internal unsafe static class UnmanagedObject
    {
        public static void* Wrap(object obj) => (void*)GCHandle.ToIntPtr(GCHandle.Alloc(obj));

        private static GCHandle UnwrapGcHandle(void* value) => GCHandle.FromIntPtr((IntPtr)value);

        public static T Unwrap<T>(void* value) => (T)UnwrapGcHandle(value).Target!;

        public static void Free(void* value) => UnwrapGcHandle(value).Free();
    }
}
