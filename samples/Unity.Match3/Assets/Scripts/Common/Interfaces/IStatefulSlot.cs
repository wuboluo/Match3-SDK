public interface IStatefulSlot
{
    bool NextState();
    void ResetState();
}