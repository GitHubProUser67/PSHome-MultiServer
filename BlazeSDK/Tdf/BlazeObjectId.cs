namespace Tdf
{
    public struct BlazeObjectId
    {
        public long Id { get; set; }
        public BlazeObjectType Type { get; set; }


        public BlazeObjectId()
        {
            Id = 0;
            Type = new BlazeObjectType();
        }

        public BlazeObjectId(BlazeObjectType type)
        {
            Id = 0;
            Type = type;
        }

        public BlazeObjectId(long id)
        {
            Id = id;
            Type = new BlazeObjectType();
        }

        public BlazeObjectId(long id, BlazeObjectType type)
        {
            Id = id;
            Type = type;
        }

        public BlazeObjectId(long id, ushort component, ushort type)
        {
            Id = id;
            Type = new BlazeObjectType(component, type);
        }


        //TODO: make sure the output order is correct, maybe in reality it is $"{Id}/{Type}"
        public override string ToString()
        {
            return $"{Type}/{Id}";
        }
    }
}
