using System;

namespace Match3
{
    public abstract class BaseGame : IDisposable
    {
        private readonly GameBoardSolver _gameBoardSolver;

        private bool _isStarted;

        protected BaseGame(GameConfig config)
        {
            _gameBoardSolver = config.GameBoardSolver;
            GameBoard = new GameBoard();
        }

        public void SetGridSlots(UnityGridSlot[,] gridSlots)
        {
            GameBoard.SetGridSlots(gridSlots);
        }

        protected GameBoard GameBoard { get; }

        public void Dispose()
        {
            GameBoard?.Dispose();
        }

        protected void StartGame()
        {
            if (_isStarted) throw new InvalidOperationException("Game has already been started.");

            // foreach (var levelGoal in _levelGoals)
            // {
            //     levelGoal.Achieved += OnLevelGoalAchieved;
            // }

            _isStarted = true;
            OnGameStarted();
        }

        protected void StopGame()
        {
            if (_isStarted == false) throw new InvalidOperationException("Game has not been started.");

            // foreach (var levelGoal in _levelGoals)
            // {
            //     levelGoal.Achieved -= OnLevelGoalAchieved;
            // }

            _isStarted = false;
            OnGameStopped();
        }

        public void ResetGameBoard()
        {
            GameBoard.ResetState();
        }

        protected abstract void OnGameStarted();
        protected abstract void OnGameStopped();

        /// 是否成功消除
        protected bool IsSolved(GridPosition position1, GridPosition position2, out SolvedData solvedData)
        {
            solvedData = _gameBoardSolver.Solve(GameBoard, position1, position2);

            // 至少要有一个可被消除的序列
            return solvedData.SolvedSequences.Count > 0;
        }

        /// 序列消除时
        protected void NotifySequencesSolved(SolvedData solvedData)
        {
            // 每个序列计分
            // foreach (var sequencesConsumer in _solvedSequencesConsumers)
            // {
            //     sequencesConsumer.OnSequencesSolved(solvedData);
            // }

            // 检查有没有在这次消除过后，完成的游戏目标
            // foreach (var levelGoal in _levelGoals)
            // {
            //     // 目标未完成
            //     if (!levelGoal.IsAchieved)
            //     {
            //         levelGoal.OnSequencesSolved(solvedData);
            //     }
            // }
        }

        /// 所有目标均完成
        protected virtual void OnAllGoalsAchieved()
        {
        }

        // /// 当一次消除一整行后，完成成就
        // private void OnLevelGoalAchieved(object sender, EventArgs e)
        // {
        //     LevelGoalAchieved?.Invoke(this, (LevelGoal) sender);
        //
        //     _achievedGoals++;
        //     if (_achievedGoals == _levelGoals.Length)
        //     {
        //         OnAllGoalsAchieved();
        //     }
        // }
    }
}