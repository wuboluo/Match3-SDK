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

        private Queue<Game_ItemComponent> _itemsPool;

        public Game_ItemGenerateComponent(GameObject itemPrefab, Transform poolRoot, SpriteAtlas sprites)
        {
            _random = new System.Random();

            _itemPrefab = itemPrefab;
            _poolRoot = poolRoot;
            _sprites = sprites.GetSprites();
        }

        /// 棋盘生成时，创建所有的棋子道具
        public void CreateBoardItems(Game_SlotComponent[,] gridSlots)
        {
            var rowCount = gridSlots.GetLength(0);
            var columnCount = gridSlots.GetLength(1);

            // 容量
            var capacity = rowCount * columnCount + Mathf.Max(rowCount, columnCount) * 2;
            InitPool(capacity);
        }

        private void InitPool(int capacity)
        {
            _itemsPool ??= new Queue<Game_ItemComponent>(capacity);
            _itemsPool = new Queue<Game_ItemComponent>(capacity);

            for (var i = 0; i < capacity; i++)
            {
                _itemsPool.Enqueue(NewItem());
            }
        }

        private Game_ItemComponent NewItem()
        {
            var item = _itemPrefab.CreateNew<Game_ItemComponent>(parent: _poolRoot);
            item.Hide();

            return item;
        }

        /// 随机一个图案
        private Game_ItemComponent SetSprite(Game_ItemComponent itemComponent)
        {
            var index = _random.Next(0, _sprites.Length);
            itemComponent.SetSprite(index, _sprites[index]);

            return itemComponent;
        }

        public Game_ItemComponent FetchItem()
        {
            return SetSprite(_itemsPool.Count <= 0 ? NewItem() : _itemsPool.Dequeue());
        }

        public void RecycleItem(Game_ItemComponent itemComponent)
        {
            _itemsPool.Enqueue(itemComponent);
        }
    }
}