namespace Tdf
{
    public interface ITdfEncoder
    {
        byte[] Encode<T>(T obj) where T : notnull;
        byte[] Encode(object obj);

        void WriteTo<T>(Stream stream, T obj) where T : notnull;
        void WriteTo(Stream stream, object obj);
    }
}
