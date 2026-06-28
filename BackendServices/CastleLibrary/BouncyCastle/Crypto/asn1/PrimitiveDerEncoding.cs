using System;
using System.Diagnostics;

namespace Org.BouncyCastle.Asn1
{
    internal class PrimitiveDerEncoding : DerEncoding
    {
        internal readonly byte[] m_contentsOctets;

        internal PrimitiveDerEncoding(int tagClass, int tagNo, byte[] contentsOctets)
            : base(tagClass, tagNo)
        {
            Debug.Assert(contentsOctets != null);
            m_contentsOctets = contentsOctets;
        }

        protected internal override int CompareLengthAndContents(DerEncoding other)
        {
            if (other is PrimitiveDerEncodingSuffixed suffixed)
                return -suffixed.CompareLengthAndContents(this);

            if (!(other is PrimitiveDerEncoding that))
                throw new InvalidOperationException();

            int length = this.m_contentsOctets.Length;
            if (length != that.m_contentsOctets.Length)
                return length - that.m_contentsOctets.Length;

            return this
                .m_contentsOctets.AsSpan(0, length)
                .SequenceCompareTo(that.m_contentsOctets.AsSpan(0, length));
        }

        public override void Encode(Asn1OutputStream asn1Out)
        {
            asn1Out.WriteIdentifier(m_tagClass, m_tagNo);
            asn1Out.WriteDL(m_contentsOctets.Length);
            asn1Out.Write(m_contentsOctets, 0, m_contentsOctets.Length);
        }

        public override int GetLength()
        {
            return Asn1OutputStream.GetLengthOfEncodingDL(m_tagNo, m_contentsOctets.Length);
        }
    }
}
