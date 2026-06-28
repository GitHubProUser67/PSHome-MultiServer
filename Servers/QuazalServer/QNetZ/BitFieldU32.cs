namespace QuazalServer.QNetZ
{
    public class BitFieldU32
    {
        public class BitFieldEntry
        {
            public byte start;
            public byte size;
            public string name;
            public uint word;

            public BitFieldEntry(byte s, byte l, string n, uint field = 0)
            {
                start = s;
                size = l;
                name = n;
                word = ExtractValue(field);
            }

            public uint ExtractValue(uint field)
            {
                var a = 32 - start - size;
                var b = 32 - size;
                var tmp = field << a;
                tmp >>= b;
                return tmp;
            }

            public uint InsertValue(uint field)
            {
                return InsertValue(field, word);
            }

            public uint InsertValue(uint field, uint value)
            {
                var a = 32 - start - size;
                var b = 32 - size;
                var mask = 0xFFFFFFFF << a;
                mask >>= b;
                var tmp = value & mask;
                mask <<= start;
                mask = ~mask;
                tmp <<= start;
                return (field & mask) | tmp;
            }
        }

        public List<BitFieldEntry> entries = new();

        public BitFieldU32(List<BitFieldEntry> e, uint data = 0)
        {
            entries = e;
            if (e == null)
                return;
            Update(data);
        }

        public void Update(uint data)
        {
            foreach (var entry in entries)
                entry.word = entry.ExtractValue(data);
        }

        public uint ToU32()
        {
            uint tmp = 0;
            foreach (var entry in entries)
                tmp = entry.InsertValue(tmp);
            return tmp;
        }
    }
}
