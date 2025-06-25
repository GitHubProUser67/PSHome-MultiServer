namespace System
{
    public class UniqueIDGenerator
    {
        private uint UniqueIDCounter;

        public UniqueIDGenerator(uint startingValue = 0)
        {
            UniqueIDCounter = startingValue;
        }

        public uint CreateUniqueID()
        {
            return ++UniqueIDCounter;
        }

        public uint GetCurrentID()
        {
            return UniqueIDCounter;
        }
    }
}
