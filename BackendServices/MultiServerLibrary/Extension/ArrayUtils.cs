using System.Linq;

namespace MultiServerLibrary.Extension
{
    public static class ArrayUtils
    {
        public static T[] AddElementToArray<T>(this T[] array, T newElement)
        {
            return array.Concat(new[] { newElement }).ToArray();
        }
    }
}
