namespace Blaze2SDK.Blaze.GameManager
{
    [Flags]
    public enum GameSettings : uint
    {
        None = 0,
        OpenToBrowsing = 1,
        OpenToMatchmaking = 2,
        OpenToInvites = 4,
        OpenToJoinByPlayer = 8,
        HostMigratable = 0x10,
        Ranked = 0x20,
        AdminOnlyInvites = 0x40,
        EnforceSingleGroupJoin = 0x80,
        JoinInProgressSupported = 0x100,
        AdminInvitesOnlyIgnoreEntryChecks = 0x200,
        EnablePersistedGameId = 0x400,
        AllowSameTeamId = 0x800
    }
}