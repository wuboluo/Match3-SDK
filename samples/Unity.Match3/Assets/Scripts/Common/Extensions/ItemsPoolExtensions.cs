using System.Collections.Generic;

public static class ItemsPoolExtensions
{
    public static void ReturnAllItems(this UnityItemGenerator itemsPool, IEnumerable<UnityGridSlot> gridSlots)
    {
        foreach (var gridSlot in gridSlots)
        {
            if (gridSlot.Item == null) continue;

            itemsPool.RecycleItem(gridSlot.Item);
            gridSlot.Item.Hide();
            gridSlot.Clear();
        }
    }
}