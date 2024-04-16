/// 横向检查器
public class HorizontalLineDetector : LineDetector
{
    // 横向需要检查左右两个方向
    private readonly GridPosition[] _lineDirections = { GridPosition.Left, GridPosition.Right };

    public override ItemSequence GetSequence(GameBoard gameBoard, GridPosition gridPosition)
    {
        return GetSequenceByDirection(gameBoard, gridPosition, _lineDirections);
    }
}