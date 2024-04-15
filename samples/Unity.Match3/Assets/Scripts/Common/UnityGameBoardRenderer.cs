using System;
using Common.Enums;
using Common.Interfaces;
using Common.Models;
using Match3.App.Interfaces;
using Match3.Core;
using Match3.Core.Structs;
using UnityEngine;

namespace Common
{
    public class UnityGameBoardRenderer : MonoBehaviour, IUnityGameBoardRenderer, IGameBoardDataProvider<IUnityGridSlot>
    {
        [Space] [SerializeField] private int _rowCount = 9; // 行
        [SerializeField] private int _columnCount = 9; // 列

        [Space] [SerializeField] private float _tileSize = 0.6f;

        [Space] [SerializeField] private TileModel[] _tileModels;

        /// 棋盘格子
        private IUnityGridSlot[,] _gridSlots;

        /// 棋子
        private IGridTile[,] _gridTiles;

        /// 原点位置，左上角棋子的位置
        private Vector3 _originPosition;

        /// 棋子对象池
        private TileItemsPool _tilesPool;

        private void Awake()
        {
            _tilesPool = new TileItemsPool(_tileModels, transform);
        }

        /// 获取棋盘格子信息
        public IUnityGridSlot[,] GetGridSlots(int level)
        {
            return _gridSlots;
        }

        /// 创建棋子
        public void CreateGridTiles(int[,] data)
        {
            _gridTiles = new IGridTile[_rowCount, _columnCount];
            _gridSlots = new IUnityGridSlot[_rowCount, _columnCount];
            _originPosition = GetOriginPosition(_rowCount, _columnCount);

            CreateGridTiles(TileType.Available);
        }

        /// 某个位置的棋子是否已启用
        public bool IsTileActive(GridPosition gridPosition)
        {
            return GetTileType(gridPosition) != TileType.Unavailable;
        }

        /// 鼠标位置是否在棋盘内，如果在就返回所在的格子位置
        public bool IsPointerOnBoard(Vector3 worldPointerPosition, out GridPosition gridPosition)
        {
            gridPosition = GetGridPositionByPointer(worldPointerPosition);
            return IsPositionOnBoard(gridPosition);
        }

        public bool IsPointerOnGrid(Vector3 worldPointerPosition, out GridPosition gridPosition)
        {
            gridPosition = GetGridPositionByPointer(worldPointerPosition);
            return IsPositionOnGrid(gridPosition);
        }

        /// 该位置在棋盘内且该位置的棋子已启用
        private bool IsPositionOnBoard(GridPosition gridPosition)
        {
            return IsPositionOnGrid(gridPosition) && IsTileActive(gridPosition);
        }

        public bool IsPositionOnGrid(GridPosition gridPosition)
        {
            return GridMath.IsPositionOnGrid(gridPosition, _rowCount, _columnCount);
        }

        /// 通过鼠标位置获取格子行列位置
        private GridPosition GetGridPositionByPointer(Vector3 worldPointerPosition)
        {
            var rowIndex = Convert.ToInt32((worldPointerPosition - _originPosition).y / _tileSize);
            var columnIndex = Convert.ToInt32((worldPointerPosition - _originPosition).x / _tileSize);

            return new GridPosition(rowIndex, columnIndex);
        }

        /// 把一个位置的棋子：启用
        public void ActivateTile(GridPosition gridPosition)
        {
            SetTile(gridPosition.RowIndex, gridPosition.ColumnIndex, TileType.Available);
        }

        /// 把一个位置的棋子：停用
        public void DeactivateTile(GridPosition gridPosition)
        {
            SetTile(gridPosition.RowIndex, gridPosition.ColumnIndex, TileType.Unavailable);
        }

        public void SetNextGridTileType(GridPosition gridPosition)
        {
            var tileType = GetTileType(gridPosition);
            SetTile(gridPosition.RowIndex, gridPosition.ColumnIndex, GetNextAvailableType(tileType));
        }

