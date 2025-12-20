using System;
using System.Collections.Generic;

namespace Horizon.LIBRARY.Database.Models
{
    public class ClanDTO
    {

        public int ClanId { get; set; }
        public string ClanName { get; set; }
        public string ClanTAG { get; set; }
        public AccountDTO ClanLeaderAccount { get; set; }
        public List<AccountDTO> ClanMember { get; set; }
        public List<ClanInvitationDTO> ClanInvitations { get; set; }
        public List<ClanMessageDTO> ClanMessages { get; set; }
        public List<ClanMetaDataDTO> SVOClanMetaData { get; set; }
        public List<ClanNewsDTO> SVOClanNews { get; set; }
        public DateTime CreateDt { get; set; }
        public DateTime? ModifiedDt { get; set; }
        public int Status { get; set; }
        public int AppId { get; set; }
        public bool IsDisbanded { get; set; }
        public string ClanMediusStats { get; set; }

        /// <summary>
        /// Collection of ladder stats.
        /// </summary>
        public int[] ClanStats { get; set; }

        /// <summary>
        /// Collection of ladder stats.
        /// </summary>
        public int[] ClanWideStats { get; set; }

        /// <summary>
        /// Collection of custom ladder stats.
        /// </summary>
        public int[] ClanCustomWideStats { get; set; }
    }

    public class CreateClanDTO
    {
        public string ClanName { get; set; }
        public string ClanTAG { get; set; }
        public int AccountId { get; set; }
        public int AppId { get; set; }
    }

    public class ClanPlayerDTO
    {
        public int ClanId { get; set; }
        public int ClanPlayerId { get; set; }
        public int AccountId { get; set; }
        public DateTime playerLevelUpdateDate { get; set; }
        public string playerName { get; set; }
        public DateTime playerStatusUpdateDate { get; set; }
        public int status { get; set; }
        public int clanPlayerLevelId { get; set; }
        public int lastMsgRead { get; set; }
        public int? ModifiedBy { get; set; }
    }

    public class ClanMetaDataDTO
    {
        public string ClanId { get; set; }
        public string MetaDataKey { get; set; }
        public string MetaDataValue { get; set; }
    }

    public class ClanNewsDTO
    {
        public int Id { get; set; }
        public string ClanId { get; set; }
        public string newsBody { get; set; }
        public DateTime CreateDt { get; set; }
        public int AppId { get; set; }
    }

    public class ClanInvitationDTO
    {
        public int Id { get; set; }
        public int ClanId { get; set; }
        public string ClanName { get; set; }
        public int AccountId { get; set; }
        public string AccountName { get; set; }
        public string InviteMsg { get; set; }
        public int AppId { get; set; }
        public int ResponseStatus { get; set; }
        public int ResponseTime { get; set; }
        public DateTime CreateDt { get; set; }
        public string ResponseMessage { get; set; }
        public string ClanLeaderName { get; set; }
    }

    public class AccountClanInvitationDTO
    {
        public int LeaderAccountId { get; set; }
        public string LeaderAccountName { get; set; }
        public ClanInvitationDTO Invitation { get; set; }
    }

    public class ClanInvitationResponseDTO
    {
        public int AccountId { get; set; }
        public int InvitationId { get; set; }
        public int Response { get; set; }
        public int ResponseTime { get; set; }
        public string ResponseMessage { get; set; }
    }

    public class ClanMessageDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
    }

    public class ClanTransferLeadershipDTO
    {
        public int ClanId { get; set; }
        public int AccountId { get; set; }
        public int NewLeaderAccountId { get; set; }
    }

    public class ClanTeamChallengeDTO
    {
        public int Id { get; set; }
        public int ChallengerClanID { get; set; }
        public int AgainstClanID { get; set; }
        public int Status { get; set; }
        public int ResponseTime { get; set; }
        public string ChallengeMsg { get; set; }
        public string ResponseMessage { get; set; }
        public int ClanChallengeId { get; set; }
    }
}