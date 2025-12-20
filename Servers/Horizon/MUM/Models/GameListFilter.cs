using Horizon.RT.Common;

namespace Horizon.MUM.Models
{
    public class GameListFilter
    {
        public uint FieldID = 0;
        public MediusGameListFilterField FilterField = 0;
        public ulong BaselineValue = 0;
        public MediusComparisonOperator ComparisonOperator = MediusComparisonOperator.LESS_THAN;
        public int Mask = -1;

        public bool IsMatch(Game game)
        {
            if (game == null)
                return false;

            #region FilterField
            switch (FilterField)
            {
                case MediusGameListFilterField.MEDIUS_FILTER_GAME_LEVEL: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.GameLevel & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_GENERIC_FIELD_1: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.GenericField1 & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_GENERIC_FIELD_2: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.GenericField2 & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_GENERIC_FIELD_3: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.GenericField3 & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_GENERIC_FIELD_4: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.GenericField4 & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_GENERIC_FIELD_5: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.GenericField5 & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_GENERIC_FIELD_6: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.GenericField6 & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_GENERIC_FIELD_7: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.GenericField7 & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_GENERIC_FIELD_8: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.GenericField8 & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_LOBBY_WORLDID: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.MediusWorldId & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_MAX_PLAYERS: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.MaxPlayers & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_MIN_PLAYERS: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.MinPlayers & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_PLAYER_COUNT: return ComparisonOperator.Compare((ulong)(game.PlayerCount & Mask), BaselineValue);
                case MediusGameListFilterField.MEDIUS_FILTER_PLAYER_SKILL_LEVEL: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.PlayerSkillLevel & Mask));
                case MediusGameListFilterField.MEDIUS_FILTER_RULES_SET: return ComparisonOperator.Compare(BaselineValue, (ulong)(game.RulesSet & Mask));
                default: return false;
            }
            #endregion
        }

        public override string ToString()
        {
            return $"({FieldID}:{FilterField}:{BaselineValue}:{ComparisonOperator}:{Mask})";
        }
    }
}