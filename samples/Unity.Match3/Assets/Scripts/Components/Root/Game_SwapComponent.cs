using System.Threading;
using Cysharp.Threading.Tasks;

namespace Match3
{
    public class Game_SwapComponent : Component
    {
        private AsyncLazy _swapItemsTask;

        /// 交换
        public UniTask SwapItems(Grid pos1, Grid pos2, CancellationToken cancellationToken = default)
        {
            if (_swapItemsTask?.Task.Status.IsCompleted() ?? true)
            {
                _swapItemsTask = SwapItemsAsync(pos1, pos2, cancellationToken).ToAsyncLazy();
            }

            return _swapItemsTask.Task;
        }

        /// 交换
        private async UniTask SwapItemsAsync(Grid pos1, Grid pos2, CancellationToken cancellationToken = default)
        {
            var solveComponent = World.Instance.Root.GetComponent<Game_SolveComponent>();

            // 交换动画
            await SwapGameBoardItemsAsync(pos1, pos2, cancellationToken);

            // 成功消除
            if (solveComponent.IsSolved(pos1, pos2, out var solvedData))
            {
                // 序列消除时，计分等
                solveComponent.NotifySequencesSolved(solvedData);

                // 列消除Jobs
                var fillComponent = World.Instance.Root.GetComponent<Game_FillComponent>();
                var solveJobs = fillComponent.GetSolveJobs(solvedData);
                await World.Instance.Root.GetComponent<Game_JobComponent>().Execute(solveJobs, cancellationToken);
            }
            else
            {
                // 交换失败，再换回来
                await SwapGameBoardItemsAsync(pos1, pos2, cancellationToken);
            }
        }

        /// 播放道具交换动画
        private async UniTask SwapGameBoardItemsAsync(Grid grid1, Grid grid2, CancellationToken cancellationToken = default)
        {
            var boardComponent = World.Instance.Root.GetComponent<Game_BoardComponent>();

            var slot1 = boardComponent.GetSlotComponent(grid1);
            var slot2 = boardComponent.GetSlotComponent(grid2);

            // 交换Jobs
            var swapJobs = new[] { new Job_SwapSlots(slot1, slot2) };
            await World.Instance.Root.GetComponent<Game_JobComponent>().Execute(swapJobs, cancellationToken);
        }
    }
}