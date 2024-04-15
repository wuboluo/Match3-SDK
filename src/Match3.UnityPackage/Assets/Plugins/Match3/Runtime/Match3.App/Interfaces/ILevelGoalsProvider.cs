using Match3.Core.Interfaces;

namespace Match3.App.Interfaces
{
    public interface ILevelGoalsProvider<TGridSlot> where TGridSlot : IGridSlot
    {
        /// 获取该等级对应的目标
        LevelGoal<TGridSlot>[] GetLevelGoals(int level, IGameBoard<TGridSlot> gameBoard);
    }
}