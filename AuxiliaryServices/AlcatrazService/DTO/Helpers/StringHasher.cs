using System.Text;
using System;

public static class StringHasher
{
	public static uint StringId(string str)
	{
		// Uses FNV-1a 32 as the base hash function
		const uint prime = 0x1000193;
		uint hash = 0x811C9DC5;

		// Convert the string to bytes in UTF-16 (each char is 2 bytes)
		byte[] data = Encoding.Unicode.GetBytes(str);
		int len = data.Length;

		for (int i = 0; i < len; i += 4)
		{
			uint value = 0;
			int end = Math.Min(i + 4, len);

			for (int j = i; j < end; ++j)
			{
				uint chr = data[j];
				value |= chr << ((j - i) * 8);
			}

			hash = (hash ^ value) * prime;
		}

		return hash;
	}
}