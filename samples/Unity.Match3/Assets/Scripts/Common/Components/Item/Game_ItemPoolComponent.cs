using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Game_ItemPoolComponent : Component
    {
        // tile gameObject 父节点
        private readonly Transform _itemsParent;

        // 每种tile的池
        private readonly Queue<Game_ItemComponent> _itemsPool;
        // private readonly Dictionary<TileType, Queue<IGridTile>> _itemsPool;

        // 每种tile的prefab
        private readonly GameObject _tilePrefab;
        // private readonly Dictionary<TileType, GameObject> _tilePrefabs;

        public Game_ItemPoolComponent(GameObject itemPrefab, Transform itemsParent)
        {
            _itemsParent = itemsParent;
            _itemsPool = new Queue<Game_ItemComponent>();
            _tilePrefab = itemPrefab;

            // _itemsPool = new Dictionary<TileType, Queue<IGridTile>>(tiles.Length);
            // _tilePrefabs = new Dictionary<TileType, GameObject>(tiles.Length);

            // 创建所有tile类型的池
            // foreach (var tile in tiles)
            // {
            //     // _tilePrefabs.Add(tile.Type, tile.Prefab);
            //     _itemsPool.Add(tile.Type, new Queue<IGridTile>());
            // }
        }

        // 从池中拿一个指定类型的 tile gameObject
        private Game_ItemComponent FetchTile()
        {
            var gridTile = _itemsPool.Count == 0 ? NewTile() : _itemsPool.Dequeue();
            gridTile.Show();

            return gridTile;
        }

        public Game_ItemComponent FetchTile(int rowIndex, int columnIndex)
        {
            var itemPoolComponent = World.Instance.Root.GetComponent<Game_ItemPoolComponent>();
            var tileItem = itemPoolComponent.FetchTile();
            var pos = World.Instance.Root.GetComponent<Game_BoardComponent>().GetWorldPosition(rowIndex, columnIndex);

            tileItem.SetWorldPosition(pos);

            return tileItem;
        }

        // 回收一个 tile 回池中
        public void RecycleTile(Game_ItemComponent tileItem)
        {
            // if (gridTile is IStatefulSlot statefulSlot)
            // {
            //     statefulSlot.ResetState();
            // }

            tileItem.Hide();
            _itemsPool.Enqueue(tileItem);
        }

        // 创建一个新的 tile gameObject
        private Game_ItemComponent NewTile()
        {
            return _tilePrefab.CreateNew<Game_ItemComponent>(parent: _itemsParent);
        }
    }
}