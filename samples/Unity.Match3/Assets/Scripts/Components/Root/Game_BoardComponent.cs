using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Match3
{
    public class Game_BoardComponent : Component
    {
        private readonly float _tileSize;
        private readonly Vector3 _originPosition;
        private Game_SlotComponent[,] _gridSlots;

        public Game_BoardComponent(int rowCount, int columnCount, float tileSize)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;

            _tileSize = tileSize;
            _originPosition = GetOriginPosition(rowCount, columnCount);
            CreateSlots();
        }

        public int RowCount { get; }
        public int ColumnCount { get; }

        public Game_SlotComponent GetSlotComponent(Grid pos)
        {
            return GetSlotComponent(pos.RowIndex, pos.ColumnIndex);
        }

        public Game_SlotComponent GetSlotComponent(int row, int column)
        {
            return GetSlotComponents()[row, column];
        }

        private Game_SlotComponent[,] GetSlotComponents()
        {
            return _gridSlots;
        }

        private void CreateSlots()
        {
            _gridSlots = new Game_SlotComponent[RowCount, ColumnCount];

            for (var row = 0; row < RowCount; row++)
            for (var column = 0; column < ColumnCount; column++)
            {
                _gridSlots[row, column] = new Game_SlotComponent(new Game_GridComponent(row, column));
            }
        }

        /// 鼠标是否在棋盘内，如果在就返回所在的格子位置
        public bool IsPointerOnBoard(Vector3 worldPointerPosition, out Grid grid)
        {
            grid = GetGridByPointer(worldPointerPosition);
            return IsInBoard(grid);
        }

        /// 鼠标位置所在格子
        private Grid GetGridByPointer(Vector3 worldPointerPosition)
        {
            var row = Convert.ToInt32((worldPointerPosition - _originPosition).y / _tileSize);
            var column = Convert.ToInt32((worldPointerPosition - _originPosition).x / _tileSize);

            return new Grid(row, column);
        }

        /// 格子的世界坐标
        public Vector3 GetWorldPosition(Grid grid)
        {
            return GetWorldPosition(grid.RowIndex, grid.ColumnIndex);
        }

        /// 某行列的世界坐标
        public Vector3 GetWorldPosition(int row, int column)
        {
            return new Vector3(column, row) * _tileSize + _originPosition;
        }

        /// 开始排列的原点位置，从左下角开始，一行一行以此排列，这样保证棋盘中心在 (0,0)点
        private Vector3 GetOriginPosition(int rowCount, int columnCount)
        {
            var offsetY = Mathf.Floor(rowCount / 2.0f) * _tileSize;
            var offsetX = Mathf.Floor(columnCount / 2.0f) * _tileSize;

            return new Vector3(-offsetX, -offsetY);
        }

        /// 此格子在棋盘内，不超过四周的边界
        public bool IsInBoard(Grid grid)
        {
            return grid.RowIndex >= 0 &&
                   grid.RowIndex < RowCount &&
                   grid.ColumnIndex >= 0 &&
                   grid.ColumnIndex < ColumnCount;
        }

        /// 格子是否可移动（只有包含道具才能被交换）
        public bool IsMovableSlot(Grid grid)
        {
            return GetSlotComponent(grid).HasItem;
        }

        /// 同一个格子
        public bool IsSameSlot(Grid pos1, Grid pos2)
        {
            return pos1.Equals(pos2);
        }

        /// 不在一条直线上（是斜对角）
        public bool IsAdjacentSlot(Grid pos1, Grid pos2)
        {
            // 当前的格子和鼠标落下时的格子在上下左右任意方向是挨着的
            return pos1.Equals(pos2 + Grid.Up) ||
                   pos1.Equals(pos2 + Grid.Down) ||
                   pos1.Equals(pos2 + Grid.Left) ||
                   pos1.Equals(pos2 + Grid.Right);
        }
    }
}