// using System.Threading;
// using Cysharp.Threading.Tasks;
//
// namespace Match3
// {
//     public class Game_SwapComponent : Component
//     {
//         /// 交换
//         public UniTask SwapItemsAsync(GridPosition position1, GridPosition position2, CancellationToken cancellationToken = default)
//         {
//             if (_swapItemsTask?.Task.Status.IsCompleted() ?? true)
//             {
//                 var _riseFillStrategyComponent = World.Instance.Root.GetComponent<Game_RiseFillStrategyComponent>();
//                 _swapItemsTask = SwapItemsAsync(_riseFillStrategyComponent, position1, position2, cancellationToken).ToAsyncLazy();
//             }
//
//             return _swapItemsTask.Task;
//         }
//
//         /// 交换
//         private async UniTask SwapItemsAsync(Game_RiseFillStrategyComponent fillStrategyComponent, GridPosition pos1, GridPosition pos2, CancellationToken cancellationToken = default)
//         {
//             // 交换动画
//             await SwapGameBoardItemsAsync(pos1, pos2, cancellationToken);
//
//             // 成功消除
//             if (IsSolved(pos1, pos2, out var solvedData))
//             {
//                 // 序列消除时，计分等
//                 NotifySequencesSolved(solvedData);
//
//                 // 列消除Jobs
//                 var solveJobs = fillStrategyComponent.GetSolveJobs(GameBoard, solvedData);
//                 await World.Instance.Root.GetComponent<Game_JobComponent>().Execute(solveJobs, cancellationToken);
//             }
//             else
//             {
//                 // 交换失败，再换回来
//                 await SwapGameBoardItemsAsync(pos1, pos2, cancellationToken);
//             }
//         }
//
//         /// 播放道具交换动画
//         private async UniTask SwapGameBoardItemsAsync(GridPosition position1, GridPosition position2, CancellationToken cancellationToken = default)
//         {
//             var gridSlot1 = GameBoard[position1.RowIndex, position1.ColumnIndex];
//             var gridSlot2 = GameBoard[position2.RowIndex, position2.ColumnIndex];
//
//             var animatedItemSwapperComponent = World.Instance.Root.GetComponent<Game_AnimatedItemSwapperComponent>();
//             await animatedItemSwapperComponent.SwapItemsAsync(gridSlot1, gridSlot2, cancellationToken);
//         }
//     }
// }