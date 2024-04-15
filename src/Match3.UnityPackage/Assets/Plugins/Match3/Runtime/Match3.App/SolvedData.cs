using System.Collections.Generic;
using Match3.Core.Interfaces;

namespace Match3.App
{
    public class SolvedData<TGridSlot> where TGridSlot : IGridSlot
    {
        public SolvedData(IReadOnlyCollection<ItemSequence<TGridSlot>> solvedSequences, IReadOnlyCollection<TGridSlot> specialItemGridSlots)
        {
            SolvedSequences = solvedSequences;
            SpecialItemGridSlots = specialItemGridSlots;
        }

        /// 消除的序列集合
        public IReadOnlyCollection<ItemSequence<TGridSlot>> SolvedSequences { get; }
        
        /// 被特殊消除掉的格子集合
        public IReadOnlyCollection<TGridSlot> SpecialItemGridSlots { get; }

        public IEnumerable<TGridSlot> GetSolvedGridSlots(bool onlyMovable = false)
        {
            foreach (var sequence in SolvedSequences)
            {
                foreach (var solvedGridSlot in sequence.SolvedGridSlots)
                {
                    if (onlyMovable && !solvedGridSlot.IsMovable)
                    {
                        continue;
                    }

                    yield return solvedGridSlot;
                }
            }
        }

        public IEnumerable<TGridSlot> GetSpecialItemGridSlots(bool excludeOccupied = false)
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