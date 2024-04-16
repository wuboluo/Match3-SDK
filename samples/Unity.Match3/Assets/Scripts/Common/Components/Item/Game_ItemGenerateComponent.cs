using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Match3
{
    public sealed class Game_ItemGenerateComponent : Component
    {
        private readonly System.Random _random;

        private readonly GameObject _itemPrefab;
        private readonly Transform _poolRoot;
        private readonly Sprite[] _sprites;

        private Queue<UnityItem> _itemsPool;

        public Game_ItemGenerateComponent(GameObject itemPrefab, Transform poolRoot, SpriteAtlas sprites)
        {
            _random = new System.Random();

            _itemPrefab = itemPrefab;
            _poolRoot = poolRoot;
            _sprites = sprites.GetSprites();
        }

        /// 棋盘生成时，创建所有的棋子道具
        public void CreateBoardItems(UnityGridSlot[,] gridSlots)
        {
            var rowCount = gridSlots.GetLength(0);
            var columnCount = gridSlots.GetLength(1);

            // 容量
            var capacity = rowCount * columnCount + Mathf.Max(rowCount, columnCount) * 2;
            InitPool(capacity);
        }

        private void InitPool(int capacity)
        {
            _itemsPool ??= new Queue<UnityItem>(capacity);
            _itemsPool = new Queue<UnityItem>(capacity);

            for (var i = 0; i < capacity; i++)
            {
                _itemsPool.Enqueue(NewItem());
            }
        }

        private UnityItem NewItem()
        {
            var item = _itemPrefab.CreateNew<UnityItem>(parent: _poolRoot);
            item.Hide();

            return item;
        }

        /// 随机一个图案
        private UnityItem SetSprite(UnityItem item)
        {
            var index = _random.Next(0, _sprites.Length);
            item.SetSprite(index, _sprites[index]);

            return item;
        }

        public UnityItem FetchItem()
        {
            return SetSprite(_itemsPool.Dequeue());
        }

        public void RecycleItem(UnityItem item)
        {
            _itemsPool.Enqueue(item);
        }
    }
}