namespace Blaze2SDK.Blaze.Playgroups
{
    [Flags]
    public enum MemberPermissions
    {
        None = 0,
        DestroyPlaygroup = 1,
        JoinGame = 2,
        KickPlaygroupMember = 4,
        ModifyPlaygroupAttributes = 8,
        ModifyPlaygroupJoinControls = 16,
    }
}