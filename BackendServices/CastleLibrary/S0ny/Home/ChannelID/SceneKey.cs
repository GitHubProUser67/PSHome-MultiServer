namespace CastleLibrary.S0ny.Home.ChannelID
{
    public class SceneKey
    {
        private readonly byte[] _bytes = new byte[16];

        public SceneKey(string idString) => GetBigEndianNetworkBytes(new Guid(idString));

        public SceneKey(byte[] bytes)
        {
            Array.Copy(bytes, 0, _bytes, 0, 16);
        }

        public SceneKey(Guid guid) => GetBigEndianNetworkBytes(guid);

        public static SceneKey New() => new(Guid.NewGuid());

        public override string ToString()
        {
            return $"{_bytes[0]:x2}{_bytes[1]:x2}{_bytes[2]:x2}{_bytes[3]:x2}-" +
                   $"{_bytes[4]:x2}{_bytes[5]:x2}-" +
                   $"{_bytes[6]:x2}{_bytes[7]:x2}-" +
                   $"{_bytes[8]:x2}{_bytes[9]:x2}-" +
                   $"{_bytes[10]:x2}{_bytes[11]:x2}{_bytes[12]:x2}" +
                   $"{_bytes[13]:x2}{_bytes[14]:x2}{_bytes[15]:x2}";
        }

        public byte[] GetBytes() => (byte[])_bytes.Clone();

        private void GetBigEndianNetworkBytes(Guid guid)
        {
            guid.ToByteArray(true).CopyTo(_bytes);
        }
    }
}
