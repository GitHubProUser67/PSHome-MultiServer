using Horizon.RT.Common;
using Horizon.LIBRARY.Common.Stream;
using System.IO;

namespace Horizon.RT.Models.AntiCheat
{
    public class AntiCheatEventInfo
    {
        public AnticheatEventCode code;
        public int AppId;
        public int world_Index;
        public int client_Index;
        public int iAccountID;
        public string acSessionkey; //SESSIONKEY_MAXLEN
        public int mData;
        public bool mPassedAntiCheat;

        public void Deserialize(BinaryReader reader)
        {
            code = reader.Read<AnticheatEventCode>();
            AppId = reader.ReadUInt16();
            world_Index = reader.ReadUInt16();
            client_Index = reader.ReadUInt16();
            iAccountID = reader.ReadInt32();
            acSessionkey = reader.ReadString(Constants.SESSIONKEY_MAXLEN);
            reader.ReadBytes(3);
            mData = reader.ReadUInt16();
            mPassedAntiCheat = reader.ReadBoolean();
            reader.ReadBytes(3);
        }

        public void Serialize(BinaryWriter writer)
        {
            writer.Write(code);
            writer.Write(world_Index);
            writer.Write(client_Index);
            writer.Write(iAccountID);
            writer.Write(acSessionkey, Constants.SESSIONKEY_MAXLEN);
            writer.Write(new byte[3]);
            writer.Write(mData);
            writer.Write(mPassedAntiCheat);
            writer.Write(new byte[3]);
        }

        public override string ToString()
        {
            return base.ToString() + " " +
                $"code: {code} " +
                $"world_Index: {world_Index} " +
                $"client_Index: {client_Index} " +
                $"iAccountID: {iAccountID} " +
                $"acSessionKey: {acSessionkey} " +
                $"mData: {mData} " +
                $"mPassedAntiCheat: {mPassedAntiCheat}";
        }
    }
}