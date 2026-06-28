using Horizon.LIBRARY.Common.Stream;
using Horizon.RT.Common;
using MultiServerLibrary.Extension;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_STARTUP_INFO_NOTIFY)]
    public class RT_MSG_SERVER_STARTUP_INFO_NOTIFY : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_STARTUP_INFO_NOTIFY;

        public byte GameHostType { get; set; } =
            (byte)MGCL_GAME_HOST_TYPE.MGCLGameHostClientServerAuxUDP;
        public uint Timebase { get; set; } = DateTimeUtils.GetUnixTimeU32();
        public List<uint> FieldsSetA = new();
        public ushort FieldsSetAExtraInfo;
        public List<(ushort, ushort, ushort)> FieldsSetB = new();
        public ushort FieldsSetBExtraInfo;
        public List<ushort> FieldsSetC = new();
        public ushort FieldsSetCExtraInfo;
        public byte[] FieldsSetCData;
        public byte[] StartupInfo;

        public override void Deserialize(MessageReader reader)
        {
            GameHostType = reader.ReadByte();
            var flags = (StartupMessageFlags)GameHostType;

            if (flags.HasFlag(StartupMessageFlags.HasStartupInfo))
                StartupInfo = reader.ReadBytes(6);

            if (flags.HasFlag(StartupMessageFlags.HasGlobalTimeReset))
                Timebase = reader.ReadUInt32();

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetA))
            {
                var countFlag = reader.ReadByte();
                var count = countFlag & 0x7F;
                var hasExtra = (countFlag & 0x80) != 0;

                if (hasExtra)
                    FieldsSetAExtraInfo = reader.ReadUInt16();

                var tempData = reader.ReadBytes(count);

                for (var i = 0; i < count; i++)
                {
                    for (var j = 0; j < 8; j++)
                    {
                        if ((tempData[i] & (1 << j)) != 0)
                            FieldsSetA.Add(reader.ReadUInt32());
                    }
                }
            }

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetB))
            {
                var countFlag = reader.ReadByte();
                var count = countFlag & 0x7F;
                var hasExtra = (countFlag & 0x80) != 0;

                if (hasExtra)
                    FieldsSetBExtraInfo = reader.ReadUInt16();

                var tempData = reader.ReadBytes(count);

                for (var i = 0; i < count; i++)
                {
                    for (var j = 0; j < 8; j++)
                    {
                        if ((tempData[i] & (1 << j)) != 0)
                            FieldsSetB.Add(
                                (reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16())
                            );
                    }
                }
            }

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetC))
            {
                var countFlag = reader.ReadByte();
                var count = countFlag & 0x7F;
                var hasExtra = (countFlag & 0x80) != 0;

                if (hasExtra)
                    FieldsSetCExtraInfo = reader.ReadUInt16();

                FieldsSetCData = reader.ReadBytes(count);

                var valueCount = reader.ReadUInt16();

                for (var i = 0; i < valueCount; i++)
                {
                    FieldsSetC.Add(reader.ReadUInt16());
                }
            }
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(GameHostType);
            var flags = (StartupMessageFlags)GameHostType;

            if (flags.HasFlag(StartupMessageFlags.HasStartupInfo))
                writer.Write(StartupInfo, 6);

            if (flags.HasFlag(StartupMessageFlags.HasGlobalTimeReset))
                writer.Write(Timebase);

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetA))
                throw new NotImplementedException(
                    "[RT_MSG_SERVER_STARTUP_INFO_NOTIFY] - HasFieldSetA serializing is not supported yet! Please report to GITHUB."
                );

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetB))
                throw new NotImplementedException(
                    "[RT_MSG_SERVER_STARTUP_INFO_NOTIFY] - HasFieldSetB serializing is not supported yet! Please report to GITHUB."
                );

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetC))
                throw new NotImplementedException(
                    "[RT_MSG_SERVER_STARTUP_INFO_NOTIFY] - HasFieldSetC serializing is not supported yet! Please report to GITHUB."
                );
        }

        public override string ToString()
        {
            var setA = string.Join(", ", FieldsSetA);
            var setB = string.Join(
                ", ",
                FieldsSetB.Select(t => $"({t.Item1},{t.Item2},{t.Item3})")
            );
            var setC = string.Join(", ", FieldsSetC);

            return base.ToString()
                + " "
                + $"GameHostType: {(MGCL_GAME_HOST_TYPE)GameHostType}, "
                + $"Timebase: {Timebase}, "
                + $"StartupInfo: {(StartupInfo != null ? BitConverter.ToString(StartupInfo) : string.Empty)}, "
                + $"FieldsSetA: [{setA}], "
                + $"FieldsSetAExtraInfo: {FieldsSetAExtraInfo}, "
                + $"FieldsSetB: [{setB}], "
                + $"FieldsSetBExtraInfo: {FieldsSetBExtraInfo}, "
                + $"FieldsSetC: [{setC}]"
                + $"FieldsSetCExtraInfo: {FieldsSetCExtraInfo}, "
                + $"FieldsSetCData: {(FieldsSetCData != null ? BitConverter.ToString(FieldsSetCData) : string.Empty)}, ";
        }
    }
}
