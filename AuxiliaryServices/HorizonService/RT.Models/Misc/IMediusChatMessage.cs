using Horizon.RT.Common;

namespace Horizon.RT.Models.Misc
{
    public interface IMediusChatMessage
    {
        MediusChatMessageType MessageType { get; }
        int TargetID { get; }
        string Message { get; }
    }
}