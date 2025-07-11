/* From: https://github.com/InductiveComputerScience/base64/tree/master

MIT License

Copyright (c) 2018 InductiveComputerScience

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using static System.Math;

namespace System
{
    public class Decoded
    {
        public double[] data;
        public bool success;
        public char[] errorMessage;
    }

    public class Encoded
    {
        public char[] data;
        public bool success;
        public char[] errorMessage;
    }

    public class Stringx
    {
        public char[] str;
    }

    public class ManagedBase64
    {
        public static Decoded Decode(char[] data, bool trimPadding = false)
        {
            bool done;
            char[] data4;
            double quads, i, currentQuad, currentTriplet, padding;
            Decoded decoded3, decoded;
            double[] table;
            bool[] validTable;

            decoded = new Decoded();
            table = GetRevTable();
            /* Alloc 1*/
            validTable = GetValidTable();
            /* Alloc 3*/
            padding = 0d;
            if (data.Length > 1d)
            {
                if (data[(int)(data.Length - 1d)] == '=')
                    padding++;
            }
            /* TODO: Evaluate && left to right and stop if false?*/
            if (data.Length > 2d)
            {
                if (data[(int)(data.Length - 2d)] == '=')
                    padding++;
            }
            /* Fixup some issues with Python driven base64 encoded data */
            if (trimPadding && padding == 2)
            {
                char[] trimmedData = new char[data.Length - 1];
                Array.Copy(data, 0, trimmedData, 0, data.Length - 1);
                data = trimmedData;
                padding = 1d;
            }
            quads = data.Length / 4d;

            /* TODO: Require init?*/
            /* Init*/
            decoded.data = new double[(int)(quads * 3d - padding)];
            decoded.errorMessage = Array.Empty<char>();
            decoded.success = true;

            done = false;
            for (i = 0d; i < quads && !done; i++)
            {
                data4 = new char[4];
                currentQuad = i * 4d;
                currentTriplet = i * 3d;
                if (padding > 0d && (i + 1d) == quads)
                {
                    if (padding == 2d)
                    {
                        data4[0] = data[(int)(currentQuad + 0d)];
                        data4[1] = data[(int)(currentQuad + 1d)];
                        data4[2] = 'A';
                        data4[3] = 'A';
                    }
                    else if (padding == 1d)
                    {
                        data4[0] = data[(int)(currentQuad + 0d)];
                        data4[1] = data[(int)(currentQuad + 1d)];
                        data4[2] = data[(int)(currentQuad + 2d)];
                        data4[3] = 'A';
                    }
                }
                else
                {
                    data4[0] = data[(int)(currentQuad + 0d)];
                    data4[1] = data[(int)(currentQuad + 1d)];
                    data4[2] = data[(int)(currentQuad + 2d)];
                    data4[3] = data[(int)(currentQuad + 3d)];
                }
                decoded3 = Decode3table(data4, table, validTable);
                if (decoded3.success)
                {
                    if (padding > 0d && i + 1d == quads)
                    {
                        if (padding == 2d)
                            decoded.data[(int)(currentTriplet + 0d)] = decoded3.data[0];
                        else if (padding == 1d)
                        {
                            decoded.data[(int)(currentTriplet + 0d)] = decoded3.data[0];
                            decoded.data[(int)(currentTriplet + 1d)] = decoded3.data[1];
                        }
                    }
                    else
                    {
                        decoded.data[(int)(currentTriplet + 0d)] = decoded3.data[0];
                        decoded.data[(int)(currentTriplet + 1d)] = decoded3.data[1];
                        decoded.data[(int)(currentTriplet + 2d)] = decoded3.data[2];
                    }
                }
                else
                {
                    decoded.success = false;
                    decoded.errorMessage = decoded3.errorMessage;
                    done = true;
                }
                Delete(decoded3);
            }

            Delete(table);
            /* Unalloc 1*/
            return decoded;
        }

        public static Decoded Decode3(char[] data)
        {
            double[] table;
            bool[] validTable;
            Decoded decoded;

            table = GetRevTable();
            /* Alloc 1*/
            validTable = GetValidTable();
            /* Alloc 2*/
            decoded = Decode3table(data, table, validTable);

            Delete(validTable);
            /* Unalloc 2*/
            Delete(table);
            /* Unalloc 1*/
            return decoded;
        }

        public static Decoded Decode3table(char[] data, double[] table, bool[] validTable)
        {
            double i, valid;
            Decoded decoded;

            decoded = new Decoded
            {
                /* Alloc r*/
                success = false,
                data = new double[3],
                errorMessage = string.Empty.ToCharArray()
            };

            if (data.Length == 4d)
            {
                valid = 0d;
                for (i = 0d; i < 4d; i++)
                {
                    if (IsValidBase64characterTable(data[(int)(i)], validTable))
                        valid++;
                }
                if (valid == 4d)
                {
                    Decode3NoChecksTable(decoded, data, table);
                    decoded.success = true;
                }
                else
                {
                    decoded.errorMessage = "Invalid character in input string.".ToCharArray();
                    decoded.success = false;
                }
            }
            else
            {
                decoded.errorMessage = "There must be exactly four characters in the input string.".ToCharArray();
                decoded.success = false;
            }

            return decoded;
        }

        public static bool IsValidBase64characterTable(char c, bool[] validTable)
        {
            return validTable[(int)(c)];
        }

        public static void Decode3NoChecks(Decoded decoded, char[] data)
        {
            double[] table;

            table = GetRevTable();
            Decode3NoChecksTable(decoded, data, table);
        }

        public static void Decode3NoChecksTable(Decoded decoded, char[] data, double[] table)
        {
            double total, i, n, r;

            total = 0d;
            for (i = 0d; i < 4d; i++)
            {
                n = GetNumber(data[(int)(4d - i - 1d)], table);
                total += n * Pow(2d, i * 6d);
            }

            for (i = 0d; i < 3d; i++)
            {
                r = total % Pow(2d, 8d);
                decoded.data[(int)(3d - i - 1d)] = r;
                total = (total - r) / Pow(2d, 8d);
            }
        }

        public static double GetNumber(char c, double[] table)
        {
            return table[(int)(c)];
        }

        public static Encoded Encode(double[] data)
        {
            Encoded encoded, encoded3;
            double padding, triplets, i, currentTriplet, currentQuad;
            double[] data3;
            bool done;

            encoded = new Encoded();

            padding = 0d;
            if ((data.Length % 3d) == 1d)
                padding = 2d;
            if ((data.Length % 3d) == 2d)
                padding = 1d;
            triplets = Ceiling(data.Length / 3d);

            /* Init*/
            encoded.data = new char[(int)(triplets * 4d)];
            encoded.errorMessage = Array.Empty<char>();
            encoded.success = true;

            done = false;
            for (i = 0d; i < triplets && !done; i++)
            {
                data3 = new double[3];
                currentTriplet = i * 3d;
                currentQuad = i * 4d;
                if (padding > 0d && i + 1d == triplets)
                {
                    if (padding == 2d)
                    {
                        data3[0] = data[(int)(currentTriplet + 0d)];
                        data3[1] = 0d;
                        data3[2] = 0d;
                    }
                    else if (padding == 1d)
                    {
                        data3[0] = data[(int)(currentTriplet + 0d)];
                        data3[1] = data[(int)(currentTriplet + 1d)];
                        data3[2] = 0d;
                    }

                }
                else
                {
                    data3[0] = data[(int)(currentTriplet + 0d)];
                    data3[1] = data[(int)(currentTriplet + 1d)];
                    data3[2] = data[(int)(currentTriplet + 2d)];
                }
                encoded3 = Encode3(data3);
                if (encoded3.success)
                {
                    encoded.data[(int)(currentQuad + 0d)] = encoded3.data[0];
                    encoded.data[(int)(currentQuad + 1d)] = encoded3.data[1];
                    if (padding > 0d && i + 1d == triplets)
                    {
                        if (padding == 2d)
                        {
                            encoded.data[(int)(currentQuad + 2d)] = '=';
                            encoded.data[(int)(currentQuad + 3d)] = '=';
                        }
                        else if (padding == 1d)
                        {
                            encoded.data[(int)(currentQuad + 2d)] = encoded3.data[2];
                            encoded.data[(int)(currentQuad + 3d)] = '=';
                        }

                    }
                    else
                    {
                        encoded.data[(int)(currentQuad + 2d)] = encoded3.data[2];
                        encoded.data[(int)(currentQuad + 3d)] = encoded3.data[3];
                    }
                }
                else
                {
                    encoded.success = false;
                    encoded.errorMessage = encoded3.errorMessage;
                    done = true;
                }
                Delete(encoded3);
            }

            return encoded;
        }

        public static Encoded Encode3(double[] data)
        {
            Encoded encoded;
            double elementsVerified, i, e;
            bool isWithinBounds, isWhole;

            encoded = new Encoded
            {
                /* Init*/
                data = new char[4]
            };
            encoded.data[0] = 'A';
            encoded.data[1] = 'A';
            encoded.data[2] = 'A';
            encoded.data[3] = 'A';
            encoded.errorMessage = Array.Empty<char>();
            encoded.success = false;

            /* Check.*/
            if (data.Length == 3d)
            {
                elementsVerified = 0d;
                for (i = 0d; i < data.Length; i++)
                {
                    e = data[(int)(i)];
                    isWithinBounds = (e >= 0d) && (e < Pow(2d, 8d));
                    isWhole = (e % 1d) == 0d;
                    if (isWithinBounds && isWhole)
                        elementsVerified++;
                    else
                    {
                        encoded.errorMessage = "Input number is too high, too low or is not a whole number.".ToCharArray();
                        encoded.success = false;
                    }
                }
                if (elementsVerified == data.Length)
                {
                    Encode3NoChecks(encoded, data);
                    encoded.success = true;
                }
            }
            else
            {
                encoded.errorMessage = "Input must contain 3 numbers.".ToCharArray();
                encoded.success = false;
            }

            return encoded;
        }

        public static void Encode3NoChecks(Encoded encoded, double[] data)
        {
            double total, i, bit6;
            char c;

            total = 0d;
            for (i = 0d; i < data.Length; i++)
            {
                total += data[(int)(data.Length - i - 1d)] * Pow(2d, i * 8d);
            }

            for (i = 0d; i < 4d; i++)
            {
                bit6 = total % Pow(2d, 6d);
                total = (total - bit6) / Pow(2d, 6d);
                c = GetCharacter(bit6);
                encoded.data[(int)(4d - i - 1d)] = c;
            }
        }

        public static char GetCharacter(double bit6)
        {
            return GetTable()[(int)(bit6)];
        }

        public static char[] GetTable()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
            return chars.ToCharArray();
        }

        public static double[] GetRevTable()
        {
            char[] table;
            double i, max;
            double[] revTable;

            table = GetTable();
            max = GetMax(table) + 1d;
            revTable = new double[(int)(max + 1d)];

            for (i = 0d; i < table.Length; i++)
            {
                revTable[(int)(table[(int)(i)])] = i;
            }

            return revTable;
        }

        public static bool[] GetValidTable()
        {
            char[] table;
            double max, i;
            bool[] validTable;

            table = GetTable();
            max = GetMax(table) + 1d;
            validTable = new bool[(int)(max)];

            for (i = 0d; i < max; i++)
            {
                validTable[(int)(i)] = IsValidBase64character((char)(i));
            }

            return validTable;
        }

        public static double GetMax(char[] table)
        {
            double maxValue;
            double v;
            double i;

            maxValue = 0d;

            for (i = 0d; i < table.Length; i++)
            {
                v = table[(int)(i)];
                maxValue = Max(maxValue, v);
            }
            return maxValue;
        }

        public static bool IsValidBase64character(char c)
        {
            char[] table;
            bool isValid;
            double i;

            table = GetTable();
            isValid = false;

            for (i = 0d; i < table.Length; i++)
            {
                if (table[(int)(i)] == c)
                    isValid = true;
            }

            return isValid;
        }

        public static Stringx StringFrom(char[] src)
        {
            return new Stringx
            {
                str = src
            };
        }

        public static double[] StringToNumberArray(char[] stringx)
        {
            double i;
            double[] array;

            array = new double[(int)(stringx.Length)];

            for (i = 0d; i < stringx.Length; i++)
            {
                array[(int)(i)] = stringx[(int)(i)];
            }
            return array;
        }

        public static char[] NumberArrayToString(double[] array)
        {
            double i;
            char[] stringx;

            stringx = new char[(int)(array.Length)];

            for (i = 0d; i < array.Length; i++)
            {
                stringx[(int)(i)] = (char)(array[(int)(i)]);
            }
            return stringx;
        }

        public static bool StringsEqual(char[] data1, char[] data2)
        {
            bool equal;
            double nrEqual, i;

            equal = false;
            if (data1.Length == data2.Length)
            {
                nrEqual = 0d;
                for (i = 0d; i < data1.Length; i++)
                {
                    if (data1[(int)(i)] == data2[(int)(i)])
                        nrEqual++;
                }
                if (nrEqual == data1.Length)
                    equal = true;
            }
            else
                equal = false;

            return equal;
        }

        public static bool NumberArraysEqual(double[] data1, double[] data2)
        {
            bool equal;
            double nrEqual, i;

            equal = false;
            if (data1.Length == data2.Length)
            {
                nrEqual = 0d;
                for (i = 0d; i < data1.Length; i++)
                {
                    if (data1[(int)(i)] == data2[(int)(i)])
                        nrEqual++;
                }
                if (nrEqual == data1.Length)
                    equal = true;
            }
            else
                equal = false;

            return equal;
        }

#pragma warning disable IDE0060
        public static void Delete(object objectx)
#pragma warning restore IDE0060
        {
            // C# has garbage collection.
        }
    }
}