// Adapted from: https://github.com/RPCS3/rpcs3/blob/master/rpcs3/Crypto/lz.cpp
// Copyright (C) 2014       Hykem <hykem@hotmail.com>
// Licensed under the terms of the GNU GPL, version 2.0 or later versions.
// http://www.gnu.org/licenses/gpl-2.0.txt

using EndianTools;
using System;

namespace HomeTools.PS3_Creator
{
    public static class LZ
    {
        private static unsafe void DecodeRange(uint* range, uint* code, byte** src)
        {
            if (((*range) >> 24) == 0)
            {
                *range <<= 8;
                *code = ((*code) << 8) + (*src)++[5];
            }
        }

        private static unsafe int DecodeBit(uint* range, uint* code, int* index, byte** src, byte* c)
        {
            DecodeRange(range, code, src);

            uint val = ((*range) >> 8) * (*c);

            *c -= (byte)((*c) >> 3);
            if (index != null)
                *index <<= 1;

            if (*code < val)
            {
                *range = val;
                *c += 31;
                if (index != null) 
                    (*index)++;
                return 1;
            }
            else
            {
                *code -= val;
                *range -= val;
                return 0;
            }
        }

        private static unsafe int DecodeNumber(byte* ptr, int index, int* bitFlag,
                                               uint* range, uint* code, byte** src)
        {
            int i = 1;

            if (index >= 3)
            {
                DecodeBit(range, code, &i, src, ptr + 0x18);
                if (index >= 4)
                {
                    DecodeBit(range, code, &i, src, ptr + 0x18);
                    if (index >= 5)
                    {
                        DecodeRange(range, code, src);
                        for (; index >= 5; index--)
                        {
                            i <<= 1;
                            *range >>= 1;
                            if (*code < *range)
                                i++;
                            else
                                *code -= *range;
                        }
                    }
                }
            }

            *bitFlag = DecodeBit(range, code, &i, src, ptr);

            if (index >= 1)
            {
                DecodeBit(range, code, &i, src, ptr + 0x8);
                if (index >= 2)
                    DecodeBit(range, code, &i, src, ptr + 0x10);
            }

            return i;
        }

        private static unsafe int DecodeWord(byte* ptr, int index, int* bitFlag,
                                             uint* range, uint* code, byte** src)
        {
            int i = 1;
            index /= 8;

            if (index >= 3)
            {
                DecodeBit(range, code, &i, src, ptr + 4);
                if (index >= 4)
                {
                    DecodeBit(range, code, &i, src, ptr + 4);
                    if (index >= 5)
                    {
                        DecodeRange(range, code, src);
                        for (; index >= 5; index--)
                        {
                            i <<= 1;
                            *range >>= 1;
                            if (*code < *range)
                                i++;
                            else
                                *code -= *range;
                        }
                    }
                }
            }

            *bitFlag = DecodeBit(range, code, &i, src, ptr);

            if (index >= 1)
            {
                DecodeBit(range, code, &i, src, ptr + 1);
                if (index >= 2)
                    DecodeBit(range, code, &i, src, ptr + 2);
            }

            return i;
        }

        public static unsafe int Decompress(byte[] output, byte[] input, uint size)
        {
            int result;

            fixed (byte* @out = output)
            fixed (byte* @in = input)
            {
                byte* start = @out;
                byte* end = @out + size;

                byte head = @in[0];

                uint range = uint.MaxValue;
                uint code = EndianAwareConverter.ToUInt32(@in, Endianness.BigEndian, 1);

                if (head > 0x80)
                {
                    result = -1;
                    if (code <= size)
                    {
                        Buffer.BlockCopy(input, 5, output, 0, (int)code);
                        result = (int)(start - @out);
                    }
                }
                else
                {
                    byte prev = 0;
                    int offset = 0;
                    int bit_flag = 0;
                    int data_length = 0;
                    int data_offset = 0;
                    byte[] tmpBytes = new byte[0xCC8];

                    byte* tmp_sect1;
                    byte* tmp_sect2;
                    byte* tmp_sect3;
                    byte* buf_start;
                    byte* buf_end;

                    // Set up a temporary buffer (sliding window).
                    Array.Fill(tmpBytes, (byte)0x80, 0, 0xCA8);

                    fixed (byte* tmp = tmpBytes)
                    {
                        while (true)
                        {
                            // Start reading at 0xB68.
                            tmp_sect1 = tmp + offset + 0xB68;

                            if (DecodeBit(&range, &code, null, &@in, tmp_sect1) == 0)
                            {
                                // Adjust offset and check for stream end.
                                if (offset > 0) offset--;
                                if (start == end)
                                    return (int)(start - @out);

                                // Locate first section.
                                int sect = (((((((int)(start - @out)) &7) << 8) +prev) >> head) &7) *((int)byte.MaxValue) - 1;

                                tmp_sect1 = tmp + sect;
                                int index = 1;

                                do
                                {
                                    DecodeBit(&range, &code, &index, &@in, tmp_sect1 + index);
                                } while ((index >> 8) == 0);

                                *start++ = (byte)index;
                            }
                            else // Compressed char stream.
                            {
                                int index = -1;

                                // Identify the data length bit field.
                                do
                                {
                                    tmp_sect1 += 8;
                                    bit_flag = DecodeBit(&range, &code, null, &@in, tmp_sect1);
                                    index += bit_flag;
                                }
                                while ((bit_flag != 0) && (index < 6)) ;

                                int b_size = 0x160;
                                tmp_sect2 = tmp + index + 0x7F1;

                                if ((index >= 0) || (bit_flag != 0))
                                {
                                    // Locate next section.
                                    int sect = (index << 5) | (((((int)(start - @out)) << index) &3) << 3) | (offset & 7);

                                    tmp_sect1 = tmp + 0xBA8 + sect;

                                    data_length = DecodeNumber(tmp_sect1, index, &bit_flag, &range, &code, &@in);
                                    if (data_length == (int)byte.MaxValue)
                                        return (int)(start - @out);
                                }
                                else // Assume one byte of advance.
                                    data_length = 1;

                                // If we got valid parameters, seek to find data offset.
                                if (data_length <= 2)
                                {
                                    tmp_sect2 += 0xF8;
                                    b_size = 0x40;  // Block size is now 0x40.
                                }

                                int diff = 0;
                                int shift = 1;

                                do
                                {
                                    diff = (shift << 4) - b_size;
                                    bit_flag = DecodeBit(&range, &code, &shift, &@in, tmp_sect2 + (shift << 3));
                                }
                                while (diff < 0);

                                if ((diff > 0) || (bit_flag != 0))
                                {
                                    // Adjust diff if needed.
                                    if (bit_flag == 0) diff -= 8;

                                    // Locate section.
                                    tmp_sect3 = tmp + 0x928 + diff;

                                    // Decode the data offset (1 bit fields).
                                    data_offset = DecodeWord(tmp_sect3, diff, &bit_flag, &range, &code, &@in);
                                }
                                else // Assume one byte of advance.
                                    data_offset = 1;

                                buf_start = start - data_offset;
                                buf_end = start + data_length + 1;

                                // Underflow.
                                if (buf_start < @out)
					                return -1;

                                // Overflow.
                                if (buf_end > end)
                                    return -1;

                                // Update offset.
                                offset = ((((int)(buf_end - @out)) +1) &1) +6;

                                // Copy data.
                                do
                                {
                                    *start++ = *buf_start++;
                                } 
                                while (start < buf_end);
                            }

                            prev = *(start - 1);
                        }
                    }
                }
            }

            return result;
        }
    }
}
