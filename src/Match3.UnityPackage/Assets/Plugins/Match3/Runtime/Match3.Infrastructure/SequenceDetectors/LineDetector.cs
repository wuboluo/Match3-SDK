using System.Collections.Generic;
using Match3.App;
using Match3.App.Interfaces;
using Match3.Core.Interfaces;
using Match3.Core.Structs;

namespace Match3.Infrastructure.SequenceDetectors
{
    /// 线性检测器
    public abstract class LineDetector<TGridSlot> : ISequenceDetector<TGridSlot> where TGridSlot : IGridSlot
    {
        /// 获取序列（横向 纵向）
        public abstract ItemSequence<TGridSlot> GetSequence(IGameBoard<TGridSlot> gameBoard, GridPosition gridPosition);

        /// 按方向获取序列
        protected ItemSequence<TGridSlot> GetSequenceByDirection(IGameBoard<TGridSlot> gameBoard, GridPosition gridPosition, IEnumerable<GridPosition> directions)
        {
            // 被消除的格子集合
            var solvedSlots = new List<TGridSlot>();
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
            return new ItemSequence<TGridSlot>(sequenceDetectorType, solvedSlots);
        }

        /// 获取格子序列
        private static IEnumerable<TGridSlot> GetSequenceOfGridSlots(IGameBoard<TGridSlot> gameBoard, TGridSlot gridSlot, GridPosition gridPosition, GridPosition direction)
        {
            var newPosition = gridPosition + direction;
            var slotsSequence = new List<TGridSlot>();

            // 不出界的范围，依次检查
            while (gameBoard.IsPositionOnBoard(newPosition))
            {
                var currentSlot = gameBoard[newPosition];
                if (currentSlot.HasItem == false)
                {
                    break;
                }

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