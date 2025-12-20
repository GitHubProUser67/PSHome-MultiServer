namespace Tdf
{
    public struct BlazeObjectType
    {
        public ushort Component { get; set; }
        public ushort Type { get; set; }
        public BlazeObjectType()
        {
            Component = 0;
            Type = 0;
        }
        public BlazeObjectType(ushort component, ushort type)
        {
            Component = component;
            Type = type;
        }

        public override string ToString()
        {
            return $"{Component}/{Type}";
        }
    }
}
