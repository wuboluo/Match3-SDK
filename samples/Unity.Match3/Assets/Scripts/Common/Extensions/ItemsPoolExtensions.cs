using System.Collections.Generic;

namespace Match3
{
    public static class ItemsPoolExtensions
    {
        public static void ReturnAllItems(this Game_ItemGenerateComponent itemsPool, IEnumerable<UnityGridSlot> gridSlots)
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
}