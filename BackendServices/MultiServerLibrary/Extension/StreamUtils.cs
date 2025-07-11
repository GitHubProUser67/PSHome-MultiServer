using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MultiServerLibrary.Extension
{
    public static class StreamUtils
    {
        public static string GetString(this Stream stream)
        {
            return Encoding.ASCII.GetString(((MemoryStream)stream).ToArray());
        }

        /// <summary>
        /// Copies a Stream to an other.
        /// <para>Copie d'un Stream � un autre.</para>
        /// </summary>
        /// <param name="input">The Stream to copy.</param>
        /// <param name="output">the Steam to copy to.</param>
        /// <param name="BufferSize">the buffersize for the copy.</param>
        public static void CopyStream(Stream input, Stream output, long BufferSize = 16 * 1024, bool ignore_errors = false)
        {
            if (BufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(BufferSize), "[StreamUtils] - CopyStream() - Buffer size must be greater than zero.");

            int bytesRead;
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            Span<byte> buffer = new byte[BufferSize];
            while ((bytesRead = input.Read(buffer)) > 0)
            {
                if (ignore_errors)
                {
                    try
                    {
                        output.Write(buffer[..bytesRead]);
                    }
                    catch { }
                }
                else
                    output.Write(buffer[..bytesRead]);
            }
#else
            byte[] buffer = new byte[BufferSize];
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                if (ignore_errors)
                {
                    try
                    {
                        output.Write(buffer, 0, bytesRead);
                    }
                    catch { }
                }
                else
                    output.Write(buffer, 0, bytesRead);
            }
#endif
        }

        /// <summary>
        /// Copies a specified number of bytes from one Stream to another.
        /// <para>Copie un nombre spécifié d'octets d'un Stream à un autre.</para>
        /// </summary>
        /// <param name="input">The Stream to copy from.</param>
        /// <param name="output">The Stream to copy to.</param>
        /// <param name="BufferSize">The buffer size to use for copying.</param>
        /// <param name="numOfBytes">The number of bytes to copy.</param>
        public static void CopyStream(Stream input, Stream output, int BufferSize, long numOfBytes, bool ignore_errors = false)
        {
            if (BufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(BufferSize), "[StreamUtils] - CopyStream() - Buffer size must be greater than zero.");
            else if (numOfBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(numOfBytes), "[StreamUtils] - CopyStream() - Number of bytes to copy must be non-negative.");
            else if (numOfBytes == 0)
                return;

            int bytesRead;
            long bytesCopied = 0;
#if NETCOREAPP2_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            Span<byte> buffer = new byte[BufferSize];
            while (bytesCopied < numOfBytes && (bytesRead = input.Read(buffer)) > 0)
            {
                int bytesToWrite = (int)Math.Min(bytesRead, numOfBytes - bytesCopied);
                if (ignore_errors)
                {
                    try
                    {
                        output.Write(buffer[..bytesToWrite]);
                    }
                    catch { }
                }
                else
                    output.Write(buffer[..bytesToWrite]);
                bytesCopied += bytesToWrite;
            }
#else
            byte[] buffer = new byte[BufferSize];
            while (bytesCopied < numOfBytes && (bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                int bytesToWrite = (int)Math.Min(bytesRead, numOfBytes - bytesCopied);
                if (ignore_errors)
                {
                    try
                    {
                        output.Write(buffer, 0, bytesToWrite);
                    }
                    catch { }
                }
                else
                    output.Write(buffer, 0, bytesToWrite);
                bytesCopied += bytesToWrite;
            }
#endif
        }

        /// <summary>
        /// Copies a Stream to an other.
        /// <para>Copie d'un Stream � un autre.</para>
        /// </summary>
        /// <param name="input">The Stream to copy.</param>
        /// <param name="output">the Steam to copy to.</param>
        /// <param name="BufferSize">the buffersize for the copy.</param>
        public static async Task CopyStreamAsync(Stream input, Stream output, long BufferSize, bool ignore_errors = false, CancellationToken token = default)
        {
            if (BufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(BufferSize), "[StreamUtils] - CopyStreamAsync() - Buffer size must be greater than zero.");

            int bytesRead;
            byte[] buffer = new byte[BufferSize];
            while ((bytesRead = await input.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false)) > 0)
            {
                if (ignore_errors)
                {
                    try
                    {
                        await output.WriteAsync(buffer, 0, bytesRead, token).ConfigureAwait(false);
                    }
                    catch { }
                }
                else
                    await output.WriteAsync(buffer, 0, bytesRead, token).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Copies a specified number of bytes from one Stream to another.
        /// <para>Copie un nombre spécifié d'octets d'un Stream à un autre.</para>
        /// </summary>
        /// <param name="input">The Stream to copy from.</param>
        /// <param name="output">The Stream to copy to.</param>
        /// <param name="BufferSize">The buffer size to use for copying.</param>
        /// <param name="numOfBytes">The number of bytes to copy.</param>
        public static async Task CopyStreamAsync(Stream input, Stream output, int BufferSize, long numOfBytes, bool ignore_errors = false, CancellationToken token = default)
        {
            if (BufferSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(BufferSize), "[StreamUtils] - CopyStreamAsync() - Buffer size must be greater than zero.");
            else if (numOfBytes < 0)
                throw new ArgumentOutOfRangeException(nameof(numOfBytes), "[StreamUtils] - CopyStreamAsync() - Number of bytes to copy must be non-negative.");
            else if (numOfBytes == 0)
                return;

            int bytesRead;
            long bytesCopied = 0;
            byte[] buffer = new byte[BufferSize];
            while (bytesCopied < numOfBytes && (bytesRead = await input.ReadAsync(buffer, 0, buffer.Length, token).ConfigureAwait(false)) > 0)
            {
                int bytesToWrite = (int)Math.Min(bytesRead, numOfBytes - bytesCopied);
                if (ignore_errors)
                {
                    try
                    {
                        await output.WriteAsync(buffer, 0, bytesToWrite, token).ConfigureAwait(false);
                    }
                    catch { }
                }
                else
                    await output.WriteAsync(buffer, 0, bytesToWrite, token).ConfigureAwait(false);
                bytesCopied += bytesToWrite;
            }
        }
    }
}
