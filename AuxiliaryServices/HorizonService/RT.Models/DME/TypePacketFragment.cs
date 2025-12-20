using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Collections.Generic;
using System;

namespace Horizon.RT.Models
{
    [MediusMessage(NetMessageClass.MessageClassDME, MediusDmeMessageIds.PacketFragment)]
    public class TypePacketFragment : BaseDMEMessage
    {
        public override byte PacketType => (byte)MediusDmeMessageIds.PacketFragment;

        public byte FragmentMessageClass;
        public byte FragmentMessageType;
        public ushort SubPacketSize;
        public ushort SubPacketCount;
        public ushort SubPacketIndex;
        public byte MultiPacketindex;
        public int PacketBufferSize;
        public int PacketBufferOffset;
        public byte[] Payload;

        public override void Deserialize(MessageReader reader)
        {
            base.Deserialize(reader);

            FragmentMessageClass = reader.ReadByte();
            FragmentMessageType = reader.ReadByte();
            SubPacketSize = reader.ReadUInt16();
            SubPacketCount = reader.ReadUInt16();
            SubPacketIndex = reader.ReadUInt16();
            MultiPacketindex = reader.ReadByte();
            reader.ReadBytes(3);
            PacketBufferSize = reader.ReadInt32();
            PacketBufferOffset = reader.ReadInt32();
            Payload = reader.ReadBytes(SubPacketSize);
        }

        public override void Serialize(MessageWriter writer)
        {
            base.Serialize(writer);

            writer.Write(FragmentMessageClass);
            writer.Write(FragmentMessageType);
            writer.Write(SubPacketSize);
            writer.Write(SubPacketCount);
            writer.Write(SubPacketIndex);
            writer.Write(MultiPacketindex);
            writer.Write(new byte[3]);
            writer.Write(PacketBufferSize);
            writer.Write(PacketBufferOffset);
            writer.Write(Payload, SubPacketSize);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"FragmentMessageClass: {FragmentMessageClass} " +
                $"FragmentMessageType: {FragmentMessageType} " +
                $"SubPacketSize: {SubPacketSize} " +
                $"SubPacketCount: {SubPacketCount} " +
                $"SubPacketIndex: {SubPacketIndex} " +
                $"MultiPacketindex: {MultiPacketindex} " +
                $"PacketBufferSize: {PacketBufferSize} " +
                $"PacketBufferOffset: {PacketBufferOffset}";
        }

        public static List<TypePacketFragment> FromPayload(NetMessageClass msgClass, byte msgType, byte[] payload)
        {
            return FromPayload(msgClass, msgType, payload, 0, payload.Length);
        }

        public static List<TypePacketFragment> FromPayload(NetMessageClass msgClass, byte msgType, byte[] payload, int index, int length)
        {
            List<TypePacketFragment> fragments = new List<TypePacketFragment>();

            int i = 0;

            while (i < length)
            {
                ushort subPacketSize = (ushort)(length - i);
                if (subPacketSize > Constants.DME_FRAGMENT_MAX_PAYLOAD_SIZE)
                    subPacketSize = Constants.DME_FRAGMENT_MAX_PAYLOAD_SIZE;

                var frag = new TypePacketFragment()
                {
                    FragmentMessageClass = (byte)msgClass,
                    FragmentMessageType = msgType,
                    SubPacketSize = subPacketSize,
                    SubPacketCount = 0,
                    SubPacketIndex = (ushort)fragments.Count,
                    MultiPacketindex = 0,
                    PacketBufferSize = length,
                    PacketBufferOffset = i,
                    Payload = new byte[subPacketSize]
                };

                // Copy payload segment into fragment payload
                Array.Copy(payload, i + index, frag.Payload, 0, subPacketSize);

                fragments.Add(frag);

                // Increment i
                i += subPacketSize;
            }

            // Recorrect fragment counts
            foreach (var frag in fragments)
                frag.SubPacketCount = (ushort)fragments.Count;

            return fragments;
        }
    }
}
