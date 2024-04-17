using System.Collections.Generic;

namespace Match3
{
    public class SolvedData
    {
        public SolvedData(IReadOnlyCollection<ItemSequence> solvedSequences, IReadOnlyCollection<Game_SlotComponent> specialItemGridSlots)
        {
            SolvedSequences = solvedSequences;
            SpecialItemGridSlots = specialItemGridSlots;
        }

        /// 消除的序列集合
        public IReadOnlyCollection<ItemSequence> SolvedSequences { get; }

        /// 被特殊消除掉的格子集合
        public IReadOnlyCollection<Game_SlotComponent> SpecialItemGridSlots { get; }

        public IEnumerable<Game_SlotComponent> GetSolvedGridSlots(bool onlyMovable = false)
        {
            foreach (var sequence in SolvedSequences)
            foreach (var solvedGridSlot in sequence.SolvedGridSlots)
            {
                if (onlyMovable && !solvedGridSlot.HasItem)
                {
                    continue;
                }

                yield return solvedGridSlot;
            }
        }

        public IEnumerable<Game_SlotComponent> GetSpecialItemGridSlots(bool excludeOccupied = false)
        {
            foreach (var specialItemGridSlot in SpecialItemGridSlots)
            {
                if (excludeOccupied && specialItemGridSlot.HasItem)
                {
                    continue;
                }

                yield return specialItemGridSlot;
            }
        }
    }
}