using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using MultiServerLibrary.Extension;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_SERVER_STARTUP_INFO_NOTIFY)]
    public class RT_MSG_SERVER_STARTUP_INFO_NOTIFY : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_SERVER_STARTUP_INFO_NOTIFY;

        public byte GameHostType { get; set; } = (byte)MGCL_GAME_HOST_TYPE.MGCLGameHostClientServerAuxUDP;
        public uint Timebase { get; set; } = DateTimeUtils.GetUnixTimeU32();
        public List<uint> FieldsSetA  = new List<uint>();
        public ushort FieldsSetAExtraInfo;
        public List<(ushort, ushort, ushort)> FieldsSetB = new List<(ushort, ushort, ushort)>();
        public ushort FieldsSetBExtraInfo;
        public List<ushort> FieldsSetC = new List<ushort>();
        public ushort FieldsSetCExtraInfo;
        public byte[] FieldsSetCData;
        public byte[] StartupInfo;

        public override void Deserialize(MessageReader reader)
        {
            GameHostType = reader.ReadByte();
            StartupMessageFlags flags = (StartupMessageFlags)GameHostType;

            if (flags.HasFlag(StartupMessageFlags.HasStartupInfo))
                StartupInfo = reader.ReadBytes(6);

            if (flags.HasFlag(StartupMessageFlags.HasGlobalTimeReset))
                Timebase = reader.ReadUInt32();

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetA))
            {
                byte countFlag = reader.ReadByte();
                int count = countFlag & 0x7F;
                bool hasExtra = (countFlag & 0x80) != 0;

                if (hasExtra)
                    FieldsSetAExtraInfo = reader.ReadUInt16();

                byte[] tempData = reader.ReadBytes(count);

                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if ((tempData[i] & (1 << j)) != 0)
                            FieldsSetA.Add(reader.ReadUInt32());
                    }
                }
            }

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetB))
            {
                byte countFlag = reader.ReadByte();
                int count = countFlag & 0x7F;
                bool hasExtra = (countFlag & 0x80) != 0;

                if (hasExtra)
                    FieldsSetBExtraInfo = reader.ReadUInt16();

                byte[] tempData = reader.ReadBytes(count);

                for (int i = 0; i < count; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        if ((tempData[i] & (1 << j)) != 0)
                            FieldsSetB.Add((reader.ReadUInt16(), reader.ReadUInt16(), reader.ReadUInt16()));
                    }
                }
            }

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetC))
            {
                byte countFlag = reader.ReadByte();
                int count = countFlag & 0x7F;
                bool hasExtra = (countFlag & 0x80) != 0;

                if (hasExtra)
                    FieldsSetCExtraInfo = reader.ReadUInt16();

                FieldsSetCData = reader.ReadBytes(count);

                ushort valueCount = reader.ReadUInt16();

                for (int i = 0; i < valueCount; i++)
                {
                    FieldsSetC.Add(reader.ReadUInt16());
                }
            }
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(GameHostType);
            StartupMessageFlags flags = (StartupMessageFlags)GameHostType;

            if (flags.HasFlag(StartupMessageFlags.HasStartupInfo))
                writer.Write(StartupInfo, 6);

            if (flags.HasFlag(StartupMessageFlags.HasGlobalTimeReset))
                writer.Write(Timebase);

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetA))
                throw new NotImplementedException("[RT_MSG_SERVER_STARTUP_INFO_NOTIFY] - HasFieldSetA serializing is not supported yet! Please report to GITHUB.");

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetB))
                throw new NotImplementedException("[RT_MSG_SERVER_STARTUP_INFO_NOTIFY] - HasFieldSetB serializing is not supported yet! Please report to GITHUB.");

            if (flags.HasFlag(StartupMessageFlags.HasFieldSetC))
                throw new NotImplementedException("[RT_MSG_SERVER_STARTUP_INFO_NOTIFY] - HasFieldSetC serializing is not supported yet! Please report to GITHUB.");
        }


        public override string ToString()
        {
            string setA = string.Join(", ", FieldsSetA);
            string setB = string.Join(", ", FieldsSetB.Select(t => $"({t.Item1},{t.Item2},{t.Item3})"));
            string setC = string.Join(", ", FieldsSetC);

            return base.ToString() + " " +
                $"GameHostType: {(MGCL_GAME_HOST_TYPE)GameHostType}, " +
                $"Timebase: {Timebase}, " +
                $"StartupInfo: {(StartupInfo != null ? BitConverter.ToString(StartupInfo) : string.Empty)}, " +
                $"FieldsSetA: [{setA}], " +
                $"FieldsSetAExtraInfo: {FieldsSetAExtraInfo}, " +
                $"FieldsSetB: [{setB}], " +
                $"FieldsSetBExtraInfo: {FieldsSetBExtraInfo}, " +
                $"FieldsSetC: [{setC}]" +
                $"FieldsSetCExtraInfo: {FieldsSetCExtraInfo}, " +
                $"FieldsSetCData: {(FieldsSetCData != null ? BitConverter.ToString(FieldsSetCData) : string.Empty)}, ";
        }
    }
}
