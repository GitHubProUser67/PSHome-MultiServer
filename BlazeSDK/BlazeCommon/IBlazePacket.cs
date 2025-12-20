using Tdf;

namespace BlazeCommon
{
    public interface IBlazePacket
    {
        FireFrame Frame { get; set; }
        object DataObj { get; }

        string ToString(IBlazeComponent component, bool inbound);
        void WriteTo(Stream stream, ITdfEncoder encoder);
        Task WriteToAsync(Stream stream, ITdfEncoder encoder);
        byte[] Encode(ITdfEncoder encoder);

        ProtoFirePacket ToProtoFirePacket(ITdfEncoder encoder);
        BlazePacket<Resp> CreateResponsePacket<Resp>(Resp data, int errorCode) where Resp : notnull;
        BlazePacket<Resp> CreateResponsePacket<Resp>(int errorCode) where Resp : notnull;
        BlazePacket<Resp> CreateResponsePacket<Resp>(Resp data) where Resp : notnull;

        IBlazePacket CreateResponsePacket(object data, int errorCode);
        IBlazePacket CreateResponsePacket(int errorCode);
        IBlazePacket CreateResponsePacket(object data);

    }
}
