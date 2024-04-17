using System.Collections.Generic;

namespace Match3
{
    public static class ItemsPoolExtensions
    {
        public static void ReturnAllItems(this Game_ItemGenerateComponent itemsPool, IEnumerable<Game_SlotComponent> gridSlots)
        {
            foreach (var gridSlot in gridSlots)
            {
                if (gridSlot.ItemComponent == null) continue;

                itemsPool.RecycleItem(gridSlot.ItemComponent);
                gridSlot.ItemComponent.Hide();
                gridSlot.Dispose();
            }
        }
    }
}