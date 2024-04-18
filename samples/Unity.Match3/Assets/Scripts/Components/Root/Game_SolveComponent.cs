using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Match3
{
    //// 游戏消除组件
    public class Game_SolveComponent : Component
    {
        // 方向序列检测器
        private readonly ILinearDetect[] _dirSequenceDetectors;

        // 特殊道具检测器
        private readonly ISpecialDetect[] _specialItemDetectors;

        public Game_SolveComponent(ILinearDetect[] dirSequenceDetectors, ISpecialDetect[] specialItemDetectors)
        {
            _dirSequenceDetectors = dirSequenceDetectors;
            _specialItemDetectors = specialItemDetectors;
        }

        /// 相邻交换
        public SolvedSlotsData Swap(params Grid[] grids)
        {
            // 被消除的整列
            var solvedSequences = new List<LinearSolveSequenceData>();

            // 被特殊消除的格子
            var specialSolvedSlotComponents = new HashSet<Game_SlotComponent>();

            // 这里只是被交换的两个格子
            foreach (var grid in grids)
            {
                // 横向 纵向
                foreach (var dirSeqDetector in _dirSequenceDetectors)
                {
                    if (dirSeqDetector.GetSequence(grid, out var sequence))
                    {
                        // 这个序列如果没有被记录
                        if (!IsNewSequence(sequence, solvedSequences))
                        {
                            continue;
                        }

                        // todo 特殊棋子处理
                        if (_specialItemDetectors != null)
                        {
                            foreach (var specialItemGridSlot in GetSpecialItemGridSlots(sequence))
                            {
                                specialSolvedSlotComponents.Add(specialItemGridSlot);
                            }
                        }

                        solvedSequences.Add(sequence);
                    }
                }
            }

            return new SolvedSlotsData(solvedSequences, specialSolvedSlotComponents);
        }

        private static bool IsNewSequence(LinearSolveSequenceData newSeq, List<LinearSolveSequenceData> sequences)
        {
            var firstGrid = newSeq.SolvedSlotComponents[0];
            return !sequences.Any(seq =>
                seq.SequenceDetectorType == newSeq.SequenceDetectorType &&
                seq.SolvedSlotComponents.Contains(firstGrid));
        }

        /// 是否成功消除
        public bool IsSolved(Grid grid1, Grid grid2, out SolvedSlotsData solvedSlotsData)
        {
            solvedSlotsData = Swap(grid1, grid2);
            // 至少要有一个可被消除的序列
            return Swap(grid1, grid2).SolvedSequences.Count > 0;
        }

        /// 序列消除时
        public void NotifySequencesSolved(SolvedSlotsData solvedSlotsData)
        {
            // 每个序列计分
            foreach (var sequence in solvedSlotsData.SolvedSequences)
            {
                World.Instance.SolveSequenceDescription(sequence);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private IEnumerable<Game_SlotComponent> GetSpecialItemGridSlots(LinearSolveSequenceData sequenceData)
        {
            foreach (var itemDetector in _specialItemDetectors)
            foreach (var solvedSlotComponents in sequenceData.SolvedSlotComponents)
            foreach (var slotComponent in itemDetector.GetSpecialItemGridSlots(solvedSlotComponents))
            {
                yield return slotComponent;
            }
        }
    }
}