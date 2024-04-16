using System.Collections.Generic;

namespace Match3
{
    public interface ISpecialItemDetector
    {
        List<UnityGridSlot> GetSpecialItemGridSlots(GameBoard gameBoard, UnityGridSlot gridSlot);
    }
}