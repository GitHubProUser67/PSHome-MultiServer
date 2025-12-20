namespace Tdf
{
    [AttributeUsage(AttributeTargets.Struct, Inherited = true, AllowMultiple = true)]
    public class TdfStruct : Attribute
    {
        public bool HasData { get; private set; }
        
        public uint TdfId { get; private set; }

        public TdfStruct()
        {
            HasData = true;
            TdfId = 0;
        }

        public TdfStruct(bool hasData)
        {
            HasData = hasData;
            TdfId = 0;
        }
        public TdfStruct(uint tdfId)
        {
            TdfId = tdfId;
        }
    }
}
