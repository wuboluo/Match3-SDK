using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Match3
{
    public class Game_BoardComponent : Component
    {
        private readonly float _tileSize;
        private readonly Game_ItemComponent[,] _gridTiles;
        private readonly Game_SlotComponent[,] _gridSlots;
        private readonly Vector3 _originPosition;

        private AsyncLazy _swapItemsTask;

        public Game_BoardComponent(int rowCount, int columnCount, float tileSize)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;

            _tileSize = tileSize;
            _gridTiles = new Game_ItemComponent[rowCount, columnCount];
            _gridSlots = new Game_SlotComponent[rowCount, columnCount];
            _originPosition = GetOriginPosition(rowCount, columnCount);
        }

        public int RowCount { get; }
        public int ColumnCount { get; }

        public Game_SlotComponent GetGridSlot(GridPosition pos)
        {
            return GetGridSlot(pos.RowIndex, pos.ColumnIndex);
        }

        public Game_SlotComponent GetGridSlot(int row, int column)
        {
            return GetGridSlots()[row, column];
        }

        public Game_SlotComponent[,] GetGridSlots()
        {
            return _gridSlots;
        }

        /// 某个位置的棋子是否已启用
        private bool IsTileActive(GridPosition gridPosition)
        {
            return true;
            // return GetTileType(gridPosition) != TileType.Unavailable;
        }

        /// 鼠标是否在棋盘内，如果在就返回所在的格子位置
        public bool IsPointerOnBoard(Vector3 worldPointerPosition, out GridPosition gridPosition)
        {
            gridPosition = GetGridPositionByPointer(worldPointerPosition);
            return IsPositionOnBoard(gridPosition);
        }

        /// 鼠标是否在格子内 and 此格子可操作
        public bool IsPositionOnBoard(GridPosition gridPosition)
        {
            return IsPositionOnGrid(gridPosition) && IsTileActive(gridPosition);
        }

        /// 鼠标是否在格子内
        private bool IsPositionOnGrid(GridPosition gridPosition)
        {
            return gridPosition.RowIndex >= 0 &&
                   gridPosition.RowIndex < RowCount &&
                   gridPosition.ColumnIndex >= 0 &&
                   gridPosition.ColumnIndex < ColumnCount;
        }

        /// 通过鼠标获取格子行列位置
        private GridPosition GetGridPositionByPointer(Vector3 worldPointerPosition)
        {
            var rowIndex = Convert.ToInt32((worldPointerPosition - _originPosition).y / _tileSize);
            var columnIndex = Convert.ToInt32((worldPointerPosition - _originPosition).x / _tileSize);

            return new GridPosition(rowIndex, columnIndex);
        }

        /// 格子是否可移动
        public bool IsMovableSlot(GridPosition gridPosition)
        {
            return GetGridSlot(gridPosition).HasItem;
        }

        // /// 获取某个位置的棋子类型
        // private TileType GetTileType(GridPosition gridPosition)
        // {
        //     return (TileType)_gridTiles[gridPosition.RowIndex, gridPosition.ColumnIndex].TypeId;
        // }

        /// 根据行列确定坐标
        public Vector3 GetWorldPosition(GridPosition gridPosition)
        {
            return GetWorldPosition(gridPosition.RowIndex, gridPosition.ColumnIndex);
        }

        /// 棋子行列确定世界坐标
        public Vector3 GetWorldPosition(int rowIndex, int columnIndex)
        {
            return new Vector3(columnIndex, rowIndex) * _tileSize + _originPosition;
        }

        /// 开始排列的原点位置，从左下角开始，一行一行以此排列，这样保证棋盘中心在 (0,0)点
        private Vector3 GetOriginPosition(int rowCount, int columnCount)
        {
            var offsetY = Mathf.Floor(rowCount / 2.0f) * _tileSize;
            var offsetX = Mathf.Floor(columnCount / 2.0f) * _tileSize;

            return new Vector3(-offsetX, -offsetY);
        }

        /// 开局的时候创建所有的棋子
        public void CreateGridTiles()
        {
            var _itemPoolComponent = World.Instance.Root.GetComponent<Game_ItemPoolComponent>();

            for (var rowIndex = 0; rowIndex < RowCount; rowIndex++)
            for (var columnIndex = 0; columnIndex < ColumnCount; columnIndex++)
            {
                var gridTile = _itemPoolComponent.FetchTile(rowIndex, columnIndex);

                _gridTiles[rowIndex, columnIndex] = gridTile;
                _gridSlots[rowIndex, columnIndex] = new Game_SlotComponent(gridTile, new GridPosition(rowIndex, columnIndex));
            }
        }

        // /// 设置某行列的棋子
        // private void SetTile(int rowIndex, int columnIndex, TileType type)
        // {
        //     // 把这个位置原来的棋子回收，如果有的话
        //     var currentTile = _gridTiles[rowIndex, columnIndex];
        //     if (currentTile != null)
        //     {
        //         var itemPoolComponent = World.Instance.Root.GetComponent<Game_ItemPoolComponent>();
        //         itemPoolComponent.RecycleTile(currentTile);
        //     }
        //
        //     // 再取一个新的放在这个位置
        //     var gridTile = GetTileFromPool(rowIndex, columnIndex, type);
        //     _gridTiles[rowIndex, columnIndex] = gridTile;
        //     _gridSlots[rowIndex, columnIndex].SetState(gridTile);
        // }

        /// 同一个格子
        public bool IsSameSlot(GridPosition pos1, GridPosition pos2)
        {
            return pos1.Equals(pos2);
        }

        /// 不在一条直线上（是斜对角）
        public bool IsDiagonalSlot(GridPosition pos1, GridPosition pos2)
        {
            // 当前的格子和鼠标落下时的格子在上下左右任意方向是挨着的
            var isSideSlot = pos1.Equals(pos2 + GridPosition.Up) ||
                             pos1.Equals(pos2 + GridPosition.Down) ||
                             pos1.Equals(pos2 + GridPosition.Left) ||
                             pos1.Equals(pos2 + GridPosition.Right);

            return !isSideSlot;
        }

        /// 交换
        public UniTask SwapItems(GridPosition pos1, GridPosition pos2, CancellationToken cancellationToken = default)
        {
            if (_swapItemsTask?.Task.Status.IsCompleted() ?? true)
            {
                _swapItemsTask = SwapItemsAsync(pos1, pos2, cancellationToken).ToAsyncLazy();
            }

            return _swapItemsTask.Task;
        }

        /// 交换
        private async UniTask SwapItemsAsync(GridPosition pos1, GridPosition pos2, CancellationToken cancellationToken = default)
        {
            // 交换动画
            await SwapGameBoardItemsAsync(pos1, pos2, cancellationToken);

            // 成功消除
            if (IsSolved(pos1, pos2, out var solvedData))
            {
                // 序列消除时，计分等
                NotifySequencesSolved(solvedData);

                // 列消除Jobs
                var fillStrategyComponent = World.Instance.Root.GetComponent<Game_RiseFillStrategyComponent>();
                var solveJobs = fillStrategyComponent.GetSolveJobs(solvedData);
                await World.Instance.Root.GetComponent<Game_JobComponent>().Execute(solveJobs, cancellationToken);
            }
            else
            {
                // 交换失败，再换回来
                await SwapGameBoardItemsAsync(pos1, pos2, cancellationToken);
            }
        }

        /// 播放道具交换动画
        private async UniTask SwapGameBoardItemsAsync(GridPosition position1, GridPosition position2, CancellationToken cancellationToken = default)
        {
            var gridSlot1 = GetGridSlot(position1);
            var gridSlot2 = GetGridSlot(position2);

            var animatedItemSwapperComponent = World.Instance.Root.GetComponent<Game_AnimatedItemSwapperComponent>();
            await animatedItemSwapperComponent.SwapItemsAsync(gridSlot1, gridSlot2, cancellationToken);
        }

        /// 是否成功消除
        private bool IsSolved(GridPosition position1, GridPosition position2, out SolvedData solvedData)
        {
            var solveComponent = World.Instance.Root.GetComponent<Game_SolveComponent>();
            solvedData = solveComponent.Swap(position1, position2);

            // 至少要有一个可被消除的序列
            return solvedData.SolvedSequences.Count > 0;
        }

        /// 能否向上移动
        public bool CanMoveUp(Game_SlotComponent gridSlotComponent, out GridPosition gridPosition)
        {
            // 如果该位置上方一个位置为空（被消除了），那么就可以上移
            var topGridSlot = GetSideGridSlot(gridSlotComponent, GridPosition.Up);
            if (topGridSlot is { HasItem: false })
            {
                gridPosition = topGridSlot.GridPosition;
                return true;
            }

            gridPosition = GridPosition.Zero;
            return false;
        }

        /// 获取某格子相邻的某方向的临格
        private Game_SlotComponent GetSideGridSlot(Game_SlotComponent gridSlotComponent, GridPosition direction)
        {
            var sideGridSlotPosition = gridSlotComponent.GridPosition + direction;

            return IsPositionOnGrid(sideGridSlotPosition)
                ? GetGridSlot(sideGridSlotPosition)
                : null;
        }

        /// 序列消除时
        private void NotifySequencesSolved(SolvedData solvedData)
        {
            // 每个序列计分
            // foreach (var sequencesConsumer in _solvedSequencesConsumers)
            // {
            //     sequencesConsumer.OnSequencesSolved(solvedData);
            // }

            // 检查有没有在这次消除过后，完成的游戏目标
            // foreach (var levelGoal in _levelGoals)
            // {
            //     // 目标未完成
            //     if (!levelGoal.IsAchieved)
            //     {
            //         levelGoal.OnSequencesSolved(solvedData);
            //     }
            // }
        }
    }
}