namespace BlazeCommon
{
    public interface IBlazeServerComponent : IBlazeComponent
    {
        BlazeServerCommandMethodInfo? GetBlazeCommandInfo(ushort commandId);
    }
}
