using System;
using System.IO;

namespace EndianTools.BinaryExtension
{
    public class LEBinaryReader : EndianAwareBinaryReader
    {
        public LEBinaryReader(Stream input) : base(input)
        {
        }

        public override byte[] ReadBytes(int length)
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return Array.Empty<byte>();

            return m_br.ReadBytes(length);
        }

        public override byte ReadByte()
        {
            byte[] bytes = ReadBytes(1);
            if (bytes.Length == 0)
                return 0;
            return bytes[0];
        }

        public override short ReadInt16()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            return m_br.ReadInt16();
        }

        public override int ReadInt32()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            return m_br.ReadInt32();
        }

        public override float ReadSingle()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            return m_br.ReadSingle();
        }

        public override ushort ReadUInt16()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            return m_br.ReadUInt16();
        }

        public override uint ReadUInt32()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            return m_br.ReadUInt32();
        }

        public override long ReadInt64()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            return m_br.ReadInt64();
        }

        public override ulong ReadUInt64()
        {
            //.NET8 m_br.BaseStream will have length 0 sometimes.like:https://github.com/dotnet/wcf/issues/5205
            if (m_br.BaseStream.Length == 0)
                return 0;

            return m_br.ReadUInt64();
        }
    }
}