using Match3.Core.Interfaces;

namespace Match3.App.Interfaces
{
    /// 消除序列
    public interface ISolvedSequencesConsumer<TGridSlot> where TGridSlot : IGridSlot
    {
        /// 当一个序列被消除时
        void OnSequencesSolved(SolvedData<TGridSlot> solvedData);
    }
}