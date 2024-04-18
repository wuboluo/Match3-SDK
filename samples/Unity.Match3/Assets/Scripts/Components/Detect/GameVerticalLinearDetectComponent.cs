namespace Match3
{
    /// 纵向检查器
    public class GameVerticalLinearDetectComponent : Component, ILinearDetect
    {
        // 横向需要检查上下两个方向
        private readonly Grid[] _lineDirections = { Grid.Up, Grid.Down };

        public GameVerticalLinearDetectComponent()
        {
            AddComponent(new Game_LineDetectComponent());
        }

        public bool GetSequence(Grid grid, out LinearSolveSequenceData sequenceData)
        {
            return GetComponent<Game_LineDetectComponent>().GetSequenceByDirection(grid, _lineDirections, out sequenceData);
        }
    }
}