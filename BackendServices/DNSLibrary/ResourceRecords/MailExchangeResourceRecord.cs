namespace DNSLibrary.ResourceRecords
{
    public class MailExchangeResourceRecord : BaseResourceRecord
    {
        private const int PREFERENCE_SIZE = 2;

        private static ResourceRecord Create(
            Domain domain,
            int preference,
            Domain exchange,
            TimeSpan ttl
        )
        {
            var pref = BitConverter.GetBytes((ushort)preference);
            var data = new byte[pref.Length + exchange.Size];

            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
            {
                Array.Reverse(pref);
            }

            pref.CopyTo(data, 0);
            exchange.ToArray().CopyTo(data, pref.Length);

            return new ResourceRecord(domain, data, RecordType.MX, RecordClass.IN, ttl);
        }

        public MailExchangeResourceRecord(IResourceRecord record, byte[] message, int dataOffset)
            : base(record)
        {
            var preference = new byte[PREFERENCE_SIZE];
            Array.Copy(message, dataOffset, preference, 0, preference.Length);

            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
            {
                Array.Reverse(preference);
            }

            dataOffset += PREFERENCE_SIZE;

            Preference = BitConverter.ToUInt16(preference, 0);
            ExchangeDomainName = Domain.FromArray(message, dataOffset);
        }

        public MailExchangeResourceRecord(
            Domain domain,
            int preference,
            Domain exchange,
            TimeSpan ttl = default
        )
            : base(Create(domain, preference, exchange, ttl))
        {
            Preference = preference;
            ExchangeDomainName = exchange;
        }

        public int Preference { get; }
        public Domain ExchangeDomainName { get; }

        public override string ToString()
        {
            return Stringify().Add("Preference", "ExchangeDomainName").ToString();
        }
    }
}
