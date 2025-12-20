using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System;

namespace Horizon.RT.Models
{

    public class sceotTelemetryProtocol_StartServiceRequest : BaseScertMessage { 

        public override RT_MSG_TYPE Id => throw new NotImplementedException();

        public byte msgType;
        public short maxBlockSize;
        public string systemId;
        public long requestId;

        public override void Deserialize(MessageReader reader)
        {
            msgType = reader.ReadByte();
            reader.ReadByte();
            maxBlockSize = reader.ReadByte();
            systemId = reader.ReadString(Constants.SYSTEMID_MAXLEN);
            requestId = reader.ReadInt64();
        }

        public override void Serialize(MessageWriter writer)
        {
            writer.Write(msgType);
            writer.Write(new byte[1]);
            writer.Write(maxBlockSize);
            writer.Write(systemId, Constants.SYSTEMID_MAXLEN);
            writer.Write(requestId);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"msgType: {msgType} " + 
                $"maxBlockSize: {maxBlockSize} " +
                $"systemId: {systemId} " +
                $"requestId: {requestId}";
        }

    }
    
}
