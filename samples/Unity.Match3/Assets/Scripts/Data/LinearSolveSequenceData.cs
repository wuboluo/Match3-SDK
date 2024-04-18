using System;
using System.Collections.Generic;

namespace Match3
{
    public class LinearSolveSequenceData
    {
        public LinearSolveSequenceData(Type sequenceDetectorType, List<Game_SlotComponent> solvedSlotComponents)
        {
            SequenceDetectorType = sequenceDetectorType;
            SolvedSlotComponents = solvedSlotComponents;
        }

        // 序列检测器类型
        public Type SequenceDetectorType { get; }

        // 消除的格子集合
        public List<Game_SlotComponent> SolvedSlotComponents { get; }
    }
}