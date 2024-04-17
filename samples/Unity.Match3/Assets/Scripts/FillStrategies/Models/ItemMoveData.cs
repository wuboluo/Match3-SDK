using UnityEngine;

namespace Match3
{
    public class ItemMoveData
    {
        public ItemMoveData(Game_ItemComponent itemComponent, Vector3[] worldPositions)
        {
            ItemComponent = itemComponent;
            WorldPositions = worldPositions;
        }

        public Game_ItemComponent ItemComponent { get; }
        public Vector3[] WorldPositions { get; }
    }
}