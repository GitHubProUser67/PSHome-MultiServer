using System.Reflection;
using Tdf;

namespace BlazeCommon
{
    public class ProtoFirePacket(FireFrame frame, byte[] data)
    {
        public FireFrame Frame { get; set; } = frame;
        public byte[] Data { get; set; } = data;

        public MemoryStream GetDataStream()
        {
            return new MemoryStream(Data, false);
        }

        public ProtoFirePacket CreateResponsePacket(int errorCode = 0)
        {
            return new ProtoFirePacket(Frame.CreateResponseFrame(errorCode), []);
        }

        public ProtoFirePacket CreateResponsePacket(byte[] data, int errorCode = 0)
        {
            return new ProtoFirePacket(Frame.CreateResponseFrame(errorCode), data);
        }

        public void WriteTo(Stream stream)
        {
            Frame.Size = (uint)Data.Length;
            Frame.WriteTo(stream);
            if (Data.Length != 0)
                stream.Write(Data, 0, Data.Length);
        }

        public async Task WriteToAsync(Stream stream)
        {
            Frame.Size = (uint)Data.Length;
            await Frame.WriteToAsync(stream).ConfigureAwait(false);
            if (Data.Length != 0)
                await stream.WriteAsync(Data, 0, Data.Length).ConfigureAwait(false);
        }

        static readonly MethodInfo decodeMethod = typeof(ITdfDecoder).GetMethod(
            nameof(ITdfDecoder.Decode),
            [typeof(Type), typeof(Stream)]
        )!;

        public IBlazePacket Decode(Type type, ITdfDecoder decoder)
        {
            var blzPacketType = typeof(BlazePacket<>).MakeGenericType(type);
            var obj = decodeMethod.Invoke(decoder, [type, GetDataStream()])!;
            return (IBlazePacket)Activator.CreateInstance(blzPacketType, Frame, obj)!;
        }
    }
}
