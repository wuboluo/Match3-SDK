namespace Match3
{
    /// 纵向检查器
    public class Game_VerticalLineDetectComponent : Component, ILineDetect
    {
        // 横向需要检查上下两个方向
        private readonly GridPosition[] _lineDirections = { GridPosition.Up, GridPosition.Down };

        public Game_VerticalLineDetectComponent()
        {
            AddComponent(new Game_LineDetectComponent());
        }

        public ItemSequence GetSequence(GameBoard gameBoard, GridPosition gridPosition)
        {
            return GetComponent<Game_LineDetectComponent>().GetSequenceByDirection(gameBoard, gridPosition, _lineDirections);
        }
    }
}