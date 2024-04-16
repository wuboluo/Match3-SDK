namespace Match3
{
    /// 序列检测器
    public interface ISequenceDetector
    {
        /// 获取序列
        ItemSequence GetSequence(GameBoard gameBoard, GridPosition gridPosition);
    }
}