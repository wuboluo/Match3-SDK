using System;
using System.Runtime.CompilerServices;

public class UnityGridSlot
{
    public UnityGridSlot(IGridSlotState state, GridPosition gridPosition)
    {
        State = state;
        GridPosition = gridPosition;
    }

    public int ItemId => Item.ContentId;

    public bool HasItem => Item != null;
    public bool IsMovable => !State.IsLocked && HasItem;
    public bool CanContainItem => State.CanContainItem;
    public bool CanSetItem => State.CanContainItem && !HasItem;
    public bool NotAvailable => !State.CanContainItem || State.IsLocked;

    public UnityItem Item { get; private set; }
    public IGridSlotState State { get; private set; }
    public GridPosition GridPosition { get; }

    public void SetState(IGridSlotState state)
    {
        State = state;
    }

    public void SetItem(UnityItem item)
    {
        EnsureItemIsNotNull(item);

        Item = item;
    }

    public void Clear()
    {
        if (!State.CanContainItem) throw new InvalidOperationException("Can not clear an unavailable grid slot.");

        Item = default;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void EnsureItemIsNotNull(UnityItem item)
    {
        if (item == null) throw new NullReferenceException(nameof(item));
    }
}