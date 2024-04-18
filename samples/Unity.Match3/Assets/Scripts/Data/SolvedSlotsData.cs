using System.Collections.Generic;

namespace Match3
{
    public class SolvedSlotsData
    {
        public SolvedSlotsData(List<LinearSolveSequenceData> solvedSequences, HashSet<Game_SlotComponent> specialSolvedSlotComponents)
        {
            SolvedSequences = solvedSequences;
            SpecialSolvedSlotComponents = specialSolvedSlotComponents;
        }

        /// 被消除的序列
        public List<LinearSolveSequenceData> SolvedSequences { get; }

        /// 被特殊消除掉的格子
        public HashSet<Game_SlotComponent> SpecialSolvedSlotComponents { get; }

        /// 获取所有序列中被消除的格子
        public IEnumerable<Game_SlotComponent> GetSolvedSlotComponentsInSequences()
        {
            foreach (var sequence in SolvedSequences)
            foreach (var slotComponent in sequence.SolvedSlotComponents)
            {
                if (slotComponent.HasItem)
                {
                    yield return slotComponent;
                }
            }
        }

        /// 获取所有被特殊消除的格子
        public IEnumerable<Game_SlotComponent> GetSpecialSolvedSlotComponents()
        {
            foreach (var slotComponent in SpecialSolvedSlotComponents)
            {
                if (!slotComponent.HasItem)
                {
                    yield return slotComponent;
                }
            }
        }
    }
}