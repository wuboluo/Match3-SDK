using System;

namespace Match3
{
    /// 游戏目标
    public abstract class LevelGoal
    {
        /// 是否完成
        public bool IsAchieved { get; private set; }

        /// 完成时
        public event EventHandler Achieved;

        /// 序列消除时（可能消除后就完成了某个目标）
        public abstract void OnSequencesSolved(SolvedData solvedData);

        /// 标记为已完成
        protected void MarkAchieved()
        {
            IsAchieved = true;
            Achieved?.Invoke(this, EventArgs.Empty);
        }
    }
}