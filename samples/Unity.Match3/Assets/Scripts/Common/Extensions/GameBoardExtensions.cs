// namespace Match3
// {
//     public static class GameBoardExtensions
//     {
//         /// 能否向上移动
//         public static bool CanMoveUp(this GameBoard gameBoard, UnityGridSlot gridSlot, out GridPosition gridPosition)
//         {
//             // 如果该位置上方一个位置为空（被消除了），那么就可以上移
//             var topGridSlot = gameBoard.GetSideGridSlot(gridSlot, GridPosition.Up);
//             if (topGridSlot is { CanSetItem: true })
//             {
//                 gridPosition = topGridSlot.GridPosition;
//                 return true;
//             }
//
//             gridPosition = GridPosition.Zero;
//             return false;
//         }
//
//         /// 获取某格子相邻的某方向的临格
//         public static UnityGridSlot GetSideGridSlot(this GameBoard gameBoard, UnityGridSlot gridSlot, GridPosition direction)
//         {
//             var sideGridSlotPosition = gridSlot.GridPosition + direction;
//
//             return gameBoard.IsPositionOnGrid(sideGridSlotPosition)
//                 ? gameBoard[sideGridSlotPosition]
//                 : null;
//         }
//     }
// }