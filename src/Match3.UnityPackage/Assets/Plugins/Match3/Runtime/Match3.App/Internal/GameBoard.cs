using System;
using System.Runtime.CompilerServices;
using Match3.App.Interfaces;
using Match3.Core;
using Match3.Core.Interfaces;
using Match3.Core.Structs;

namespace Match3.App.Internal
{
    internal class GameBoard<TGridSlot> : IGameBoard<TGridSlot>, IDisposable where TGridSlot : IGridSlot
    {
        private TGridSlot[,] _gridSlots;

        public int RowCount { get; private set; }
        public int ColumnCount { get; private set; }

        public TGridSlot this[GridPosition gridPosition] => _gridSlots[gridPosition.RowIndex, gridPosition.ColumnIndex];
        public TGridSlot this[int rowIndex, int columnIndex] => _gridSlots[rowIndex, columnIndex];

        public void SetGridSlots(TGridSlot[,] gridSlots)
        {
            if (_gridSlots != null)
            {
                throw new InvalidOperationException("Grid slots have already been created.");
            }

            RowCount = gridSlots.GetLength(0);
            ColumnCount = gridSlots.GetLength(1);

            _gridSlots = gridSlots;
        }

        public bool IsPositionOnGrid(GridPosition gridPosition)
        {
            // 确保棋盘不为空
            EnsureGridSlotsIsNotNull();

            // 在棋盘范围内
            return GridMath.IsPositionOnGrid(this, gridPosition);
        }

        public bool IsPositionOnBoard(GridPosition gridPosition)
        {
            return IsPositionOnGrid(gridPosition) && _gridSlots[gridPosition.RowIndex, gridPosition.ColumnIndex].CanContainItem;
        }

        public void ResetState()
        {
            RowCount = 0;
            ColumnCount = 0;
            _gridSlots = null;
        }

        public void Dispose()
        {
            if (_gridSlots == null)
            {
                return;
            }

            Array.Clear(_gridSlots, 0, _gridSlots.Length);
            ResetState();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EnsureGridSlotsIsNotNull()
        {
            if (_gridSlots == null)
            {
                throw new InvalidOperationException("Grid slots are not created.");
            }
        }
    }
}