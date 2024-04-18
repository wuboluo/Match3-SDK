using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Match3
{
    /// 同列上升填充策略组件
    public class Game_FillComponent : Component
    {
        private readonly Game_ItemPoolComponent _itemPoolComponent;
        private readonly Game_BoardComponent _boardComponent;

        public Game_FillComponent()
        {
            _itemPoolComponent = World.Instance.Root.GetComponent<Game_ItemPoolComponent>();
            _boardComponent = World.Instance.Root.GetComponent<Game_BoardComponent>();
        }

        /// 填充Jobs（开局填充满整个棋盘）
        public IEnumerable<IJob> GetBoardFillJobs()
        {
            var showItemComponents = new List<Game_ItemComponent>();

            for (var row = 0; row < _boardComponent.RowCount; row++)
            for (var column = 0; column < _boardComponent.ColumnCount; column++)
            {
                var slotComponent = _boardComponent.GetSlotComponent(row, column);
                var itemComponent = _itemPoolComponent.Fetch();

                // 道具的坐标 = 格子的坐标
                itemComponent.SetWorldPosition(GetWorldPosition(slotComponent.GridComponent.GetGrid()));
                slotComponent.SetItem(itemComponent);

                showItemComponents.Add(itemComponent);
            }

            return new[] { new Job_ShowAllItems(showItemComponents) };
        }

        /// 消除Jobs（消除Job + 向上补齐Job + 填满棋盘Job）
        public IEnumerable<IJob> GetSolveJobs(SolvedSlotsData solvedSlotsData)
        {
            var jobs = new List<IJob>();

            // 用hashSet是为了可以在TL型过滤掉共用的棋子
            var solvedSlotComponents = new HashSet<Game_SlotComponent>();

            // 1. 消除道具Jobs
            var hideItemComponents = new List<Game_ItemComponent>();
            {
                // 序列中的
                foreach (var slotComponent in solvedSlotsData.GetSolvedSlotComponentsInSequences())
                {
                    HandleSolvedSlot(slotComponent);
                }

                // 特殊消除的
                foreach (var slotComponent in solvedSlotsData.GetSpecialSolvedSlotComponents())
                {
                    HandleSolvedSlot(slotComponent);
                }

                jobs.Add(new Job_HideItems(hideItemComponents));

                // 处理被消除的格子。移除格子中的道具，并把这个道具添加进Hide列表中
                void HandleSolvedSlot(Game_SlotComponent _slotComponent)
                {
                    if (_slotComponent.HasItem && solvedSlotComponents.Add(_slotComponent))
                    {
                        hideItemComponents.Add(_slotComponent.ItemComponent);
                        _slotComponent.SetItem(null);
                    }
                }
            }

            // 2. 所有被消除的格子所在列，下方的道具顶上去Jobs
            var subUpFillJobs = new List<IJob>();
            {
                foreach (var slotComponent in solvedSlotComponents)
                {
                    // 被消除格子所在列中，可能会向上补齐的道具路径
                    var paths = GetUpFillItemPaths(slotComponent.GridComponent.GetGrid().ColumnIndex);
                    if (paths.Count > 0)
                    {
                        subUpFillJobs.Add(new Job_MoveItems(paths));
                    }
                }

                jobs.AddRange(subUpFillJobs);
            }

            // 3. 从棋盘底下向上填充满棋盘的Jobs
            jobs.AddRange(NewUpItemPaths());

            return jobs;
        }

        /// 从棋盘下面补充上来的新的道具路径Jobs
        private IEnumerable<IJob> NewUpItemPaths()
        {
            var moveJobs = new List<IJob>();

            // 遍历整个棋盘
            for (var column = 0; column < _boardComponent.ColumnCount; column++)
            {
                // 一列中所有需要移动的道具路径集合
                var movePaths = new List<ItemMovePathData>();

                for (var row = 0; row < _boardComponent.RowCount; row++)
                {
                    // 如果这个格子里有道具，就不需要填充
                    var slotComponent = _boardComponent.GetSlotComponent(row, column);
                    if (!slotComponent.HasItem)
                    {
                        // 在-1行创建一个新的道具，并在道具最终所在的格子绑定这个道具
                        var itemComponent = _itemPoolComponent.Fetch();
                        itemComponent.SetWorldPosition(GetWorldPosition(new Grid(-1, column)));
                        slotComponent.SetItem(itemComponent);

                        // 设置新道具的路径，终点为刚才设置的对应的格子
                        var path = new ItemMovePathData(itemComponent, new[] { GetWorldPosition(slotComponent.GridComponent.GetGrid()) });

                        // 加到这列中
                        movePaths.Add(path);
                    }
                }

                // 如果此列存在需要移动的格子
                if (movePaths.Count > 0)
                {
                    moveJobs.Add(new Job_RiseItems(movePaths, delayMultiplier: 1));
                }
            }

            return moveJobs;
        }

        /// 一列中，上面的被消除了，下面的要顶上去。下面需要移动的道具的路径
        private List<ItemMovePathData> GetUpFillItemPaths(int column)
        {
            var movePaths = new List<ItemMovePathData>();

            // 从最上面那行往下找
            for (var row = _boardComponent.RowCount - 1; row >= 0; row--)
            {
                var slotComponent = _boardComponent.GetSlotComponent(row, column);

                // 只考虑存在道具的格子，因为如果一列中上面的被消除了，下面的格子要顶上去
                if (slotComponent.HasItem)
                {
                    // 找到这个格子可以顶到的最高行
                    if (CanUp(slotComponent, out var endGrid))
                    {
                        // 把这个格子中的道具清空，因为这个道具上移了
                        var item = slotComponent.ItemComponent;
                        slotComponent.SetItem(null);

                        // 道具上移的路径
                        movePaths.Add(new ItemMovePathData(item, new[] { GetWorldPosition(endGrid) }));

                        // 把这个道具设置到终点的格子
                        _boardComponent.GetSlotComponent(endGrid).SetItem(item);
                    }
                }
            }

            // 因为是从上往下找的，所以先被加进去的是最上面的行，翻转过来就可以由上打头一起上去
            movePaths.Reverse();
            return movePaths;
        }

        /// 能否上升
        private bool CanUp(Game_SlotComponent slotComponent, out Grid endGrid)
        {
            // 记录初始位置，下面会用这个去模拟上移，找到最高能移动到的行对应的格子
            var tempSlot = slotComponent;

            // 直到上方邻格不为空，否则目的地格子就依次向上
            while (CanMoveUp(tempSlot, out var topGrid))
            {
                tempSlot = _boardComponent.GetSlotComponent(topGrid);
            }

            // 如果目的地和当前的一样，说明开始时上方就不为空，那么就无法上移
            endGrid = tempSlot.GridComponent.GetGrid();
            return tempSlot != slotComponent;
        }

        /// 能否向上移动
        public bool CanMoveUp(Game_SlotComponent slotComponent, out Grid grid)
        {
            // 如果上方邻格是空的
            var topAdjacentSlot = GetSideGridSlot(slotComponent, Grid.Up);
            if (topAdjacentSlot is { HasItem: false })
            {
                grid = topAdjacentSlot.GridComponent.GetGrid();
                return true;
            }

            grid = Grid.Zero;
            return false;
        }

        /// 获取某方向的邻格
        private Game_SlotComponent GetSideGridSlot(Game_SlotComponent slotComponent, Grid dir)
        {
            var adjacentSlot = slotComponent.GridComponent.GetGrid() + dir;

            return _boardComponent.IsInBoard(adjacentSlot)
                ? _boardComponent.GetSlotComponent(adjacentSlot)
                : null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Vector3 GetWorldPosition(Grid grid)
        {
            return _boardComponent.GetWorldPosition(grid);
        }
    }
}