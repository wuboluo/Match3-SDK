using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Game_ItemPoolComponent : Component
    {
        private readonly Transform _itemsParent;
        private readonly GameObject _itemPrefab;
        private readonly Queue<Game_ItemComponent> _itemPool;

        public Game_ItemPoolComponent(GameObject itemPrefab, Transform itemsParent)
        {
            _itemsParent = itemsParent;
            _itemPrefab = itemPrefab;
            _itemPool = new Queue<Game_ItemComponent>();
            InitPool();
        }

        private void InitPool()
        {
            var rowCount = World.Instance.data.rowCount;
            var columnCount = World.Instance.data.columnCount;

            // 容量
            var capacity = rowCount * columnCount + Mathf.Max(rowCount, columnCount) * 2;
            for (var i = 0; i < capacity; i++)
            {
                _itemPool.Enqueue(New());
            }
        }

        public Game_ItemComponent FetchTileItem()
        {
            var gridTile = _itemPool.Count == 0 ? New() : _itemPool.Dequeue();
            gridTile.Show();

            return gridTile;
        }

        // 回收一个 tile 回池中
        public void RecycleTileItem(Game_ItemComponent itemComponent)
        {
            itemComponent.Hide();
            _itemPool.Enqueue(itemComponent);
        }

        private Game_ItemComponent New()
        {
            var itemComponent = _itemPrefab.CreateNew<Game_ItemComponent>(parent: _itemsParent);
            itemComponent.Hide();
 
            return itemComponent;
        }
    }
}