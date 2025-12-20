using System.Text;

namespace SSFWServer.ChannelID
{
    public class SceneKey
    {
        private byte[] _a = new byte[4];
        private byte[] _b = new byte[2];
        private byte[] _c = new byte[2];
        private byte[] _d = new byte[2];
        private byte[] _e = new byte[6];

        public SceneKey(string idString) => ConvertFromGUID(new Guid(idString));

        public SceneKey(byte[] bytes)
        {
            Array.Copy(bytes, 0, _a, 0, 4);
            Array.Copy(bytes, 4, _b, 0, 2);
            Array.Copy(bytes, 6, _c, 0, 2);
            Array.Copy(bytes, 8, _d, 0, 2);
            Array.Copy(bytes, 10, _e, 0, 6);
        }

        public SceneKey(Guid guid) => ConvertFromGUID(guid);

        public static SceneKey New() => new SceneKey(Guid.NewGuid());

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendFormat("{0:x2}{1:x2}{2:x2}{3:x2}-", _a[0], _a[1], _a[2], _a[3]);
            stringBuilder.AppendFormat("{0:x2}{1:x2}-", _b[0], _b[1]);
            stringBuilder.AppendFormat("{0:x2}{1:x2}-", _c[0], _c[1]);
            stringBuilder.AppendFormat("{0:x2}{1:x2}-", _d[0], _d[1]);
            stringBuilder.AppendFormat("{0:x2}{1:x2}{2:x2}{3:x2}{4:x2}{5:x2}", _e[0], _e[1], _e[2], _e[3], _e[4], _e[5]);
            return stringBuilder.ToString();
        }

        public byte[] GetBytes()
        {
            byte[] bytes = new byte[16];
            _a.CopyTo(bytes, 0);
            _b.CopyTo(bytes, 4);
            _c.CopyTo(bytes, 6);
            _d.CopyTo(bytes, 8);
            _e.CopyTo(bytes, 10);
            return bytes;
        }

        private static byte[] ReverseBytes(byte[] sourceBytes)
        {
            byte[] numArray = new byte[sourceBytes.Length];
            for (int index = 0; index < sourceBytes.Length; ++index)
                numArray[index] = sourceBytes[sourceBytes.Length - index - 1];
            return numArray;
        }

        private void ConvertFromGUID(Guid sourceGUID)
        {
            byte[] byteArray = sourceGUID.ToByteArray();
            Array.Copy(byteArray, 0, _a, 0, 4);
            Array.Copy(byteArray, 4, _b, 0, 2);
            Array.Copy(byteArray, 6, _c, 0, 2);
            Array.Copy(byteArray, 8, _d, 0, 2);
            Array.Copy(byteArray, 10, _e, 0, 6);
            _a = ReverseBytes(_a);
            _b = ReverseBytes(_b);
            _c = ReverseBytes(_c);
        }
    }
}