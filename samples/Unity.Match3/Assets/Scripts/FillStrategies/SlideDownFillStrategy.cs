using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Common.Interfaces;
using FillStrategies.Jobs;
using FillStrategies.Models;
using Match3.App;
using Match3.App.Interfaces;
using Match3.Core.Structs;

namespace FillStrategies
{
    // 下滑填充策略
    public class SlideDownFillStrategy : BaseFillStrategy
    {
        public SlideDownFillStrategy(IAppContext appContext) : base(appContext)
        {
        }

        public override string Name => "Slide Down Fill Strategy";

        public override IEnumerable<IJob> GetSolveJobs(IGameBoard<IUnityGridSlot> gameBoard, SolvedData<IUnityGridSlot> solvedData)
        {
            // 任务集合
            var jobs = new List<IJob>();
            var itemsToHide = new List<IUnityItem>();

            // 被消除的棋子集合
            var solvedGridSlots = new HashSet<IUnityGridSlot>();

            // 记录序列中消除的棋子
            foreach (var solvedGridSlot in solvedData.GetSolvedGridSlots())
            {
                if (!solvedGridSlot.IsMovable)
                {
                    continue;
                }

                // 用hashSet是为了可以在TL型过滤掉共用的棋子
                if (!solvedGridSlots.Add(solvedGridSlot))
                {
                    continue;
                }

                var currentItem = solvedGridSlot.Item;
                itemsToHide.Add(currentItem);
                solvedGridSlot.Clear();

                // 回收棋子
                RecycleItemToPool(currentItem);
            }

            // 记录特殊消除的棋子
            foreach (var specialItemGridSlot in solvedData.GetSpecialItemGridSlots())
            {
                solvedGridSlots.Add(specialItemGridSlot);
            }

            foreach (var solvedGridSlot in solvedGridSlots.OrderBy(slot => CanDropFromTop(gameBoard, slot.GridPosition)))
            {
                var itemsMoveData = GetColumnItemsMoveData(gameBoard, solvedGridSlot.GridPosition.ColumnIndex);
                if (itemsMoveData.Count != 0)
                {
                    jobs.Add(new ItemsMoveJob(itemsMoveData));
                }
            }

            solvedGridSlots.Clear();
            jobs.Add(new ItemsHideJob(itemsToHide));
            jobs.AddRange(GetRollDownJobs(gameBoard, 1, 0));
            jobs.AddRange(GetFillJobs(gameBoard, 0, 1));

            return jobs;
        }

        private IEnumerable<IJob> GetFillJobs(IGameBoard<IUnityGridSlot> gameBoard, int delayMultiplier, int executionOrder)
        {
            var jobs = new List<IJob>();

            for (var columnIndex = 0; columnIndex < gameBoard.ColumnCount; columnIndex++)
            {
                var gridSlot = gameBoard[0, columnIndex];
                if (!gridSlot.CanSetItem)
                {
                    continue;
                }

                jobs.Add(new ItemsRiseJob(GetGenerateJobs(gameBoard, columnIndex), delayMultiplier, executionOrder));
            }

            return jobs;
        }

        private IEnumerable<ItemMoveData> GetGenerateJobs(IGameBoard<IUnityGridSlot> gameBoard, int columnIndex)
        {
            var gridSlot = gameBoard[0, columnIndex];
            var itemsDropData = new List<ItemMoveData>();

            while (!gridSlot.HasItem)
            {
                var item = FetchItemFromPool();
                var itemGeneratorPosition = new GridPosition(-1, columnIndex);
                item.SetWorldPosition(GetWorldPosition(itemGeneratorPosition));

                var dropPositions = FilterPositions(gridSlot.GridPosition, GetRisePositions(gameBoard, gridSlot));
                if (dropPositions.Count == 0)
                {
                    gridSlot.SetItem(item);
                    itemsDropData.Add(new ItemMoveData(item, new[] { GetWorldPosition(gridSlot.GridPosition) }));
                    break;
                }

                var destinationGridPosition = dropPositions[^1];
                var destinationGridSlot = gameBoard[destinationGridPosition];

                destinationGridSlot.SetItem(item);
                itemsDropData.Add(new ItemMoveData(item, dropPositions.Select(GetWorldPosition).ToArray()));
            }

            itemsDropData.Reverse();

            return itemsDropData;
        }

        private IEnumerable<IJob> GetRollDownJobs(IGameBoard<IUnityGridSlot> gameBoard, int delayMultiplier, int executionOrder)
        {
            var jobs = new List<IJob>();

            for (var rowIndex = gameBoard.RowCount - 1; rowIndex >= 0; rowIndex--)
            {
                var itemsMoveData = GetRowItemsMoveData(gameBoard, rowIndex);

                if (itemsMoveData.Count != 0)
                {
                    jobs.Add(new ItemsMoveJob(itemsMoveData, delayMultiplier, executionOrder));
                }
            }

            return jobs;
        }

