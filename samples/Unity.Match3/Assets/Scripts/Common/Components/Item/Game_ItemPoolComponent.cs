using System.Collections.Generic;
using UnityEngine;

namespace Match3
{
    public class Game_ItemPoolComponent : Component
    {
        // tile gameObject 父节点
        private readonly Transform _itemsParent;

        // 每种tile的池
        private readonly Dictionary<TileType, Queue<IGridTile>> _itemsPool;

        // 每种tile的prefab
        private readonly Dictionary<TileType, GameObject> _tilePrefabs;

        public Game_ItemPoolComponent(TileModel[] tiles, Transform itemsParent)
        {
            _itemsParent = itemsParent;
            _itemsPool = new Dictionary<TileType, Queue<IGridTile>>(tiles.Length);
            _tilePrefabs = new Dictionary<TileType, GameObject>(tiles.Length);

            // 创建所有tile类型的池
            foreach (var tile in tiles)
            {
                _tilePrefabs.Add(tile.Type, tile.Prefab);
                _itemsPool.Add(tile.Type, new Queue<IGridTile>());
            }
        }

        // 从池中拿一个指定类型的 tile gameObject
        public IGridTile FetchTile(TileType tileType)
        {
            var pool = _itemsPool[tileType];
            var gridTile = pool.Count == 0 ? NewTile(_tilePrefabs[tileType]) : pool.Dequeue();
            gridTile.SetActive(true);

            return gridTile;
        }

        // 回收一个 tile 回池中
        public void RecycleTile(IGridTile gridTile)
        {
            if (gridTile is IStatefulSlot statefulSlot) statefulSlot.ResetState();

            gridTile.SetActive(false);
            _itemsPool[(TileType)gridTile.TypeId].Enqueue(gridTile);
        }

        // 创建一个新的 tile gameObject
        private IGridTile NewTile(GameObject tilePrefab)
        {
            return tilePrefab.CreateNew<IGridTile>(parent: _itemsParent);
        }
    }
}