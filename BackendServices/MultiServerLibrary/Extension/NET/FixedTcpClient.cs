using System.Net.Sockets;

namespace MultiServerLibrary.Extension.NET
{
    /// <summary>
    /// Result of a read operation.
    /// </summary>
    public class ReadResult
    {
        /// <summary>
        /// Status of the read operation.
        /// </summary>
        public ReadResultStatus Status = ReadResultStatus.Success;

        /// <summary>
        /// Number of bytes read.
        /// </summary>
        public long BytesRead;

        /// <summary>
        /// Stream containing data.
        /// </summary>
        public Stream DataStream = null;

        /// <summary>
        /// Byte data from the stream.  Using this property will fully read the data stream and it will no longer be readable.
        /// </summary>
        public byte[] Data
        {
            get
            {
                if (_Data != null)
                {
                    return _Data;
                }
                else
                {
                    if (BytesRead > 0 && DataStream != null && DataStream.CanRead)
                    {
                        if (DataStream is MemoryStream ms)
                            _Data = ms.ToArray();
                        else
                        {
                            using (MemoryStream ms1 = new MemoryStream())
                            {
                                StreamUtils.CopyStream(DataStream, ms1);
                                _Data = ms1.ToArray();
                            }
                        }
                        return _Data;
                    }

                    return null;
                }
            }
        }

        private byte[] _Data = null;

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public ReadResult() { }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="status">Status of the read operation.</param>
        /// <param name="bytesRead">Number of bytes read.</param>
        /// <param name="data">Stream containing data.</param>
        public ReadResult(ReadResultStatus status, long bytesRead, MemoryStream data)
        {
            Status = status;
            BytesRead = bytesRead;
            DataStream = data;
        }
    }

    /// <summary>
    /// Read result status.
    /// </summary>
    public enum ReadResultStatus
    {
        /// <summary>
        /// The requested client was not found (only applicable for server read requests).
        /// </summary>
        ClientNotFound,

        /// <summary>
        /// The read operation was successful.
        /// </summary>
        Success,

        /// <summary>
        /// The operation timed out (reserved for future use).
        /// </summary>
        Timeout,

        /// <summary>
        /// The connection was lost.
        /// </summary>
        Disconnected,

        /// <summary>
        /// The request was canceled.
        /// </summary>
        Canceled,
    }

    /// <summary>
    /// Result of a write operation.
    /// </summary>
    public class WriteResult
    {
        /// <summary>
        /// Status of the write operation.
        /// </summary>
        public WriteResultStatus Status = WriteResultStatus.Success;

        /// <summary>
        /// Number of bytes written.
        /// </summary>
        public long BytesWritten;

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        public WriteResult() { }

        /// <summary>
        /// Instantiate the object.
        /// </summary>
        /// <param name="status">Status of the write operation.</param>
        /// <param name="bytesWritten">Number of bytes written.</param>
        public WriteResult(WriteResultStatus status, long bytesWritten)
        {
            Status = status;
            BytesWritten = bytesWritten;
        }
    }

    /// <summary>
    /// Write result status.
    /// </summary>
    public enum WriteResultStatus
    {
        /// <summary>
        /// The requested client was not found (only applicable for server read requests).
        /// </summary>
        ClientNotFound,

        /// <summary>
        /// The write operation was successful.
        /// </summary>
        Success,

        /// <summary>
        /// The operation timed out (reserved for future use).
        /// </summary>
        Timeout,

        /// <summary>
        /// The connection was lost.
        /// </summary>
        Disconnected,

        /// <summary>
        /// The request was canceled.
        /// </summary>
        Canceled,
    }

    public class FixedTcpClient : IDisposable
    {
        private TcpClient _client;
        private Stream _clientStream = null;

        private readonly CancellationToken _Token;

        private readonly SemaphoreSlim _WriteSemaphore = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _ReadSemaphore = new SemaphoreSlim(1, 1);

        public FixedTcpClient()
        {
            _client = new TcpClient();
            _Token = new CancellationTokenSource().Token;
        }

        public FixedTcpClient(TcpClient client, CancellationToken token)
        {
            _client = client;
            _Token = token;
        }

        public TcpClient Client => _client;

