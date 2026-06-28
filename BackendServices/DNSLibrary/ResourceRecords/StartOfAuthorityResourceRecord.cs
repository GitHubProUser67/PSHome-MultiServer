using System.Runtime.InteropServices;
using DNSLibrary.Utils;
using EndianTools;
using EndianTools.Marshalling;

namespace DNSLibrary.ResourceRecords
{
    public class StartOfAuthorityResourceRecord : BaseResourceRecord
    {
        private static ResourceRecord Create(
            Domain domain,
            Domain master,
            Domain responsible,
            long serial,
            TimeSpan refresh,
            TimeSpan retry,
            TimeSpan expire,
            TimeSpan minTtl,
            TimeSpan ttl
        )
        {
            var data = new ByteStream(Options.SIZE + master.Size + responsible.Size);
            var tail = new Options()
            {
                SerialNumber = serial,
                RefreshInterval = refresh,
                RetryInterval = retry,
                ExpireInterval = expire,
                MinimumTimeToLive = minTtl,
            };

            data.Append(master.ToArray())
                .Append(responsible.ToArray())
                .Append(Struct.GetBytes(tail));

            return new ResourceRecord(domain, data.ToArray(), RecordType.SOA, RecordClass.IN, ttl);
        }

        public StartOfAuthorityResourceRecord(
            IResourceRecord record,
            byte[] message,
            int dataOffset
        )
            : base(record)
        {
            MasterDomainName = Domain.FromArray(message, dataOffset, out dataOffset);
            ResponsibleDomainName = Domain.FromArray(message, dataOffset, out dataOffset);

            if (dataOffset + Options.SIZE > message.Length)
                throw new ArgumentException(
                    "Message too short for StartOfAuthorityResourceRecord Options"
                );

            var tail = new Options()
            {
                SerialNumber = EndianAwareConverter.ToUInt32(
                    message,
                    Endianness.BigEndian,
                    (uint)dataOffset
                ),
                RefreshInterval = TimeSpan.FromSeconds(
                    EndianAwareConverter.ToUInt32(
                        message,
                        Endianness.BigEndian,
                        (uint)(dataOffset + 4)
                    )
                ),
                RetryInterval = TimeSpan.FromSeconds(
                    EndianAwareConverter.ToUInt32(
                        message,
                        Endianness.BigEndian,
                        (uint)(dataOffset + 8)
                    )
                ),
                ExpireInterval = TimeSpan.FromSeconds(
                    EndianAwareConverter.ToUInt32(
                        message,
                        Endianness.BigEndian,
                        (uint)(dataOffset + 12)
                    )
                ),
                MinimumTimeToLive = TimeSpan.FromSeconds(
                    EndianAwareConverter.ToUInt32(
                        message,
                        Endianness.BigEndian,
                        (uint)(dataOffset + 16)
                    )
                ),
            };

            SerialNumber = tail.SerialNumber;
            RefreshInterval = tail.RefreshInterval;
            RetryInterval = tail.RetryInterval;
            ExpireInterval = tail.ExpireInterval;
            MinimumTimeToLive = tail.MinimumTimeToLive;
        }

        public StartOfAuthorityResourceRecord(
            Domain domain,
            Domain master,
            Domain responsible,
            long serial,
            TimeSpan refresh,
            TimeSpan retry,
            TimeSpan expire,
            TimeSpan minTtl,
            TimeSpan ttl = default
        )
            : base(Create(domain, master, responsible, serial, refresh, retry, expire, minTtl, ttl))
        {
            MasterDomainName = master;
            ResponsibleDomainName = responsible;

            SerialNumber = serial;
            RefreshInterval = refresh;
            RetryInterval = retry;
            ExpireInterval = expire;
            MinimumTimeToLive = minTtl;
        }

        public StartOfAuthorityResourceRecord(
            Domain domain,
            Domain master,
            Domain responsible,
            Options options = default,
            TimeSpan ttl = default
        )
            : this(
                domain,
                master,
                responsible,
                options.SerialNumber,
                options.RefreshInterval,
                options.RetryInterval,
                options.ExpireInterval,
                options.MinimumTimeToLive,
                ttl
            ) { }

        public Domain MasterDomainName { get; }
        public Domain ResponsibleDomainName { get; }
        public long SerialNumber { get; }
        public TimeSpan RefreshInterval { get; }
        public TimeSpan RetryInterval { get; }
        public TimeSpan ExpireInterval { get; }
        public TimeSpan MinimumTimeToLive { get; }

        public override string ToString()
        {
            return Stringify()
                .Add("MasterDomainName", "ResponsibleDomainName", "SerialNumber")
                .ToString();
        }

        [Endian(Endianness.BigEndian)]
        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        public struct Options
        {
            public const int SIZE = 20;

            private uint serialNumber;
            private uint refreshInterval;
            private uint retryInterval;
            private uint expireInterval;
            private uint ttl;

            public long SerialNumber
            {
                readonly get { return serialNumber; }
                set { serialNumber = (uint)value; }
            }

            public TimeSpan RefreshInterval
            {
                readonly get { return TimeSpan.FromSeconds(refreshInterval); }
                set { refreshInterval = (uint)value.TotalSeconds; }
            }

            public TimeSpan RetryInterval
            {
                readonly get { return TimeSpan.FromSeconds(retryInterval); }
                set { retryInterval = (uint)value.TotalSeconds; }
            }

            public TimeSpan ExpireInterval
            {
                readonly get { return TimeSpan.FromSeconds(expireInterval); }
                set { expireInterval = (uint)value.TotalSeconds; }
            }

            public TimeSpan MinimumTimeToLive
            {
                readonly get { return TimeSpan.FromSeconds(ttl); }
                set { ttl = (uint)value.TotalSeconds; }
            }
        }
    }
}
