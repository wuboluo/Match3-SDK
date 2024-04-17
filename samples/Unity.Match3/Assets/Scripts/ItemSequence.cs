using System;
using System.Collections.Generic;

namespace Match3
{
    public class ItemSequence
    {
        public ItemSequence(Type sequenceDetectorType, IReadOnlyList<Game_SlotComponent> solvedGridSlots)
        {
            SequenceDetectorType = sequenceDetectorType;
            SolvedGridSlots = solvedGridSlots;
        }

        // 序列检测器类型
        public Type SequenceDetectorType { get; }

        // 消除的格子集合
        public IReadOnlyList<Game_SlotComponent> SolvedGridSlots { get; }
    }
}