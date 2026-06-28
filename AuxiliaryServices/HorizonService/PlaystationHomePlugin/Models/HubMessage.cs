using EndianTools;

namespace Horizon.PlaystationHomePlugin.Models
{
    public class HubMessage : IDisposable
    {
        private const int MaxMessageSize = 0x2B0;

        public short ExtraInfo0 { get; private set; } // Padding? Version?
        public short ExtraInfo1 { get; private set; } // Often ProtocolVersion

        public short MessageId { get; private set; }
        public short MessageDestinationType { get; private set; }

        private int _decodeSeekIndex = 0;
        private int _encodeSeekIndex = 0;

        private bool disposedValue;

        private byte[] _rawEncodeBuffer = new byte[MaxMessageSize];
        private byte[] _rawDecodeBuffer = new byte[MaxMessageSize];

        public HubMessage(byte[] RawMessageData)
        {
            EncodeNextRawData(RawMessageData);

            Array.Copy(_rawEncodeBuffer, 0, _rawDecodeBuffer, 0, MaxMessageSize - 1);

            int tmp = DecodeNextInt();

            MessageId = (short)((tmp >> 16) & ushort.MaxValue);
            ExtraInfo0 = (short)tmp;

            tmp = DecodeNextInt();

            MessageDestinationType = (short)((tmp >> 16) & ushort.MaxValue);
            ExtraInfo1 = (short)tmp;
        }

        public bool IsEncodeOperationValid(int nextEncodeSize)
        {
            int newPosition = _encodeSeekIndex + nextEncodeSize;

            if (newPosition > MaxMessageSize)
                return false;

            return newPosition <= MaxMessageSize;
        }

        public bool IsDecodeOperationValid(int currentOffset)
        {
            int newPosition = currentOffset + _decodeSeekIndex;

            if (newPosition <= MaxMessageSize - 1)
                return newPosition < MaxMessageSize;

            return false;
        }

        public float DecodeNextFloat()
        {
            if (!IsDecodeOperationValid(4))
                return -1.0f;

            float res = EndianAwareConverter.ToSingle(
                _rawDecodeBuffer,
                Endianness.BigEndian,
                (uint)_decodeSeekIndex
            );

            _decodeSeekIndex += 4;

            return res;
        }

        public short DecodeNextShort()
        {
            if (!IsDecodeOperationValid(2))
                return -1;

            short res = EndianAwareConverter.ToInt16(
                _rawDecodeBuffer,
                Endianness.BigEndian,
                (uint)_decodeSeekIndex
            );

            _decodeSeekIndex += 2;

            return res;
        }

        public int DecodeNextInt()
        {
            if (!IsDecodeOperationValid(4))
                return -1;

            int res = EndianAwareConverter.ToInt32(
                _rawDecodeBuffer,
                Endianness.BigEndian,
                (uint)_decodeSeekIndex
            );

            _decodeSeekIndex += 4;

            return res;
        }

        public byte DecodeNextByte()
        {
            if (!IsDecodeOperationValid(1))
                return byte.MaxValue;

            byte value = _rawDecodeBuffer[_decodeSeekIndex];

            _decodeSeekIndex += 1;

            return value;
        }

        public byte[] DecodeNextRawData(int dataSize)
        {
            if (!IsDecodeOperationValid(dataSize))
                return Array.Empty<byte>();

            byte[] copy = new byte[dataSize];

            Array.Copy(_rawDecodeBuffer, _decodeSeekIndex, copy, 0, dataSize);

            _decodeSeekIndex += dataSize;

            return copy;
        }

        public byte[] DecodeNextString()
        {
            // Find null terminator
            int length = 0;
            while (
                _decodeSeekIndex + length < _rawDecodeBuffer.Length
                && _rawDecodeBuffer[_decodeSeekIndex + length] != 0
            )
                length++;

            // Include null terminator
            length += 1;

            return DecodeNextRawData(length);
        }

        public void DecodeManualIncrementRawDataIndex(int dataSize)
        {
            _decodeSeekIndex += dataSize;
        }

        public byte[] DecodeManualGetNextRawData()
        {
            int remainingLength = _rawDecodeBuffer.Length - _decodeSeekIndex;
            if (remainingLength <= 0)
                return Array.Empty<byte>();

            byte[] copy = new byte[remainingLength];
            Array.Copy(
                _rawDecodeBuffer,
                _decodeSeekIndex,
                copy,
                0,
                _rawDecodeBuffer.Length - _decodeSeekIndex
            );
            return copy;
        }

        public void EncodeNextShort(short val)
        {
            if (!IsEncodeOperationValid(2))
                return;

            byte[] bytes = BitConverter.GetBytes(val);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(bytes); // store as big-endian

            Array.Copy(bytes, 0, _rawEncodeBuffer, _encodeSeekIndex, 2);
            _encodeSeekIndex += 2;
        }

        public void EncodeNextInt(int val)
        {
            if (!IsEncodeOperationValid(4))
                return;

            byte[] bytes = BitConverter.GetBytes(val);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(bytes); // store as big-endian

            Array.Copy(bytes, 0, _rawEncodeBuffer, _encodeSeekIndex, 4);
            _encodeSeekIndex += 4;
        }

        public void EncodeNextFloat(float val)
        {
            if (!IsEncodeOperationValid(4))
                return;

            byte[] bytes = BitConverter.GetBytes(val);
            if (EndianTools.EndianAwareConverter.isLittleEndianSystem)
                Array.Reverse(bytes); // store as big-endian

            Array.Copy(bytes, 0, _rawEncodeBuffer, _encodeSeekIndex, 4);
            _encodeSeekIndex += 4;
        }

        public void EncodeNextRawData(byte[] data)
        {
            if (!IsEncodeOperationValid(data.Length))
                return;
            Array.Copy(data, 0, _rawEncodeBuffer, _encodeSeekIndex, data.Length);
            _encodeSeekIndex += data.Length;
        }

        public void EncodeNextString(byte[] bytes)
        {
            byte[] bytesWithNull = new byte[bytes.Length + 1]; // add null terminator
            Array.Copy(bytes, bytesWithNull, bytes.Length);

            EncodeNextRawData(bytesWithNull);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Make sure to make the buffers GC friendly.
                    _rawEncodeBuffer = null;
                    _rawDecodeBuffer = null;
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
