namespace BlazeCommon.PacketDisplayAttributes
{
    public class DisplayAsDateTime(TimeFormat format) : Attribute
    {
        public TimeFormat Format { get; set; } = format;
    }

    public enum TimeFormat
    {
        UnixSeconds,
        UnixMilliseconds,
        UnixMicroseconds,
    }
}
