using DNSLibrary.Utils;

namespace DNSLibrary.ResourceRecords
{
    public abstract class BaseResourceRecord(IResourceRecord record) : IResourceRecord
    {
        private readonly IResourceRecord record = record;

        public Domain Name
        {
            get { return record.Name; }
        }

        public RecordType Type
        {
            get { return record.Type; }
        }

        public RecordClass Class
        {
            get { return record.Class; }
        }

        public TimeSpan TimeToLive
        {
            get { return record.TimeToLive; }
        }

        public int DataLength
        {
            get { return record.DataLength; }
        }

        public byte[] Data
        {
            get { return record.Data; }
        }

        public int Size
        {
            get { return record.Size; }
        }

        public byte[] ToArray()
        {
            return record.ToArray();
        }

        internal ObjectStringifier Stringify()
        {
            return ObjectStringifier
                .New(this)
                .Add(
                    nameof(Name),
                    nameof(Type),
                    nameof(Class),
                    nameof(TimeToLive),
                    nameof(DataLength)
                );
        }
    }
}
