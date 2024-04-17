namespace Match3
{
    public interface ILineDetect
    {
        /// 获取序列（横向 纵向）
        ItemSequence GetSequence(GridPosition gridPosition);
    }
}