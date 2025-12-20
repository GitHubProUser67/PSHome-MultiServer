using System.Collections.Generic;
using System.Linq;

namespace MultiServerLibrary.Extension
{
    public static class ArrayUtils
    {
        public static T[] AddElementToArray<T>(this T[] array, T newElement)
        {
            return array.Concat(new[] { newElement }).ToArray();
        }

        public static T[] RemoveElementFromArray<T>(this T[] array, T elementToRemove)
        {
            return array.Where(item => !EqualityComparer<T>.Default.Equals(item, elementToRemove))
                        .ToArray();
        }
    }
}
