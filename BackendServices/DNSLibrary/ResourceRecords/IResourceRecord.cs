namespace DNSLibrary.ResourceRecords
{
    public interface IResourceRecord : IMessageEntry
    {
        TimeSpan TimeToLive { get; }
        int DataLength { get; }
        byte[] Data { get; }
    }
}