        private static TileType GetNextAvailableType(TileType type)
        {
            var index = (int)type + 1;
            var resultGroup = TileType.Available;
            var groupValues = (TileType[])Enum.GetValues(typeof(TileType));

            if (index < groupValues.Length)
            {
                resultGroup = groupValues[index];
            }

            return resultGroup;
        }

        /// 获取某个位置的棋子类型
        public TileType GetTileType(GridPosition gridPosition)
        {
            return (TileType)_gridTiles[gridPosition.RowIndex, gridPosition.ColumnIndex].TypeId;
        }

        /// 重置棋盘
        public void ResetGridTiles()
        {
            SetAllGridTiles(TileType.Available);
        }

        public void Dispose()
        {
            DisposeGridTiles();
            DisposeGameBoardData();
        }

        // 根据行列确定坐标
        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return GetWorldPosition(gridPosition.RowIndex, gridPosition.ColumnIndex);
        }

        private Vector3 GetWorldPosition(int rowIndex, int columnIndex)
        {
            return new Vector3(columnIndex, rowIndex) * _tileSize + _originPosition;
        }

        // 开始排列的原点位置，从左下角开始，一行一行以此排列
        // 这样保证棋盘中心在 (0,0)点
        private Vector3 GetOriginPosition(int rowCount, int columnCount)
        {
            var offsetY = Mathf.Floor(rowCount / 2.0f) * _tileSize;
            var offsetX = Mathf.Floor(columnCount / 2.0f) * _tileSize;

            return new Vector3(-offsetX, -offsetY);
        }

        // 开局的时候创建所有的棋子
        private void CreateGridTiles(TileType defaultTileType)
        {
            for (var rowIndex = 0; rowIndex < _rowCount; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < _columnCount; columnIndex++)
                {
                    var gridTile = GetTileFromPool(rowIndex, columnIndex, defaultTileType);

                    _gridTiles[rowIndex, columnIndex] = gridTile;
                    _gridSlots[rowIndex, columnIndex] = new UnityGridSlot(gridTile, new GridPosition(rowIndex, columnIndex));
                }
            }
        }

        // 把棋盘上所有的棋子设置为指定类型
        private void SetAllGridTiles(TileType type)
        {
            for (var rowIndex = 0; rowIndex < _rowCount; rowIndex++)
            {
                for (var columnIndex = 0; columnIndex < _columnCount; columnIndex++)
                {
                    SetTile(rowIndex, columnIndex, type);
                }
            }
        }

        // 设置某行列的棋子
        private void SetTile(int rowIndex, int columnIndex, TileType type)
        {
            // 把这个位置原来的棋子回收，如果有的话
            var currentTile = _gridTiles[rowIndex, columnIndex];
            if (currentTile != null)
            {
                _tilesPool.RecycleTile(currentTile);
            }

            // 再取一个新的放在这个位置
            var gridTile = GetTileFromPool(rowIndex, columnIndex, type);
            _gridTiles[rowIndex, columnIndex] = gridTile;
            _gridSlots[rowIndex, columnIndex].SetState(gridTile);
        }

        // 从池中拿一个tile gameObject
        private IGridTile GetTileFromPool(int rowIndex, int columnIndex, TileType type)
        {
            var gridTile = _tilesPool.FetchTile(type);
            gridTile.SetWorldPosition(GetWorldPosition(rowIndex, columnIndex));

            return gridTile;
        }

        private void DisposeGridTiles()
        {
            if (_gridTiles == null)
            {
                return;
            }

            foreach (var gridSlotTile in _gridTiles)
            {
                gridSlotTile.Dispose();
            }

            Array.Clear(_gridTiles, 0, _gridTiles.Length);
            _gridTiles = null;
        }

        private void DisposeGameBoardData()
        {
            if (_gridSlots == null)
            {
                return;
            }

            Array.Clear(_gridSlots, 0, _gridSlots.Length);
            _gridSlots = null;
        }
    }
}