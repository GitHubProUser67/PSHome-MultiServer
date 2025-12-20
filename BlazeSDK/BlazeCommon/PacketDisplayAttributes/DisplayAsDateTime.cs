namespace BlazeCommon.PacketDisplayAttributes
{
    public class DisplayAsDateTime : Attribute
    {
        public TimeFormat Format { get; set; }

        public DisplayAsDateTime(TimeFormat format)
        {
            Format = format;
        }
    }

    public enum TimeFormat
    {
        UnixSeconds,
        UnixMilliseconds,
        UnixMicroseconds
    }
}
