using QuazalServer.RDVServices.DDL.Models;
using QuazalServer.QNetZ.Attributes;
using QuazalServer.QNetZ.Interfaces;
using QuazalServer.QNetZ;
using RDVServices;

namespace QuazalServer.RDVServices.GameServices.PS3RaymanLegendsServices
{
    /// <summary>
    /// Ubi achievements service
    /// </summary>
    [RMCService((ushort)RMCProtocolId.UplayWinService)]
    public class UplayWinService : RMCServiceBase
    {
        [RMCMethod(1)]
        public RMCResult GetActions(int start_row_index, int maximum_rows, string sort_expression, string culture_name)
        {
            UNIMPLEMENTED();

            var result = new List<UplayAction>();
            return Result(result);
        }

        [RMCMethod(2)]
        public RMCResult GetActionsCompleted(int start_row_index, int maximum_rows, string sort_expression, string culture_name)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(3)]
        public RMCResult GetActionsCount(string game_code)
        {
            UNIMPLEMENTED();

            int actions_count = 0;
            return Result(new { actions_count });
        }

        [RMCMethod(4)]
        public RMCResult GetActionsCompletedCount(string game_code)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(5)]
        public RMCResult GetRewards(int start_row_index, int maximum_rows, string sort_expression, string culture_name)
        {
            var rewards = new List<UPlayReward>()
            {

            };

            // return 
            return Result(rewards);
        }

        [RMCMethod(6)]
        public RMCResult GetRewardsPurchased(int startRowIndex, int maximumRows, string sortExpression, string cultureName)
        {
            UNIMPLEMENTED();

            var rewards = new List<UPlayReward>();

            // return 
            return Result(rewards);
        }

        [RMCMethod(7)]
        public RMCResult UplayWelcome(string culture)
        {
            var result = new List<UplayAction>();
            return Result(result);
        }

        [RMCMethod(8)]
        public RMCResult SetActionCompleted(string actionCode, string cultureName)
        {
            UNIMPLEMENTED();
            var unlockedAction = new UplayAction()
            {
                m_code = actionCode,
                m_description = actionCode + "_description",
                m_gameCode = "UNK",
                m_name = actionCode + "_action",
                m_value = 1,
            };
            unlockedAction.m_platforms.Add(new UplayActionPlatform()
            {
                m_completed = true,
                m_platformCode = "PS3",
                m_specificKey = string.Empty
            });

            return Result(unlockedAction);
        }

        [RMCMethod(9)]
        public RMCResult SetActionsCompleted(IEnumerable<string> actionCodeList, string cultureName)
        {
            var actionList = new List<UplayAction>();
            return Result(actionList);
        }

        [RMCMethod(10)]
        public RMCResult GetUserToken()
        {
            UNIMPLEMENTED();
            return Result("TeStToKeN");
        }

        [RMCMethod(11)]
        public RMCResult GetVirtualCurrencyUserBalance()
        {
            int numOfTokens = 0;

            if (Context != null && Context.Client.PlayerInfo != null)
                numOfTokens = DBHelper.GetUbiTokensDataByUserName(Context.Handler.Factory.Item1, Context.Client.PlayerInfo.Name!);

            return Result(new { numOfTokens });
        }

        [RMCMethod(12)]
        public RMCResult GetSectionsByKey(string culture_name, string section_key)
        {
            UNIMPLEMENTED();
            return Error(0);
        }

        [RMCMethod(13)]
        public RMCResult BuyReward(string reward_code)
        {
            UNIMPLEMENTED();
            return Error(0);
        }
    }
}
