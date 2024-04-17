using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Match3
{
    public class Game_BoardComponent : Component
    {
        private readonly int _rowCount;
        private readonly int _columnCount;
        private readonly float _tileSize;
        private readonly Vector3 _originPosition;

        private readonly UnityGridSlot[,] _gridSlots;
        private readonly IGridTile[,] _gridTiles;

        private AsyncLazy _swapItemsTask;

        public GameBoard GameBoard { get; }

        public Game_BoardComponent(int rowCount, int columnCount, float tileSize)
        {
            _rowCount = rowCount;
            _columnCount = columnCount;
            _tileSize = tileSize;
            _originPosition = GetOriginPosition(_rowCount, _columnCount);
            _gridTiles = new IGridTile[_rowCount, _columnCount];
            _gridSlots = new UnityGridSlot[_rowCount, _columnCount];

            GameBoard = new GameBoard();
        }

        public UnityGridSlot[,] GetGridSlots()
        {
            return _gridSlots;
        }

        /// 某个位置的棋子是否已启用
        private bool IsTileActive(GridPosition gridPosition)
        {
            return GetTileType(gridPosition) != TileType.Unavailable;
        }

        /// 鼠标是否在棋盘内，如果在就返回所在的格子位置
        public bool IsPointerOnBoard(Vector3 worldPointerPosition, out GridPosition gridPosition)
        {
            gridPosition = GetGridPositionByPointer(worldPointerPosition);
            return IsPositionOnBoard(gridPosition);
        }

        /// 鼠标是否在格子内 and 此格子可操作
        private bool IsPositionOnBoard(GridPosition gridPosition)
        {
            return IsPositionOnGrid(gridPosition) && IsTileActive(gridPosition);
        }

        /// 鼠标是否在格子内
        private bool IsPositionOnGrid(GridPosition gridPosition)
        {
            return GridMath.IsPositionOnGrid(gridPosition, _rowCount, _columnCount);
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
            return GameBoard[gridPosition].IsMovable;
        }

        /// 获取某个位置的棋子类型
        private TileType GetTileType(GridPosition gridPosition)
        {
            return (TileType)_gridTiles[gridPosition.RowIndex, gridPosition.ColumnIndex].TypeId;
        }

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

        /// 开始排列的原点位置，从左下角开始，一行一行以此排列
        /// 这样保证棋盘中心在 (0,0)点
        private Vector3 GetOriginPosition(int rowCount, int columnCount)
        {
            var offsetY = Mathf.Floor(rowCount / 2.0f) * _tileSize;
            var offsetX = Mathf.Floor(columnCount / 2.0f) * _tileSize;

            return new Vector3(-offsetX, -offsetY);
        }

        /// 开局的时候创建所有的棋子
        public void CreateGridTiles(TileType defaultTileType)
        {
            var _itemPoolComponent = World.Instance.Root.GetComponent<Game_ItemPoolComponent>();

            for (var rowIndex = 0; rowIndex < _rowCount; rowIndex++)
            for (var columnIndex = 0; columnIndex < _columnCount; columnIndex++)
            {
                var gridTile = _itemPoolComponent.FetchTile(rowIndex, columnIndex, defaultTileType);

                _gridTiles[rowIndex, columnIndex] = gridTile;
                _gridSlots[rowIndex, columnIndex] = new UnityGridSlot(gridTile, new GridPosition(rowIndex, columnIndex));
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
        public bool IsSameSlot(GridPosition downPosition, GridPosition slotPosition)
        {
            return downPosition.Equals(slotPosition);
        }

        /// 不在一条直线上（是斜对角）
        public bool IsDiagonalSlot(GridPosition downPosition, GridPosition slotPosition)
        {
            // 当前的格子和鼠标落下时的格子在上下左右任意方向是挨着的
            var isSideSlot = slotPosition.Equals(downPosition + GridPosition.Up) ||
                             slotPosition.Equals(downPosition + GridPosition.Down) ||
                             slotPosition.Equals(downPosition + GridPosition.Left) ||
                             slotPosition.Equals(downPosition + GridPosition.Right);

            return !isSideSlot;
        }

        /// 交换
        public UniTask SwapItemsAsync(GridPosition position1, GridPosition position2, CancellationToken cancellationToken = default)
        {
            if (_swapItemsTask?.Task.Status.IsCompleted() ?? true)
            {
                var _riseFillStrategyComponent = World.Instance.Root.GetComponent<Game_RiseFillStrategyComponent>();
                _swapItemsTask = SwapItemsAsync(_riseFillStrategyComponent, position1, position2, cancellationToken).ToAsyncLazy();
            }

            return _swapItemsTask.Task;
        }

        /// 交换
        private async UniTask SwapItemsAsync(Game_RiseFillStrategyComponent fillStrategyComponent, GridPosition pos1, GridPosition pos2, CancellationToken cancellationToken = default)
        {
            // 交换动画
            await SwapGameBoardItemsAsync(pos1, pos2, cancellationToken);

            // 成功消除
            if (IsSolved(pos1, pos2, out var solvedData))
            {
                // 序列消除时，计分等
                NotifySequencesSolved(solvedData);

                // 列消除Jobs
                var solveJobs = fillStrategyComponent.GetSolveJobs(GameBoard, solvedData);
                await World.Instance.Root.GetComponent<Game_JobComponent>().ExecuteJobsAsync(solveJobs, cancellationToken);
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
            var gridSlot1 = GameBoard[position1.RowIndex, position1.ColumnIndex];
            var gridSlot2 = GameBoard[position2.RowIndex, position2.ColumnIndex];

            var animatedItemSwapperComponent = World.Instance.Root.GetComponent<Game_AnimatedItemSwapperComponent>();
            await animatedItemSwapperComponent.SwapItemsAsync(gridSlot1, gridSlot2, cancellationToken);
        }

        // /// 执行Jobs
        // public UniTask ExecuteJobsAsync(IEnumerable<IJob> jobs, CancellationToken cancellationToken = default)
        // {
        //     return JobsExecutor.ExecuteJobsAsync(jobs, cancellationToken);
        // }

        /// 是否成功消除
        private bool IsSolved(GridPosition position1, GridPosition position2, out SolvedData solvedData)
        {
            var solveComponent = World.Instance.Root.GetComponent<Game_SolveComponent>();
            solvedData = solveComponent.Swap(GameBoard, position1, position2);

            // 至少要有一个可被消除的序列
            return solvedData.SolvedSequences.Count > 0;
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