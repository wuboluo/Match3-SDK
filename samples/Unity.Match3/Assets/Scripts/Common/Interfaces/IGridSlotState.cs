namespace Match3
{
    public interface IGridSlotState
    {
        // 格子类型
        int TypeId { get; }

        // 格子是否被锁定
        bool IsLocked { get; }

        // 格子是否包含棋子 // todo
        bool CanContainItem { get; }
    }
}