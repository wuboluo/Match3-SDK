using UnityEngine;

namespace Match3
{
    public class ItemMovePathData
    {
        public ItemMovePathData(Game_ItemComponent itemComponent, Vector3[] paths)
        {
            ItemComponent = itemComponent;
            Paths = paths;
        }

        public Game_ItemComponent ItemComponent { get; }
        public Vector3[] Paths { get; }
    }
}