using QuazalServer.QNetZ.DDL;

namespace QuazalServer.RDVServices.RMC
{
    public abstract class RMCPResponse
    {
        public abstract override string ToString();
        public abstract string PayloadToString();
        public abstract byte[] ToBuffer();
    }

    public class RMCPResponseEmpty : RMCPResponse
    {
        public override byte[] ToBuffer()
        {
            return Array.Empty<byte>();
        }

        public override string ToString()
        {
            return "[RMCPacketResponseEmpty]";
        }

        public override string PayloadToString()
        {
            return string.Empty;
        }
    }

    // Wrapper class for DDL (or ANY object)
    public class RMCPResponseDDL<T> : RMCPResponse where T : class
    {
        public RMCPResponseDDL(T data)
        {
            objectData = data;
        }

        T objectData;

        public override string PayloadToString()
        {
            return DDLSerializer.ObjectToString(objectData);
        }

        public override byte[] ToBuffer()
        {
            using (MemoryStream m = new())
            {
                DDLSerializer.WriteObject(objectData, m);

                return m.ToArray();
            }
        }

        public override string ToString()
        {
            return $"[ResponseDDL<{typeof(T).Name}>]";
        }
    }
}
