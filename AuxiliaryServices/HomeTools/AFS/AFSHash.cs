using System;
using System.Text.RegularExpressions;
#if NETCOREAPP || NETSTANDARD1_0_OR_GREATER || NET40_OR_GREATER
using System.Threading.Tasks;
#endif
namespace HomeTools.AFS
{
    public class AFSHash
    {
        public AFSHash(string text)
        {
            m_source = text;
            ComputeHash(text);
        }

        public int Value
        {
            get
            {
                return m_hash;
            }
        }

        private void ComputeHash(string text)
        {
            int hash = 0;
#if NETCOREAPP || NETSTANDARD1_0_OR_GREATER || NET40_OR_GREATER
            int length = text.IndexOf('\0');
            if (length == -1)
                length = text.Length;

            int[] values = new int[length];

            // Preprocess text to values
            Parallel.For(0, length, i =>
            {
                char c = char.ToLower(text[i]);
                if (c == '\\')
                    c = '/';
                else if ((c + 0xbf) < 0x1a)
                    c = (char)(c + ' ');

                values[i] = Convert.ToInt32(c);
            });

            // Precompute powers of 37
            int[] powers = new int[length];
            powers[length - 1] = 1;
            for (int i = length - 2; i >= 0; i--)
                powers[i] = powers[i + 1] * 37;

            // Parallel sum of partial hashes
            object lockObj = new object();
            Parallel.For(0, length, () => 0, (i, state, local) =>
            {
                local += values[i] * powers[i];
                return local;
            },
            local =>
            {
                lock (lockObj)
                    hash += local;
            });
#else
            foreach (char ch in text.ToLower())
            {
                char c = ch;

                if (c == '\0')
                    break;
                if (c == '\\')
                    c = '/';
                else if ((c + 0xbf) < 0x1a)
                    c = (char)(c + ' ');

                hash = hash * 37 + Convert.ToInt32(c);
            }
#endif
            m_hash = hash;
        }

        public static string EscapeString(string TextContent)
        {
            string text = Regex.Replace(TextContent, "file:(\\/+)resource_root\\/build\\/", string.Empty, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "file:", string.Empty, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "///", string.Empty, RegexOptions.IgnoreCase);
            return Regex.Replace(text, "/", "\\", RegexOptions.IgnoreCase);
        }

        private int m_hash;

        private string m_source;
    }
}