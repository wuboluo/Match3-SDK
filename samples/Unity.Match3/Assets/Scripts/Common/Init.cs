using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Match3
{
    public class Init : MonoBehaviour
    {
        private Game_BoardRenderComponent _boardRenderComponent;
        private Game_ItemGenerateComponent _itemGenerateComponent;

        private void Start()
        {
            _boardRenderComponent = World.Instance.Root.GetComponent<Game_BoardRenderComponent>();
            _itemGenerateComponent = World.Instance.Root.GetComponent<Game_ItemGenerateComponent>();
            
            StartGame();
        }

        private void StartGame()
        {
            // 绘制棋盘尺寸
            _boardRenderComponent.CreateGridTiles();
            
            // 在每个格子中创建道具
            var gridData = _boardRenderComponent.GetGridSlots();
            var rowCount = gridData.GetLength(0);
            var columnCount = gridData.GetLength(1);
            var itemsPoolCapacity = rowCount * columnCount + Mathf.Max(rowCount, columnCount) * 2;
            _itemGenerateComponent.CreateItems(itemsPoolCapacity);
            // todo 刚开始不能存在3连以上的

            // 填充棋盘
            var unityGame = World.Instance.GetUnityGame();
            unityGame.SetGameBoardFillStrategy(new RiseFillStrategy());
            unityGame.StartAsync().Forget();

            Debug.Log("游戏开始");
        }
    }
}