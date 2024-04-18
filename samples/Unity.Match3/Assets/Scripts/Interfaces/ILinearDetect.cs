namespace Match3
{
    public interface ILinearDetect
    {
        /// 获取序列（横向 纵向）
        bool GetSequence(Grid grid, out LinearSolveSequenceData sequenceData);
    }
}