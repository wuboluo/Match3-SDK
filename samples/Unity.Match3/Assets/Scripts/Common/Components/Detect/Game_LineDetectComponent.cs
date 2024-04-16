using System.Collections.Generic;

namespace Match3
{
    /// 线性检测器
    public class Game_LineDetectComponent : Component
    {
        /// 按方向获取序列
        public ItemSequence GetSequenceByDirection(GameBoard gameBoard, GridPosition gridPosition, IEnumerable<GridPosition> directions)
        {
            // 被消除的格子集合
            var solvedSlots = new List<UnityGridSlot>();
            var gridSlot = gameBoard[gridPosition];

            // direction：上下or左右
            foreach (var direction in directions)
            {
                var slots = GetSequenceOfGridSlots(gameBoard, gridSlot, gridPosition, direction);
                solvedSlots.AddRange(slots);
            }

            // 少于3个不能消
            if (solvedSlots.Count < 2)
            {
                return null;
            }

            // 添加可被消除的格子
            solvedSlots.Add(gridSlot);

            // Horizontal 或 Vertical
            var sequenceDetectorType = GetType();

            // 可被消除的道具序列
            return new ItemSequence(sequenceDetectorType, solvedSlots);
        }

        /// 获取格子序列
        private static IEnumerable<UnityGridSlot> GetSequenceOfGridSlots(GameBoard gameBoard, UnityGridSlot gridSlot, GridPosition gridPosition, GridPosition direction)
        {
            var newPosition = gridPosition + direction;
            var slotsSequence = new List<UnityGridSlot>();

            // 不出界的范围，依次检查
            while (gameBoard.IsPositionOnBoard(newPosition))
            {
                var currentSlot = gameBoard[newPosition];
                if (currentSlot.HasItem == false) break;

                // 如果是同一种类型，记录这个并准备检查后面的
                if (currentSlot.ItemId == gridSlot.ItemId)
                {
                    newPosition += direction;
                    slotsSequence.Add(currentSlot);
                }
                else
                {
                    break;
                }
            }

            return slotsSequence;
        }
    }
}