using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using Random = System.Random;

namespace Match3
{
    public sealed class Game_ItemGenerateComponent : Component
    {
        private Random _random;

        private GameObject _itemPrefab;
        private Transform _container;
        private Sprite[] _sprites;

        private Queue<UnityItem> _itemsPool;

        public void Init(GameObject itemPrefab, Transform container, SpriteAtlas sprites)
        {
            _random = new Random();

            _itemPrefab = itemPrefab;
            _container = container;
            _sprites = sprites.GetSprites();
        }

        public UnityItem CreateItem()
        {
            var item = _itemPrefab.CreateNew<UnityItem>(parent: _container);
            item.Hide();

            return item;
        }

        private UnityItem ConfigureItem(UnityItem item)
        {
            var index = _random.Next(0, _sprites.Length);
            item.SetSprite(index, _sprites[index]);

            return item;
        }

        public void CreateItems(int capacity)
        {
            if (_itemsPool != null)
            {
                throw new InvalidOperationException("Items have already been created.");
            }

            _itemsPool = new Queue<UnityItem>(capacity);

            for (var i = 0; i < capacity; i++)
            {
                _itemsPool.Enqueue(CreateItem());
            }
        }

        public UnityItem FetchItem()
        {
            return ConfigureItem(_itemsPool.Dequeue());
        }

        public void RecycleItem(UnityItem item)
        {
            _itemsPool.Enqueue(item);
        }
    }
}