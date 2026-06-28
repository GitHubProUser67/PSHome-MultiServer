namespace MultiServerLibrary.Extension
{
    public static class ArrayUtils
    {
        extension<T>(T[] array)
        {
            public T[] AddElementToArray(T newElement)
            {
                return [.. array, newElement];
            }

            public T[] RemoveElementFromArray(T elementToRemove)
            {
                return
                [
                    .. array.Where(item =>
                        !EqualityComparer<T>.Default.Equals(item, elementToRemove)
                    ),
                ];
            }
        }
    }
}
