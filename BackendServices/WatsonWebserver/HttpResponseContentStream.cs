using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MultiServerLibrary.Extension.NET;

namespace WatsonWebserver
{
    /// <summary>
    /// From WebOne.
    /// A wrapper around a <see cref="Stream"/> that can write HTTP response bodies in according to RFC 9112 §7.
    /// </summary>
    public class HttpResponseContentStream : Stream
    {
        //RTFM: https://datatracker.ietf.org/doc/html/rfc9112#section-7

        private const string newLine = "\r\n";

        private readonly int milisecondsDelay;

        private readonly byte[] newLineBytes = Encoding.ASCII.GetBytes(newLine);
        private readonly FixedTcpClient inner;
        private readonly bool UseChunkedTransfer;

        public HttpResponseContentStream(
            int milisecondsDelay,
            FixedTcpClient inner,
            bool UseChunkedTransfer
        )
        {
            this.milisecondsDelay = milisecondsDelay;
            this.inner = inner;
            this.UseChunkedTransfer = UseChunkedTransfer;
        }

        public override void Flush()
        {
            // Ignore (handled via the tcp client class).
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Writes a sequence of bytes to the client.
        /// </summary>
        /// <param name="buffer">Array of bytes containing the data payload.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The number of bytes to be written to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            byte[] destinationBytes = new byte[count];
            Array.Copy(buffer, offset, destinationBytes, 0, count);

            if (UseChunkedTransfer)
            {
                // Send chunk
                string chunkIdent = (count - offset).ToString("X");

                inner
                    .SendAsync(
                        milisecondsDelay,
                        Encoding.ASCII.GetBytes(chunkIdent + newLine),
                        chunkIdent.Length + newLine.Length
                    )
                    .ContinueWith(t1 =>
                    {
                        if (
                            t1.IsCompletedSuccessfully
                            && t1.Result.Status == WriteResultStatus.Success
                        )
                            return inner.SendAsync(milisecondsDelay, destinationBytes, count);

                        return Task.FromResult(
                            new WriteResult() { Status = WriteResultStatus.Canceled }
                        );
                    })
                    .Unwrap()
                    .ContinueWith(t2 =>
                    {
                        if (
                            t2.IsCompletedSuccessfully
                            && t2.Result.Status == WriteResultStatus.Success
                        )
                            return inner.SendAsync(
                                milisecondsDelay,
                                newLineBytes,
                                newLineBytes.Length
                            );

                        return Task.CompletedTask;
                    })
                    .Unwrap()
                    .Wait();
            }
            else
                // Just write the body
                inner.SendAsync(milisecondsDelay, destinationBytes, count).Wait();
        }

        /// <summary>
        /// If the data transfer channel between server and client is based on encoded transfer, send mark of content end,
        /// required to properly finish the transfer session.
        /// </summary>
        /// <param name="trailer">Trailing header (if any)</param>
        public async Task WriteTerminatorAsync(string trailer = "")
        {
            if (UseChunkedTransfer)
            {
                // Write terminating chunk if need
                try
                {
                    await inner
                        .SendAsync(
                            milisecondsDelay,
                            Encoding.ASCII.GetBytes($"0{newLine}"),
                            newLine.Length + 1
                        )
                        .ConfigureAwait(false);
                    await inner
                        .SendAsync(
                            milisecondsDelay,
                            Encoding.ASCII.GetBytes(trailer + newLine),
                            trailer.Length + newLine.Length
                        )
                        .ConfigureAwait(false);
                }
                catch
                { /* Sometimes an connection lost may occur here. It's not a reason to worry. */
                }
                ;
            }
        }

        public override bool CanRead => false;
        public override bool CanSeek => false;
        public override bool CanWrite => true;

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position { get; set; }
    }
}
