namespace Blaze2SDK.Blaze.Util
{
    public enum FilterResult : int
    {
        FILTER_RESULT_PASSED = 0x0,
        FILTER_RESULT_OFFENSIVE = 0x1,
        FILTER_RESULT_UNPROCESSED = 0x2,
        FILTER_RESULT_STRING_TOO_LONG = 0x3,
        FILTER_RESULT_OTHER = 0x4,
    }
}
