using System.Runtime.InteropServices;
using DNSLibrary.Utils;
using EndianTools;
using EndianTools.Marshalling;

namespace DNSLibrary
{
    public class Question(
        Domain domain,
        RecordType type = RecordType.A,
        RecordClass klass = RecordClass.IN
        ) : IMessageEntry
    {
        public static IList<Question> GetAllFromArray(byte[] message, int offset, int questionCount)
        {
            return GetAllFromArray(message, offset, questionCount, out _);
        }

        public static IList<Question> GetAllFromArray(
            byte[] message,
            int offset,
            int questionCount,
            out int endOffset
        )
        {
            IList<Question> questions = [with(questionCount)];

            for (var i = 0; i < questionCount; i++)
                questions.Add(FromArray(message, offset, out offset));

            endOffset = offset;
            return questions;
        }

        public static Question FromArray(byte[] message, int offset)
        {
            return FromArray(message, offset, out _);
        }

        public static Question FromArray(byte[] message, int offset, out int endOffset)
        {
            var domain = Domain.FromArray(message, offset, out offset);

            if (offset + Tail.SIZE > message.Length)
                throw new ArgumentException("Message too short for question tail");

            var tail = new Tail
            {
                Type = (RecordType)
                    EndianAwareConverter.ToUInt16(message, Endianness.BigEndian, (uint)offset),
                Class = (RecordClass)
                    EndianAwareConverter.ToUInt16(
                        message,
                        Endianness.BigEndian,
                        (uint)(offset + 2)
                    ),
            };

            endOffset = offset + Tail.SIZE;

            return new Question(domain, tail.Type, tail.Class);
        }

        private readonly Domain domain = domain;
        private readonly RecordType type = type;
        private readonly RecordClass klass = klass;

        public Domain Name
        {
            get { return domain; }
        }

        public RecordType Type
        {
            get { return type; }
        }

        public RecordClass Class
        {
            get { return klass; }
        }

        public int Size
        {
            get { return domain.Size + Tail.SIZE; }
        }

        public byte[] ToArray()
        {
            var result = new ByteStream(Size);

            result
                .Append(domain.ToArray())
                .Append(Struct.GetBytes(new Tail { Type = Type, Class = Class }));

            return result.ToArray();
        }

        public override string ToString()
        {
            return ObjectStringifier
                .New(this)
                .Add(nameof(Name), nameof(Type), nameof(Class))
                .ToString();
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        private struct Tail
        {
            public const int SIZE = 4;

            private ushort type;
            private ushort klass;

            public RecordType Type
            {
                readonly get { return (RecordType)type; }
                set { type = (ushort)value; }
            }

            public RecordClass Class
            {
                readonly get { return (RecordClass)klass; }
                set { klass = (ushort)value; }
            }
        }
    }
}
