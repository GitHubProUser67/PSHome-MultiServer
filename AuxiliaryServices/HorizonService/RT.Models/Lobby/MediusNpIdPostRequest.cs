using System.IO;
using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.Text;

namespace Horizon.RT.Models
{
    /// <summary>
    /// Makes a PostRequest to the Medius Lobby Server on Connect sending the SCE_NPID_MAXLEN data blob
    /// </summary>
    [MediusMessage(NetMessageClass.MessageClassLobbyExt, MediusLobbyExtMessageIds.NpIdPostRequest)]
    public class MediusNpIdPostRequest : BaseLobbyExtMessage, IMediusRequest
    {
        public override byte PacketType => (byte)MediusLobbyExtMessageIds.NpIdPostRequest;

        /// <summary>
        /// Message ID
        /// </summary>
        public MessageId MessageID { get; set; }
        /// <summary>
        /// Session Key
        /// </summary>
        public string SessionKey; // SESSIONKEY_MAXLEN
        /// <summary>
        /// SceNpId
        /// </summary>
        //public string SceNpId; // SCENPID_MAXLEN

        //SCE_NPID_MAXLEN = 36;

        //SceNpId
        //handle
        public byte[] data;
        public byte term;
        public byte[] dummy;

        public byte[] opt;
        public byte[] reserved;
        

        public override void Deserialize(MessageReader reader)
        {
            // 
            base.Deserialize(reader);

            //
            MessageID = reader.Read<MessageId>();

            // 
            SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            
            //SCE_NPID Data Blob
            data = reader.ReadBytes(16);
            term = reader.ReadByte();
            dummy = reader.ReadBytes(3);

            //
            opt = reader.ReadBytes(8);
            reserved = reader.ReadBytes(8);
            
        }

        public override void Serialize(MessageWriter writer)
        {
            // 
            base.Serialize(writer);

            //
            writer.Write(MessageID ?? MessageId.Empty);

            // 
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);

            
            //SCE_NPID Data Blob
            //SCENpOnlineId
            writer.Write(data);
            writer.Write(term);
            writer.Write(dummy);

            //
            writer.Write(opt);
            writer.Write(reserved);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"MessageID: {MessageID} " +
                $"SessionKey: {SessionKey} " +
                //$"SceNpId: {SceNpId}";

                $"Data: {Encoding.Default.GetString(data)} " +
                $"Term: {term} " +
                $"Dummy: {Encoding.Default.GetString(dummy)} " +
                $"Opt: {Encoding.Default.GetString(opt)}" +
                $"Reserved: {Encoding.Default.GetString(reserved)}";
                
        }
    }
}
