using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Match3
{
    /// 游戏消除器
    public class GameBoardSolver
    {
        // 方向序列检测器
        private readonly ILineDetect[] _dirSequenceDetectors;

        // 特殊道具检测器
        private readonly ISpecialItemDetector[] _specialItemDetectors;

        public GameBoardSolver(ILineDetect[] dirSequenceDetectors, ISpecialItemDetector[] specialItemDetectors)
        {
            _dirSequenceDetectors = dirSequenceDetectors;
            _specialItemDetectors = specialItemDetectors;
        }

        /// 消除
        public SolvedData Solve(GameBoard gameBoard, params GridPosition[] gridPositions)
        {
            Debug.Log("交换");

            var resultSequences = new Collection<ItemSequence>();
            var specialItemGridSlots = new HashSet<UnityGridSlot>();

            foreach (var gridPosition in gridPositions)
                // 横向 纵向
            foreach (var dirSeqDetector in _dirSequenceDetectors)
            {
                var sequence = dirSeqDetector.GetSequence(gameBoard, gridPosition);
                if (sequence == null)
                    // 不足3个时返回null
                    continue;

                if (!IsNewSequence(sequence, resultSequences)) continue;

                // todo 
                if (_specialItemDetectors != null)
                    foreach (var specialItemGridSlot in GetSpecialItemGridSlots(gameBoard, sequence))
                        specialItemGridSlots.Add(specialItemGridSlot);

                resultSequences.Add(sequence);
            }

            return new SolvedData(resultSequences, specialItemGridSlots);
        }

        private static bool IsNewSequence(ItemSequence newSequence, IEnumerable<ItemSequence> sequences)
        {
            var sequencesByType = sequences.Where(sequence => sequence.SequenceDetectorType == newSequence.SequenceDetectorType);
            var newSequenceGridSlot = newSequence.SolvedGridSlots[0];

            return sequencesByType.All(sequence => !sequence.SolvedGridSlots.Contains(newSequenceGridSlot));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<UnityGridSlot> GetSpecialItemGridSlots(GameBoard gameBoard, ItemSequence sequence)
        {
            foreach (var itemDetector in _specialItemDetectors)
            foreach (var solvedGridSlot in sequence.SolvedGridSlots)
            foreach (var specialItemGridSlot in itemDetector.GetSpecialItemGridSlots(gameBoard, solvedGridSlot))
            {
                var hasNextState = ((IStatefulSlot)specialItemGridSlot.State).NextState();
                if (hasNextState) continue;

                yield return specialItemGridSlot;
            }
        }
    }
}