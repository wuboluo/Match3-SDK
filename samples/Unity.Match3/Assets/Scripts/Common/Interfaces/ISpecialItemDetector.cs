using System.Collections.Generic;

public interface ISpecialItemDetector
{
    List<UnityGridSlot> GetSpecialItemGridSlots(GameBoard gameBoard, UnityGridSlot gridSlot);
}