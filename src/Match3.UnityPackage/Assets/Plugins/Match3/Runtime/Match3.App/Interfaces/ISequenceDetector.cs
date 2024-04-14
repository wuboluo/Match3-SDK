using Match3.Core.Interfaces;
using Match3.Core.Structs;

namespace Match3.App.Interfaces
{
    /// 序列检测器
    public interface ISequenceDetector<TGridSlot> where TGridSlot : IGridSlot
    {
        /// 获取序列
        ItemSequence<TGridSlot> GetSequence(IGameBoard<TGridSlot> gameBoard, GridPosition gridPosition);
    }
}