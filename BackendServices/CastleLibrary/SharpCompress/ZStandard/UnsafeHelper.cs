using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CastleLibrary.SharpCompress.ZStandard
{
    public unsafe static class UnsafeHelper
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Malloc(ulong size)
        {
            var ptr = NativeMemory.Alloc((nuint)size);
            return ptr;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* Calloc(ulong num, ulong size)
        {
            return NativeMemory.AllocZeroed((nuint)num, (nuint)size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memcpy(void* destination, void* source, uint size) =>
            System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(destination, source, size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memset(void* memPtr, byte val, uint size) =>
            System.Runtime.CompilerServices.Unsafe.InitBlockUnaligned(memPtr, val, size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Free(void* ptr)
        {
            NativeMemory.Free(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T* GetArrayPointer<T>(T[] array)
            where T : unmanaged
        {
            var size = (uint)(sizeof(T) * array.Length);
            // This function is used to allocate memory for static data blocks.
            // We have to use AllocateTypeAssociatedMemory and link the memory's
            // lifetime to this assembly, in order to prevent memory leaks when
            // loading the assembly in an unloadable AssemblyLoadContext.
            // While introduced in .NET 5, we call this only in .NET 9+, because
            // it's not implemented in the Mono runtime until then.
            var destination = (T*)
                RuntimeHelpers.AllocateTypeAssociatedMemory(typeof(UnsafeHelper), (int)size);
            fixed (void* source = &array[0])
            {
                System.Runtime.CompilerServices.Unsafe.CopyBlockUnaligned(
                    destination,
                    source,
                    size
                );
            }

            return destination;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Assert(bool condition, string? message = null)
        {
            if (!condition)
            {
                throw new ArgumentException(message ?? "assert failed");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Memmove(void* destination, void* source, ulong size) =>
            Buffer.MemoryCopy(source, destination, size, size);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Memcmp(void* buf1, void* buf2, ulong size)
        {
            Assert(size <= int.MaxValue);
            var intSize = (int)size;
            return new ReadOnlySpan<byte>(buf1, intSize).SequenceCompareTo(
                new ReadOnlySpan<byte>(buf2, intSize)
            );
        }
    }
}
