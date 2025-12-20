namespace Blaze3SDK.Blaze.GameManager
{
    [Flags]
    public enum GameSettings
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
        IgnoreEntryCriteriaWithInvite = 0x400,
        EnablePersistedGameId = 0x800,
        AllowSameTeamId = 0x1000,
        Virtualized = 0x2000,
        SendOrphanedGameReportEvent = 0x4000,
        AllowAnyReputation = 0x8000,
        LockedAsBusy = 0x10000,
        DisconnectReservation = 0x20000,
        DynamicReputationRequirement = 0x40000,
        FriendsBypassClosedToJoinByPlayer = 0x80000,
        AllowMemberGameAttributeEdit = 0x100000
    }
}