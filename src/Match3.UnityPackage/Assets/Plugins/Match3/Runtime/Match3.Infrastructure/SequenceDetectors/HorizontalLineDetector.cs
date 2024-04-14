using Match3.App;
using Match3.App.Interfaces;
using Match3.Core.Interfaces;
using Match3.Core.Structs;

namespace Match3.Infrastructure.SequenceDetectors
{
    /// 横向检查器
    public class HorizontalLineDetector<TGridSlot> : LineDetector<TGridSlot> where TGridSlot : IGridSlot
    {
        // 横向需要检查左右两个方向
        private readonly GridPosition[] _lineDirections = { GridPosition.Left, GridPosition.Right };

        public override ItemSequence<TGridSlot> GetSequence(IGameBoard<TGridSlot> gameBoard, GridPosition gridPosition)
        {
            return GetSequenceByDirection(gameBoard, gridPosition, _lineDirections);
        }
    }
}