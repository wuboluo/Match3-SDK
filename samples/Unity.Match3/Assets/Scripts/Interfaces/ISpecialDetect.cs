using System.Collections.Generic;

namespace Match3
{
    public interface ISpecialDetect
    {
        List<Game_SlotComponent> GetSpecialItemGridSlots(Game_SlotComponent gridSlotComponent);
    }
}