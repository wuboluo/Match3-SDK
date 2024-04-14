using System;
using System.Collections.Generic;
using Match3.Core.Interfaces;

namespace Match3.App
{
    public class ItemSequence<TGridSlot> where TGridSlot : IGridSlot
    {
        public ItemSequence(Type sequenceDetectorType, IReadOnlyList<TGridSlot> solvedGridSlots)
        {
            SequenceDetectorType = sequenceDetectorType;
            SolvedGridSlots = solvedGridSlots;
        }

        // 序列检测器类型
        public Type SequenceDetectorType { get; }

        // 消除的格子集合
        public IReadOnlyList<TGridSlot> SolvedGridSlots { get; }
    }
}