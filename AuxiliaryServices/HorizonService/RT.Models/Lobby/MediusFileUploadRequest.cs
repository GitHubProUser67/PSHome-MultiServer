using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Initiate an upload from the client to the server.
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobby, MediusLobbyMessageIds.FileUpload)]
    public class MediusFileUploadRequest : BaseLobbyMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyMessageIds.FileUpload;

        /// <summary>
        /// File Information
        /// </summary>
        public MediusFile MediusFileInfo;
        /// <summary>
        /// Data packet to upload
        /// </summary>
        public uint PucDataStart;
        //public byte[] PucDataStart;
        /// <summary>
        /// Size of data packet to upload
        /// </summary>
        public uint UiDataSize;
        /// <summary>
        /// ID specified by Client to assosciate with this request
        /// </summary>
        public MessageId MessageID { get; set; }

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            // 
            MediusFileInfo = reader.Read<MediusFile>();
            PucDataStart = reader.ReadUInt32();
            //reader.ReadBytes((int)UiDataSize);
            UiDataSize = reader.ReadUInt32();


            //
            MessageID = reader.Read<MessageId>();
            reader.ReadBytes(3);
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            // 
            writer.Write(MediusFileInfo);
            writer.Write(PucDataStart);
            writer.Write(UiDataSize);

            //
            writer.Write(MessageID ?? MessageId.Empty);
            writer.Write(new byte[3]);
        }


        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"MediusFileInfo: {MediusFileInfo} " +
                $"PucDataStart: {PucDataStart} " +
                $"UiDataSize: {UiDataSize}";
        }
    }
}
