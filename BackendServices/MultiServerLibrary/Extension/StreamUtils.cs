namespace MultiServerLibrary.Extension
{
    public static class StreamUtils
    {
        /// <summary>
        /// Copies a Stream to an other.
        /// <para>Copie d'un Stream � un autre.</para>
        /// </summary>
        /// <param name="input">The Stream to copy.</param>
        /// <param name="output">the Steam to copy to.</param>
        /// <param name="BufferSize">the buffersize for the copy.</param>
        public static void CopyStream(
            Stream input,
            Stream output,
            long BufferSize = 16 * 1024,
            bool ignore_errors = false
        )
        {
            if (BufferSize <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(BufferSize),
                    "[StreamUtils] - CopyStream() - Buffer size must be greater than zero."
                );

            int bytesRead;
            Span<byte> buffer = new byte[BufferSize];
            while ((bytesRead = input.Read(buffer)) > 0)
            {
                if (ignore_errors)
                {
                    try
                    {
                        output.Write(buffer[..bytesRead]);
                    }
                    catch
                    {
                        // Not Important.
                    }
                }
                else
                    output.Write(buffer[..bytesRead]);
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
        public static void CopyStream(
            Stream input,
            Stream output,
            int BufferSize,
            long numOfBytes,
            bool ignore_errors = false
        )
        {
            if (BufferSize <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(BufferSize),
                    "[StreamUtils] - CopyStream() - Buffer size must be greater than zero."
                );
            else if (numOfBytes < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(numOfBytes),
                    "[StreamUtils] - CopyStream() - Number of bytes to copy must be non-negative."
                );

            int bytesRead;
            long bytesCopied = 0;
            Span<byte> buffer = new byte[BufferSize];
            while (bytesCopied < numOfBytes && (bytesRead = input.Read(buffer)) > 0)
            {
                var bytesToWrite = (int)Math.Min(bytesRead, numOfBytes - bytesCopied);
                if (ignore_errors)
                {
                    try
                    {
                        output.Write(buffer[..bytesToWrite]);
                    }
                    catch
                    {
                        // Not Important.
                    }
                }
                else
                    output.Write(buffer[..bytesToWrite]);
                bytesCopied += bytesToWrite;
            }
        }

        /// <summary>
        /// Copies a Stream to an other.
        /// <para>Copie d'un Stream � un autre.</para>
        /// </summary>
        /// <param name="input">The Stream to copy.</param>
        /// <param name="output">the Steam to copy to.</param>
        /// <param name="BufferSize">the buffersize for the copy.</param>
        public static async Task CopyStreamAsync(
            Stream input,
            Stream output,
            long BufferSize,
            bool ignore_errors = false,
            CancellationToken token = default
        )
        {
            if (BufferSize <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(BufferSize),
                    "[StreamUtils] - CopyStreamAsync() - Buffer size must be greater than zero."
                );

            int bytesRead;
            var buffer = new byte[BufferSize];
            while ((bytesRead = await input.ReadAsync(buffer, token).ConfigureAwait(false)) > 0)
            {
                if (ignore_errors)
                {
                    try
                    {
                        await output
                            .WriteAsync(buffer.AsMemory(0, bytesRead), token)
                            .ConfigureAwait(false);
                    }
                    catch
                    {
                        // Not Important.
                    }
                }
                else
                    await output
                        .WriteAsync(buffer.AsMemory(0, bytesRead), token)
                        .ConfigureAwait(false);
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
        public static async Task CopyStreamAsync(
            Stream input,
            Stream output,
            int BufferSize,
            long numOfBytes,
            bool ignore_errors = false,
            CancellationToken token = default
        )
        {
            if (BufferSize <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(BufferSize),
                    "[StreamUtils] - CopyStreamAsync() - Buffer size must be greater than zero."
                );
            else if (numOfBytes < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(numOfBytes),
                    "[StreamUtils] - CopyStreamAsync() - Number of bytes to copy must be non-negative."
                );

            int bytesRead;
            long bytesCopied = 0;
            var buffer = new byte[BufferSize];
            while (
                bytesCopied < numOfBytes
                && (bytesRead = await input.ReadAsync(buffer, token).ConfigureAwait(false)) > 0
            )
            {
                var bytesToWrite = (int)Math.Min(bytesRead, numOfBytes - bytesCopied);
                if (ignore_errors)
                {
                    try
                    {
                        await output
                            .WriteAsync(buffer.AsMemory(0, bytesToWrite), token)
                            .ConfigureAwait(false);
                    }
                    catch
                    {
                        // Not Important.
                    }
                }
                else
                    await output
                        .WriteAsync(buffer.AsMemory(0, bytesToWrite), token)
                        .ConfigureAwait(false);
                bytesCopied += bytesToWrite;
            }
        }
    }
}
