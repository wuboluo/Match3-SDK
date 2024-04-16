namespace Match3
{
    public interface IStatefulSlot
    {
        bool NextState();
        void ResetState();
    }
}