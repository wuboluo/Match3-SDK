namespace Match3.Infrastructure.Interfaces
{
    public interface IItemsPool<TItem>
    {
        TItem FetchItem();
        void RecycleItem(TItem item);
    }
}