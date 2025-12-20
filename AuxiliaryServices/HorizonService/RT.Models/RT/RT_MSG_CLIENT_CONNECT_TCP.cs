using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System;

namespace Horizon.RT.Models
{
    [ScertMessage(RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_TCP)]
    public class RT_MSG_CLIENT_CONNECT_TCP : BaseScertMessage
    {
        public override RT_MSG_TYPE Id => RT_MSG_TYPE.RT_MSG_CLIENT_CONNECT_TCP;

        public int TargetWorldId = 1;
        public byte UNK0;
        public byte[] UNK1;
        public int AppId;
        public RSA_KEY Key;

        public string SessionKey = null;
        public string AccessToken = null;

        public override void Deserialize(MessageReader reader)
        {
            SessionKey = null;
            AccessToken = null;

            if (reader.MediusVersion < 109)
            {
                UNK1 = reader.ReadBytes(3);
                TargetWorldId = reader.ReadByte();
                UNK0 = reader.ReadByte();
            }
            else
                TargetWorldId = reader.ReadInt32();
            AppId = reader.ReadInt32();
            Key = reader.Read<RSA_KEY>();

            if (reader.BaseStream.Position < reader.BaseStream.Length)
            {
                SessionKey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
                AccessToken = reader.ReadString(Constants.NET_ACCESS_KEY_LEN);
            }
        }

        public override void Serialize(MessageWriter writer)
        {
            if (writer.MediusVersion < 109)
            {
                writer.Write(new byte[3]);
                writer.Write((byte)TargetWorldId);
                writer.Write(UNK0);
            }
            else
                writer.Write(TargetWorldId);
            writer.Write(AppId);
            writer.Write(Key ?? RSA_KEY.Empty);
            writer.Write(SessionKey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(AccessToken, Constants.NET_ACCESS_KEY_LEN);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"TargetWorldId: {TargetWorldId} " +
                $"UNK1: {(UNK1 != null ? BitConverter.ToString(UNK1) : string.Empty)} " +
                $"UNK0: {UNK0:X2} " +
                $"AppId: {AppId} " +
                $"Key: {Key} " +
                $"SessionKey: {SessionKey} " +
                $"AccessToken: {AccessToken}";
        }
    }
}