        public Stream ClientStream
        {
            get { return _clientStream; }
            set { _clientStream = value; }
        }

        public void Dispose()
        {
            if (_clientStream != null)
            {
                try
                {
                    _clientStream.Close();
                }
                catch
                {
                    // Not Important.
                }
            }
            try
            {
                _client.Close();
                _client.Dispose();
                _client = null;
            }
            catch
            {
                // Not Important.
            }
            GC.SuppressFinalize(this);
        }

        public async Task<ReadResult> ReadAsync(
            int milisecondsDelay,
            long count,
            int bufferSize,
            CancellationToken token = default
        )
        {
            if (token == default)
                token = _Token;

            return await ReadInternalAsync(milisecondsDelay, count, bufferSize, token)
                .ConfigureAwait(false);
        }

        public async Task<ReadResult> ReadAsync(
            long count,
            int bufferSize,
            CancellationToken token = default
        )
        {
            if (token == default)
                token = _Token;

            return await ReadInternalAsync(-1, count, bufferSize, token).ConfigureAwait(false);
        }

        public async Task<WriteResult> SendAsync(
            int milisecondsDelay,
            byte[] data,
            int bufferSize,
            CancellationToken token = default
        )
        {
            if (data == null || data.Length < 1)
                data = Array.Empty<byte>();

            using (MemoryStream ms = new MemoryStream())
            {
                await ms.WriteAsync(data, token).ConfigureAwait(false);
                ms.Seek(0, SeekOrigin.Begin);

                return await SendAsync(milisecondsDelay, data.Length, bufferSize, ms, token)
                    .ConfigureAwait(false);
            }
        }

        public async Task<WriteResult> SendAsync(
            byte[] data,
            int bufferSize,
            CancellationToken token = default
        )
        {
            if (data == null || data.Length < 1)
                data = Array.Empty<byte>();

            using (MemoryStream ms = new MemoryStream())
            {
                await ms.WriteAsync(data, token).ConfigureAwait(false);
                ms.Seek(0, SeekOrigin.Begin);

                return await SendAsync(-1, data.Length, bufferSize, ms, token)
                    .ConfigureAwait(false);
            }
        }

        public async Task<WriteResult> SendAsync(
            int milisecondsDelay,
            long contentLength,
            int bufferSize,
            Stream stream,
            CancellationToken token = default
        )
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (!stream.CanRead)
                throw new InvalidOperationException(
                    "[FixedTcpClient] - Cannot read from supplied stream."
                );
            else if (token == default)
                token = _Token;

            return await SendInternalAsync(
                    milisecondsDelay,
                    contentLength,
                    bufferSize,
                    stream,
                    token
                )
                .ConfigureAwait(false);
        }

        public async Task<WriteResult> SendAsync(
            long contentLength,
            int bufferSize,
            Stream stream,
            CancellationToken token = default
        )
        {
            ArgumentNullException.ThrowIfNull(stream);
            if (!stream.CanRead)
                throw new InvalidOperationException(
                    "[FixedTcpClient] - Cannot read from supplied stream."
                );
            else if (token == default)
                token = _Token;

            return await SendInternalAsync(-1, contentLength, bufferSize, stream, token)
                .ConfigureAwait(false);
        }

