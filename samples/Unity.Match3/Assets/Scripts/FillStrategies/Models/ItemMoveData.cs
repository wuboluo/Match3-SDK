using UnityEngine;

namespace Match3
{
    public class ItemMoveData
    {
        public ItemMoveData(UnityItem item, Vector3[] worldPositions)
        {
            Item = item;
            WorldPositions = worldPositions;
        }

        public UnityItem Item { get; }
        public Vector3[] WorldPositions { get; }
    }
}