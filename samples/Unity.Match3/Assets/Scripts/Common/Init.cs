using UnityEngine;
using UnityEngine.UI;

namespace Match3
{
    public class Init : MonoBehaviour
    {
        private Game_BoardComponent _boardComponent;
        private Game_ItemGenerateComponent _itemGenerateComponent;
        private Game_RiseFillStrategyComponent _riseFillStrategyComponent;
        private Game_JobComponent _jobComponent;

        public Button startBtn;

        private void Start()
        {
            _boardComponent = World.Instance.Root.GetComponent<Game_BoardComponent>();
            _itemGenerateComponent = World.Instance.Root.GetComponent<Game_ItemGenerateComponent>();
            _riseFillStrategyComponent = World.Instance.Root.GetComponent<Game_RiseFillStrategyComponent>();
            _jobComponent = World.Instance.Root.GetComponent<Game_JobComponent>();

            startBtn.onClick.AddListener(StartGame);
        }

        private void StartGame()
        {
            // 绘制棋盘尺寸
            _boardComponent.CreateGridTiles();

            // 初始化道具池，创建道具
            _itemGenerateComponent.CreateBoardItems(_boardComponent.GetGridSlots());

            // 填充棋盘
            var boardFillJobs = _riseFillStrategyComponent.GetBoardFillJobs();
            _jobComponent.Execute(boardFillJobs);

            Debug.Log("游戏开始");
        }
    }
}