// using UnityEngine;
//
// namespace Match3
// {
//     /// 任务：单次消除一整行
//     public class CollectRowMaxItems : LevelGoal
//     {
//         private readonly int _maxRowLength;
//
//         public CollectRowMaxItems()
//         {
//             _maxRowLength = GetMaxRowLength(gameBoard);
//         }
//
//         /// 如果一次消除了一整行，就NB了
//         public override void OnSequencesSolved(SolvedData solvedData)
//         {
//             // 所有的消除序列
//             foreach (var sequence in solvedData.SolvedSequences)
//             {
//                 // 纵向的不算，因为这个目标就是单次消除一整行
//                 if (sequence.SequenceDetectorType != typeof(Game_HorizontalLineDetectComponent)) continue;
//
//                 // 如果单次消除了一行最大格子数量
//                 if (sequence.SolvedGridSlots.Count == _maxRowLength)
//                     // 标记此任务已完成
//                     MarkAchieved();
//             }
//         }
//
//         /// 单行最多有几个可用的格子，如果中间有障碍的话，可能小于棋盘宽度
//         private static int GetMaxRowLength(GameBoard gameBoard)
//         {
//             var maxRowLength = 0;
//
//             for (var rowIndex = 0; rowIndex < gameBoard.RowCount; rowIndex++)
//             {
//                 var maxRowSlots = 0;
//                 var availableSlots = 0;
//
//                 for (var columnIndex = 0; columnIndex < gameBoard.ColumnCount; columnIndex++)
//                 {
//                     if (gameBoard[rowIndex, columnIndex].State.CanContainItem)
//                     {
//                         availableSlots++;
//                         continue;
//                     }
//
//                     if (availableSlots > maxRowSlots) maxRowSlots = availableSlots;
//
//                     availableSlots = 0;
//                 }
//
//                 var maxLength = Mathf.Max(maxRowSlots, availableSlots);
//                 if (maxLength > maxRowLength) maxRowLength = maxLength;
//             }
//
//             return maxRowLength;
//         }
//     }
// }