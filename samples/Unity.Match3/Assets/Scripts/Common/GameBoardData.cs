using UnityEngine;
using UnityEngine.U2D;

namespace Match3
{
    public class GameBoardData : MonoBehaviour
    {
        public int rowCount = 5;
        public int columnCount = 7;
        public float tileSize = 0.6f;

        [Space]
        public GameObject itemPrefab;
        public SpriteAtlas artAtlas;
        public Transform itemsPool;
        
        [Space]
        public TileModel[] tileModels;
    }
}