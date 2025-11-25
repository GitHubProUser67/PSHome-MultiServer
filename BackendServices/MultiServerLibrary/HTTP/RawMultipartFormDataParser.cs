using MultiServerLibrary.Extension;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MultiServerLibrary.HTTP
{
    // Fixup class for any silly API using raw bytes as a multipart parameter (yes AnsadaPhone, you made me do this foolish you...)
    public class RawMultipartField
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }
    }

    public static class RawMultipartFormDataParser
    {
        public static List<RawMultipartField> Parse(Stream stream, string boundary)
        {
            var fields = new List<RawMultipartField>();

            int nextBoundary;
            int pos = 0;

            byte[] body;
            byte[] boundaryBytes = Encoding.UTF8.GetBytes("--" + boundary);

            if (stream is MemoryStream ms)
                body = ms.ToArray();
            else
            {
                using MemoryStream tempMs = new MemoryStream();
                stream.CopyTo(tempMs);
                body = tempMs.ToArray();
            }

            // Iterate over each boundary occurrence
            while ((nextBoundary = ByteUtils.FindBytePattern(body, boundaryBytes, pos)) >= 0)
            {
                int start = nextBoundary + boundaryBytes.Length;

                if (start + 2 >= body.Length)
                    break;

                // Skip CRLF after boundary
                if (body[start] == (byte)'\r' && body[start + 1] == (byte)'\n')
                    start += 2;

                int headerEnd = ByteUtils.FindBytePattern(body, Encoding.UTF8.GetBytes("\r\n\r\n"), start);
                if (headerEnd < 0)
                    break;

                string header = Encoding.UTF8.GetString(body, start, headerEnd - start);

                var nameMatch = Regex.Match(header, "name=\"([^\"]+)\"");
                if (!nameMatch.Success)
                    break;

                string name = nameMatch.Groups[1].Value;

                int dataStart = headerEnd + 4;

                int nextBoundaryStart = ByteUtils.FindBytePattern(body, boundaryBytes, dataStart);
                if (nextBoundaryStart < 0)
                    nextBoundaryStart = body.Length;

                int dataEnd = nextBoundaryStart - 2;
                if (dataEnd < dataStart)
                    dataEnd = dataStart;

                byte[] data = new byte[dataEnd - dataStart];

                Array.Copy(body, dataStart, data, 0, data.Length);

                fields.Add(new RawMultipartField
                {
                    Name = name,
                    Data = data
                });

                pos = nextBoundaryStart;
            }

            return fields;
        }
    }
}
