using EndianTools;
using System.Collections;
using NetHasher.CRC;

namespace SSFWServer.ChannelID
{
    public class SIDKeyGenerator
    {
        private static SIDKeyGenerator? _instance;
        private static readonly int[,] _scatterTable = new int[16, 2]
        {
          {
            2,
            1
          },
          {
            2,
            9
          },
          {
            1,
            4
          },
          {
            11,
            3
          },
          {
            14,
            12
          },
          {
            10,
            5
          },
          {
            9,
            1
          },
          {
            3,
            7
          },
          {
            2,
            10
          },
          {
            1,
            4
          },
          {
            5,
            9
          },
          {
            5,
            13
          },
          {
            7,
            2
          },
          {
            6,
            7
          },
          {
            14,
            8
          },
          {
            1,
            8
          }
        };

        private static readonly int[,] _newerScatterTable = new int[16, 2]
        {
          {
            3,
            12
          },
          {
            8,
            6
          },
          {
            2,
            8
          },
          {
            4,
            5
          },
          {
            5,
            1
          },
          {
            4,
            10
          },
          {
            1,
            3
          },
          {
            11,
            5
          },
          {
            3,
            4
          },
          {
            5,
            6
          },
          {
            13,
            10
          },
          {
            7,
            5
          },
          {
            2,
            9
          },
          {
            3,
            9
          },
          {
            10,
            8
          },
          {
            4,
            10
          }
        };

        public static SIDKeyGenerator Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SIDKeyGenerator();

                return _instance;
            }
        }

        public SceneKey Generate(ushort SceneID)
        {
            byte[] bytes2 = BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseUshort(SceneID) : SceneID);
            byte[] bytes3 = SceneKey.New().GetBytes();
            int index1 = bytes3[0] & 15;
            bytes3[_scatterTable[index1, 0]] = bytes2[0];
            bytes3[_scatterTable[index1, 1]] = bytes2[1];
            byte[] numArray = new byte[16];
            new BitArray(bytes3).Xor(new BitArray(new SceneKey(new Guid("44E790BB-D88D-4d4f-9145-098931F62F7B")).GetBytes())).CopyTo(numArray, 0);
            numArray[15] = CRC8.Create(numArray, 0, 15);
            return new SceneKey(numArray);
        }

        public SceneKey GenerateNewerType(ushort SceneID)
        {
            byte[] bytes2 = BitConverter.GetBytes(!BitConverter.IsLittleEndian ? EndianUtils.ReverseUshort(SceneID) : SceneID);
            byte[] bytes3 = SceneKey.New().GetBytes();
            int index1 = bytes3[0] & 15;
            bytes3[_newerScatterTable[index1, 0]] = bytes2[0];
            bytes3[_newerScatterTable[index1, 1]] = bytes2[1];
            byte[] numArray = new byte[16];
            new BitArray(bytes3).Xor(new BitArray(new SceneKey(new byte[] { 0xB9, 0x20, 0x86, 0xBC, 0x3E, 0x8B, 0x4A, 0xDF, 0xA3, 0x01, 0x4D, 0xEE, 0x2F, 0xA3, 0xAB, 0x69 }).GetBytes())).CopyTo(numArray, 0);
            ushort crc = EndianUtils.ReverseUshort(CRC16.Create(numArray, 0, 14));
            numArray[14] = (byte)(crc >> 8);
            numArray[15] = (byte)crc;
            return new SceneKey(numArray);
        }

        public ushort ExtractSceneID(SceneKey Key)
        {
            byte[] numArray = new byte[16];
            new BitArray(Key.GetBytes()).Xor(new BitArray(new SceneKey(new Guid("44E790BB-D88D-4d4f-9145-098931F62F7B")).GetBytes())).CopyTo(numArray, 0);
            int index1 = numArray[0] & 15;
            ushort sceneID = (ushort)(numArray[_scatterTable[index1, 0]] | (uint)numArray[_scatterTable[index1, 1]] << 8);
            if (sceneID < ushort.MinValue || sceneID > ushort.MaxValue)
                throw new InvalidOperationException($"[SIDKeyGenerator] - Invalid SceneKey passed to the function!");
            return sceneID;
        }

        public ushort ExtractSceneIDNewerType(SceneKey Key)
        {
            byte[] numArray = new byte[16];
            new BitArray(Key.GetBytes()).Xor(new BitArray(new SceneKey(new byte[] { 0xB9, 0x20, 0x86, 0xBC, 0x3E, 0x8B, 0x4A, 0xDF, 0xA3, 0x01, 0x4D, 0xEE, 0x2F, 0xA3, 0xAB, 0x69 }).GetBytes())).CopyTo(numArray, 0);
            int index1 = numArray[0] & 15;
            ushort sceneID = (ushort)(numArray[_newerScatterTable[index1, 0]] | (uint)numArray[_newerScatterTable[index1, 1]] << 8);
            if (sceneID < ushort.MinValue || sceneID > ushort.MaxValue)
                throw new InvalidOperationException($"[SIDKeyGenerator] - Invalid Newer SceneKey passed to the function!");
            return sceneID;
        }

        public static void Verify(SceneKey Key)
        {
            byte[] data = !Key.ToString().Equals("00000000-0000-0000-0000-000000000000") ? Key.GetBytes() : throw new InvalidDataException($"[SIDKeyGenerator] - Invalid SceneKey passed to the function!");
            if (CRC8.Create(data, 0, 15) != data[15])
                throw new InvalidOperationException($"[SIDKeyGenerator] - Failed to verify SceneKey passed to the function!");
        }

        public static void VerifyNewerKey(SceneKey Key)
        {
            byte[] data = !Key.ToString().Equals("00000000-0000-0000-0000-000000000000") ? Key.GetBytes() : throw new InvalidDataException($"[SIDKeyGenerator] - Invalid Newer SceneKey passed to the function!");
            if (EndianUtils.ReverseUshort(CRC16.Create(data, 0, 14)) != (ushort)(data[14] << 8 | data[15]))
                throw new InvalidOperationException($"[SIDKeyGenerator] - Failed to verify Newer SceneKey passed to the function!");
        }
    }
}
