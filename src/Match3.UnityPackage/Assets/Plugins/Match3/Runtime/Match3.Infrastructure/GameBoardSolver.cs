using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Match3.App;
using Match3.App.Interfaces;
using Match3.Core.Interfaces;
using Match3.Core.Structs;
using UnityEngine;

namespace Match3.Infrastructure
{
    /// 游戏消除器
    public class GameBoardSolver<TGridSlot> : IGameBoardSolver<TGridSlot> where TGridSlot : IGridSlot
    {
        // 方向序列检测器
        private readonly ISequenceDetector<TGridSlot>[] _dirSequenceDetectors;

        // 特殊道具检测器
        private readonly ISpecialItemDetector<TGridSlot>[] _specialItemDetectors;

        public GameBoardSolver(ISequenceDetector<TGridSlot>[] dirSequenceDetectors, ISpecialItemDetector<TGridSlot>[] specialItemDetectors)
        {
            _dirSequenceDetectors = dirSequenceDetectors;
            _specialItemDetectors = specialItemDetectors;
        }

        /// 消除
        public SolvedData<TGridSlot> Solve(IGameBoard<TGridSlot> gameBoard, params GridPosition[] gridPositions)
        {
            Debug.Log("尝试消除");

            var resultSequences = new Collection<ItemSequence<TGridSlot>>();
            var specialItemGridSlots = new HashSet<TGridSlot>();

            foreach (var gridPosition in gridPositions)
            {
                // 横向 纵向
                foreach (var dirSeqDetector in _dirSequenceDetectors)
                {
                    var sequence = dirSeqDetector.GetSequence(gameBoard, gridPosition);
                    if (sequence == null)
                    {
                        // 不足3个时返回null
                        continue;
                    }

                    if (!IsNewSequence(sequence, resultSequences))
                    {
                        continue;
                    }

                    foreach (var specialItemGridSlot in GetSpecialItemGridSlots(gameBoard, sequence))
                    {
                        specialItemGridSlots.Add(specialItemGridSlot);
                    }

                    resultSequences.Add(sequence);
                }
            }

            return new SolvedData<TGridSlot>(resultSequences, specialItemGridSlots);
        }

        private static bool IsNewSequence(ItemSequence<TGridSlot> newSequence, IEnumerable<ItemSequence<TGridSlot>> sequences)
        {
            var sequencesByType = sequences.Where(sequence => sequence.SequenceDetectorType == newSequence.SequenceDetectorType);
            var newSequenceGridSlot = newSequence.SolvedGridSlots[0];

            return sequencesByType.All(sequence => !sequence.SolvedGridSlots.Contains(newSequenceGridSlot));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<TGridSlot> GetSpecialItemGridSlots(IGameBoard<TGridSlot> gameBoard, ItemSequence<TGridSlot> sequence)
        {
            foreach (var itemDetector in _specialItemDetectors)
            {
                foreach (var solvedGridSlot in sequence.SolvedGridSlots)
                {
                    foreach (var specialItemGridSlot in itemDetector.GetSpecialItemGridSlots(gameBoard, solvedGridSlot))
                    {
                        var hasNextState = ((IStatefulSlot)specialItemGridSlot.State).NextState();
                        if (hasNextState)
                        {
                            continue;
                        }

                        yield return specialItemGridSlot;
                    }
                }
            }
        }
    }
}