        private async Task<WriteResult> SendInternalAsync(
            int milisecondsDelay,
            long contentLength,
            int bufferSize,
            Stream stream,
            CancellationToken token
        )
        {
            if (_client == null)
                return new WriteResult(WriteResultStatus.ClientNotFound, 0);

            WriteResult result = new WriteResult(WriteResultStatus.Success, 0);

            try
            {
                while (true)
                {
                    if (await _WriteSemaphore.WaitAsync(10, token).ConfigureAwait(false))
                        break;
                    await Task.Delay(10, token).ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException)
            {
                result.Status = WriteResultStatus.Canceled;
                return result;
            }
            catch (OperationCanceledException)
            {
                result.Status = WriteResultStatus.Canceled;
                return result;
            }

            Task<WriteResult> task = Task.Run(
                async () =>
                {
                    try
                    {
                        if (contentLength > 0 && stream != null && stream.CanRead)
                        {
                            long bytesRemaining = contentLength;
                            byte[] buffer = new byte[bufferSize];

                            while (bytesRemaining > 0)
                            {
                                int bytesRead = await stream
                                    .ReadAsync(
                                        buffer.AsMemory(
                                            0,
                                            (int)Math.Min(buffer.Length, bytesRemaining)
                                        ),
                                        token
                                    )
                                    .ConfigureAwait(false);
                                if (bytesRead > 0)
                                {
                                    await _clientStream
                                        .WriteAsync(buffer.AsMemory(0, bytesRead), token)
                                        .ConfigureAwait(false);
                                    await _clientStream.FlushAsync(token).ConfigureAwait(false);

                                    result.BytesWritten += bytesRead;
                                    bytesRemaining -= bytesRead;
                                }
                                else if (bytesRead == 0)
                                    throw new EndOfStreamException(
                                        "[FixedTcpClient] - End of stream for write operation."
                                    );
                            }
                        }
                    }
                    catch (TaskCanceledException)
                    {
                        result.Status = WriteResultStatus.Canceled;
                    }
                    catch (OperationCanceledException)
                    {
                        result.Status = WriteResultStatus.Canceled;
                    }
                    catch
                    {
                        result.Status = WriteResultStatus.Disconnected;
                    }

                    return result;
                },
                token
            );

            Task first = await Task.WhenAny(task, Task.Delay(milisecondsDelay, token))
                .ConfigureAwait(false);

            _WriteSemaphore.Release();

            if (first == task)
                return task.Result;

            result.Status = WriteResultStatus.Canceled;
            return result;
        }

        private async Task<ReadResult> ReadInternalAsync(
            int milisecondsDelay,
            long count,
            int bufferSize,
            CancellationToken token
        )
        {
            if (count < 1)
                return new ReadResult(ReadResultStatus.Success, 0, null);
            else if (_client == null)
                return new ReadResult(ReadResultStatus.ClientNotFound, 0, null);

            ReadResult result = new ReadResult(ReadResultStatus.Success, 0, null);

            try
            {
                while (true)
                {
                    if (await _ReadSemaphore.WaitAsync(10, token).ConfigureAwait(false))
                        break;
                    await Task.Delay(10, token).ConfigureAwait(false);
                }
            }
            catch (TaskCanceledException)
            {
                result.Status = ReadResultStatus.Canceled;
                return result;
            }
            catch (OperationCanceledException)
            {
                result.Status = ReadResultStatus.Canceled;
                return result;
            }

            Task<ReadResult> task = Task.Run(
                async () =>
                {
                    try
                    {
                        MemoryStream ms = new MemoryStream();
                        long bytesRemaining = count;
                        byte[] buffer = new byte[bufferSize];

                        while (bytesRemaining > 0)
                        {
                            if (bytesRemaining < bufferSize)
                                buffer = new byte[bytesRemaining];

                            int bytesRead = await _clientStream
                                .ReadAsync(buffer, token)
                                .ConfigureAwait(false);

                            if (bytesRead > 0)
                            {
                                await ms.WriteAsync(buffer.AsMemory(0, bytesRead), token)
                                    .ConfigureAwait(false);
                                result.BytesRead += bytesRead;
                                bytesRemaining -= bytesRead;
                            }
                            else
                            {
                                // Zero bytes read indicates graceful disconnect
                                result.Status = ReadResultStatus.Disconnected;
                                result.DataStream = null;
                                Dispose();
                                return result;
                            }
                        }

                        ms.Seek(0, SeekOrigin.Begin);
                        result.DataStream = ms;
                    }
                    catch (TaskCanceledException)
                    {
                        result.Status = ReadResultStatus.Canceled;
                    }
                    catch (OperationCanceledException)
                    {
                        result.Status = ReadResultStatus.Canceled;
                    }
                    catch
                    {
                        result.Status = ReadResultStatus.Disconnected;
                        result.BytesRead = 0;
                        result.DataStream = null;
                        Dispose();
                    }

                    return result;
                },
                token
            );

            Task first = await Task.WhenAny(task, Task.Delay(milisecondsDelay, token))
                .ConfigureAwait(false);

            _ReadSemaphore.Release();

            if (first == task)
                return task.Result;

            result.Status = ReadResultStatus.Canceled;
            return result;
        }
    }
}
