using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Match3
{
    public class UnityGame : Match3Game
    {
        private readonly Game_BoardRenderComponent _boardRenderComponent;
        private readonly CanvasInputSystem _inputSystem;

        private bool _isDragMode;
        private GridPosition _slotDownPosition;

        public UnityGame(CanvasInputSystem inputSystem, GameConfig config) : base(config)
        {
            _inputSystem = inputSystem;
            _boardRenderComponent = World.Instance.Root.GetComponent<Game_BoardRenderComponent>();
        }

        protected override void OnGameStarted()
        {
            _inputSystem.PointerDown += OnPointerDown;
            _inputSystem.PointerDrag += OnPointerDrag;
        }

        protected override void OnGameStopped()
        {
            _inputSystem.PointerDown -= OnPointerDown;
            _inputSystem.PointerDrag -= OnPointerDrag;
        }

        public IEnumerable<UnityGridSlot> GetGridSlots()
        {
            for (var rowIndex = 0; rowIndex < GameBoard.RowCount; rowIndex++)
            for (var columnIndex = 0; columnIndex < GameBoard.ColumnCount; columnIndex++)
                yield return GameBoard[rowIndex, columnIndex];
        }

        private void OnPointerDown(object sender, PointerEventArgs pointer)
        {
            // 鼠标落在的位置的格子是否可移动
            if (IsPointerOnBoard(pointer.WorldPosition, out _slotDownPosition) && IsMovableSlot(_slotDownPosition)) _isDragMode = true;
        }

        private void OnPointerDrag(object sender, PointerEventArgs pointer)
        {
            if (_isDragMode == false) return;

            // 出界了 or 该棋子不可移动
            if (!IsPointerOnBoard(pointer.WorldPosition, out var slotPosition) || !IsMovableSlot(slotPosition))
            {
                _isDragMode = false;
                return;
            }

            // 是同一个格子 or 不在一条直线上
            if (IsSameSlot(slotPosition) || IsDiagonalSlot(slotPosition)) return;

            _isDragMode = false;

            // 鼠标落下的位置和当前位置交换
            // todo 是不是要在up的时候交换
            SwapItemsAsync(_slotDownPosition, slotPosition).Forget();
        }

        /// 在棋盘内
        private bool IsPointerOnBoard(Vector3 pointerWorldPosition, out GridPosition slotDownPosition)
        {
            return _boardRenderComponent.IsPointerOnBoard(pointerWorldPosition, out slotDownPosition);
        }

        /// 可被交换的
        private bool IsMovableSlot(GridPosition gridPosition)
        {
            return GameBoard[gridPosition].IsMovable;
        }

        /// 同一个
        private bool IsSameSlot(GridPosition slotPosition)
        {
            return _slotDownPosition.Equals(slotPosition);
        }

        /// 不在一条直线上
        private bool IsDiagonalSlot(GridPosition slotPosition)
        {
            // 当前的格子和鼠标落下时的格子在上下左右任意方向是挨着的
            var isSideSlot = slotPosition.Equals(_slotDownPosition + GridPosition.Up) ||
                             slotPosition.Equals(_slotDownPosition + GridPosition.Down) ||
                             slotPosition.Equals(_slotDownPosition + GridPosition.Left) ||
                             slotPosition.Equals(_slotDownPosition + GridPosition.Right);

            return isSideSlot == false;
        }
    }
}