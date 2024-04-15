using System.Text;
using Common.Interfaces;
using Match3.App;
using Match3.App.Interfaces;
using UnityEngine;

namespace Common
{
    /// 游戏计分板
    public class GameScoreBoard : ISolvedSequencesConsumer<IUnityGridSlot>
    {
        /// 当序列被消除时
        public void OnSequencesSolved(SolvedData<IUnityGridSlot> solvedData)
        {
            foreach (var sequence in solvedData.SolvedSequences)
            {
                RegisterSequenceScore(sequence);
            }
        }

        private static void RegisterSequenceScore(ItemSequence<IUnityGridSlot> sequence)
        {
            // todo：combo 计分 出兵 
            Debug.Log(GetSequenceDescription(sequence));
        }

        private static string GetSequenceDescription(ItemSequence<IUnityGridSlot> sequence)
        {
            var stringBuilder = new StringBuilder();

            var detectorType = sequence.SequenceDetectorType.Name.Contains("Ver") ? "纵向" : "横向";
            stringBuilder.Append($"棋子种类：<color=yellow>{sequence.SolvedGridSlots[0].Item.ContentId}</color>");
            stringBuilder.Append("  |  ");
            stringBuilder.Append($"方向：<color=yellow>{detectorType}</color>");
            stringBuilder.Append("  |  ");
            stringBuilder.Append($"数量：<color=yellow>{sequence.SolvedGridSlots.Count}</color>");

            return stringBuilder.ToString();
        }
    }
}