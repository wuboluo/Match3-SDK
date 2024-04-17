namespace Match3
{
    public class GameCore
    {
        private readonly CanvasInputSystem _inputSystem;

        private readonly Game_BoardComponent _boardComponent;
        private readonly Game_RiseFillStrategyComponent _riseFillStrategyComponent;
        private readonly Game_AnimatedItemSwapperComponent _animatedItemSwapperComponent;

        private bool _isDrag;
        private GridPosition _slotDownPosition;

        public GameCore()
        {
            _inputSystem = World.Instance.inputSystem;
            _boardComponent = World.Instance.Root.GetComponent<Game_BoardComponent>();
        }
    }
}