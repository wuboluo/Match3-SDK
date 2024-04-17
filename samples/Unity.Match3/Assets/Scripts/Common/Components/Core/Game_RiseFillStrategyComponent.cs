using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Match3
{
    /// 同列上升填充策略组件
    public class Game_RiseFillStrategyComponent : Component
    {
        private readonly Game_ItemGenerateComponent _itemGenerateComponent;
        private readonly Game_BoardComponent _boardComponent;

        public Game_RiseFillStrategyComponent()
        {
            _itemGenerateComponent = World.Instance.Root.GetComponent<Game_ItemGenerateComponent>();
            _boardComponent = World.Instance.Root.GetComponent<Game_BoardComponent>();
        }

        /// 棋盘填充Jobs
        public IEnumerable<IJob> GetBoardFillJobs()
        {
            // todo 要不要换个地方写
            // gameBoard.SetGridSlots(_gridSlots);

            var itemsToShow = new List<Game_ItemComponent>();

            for (var rowIndex = 0; rowIndex < _boardComponent.RowCount; rowIndex++)
            for (var columnIndex = 0; columnIndex < _boardComponent.ColumnCount; columnIndex++)
            {
                var gridSlot = _boardComponent.GetGridSlot(rowIndex, columnIndex);
                if (!gridSlot.HasItem) continue;

                var item = FetchItemFromPool();
                item.SetWorldPosition(GetWorldPosition(gridSlot.GridPosition));

                gridSlot.SetItem(item);
                itemsToShow.Add(item);
            }

            return new[] { new ItemsShowJob(itemsToShow) };
        }

        /// 列填充Jobs
        public IEnumerable<IJob> GetFillJobs()
        {
            var jobs = new List<IJob>();

            for (var columnIndex = 0; columnIndex < _boardComponent.ColumnCount; columnIndex++)
            {
                // 每一列的上升填充Job
                var itemsRiseData = new List<ItemMoveData>();

                for (var rowIndex = 0; rowIndex < _boardComponent.RowCount; rowIndex++)
                {
                    var gridSlot = _boardComponent.GetGridSlot(rowIndex, columnIndex);
                    if (gridSlot.HasItem)
                    {
                        // 障碍就不填充，跳过
                        continue;
                    }

                    // 生成道具在该列合适的行数
                    var item = FetchItemFromPool();
                    var itemGeneratorPosition = GetItemGeneratorPosition(rowIndex, columnIndex);
                    item.SetWorldPosition(GetWorldPosition(itemGeneratorPosition));

                    // 给这个道具绑定一个移动到对应行的移动Job
                    var itemRiseData = new ItemMoveData(item, new[] { GetWorldPosition(gridSlot.GridPosition) });

                    // 格子绑定道具
                    gridSlot.SetItem(item);

                    // 该列上升Job包含每个格子的移动
                    itemsRiseData.Add(itemRiseData);
                }

                // 如果此列存在需要移动的格子
                if (itemsRiseData.Count > 0)
                {
                    jobs.Add(new ItemsRiseJob(itemsRiseData, delayMultiplier: 1));
                }
            }

            return jobs;
        }

        /// 列消除Jobs
        public IEnumerable<IJob> GetSolveJobs(SolvedData solvedData)
        {
            var jobs = new List<IJob>();
            var itemsToHide = new List<Game_ItemComponent>();
            var solvedGridSlots = new HashSet<Game_SlotComponent>();

            // 记录序列中消除的棋子
            foreach (var solvedGridSlot in solvedData.GetSolvedGridSlots())
            {
                if (!solvedGridSlot.HasItem) continue;

                // 用hashSet是为了可以在TL型过滤掉共用的棋子
                if (!solvedGridSlots.Add(solvedGridSlot)) continue;

                var currentItem = solvedGridSlot.ItemComponent;
                itemsToHide.Add(currentItem);
                solvedGridSlot.Dispose();

                RecycleItemToPool(currentItem);
            }

            // 被特殊消除的棋子
            foreach (var specialItemGridSlot in solvedData.GetSpecialItemGridSlots())
            {
                solvedGridSlots.Add(specialItemGridSlot);
            }

            foreach (var solvedGridSlot in solvedGridSlots)
            {
                var itemsMoveData = GetItemsMoveData(solvedGridSlot.GridPosition.ColumnIndex);
                if (itemsMoveData.Count != 0)
                {
                    // 道具移动Job
                    jobs.Add(new ItemsMoveJob(itemsMoveData));
                }
            }

            solvedGridSlots.Clear();
            jobs.Add(new ItemsHideJob(itemsToHide));
            jobs.AddRange(GetFillJobs());

            return jobs;
        }

        /// 获取某一列上发生移动的格子的道具和其终点位置
        private List<ItemMoveData> GetItemsMoveData(int columnIndex)
        {
            var itemsRiseData = new List<ItemMoveData>();

            // 从地图最上一行依次向下
            for (var rowIndex = _boardComponent.RowCount - 1; rowIndex >= 0; rowIndex--)
            {
                // 每行中指定列的格子
                var gridSlot = _boardComponent.GetGridSlot(rowIndex, columnIndex);
                if (!gridSlot.HasItem)
                {
                    continue;
                }

                // 这个格子能否向上移动
                if (!CanRise(gridSlot, out var destinationGridPosition))
                {
                    continue;
                }

                var item = gridSlot.ItemComponent;
                gridSlot.Dispose();
                itemsRiseData.Add(new ItemMoveData(item, new[] { GetWorldPosition(destinationGridPosition) }));
                _boardComponent.GetGridSlot(destinationGridPosition).SetItem(item);
            }

            itemsRiseData.Reverse();
            return itemsRiseData;
        }

        /// 获取道具生成位置
        private GridPosition GetItemGeneratorPosition(int rowIndex, int columnIndex)
        {
            // 从消除位置依次向下检查，如果碰到障碍，就从障碍处生成
            while (rowIndex >= 0)
            {
                // var gridSlot = _boardComponent.GetGridSlot(rowIndex, columnIndex);
                // if (gridSlot.NotAvailable) return new GridPosition(rowIndex, columnIndex);

                rowIndex--;
            }

            // 默认从最下面行下面生成
            return new GridPosition(-1, columnIndex);
        }

        /// 能否上升
        private bool CanRise(Game_SlotComponent gridSlotComponent, out GridPosition destinationGridPosition)
        {
            // 目的地格子位置
            var destinationGridSlot = gridSlotComponent;

            // 直到上方临格不为空，否则目的地格子就依次向上
            while (_boardComponent.CanMoveUp(destinationGridSlot, out var topGridPosition))
            {
                destinationGridSlot = _boardComponent.GetGridSlot(topGridPosition);
            }

            // 如果目的地和当前的一样，说明开始时上方就不为空，那么就无法上移
            destinationGridPosition = destinationGridSlot.GridPosition;
            return destinationGridSlot != gridSlotComponent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return _boardComponent.GetWorldPosition(gridPosition);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Game_ItemComponent FetchItemFromPool()
        {
            return _itemGenerateComponent.FetchItem();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RecycleItemToPool(Game_ItemComponent itemComponent)
        {
            _itemGenerateComponent.RecycleItem(itemComponent);
        }
    }
}