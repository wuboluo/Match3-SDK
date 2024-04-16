﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Match3
{
    public abstract class Match3Game : BaseGame
    {
        private readonly AnimatedItemSwapper _itemSwapper;

        // 填充策略
        private RiseFillStrategy _fillStrategy;
        private AsyncLazy _swapItemsTask;

        protected Match3Game(GameConfig config) : base(config)
        {
            _itemSwapper = config.ItemSwapper;
        }

        private bool IsSwapItemsCompleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _swapItemsTask == null || _swapItemsTask.Task.Status.IsCompleted();
        }

        public async UniTask StartAsync(CancellationToken cancellationToken = default)
        {
            if (_fillStrategy == null) throw new NullReferenceException(nameof(_fillStrategy));

            await FillBoardAsync(_fillStrategy, cancellationToken);

            StartGame();
        }

        public async UniTask StopAsync()
        {
            if (IsSwapItemsCompleted == false) await _swapItemsTask;

            StopGame();
        }

        public void SetGameBoardFillStrategy(RiseFillStrategy fillStrategy)
        {
            _fillStrategy = fillStrategy;
        }

        protected override void OnAllGoalsAchieved()
        {
            RaiseGameFinishedAsync().Forget();
        }

        /// 棋盘填充
        private async UniTask FillBoardAsync(RiseFillStrategy fillStrategy, CancellationToken cancellationToken = default)
        {
            var fillBoardJobs = fillStrategy.GetBoardFillJobs(GameBoard);
            await ExecuteJobsAsync(fillBoardJobs, cancellationToken);
        }

        /// 交换
        protected UniTask SwapItemsAsync(GridPosition position1, GridPosition position2, CancellationToken cancellationToken = default)
        {
            if (_swapItemsTask?.Task.Status.IsCompleted() ?? true) _swapItemsTask = SwapItemsAsync(_fillStrategy, position1, position2, cancellationToken).ToAsyncLazy();

            return _swapItemsTask.Task;
        }

        /// 交换
        private async UniTask SwapItemsAsync(RiseFillStrategy fillStrategy, GridPosition position1, GridPosition position2, CancellationToken cancellationToken = default)
        {
            // 交换动画
            await SwapGameBoardItemsAsync(position1, position2, cancellationToken);

            // 成功消除
            if (IsSolved(position1, position2, out var solvedData))
            {
                // 序列消除时，计分等
                NotifySequencesSolved(solvedData);

                // 列消除Jobs
                var solveJobs = fillStrategy.GetSolveJobs(GameBoard, solvedData);
                await ExecuteJobsAsync(solveJobs, cancellationToken);
            }
            else
            {
                // 交换失败，再换回来
                await SwapGameBoardItemsAsync(position1, position2, cancellationToken);
            }
        }

        /// 播放道具交换动画
        private async UniTask SwapGameBoardItemsAsync(GridPosition position1, GridPosition position2, CancellationToken cancellationToken = default)
        {
            var gridSlot1 = GameBoard[position1.RowIndex, position1.ColumnIndex];
            var gridSlot2 = GameBoard[position2.RowIndex, position2.ColumnIndex];

            await _itemSwapper.SwapItemsAsync(gridSlot1, gridSlot2, cancellationToken);
        }

        /// 执行Jobs
        private static UniTask ExecuteJobsAsync(IEnumerable<IJob> jobs, CancellationToken cancellationToken = default)
        {
            return JobsExecutor.ExecuteJobsAsync(jobs, cancellationToken);
        }

        private async UniTask RaiseGameFinishedAsync()
        {
            if (IsSwapItemsCompleted == false) await _swapItemsTask;

            base.OnAllGoalsAchieved();
        }
    }
}