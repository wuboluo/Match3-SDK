using Cysharp.Threading.Tasks;

namespace Match3
{
    public class Game_PointerActionComponent : Component
    {
        private readonly Game_BoardComponent _boardComponent;
        private readonly CanvasInputSystem _inputSystem;

        private GridPosition _downPosition;
        private bool _isDrag;

        public Game_PointerActionComponent()
        {
            _boardComponent = World.Instance.Root.GetComponent<Game_BoardComponent>();
            _inputSystem = World.Instance.inputSystem;

            _inputSystem.PointerDown += OnPointerDown;
            _inputSystem.PointerDrag += OnPointerDrag;
        }

        private void OnPointerDown(object sender, PointerEventArgs pointer)
        {
            // 鼠标落在的位置的格子是否可移动
            if (_boardComponent.IsPointerOnBoard(pointer.WorldPosition, out _downPosition)
                && _boardComponent.IsMovableSlot(_downPosition))
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

            // 出界了 or 该棋子不可移动
            if (!_boardComponent.IsPointerOnBoard(pointer.WorldPosition, out var slotPosition)
                || !_boardComponent.IsMovableSlot(slotPosition))
            {
                _isDrag = false;
                return;
            }

            // 是同一个格子 or 不在一条直线上
            if (_boardComponent.IsSameSlot(_downPosition, slotPosition) 
                || _boardComponent.IsDiagonalSlot(_downPosition, slotPosition))
            {
                return;
            }

            _isDrag = false;

            // 鼠标落下的位置和当前位置交换 todo 是不是要在up的时候交换
            _boardComponent.SwapItemsAsync(_downPosition, slotPosition).Forget();
        }
    }
}