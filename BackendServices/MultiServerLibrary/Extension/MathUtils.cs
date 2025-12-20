namespace MultiServerLibrary.Extension
{
    public static class MathUtils
    {
        public static string ToUuid(this int number)
        {
            return $"00000000-00000000-00000000-{number:D8}";
        }
    }
}
