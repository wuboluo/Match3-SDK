/// 纵向检查器
public class VerticalLineDetector : LineDetector
{
    // 横向需要检查上下两个方向
    private readonly GridPosition[] _lineDirections = { GridPosition.Up, GridPosition.Down };

    public override ItemSequence GetSequence(GameBoard gameBoard, GridPosition gridPosition)
    {
        return GetSequenceByDirection(gameBoard, gridPosition, _lineDirections);
    }
}