        private static bool CanDropFromTop(IGameBoard<IUnityGridSlot> gameBoard, GridPosition gridPosition)
        {
            // 被消除的格子位置
            var rowIndex = gridPosition.RowIndex;
            var columnIndex = gridPosition.ColumnIndex;

            while (rowIndex >= 0)
            {
                var gridSlot = gameBoard[rowIndex, columnIndex];
                if (gridSlot.NotAvailable)
                {
                    return false;
                }

                rowIndex--;
            }

            return true;
        }

        /// 某行中需要移动的棋子
        private List<ItemMoveData> GetRowItemsMoveData(IGameBoard<IUnityGridSlot> gameBoard, int rowIndex)
        {
            var itemsMoveData = new List<ItemMoveData>();

            for (var columnIndex = 0; columnIndex < gameBoard.ColumnCount; columnIndex++)
            {
                var itemMoveData = GetItemMoveData(gameBoard, rowIndex, columnIndex);
                if (itemMoveData != null)
                {
                    itemsMoveData.Add(itemMoveData);
                }
            }

            itemsMoveData.Reverse();
            return itemsMoveData;
        }

        /// 某列中需要移动的棋子
        private List<ItemMoveData> GetColumnItemsMoveData(IGameBoard<IUnityGridSlot> gameBoard, int columnIndex)
        {
            var itemsMoveData = new List<ItemMoveData>();

            for (var rowIndex = gameBoard.RowCount - 1; rowIndex >= 0; rowIndex--)
            {
                var itemMoveData = GetItemMoveData(gameBoard, rowIndex, columnIndex);
                if (itemMoveData != null)
                {
                    itemsMoveData.Add(itemMoveData);
                }
            }

            itemsMoveData.Reverse();
            return itemsMoveData;
        }

        private ItemMoveData GetItemMoveData(IGameBoard<IUnityGridSlot> gameBoard, int rowIndex, int columnIndex)
        {
            var gridSlot = gameBoard[rowIndex, columnIndex];
            if (!gridSlot.IsMovable)
            {
                return null;
            }

            var dropPositions = FilterPositions(gridSlot.GridPosition, GetRisePositions(gameBoard, gridSlot));
            if (dropPositions.Count == 0)
            {
                return null;
            }

            var item = gridSlot.Item;
            gridSlot.Clear();
            gameBoard[dropPositions.Last()].SetItem(item);

            return new ItemMoveData(item, dropPositions.Select(GetWorldPosition).ToArray());
        }

        /// 上升经过的格子集合
        private static List<GridPosition> GetRisePositions(IGameBoard<IUnityGridSlot> gameBoard, IUnityGridSlot gridSlot)
        {
            var riseGridPositions = new List<GridPosition>();

            // 直到上方临格不是空格
            while (gameBoard.CanMoveUp(gridSlot, out var upGridPosition))
            {
                gridSlot = gameBoard[upGridPosition];
                riseGridPositions.Add(upGridPosition);
            }

            // 能否可以斜方向滑落
            if (!CanRiseDiagonally(gameBoard, gridSlot, out var diagonalGridPosition))
            {
                return riseGridPositions;
            }

            riseGridPositions.Add(diagonalGridPosition);
            riseGridPositions.AddRange(GetRisePositions(gameBoard, gameBoard[diagonalGridPosition]));

            return riseGridPositions;
        }

        private static bool CanRiseDiagonally(IGameBoard<IUnityGridSlot> gameBoard, IUnityGridSlot gridSlot, out GridPosition gridPosition)
        {
            return CanRiseDiagonally(gameBoard, gridSlot, GridPosition.Left, out gridPosition) ||
                   CanRiseDiagonally(gameBoard, gridSlot, GridPosition.Right, out gridPosition);
        }

        private static bool CanRiseDiagonally(IGameBoard<IUnityGridSlot> gameBoard, IUnityGridSlot gridSlot, GridPosition direction, out GridPosition gridPosition)
        {
            var sideGridSlot = gameBoard.GetSideGridSlot(gridSlot, direction);
            var upGridSlot = gameBoard.GetSideGridSlot(gridSlot, GridPosition.Up);

            if (sideGridSlot is { NotAvailable: true } && upGridSlot != null && !upGridSlot.State.IsLocked)
            {
                return gameBoard.CanMoveUp(sideGridSlot, out gridPosition);
            }

            gridPosition = GridPosition.Zero;
            return false;
        }

        private static List<GridPosition> FilterPositions(GridPosition currentGridPosition, List<GridPosition> gridPositions)
        {
            if (gridPositions.Count is 0 or 1)
            {
                return gridPositions;
            }

            var startColumnIndex = currentGridPosition.ColumnIndex;
            var filteredGridPositions = new HashSet<GridPosition>();

            for (var i = 0; i < gridPositions.Count; i++)
            {
                var gridPosition = gridPositions[i];

                if (startColumnIndex == gridPosition.ColumnIndex)
                {
                    if (i == gridPositions.Count - 1)
                    {
                        filteredGridPositions.Add(gridPosition);
                    }

                    continue;
                }

                if (i > 0)
                {
                    filteredGridPositions.Add(gridPositions[i - 1]);
                }

                filteredGridPositions.Add(gridPosition);

                startColumnIndex = gridPosition.ColumnIndex;
            }

            return filteredGridPositions.ToList();
        }
    }
}