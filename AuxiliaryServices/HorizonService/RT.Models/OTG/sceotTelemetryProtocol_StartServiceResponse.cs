using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System;

namespace Horizon.RT.Models
{

    public class sceotTelemetryProtocol_StartServiceResponse : BaseScertMessage { 

        public override RT_MSG_TYPE Id => throw new NotImplementedException();

        public byte msgType;
        public byte result;
        public long requestId;

        public override void Deserialize(MessageReader reader)
        {
            msgType = reader.ReadByte();
            result = reader.ReadByte();
            reader.ReadBytes(2);
            requestId = reader.ReadInt64();
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(msgType);
            writer.Write(result);
            writer.Write(new byte[2]);
            writer.Write(requestId);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"msgType: {msgType} " + 
                $"result: {result} " +
                $"requestId: {requestId}";
        }

    }
    
}
