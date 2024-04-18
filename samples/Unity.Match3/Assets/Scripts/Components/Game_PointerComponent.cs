using Cysharp.Threading.Tasks;

namespace Match3
{
    public class Game_PointerComponent : Component
    {
        private readonly Game_BoardComponent _boardComponent;
        private readonly Game_SwapComponent _swapComponent;
        private readonly CanvasInputSystem _inputSystem;

        private Grid _downGrid;
        private bool _isDrag;

        public Game_PointerComponent()
        {
            _boardComponent = World.Instance.Root.GetComponent<Game_BoardComponent>();
            _swapComponent = World.Instance.Root.GetComponent<Game_SwapComponent>();
            _inputSystem = World.Instance.inputSystem;

            _inputSystem.PointerDown += OnPointerDown;
            _inputSystem.PointerDrag += OnPointerDrag;
            _inputSystem.PointerUp += OnPointerUp;
        }

        private void OnPointerDown(object sender, PointerEventArgs pointer)
        {
            // 鼠标在棋盘内 and 鼠标所在的格子可移动交换
            if (_boardComponent.IsPointerOnBoard(pointer.WorldPosition, out _downGrid) &&
                _boardComponent.IsMovableSlot(_downGrid))
            {
                _isDrag = true;
            }
        }

        private void OnPointerDrag(object sender, PointerEventArgs pointer)
        {
            if (!_isDrag)
            {
                return;
            }

            // 鼠标不在棋盘内 or 鼠标所在的格子不可移动交换
            if (!_boardComponent.IsPointerOnBoard(pointer.WorldPosition, out var currentGrid) ||
                !_boardComponent.IsMovableSlot(currentGrid))
            {
                _isDrag = false;
                return;
            }

            // 鼠标当前所在的格子和点击时的是同一个 or 和点击时的格子不相邻
            if (_boardComponent.IsSameSlot(_downGrid, currentGrid) ||
                !_boardComponent.IsAdjacentSlot(_downGrid, currentGrid))
            {
                return;
            }

            _isDrag = false;

            // 鼠标当前所在的格子和点击时的格子交换
            _swapComponent.SwapItems(_downGrid, currentGrid).Forget();
        }

        private void OnPointerUp(object sender, PointerEventArgs pointer)
        {
        }
    }
}