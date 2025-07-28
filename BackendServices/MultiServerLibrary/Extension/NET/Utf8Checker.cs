using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace System.Text
{
    /// https://stackoverflow.com/questions/54309810/how-to-detect-any-non-utf8-character-in-a-file-in-c
    ///
    /// <summary>
    /// Interface for checking for utf8.
    /// </summary>
    public interface IUtf8Checker
    {
        /// <summary>
        /// Check if file is utf8 encoded.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>true if utf8 encoded, otherwise false.</returns>
        bool Check(string fileName);

        /// <summary>
        /// Check if stream is utf8 encoded.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns>true if utf8 encoded, otherwise false.</returns>
        bool IsUtf8(Stream stream);

        /// <summary>
        /// Return a list of found errors of type of IErrorUtf8Checker
        /// </summary>
        /// <returns>List of errors found through the Check metod</returns>
        List<IErrorUtf8Checker> GetErrorList();
    }

    public interface IErrorUtf8Checker
    {

    }

    /// <summary>
    /// http://anubis.dkuug.dk/JTC1/SC2/WG2/docs/n1335
    /// 
    /// http://www.cl.cam.ac.uk/~mgk25/ucs/ISO-10646-UTF-8.html
    /// 
    /// http://www.unicode.org/versions/corrigendum1.html
    /// 
    /// http://www.ietf.org/rfc/rfc2279.txt
    /// 
    /// </summary>
    public class Utf8Checker : IUtf8Checker
    {
        // newLineArray = used to understand the new line sequence 
        private static byte[] newLineArray = new byte[2] { 13, 10 };
        private int line = 1;
        private byte[] lineArray = new byte[2] { 0, 0 };

        // used to keep trak of number of errors found into the file            
        private List<IErrorUtf8Checker> errorsList;

        public Utf8Checker()
        {
            errorsList = new List<IErrorUtf8Checker>();
        }

        public int getNumberOfErrors()
        {
            return errorsList.Count();
        }

        public bool Check(string fileName)
        {
            using (BufferedStream fstream = new BufferedStream(File.OpenRead(fileName)))
                return IsUtf8(fstream);
        }

        public int getLine()
        {
            return line;
        }

        public List<IErrorUtf8Checker> GetErrorList()
        {
            return errorsList;
        }

        /// <summary>
        /// Check if stream is utf8 encoded.
        /// Notice: stream is read completely in memory!
        /// </summary>
        /// <param name="stream">Stream to read from.</param>
        /// <returns>True if the whole stream is utf8 encoded.</returns>
        public bool IsUtf8(Stream stream)
        {
            int count = 4 * 1024;
            byte[] buffer;
            int read;
            while (true)
            {
                buffer = new byte[count];
                stream.Seek(0, SeekOrigin.Begin);
                read = stream.Read(buffer, 0, count);
                if (read < count)
                    break;
                buffer = null;
                count *= 2;
            }
            return IsUtf8(buffer, read);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public bool IsUtf8(byte[] buffer, int length)
        {
            int position = 0;
            int bytes = 0;
            bool ret = true;
            while (position < length)
            {
                if (!IsValid(buffer, position, length, ref bytes))
                {
                    ret = false;
                    errorsList.Add(new ErrorUtf8Checker(getLine(), buffer[position]));

                }
                position += bytes;
            }
            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public bool IsValid(byte[] buffer, int position, int length, ref int bytes)
        {
            if (length > buffer.Length)
                throw new ArgumentException("Invalid length");

            if (position > length - 1)
            {
                bytes = 0;
                return true;
            }

            byte ch = buffer[position];
            char ctest = (char)ch; // for debug  only

            this.detectNewLine(ch);

            if (ch <= 0x7F)
            {
                bytes = 1;
                return true;
            }

            if (ch >= 0xc2 && ch <= 0xdf)
            {
                if (position >= length - 2)
                {
                    bytes = 0;
                    return false;
                }
                if (buffer[position + 1] < 0x80 || buffer[position + 1] > 0xbf)
                {
                    //bytes = 0;
                    return false;
                }
                bytes = 2;
                return true;
            }

            if (ch == 0xe0)
            {
                if (position >= length - 3)
                {
                    //bytes = 0;
                    return false;
                }

                if (buffer[position + 1] < 0xa0 || buffer[position + 1] > 0xbf ||
                    buffer[position + 2] < 0x80 || buffer[position + 2] > 0xbf)
                {
                    //bytes = 0;
                    return false;
                }
                bytes = 3;
                return true;
            }


            if (ch >= 0xe1 && ch <= 0xef)
            {
                if (position >= length - 3)
                {
                    //bytes = 0;
                    return false;
                }

                if (buffer[position + 1] < 0x80 || buffer[position + 1] > 0xbf ||
                    buffer[position + 2] < 0x80 || buffer[position + 2] > 0xbf)
                {
                    //bytes = 0;
                    return false;
                }

                bytes = 3;
                return true;
            }

            if (ch == 0xf0)
            {
                if (position >= length - 4)
                {
                    //bytes = 0;
                    return false;
                }

                if (buffer[position + 1] < 0x90 || buffer[position + 1] > 0xbf ||
                    buffer[position + 2] < 0x80 || buffer[position + 2] > 0xbf ||
                    buffer[position + 3] < 0x80 || buffer[position + 3] > 0xbf)
                {
                    //bytes = 0;
                    return false;
                }

                bytes = 4;
                return true;
            }

            if (ch == 0xf4)
            {
                if (position >= length - 4)
                {
                    //bytes = 0;
                    return false;
                }

                if (buffer[position + 1] < 0x80 || buffer[position + 1] > 0x8f ||
                    buffer[position + 2] < 0x80 || buffer[position + 2] > 0xbf ||
                    buffer[position + 3] < 0x80 || buffer[position + 3] > 0xbf)
                {
                    //bytes = 0;
                    return false;
                }

                bytes = 4;
                return true;
            }

            if (ch >= 0xf1 && ch <= 0xf3)
            {
                if (position >= length - 4)
                {
                    //bytes = 0;
                    return false;
                }

                if (buffer[position + 1] < 0x80 || buffer[position + 1] > 0xbf ||
                    buffer[position + 2] < 0x80 || buffer[position + 2] > 0xbf ||
                    buffer[position + 3] < 0x80 || buffer[position + 3] > 0xbf)
                {
                    //bytes = 0;
                    return false;
                }

                bytes = 4;
                return true;
            }

            return false;
        }

        private void detectNewLine(byte ch)
        {
            // looking for second char for new line (char 13 feed)
            if (lineArray[0] == newLineArray[0])
            {
                if (ch == newLineArray[1])
                {
                    // found new line
                    lineArray[1] = ch;
                    line++;
                    // reset work array: lineArray
                    lineArray[1] = 0;
                }
                // we have to reset work array because CR(13)LF(10) must be in sequence
                lineArray[0] = 0;
            }
            else
            {
                // found first character (char 10 return)
                if (ch == newLineArray[0])
                    lineArray[0] = ch;
            }
        }
    }

    public class ErrorUtf8Checker : IErrorUtf8Checker
    {
        private int line;
        private byte ch;

        public ErrorUtf8Checker(int line, byte character)
        {
            this.line = line;
            ch = character;
        }

        public ErrorUtf8Checker(int line)
        {
            this.line = line;
        }

        public override string ToString()
        {
            string s;
            try
            {
                if (ch > 0)
                    s = "line: " + line + " code: " + ch + ", char: " + (char)ch;
                else
                    s = "line: " + line;
                return s;
            }
            catch
            {
                return base.ToString();
            }
        }
    }
}
