namespace Match3
{
    /// 横向检查器
    public class GameHorizontalLinearDetectComponent : Component, ILinearDetect
    {
        // 横向需要检查左右两个方向
        private readonly Grid[] _lineDirections = { Grid.Left, Grid.Right };

        public GameHorizontalLinearDetectComponent()
        {
            AddComponent(new Game_LineDetectComponent());
        }

        public bool GetSequence(Grid grid, out LinearSolveSequenceData sequenceData)
        {
            return GetComponent<Game_LineDetectComponent>().GetSequenceByDirection(grid, _lineDirections, out sequenceData);
        }
    }
}