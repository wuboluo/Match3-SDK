using UnityEngine;

namespace Match3
{
    public class Init : MonoBehaviour
    {
        private Game_BoardComponent _boardComponent;
        private Game_ItemGenerateComponent _itemGenerateComponent;

        private void Start()
        {
            _boardComponent = World.Instance.Root.GetComponent<Game_BoardComponent>();
            _itemGenerateComponent = World.Instance.Root.GetComponent<Game_ItemGenerateComponent>();

            StartGame();
        }

        private void StartGame()
        {
            // 绘制棋盘尺寸
            _boardComponent.CreateGridTiles(TileType.Available);
            
            // 初始化道具池，创建道具
            _itemGenerateComponent.CreateBoardItems(_boardComponent.GetGridSlots());

            // 填充棋盘
            // var unityGame = World.Instance.GetUnityGame();
            // unityGame.SetGameBoardFillStrategy(new Game_RiseFillStrategyComponent());

            // unityGame.StartAsync().Forget();
            Debug.Log("游戏开始");
        }
    }
}