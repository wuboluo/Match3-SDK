namespace Match3
{
    /// 横向检查器
    public class Game_HorizontalLineDetectComponent : Component, ILineDetect
    {
        // 横向需要检查左右两个方向
        private readonly GridPosition[] _lineDirections = { GridPosition.Left, GridPosition.Right };

        public Game_HorizontalLineDetectComponent()
        {
            AddComponent(new Game_LineDetectComponent());
        }

        public ItemSequence GetSequence(GridPosition gridPosition)
        {
            return GetComponent<Game_LineDetectComponent>().GetSequenceByDirection(gridPosition, _lineDirections);
        }
    }
}