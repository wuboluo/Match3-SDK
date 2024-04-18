using System.Collections.Generic;

namespace Match3
{
    /// 线性检测器
    public class Game_LineDetectComponent : Component
    {
        /// 按方向获取序列
        public bool GetSequenceByDirection(Grid grid, IEnumerable<Grid> directions, out LinearSolveSequenceData sequenceData)
        {
            var boardComponent = World.Instance.Root.GetComponent<Game_BoardComponent>();

            // 被消除的格子集合
            var solvedSlotComponents = new List<Game_SlotComponent>();

            // 被检查的格子
            var slotComponent = boardComponent.GetSlotComponent(grid);

            // 上下左右 4遍
            foreach (var dir in directions)
            {
                var sameSlotComponents = CollectSameClotsInDirection(slotComponent, grid, dir);
                solvedSlotComponents.AddRange(sameSlotComponents);
            }

            // 加上自己至少3个才能消除
            solvedSlotComponents.Add(slotComponent);
            if (solvedSlotComponents.Count >= 3)
            {
                // Horizontal 或 Vertical
                var sequenceDetectorType = GetType();

                // 可被消除的道具序列
                sequenceData = new LinearSolveSequenceData(sequenceDetectorType, solvedSlotComponents);
                return true;
            }

            sequenceData = default;
            return false;
        }

        /// 从一个指定的格子开始，沿着给定的方向，递归检查并收集直到遇到不同类型道具的同类型道具的格子
        private static IEnumerable<Game_SlotComponent> CollectSameClotsInDirection(Game_SlotComponent slotComponent, Grid grid, Grid dir)
        {
            var boardComponent = World.Instance.Root.GetComponent<Game_BoardComponent>();

            // 当前格子上下左右的邻格
            var adjacentGrid = grid + dir;
            var slotComponents = new List<Game_SlotComponent>();

            // 在一个方向上递归检查，直到碰见不同类型的道具
            while (boardComponent.IsInBoard(adjacentGrid))
            {
                // 邻格
                var adjacentSlotComponent = boardComponent.GetSlotComponent(adjacentGrid);
                if (!adjacentSlotComponent.HasItem)
                {
                    break;
                }

                // 如果是同一种类型，递归下去
                if (adjacentSlotComponent.ItemId == slotComponent.ItemId)
                {
                    adjacentGrid += dir;
                    slotComponents.Add(adjacentSlotComponent);
                }
                else
                {
                    break;
                }
            }

            return slotComponents;
        }
    }
}