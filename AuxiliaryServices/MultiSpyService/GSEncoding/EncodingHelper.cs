using System;

namespace MultiSpyService.GSEncoding
{
    public class EncodingHelper
    {
        public static byte[] GenerateValidationKey()
        {
            long timestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks / 10000L;
            byte[] key = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                do
                {
                    timestamp = (timestamp * 214013L + 2531011L) & 0x7FL;
                }
                while (timestamp < 33L || timestamp >= 127L);
                key[i] = (byte)timestamp;
            }
            return key;
        }

        public static byte[] Decode(byte[] key, byte[] validate, byte[] data, long size)
        {
            if (key != null && validate != null && data != null && size >= 0L)
            {
                return DecodeInternal(key, validate, data, size, null);
            }
            return null;
        }

        private static byte[] DecodeInternal(byte[] state, byte[] key, byte[] data, long dataSize, EncodingData context)
        {
            byte[] encodingKey = new byte[261];
            byte[] currentKey = (context != null) ? context.EncodingKey : encodingKey;

            if (context == null || context.Start == 0L)
            {
                data = PrepareDataForDecoding(ref currentKey, ref state, key, ref data, ref dataSize, ref context);
                if (data == null)
                    return null;
            }

            if (context == null)
            {
                ProcessDecoding(ref currentKey, ref data, dataSize);
                return data;
            }

            if (context.Start != 0L)
            {
                byte[] remainingData = new byte[dataSize - context.Offset];
                Array.ConstrainedCopy(data, (int)context.Offset, remainingData, 0, (int)(dataSize - context.Offset));
                long processed = ProcessDecoding(ref currentKey, ref remainingData, dataSize - context.Offset);
                Array.ConstrainedCopy(remainingData, 0, data, (int)context.Offset, (int)(dataSize - context.Offset));
                context.Offset += processed;

                byte[] finalData = new byte[dataSize - context.Start];
                Array.ConstrainedCopy(data, (int)context.Start, finalData, 0, (int)(dataSize - context.Start));
                return finalData;
            }

            return null;
        }

        public static byte[] Encode(byte[] key, byte[] validate, byte[] data, long size)
        {
            byte[] combinedData = new byte[size + 23L];
            byte[] header = new byte[23];

            if (key != null && validate != null && data != null && size >= 0L)
            {
                int keyLength = key.Length;
                int validateLength = validate.Length;
                int seed = new Random().Next();

                for (int i = 0; i < header.Length; i++)
                {
                    seed = seed * 214013 + 2531011;
                    header[i] = (byte)((seed ^ key[i % keyLength] ^ validate[i % validateLength]) % 256);
                }

                header[0] = 235;
                header[1] = 0;
                header[2] = 0;
                header[8] = 228;

                for (long i = size - 1L; i >= 0L; i--)
                {
                    combinedData[header.Length + i] = data[i];
                }

                Array.Copy(header, combinedData, header.Length);
                size += header.Length;
                long combinedSize = size;

                byte[] encodedData = EncodeInternal(key, validate, combinedData, combinedSize, null);

                byte[] finalResult = new byte[encodedData.Length + header.Length];
                Array.Copy(header, 0, finalResult, 0, header.Length);
                Array.Copy(encodedData, 0, finalResult, header.Length, encodedData.Length);

                return finalResult;
            }

            return null;
        }

        private static byte[] EncodeInternal(byte[] state, byte[] key, byte[] data, long dataSize, EncodingData context)
        {
            byte[] encodingKey = new byte[261];
            byte[] currentKey = (context != null) ? context.EncodingKey : encodingKey;

            if (context == null || context.Start == 0L)
            {
                data = PrepareDataForDecoding(ref currentKey, ref state, key, ref data, ref dataSize, ref context);
                if (data == null)
                    return null;
            }

            if (context == null)
            {
                ProcessEncoding(ref currentKey, ref data, dataSize);
                return data;
            }

            if (context.Start != 0L)
            {
                byte[] remainingData = new byte[dataSize - context.Offset];
                Array.ConstrainedCopy(data, (int)context.Offset, remainingData, 0, (int)(dataSize - context.Offset));
                long processed = ProcessEncoding(ref currentKey, ref remainingData, dataSize - context.Offset);
                Array.ConstrainedCopy(remainingData, 0, data, (int)context.Offset, (int)(dataSize - context.Offset));
                context.Offset += processed;

                byte[] finalData = new byte[dataSize - context.Start];
                Array.ConstrainedCopy(data, (int)context.Start, finalData, 0, (int)(dataSize - context.Start));
                return finalData;
            }

            return null;
        }

        private static byte[] PrepareDataForDecoding(ref byte[] encodingKey, ref byte[] state, byte[] key, ref byte[] data, ref long dataSize, ref EncodingData context)
        {
            long headerSize = (data[0] ^ 0xEC) + 2;
            byte[] keyBytes = new byte[8];

            if (dataSize < headerSize)
                return null;

            long keyDataSize = data[headerSize - 1L] ^ 0xEA;
            if (dataSize < headerSize + keyDataSize)
                return null;

            Array.Copy(data, keyBytes, 8);
            byte[] payload = new byte[dataSize - headerSize];
            Array.ConstrainedCopy(data, (int)headerSize, payload, 0, (int)(dataSize - headerSize));

            MixKeys(ref state, ref key, ref keyBytes, payload, keyDataSize);

            Array.ConstrainedCopy(payload, 0, data, (int)headerSize, (int)(dataSize - headerSize));
            dataSize -= headerSize;

            if (context == null)
            {
                byte[] trimmedData = new byte[dataSize];
                Array.ConstrainedCopy(data, (int)headerSize, trimmedData, 0, (int)dataSize);
                return trimmedData;
            }
            else
            {
                context.Offset = headerSize;
                context.Start = headerSize;
            }

            return data;
        }

