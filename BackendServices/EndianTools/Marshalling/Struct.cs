using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;

namespace EndianTools.Marshalling
{
    public static class Struct
    {
        private static byte[] ConvertEndian<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T
        >(byte[] data)
        {
            var type = typeof(T);
            var fields = type.GetFields(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance
            );
            EndianAttribute endian = null;

            if (type.GetTypeInfo().IsDefined(typeof(EndianAttribute), false))
                endian = (EndianAttribute)
                    type.GetTypeInfo().GetCustomAttributes(typeof(EndianAttribute), false).First();

            foreach (var field in fields)
            {
                if (endian == null && !field.IsDefined(typeof(EndianAttribute), false))
                    continue;

                var offset = Marshal.OffsetOf<T>(field.Name).ToInt32();
#pragma warning disable 618
                var length = Marshal.SizeOf(field.FieldType);
#pragma warning restore 618
                endian ??= (EndianAttribute)
                    field.GetCustomAttributes(typeof(EndianAttribute), false).First();

                if (
                    (
                        endian.Endianness == Endianness.BigEndian
                        && EndianAwareConverter.isLittleEndianSystem
                    )
                    || (
                        endian.Endianness == Endianness.LittleEndian
                        && !EndianAwareConverter.isLittleEndianSystem
                    )
                )
                    Array.Reverse(data, offset, length);
            }

            return data;
        }

        public static byte[] GetBytes<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T
        >(T obj)
            where T : struct
        {
            var data = new byte[Marshal.SizeOf(obj)];
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);

            try
            {
                Marshal.StructureToPtr(obj, handle.AddrOfPinnedObject(), false);
                return ConvertEndian<T>(data);
            }
            finally
            {
                handle.Free();
            }
        }
    }
}
