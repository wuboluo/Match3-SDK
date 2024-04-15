using Common.Interfaces;
using Common.LevelGoals;
using Match3.App;
using Match3.App.Interfaces;

namespace Common
{
    public class LevelGoalsProvider : ILevelGoalsProvider<IUnityGridSlot>
    {
        /// 游戏目标集合
        public LevelGoal<IUnityGridSlot>[] GetLevelGoals(int level, IGameBoard<IUnityGridSlot> gameBoard)
        {
            // 目前只有一个：单次消除一整行
            return new LevelGoal<IUnityGridSlot>[] { new CollectRowMaxItems(gameBoard) };
        }
    }
}