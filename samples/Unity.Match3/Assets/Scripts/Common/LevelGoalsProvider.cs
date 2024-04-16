namespace Match3
{
    public class LevelGoalsProvider
    {
        /// 游戏目标集合
        public LevelGoal[] GetLevelGoals(int level, GameBoard gameBoard)
        {
            // 目前只有一个：单次消除一整行
            return new LevelGoal[] { new CollectRowMaxItems(gameBoard) };
        }
    }
}