        private static void MixKeys(ref byte[] state, ref byte[] key, ref byte[] keyBytes, byte[] payload, long keyDataSize)
        {
            long validateSize = key.Length;
            for (long i = 0; i < keyDataSize; i++)
            {
                keyBytes[(key[i % validateSize] * i) & 7L] ^= (byte)(keyBytes[i & 7L] ^ payload[i]);
            }

            long keyBytesSize = 8L;
            InitializeState(ref state, ref keyBytes, ref keyBytesSize);
        }

        private static void InitializeState(ref byte[] state, ref byte[] keyBytes, ref long keySize)
        {
            long pos = 0L;
            long offset = 0L;

            if (keySize >= 1L)
            {
                for (long i = 0; i <= 255L; i++)
                {
                    state[i] = (byte)i;
                }

                for (long i = 255L; i >= 0L; i--)
                {
                    byte swapIndex = (byte)NextKeyIndex(state, i, keyBytes, keySize, ref pos, ref offset);
                    (state[i], state[swapIndex]) = (state[swapIndex], state[i]);
                }

                state[256] = state[1];
                state[257] = state[3];
                state[258] = state[5];
                state[259] = state[7];
                state[260] = state[pos & 0xFF];
            }
        }

        private static long NextKeyIndex(byte[] state, long index, byte[] keyBytes, long keySize, ref long pos, ref long offset)
        {
            long count = 0L;
            long mask = 1L;
            if (index == 0L) return 0L;
            if (index > 1L)
            {
                while (mask < index)
                    mask = (mask << 1) + 1L;
            }

            long result;
            do
            {
                pos = state[pos & 0xFF] + keyBytes[offset];
                offset++;
                if (offset >= keySize)
                {
                    offset = 0L;
                    pos += keySize;
                }
                count++;
                result = (count <= 11L) ? (pos & mask) : (pos & (mask % index));
            }
            while (result > index);

            return result;
        }

        private static long ProcessDecoding(ref byte[] state, ref byte[] data, long dataSize)
        {
            for (long i = 0; i < dataSize; i++)
            {
                data[i] = DecodeByte(ref state, data[i]);
            }
            return dataSize;
        }

        private static long ProcessEncoding(ref byte[] state, ref byte[] data, long dataSize)
        {
            for (long i = 0; i < dataSize; i++)
            {
                data[i] = EncodeByte(ref state, data[i]);
            }
            return dataSize;
        }

        private static byte DecodeByte(ref byte[] state, byte dataByte)
        {
            int num = state[256];
            int num2 = state[257];
            int num3 = state[num];
            state[256] = (byte)((num + 1) % 256);
            state[257] = (byte)((num2 + num3) % 256);
            num = state[260];
            num2 = state[257];
            num2 = state[num2];
            num3 = state[num];
            state[num] = (byte)num2;
            num = state[259];
            num2 = state[257];
            num = state[num];
            state[num2] = (byte)num;
            num = state[256];
            num2 = state[259];
            num = state[num];
            state[num2] = (byte)num;
            num = state[256];
            state[num] = (byte)num3;
            num2 = state[258];
            num = state[num3];
            num3 = state[259];
            num2 = (num2 + num) % 256;
            state[258] = (byte)num2;
            num = num2;
            num3 = state[num3];
            num2 = state[257];
            num2 = state[num2];
            num = state[num];
            num3 = (num3 + num2) % 256;
            num2 = state[260];
            num2 = state[num2];
            num3 = (num3 + num2) % 256;
            num2 = state[num3];
            num3 = state[256];
            num3 = state[num3];
            num = (num + num3) % 256;
            num3 = state[num2];
            num2 = state[num];
            state[260] = dataByte;
            num3 = (num3 ^ num2 ^ dataByte) % 256;
            state[259] = (byte)num3;
            return (byte)num3;
        }

        private static byte EncodeByte(ref byte[] state, byte dataByte)
        {
            int num = state[256];
            int num2 = state[257];
            int num3 = state[num];
            state[256] = (byte)((num + 1) % 256);
            state[257] = (byte)((num2 + num3) % 256);
            num = state[260];
            num2 = state[257];
            num2 = state[num2];
            num3 = state[num];
            state[num] = (byte)num2;
            num = state[259];
            num2 = state[257];
            num = state[num];
            state[num2] = (byte)num;
            num = state[256];
            num2 = state[259];
            num = state[num];
            state[num2] = (byte)num;
            num = state[256];
            state[num] = (byte)num3;
            num2 = state[258];
            num = state[num3];
            num3 = state[259];
            num2 = (num2 + num) % 256;
            state[258] = (byte)num2;
            num = num2;
            num3 = state[num3];
            num2 = state[257];
            num2 = state[num2];
            num = state[num];
            num3 = (num3 + num2) % 256;
            num2 = state[260];
            num2 = state[num2];
            num3 = (num3 + num2) % 256;
            num2 = state[num3];
            num3 = state[256];
            num3 = state[num3];
            num = (num + num3) % 256;
            num3 = state[num2];
            num2 = state[num];
            num3 = (num3 ^ num2 ^ dataByte) % 256;
            state[260] = (byte)num3;
            state[259] = dataByte;
            return (byte)num3;
        }
    }
}
